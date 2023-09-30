using Components;
using Datalaag;
using Globals;

namespace Generators {
    internal class StaticGenerator : IMazeGenerator {

        private readonly string mazeString;
        private readonly IComponent[] extraComponents;
        public StaticGenerator(String mazeString) : this(mazeString, Array.Empty<IComponent>()) { }
        public StaticGenerator(String mazeString, IComponent[] extraComponents) {
            this.mazeString = FileManager.Load(mazeString);
            this.extraComponents = extraComponents;
        }

        public Maze Generate() {
            String[] mazeDataArray = mazeString.Split("\n");
            int[] dimensions = mazeDataArray[0].Split(" ").Select(Int32.Parse).ToArray();//should be of length 2
            bool[][] integerMazeDataArray = mazeDataArray.Skip(1).Select(MazeDataConverter).ToArray();
            if (dimensions.Length != 2) {
                throw new ArgumentException($"invalid dimensions expected 2 values, received {dimensions.Length}");
            }
            Maze maze = new(dimensions[0], dimensions[1], false, this.extraComponents);
            if (integerMazeDataArray.Length < maze.Width * maze.Height) {
                throw new ArgumentException($"number of expected cells({maze.Width * maze.Height}) exceeds number of given cells ({integerMazeDataArray.Length})");
            }

            for (int i = 0; i < maze.Height; i++) {
                for (int j = 0; j < maze.Width; j++) {
                    for (int k = 0; k < integerMazeDataArray[i * maze.Width + j].Length; k++) {
                        bool setWallValue = integerMazeDataArray[i * maze.Width + j][k];
                        if(setWallValue)maze.maze[j, i].SetWall(k, setWallValue, setWallValue);//set true walls, but not empty walls
                    }
                }
            }
            return maze;
        }

        private static bool[] MazeDataConverter(String str) {
            return str.Split(" ").Select(Int32.Parse).Select(Convert.ToBoolean).ToArray();
        }
    }
}