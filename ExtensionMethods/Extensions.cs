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

        public static String Print(this Maze maze) {
            String str = "";
            int width = maze.maze.GetLength(0);
            int height = maze.maze.GetLength(1);
            
            for (int i = 0; i < width; i++) {
                if (maze.maze[i, 0].walls[0]) str+=".-";
                else str+=". ";
            }
            str+=".\n";
            for (int i = 0; i < height; i++) {
                if (maze.maze[0, i].walls[3]) str+="| ";
                else str+="  ";
                for (int j = 0; j < width; j++) {
                    if (maze.maze[j, i].walls[1]) { str += "| "; }
                    else { str += "  "; }
                }
                str += "\n";
                for (int j=0; j < width;j++) {
                    if (maze.maze[j, i].walls[2]) { str += ".-"; }
                    else { str += ". "; }
                }
                str+=".\n";
            }
            return str;
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