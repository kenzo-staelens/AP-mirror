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

        public void Generate(object sender, RoutedEventArgs e) {
            try {
                MazeGeneratorFactory factory = new MazeGeneratorFactory();
                IMazeGenerator gen = factory.Create(MazeGeneratorTypes.Static);
                Maze maze = gen.Generate();
                Render(maze);
            }catch(Exception ex) {
                ErrorLabel.Content = ex.ToString();
                ErrorLabel.Visibility = System.Windows.Visibility.Visible;
            }
        }

        public Line buildLine(int x1, int x2, int y1, int y2, int thickness) {
            Line line = new Line();
            SolidColorBrush brush = new SolidColorBrush();
            brush.Color = Color.FromRgb(0, 0, 0);
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
            SolidColorBrush brush = new SolidColorBrush();
            brush.Color = Color.FromRgb(0, 0, 0);
            for (int i = 0; i < maze.Width; i++) {
                for(int j = 0; j < maze.Height; j++) {
                    Cell cell = maze.maze[j, i];
                    WallDataComponent? wdc = (WallDataComponent?)cell.GetComponent(typeof(WallDataComponent));
                    if (wdc == null) continue;//cannot render
                    //horizontal lines
                    Line line = buildLine(line_length_width * i, line_length_width * (i + 1),
                        line_length_height * j, line_length_height * j,
                        ((WallDataComponent)wdc).Width);
                    Line line2 = buildLine(line_length_width * i, line_length_width * (i + 1),
                        line_length_height * (j + 1), line_length_height * (j + 1),
                        ((WallDataComponent)wdc).Width);
                    //vertical lines
                    Line line3 = buildLine(line_length_width * i, line_length_width * i,
                        line_length_height * j, line_length_height * (j + 1),
                        ((WallDataComponent)wdc).Width);
                    Line line4 = buildLine(line_length_width * (i+1), line_length_width * (i + 1),
                        line_length_height * j, line_length_height * (j + 1),
                        ((WallDataComponent)wdc).Width);

                    if (cell.walls[0]) DrawableCanvas.Children.Add(line);
                    if (cell.walls[2]) DrawableCanvas.Children.Add(line2);
                    if (cell.walls[3]) DrawableCanvas.Children.Add(line3);
                    if (cell.walls[1]) DrawableCanvas.Children.Add(line4);
                    ErrorLabel.Content = String.Join(", ", cell.walls);
                    ErrorLabel.Visibility = Visibility.Visible;
                    //return;

                }
            }
            //testing with lines
            /*Line myLine = new Line();
            SolidColorBrush mySolidColorBrush = new SolidColorBrush();
            mySolidColorBrush.Color = Color.FromRgb(0, 0, 0);
            myLine.Stroke = mySolidColorBrush;
            myLine.X1 = 1;
            myLine.X2 = 50;
            myLine.Y1 = 1;
            myLine.Y2 = 1;
            myLine.HorizontalAlignment = HorizontalAlignment.Left;
            myLine.VerticalAlignment = VerticalAlignment.Center;
            myLine.StrokeThickness = 2;
            DrawableCanvas.Children.Add(myLine);*/
        }
    }
}
