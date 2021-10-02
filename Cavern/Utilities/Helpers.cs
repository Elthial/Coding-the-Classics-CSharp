using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace CodingClassics
{
    public static class Helpers
    {
        private static Random rng = new Random();

        public static Vector2 Normalised(float X, float Y)
        {
            /* Return a unit vector
            # Get length of vector (x,y) - math.hypot uses Pythagoras' theorem to get length of hypotenuse
            # of right-angle triangle with sides of length x and y
            # todo note on safety */
            float length = (float)Math.Sqrt((X * X) + (Y * Y)); 
            return new Vector2(X / length, Y / length);            
        }

        public static int Sign(int X)
        {
            // Returns -1 or 1 depending on whether number is positive or negative
            return (X < 0) ? -1 : 1;
        }

        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }


        public static int RandInt(int LowerRange, int UpperRange)
        {
            return rng.Next(LowerRange, UpperRange);
        }
    }
}