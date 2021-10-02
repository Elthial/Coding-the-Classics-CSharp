using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cavern.Actors
{

    //Class for pickups including fruit, extra health and extra life
    class Fruit : GravityActor
    {
        GameSession Session;
        int time_to_live;
        FruitType type;
        List<FruitType> types = new List<FruitType>();

        public Fruit(Vector2 Pos, RobotType trapped_enemy_type = 0, GameSession GameSession) : base(Pos)
        {
            this.Session = GameSession;
            //Choose which type of fruit we're going to be.
            if (trapped_enemy_type.Equals(RobotType.TYPE_NORMAL))
            {
                //self.type = choice([Fruit.APPLE, Fruit.RASPBERRY, Fruit.LEMON]);
            }
            else
            {
                // If trapped_enemy_type is 1, it means this fruit came from bursting an orb containing the more dangerous type
                // of enemy. In this case there is a chance of getting an extra help or extra life power up
                // We create a list containing the possible types of fruit, in proportions based on the probability we want
                // each type of fruit to be chosen
                types = 10 * [FruitType.APPLE, FruitType.RASPBERRY, FruitType.LEMON];   // Each of these appear in the list 10 times
                types += 9 * [FruitType.EXTRA_HEALTH];                       // This appears 9 times
                types += [FruitType.EXTRA_LIFE];                               // This only appears once
                //self.type = choice(types);                                   // Randomly choose one from the list
            }

            this.time_to_live = 500; // Counts down to zero
        }


    public void update()
        {
            base.update();

            //Does the player exist, and are they colliding with us?
            if (Session.player && Session.player.collidepoint(this.center))
            {
                if (this.type.Equals(FruitType.EXTRA_HEALTH))
                {
                    Session.player.health = Math.Min(3, Session.player.health + 1);
                    Session.play_sound("bonus");
                }
                else if (this.type.Equals(FruitType.EXTRA_LIFE))
                {
                    Session.player.lives += 1;
                    Session.play_sound("bonus");
                }
                else
                {
                    Session.player.score += (this.type + 1) * 100;
                    Session.play_sound("score");
                }

                this.time_to_live = 0;   //Disappear
            }
            else
            {
                this.time_to_live -= 1;
            }

            if (this.time_to_live <= 0)
            {
                //Create 'pop' animation
                Session.pops.append(new Pop(new Vector2(this.X, this.Y - 27), 0));
            }

            var anim_frame = $"{([0, 1, 2, 1][(Session.timer / 6) % 4])}";
            this.Image = $"fruit{this.type}{anim_frame}";
        }
}
}
