using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace CodingClassics
{
    public static class Helpers
    {
        public static Vector2 normalised(int X, int Y)
        {
            /* Return a unit vector
            # Get length of vector (x,y) - math.hypot uses Pythagoras' theorem to get length of hypotenuse
            # of right-angle triangle with sides of length x and y
            # todo note on safety */
            float length = (float)Math.Sqrt((X * X) + (Y * Y)); 
            return new Vector2(X / length, Y / length);            
        }

        public static int sign(int X)
        {
            // Returns -1 or 1 depending on whether number is positive or negative
            return (X < 0) ? -1 : 1;
        }  
    }
}
