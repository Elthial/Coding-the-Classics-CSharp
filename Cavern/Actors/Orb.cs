using CodingClassics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cavern.Actors
{
    class Orb : CollideActor
    {
        GameSession Session;
        const int MAX_TIMER = 250;
        private int direction_x;
        private bool floating;
        private object trapped_enemy_type;
        private int timer;
        private int blown_frames;

        public Orb(Vector2 Pos, int dir_x, GameSession GameSession) : base(Pos)
        {
            this.Session = GameSession;
            //Orbs are initially blown horizontally, then start floating upwards
            this.direction_x = dir_x;
            this.floating = false;
            this.trapped_enemy_type = null;      // Number representing which type of enemy is trapped in this bubble
            this.timer = -1;
            this.blown_frames = 6; // Number of frames during which we will be pushed horizontally
        }

        public bool hit_test(Bolt bolt)
        {
            //Check for collision with a bolt
            var collided = this.collidepoint(bolt.Pos);
            if (collided)
                this.timer = Orb.MAX_TIMER - 1;
            return collided;
        }

        public void update()
        {
            this.timer += 1;

            if (this.floating)
            {
                //Float upwards
                this.move(0, -1, Helpers.RandInt(1, 2));
            }
            else
            {
                //Move horizontally
                if (this.move(this.direction_x, 0, 4))
                {
                    //If we hit a block, start floating
                    this.floating = true;
                }
            }

            if (this.timer == this.blown_frames)
            {
                this.floating = true;
            }
            else if (this.timer >= Orb.MAX_TIMER || this.Y <= -40)
            {
                //Pop if our lifetime has run out or if we have gone off the top of the screen
                Session.pops.append(new Pop(this.Pos, 1));
                if (this.trapped_enemy_type != null)
                {
                    //trapped_enemy_type is either zero or one. A value of one means there's a chance of creating a
                    //powerup such as an extra life or extra health
                    Session.fruits.append(new Fruit(this.Pos, this.trapped_enemy_type))
                }
                Session.play_sound("pop", 4);
            }

            if (this.timer < 9)
            {
                //Orb grows to full size over the course of 9 frames - the animation frame updating every 3 frames
                this.Image = $"orb{(this.timer / 3)}";
            }
            else
            {
                if (this.trapped_enemy_type != null)
                {
                    this.Image = $"trap{(this.trapped_enemy_type)}{((this.timer / 4) % 8)}";
                }
                else
                {
                    this.Image = $"orb{(3 + (((this.timer - 9) / 8) % 4))}";
                }
            }
        }
    }
}