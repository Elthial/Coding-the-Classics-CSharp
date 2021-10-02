using Cavern.Actors;
using CodingClassics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cavern
{
    class GameSession
    {       
        public Player player;
        private int level_colour;
        public int level;
        private int grid;
        public int timer;
        private List<int> Pending_enemies;
        private List<Fruit> Fruits;
        private List<Bolt> Bolts;
        private List<Robot> Enemies;
        private List<Pop> Pops;
        public List<Orb> Orbs;

        private readonly SpriteBatch _spriteBatch;
        private readonly Dictionary<string, Texture2D> _Texture2Ds;
        private readonly Dictionary<string, SoundEffect> _soundeffects;

        public GameSession(Cavern cavern, Player player = null)
        {
            //Setup of XNA systems
            _spriteBatch = cavern._spriteBatch;
            _Texture2Ds = cavern._Texture2Ds;
            _soundeffects = cavern._soundeffects;

            this.player = player;
            this.level_colour = -1;
            this.level = -1;
            this.next_level();
        }

        public float fire_probability()
        {
            //Likelihood per frame of each robot firing a bolt - they fire more often on higher levels
            return 0.001f + (0.0001f * Math.Min(100, this.level));
        }

        public int max_enemies()
        {
            //Maximum number of enemies on-screen at once – increases as you progress through the levels
            return Math.Min((this.level + 6) / 2, 8);
        }

        public void next_level()
        {
            this.level_colour = (this.level_colour + 1) % 4;
            this.level += 1;

            //Set up grid
            this.grid = Cavern.LEVELS[this.level % (Cavern.LEVELS).Length];

            //# The last row is a copy of the first row
            //# Note that we don't do 'self.grid.append(self.grid[0])'. That would alter the original data in the LEVELS list
            //# Instead, what this line does is create a brand new list, which is distinct from the list in LEVELS, and
            //# consists of the level data plus the first row of the level. It's also interesting to note that you can't
            //# do 'self.grid += [self.grid[0]]', because that's equivalent to using append.
            //# As an alternative, we could have copied the list on the line below '# Set up grid', by writing
            //# 'self.grid = list(LEVELS...', then used append or += on the line below.
            this.grid = this.grid + [this.grid[0]];

            this.timer = -1;

            if (!(this.player is null))
                this.player.reset();

            this.Fruits = new List<Fruit>();
            this.Bolts = new List<Bolt>();
            this.Enemies = new List<Robot>();
            this.Pops = new List<Pop>();
            this.Orbs = new List<Orb>();

            //# At the start of each level we create a list of pending enemies - enemies to be created as the level plays out.
            //# When this list is empty, we have no more enemies left to create, and the level will end once we have destroyed
            //# all enemies currently on-screen. Each element of the list will be either 0 or 1, where 0 corresponds to
            //# a standard enemy, and 1 is a more powerful enemy.
            //# First we work out how many total enemies and how many of each type to create
            var num_enemies = 10 + this.level;
            var num_strong_enemies = 1 + (int)(this.level / 1.5);
            var num_weak_enemies = num_enemies - num_strong_enemies;

            //Then we create the list of pending enemies, using Python's ability to create a list by multiplying a list
            //by a number, and by adding two lists together. The resulting list will consist of a series of copies of
            //the number 1 (the number depending on the value of num_strong_enemies), followed by a series of copies of
            //the number zero, based on num_weak_enemies.
            for (int i = 0; i < num_strong_enemies; i++)
            {
                this.Pending_enemies.Add((int)RobotType.TYPE_AGGRESSIVE);
            }

            for (int i = 0; i < num_weak_enemies; i++)
            {
                this.Pending_enemies.Add((int)RobotType.TYPE_NORMAL);
            }

            //Finally we shuffle the list so that the order is randomised (using Python's random.shuffle function)
            Helpers.Shuffle(this.Pending_enemies);

            play_sound("level", 1);
        }

        public int get_robot_spawn_x()
        {
            //Find a spawn location for a robot, by checking the top row of the grid for empty spots
            //Start by choosing a random grid column
            var r = Helpers.RandInt(0, Cavern.NUM_COLUMNS - 1);

            for (int i = 0; i < Cavern.NUM_COLUMNS; i++)
            {
                //# Keep looking at successive columns (wrapping round if we go off the right-hand side) until
                //# we find one where the top grid column is unoccupied
                var grid_x = (r + i) % Cavern.NUM_COLUMNS;
                if (grid[0][grid_x] == ' ')
                {
                    return Cavern.GRID_BLOCK_SIZE * grid_x + Cavern.LEVEL_X_OFFSET + 12;
                }
            }

            //If we failed to find an opening in the top grid row (shouldn't ever happen), just spawn the enemy
            //in the centre of the screen
            return Cavern.WIDTH / 2;
        }

        public void Update()
        {
            this.timer += 1;

            //# Update all objects
            //        for obj in self.fruits + self.bolts + self.enemies + self.pops + [self.player] + self.orbs:
            //            if obj:
            //                obj.update()

            //# Use list comprehensions to remove objects which are no longer wanted from the lists. For example, we recreate
            //# self.fruits such that it contains all existing fruits except those whose time_to_live counter has reached zero
            //        self.fruits = [f for f in self.fruits if f.time_to_live > 0]
            //        self.bolts = [b for b in self.bolts if b.active]
            //        self.enemies = [e for e in self.enemies if e.alive]
            //        self.pops = [p for p in self.pops if p.timer < 12]
            //        self.orbs = [o for o in self.orbs if o.timer < 250 and o.y > -40]

            //# Every 100 frames, create a random fruit (unless there are no remaining enemies on this level)
            //        if self.timer % 100 == 0 and len(self.pending_enemies + self.enemies) > 0:
            //            # Create fruit at random position
            //            self.fruits.append(Fruit((randint(70, 730), randint(75, 400))))

            //# Every 81 frames, if there is at least 1 pending enemy, and the number of active enemies is below the current
            //# level's maximum enemies, create a robot
            //        if self.timer % 81 == 0 and len(self.pending_enemies) > 0 and len(self.enemies) < self.max_enemies():
            //            # Retrieve and remove the last element from the pending enemies list
            //            robot_type = self.pending_enemies.pop()
            //            pos = (self.get_robot_spawn_x(), -30)
            //            self.enemies.append(Robot(pos, robot_type))

            //# End level if there are no enemies remaining to be created, no existing enemies, no fruit, no popping orbs,
            //# and no orbs containing trapped enemies. (We don't want to include orbs which don't contain trapped enemies,
            //# as the level would never end if the player kept firing new orbs)
            //        if len(self.pending_enemies + self.fruits + self.enemies + self.pops) == 0:
            //            if len([orb for orb in self.orbs if orb.trapped_enemy_type != None]) == 0:
            //                self.next_level()
        }

        public void Draw()
        {
            //Draw appropriate background for this level
            //        screen.blit("bg%d" % self.level_colour, (0, 0))

            var block_sprite = $"block{this.level % 4}";

            //Display blocks
            for (int row_y = 0; row_y < Cavern.NUM_ROWS; row_y++)
            {
                var row = this.grid[row_y];
                if (row.length > 0)
                {
                    //# Initial offset - large blocks at edge of level are 50 pixels wide
                    var x = Cavern.LEVEL_X_OFFSET;
                    for (int block = 0; block < row; block++)
                    {
                        if (block != ' ')
                        {
                            //screen.blit(block_sprite, (x, row_y* GRID_BLOCK_SIZE))
                            x += Cavern.GRID_BLOCK_SIZE;
                        }
                    }
                }
            }

            //Draw all objects
            //        all_objs = self.fruits + self.bolts + self.enemies + self.pops + self.orbs
            //        all_objs.append(self.player)
            //        for obj in all_objs:
            //            if obj:
            //                obj.draw()
        }

        public void play_sound(string name, int count = 1)
        {
            //# Some sounds have multiple varieties. If count > 1, we'll randomly choose one from those
            //# We don't play any sounds if there is no player (e.g. if we're on the menu)
            if (!(this.player is null))
            {
                //# Pygame Zero allows you to write things like 'sounds.explosion.play()'
                //# This automatically loads and plays a file named 'explosion.wav' (or .ogg) from the sounds folder (if
                //# such a file exists)
                //# But what if you have files named 'explosion0.ogg' to 'explosion5.ogg' and want to randomly choose
                //# one of them to play? You can generate a string such as 'explosion3', but to use such a string
                //# to access an attribute of Pygame Zero's sounds object, we must use Python's built-in function getattr
                try
                {
                    var rand = new Random().Next(0, count - 1);
                    _soundeffects[$"{name}{rand}"].Play();
                }
                catch (System.Exception)
                {
                    throw;
                }
            }
        }        
    }
}