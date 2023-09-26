using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExtensionMethods;

namespace Globals {
    public class Maze {
        //width, height in number of cells
        Cell[,] maze;
        public Maze(int width, int height) {
            this.maze = new Cell[width, height];
            for(int i = 0; i < width; i++) {
                for(int j = 0; j < height; j++) {
                    maze[i, j] = new Cell(i, j);
                }
            }
            for (int i = 0; i < width; i++) {
                for (int j = 0; j < height; j++) {
                    Cell[] neighbours = new Cell[4];
                    if (i != 0) neighbours[0] = maze[i - 1, j]; //cell to the left
                    if (i != width - 1) neighbours[1] = maze[i + 1, j]; //cell to the right
                    if (j != 0) neighbours[2] = maze[i, j - 1]; //cell above
                    if (j != height - 1) neighbours[3] = maze[i, j + 1]; //cell below
                    Extensions.Filter(ref neighbours);
                    maze[i, j].Neighbours = neighbours;
                }
            }
        }
    }
}
