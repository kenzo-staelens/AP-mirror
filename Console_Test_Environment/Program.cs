using ExtensionMethods;
using Globals;
using Generators;
using Components;

namespace Console_Test_Environment {
    internal class Program {
        static void Main(string[] args) {
            //Maze maze = new Maze(8, 8, false);
            //maze.maze[0,0].SetWall(3, true);
            MazeGeneratorFactory factory = new MazeGeneratorFactory();
            MazeConstructionComponent constructionData = new MazeConstructionComponent(0, 0, "testmaze.txt");
            IMazeGenerator gen = factory.Create(MazeGeneratorTypes.Static, constructionData); //new StaticGenerator("./testmaze.txt");
            Maze maze = gen.Generate();
            maze.Print();
            Console.WriteLine(String.Join(", ", maze.maze[0, 0].walls));
            Console.WriteLine(String.Join(", ", maze.maze[0, 1].walls));
            Console.WriteLine(String.Join(", ", maze.maze[1, 0].walls));
            Console.WriteLine(String.Join(", ", maze.maze[1, 1].walls));
        }
    }
}