using CodingClassics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cavern.Actors
{
    class GravityActor : CollideActor
    {

        const int MAX_FALL_SPEED = 10;
        protected int vel_y;
        protected bool landed;

        public GravityActor(Vector2 Pos) : base(Pos, ANCHOR.ANCHOR_CENTRE_BOTTOM)
        {         
            this.vel_y = 0;
            this.landed = false;
        }

        public void update(bool detect = true)
        {
            //Apply gravity, without going over the maximum fall speed
            this.vel_y = Math.Min(this.vel_y + 1, GravityActor.MAX_FALL_SPEED);

            /** The detect parameter indicates whether we should check for collisions with blocks as we fall. Normally we
                want this to be the case - hence why this parameter is optional, and is True by default. If the player is
                in the process of losing a life, however, we want them to just fall out of the level, so False is passed
                in this case. **/
            if (detect)
            {
                //Move vertically in the appropriate direction, at the appropriate speed
                if (this.move(0, Helpers.Sign(this.vel_y), Math.Abs(this.vel_y)))
                {
                    //If move returned True, we must have landed on a block.
                    // Note that move doesn't apply any collision detection when the player is moving up - only down
                    this.vel_y = 0;
                    this.landed = true;
                }

                if (this.top >= Cavern.HEIGHT)
                {
                    //Fallen off bottom - reappear at top
                    this.Y = 1;
                }
            }
            else
            {
                //Collision detection disabled - just update the Y coordinate without any further checks
                this.Y += this.vel_y;
            }
        }
    }
}