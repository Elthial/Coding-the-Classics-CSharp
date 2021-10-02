using Microsoft.Xna.Framework;
using System;

namespace Cavern.Actors
{
    //Call constructor of parent class. Initial pos is 0,0 but reset is always called straight afterwards which
    //will set the actual starting position.
    class Player : GravityActor
    {
        GameSession Session;
        public int lives;
        public int score;
        private int hurt_timer;
        public int health;
        private int direction_x;
        private int fire_timer;
        private Orb blowing_orb;
        private int dx;

        public Player(GameSession GameSession) : base(new Vector2(0, 0))
        {
            this.Session = GameSession;
            this.lives = 2;
            this.score = 0;
        }

        public void reset()
        {
            this.Pos = new Vector2(Cavern.WIDTH / 2, 100);
            this.vel_y = 0;
            this.direction_x = 1;            // -1 = left, 1 = right
            this.fire_timer = 0;
            this.hurt_timer = 100;  // Invulnerable for this many frames
            this.health = 3;
            this.blowing_orb = null;
        }



        public bool hit_test(Bolt other)
        {
            //Check for collision between player and bolt - called from Bolt.update. Also check hurt_timer - after being hurt,
            //there is a period during which the player cannot be hurt again
            if (this.collidepoint(other.Pos) && this.hurt_timer < 0)
            {
                //Player loses 1 health, is knocked in the direction the bolt had been moving, and can't be hurt again
                //for a while
                this.hurt_timer = 200;
                this.health -= 1;
                this.vel_y = -12;
                this.landed = false;
                this.direction_x = other.direction_x;
                if (this.health > 0)
                {
                    Session.play_sound("ouch", 4);
                }
                else
                {
                    Session.play_sound("die");
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public update()
        {
            //Call GravityActor.update - parameter is whether we want to perform collision detection as we fall. If health
            //is zero, we want the player to just fall out of the level
            base.update(this.health > 0);

            this.fire_timer -= 1;
            this.hurt_timer -= 1;

            if (this.landed)
            {
                //Hurt timer starts at 200, but drops to 100 once the player has landed
                this.hurt_timer = Math.Min(this.hurt_timer, 100);
            }

            if (this.hurt_timer > 100)
            {
                //We've just been hurt. Either carry out the sideways motion from being knocked by a bolt, or if health is
                //zero, we're dropping out of the level, so check for our sprite reaching a certain Y coordinate before
                //reducing our lives count and responding the player. We check for the Y coordinate being the screen height
                //plus 50%, rather than simply the screen height, because the former effectively gives us a short delay
                //before the player respawns.
                if (this.health > 0)
                {
                    this.move(this.direction_x, 0, 4);
                }
                else
                {
                    //                if self.top >= HEIGHT*1.5:
                    //                    self.lives -= 1
                    //                    self.reset()
                }
            }
            else
            {
                //We're not hurt
                //Get keyboard input. dx represents the direction the player is facing
                dx = 0;
                if (keyboard.left)
                    dx = -1;
                else if (keyboard.right)
                    dx = 1;

                if (dx != 0)
                {
                    this.direction_x = dx;

                    //If we haven't just fired an orb, carry out horizontal movement
                    if (this.fire_timer < 10)
                        this.move(dx, 0, 4);
                }

                //Do we need to create a new orb? Space must have been pressed and released, the minimum time between
                //orbs must have passed, and there is a limit of 5 orbs.
                if (space_pressed() && this.fire_timer <= 0 && len(Session.Orbs) < 5)
                    {
                    //x position will be 38 pixels in front of the player position, while ensuring it is within the
                    //bounds of the level
                    X = Math.Min(730, Math.Max(70, this.X + this.direction_x * 38));
                    Y = this.Y - 35;
                    this.blowing_orb = new Orb(new Vector2(X, Y), this.direction_x);
                    Session.Orbs.append(this.blowing_orb);
                    Session.play_sound("blow", 4);
                    this.fire_timer = 20;
                }

                if (keyboard.up && this.vel_y == 0 && this.landed)
                    {
                    //Jump
                    this.vel_y = -16;
                    this.landed = false;
                    Session.play_sound("jump");
                }
            }

            //Holding down space causes the current orb (if there is one) to be blown further
            if (keyboard.space)
            {
                if (this.blowing_orb)
                {
                    //Increase blown distance up to a maximum of 120
                    this.blowing_orb.blown_frames += 4;
                    if (this.blowing_orb.blown_frames >= 120)
                    {
                        //Can't be blown any further
                        this.blowing_orb = null;
                    }
                }
            }
            else
            {
                //If we let go of space, we relinquish control over the current orb - it can't be blown any further
                this.blowing_orb = null;
            }



            //Set sprite image. If we're currently hurt, the sprite will flash on and off on alternate frames.
            this.Image = "blank";
            if (this.hurt_timer <= 0 || this.hurt_timer % 2 == 1)
            {
                var dir_index = this.direction_x > 0 ? "1" : "0";
                if (this.hurt_timer > 100)
                {
                    if (this.health > 0)
                        this.Image = $"recoil{dir_index}";
                    else
                        this.Image = $"fall{(Session.timer / 4) % 2}";
                }
                else if (this.fire_timer > 0)
                {
                    this.Image = $"blow{dir_index}";
                }
                else if (dx == 0)
                {
                    this.Image = "still";
                }
                else
                {
                    this.Image = $"run{dir_index}{(Session.timer / 8) % 4}";
                }
            }
        }
    }
}