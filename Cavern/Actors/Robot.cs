using CodingClassics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cavern.Actors
{

    class Robot : GravityActor
    {
        GameSession Session;

        int speed;
        int direction_x;
        bool alive;
        int change_dir_timer;
        int fire_timer;
        RobotType type;

        public Robot(Vector2 Pos, RobotType type, GameSession GameSession) : base(Pos)
        {
            this.Session = GameSession;
            this.type = type;

            this.speed = Helpers.RandInt(1, 3);
            this.direction_x = 1;
            this.alive = true;

            this.change_dir_timer = 0;
            this.fire_timer = 100;
        }


        public update(self)
        {
            
            base.update();

            this.change_dir_timer -= 1;
            this.fire_timer += 1;

            //Move in current direction - turn around if we hit a wall
            if (this.move(this.direction_x, 0, this.speed))
                this.change_dir_timer = 0;

            if (this.change_dir_timer <= 0)
            {
                //Randomly choose a direction to move in
                //If there's a player, there's a two thirds chance that we'll move towards them
                var directions = [-1, 1];
                if (Session.player)
                    directions.append(Helpers.Sign(Session.player.X - this.X));
                this.direction_x = choice(directions);
                this.change_dir_timer = Helpers.RandInt(100, 250);
            }

            //The more powerful type of robot can deliberately shoot at orbs - turning to face them if necessary
            if (this.type == RobotType.TYPE_AGGRESSIVE && this.fire_timer >= 24)
            {
                //Go through all orbs to see if any can be shot at
                foreach (Orb orb in Session.Orbs)
                {
                    //The orb must be at our height, and within 200 pixels on the x axis
                    if (orb.Y >= this.top && orb.Y < this.bottom && Math.Abs(orb.X - this.X) < 200)
                    {
                        this.direction_x = Helpers.Sign(orb.X - this.X);
                        this.fire_timer = 0;
                        break;
                    }
                }
            }

            //Check to see if we can fire at player
            if (this.fire_timer >= 12)
            {
                //Random chance of firing each frame. Likelihood increases 10 times if player is at the same height as us
                var fire_probability = Session.fire_probability();
                if (Session.player && this.top < Session.player.bottom && this.bottom > Session.player.top)
                    fire_probability *= 10;
                if (random() < fire_probability)
                {
                    this.fire_timer = 0;
                    Session.play_sound("laser", 4);
                    // _soundeffects["down"].Play();
                }
            }
            else if (this.fire_timer == 8)
            {
                //Once the fire timer has been set to 0, it will count up - frame 8 of the animation is when the actual bolt is fired
                Session.bolts.append(new Bolt((this.X + this.direction_x * 20, this.Y - 38), this.direction_x));

                //Am I colliding with an orb? If so, become trapped by it
                foreach (Orb orb in Session.Orbs)
                {
                    if (orb.trapped_enemy_type is null && this.collidepoint(orb.center))
                    {
                        this.alive = false;
                        orb.floating = true;
                        orb.trapped_enemy_type = this.type;
                        Session.play_sound("trap", 4);
                        break;
                    }
                }

                //Choose and set sprite image
                var direction_idx = this.direction_x > 0 ? "1" : "0";
                var image = $"robot{this.type}{direction_idx}";
                if (this.fire_timer < 12)
                    image += $"{5 + (this.fire_timer / 4)}";
                else
                    image += $"{1 + ((Session.timer / 4) % 4)}";
                this.Image = image;
            }
        }
    }
}
