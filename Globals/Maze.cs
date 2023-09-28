using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilityFunctions;
using Components;

namespace Globals {
    public struct Maze {
        //width, height in number of cells
        public Cell[,] maze;
        public int Width { get; private set; }
        public int Height { get; private set; }

        public Maze(int width, int height) : this(width, height, true) { }
        public Maze(int width, int height, bool isDefaultSet) {
            this.Width = width;
            this.Height = height;
            this.maze = new Cell[width, height];
            for(int i = 0; i < width; i++) {
                for(int j = 0; j < height; j++) {
                    var components = new IComponent[] { new WallDataComponent(2) };
                    maze[i, j] = new Cell(i, j, isDefaultSet, components);
                }
            }
            for (int i = 0; i < width; i++) {
                for (int j = 0; j < height; j++) {
                    Cell[] neighbours = new Cell[4];
                    if (j != 0) neighbours[0] = maze[i, j - 1]; //cell above
                    if (i != width - 1) neighbours[1] = maze[i + 1, j]; //cell to the right
                    if (j != height - 1) neighbours[2] = maze[i, j + 1]; //cell below
                    if (i != 0) neighbours[3] = maze[i - 1, j]; //cell to the left
                    
                    //Utility.Filter(ref neighbours);
                    maze[i, j].Neighbours = neighbours;
                }
            }
        }
    }
}
