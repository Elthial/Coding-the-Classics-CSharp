using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Boing.Actors
{
    public class Actor
    {
        public string name;
        public int X;
        public int Y;
        public string image;
        
        public (int x, int y) pos;

        public Actor(string Name, (int x, int y) Pos)
        {
            name = Name;
            X = Pos.x;
            Y = Pos.y;
        }

        public void draw()
        {
            //draw image at pos
        }
    }
}
