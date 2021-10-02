using Cavern.Actors;
using Cavern.enums;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;

namespace Cavern
{
    public class Cavern : Game
    {
        private GraphicsDeviceManager _graphics;
        internal SpriteBatch _spriteBatch;
        internal KeyboardState _keyboard;
        internal Dictionary<string, SoundEffect> _soundeffects;
        internal Dictionary<string, Texture2D> _Texture2Ds;

        private Song BackgroundSong;

        // Set up constants
        public const int WIDTH = 800;
        public static int HEIGHT = 480;
        public static string TITLE = "Cavern";

        public static int NUM_ROWS = 18;
        public static int NUM_COLUMNS = 28;

        public static int LEVEL_X_OFFSET = 50;
        public static int GRID_BLOCK_SIZE = 25;

        //ANCHOR_CENTRE = ("center", "center")
        //ANCHOR_CENTRE_BOTTOM = ("center", "bottom")

        public static string[,] LEVELS = { {"XXXXX     XXXXXXXX     XXXXX",
                                           "","","","",
                                           "   XXXXXXX        XXXXXXX   ",
                                           "","","",
                                           "   XXXXXXXXXXXXXXXXXXXXXX   ",
                                           "","","",
                                           "XXXXXXXXX          XXXXXXXXX",
                                           "","","" },

                                          { "XXXX    XXXXXXXXXXXX    XXXX",
                                           "","","","",
                                           "    XXXXXXXXXXXXXXXXXXXX    ",
                                           "","","",
                                           "XXXXXX                XXXXXX",
                                           "      X              X      ",
                                           "       X            X       ",
                                           "        X          X        ",
                                           "         X        X         ",
                                           "","","" },

                                          { "XXXX    XXXX    XXXX    XXXX",
                                           "","","","",
                                           "  XXXXXXXX        XXXXXXXX  ",
                                           "","","",
                                           "XXXX      XXXXXXXX      XXXX",
                                           "","","",
                                           "    XXXXXX        XXXXXX    ",
                                           "","","" } };



        // Widths of the letters A to Z in the font images
        public static int[] CHAR_WIDTH = {27, 26, 25, 26, 25, 25, 26, 25, 12, 26, 26, 25, 33, 25, 26,
              25, 27, 26, 26, 25, 26, 26, 38, 25, 25, 25 };

        GameSession Session;
        GameState State;

        // Is the space bar currently being pressed down?
        bool space_down = false;

        Dictionary<string, int> IMAGE_WIDTH = new Dictionary<string, int>()
        {
            { "life", 44 },
            { "plus", 40 },
            { "health", 40 }
        };

        public Cavern()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            //Set the initial game state
            State = GameState.MENU;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            BackgroundSong = Content.Load<Song>($"music/theme");
            MediaPlayer.IsRepeating = true;

            _soundeffects = new Dictionary<string, SoundEffect>
            {
                { "appear0", Content.Load<SoundEffect>($"Sounds/appear0") },
                { "blow0", Content.Load<SoundEffect>($"Sounds/blow0") },
                { "blow1", Content.Load<SoundEffect>($"Sounds/blow1") },
                { "blow2", Content.Load<SoundEffect>($"Sounds/blow2") },
                { "blow3", Content.Load<SoundEffect>($"Sounds/blow3") },
                { "bonus0", Content.Load<SoundEffect>($"Sounds/bonus0") },
                { "die0", Content.Load<SoundEffect>($"Sounds/die0") },
                { "jump0", Content.Load<SoundEffect>($"Sounds/jump0") },
                { "land0", Content.Load<SoundEffect>($"Sounds/land0") },
                { "land1", Content.Load<SoundEffect>($"Sounds/land1") },
                { "land2", Content.Load<SoundEffect>($"Sounds/land2") },
                { "land3", Content.Load<SoundEffect>($"Sounds/land3") },
                { "laser0", Content.Load<SoundEffect>($"Sounds/laser0") },
                { "laser1", Content.Load<SoundEffect>($"Sounds/laser1") },
                { "laser2", Content.Load<SoundEffect>($"Sounds/laser2") },
                { "laser3", Content.Load<SoundEffect>($"Sounds/laser3") },
                { "level0", Content.Load<SoundEffect>($"Sounds/level0") },
                { "life0", Content.Load<SoundEffect>($"Sounds/life0") },
                { "ouch0", Content.Load<SoundEffect>($"Sounds/ouch0") },
                { "ouch1", Content.Load<SoundEffect>($"Sounds/ouch1") },
                { "ouch2", Content.Load<SoundEffect>($"Sounds/ouch2") },
                { "ouch3", Content.Load<SoundEffect>($"Sounds/ouch3") },
                { "over0", Content.Load<SoundEffect>($"Sounds/over0") },
                { "pop0", Content.Load<SoundEffect>($"Sounds/pop0") },
                { "pop1", Content.Load<SoundEffect>($"Sounds/pop1") },
                { "pop2", Content.Load<SoundEffect>($"Sounds/pop2") },
                { "pop3", Content.Load<SoundEffect>($"Sounds/pop3") },
                { "score0", Content.Load<SoundEffect>($"Sounds/score0") },
                { "trap0", Content.Load<SoundEffect>($"Sounds/trap0") },
                { "trap1", Content.Load<SoundEffect>($"Sounds/trap1") },
                { "trap2", Content.Load<SoundEffect>($"Sounds/trap2") },
                { "trap3", Content.Load<SoundEffect>($"Sounds/trap3") },
                { "vanish0", Content.Load<SoundEffect>($"Sounds/vanish0") }
            };

            _Texture2Ds = new Dictionary<string, Texture2D>
            {
               { "bg0", Content.Load<Texture2D>($"images/bg0") },
               { "bg1", Content.Load<Texture2D>($"images/bg1") },
               { "bg2", Content.Load<Texture2D>($"images/bg2") },
               { "bg3", Content.Load<Texture2D>($"images/bg3") },
               { "blank", Content.Load<Texture2D>($"images/blank") },
               { "block0", Content.Load<Texture2D>($"images/block0") },
               { "block1", Content.Load<Texture2D>($"images/block1") },
               { "block2", Content.Load<Texture2D>($"images/block2") },
               { "block3", Content.Load<Texture2D>($"images/block3") },
               { "blow0", Content.Load<Texture2D>($"images/blow0") },
               { "blow1", Content.Load<Texture2D>($"images/blow1") },
               { "bolt00", Content.Load<Texture2D>($"images/bolt00") },
               { "bolt01", Content.Load<Texture2D>($"images/bolt01") },
               { "bolt10", Content.Load<Texture2D>($"images/bolt10") },
               { "bolt11", Content.Load<Texture2D>($"images/bolt11") },
               { "cursor", Content.Load<Texture2D>($"images/cursor") },
               { "fall0", Content.Load<Texture2D>($"images/fall0") },
               { "fall1", Content.Load<Texture2D>($"images/fall1") },
               { "font032", Content.Load<Texture2D>($"images/font032") },
               { "font048", Content.Load<Texture2D>($"images/font048") },
               { "font049", Content.Load<Texture2D>($"images/font049") },
               { "font050", Content.Load<Texture2D>($"images/font050") },
               { "font051", Content.Load<Texture2D>($"images/font051") },
               { "font052", Content.Load<Texture2D>($"images/font052") },
               { "font053", Content.Load<Texture2D>($"images/font053") },
               { "font054", Content.Load<Texture2D>($"images/font054") },
               { "font055", Content.Load<Texture2D>($"images/font055") },
               { "font056", Content.Load<Texture2D>($"images/font056") },
               { "font057", Content.Load<Texture2D>($"images/font057") },
               { "font065", Content.Load<Texture2D>($"images/font065") },
               { "font066", Content.Load<Texture2D>($"images/font066") },
               { "font067", Content.Load<Texture2D>($"images/font067") },
               { "font068", Content.Load<Texture2D>($"images/font068") },
               { "font069", Content.Load<Texture2D>($"images/font069") },
               { "font070", Content.Load<Texture2D>($"images/font070") },
               { "font071", Content.Load<Texture2D>($"images/font071") },
               { "font072", Content.Load<Texture2D>($"images/font072") },
               { "font073", Content.Load<Texture2D>($"images/font073") },
               { "font074", Content.Load<Texture2D>($"images/font074") },
               { "font075", Content.Load<Texture2D>($"images/font075") },
               { "font076", Content.Load<Texture2D>($"images/font076") },
               { "font077", Content.Load<Texture2D>($"images/font077") },
               { "font078", Content.Load<Texture2D>($"images/font078") },
               { "font079", Content.Load<Texture2D>($"images/font079") },
               { "font080", Content.Load<Texture2D>($"images/font080") },
               { "font081", Content.Load<Texture2D>($"images/font081") },
               { "font082", Content.Load<Texture2D>($"images/font082") },
               { "font083", Content.Load<Texture2D>($"images/font083") },
               { "font084", Content.Load<Texture2D>($"images/font084") },
               { "font085", Content.Load<Texture2D>($"images/font085") },
               { "font086", Content.Load<Texture2D>($"images/font086") },
               { "font087", Content.Load<Texture2D>($"images/font087") },
               { "font088", Content.Load<Texture2D>($"images/font088") },
               { "font089", Content.Load<Texture2D>($"images/font089") },
               { "font090", Content.Load<Texture2D>($"images/font090") },
               { "fruit00", Content.Load<Texture2D>($"images/fruit00") },
               { "fruit01", Content.Load<Texture2D>($"images/fruit01") },
               { "fruit02", Content.Load<Texture2D>($"images/fruit02") },
               { "fruit10", Content.Load<Texture2D>($"images/fruit10") },
               { "fruit11", Content.Load<Texture2D>($"images/fruit11") },
               { "fruit12", Content.Load<Texture2D>($"images/fruit12") },
               { "fruit20", Content.Load<Texture2D>($"images/fruit20") },
               { "fruit21", Content.Load<Texture2D>($"images/fruit21") },
               { "fruit22", Content.Load<Texture2D>($"images/fruit22") },
               { "fruit30", Content.Load<Texture2D>($"images/fruit30") },
               { "fruit31", Content.Load<Texture2D>($"images/fruit31") },
               { "fruit32", Content.Load<Texture2D>($"images/fruit32") },
               { "fruit40", Content.Load<Texture2D>($"images/fruit40") },
               { "fruit41", Content.Load<Texture2D>($"images/fruit41") },
               { "fruit42", Content.Load<Texture2D>($"images/fruit42") },
               { "health", Content.Load<Texture2D>($"images/health") },
               { "jump0", Content.Load<Texture2D>($"images/jump0") },
               { "jump1", Content.Load<Texture2D>($"images/jump1") },
               { "life", Content.Load<Texture2D>($"images/life") },
               { "orb0", Content.Load<Texture2D>($"images/orb0") },
               { "orb1", Content.Load<Texture2D>($"images/orb1") },
               { "orb2", Content.Load<Texture2D>($"images/orb2") },
               { "orb3", Content.Load<Texture2D>($"images/orb3") },
               { "orb4", Content.Load<Texture2D>($"images/orb4") },
               { "orb5", Content.Load<Texture2D>($"images/orb5") },
               { "orb6", Content.Load<Texture2D>($"images/orb6") },
               { "over", Content.Load<Texture2D>($"images/over") },
               { "plus", Content.Load<Texture2D>($"images/plus") },
               { "pop00", Content.Load<Texture2D>($"images/pop00") },
               { "pop01", Content.Load<Texture2D>($"images/pop01") },
               { "pop02", Content.Load<Texture2D>($"images/pop02") },
               { "pop03", Content.Load<Texture2D>($"images/pop03") },
               { "pop04", Content.Load<Texture2D>($"images/pop04") },
               { "pop05", Content.Load<Texture2D>($"images/pop05") },
               { "pop06", Content.Load<Texture2D>($"images/pop06") },
               { "pop10", Content.Load<Texture2D>($"images/pop10") },
               { "pop11", Content.Load<Texture2D>($"images/pop11") },
               { "pop12", Content.Load<Texture2D>($"images/pop12") },
               { "pop13", Content.Load<Texture2D>($"images/pop13") },
               { "pop14", Content.Load<Texture2D>($"images/pop14") },
               { "pop15", Content.Load<Texture2D>($"images/pop15") },
               { "pop16", Content.Load<Texture2D>($"images/pop16") },
               { "recoil0", Content.Load<Texture2D>($"images/recoil0") },
               { "recoil1", Content.Load<Texture2D>($"images/recoil1") },
               { "robot000", Content.Load<Texture2D>($"images/robot000") },
               { "robot001", Content.Load<Texture2D>($"images/robot001") },
               { "robot002", Content.Load<Texture2D>($"images/robot002") },
               { "robot003", Content.Load<Texture2D>($"images/robot003") },
               { "robot004", Content.Load<Texture2D>($"images/robot004") },
               { "robot005", Content.Load<Texture2D>($"images/robot005") },
               { "robot006", Content.Load<Texture2D>($"images/robot006") },
               { "robot007", Content.Load<Texture2D>($"images/robot007") },
               { "robot010", Content.Load<Texture2D>($"images/robot010") },
               { "robot011", Content.Load<Texture2D>($"images/robot011") },
               { "robot012", Content.Load<Texture2D>($"images/robot012") },
               { "robot013", Content.Load<Texture2D>($"images/robot013") },
               { "robot014", Content.Load<Texture2D>($"images/robot014") },
               { "robot015", Content.Load<Texture2D>($"images/robot015") },
               { "robot016", Content.Load<Texture2D>($"images/robot016") },
               { "robot017", Content.Load<Texture2D>($"images/robot017") },
               { "robot100", Content.Load<Texture2D>($"images/robot100") },
               { "robot101", Content.Load<Texture2D>($"images/robot101") },
               { "robot102", Content.Load<Texture2D>($"images/robot102") },
               { "robot103", Content.Load<Texture2D>($"images/robot103") },
               { "robot104", Content.Load<Texture2D>($"images/robot104") },
               { "robot105", Content.Load<Texture2D>($"images/robot105") },
               { "robot106", Content.Load<Texture2D>($"images/robot106") },
               { "robot107", Content.Load<Texture2D>($"images/robot107") },
               { "robot110", Content.Load<Texture2D>($"images/robot110") },
               { "robot111", Content.Load<Texture2D>($"images/robot111") },
               { "robot112", Content.Load<Texture2D>($"images/robot112") },
               { "robot113", Content.Load<Texture2D>($"images/robot113") },
               { "robot114", Content.Load<Texture2D>($"images/robot114") },
               { "robot115", Content.Load<Texture2D>($"images/robot115") },
               { "robot116", Content.Load<Texture2D>($"images/robot116") },
               { "robot117", Content.Load<Texture2D>($"images/robot117") },
               { "run00", Content.Load<Texture2D>($"images/run00") },
               { "run01", Content.Load<Texture2D>($"images/run01") },
               { "run02", Content.Load<Texture2D>($"images/run02") },
               { "run03", Content.Load<Texture2D>($"images/run03") },
               { "run10", Content.Load<Texture2D>($"images/run10") },
               { "run11", Content.Load<Texture2D>($"images/run11") },
               { "run12", Content.Load<Texture2D>($"images/run12") },
               { "run13", Content.Load<Texture2D>($"images/run13") },
               { "space0", Content.Load<Texture2D>($"images/space0") },
               { "space1", Content.Load<Texture2D>($"images/space1") },
               { "space2", Content.Load<Texture2D>($"images/space2") },
               { "space3", Content.Load<Texture2D>($"images/space3") },
               { "space4", Content.Load<Texture2D>($"images/space4") },
               { "space5", Content.Load<Texture2D>($"images/space5") },
               { "space6", Content.Load<Texture2D>($"images/space6") },
               { "space7", Content.Load<Texture2D>($"images/space7") },
               { "space8", Content.Load<Texture2D>($"images/space8") },
               { "space9", Content.Load<Texture2D>($"images/space9") },
               { "stand0", Content.Load<Texture2D>($"images/stand0") },
               { "stand1", Content.Load<Texture2D>($"images/stand1") },
               { "still", Content.Load<Texture2D>($"images/still") },
               { "title", Content.Load<Texture2D>($"images/title") },
               { "trap00", Content.Load<Texture2D>($"images/trap00") },
               { "trap01", Content.Load<Texture2D>($"images/trap01") },
               { "trap02", Content.Load<Texture2D>($"images/trap02") },
               { "trap03", Content.Load<Texture2D>($"images/trap03") },
               { "trap04", Content.Load<Texture2D>($"images/trap04") },
               { "trap05", Content.Load<Texture2D>($"images/trap05") },
               { "trap06", Content.Load<Texture2D>($"images/trap06") },
               { "trap07", Content.Load<Texture2D>($"images/trap07") },
               { "trap10", Content.Load<Texture2D>($"images/trap10") },
               { "trap11", Content.Load<Texture2D>($"images/trap11") },
               { "trap12", Content.Load<Texture2D>($"images/trap12") },
               { "trap13", Content.Load<Texture2D>($"images/trap13") },
               { "trap14", Content.Load<Texture2D>($"images/trap14") },
               { "trap15", Content.Load<Texture2D>($"images/trap15") },
               { "trap16", Content.Load<Texture2D>($"images/trap16") },
               { "trap17", Content.Load<Texture2D>($"images/trap17") }
            };


            // Create a new Game object, without any players  
            Session = new GameSession();

            // Set up sound system and start music
            MediaPlayer.Play(BackgroundSong);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (State == GameState.MENU)
            {
                if (space_pressed())
                {
                    // Switch to play state, and create a new Game object, passing it a new Player object to use
                    State = GameState.PLAY;
                    Session = new GameSession(new Player());
                }
                else
                {
                    Session.Update();
                }
            }
            else if (State == GameState.PLAY)
            {
                if (Session.player.lives < 0)
                {
                    Session.play_sound("over");
                    State = GameState.GAME_OVER;
                }
                else
                {
                    Session.Update();
                }
            }
            else if (State == GameState.GAME_OVER)
            {
                if (space_pressed())
                {
                    // Switch to menu state, and create a new game object without a player
                    State = GameState.MENU;
                    Session = new GameSession();
                }
            }


            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _spriteBatch.Begin();
            Session.Draw();

            if (State.Equals(GameState.MENU))
            {
                // Draw title screen
                _spriteBatch.Draw(_Texture2Ds["title"], Vector2.Zero, Color.White);       

                /** Draw "Press SPACE" animation, which has 10 frames numbered 0 to 9
                    The first part gives us a number between 0 and 159, based on the game timer
                    Dividing by 4 means we go to a new animation frame every 4 frames
                    We enclose this calculation in the min function, with the other argument being 9, which results in the
                    animation staying on frame 9 for three quarters of the time. Adding 40 to the game timer is done to alter
                    which stage the animation is at when the game first starts **/
                var anim_frame = Math.Min(((Session.timer + 40) % 160) / 4, 9);

                _spriteBatch.Draw(_Texture2Ds[$"space{anim_frame}"], new Vector2(130, 280), Color.White);          
            }
            else if (State == GameState.PLAY)
            {
                draw_status();
            }
            else if (State.Equals(GameState.GAME_OVER))
            {
                draw_status();
                // Display "Game Over" image
                _spriteBatch.Draw(_Texture2Ds["over"], Vector2.Zero, Color.White);             
            }

            _spriteBatch.End();
            base.Draw(gameTime);
        }


        // Has the space bar just been pressed? i.e. gone from not being pressed, to being pressed
        private bool space_pressed()
        {
            _keyboard = Keyboard.GetState();

            if (_keyboard.IsKeyDown(Keys.Space))
            {
                if (space_down)
                {
                    // Space was down previous frame, and is still down
                    return false;
                }
                else
                {
                    // Space wasn't down previous frame, but now is
                    space_down = true;
                    return true;
                }
            }
            else
            {
                space_down = false;
                return false;
            }
        }

        public int char_width(char Character)
        {
            // Return width of given character. For characters other than the letters A to Z (i.e. space, and the digits 0 to 9),
            //the width of the letter A is returned. ord gives the ASCII/Unicode code for the given character.
            var index = Math.Max(0, ord(Character) - 65);
            return CHAR_WIDTH[index];
        }


        private void draw_text(string text, int y, int? x = null)
        {
            if (x is null)
            {
                // If no X pos specified, draw text in centre of the screen - must first work out total width of text
                x = (WIDTH - sum([char_width(c) for c in text])) / 2;
            }
        
            foreach(char c in text)
            {
                _spriteBatch.Draw(_Texture2Ds[$"font0{(ord(c))}"], new Vector2((int)x, y), Color.White);              
                x += char_width(c);
            }
 
        
        }

        public bool Block(int x, int y)
        {
            // Is there a level grid block at these coordinates?
            var grid_x = (x - LEVEL_X_OFFSET) / GRID_BLOCK_SIZE;
            var grid_y = y / GRID_BLOCK_SIZE;
            if (grid_y > 0 && grid_y < NUM_ROWS)
            {
                var row = Session.grid[grid_y];
                return (grid_x >= 0) && (grid_x < NUM_COLUMNS) && row.length > 0 && row[grid_x] != " ";
            }
            else
            {
                return false;
            }
        }


        private void draw_status()
        {
            // Display score, right-justified at edge of screen
            var number_width = CHAR_WIDTH[0];
            var s = $"{Session.player.score}";
            draw_text(s, 451, WIDTH - 2 - (number_width * s.Length));

            // Display level number
            draw_text($"LEVEL {Session.level + 1}", 451);
        
            // Display lives and health
            // We only display a maximum of two lives - if there are more than two, a plus symbol is displayed
            var lives_health = IMAGE_WIDTH["life"] * Math.Min(2, Session.player.lives);
            if (Session.player.lives > 2)
            {
                lives_health.append("plus");
            }

            if (Session.player.lives >= 0)
            {
                lives_health += IMAGE_WIDTH["health"] * Session.player.health;
            }


            var x = 0;
            foreach (image i in lives_health)
            {
                _spriteBatch.Draw(_Texture2Ds[image], new Vector2(x, 450), Color.White);
                
                x += IMAGE_WIDTH[image];
            }
        }
    }
}