using Boing;
using Boing.enums;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;

namespace CodingClassics
{
    public class Boing : Game
    {
        private GraphicsDeviceManager _graphics;
        internal SpriteBatch _spriteBatch;
        internal Dictionary<string, SoundEffect> _soundeffects;
        internal Dictionary<string, Texture2D> _Texture2Ds;
        internal KeyboardState _keyboard;

        private Song BackgroundSong;

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
 
        public Boing()
        {            
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = WIDTH;
            _graphics.PreferredBackBufferHeight = HEIGHT;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.Title = TITLE;
        }

        protected override void Initialize()
        {        
            //Set the initial game state
            State = GameState.MENU;

            num_players = 1;

            //Is space currently being held down?
            space_down = false;            

            

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            BackgroundSong = Content.Load<Song>($"music/theme");          
            MediaPlayer.IsRepeating = true;

            _soundeffects = new Dictionary<string, SoundEffect>
            {
                { "bounce_synth0", Content.Load<SoundEffect>($"Sounds/bounce_synth0") },
                { "bounce0", Content.Load<SoundEffect>($"Sounds/bounce0") },
                { "bounce1", Content.Load<SoundEffect>($"Sounds/bounce1") },
                { "bounce2", Content.Load<SoundEffect>($"Sounds/bounce2") },
                { "bounce3", Content.Load<SoundEffect>($"Sounds/bounce3") },
                { "bounce4", Content.Load<SoundEffect>($"Sounds/bounce4") },
                { "down", Content.Load<SoundEffect>($"Sounds/down") },
                { "hit_fast0", Content.Load<SoundEffect>($"Sounds/hit_fast0") },
                { "hit_medium0", Content.Load<SoundEffect>($"Sounds/hit_medium0") },
                { "hit_slow0", Content.Load<SoundEffect>($"Sounds/hit_slow0") },
                { "hit_synth0", Content.Load<SoundEffect>($"Sounds/hit_synth0") },
                { "hit_veryfast0", Content.Load<SoundEffect>($"Sounds/hit_veryfast0") },
                { "hit0", Content.Load<SoundEffect>($"Sounds/hit0") },
                { "hit1", Content.Load<SoundEffect>($"Sounds/hit1") },
                { "hit2", Content.Load<SoundEffect>($"Sounds/hit2") },
                { "hit3", Content.Load<SoundEffect>($"Sounds/hit3") },
                { "hit4", Content.Load<SoundEffect>($"Sounds/hit4") },
                { "score_goal0", Content.Load<SoundEffect>($"Sounds/score_goal0") },
                { "up", Content.Load<SoundEffect>($"Sounds/up") }                
            };

            _Texture2Ds = new Dictionary<string, Texture2D>
            {
                { "ball", Content.Load<Texture2D>($"images/ball") },
                { "bat00", Content.Load<Texture2D>($"images/bat00") },
                { "bat01", Content.Load<Texture2D>($"images/bat01") },
                { "bat02", Content.Load<Texture2D>($"images/bat02") },
                { "bat10", Content.Load<Texture2D>($"images/bat10") },
                { "bat11", Content.Load<Texture2D>($"images/bat11") },
                { "bat12", Content.Load<Texture2D>($"images/bat12") },
                { "blank", Content.Load<Texture2D>($"images/blank") },
                { "digit00", Content.Load<Texture2D>($"images/digit00") },
                { "digit01", Content.Load<Texture2D>($"images/digit01") },
                { "digit02", Content.Load<Texture2D>($"images/digit02") },
                { "digit03", Content.Load<Texture2D>($"images/digit03") },
                { "digit04", Content.Load<Texture2D>($"images/digit04") },
                { "digit05", Content.Load<Texture2D>($"images/digit05") },
                { "digit06", Content.Load<Texture2D>($"images/digit06") },
                { "digit07", Content.Load<Texture2D>($"images/digit07") },
                { "digit08", Content.Load<Texture2D>($"images/digit08") },
                { "digit09", Content.Load<Texture2D>($"images/digit09") },
                { "digit10", Content.Load<Texture2D>($"images/digit10") },
                { "digit11", Content.Load<Texture2D>($"images/digit11") },
                { "digit12", Content.Load<Texture2D>($"images/digit12") },
                { "digit13", Content.Load<Texture2D>($"images/digit13") },
                { "digit14", Content.Load<Texture2D>($"images/digit14") },
                { "digit15", Content.Load<Texture2D>($"images/digit15") },
                { "digit16", Content.Load<Texture2D>($"images/digit16") },
                { "digit17", Content.Load<Texture2D>($"images/digit17") },
                { "digit18", Content.Load<Texture2D>($"images/digit18") },
                { "digit19", Content.Load<Texture2D>($"images/digit19") },
                { "digit20", Content.Load<Texture2D>($"images/digit20") },
                { "digit21", Content.Load<Texture2D>($"images/digit21") },
                { "digit22", Content.Load<Texture2D>($"images/digit22") },
                { "digit23", Content.Load<Texture2D>($"images/digit23") },
                { "digit24", Content.Load<Texture2D>($"images/digit24") },
                { "digit25", Content.Load<Texture2D>($"images/digit25") },
                { "digit26", Content.Load<Texture2D>($"images/digit26") },
                { "digit27", Content.Load<Texture2D>($"images/digit27") },
                { "digit28", Content.Load<Texture2D>($"images/digit28") },
                { "digit29", Content.Load<Texture2D>($"images/digit29") },
                { "effect0", Content.Load<Texture2D>($"images/effect0") },
                { "effect1", Content.Load<Texture2D>($"images/effect1") },
                { "impact0", Content.Load<Texture2D>($"images/impact0") },
                { "impact1", Content.Load<Texture2D>($"images/impact1") },
                { "impact2", Content.Load<Texture2D>($"images/impact2") },
                { "impact3", Content.Load<Texture2D>($"images/impact3") },
                { "impact4", Content.Load<Texture2D>($"images/impact4") },
                { "menu0", Content.Load<Texture2D>($"images/menu0") },
                { "menu1", Content.Load<Texture2D>($"images/menu1") },
                { "over", Content.Load<Texture2D>($"images/over") },
                { "table", Content.Load<Texture2D>($"images/table") },
            };

            // Create a new Game object, without any players  
            Session = new GameSession(this, null, null);

            MediaPlayer.Play(BackgroundSong);
        }

        protected override void Update(GameTime gameTime)
        {
            _keyboard = Keyboard.GetState();

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || _keyboard.IsKeyDown(Keys.Escape))
                Exit();            

            // Work out whether the space key has just been pressed - i.e. in the previous frame it wasn't down, and in this frame it is.
            var space_pressed = false;
            if (_keyboard.IsKeyDown(Keys.Space) && !space_down)
            {
                space_pressed = true;                
            }
            space_down = _keyboard.IsKeyDown(Keys.Space);

            if (State == GameState.MENU)
            {
                if (space_pressed)
                {
                    /* Switch to play state, and create a new Game object, passing it the controls function for player 1, 
                     * and if we're in 2 player mode, the controls function for player 2 (otherwise the 'None' value 
                     * indicating this player should be computer-controlled) */
                    State = GameState.PLAY;

                    List<Func<int>> controls = new List<Func<int>> { new Func<int>(Session.P1_controls) };
                    if (num_players.Equals(2))
                    {
                        controls.Add(new Func<int>(Session.P2_controls));
                    }
                    else
                    {
                        controls.Add(null);
                    }
                    
                   Session = new GameSession(this, controls[0], controls[1]);               
                }
                else
                {
                    //Detect up/down keys
                    if (num_players.Equals(2) && _keyboard.IsKeyDown(Keys.Up))
                    {                       
                        _soundeffects["up"].Play();
                        num_players = 1;
                    }
                    else
                    {
                        if (num_players.Equals(1) && _keyboard.IsKeyDown(Keys.Down))
                        {              
                            _soundeffects["down"].Play();
                            num_players = 2;
                        }
                    }

                    //Update the 'attract mode' game in the background (two AIs playing each other)
                    Session.Update();
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
                    else
                    {
                        Session.Update();
                    }
                }                

                if(State == GameState.GAME_OVER)
                {
                    if (space_pressed)
                    {
                        //Reset to menu state
                        State = GameState.MENU;
                        num_players = 1;

                        //Create a new Game object, without any players
                        Session = new GameSession(this, null, null);
                    }
                }
            }
                        
            

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _spriteBatch.Begin();
            Session.Draw();

            if (State == GameState.MENU)
            {
                var menu_image = $"menu{num_players - 1}";             
                _spriteBatch.Draw(_Texture2Ds[menu_image], Vector2.Zero, Color.White);               
                //screen.blit(menu_image, (0, 0));
            }
            else
            {
                if(State == GameState.GAME_OVER)
                {                    
                    _spriteBatch.Draw(_Texture2Ds["over"], Vector2.Zero, Color.White);                    
                    //screen.blit("over", (0, 0));
                }
            }
            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
