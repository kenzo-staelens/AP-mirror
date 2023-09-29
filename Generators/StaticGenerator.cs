using System.CodeDom.Compiler;
using System.IO.Enumeration;
using System.Runtime.CompilerServices;
using Globals;
using Datalaag;
using Components;

namespace Generators {
    internal class StaticGenerator : IMazeGenerator{

        private readonly string filename;
        private readonly IComponent[] extraComponents;
        public StaticGenerator(String filename) : this(filename, Array.Empty<IComponent>()) { }
        public StaticGenerator(String filename, IComponent[] extraComponents) {
            this.filename = filename;
            this.extraComponents = extraComponents;
        }

        public Maze Generate() {
            String[] mazeDataArray = FileManager.Load(this.filename).Split("\n");
            int[] dimensions = mazeDataArray[0].Split(" ").Select(Int32.Parse).ToArray();//should be of length 2
            bool[][] integerMazeDataArray = mazeDataArray.Skip(1).Select(MazeDataConverter).ToArray();
            if(dimensions.Length != 2 ) {
                throw new ArgumentException($"invalid dimensions expected 2 values, received {dimensions.Length}");
            }
            Maze maze = new(dimensions[0], dimensions[1], true, this.extraComponents);
            if (integerMazeDataArray.Length < maze.Width * maze.Height) {
                throw new ArgumentException($"number of expected cells({maze.Width * maze.Height}) exceeds number of given cells ({integerMazeDataArray.Length})");
            }
            
            for(int i = 0; i < maze.Height; i++) {
                for(int j=0;j<maze.Width; j++) {
                    for (int k = 0; k < integerMazeDataArray[i * maze.Width + j].Length; k++) {
                        bool setWallValue = integerMazeDataArray[i * maze.Width + j][k];
                        maze.maze[j, i].SetWall(k, setWallValue, setWallValue);//cascades true walls, but not empty walls
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