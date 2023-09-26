﻿using System.Runtime.CompilerServices;

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
    }
}