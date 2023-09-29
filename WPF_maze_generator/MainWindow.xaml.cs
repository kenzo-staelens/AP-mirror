using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Globals;
using Generators;
using Components;
using ExtensionMethods;

namespace WPF_maze_generator {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
            Generator.ItemsSource = Enum.GetValues(typeof(MazeGeneratorTypes));
            Generator.SelectedIndex = 0;
            Generate(null, null);
        }

        public void GeneratorChanged(object sender, RoutedEventArgs e) {
            if ((MazeGeneratorTypes)Generator.SelectedItem != MazeGeneratorTypes.Static) {
                WidthTextBox.IsEnabled = true;
                HeightTextBox.IsEnabled = true;
                FilenameTextBox.IsEnabled = false;
            }
            else {
                WidthTextBox.IsEnabled = false;
                HeightTextBox.IsEnabled = false;
                FilenameTextBox.IsEnabled = true;
            }
        }

        public void Generate(object? sender, RoutedEventArgs? e) {
            try {
                MazeGeneratorFactory factory = new();
                MazeConstructionComponent constuctionData = new(Int32.Parse(WidthTextBox.Text),
                    Int32.Parse(HeightTextBox.Text), $"./{FilenameTextBox.Text}");
                IMazeGenerator gen = factory.Create((MazeGeneratorTypes)Generator.SelectedItem, constuctionData);
                Maze maze = gen.Generate();
                DrawableCanvas.Children.Clear();
                Render(maze);
            }catch(Exception ex) {
                ErrorLabel.Content = ex.ToString();
                ErrorLabel.Visibility = Visibility.Visible;
            }
        }

        private static Line BuildLine(int x1, int x2, int y1, int y2, int thickness) {
            Line line = new();
            SolidColorBrush brush = new() {
                Color = Color.FromRgb(0, 0, 0)
            };
            line.Stroke = brush;
            line.X1 = x1;
            line.X2 = x2;
            line.Y1 = y1;
            line.Y2 = y2;
            line.StrokeThickness = thickness;
            if (x1 == x2) {
                line.HorizontalAlignment = HorizontalAlignment.Center;
                line.VerticalAlignment = VerticalAlignment.Top;
            }
            else if (y1 == y2) {
                line.HorizontalAlignment = HorizontalAlignment.Left;
                line.VerticalAlignment = VerticalAlignment.Center;
            }
            return line;
        }

        public void Render(Maze maze) {
            // pixels / #cells
            int line_length_width = (int)(DrawableCanvas.Width / maze.Width);
            int line_length_height = (int)(DrawableCanvas.Height / maze.Height);
            for (int i = 0; i < maze.Width; i++) {
                for(int j = 0; j < maze.Height; j++) {
                    Cell cell = maze.maze[i, j];
                    WallDataComponent? wdc = (WallDataComponent?)cell.GetComponent(typeof(WallDataComponent));
                    if (wdc == null) continue;//cannot render
                    //horizontal lines
                    Line line = BuildLine(line_length_width * i, line_length_width * (i + 1),
                        line_length_height * j, line_length_height * j,
                        ((WallDataComponent)wdc).Width);
                    Line line2 = BuildLine(line_length_width * i, line_length_width * (i + 1),
                        line_length_height * (j + 1), line_length_height * (j + 1),
                        ((WallDataComponent)wdc).Width);
                    //vertical lines
                    Line line3 = BuildLine(line_length_width * i, line_length_width * i,
                        line_length_height * j, line_length_height * (j + 1),
                        ((WallDataComponent)wdc).Width);
                    Line line4 = BuildLine(line_length_width * (i+1), line_length_width * (i + 1),
                        line_length_height * j, line_length_height * (j + 1),
                        ((WallDataComponent)wdc).Width);

                    if (cell.walls[0]) DrawableCanvas.Children.Add(line);
                    if (cell.walls[2]) DrawableCanvas.Children.Add(line2);
                    if (cell.walls[3]) DrawableCanvas.Children.Add(line3);
                    if (cell.walls[1]) DrawableCanvas.Children.Add(line4);
                }
            }
        }
    }
}
