using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodingClassics.Actors
{
    public class Actor
    {
        public string Image;
        public Vector2 Pos;
        public int X => (int)Pos.X;
        public int Y => (int)Pos.Y;        
        
        public Actor(string image, Vector2 pos)
        {
            Image = image;
            Pos = pos;          
        }

        public void Draw(SpriteBatch _spriteBatch, Dictionary<string, Texture2D> _Texture2Ds)
        {        
            _spriteBatch.Draw(_Texture2Ds[Image], Pos, Color.White);  //draw image at pos
        }
    }
}
