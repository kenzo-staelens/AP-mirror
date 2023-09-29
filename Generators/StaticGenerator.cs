using System.CodeDom.Compiler;
using System.IO.Enumeration;
using System.Runtime.CompilerServices;
using Globals;
using Datalaag;

namespace Generators {
    internal class StaticGenerator : IMazeGenerator{

        private readonly string filename;
        public StaticGenerator(String filename) {
            this.filename = filename;
        }

        public Maze Generate() {
            String[] mazeDataArray = FileManager.Load(this.filename).Split("\n");
            int[] dimensions = mazeDataArray[0].Split(" ").Select(Int32.Parse).ToArray();//should be of length 2
            bool[][] integerMazeDataArray = mazeDataArray.Skip(1).Select(MazeDataConverter).ToArray();
            if(dimensions.Length != 2 ) {
                throw new ArgumentException($"invalid dimensions expected 2 values, received {dimensions.Length}");
            }
            Maze maze = new(dimensions[0], dimensions[1]);
            if (integerMazeDataArray.Length != maze.Width * maze.Height) {
                throw new ArgumentException($"number of expected cells({maze.Width * maze.Height}) does not match number of given cells {integerMazeDataArray.Length}");
            }
            
            for(int i = 0; i < maze.Width; i++) {
                for(int j=0;j<maze.Height; j++) {
                    for (int k = 0; k < integerMazeDataArray[i * maze.Height + j].Length; k++) {
                        maze.maze[i, j].SetWall(k, integerMazeDataArray[i * maze.Height + j][k], false);
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