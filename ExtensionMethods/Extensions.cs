using System.Runtime.CompilerServices;
using Globals;

namespace ExtensionMethods {
    public static class Extensions {
        //filter out null values from cell array
        public static T[] Filter<T>(this T[] array) {
            int count = 0;
            foreach (T item in array) {//calculate new array lenght
                if (item != null) count++;
            }
            T[] new_array = new T[count];
            count = 0;
            foreach (T item in array) {
                if (item != null) {
                    new_array[count]= item;
                    count++;
                }
            }
            return new_array;
        }

        public static String Multiply(this String str, int count) {
            String s = "";
            for(int i=0;i<count;i++) { s += str; }
            return s;
        }

        public static void Print(this Maze maze) {
            int width = maze.maze.GetLength(1);
            int height = maze.maze.GetLength(0);
            for(int i = 0; i < width; i++) {
                if (maze.maze[0, i].walls[0]) Console.Write(".-");
                else Console.Write(". ");
            }
            Console.WriteLine();
            for(int i = 0; i < height; i++) {
                if (maze.maze[i, 0].walls[3]) Console.Write("| ");
                else Console.Write("  ");
                for (int j = 0; j < width; j++) {
                    if (maze.maze[i, j].walls[1]) { Console.Write("| "); }
                    else { Console.Write("  "); }
                }
                Console.WriteLine();
                for (int j=0; j < height;j++) {
                    if (maze.maze[i, j].walls[2]) { Console.Write(".-"); }
                    else { Console.Write(". "); }
                }
                Console.WriteLine();
            }
        }

        public static String ToPrintable<T>(this T[,] matrix) {
            String printable="";
            for (int i = 0; i < matrix.GetLength(0); i++) {
                for (int j = 0; j < matrix.GetLength(1); j++) {
                    printable += matrix[i, j] + "\t";
                }
                printable += "\n";
            }
            return printable;
        }
    }

}