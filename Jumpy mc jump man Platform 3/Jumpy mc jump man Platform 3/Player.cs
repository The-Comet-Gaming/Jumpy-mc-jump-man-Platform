using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace Jumpy_mc_jump_man_Platform_3
{
    class Player
    {
        Sprite sprite = new Sprite();
        // need to  keep a refernce to the game object so we can check for collisions on the map
        Game1 game = null;
        bool isFalling = true;
        bool isJumping = false;
        bool autoJump = true;

        Vector2 position = Vector2.Zero;
        Vector2 velocity = Vector2.Zero;

        //Jump Sound and Instance
        SoundEffect jumpSound;
        SoundEffectInstance jumpSoundInstance;

        public Vector2 Position
        {
            get { return sprite.position; }
            set { sprite.position = value; }
        }

        public Vector2 Velocity
        {
            get { return velocity; }
        }

        public Rectangle Bounds
        {
            get { return sprite.Bounds; }
        }

        public bool IsJumping
        {
            get { return isJumping; }
        }
        
        public void JumpOnCollision()
        {
            autoJump = true;
        }


            KeyboardState state;

            public Player(Game1 game)
            {
            this.game = game;
            isFalling = true;
            isJumping = false;
            Vector2 position = Vector2.Zero;
            Vector2 velocity = Vector2.Zero;
        }
        
        public void Load(ContentManager content)
            {
            AnimatedTexture animation = new AnimatedTexture(Vector2.Zero, 0, 1, 1);
            animation.Load(content, "walk", 12, 20);

            jumpSound = content.Load<SoundEffect>("Jump(edited)");
            jumpSoundInstance = jumpSound.CreateInstance();

            sprite.Add(animation, 0, -5);
            sprite.Pause();
            }

            public void Update(float deltaTime)
            {
                UpdateInput(deltaTime);
                sprite.Update(deltaTime);
            }
        public void UpdateInput(float deltaTime)
        {
            bool wasMovingleft = velocity.X < 0;
            bool wasMovingRight = velocity.X > 0;
            bool falling = isFalling;

            Vector2 acceleration = new Vector2(0, Game1.gravity);

            if (Keyboard.GetState().IsKeyDown(Keys.Left) == true)
            {
                acceleration.X -= Game1.acceleration;
                sprite.SetFlipped(true);
                sprite.Play();
            }
            else if (wasMovingleft == true)
            {
                acceleration.X += Game1.friction;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Right) == true)
            {
                acceleration.X += Game1.acceleration;
                sprite.SetFlipped(false);
                sprite.Play();
            }
            else if (wasMovingRight == true)
            {
                acceleration.X -= Game1.friction;
            }
            if ((Keyboard.GetState().IsKeyDown(Keys.Space) == true && this.isJumping == false && falling == false) || autoJump == true)
            {
                autoJump = false;
                acceleration.Y -= Game1.jumpImpulse;
                this.isJumping = true;
                jumpSoundInstance.Play();
            }

            // intergrate the forces to calculate the new position and velocity
            velocity += acceleration * deltaTime;

            //clamp the velocity so the player doesn't go to fast
            velocity.X = MathHelper.Clamp(velocity.X, -Game1.maxVelocity.X, Game1.maxVelocity.X);

            velocity.Y = MathHelper.Clamp(velocity.Y, -Game1.maxVelocity.Y, Game1.maxVelocity.Y);

            sprite.position += velocity * deltaTime;
            // more code will follow

            if ((wasMovingleft && (velocity.X > 0)) || (wasMovingRight && (velocity.X < 0)))
            {
                //clamp at zero to prevent friction from making us jiggle from side to side
                velocity.X = 0;
                sprite.Pause();
            }

            //collision detection
            //Our collision detection is grealy simplified by the fact that the player is a rectangle and is exactly the same size as a signle tile.
            //So we know that the player can only ever occupy 1, 2 or 4 cells.
            //This means we can short-circuit and avoid building a general 
            //purpose collision detection engine by simply looking at the 1 to 4 cells that the play ocupies.
            int tx = game.PixelToTile(sprite.position.X);
            int ty = game.PixelToTile(sprite.position.Y);
            //nx = true if the player overlaps right
            bool nx = (sprite.position.X) % Game1.tile != 0;
            //ny = true if the player overlaps below
            bool ny = (sprite.position.Y) % Game1.tile != 0;
            bool cell = game.CellAtPixelCoord(tx, ty) != 0;
            bool cellright = game.CellAtPixelCoord(tx + 1, ty) != 0;
            bool celldown = game.CellAtPixelCoord(tx, ty + 1) != 0;
            bool celldiag = game.CellAtPixelCoord(tx + 1, ty + 1) != 0;

            //If the player has vertical velocity, then check to see if they've hit a platform above or below, 
            //in which case, stop their vertical velocity and clamp their y position.
            if (this.velocity.Y > 0)
            {
                if ((celldown && !cell) || (celldiag && !cellright && nx))
                {
                    //clamp the y position and stop the player from falling into the platform below.
                    sprite.position.Y = game.TileToPixel(ty);
                    this.velocity.Y = 0;    //stop downward velocity 
                    this.isFalling = false; //no longer falling
                    this.isJumping = false; //or jumping
                    ny = false;             // - no longer overlaps the cells below
                }
            }
            else if (this.velocity.Y < 0)
            {
                if ((cell && !celldown) || (cellright && !celldiag && nx))
                {
                    //clamp the y position to avoid falling into platform below
                    sprite.position.Y = game.TileToPixel(ty + 1);
                    this.velocity.Y = 0;    //stop upward velocity 
                    //the player is no longer really in that cell,
                    //we clamp them to the cell below
                    cell = celldown;
                    cellright = celldiag;   //(ditto)
                    ny = false;             // - no longer overlaps the cells below
                }
            }
            //Once the vertical  velocity is taken care of,
            //we can apply similar logic to the horizontal: 
            if (this.velocity.X > 0)
            {
                if ((cellright && !cell) || (celldiag && !celldown && ny))
                {
                    //clamp the x position and stop the player from falling into the platform we just hit.
                    sprite.position.X = game.TileToPixel(tx);
                    this.velocity.X = 0;    //stop horizontal velocity 
                    sprite.Pause();
                }
            }
            else if (this.velocity.X < 0)
            {
                if ((cell && !cellright) || (celldown && !celldiag && ny))
                {
                    //clamp the y position to avoid falling into platform below
                    sprite.position.X = game.TileToPixel(tx + 1);
                    this.velocity.X = 0;    //stop horizontal velocity 
                    sprite.Pause();
                }
            }
            //the last calculation for our update() method is to dectect
            //if the player is now falling or not. We can do that by looking to see if 
            //there is a platform below them
            this.isFalling = !(celldown || (nx && celldiag));
        }
            public void Draw(SpriteBatch spriteBatch)
            {
                sprite.Draw(spriteBatch);
            }
        }
    }
