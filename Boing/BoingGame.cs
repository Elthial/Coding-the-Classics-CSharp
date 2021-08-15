using Boing;
using Boing.enums;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace CodingClassics
{
    public class BoingGame : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Dictionary<string, SoundEffect> _soundeffects;

        // Global variables  
        public static readonly string TITLE = "Boing!";
        public static readonly int WIDTH = 800;
        public static readonly  int HEIGHT = 480;
        public static int HALF_WIDTH => (WIDTH / 2);
        public static int HALF_HEIGHT => (HEIGHT / 2);
        public static readonly int PLAYER_SPEED = 6;
        public static readonly int MAX_AI_SPEED = 6;


        GameSession Session;
        GameState State;
        int num_players;
        bool space_down;

        public BoingGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;       
        }

        protected override void Initialize()
        {
            //Set the initial game state
            State = GameState.MENU;

            num_players = 1;

            //Is space currently being held down?
            space_down = false;            

            // Create a new Game object, without any players  
            Session = new GameSession(null, null);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _soundeffects = new Dictionary<string, SoundEffect>
            {
                { "up", Content.Load<SoundEffect>($"Sounds/up") },
                { "down", Content.Load<SoundEffect>($"Sounds/down") },
            };          
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            var keyboard = Keyboard.GetState();

            // Work out whether the space key has just been pressed - i.e. in the previous frame it wasn't down, and in this frame it is.
            var space_pressed = false;
            if (keyboard.IsKeyDown(Keys.Space) && !space_down)
            {
                space_pressed = true;                
            }
            space_down = keyboard.IsKeyDown(Keys.Space);

            if (State == GameState.MENU)
            {
                if (space_pressed)
                {
                    /* Switch to play state, and create a new Game object, passing it the controls function for player 1, 
                     * and if we're in 2 player mode, the controls function for player 2 (otherwise the 'None' value 
                     * indicating this player should be computer-controlled) */
                    State = GameState.PLAY;

                    List<Func<int>> controls = new List<Func<int>> { new Func<int>(Session.p1_controls) };
                    if (num_players.Equals(2))
                    {
                        controls.Add(new Func<int>(Session.p2_controls));
                    }
                    else
                    {
                        controls.Add(null);
                    }
                    
                   Session = new GameSession(controls[0], controls[1]);               
                }
                else
                {
                    //Detect up/down keys
                    if (num_players.Equals(2) && keyboard.IsKeyDown(Keys.Up))
                    {                       
                        _soundeffects["up"].Play();
                        num_players = 1;
                    }
                    else
                    {
                        if (num_players.Equals(1) && keyboard.IsKeyDown(Keys.Down))
                        {              
                            _soundeffects["down"].Play();
                            num_players = 2;
                        }
                    }

                    //Update the 'attract mode' game in the background (two AIs playing each other)
                    Session.update();
                }
            }
            else
            {
                if(State == GameState.PLAY)
                {
                    // Has anyone won?
                    if (Math.Max(Session.bats[0].Score, Session.bats[1].Score) > 9)
                    {
                        State = GameState.GAME_OVER;
                    }                       
                }
                else
                {
                    Session.update();
                }

                if(State == GameState.GAME_OVER)
                {
                    if (space_pressed)
                    {
                        //Reset to menu state
                        State = GameState.MENU;
                        num_players = 1;

                        //Create a new Game object, without any players
                        Session = new GameSession(null, null);
                    }
                }
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            Session.draw();

            if (State == GameState.MENU)
            {
                var menu_image = $"menu{num_players - 1}";
                //screen.blit(menu_image, (0, 0));
            }
            else
            {
                if(State == GameState.GAME_OVER)
                {
                    //screen.blit("over", (0, 0));
                    //var tex = new Texture()
                    //SpriteBatch.Draw()
                }
            }                       

            base.Draw(gameTime);
        }
    }
}
