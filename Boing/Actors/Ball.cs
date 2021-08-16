using CodingClassics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodingClassics.Actors
{
    class Ball : Actor
    {
        private int dX;
        private int dY;
        private int Speed;
        readonly GameSession _game;

        public Ball(GameSession game, int dx) : base("ball", new Vector2(0,0))
        {
            //Need to talk with parent
            _game = game;

            this.

            Pos.X = CodingClassics.Boing.HALF_WIDTH;
            Pos.Y = CodingClassics.Boing.HALF_HEIGHT;

            // dx and dy together describe the direction in which the ball is moving. For example, if dx and dy are 1 and 0,
            // the ball is moving to the right, with no movement up or down. If both values are negative, the ball is moving
            // left and up, with the angle depending on the relative values of the two variables. If you're familiar with
            // vectors, dx and dy represent a unit vector. If you're not familiar with vectors, see the explanation in the
            // book.
            dX = dx;
            dY = 0;

            Speed = 5;
        }

        public void Update()
        {
            //Each frame, we move the ball in a series of small steps - the number of steps being based on its speed attribute
            for (int i = 0; i < Speed; i++)
            {
                // Store the previous x position
                var original_x = X;

                // Move the ball based on dx and dy
                Pos.X += dX;
                Pos.Y += dY;

                // Check to see if ball needs to bounce off a bat

                /* To determine whether the ball might collide with a bat, we first measure the horizontal distance from the
                   ball to the centre of the screen, and check to see if its edge has gone beyond the edge of the bat.

                   The centre of each bat is 40 pixels from the edge of the screen, or to put it another way, 360 pixels from the centre of the screen. 
                
                   The bat is 18 pixels wide and the ball is 14 pixels wide. 
                   Given that these sprites are anchored from their centres, when determining if they overlap or touch, we need to look at their half-widths - 9 and 7. 
                   Therefore, if the centre of the ball is 344 pixels from the centre of the screen, it can bounce off a bat 
                   (assuming the bat is in the right position on the Y axis - checked shortly afterwards).*/

                // We also check the previous X position to ensure that this is the first frame in which the ball crossed the threshold. 

                if ((Math.Abs(X - CodingClassics.Boing.HALF_WIDTH) >= 344) && (Math.Abs(original_x - CodingClassics.Boing.HALF_WIDTH) < 344))
                {

                    /* Now that we know the edge of the ball has crossed the threshold on the x-axis, we need to check to
                    # see if the bat on the relevant side of the arena is at a suitable position on the y-axis for the
                    # ball collide with it. */

                    int new_dir_x;
                    Bat bat;

                    if (X < CodingClassics.Boing.HALF_WIDTH)
                    {
                        new_dir_x = 1;
                        bat = _game.bats[0];
                    }
                    else
                    {
                        new_dir_x = -1;
                        bat = _game.bats[1];
                    }

                    var difference_y = Y - bat.Y;

                    if ((difference_y > -64) && (difference_y < 64))
                    {
                        // Ball has collided with bat - calculate new direction vector

                        /* To understand the maths used below, we first need to consider what would happen with this kind of
                        # collision in the real world. The ball is bouncing off a perfectly vertical surface. This makes for a
                        # pretty simple calculation. Let's take a ball which is travelling at 1 metre per second to the right,
                        # and 2 metres per second down. Imagine this is taking place in space, so gravity isn't a factor.
                        # After the ball hits the bat, it's still going to be moving at 2 m/s down, but it's now going to be
                        # moving 1 m/s to the left instead of right. So its speed on the y-axis hasn't changed, but its
                        # direction on the x-axis has been reversed. This is extremely easy to code - "self.dx = -self.dx".
                        # However, games don't have to perfectly reflect reality.
                        # In Pong, hitting the ball with the upper or lower parts of the bat would make it bounce diagonally
                        # upwards or downwards respectively. This gives the player a degree of control over where the ball
                        # goes. To make for a more interesting game, we want to use realistic physics as the starting point,
                        # but combine with this the ability to influence the direction of the ball. When the ball hits the
                        # bat, we're going to deflect the ball slightly upwards or downwards depending on where it hit the
                        # bat. This gives the player a bit of control over where the ball goes. */

                        // Bounce the opposite way on the X axis
                        dX = -dX;

                        // Deflect slightly up or down depending on where ball hit bat
                        dY += difference_y / 128;

                        // Limit the Y component of the vector so we don't get into a situation where the ball is bouncing up and down too rapidly
                        dY = Math.Min(Math.Max(dY, -1), 1);

                        // Ensure our direction vector is a unit vector, i.e. represents a distance of the equivalent of 1 pixel regardless of its angle
                        var vec2 = Helpers.Normalised(dX, dY);
                        dX = (int)vec2.X;
                        dY = (int)vec2.Y;

                        // Create an impact effect
                        _game.impacts.Add(new Impact(new Vector2(X - new_dir_x * 10, Y)));

                        // Increase speed with each hit
                        Speed++;

                        // Add an offset to the AI player's target Y position, so it won't aim to hit the ball exactly in the centre of the bat
                        var rand = new Random();
                        _game.ai_offset = (int)((rand.NextDouble() * 2.0 - 1.0) * 10);
                      
                        // Bat glows for 10 frames
                        bat.Timer = 10;

                        // Play hit sounds, with more intense sound effects as the ball gets faster
                        _game.Play_sound("hit", 5);  // play every time in addition to:

                        if (Speed <= 10)
                            _game.Play_sound("hit_slow", 1);

                        if (Speed <= 12)
                            _game.Play_sound("hit_medium", 1);

                        if (Speed <= 16)
                            _game.Play_sound("hit_fast", 1);

                        if (Speed <= 17)
                            _game.Play_sound("hit_veryfast", 1);
                    }
                }

                // The top and bottom of the arena are 220 pixels from the centre
                if (Math.Abs(Y - CodingClassics.Boing.HALF_HEIGHT) > 220)
                {
                    // Invert vertical direction and apply new dy to y so that the ball is no longer overlapping with the edge of the arena
                    dY = -dY;
                    Pos.Y += dY;

                    // Create impact effect
                    _game.impacts.Add(new Impact(Pos));

                    // Sound effect
                    _game.Play_sound("bounce", 5);
                    _game.Play_sound("bounce_synth", 1);
                }
            }

        }

        public bool OutofBounds()
        {
            // Has ball gone off the left or right edge of the screen?
            return (X < 0) || (X > CodingClassics.Boing.WIDTH);
        }
    }
}