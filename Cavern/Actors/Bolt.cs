using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cavern.Actors
{
    class Bolt : CollideActor
    {
        GameSession Session;
        const int SPEED = 7;
        bool Active;
        public int direction_x;

        public Bolt(Vector2 pos, int dir_x, GameSession GameSession) : base(pos)
        {
            this.Session = GameSession;
            this.direction_x = dir_x;
            this.Active = true;
        }

        public void update()
        {
            //Move horizontally and check to see if we've collided with a block
            if (this.move(this.direction_x, 0, Bolt.SPEED))
            {
                // Collided
                this.Active = false;
            }
            else
            {
                var objects = new List<object>();
                objects.AddRange(Session.Orbs);
                objects.Add([Session.player]);
                //We didn't collide with a block - check to see if we collided with an orb or the player
                foreach (object obj in objects)
                {
                    if (obj && obj.hit_test(this))
                    { 
                        this.Active = false;
                        break;
                    }
                }
                           
            }

            var direction_idx = this.direction_x > 0 ? "1" : "0";
            var anim_frame = $"{(Session.timer / 4) % 2}";
            this.Image = "bolt" + direction_idx + anim_frame;
        }
    }
}