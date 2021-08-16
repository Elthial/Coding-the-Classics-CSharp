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
            var texture = _Texture2Ds[Image];
            var offsetx = texture.Width / 2;
            var offsety = texture.Height / 2;

            var offsetPos = new Vector2(Pos.X - offsetx, Pos.Y - offsety);

            _spriteBatch.Draw(texture, offsetPos, Color.White);  //draw image at pos
        }
    }
}
