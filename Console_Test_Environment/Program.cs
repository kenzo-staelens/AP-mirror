using ExtensionMethods;
using Globals;

namespace Console_Test_Environment {
    internal class Program {
        static void Main(string[] args) {
            Maze maze = new Maze(8, 8, false);
            maze.maze[0,0].SetWall(3, true);
            maze.Print();
        }
    }
}