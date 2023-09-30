using Components;
using Globals;

namespace Generators {
    internal class RecursiveDivisionGenerator : IMazeGenerator {
        private readonly int width, height;
        private readonly Random random;
        private readonly IComponent[] extraComponents;
        public RecursiveDivisionGenerator(int width, int height) : this(width, height, Array.Empty<IComponent>()) { }
        public RecursiveDivisionGenerator(int width, int height, IComponent[] extraComponents) {
            this.width = width;
            this.height = height;
            this.random = new();
            this.extraComponents = extraComponents;
        }

        /// <summary>
        /// genereers doolhof met methode van recursief delen van cellen
        /// </summary>
        /// <returns></returns>
        /// <see href="https://en.wikipedia.org/wiki/Maze_generation_algorithm"/>
        public Maze Generate() {
            Maze maze = new(this.width, this.height, false, this.extraComponents);
            Stack<int[]> stack = new();
            stack.Push(new int[] { 0, 0, maze.Width - 1, maze.Height - 1 });//top left corner indexes ; bottom right corner indexes
            while (stack.Count > 0) {
                int[] item = stack.Pop();
                if (item == null) continue;
                int itemWidth = item[2] - item[0] + 1;
                int itemHeight = item[3] - item[1] + 1;
                if ((itemWidth * itemHeight < 6) && !(itemWidth == 2 && itemHeight == 2)) continue;
                //if (itemWidth * itemHeight < 6) { //super weird bug with true && true = false???
                //    //don't divide smaller than 6 -> don't generate cells smaller than 3 except for 2*2
                //    if (!(itemWidth == 2 && itemHeight == 2)) {
                //        continue;
                //    }
                //}
                if (itemWidth == 1 || itemHeight == 1) continue; //optimization step
                BuildDivision(item, itemWidth, itemHeight, stack, maze);
            }
            BuildOuterWalls(maze);
            return maze;
        }

        private void BuildDivision(int[] item, int itemWidth, int itemHeight, Stack<int[]> stack, Maze maze) {
            int[]? newCell1, newCell2;
            int randomNum;
            bool DivisionIsHorizontal;
            while (true) {
                DivisionIsHorizontal = (this.random.NextDouble() > (float)itemWidth / (itemHeight + itemWidth)); //lagere bias voor lange gangen
                if (DivisionIsHorizontal) {
                    randomNum = this.random.Next(itemHeight - 1);//max value excluded
                    newCell1 = new int[] { item[0], item[1], item[2], item[1] + randomNum };
                    newCell2 = new int[] { item[0], item[1] + randomNum + 1, item[2], item[3] };
                }
                else {//vertical division
                    randomNum = this.random.Next(itemWidth - 1);//max value excluded
                    newCell1 = new int[] { item[0], item[1], item[0] + randomNum, item[3] };
                    newCell2 = new int[] { item[0] + randomNum + 1, item[1], item[2], item[3] };
                }
                if (!PostCheck(newCell1, newCell2)) break;
            }
            BuildWalls(maze, newCell1, DivisionIsHorizontal, itemWidth, itemHeight);
            stack.Push(newCell1);
            stack.Push(newCell2);
        }

        private void BuildWalls(Maze maze, int[] newCell, bool isHorizontal, int width, int height) {
            int breakpoint;
            if (isHorizontal) {//bottom wall
                breakpoint = this.random.Next(width);
                for (int i = newCell[0]; i < newCell[2] + 1; i++) {
                    maze.maze[i, newCell[3]].SetWall(2, true);
                }
                maze.maze[newCell[0] + breakpoint, newCell[3]].SetWall(2, false);
            }
            else {//left wall
                breakpoint = this.random.Next(height);
                for (int i = newCell[1]; i < newCell[3] + 1; i++) {
                    maze.maze[newCell[2], i].SetWall(1, true);
                }
                maze.maze[newCell[2], newCell[1] + breakpoint].SetWall(1, false);
            }
        }

        private static void BuildOuterWalls(Maze maze) {
            for (int i = 0; i < maze.Width; i++) {
                maze.maze[i, 0].SetWall(0, true, false);//no cascade needed
                maze.maze[i, maze.Height - 1].SetWall(2, true);//no cascade needed
            }
            for (int i = 0; i < maze.Height; i++) {
                maze.maze[0, i].SetWall(3, true, false);
                maze.maze[maze.Width - 1, i].SetWall(1, true);
            }
        }

        private static bool PostCheck(int[] newCell1, int[] newCell2) {
            int cellSize1 = (newCell1[2] - newCell1[0] + 1) * (newCell1[3] - newCell1[1] + 1);
            int cellSize2 = (newCell2[2] - newCell2[0] + 1) * (newCell2[3] - newCell2[1] + 1);
            //returns false wanneer beide newCells >=3 zijn of cellen beiden 2 groot zijn (edge case voor 2*2)
            Console.WriteLine($"{cellSize1} {cellSize2}");
            if (cellSize1 == 2 && cellSize2 == 2) return false;
            return (cellSize1 < 3 || cellSize2 < 3) || cellSize1 == 0 || cellSize2 == 0;
        }
    }
}
