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
using HelixToolkit.Wpf;
using System.Collections.Generic;
using System.Windows.Input;
using System.Diagnostics;
using UtilityFunctions;
using System.Reflection;

namespace WPF_physics_simulator {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        //magic numbers
        private static readonly int cellsize = 100;
        private static readonly int defaultWallWidth = cellsize / 5;
        private static readonly int defaultWallHeight = cellsize / 2;
        private static readonly double inclineDelta = Math.PI / 45; //2 graden incline

        //initialized objects
        private readonly Maze maze;
        private readonly Ball ball;
        private readonly Globals.Rect[] physicsRectangles;
        private readonly PhysicsSimulator physicsSimulator;
        private readonly Stopwatch stopwatch;
        private GeometryModel3D[] renderingObjects;

        //tracked values
        private double AngleX;
        private double AngleY;

        private static readonly bool debug = false;

#pragma warning disable CS8618
        public MainWindow() {
            InitializeComponent();
            try {
                //init objects
                MazeGeneratorFactory factory = new(new IComponent[] { new WallDataComponent(defaultWallWidth) });
                MazeConstructionComponent constructionData = new(-1, -1, "default.txt");// -1 to indicate unused
                IMazeGenerator generator = factory.Create(MazeGeneratorTypes.Static, constructionData);

                this.maze = generator.Generate();
                this.ball = new(cellsize/2, cellsize/2, cellsize/4, new IComponent[] {new PhysicsComponent()});//x,y,size (=radius), physicscomponent
                
                //precompute physics & rendering objects
                this.physicsRectangles = CalculatePhysicsObjects(cellsize);
                this.renderingObjects = CalculateRenderingObjects(cellsize);
                this.physicsSimulator = new(physicsRectangles, ball, maze, cellsize);
                
                stopwatch = new();
                stopwatch.Start();
                CompositionTarget.Rendering += Loop;
            } catch(Exception ex) {
                Writable.Content = ex.Message;
            }
        }
#pragma warning restore CS8618

        private void OnKeyDownHandler(object Sender, KeyEventArgs e) {
            if (e.Key == Key.O) {
                AngleY -= inclineDelta;
            }
            if (e.Key == Key.K) {
                AngleX -= inclineDelta;
            }
            if (e.Key == Key.L) {
                AngleY += inclineDelta;
            }
            if (e.Key == Key.M) {
                AngleX += inclineDelta;
            }
            e.Handled = true;
        }

        private void Loop(object? Sender, EventArgs e) {
            try {
                long millis = stopwatch.ElapsedMilliseconds;
                millis = 15;
                var pc = physicsSimulator.Simulate(AngleX, AngleY, millis);
                Render(defaultWallHeight);
                stopwatch.Restart();
                if(debug) Writable.Content = $"x:{AngleX} y:{AngleY}\nmillis:{millis}\nForce: {pc.Force.X} {pc.Force.Y}\nVelocity {pc.Velocity.X} {pc.Velocity.Y}\nAcceleration {pc.Acceleration.X} {pc.Acceleration.Y}\nPos {ball.X} {ball.Y}";
            }
            catch(Exception ex) {
                Writable.Content = ex.Message;
            }
        }

        public Globals.Rect[] CalculatePhysicsObjects(int cellsize) {
            List<Globals.Rect> rects = new();
            WallDataComponent? wdc;
            int counter = 0;
            Globals.Rect rect;
            foreach (Cell c in this.maze.maze) {
                wdc = (WallDataComponent?)(c.GetComponent(typeof(WallDataComponent)));
                wdc ??= new WallDataComponent(defaultWallWidth);
                if (c.Walls[0]) {
                    rect = new Globals.Rect(c.x * cellsize+0.1, c.y * cellsize, (c.x + 1) * cellsize, c.y * cellsize + ((WallDataComponent)wdc).Width, counter);
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
                rects.Add(rect);
                counter++;
            }
            counter = maze.Height-1;
            for (int i = 0; i < maze.Width; i++) {
                Cell c = maze.maze[i, maze.Height - 1];
                wdc = (WallDataComponent?)(c.GetComponent(typeof(WallDataComponent)));
                wdc ??= new WallDataComponent(2);
                rect = new Globals.Rect(c.x * cellsize, (c.y + 1) * cellsize - ((WallDataComponent)wdc).Width, (c.x + 1) * cellsize, (c.y + 1) * cellsize, counter);
                rects.Add(rect);
                counter += maze.Width;
            }
            return rects.ToArray();
        }

        public GeometryModel3D[] CalculateRenderingObjects(int height) {
            List<GeometryModel3D> models = new();
            GeometryModel3D model;
            foreach (Globals.Rect rect in this.physicsRectangles) {
                model = RenderingTools.CreateGeometry(rect.x1, rect.y1, rect.x2, rect.y2, height);
                if (rect.Mark) {
                    model.Material = Materials.Red;
                }
                if (rect.Collides) model.Material = Materials.Green;
                models.Add(model);
            }

            model = RenderingTools.CreateGeometry(0, 0, cellsize * maze.Width, cellsize * maze.Height, 15, -15.001);
            models.Add(model);
            return models.ToArray();
        }

        public void Render(int height) {
            model3dContainer.Children.Clear();
            Model3DGroup group = new();
            if (debug) this.renderingObjects = CalculateRenderingObjects(height);
            foreach (GeometryModel3D model in renderingObjects) group.Children.Add(model);
            

            //bal
            GeometryModel3D ballModel;
            var builder = new MeshBuilder(true, true);
            var position = new Point3D(this.ball.X, - this.ball.Y, this.ball.Size);
            builder.AddSphere(position, ball.Size, 15, 15);
            ballModel = new GeometryModel3D(builder.ToMesh(), Materials.Red);
            group.Children.Add(ballModel);
            
            //transform translate + rotate
            group.Transform = RenderingTools.GetTransformation(AngleX, AngleY, cellsize, maze.Width, maze.Height);
            model3dContainer.Content = group;
        }
    }
}
