using Globals;
using Generators;
using Components;
using System.Windows;
using HelixToolkit;
using _3DTools;
using System.Reflection.Emit;
using System;
using System.Windows.Media.Media3D;
using System.Windows.Media;
using System.Reflection;
using HelixToolkit.Wpf;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace WPF_physics_simulator {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private readonly Maze maze;
        private readonly Ball ball;
        private readonly Globals.Rect[] physicsRectangles;
        public MainWindow() {
            InitializeComponent();
            MazeGeneratorFactory factory = new(new IComponent[] { new WallDataComponent(2) });
            MazeConstructionComponent constructionData = new(-1, -1, "default.txt");// -1 to indicate unused
            IMazeGenerator generator = factory.Create(MazeGeneratorTypes.Static, constructionData);
            int cellsize = 10;
            this.maze = generator.Generate();
            this.ball = new(5, 5, cellsize/4);//x,y,size (=radius)
            this.physicsRectangles = CalculatePhysicsObjects(cellsize);
            Render(15);
        }

        public Globals.Rect[] CalculatePhysicsObjects(int cellsize) {
            List<Globals.Rect> rects = new();
            WallDataComponent? wdc;
            int counter = 0;
            Globals.Rect rect;
            foreach (Cell c in this.maze.maze) {
                wdc = (WallDataComponent?)(c.GetComponent(typeof(WallDataComponent)));
                wdc ??= new WallDataComponent(2);
                if (c.Walls[0]) {
                    rect = new Globals.Rect(c.x * cellsize, c.y * cellsize, (c.x + 1) * cellsize, c.y * cellsize + ((WallDataComponent)wdc).Width, counter);
                    rects.Add(rect);
                }
                if (c.Walls[3]) {
                    rect = new Globals.Rect(c.x * cellsize, c.y * cellsize, c.x * cellsize + ((WallDataComponent)wdc).Width, (c.y + 1) * cellsize, counter);
                    rects.Add(rect);
                }
                counter++;
            }
            counter = maze.Width * (maze.Height - 1)+1;
            for (int i = 0; i < maze.Width; i++) {
                Cell c = maze.maze[i, maze.Height - 1];
                wdc = (WallDataComponent?)(c.GetComponent(typeof(WallDataComponent)));
                wdc ??= new WallDataComponent(2);
                rect = new Globals.Rect(c.x * cellsize, (c.y + 1) * cellsize - ((WallDataComponent)wdc).Width, (c.x + 1) * cellsize, (c.y + 1) * cellsize, counter);
                rects.Add(rect);
                counter++;
            }
            counter = maze.Width - 1;
            for (int i = 0; i < maze.Height; i++) {
                Cell c = maze.maze[maze.Width - 1, i];
                wdc = (WallDataComponent?)(c.GetComponent(typeof(WallDataComponent)));
                wdc ??= new WallDataComponent(2);
                rect = new Globals.Rect((c.x + 1) * cellsize - ((WallDataComponent)wdc).Width, c.y * cellsize, (c.x + 1) * cellsize, (c.y + 1) * cellsize, counter);
                rects.Add(rect);
                counter += maze.Width;
            }
            return rects.ToArray();
        }

        public void Render(int height) {
            model3dContainer.Children.Clear();
            Model3DGroup group = new();
            GeometryModel3D model;
            foreach (Globals.Rect rect in this.physicsRectangles) {
                model = CreateGeometry(rect.x1, rect.y1, rect.x2, rect.y2, height);
                group.Children.Add(model);
            }
            
            var builder = new MeshBuilder(true, true);
            var position = new Point3D(this.ball.X, - this.ball.Y, this.ball.Size);
            builder.AddSphere(position, ball.Size, 15, 15);

            model = new GeometryModel3D(builder.ToMesh(), Materials.Red);
            group.Children.Add(model);
            model3dContainer.Content = group;
        }

        public static GeometryModel3D CreateGeometry(double x1, double y1, double  x2, double y2, double height) {
            GeometryModel3D model = new() {
                Material = Materials.White,
                Geometry = new MeshGeometry3D {
                    Positions = new Point3DCollection {
                        new Point3D(x1, -y1, 0),
                        new Point3D(x2, -y1, 0),
                        new Point3D(x1, -y1, height),
                        new Point3D(x2, -y1, height),
                        new Point3D(x1, -y2, 0),
                        new Point3D(x2, -y2, 0),
                        new Point3D(x1, -y2, height),
                        new Point3D(x2, -y2, height)
                    },
                    TriangleIndices = new Int32Collection {
                        1,3,2,
                        0,1,2,
                        3,1,7,
                        1,5,7,
                        7,5,6,
                        5,4,6,
                        0,2,6,
                        4,0,6,
                        3,7,2,
                        7,6,2,
                        5,1,0,
                        4,5,0
                    },

                },
            };
            return model;
        }
    }
}
