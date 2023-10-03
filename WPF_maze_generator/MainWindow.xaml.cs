using Components;
using Generators;
using Globals;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using ExtensionMethods;
using System.Threading.Tasks;

namespace WPF_maze_generator {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private readonly MazeGeneratorFactory factory;
        public MainWindow() {
            InitializeComponent();
            Generator.ItemsSource = Enum.GetValues(typeof(MazeGeneratorTypes));
            Generator.SelectedIndex = 0;
            try {
                this.factory = new(new IComponent[] { new WallDataComponent(2) });
                Generate(null, null);
            }
            catch (Exception ex) {
                this.factory = new(Array.Empty<IComponent>());
                ErrorLabel.Content = ex.Message.ToString();
                ErrorLabel.Visibility = Visibility.Visible;
            }

        }

        public void GeneratorChanged(object sender, RoutedEventArgs e) {
            if ((MazeGeneratorTypes)Generator.SelectedItem != MazeGeneratorTypes.Static) {
                WidthTextBox.IsEnabled = true;
                HeightTextBox.IsEnabled = true;
                FilenameTextBox.IsEnabled = false;
                FileDialogButton.IsEnabled = false;
            }
            else {
                WidthTextBox.IsEnabled = false;
                HeightTextBox.IsEnabled = false;
                FilenameTextBox.IsEnabled = true;
                FileDialogButton.IsEnabled = true;
            }
        }

        //source: https://stackoverflow.com/questions/10315188/open-file-dialog-and-select-a-file-using-wpf-controls-and-c-sharp
        private void OpenFile(object sender, RoutedEventArgs e) {
            try {
                // Create OpenFileDialog 
                Microsoft.Win32.OpenFileDialog dlg = new() {
                    DefaultExt = ".txt",
                    Filter = "Text Files (*.txt) |*.txt"
                };
                Nullable<bool> result = dlg.ShowDialog();
                if (result == true) {
                    // Open document 
                    string filename = dlg.FileName;
                    FilenameTextBox.Text = filename;
                }
            }
            catch(Exception ex) {
                ErrorLabel.Content = ex.Message.ToString();
                ErrorLabel.Visibility = Visibility.Visible;
            }
        }

        public void Generate(object? sender, RoutedEventArgs? e) {
            try {
                MazeConstructionComponent constructionData = new(Int32.Parse(WidthTextBox.Text),
                    Int32.Parse(HeightTextBox.Text), FilenameTextBox.Text);
                IMazeGenerator gen = factory.Create((MazeGeneratorTypes)Generator.SelectedItem, constructionData);
                if ((MazeGeneratorTypes)Generator.SelectedItem != MazeGeneratorTypes.Static &&
                    (constructionData.Width > 200 || constructionData.Height > 200)) throw new Exception("maximum grootte van doolhof is 200x200");
                Maze maze = gen.Generate();
                DrawableCanvas.Children.Clear();
                ErrorLabel.Visibility = Visibility.Hidden;
                int width = (int)(DrawableCanvas.Width / maze.Width) / 2;
                int height = (int)(DrawableCanvas.Height / maze.Height) / 2;
                width = Math.Min(width, height); height = Math.Min(width, height);
                Ball ball = new(width, height, Math.Min(width, height) - 2);
                Render(maze, ball);
            }
            catch (Exception ex) {
                ErrorLabel.Content = ex.Message.ToString();
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

        public void Render(Maze maze, Ball ball) {
            Render(maze);
            try {
                SolidColorBrush brush = new() { Color = Color.FromRgb(255, 0, 0) };
                Ellipse point = new() {
                    Width = ball.Size,
                    Height = ball.Size,
                    StrokeThickness = 1,
                    Fill = brush,
                    Stroke = brush,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    RenderTransform = new TranslateTransform() {
                        X = ball.X - ball.Size / 2, //align center werkt niet bij rendertransform
                        Y = ball.Y - ball.Size / 2,
                    }
                };
                DrawableCanvas.Children.Add(point);
            }catch{ /*exception ignored for ball render*/}

        }

        public void Render(Maze maze) {
            // pixels / #cells
            double line_length_width = (DrawableCanvas.Width / maze.Width);
            double line_length_height = (DrawableCanvas.Height / maze.Height);
            line_length_width= Math.Min(line_length_width, line_length_height);
            line_length_height = Math.Min(line_length_width, line_length_height);
            for (int i = 0; i < maze.Width; i++) {
                for (int j = 0; j < maze.Height; j++) {
                    Cell cell = maze.maze[i, j];
                    WallDataComponent? wdc = (WallDataComponent?)cell.GetComponent(typeof(WallDataComponent));
                    if (wdc == null) continue;//cannot render
                    //horizontal lines
                    Line line = BuildLine((int)(line_length_width * i), (int)(line_length_width * (i + 1)),
                        (int)(line_length_height * j), (int)(line_length_height * j),
                        ((WallDataComponent)wdc).Width);
                    Line line2 = BuildLine((int)(line_length_width * i), (int)(line_length_width * (i + 1)),
                        (int)(line_length_height * (j + 1)), (int)(line_length_height * (j + 1)),
                        ((WallDataComponent)wdc).Width);
                    //vertical lines
                    Line line3 = BuildLine((int)(line_length_width * i), (int)(line_length_width * i),
                        (int)(line_length_height * j), (int)(line_length_height * (j + 1)),
                        ((WallDataComponent)wdc).Width);
                    Line line4 = BuildLine((int)(line_length_width * (i + 1)),(int)(line_length_width * (i + 1)),
                        (int)(line_length_height * j), (int)(line_length_height * (j + 1)),
                        ((WallDataComponent)wdc).Width);

                    if (cell.Walls[0]) DrawableCanvas.Children.Add(line);
                    if (cell.Walls[2]) DrawableCanvas.Children.Add(line2);
                    if (cell.Walls[3]) DrawableCanvas.Children.Add(line3);
                    if (cell.Walls[1]) DrawableCanvas.Children.Add(line4);
                }
            }
        }
    }
}
