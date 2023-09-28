using ExtensionMethods;
using Globals;
using Generators;

namespace Console_Test_Environment {
    internal class Program {
        static void Main(string[] args) {
            //Maze maze = new Maze(8, 8, false);
            //maze.maze[0,0].SetWall(3, true);
            MazeGeneratorFactory factory = new MazeGeneratorFactory();
            IMazeGenerator gen = factory.Create(MazeGeneratorTypes.Static); //new StaticGenerator("./testmaze.txt");
            Maze maze = gen.Generate();
            maze.Print();
        }
    }
}