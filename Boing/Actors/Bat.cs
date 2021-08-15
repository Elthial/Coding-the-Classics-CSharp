using CodingClassics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Boing.Actors
{
    class Bat : Actor
    {
        public int Player;
        public int Score;
        public int Timer;
    
        private Func<int> Move_func;

        GameSession _game;

        public Bat(GameSession game, int player, Func<int> move_func = null) : base("blank", (((player == 0) ? 40 : 760), BoingGame.HALF_HEIGHT))
        {
            //Need to talk to parent
            _game = game;

            X = ((player == 0) ? 40 : 760);
            Y = BoingGame.HALF_HEIGHT;
           
            Player = player;
            Score = 0;

            /* move_func is a function we may or may not have been passed by the code which created this object. If this bat
              is meant to be player controlled, move_func will be a function that when called, returns a number indicating
              the direction and speed in which the bat should move, based on the keys the player is currently pressing.
              If move_func is None, this indicates that this bat should instead be controlled by the AI method. */
            if (move_func != null)
            {
                Move_func = move_func;
            }
            else
            {
                Move_func = new Func<int>(AI);
            }

            /* Each bat has a timer which starts at zero and counts down by one every frame. When a player concedes a point,
               their timer is set to 20, which causes the bat to display a different animation frame. It is also used to
               decide when to create a new ball in the centre of the screen - see comments in Game.update for more on this.
               Finally, it is used in Game.draw to determine when to display a visual effect over the top of the background */
            Timer = 0;
        }

        public void update()
        {
            Timer -= 1;

            // Our movement function tells us how much to move on the Y axis
            var y_movement = Move_func();

            //Apply y_movement to y position, ensuring bat does not go through the side walls
            Y = Math.Min(400, Math.Max(80, Y + y_movement));

            /* Choose the appropriate sprite. There are 3 sprites per player - e.g. bat00 is the left-hand player's
               standard bat sprite, bat01 is the sprite to use when the ball has just bounced off the bat, and bat02
               is the sprite to use when the bat has just missed the ball and the ball has gone out of bounds.
               bat10, 11 and 12 are the equivalents for the right-hand player */

            var frame = 0;
            if (Timer > 0)
            {

                if (_game.ball.outofBounds())
                {
                    frame = 2;
                }
                else
                {
                    frame = 1;
                }
            }

            image = $"bat{Player}{frame}";
        }

        private int AI()
        {

            //Returns a number indicating how the computer player will move - e.g. 4 means it will move 4 pixels downthe screen.

            //To decide where we want to go, we first check to see how far we are from the ball.
            var x_distance = Math.Abs(_game.ball.X - X);

            /*If the ball is far away, we move towards the centre of the screen (HALF_HEIGHT), on the basis that we don't
              yet know whether the ball will be in the top or bottom half of the screen when it reaches our position on
              the X axis. By waiting at a central position, we're as ready as it's possible to be for all eventualities. */
            var target_y_1 = BoingGame.HALF_HEIGHT;

            /* If the ball is close, we want to move towards its position on the Y axis. We also apply a small offset which
               is randomly generated each time the ball bounces. This is to make the computer player slightly less robotic
               - a human player wouldn't be able to hit the ball right in the centre of the bat each time. */
            var target_y_2 = _game.ball.Y + _game.ai_offset;

            /* The final step is to work out the actual Y position we want to move towards. We use what's called a weighted
               average - taking the average of the two target Y positions we've previously calculated, but shifting the
               balance towards one or the other depending on how far away the ball is. If the ball is more than 400 pixels
               (half the screen width) away on the X axis, our target will be half the screen height (target_y_1). If the
               ball is at the same position as us on the X axis, our target will be target_y_2. If it's 200 pixels away,
               we'll aim for halfway between target_y_1 and target_y_2. This reflects the idea that as the ball gets closer,
               we have a better idea of where it's going to end up. */
            var weight1 = Math.Min(1, x_distance / BoingGame.HALF_WIDTH);
            var weight2 = 1 - weight1;

            var target_y = (weight1 * target_y_1) + (weight2 * target_y_2);

            //Subtract target_y from our current Y position, then make sure we can't move any further than MAX_AI_SPEED each frame
            return Math.Min(BoingGame.MAX_AI_SPEED, Math.Max(-BoingGame.MAX_AI_SPEED, target_y - Y));
        }
    }
}