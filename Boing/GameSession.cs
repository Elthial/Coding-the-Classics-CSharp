using CodingClassics.Actors;
using CodingClassics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodingClassics
{
    class GameSession
    {
        public Bat[] bats;
        public Ball  ball;
        public List<Impact> impacts;
      
        public int ai_offset;
        private SpriteBatch _spriteBatch;
        private Dictionary<string, Texture2D> _Texture2Ds;
        private Dictionary<string, SoundEffect> _soundeffects;     
        private KeyboardState keyboard;

        public GameSession(Boing boing, Func<int> p1_controls = null, Func<int> p2_controls = null) 
        {
            //Setup of XNA systems
            _spriteBatch = boing._spriteBatch;
            _Texture2Ds = boing._Texture2Ds;
            _soundeffects = boing._soundeffects;

            // Create a list of two bats, giving each a player number and a function to use to receive control inputs (or the value None if this is intended to be an AI player)
            bats = new Bat[] { new Bat(this, 0, p1_controls), new Bat(this, 1, p2_controls) };

            //Create a ball object
            ball = new Ball(this, -1);

            //Create an empty list which will later store the details of currently playing impact animations - these are displayed for a short time every time the ball bounces
            impacts = new List<Impact>();

            //Add an offset to the AI player's target Y position, so it won't aim to hit the ball exactly in the centre of the bat
            ai_offset = 0;
        }

        public void update()
        {
            keyboard = Keyboard.GetState();

            //Update all active objects     
            foreach (Bat b in bats) { b.update(); }
            ball.update();
            foreach (Impact i in impacts) { i.update(); }


            /*Remove any expired impact effects from the list. We go through the list backwards, starting from the last
              element, and delete any elements those time attribute has reached 10. We go backwards through the list
              instead of forwards to avoid a number of issues which occur in that scenario. In the next chapter we will
              look at an alternative technique for removing items from a list, using list comprehensions. */
            for (int i = impacts.Count - 1; i >= 0; i--)
            {
                if (impacts[i].time >= 10)
                {
                    impacts.RemoveAt(i);
                }
            }

            //Has ball gone off the left or right edge of the screen?
            if (ball.outofBounds())
            {
                //Work out which player gained a point, based on whether the ball was on the left or right-hand side of the screen
                var scoring_player = (ball.X < (CodingClassics.Boing.WIDTH / 2)) ? 1 : 0;
                var losing_player = 1 - scoring_player;

                /*We use the timer of the player who has just conceded a point to decide when to create a new ball in the centre of the level. 
                  This timer starts at zero at the beginning of the game and counts down by one every frame. 
                
                  Therefore, on the frame where the ball first goes off the screen, the timer will be less than zero.

                  We set it to 20, which means that this player's bat will display a different animation frame for 20 frames, and a new ball will be created after 20 frames */
                if (bats[losing_player].Timer < 0)
                {
                    bats[scoring_player].Score += 1;  
                    play_sound("score_goal", 1);
                    bats[losing_player].Timer = 20;
                }
                else
                {
                    if(bats[losing_player].Timer == 0)
                    {
                        // After 20 frames, create a new ball, heading in the direction of the player who just missed the ball
                        var direction = (losing_player == 0) ? -1 : 1;
                        ball = new Ball(this, direction);
                    }
                }   
            }
        }

        public void draw()
        {
            //Draw background
            _spriteBatch.Draw(_Texture2Ds["table"], Vector2.Zero, Color.White);
            //screen.blit("table", (0,0))

            int[] players = { 0, 1 };

            //Draw 'just scored' effects, if required
            foreach(int p in players)
            {
                if ((bats[p].Timer > 0) && (ball.outofBounds()))
                {
                    _spriteBatch.Draw(_Texture2Ds[$"effect{p}"], Vector2.Zero, Color.White);
                    //screen.blit("effect" + str(p), (0,0))
                }
            } 

            //Draw bats, ball and impact effects - in that order.
            foreach(Bat b in bats) { b.draw(_spriteBatch, _Texture2Ds); }
            ball.draw(_spriteBatch, _Texture2Ds);
            foreach(Impact i in impacts) { i.draw(_spriteBatch, _Texture2Ds); }

            //Display scores - outer loop goes through each player
            foreach(int p in players)
            {
                //Convert score into a string of 2 digits (e.g. "05") so we can later get the individual digits
                var score = bats[p].Score.ToString("{0:02d}");
                //Inner loop goes through each digit
                foreach (int i in players)
                {
                    //Digit sprites are numbered 00 to 29, where the first digit is the colour (0 = grey, 1 = blue, 2 = green) and the second digit is the digit itself
                    //Colour is usually grey but turns red or green (depending on player number) when a point has just been scored
                    var colour = "0";
                    var other_p = (1 - p);
                    if ((bats[other_p].Timer > 0) && (ball.outofBounds()))
                    {
                        colour =  (p == 0) ? "2" : "1";
                        var image = $"digit{colour}{score[i]}";
                        var vector = new Vector2(255 + (160 * p) + (i * 55), 46);
                        _spriteBatch.Draw(_Texture2Ds[image], Vector2.Zero, Color.White);
                        //screen.blit(image, (255 + (160 * p) + (i* 55), 46))
                    }     
                }         
            }
        }

        public void play_sound(string name, int count= 1)
        {
            //Some sounds have multiple varieties. If count > 1, we'll randomly choose one from those
            //We don't play any in-game sound effects if player 0 is an AI player - as this means we're on the menu

            if (bats[0].Move_func != bats[0].AI)
            {
                /*Pygame Zero allows you to write things like 'sounds.explosion.play()'
                  This automatically loads and plays a file named 'explosion.wav' (or .ogg) from the sounds folder (if such a file exists)
                  But what if you have files named 'explosion0.ogg' to 'explosion5.ogg' and want to randomly choose one of them to play? 
                  You can generate a string such as 'explosion3', but to use such a string to access an attribute of Pygame Zero's sounds object, 
                  we must use Python's built-in function getattr */
                try
                {
                    //getattr(sounds, name + str(random.randint(0, count - 1))).play()
                    var rand = new Random().Next(0, count - 1);
                    _soundeffects[$"{name}{rand}"].Play();
                }
                catch (System.Exception)
                {
                    throw;
                }
            }
        }

        public int p1_controls()
        {
            int move = 0;
            if(keyboard.IsKeyDown(Keys.Z) || keyboard.IsKeyDown(Keys.Down))
            {
                move = CodingClassics.Boing.PLAYER_SPEED;
            }
            else
            {
                if (keyboard.IsKeyDown(Keys.A) || keyboard.IsKeyDown(Keys.Up))
                {
                    move = -CodingClassics.Boing.PLAYER_SPEED;
                }
            }
            return move;
        }

        public int p2_controls()
        {
            int move = 0;
            if (keyboard.IsKeyDown(Keys.M))
            {
                move = CodingClassics.Boing.PLAYER_SPEED;
            }
            else
            {
                if (keyboard.IsKeyDown(Keys.K))
                {
                    move = -CodingClassics.Boing.PLAYER_SPEED;
                }
            }
            return move;
        }
    }
}