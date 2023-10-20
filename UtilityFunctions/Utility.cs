namespace UtilityFunctions {
    public static class Utility {
        public static void Filter<T>(ref T[] array) {
            int count = 0;
            foreach (T item in array) {//calculate new array lenght
                if (item != null) count++;
            }
            T[] new_array = new T[count];
            count = 0;
            foreach (T item in array) {
                if (item != null) {
                    new_array[count] = item;
                    count++;
                }
            }
            array = new_array;
        }

        public static double RadiansToDegrees(double radians) {
            return radians / Math.PI * 90;
        }
    }
}