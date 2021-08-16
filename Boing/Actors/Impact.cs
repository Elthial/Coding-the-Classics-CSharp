using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodingClassics.Actors
{
    // Class for an animation which is displayed briefly whenever the ball bounces
    class Impact : Actor
    {
        public int time;
        
        public Impact(Vector2 pos) : base("blank", pos)
        {         
            time = 0;
        }

        public void Update()
        {
            // There are 5 impact sprites numbered 0 to 4. We update to a new sprite every 2 frames.
            Image = $"impact{(time / 2)}";
            //The Game class maintains a list of Impact instances. 
            //In Game.update, if the timer for an object has gone beyond 10, the object is removed from the list.
            time += 1;
        }
    }
}
