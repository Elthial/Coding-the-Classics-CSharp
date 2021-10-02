using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cavern.Actors
{
    public enum Anchor
    {
        ANCHOR_CENTRE,
        ANCHOR_CENTRE_BOTTOM
    }

    class CollideActor : Actor
    {
        public CollideActor(Vector2 pos, Anchor anchor = Anchor.ANCHOR_CENTRE) : base("blank", pos, anchor)
        { }


        public bool move(int dx, int dy, int speed)
        {
            var new_x = (int)X;
            var new_y = (int)Y;

            // Movement is done 1 pixel at a time, which ensures we don't get embedded into a wall we're moving towards
            for(int i = 0; i <= speed; i++)
            {
                new_x += dx;
                new_y += dy;

                if ((new_x < 70) || (new_x > 730))
                {
                    //Collided with edge of level
                    return true;
                }


                /** Normally you don't need brackets surrounding the condition for an if statement (unlike many other
                    languages), but in the case where the condition is split into multiple lines, using brackets removes
                    the need to use the \ symbol at the end of each line.

                    The code below checks to see if we're position we're trying to move into overlaps with a block. We only
                    need to check the direction we're actually moving in. So first, we check to see if we're moving down
                    (dy > 0). If that's the case, we then check to see if the proposed new y coordinate is a multiple of
                    GRID_BLOCK_SIZE. If it is, that means we're directly on top of a place where a block might be. If that's
                    also true, we then check to see if there is actually a block at the given position. If there's a block
                    there, we return True and don't update the object to the new position.

                    For movement to the right, it's the same except we check to ensure that the new x coordinate is a multiple
                    of GRID_BLOCK_SIZE. For moving left, we check to see if the new x coordinate is the last (right-most)
                    pixel of a grid block.

                    Note that we don't check for collisions when the player is moving up.**/

                if (
                     (dy > 0 && (new_y % Cavern.GRID_BLOCK_SIZE) == 0) ||
                     (dx > 0 && (new_x % Cavern.GRID_BLOCK_SIZE) == 0) ||
                     (dx < 0 && (new_x % Cavern.GRID_BLOCK_SIZE) == Cavern.GRID_BLOCK_SIZE - 1) &&
                     (Cavern.Block(new_x, new_y))
                   )
                {
                    return true;
                }
           
                //We only update the object's position if there wasn't a block there.
                Pos = new Vector2(new_x, new_y);
            }

            //Didn't collide with block or edge of level
            return false;
        }
    }
}