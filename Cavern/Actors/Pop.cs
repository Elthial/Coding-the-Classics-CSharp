using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cavern.Actors
{
    class Pop : Actor
    {
        private int timer;
        private int type;

        public Pop(Vector2 Pos, int type) : base("blank", Pos)
        {
            this.type = type;
            this.timer = -1;
        }

        public void update()
        {
            this.timer += 1;
            this.Image = $"pop{(this.type)}{(this.timer / 2)}";
        }
    }
}