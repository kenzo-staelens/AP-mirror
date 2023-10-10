using Components;
using ExtensionMethods;
using Generators;
using Globals;

namespace Console_Test_Environment {
    internal class Program {
        static void Main() {
            MazeGeneratorFactory factory = new();
            MazeConstructionComponent constructionData = new(11, 11, "testmaze.txt");
            IMazeGenerator gen = factory.Create(MazeGeneratorTypes.Destructive, constructionData); //new StaticGenerator("./testmaze.txt");
            Console.WriteLine("start");
            Maze maze = gen.Generate();
            Console.WriteLine(maze.ToStatic());
            Console.WriteLine(maze.Print());
        }
    }
}