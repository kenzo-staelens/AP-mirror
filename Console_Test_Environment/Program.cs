using ExtensionMethods;
using Globals;
using Generators;
using Components;

namespace Console_Test_Environment {
    internal class Program {
        static void Main() {
            MazeGeneratorFactory factory = new();
            MazeConstructionComponent constructionData = new(11, 11, "testmaze.txt");
            IMazeGenerator gen = factory.Create(MazeGeneratorTypes.Additive, constructionData); //new StaticGenerator("./testmaze.txt");
            Console.WriteLine("start");
            Maze maze = gen.Generate();
            Console.WriteLine(maze.Print());
        }
    }
}