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
using System.Windows.Input;
using System.Diagnostics;

namespace WPF_physics_simulator {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private readonly Maze maze;
        private readonly Ball ball;
        private readonly Globals.Rect[] physicsRectangles;
        private double AngleX;
        private double AngleY;
        private PhysicsSimulator physicsSimulator;

        private int cellsize = 100;
        private Stopwatch stopwatch;
        public MainWindow() {
            InitializeComponent();
            MazeGeneratorFactory factory = new(new IComponent[] { new WallDataComponent(cellsize/5) });
            MazeConstructionComponent constructionData = new(-1, -1, "default.txt");// -1 to indicate unused
            IMazeGenerator generator = factory.Create(MazeGeneratorTypes.Static, constructionData);
            
            this.maze = generator.Generate();
            this.ball = new(cellsize/2, cellsize/2, cellsize/4, new IComponent[] {new PhysicsComponent()});//x,y,size (=radius), physicscomponent
            try {
                this.physicsRectangles = CalculatePhysicsObjects(cellsize);
                this.physicsSimulator = new(physicsRectangles, ball, maze.Width, maze.Height, cellsize);
                stopwatch = new();
                stopwatch.Start();
                CompositionTarget.Rendering += loop;
            } catch(Exception ex) {
                Writable.Content = ex.Message;
            }
            
        }

        private void OnKeyDownHandler(object sender, KeyEventArgs e) {
            if (e.Key == Key.O) {
                AngleY -= Math.PI / 45;//2 graden incline
                //ball.Y -= 10;
            }
            if (e.Key == Key.K) {
                AngleX -= Math.PI / 45;//2 graden incline
                //ball.X -= 10;
            }
            if (e.Key == Key.L) {
                AngleY += Math.PI / 45;//2 graden incline
                //ball.Y += 10;
            }
            if (e.Key == Key.M) {
                AngleX += Math.PI / 45;//2 graden incline
                //ball.X += 10;
            }
            e.Handled = true;
        }

        private void loop(object Sender, EventArgs e) {
            try {
                long millis = stopwatch.ElapsedMilliseconds;
                millis = 15;
                var pc = physicsSimulator.Simulate(AngleX, AngleY, millis);
                Render(cellsize/2);
                stopwatch.Restart();
                Writable.Content = $"x:{AngleX} y:{AngleY}\nmillis:{millis}\nForce: {pc.Force.X} {pc.Force.Y}\nVelocity {pc.Velocity.X} {pc.Velocity.Y}\nAcceleration {pc.Acceleration.X} {pc.Acceleration.Y}\nPos {ball.X} {ball.Y}";
            }
            catch(Exception ex) {
                Writable.Content = ex.Message;
            }
        }

        private double RadiansToDegrees(double radians) {
            return radians / Math.PI * 90;
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

            counter = maze.Width * (maze.Height - 1)-1;
            for (int i = 0; i < maze.Height; i++) {
                Cell c = maze.maze[maze.Width - 1,i];
                wdc = (WallDataComponent?)(c.GetComponent(typeof(WallDataComponent)));
                wdc ??= new WallDataComponent(2);
                rect = new Globals.Rect((c.x + 1) * cellsize - ((WallDataComponent)wdc).Width, c.y * cellsize, (c.x + 1) * cellsize, (c.y + 1) * cellsize, counter);
                if (!c.Walls[1]) continue;
                rects.Add(rect);
                counter++;
            }
            counter = maze.Height-1;
            for (int i = 0; i < maze.Width; i++) {
                Writable.Content = ($"{maze.maze[i, maze.Height - 1].x} {maze.maze[i, maze.Height - 1].y} {counter}");
                Cell c = maze.maze[i, maze.Height - 1];
                wdc = (WallDataComponent?)(c.GetComponent(typeof(WallDataComponent)));
                wdc ??= new WallDataComponent(2);
                rect = new Globals.Rect(c.x * cellsize, (c.y + 1) * cellsize - ((WallDataComponent)wdc).Width, (c.x + 1) * cellsize, (c.y + 1) * cellsize, counter);
                if (!c.Walls[2]) continue;
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
                if (rect.Mark) {
                    model.Material = Materials.Red;
                }
                if(rect.Collides) model.Material = Materials.Green;
                group.Children.Add(model);
            }

            model = CreateGeometry(0, 0, cellsize * maze.Width, cellsize * maze.Height, 15,-15.001);
            group.Children.Add(model);

            var builder = new MeshBuilder(true, true);
            var position = new Point3D(this.ball.X, - this.ball.Y, this.ball.Size);
            builder.AddSphere(position, ball.Size, 15, 15);

            model = new GeometryModel3D(builder.ToMesh(), Materials.Red);
            group.Children.Add(model);
            
            group.Transform = GetTransformation();
            model3dContainer.Content = group;
        }

        private Transform3DGroup GetTransformation() {
            var translateTransform = new TranslateTransform3D(-cellsize * maze.Width / 2, cellsize * maze.Height / 2, 0);
            RotateTransform3D RotateTransform3D_X = new RotateTransform3D();
            AxisAngleRotation3D AxisAngleRotation3d_X = new AxisAngleRotation3D();
            AxisAngleRotation3d_X.Axis = new Vector3D(0, 1, 0);
            AxisAngleRotation3d_X.Angle = RadiansToDegrees(AngleX);
            RotateTransform3D_X.Rotation = AxisAngleRotation3d_X;
            RotateTransform3D RotateTransform3D_Y = new RotateTransform3D();
            AxisAngleRotation3D AxisAngleRotation3d_Y = new AxisAngleRotation3D();
            AxisAngleRotation3d_Y.Axis = new Vector3D(1, 0, 0);
            AxisAngleRotation3d_Y.Angle = RadiansToDegrees(AngleY);
            RotateTransform3D_Y.Rotation = AxisAngleRotation3d_Y;
            var transformGroup = new Transform3DGroup();
            transformGroup.Children.Add(translateTransform);
            transformGroup.Children.Add(RotateTransform3D_X);
            transformGroup.Children.Add(RotateTransform3D_Y);
            return transformGroup;
        }

        public static GeometryModel3D CreateGeometry(double x1, double y1, double x2, double y2, double height) {
            return CreateGeometry(x1, y1, x2, y2, height, 0);
        }

        public static GeometryModel3D CreateGeometry(double x1, double y1, double  x2, double y2, double height, double height_start) {
            GeometryModel3D model = new() {
                Material = Materials.White,
                Geometry = new MeshGeometry3D {
                    Positions = new Point3DCollection {
                        new Point3D(x1, -y1, height_start),
                        new Point3D(x2, -y1, height_start),
                        new Point3D(x1, -y1, height_start + height),
                        new Point3D(x2, -y1, height_start + height),
                        new Point3D(x1, -y2, height_start),
                        new Point3D(x2, -y2, height_start),
                        new Point3D(x1, -y2, height_start + height),
                        new Point3D(x2, -y2, height_start + height)
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
