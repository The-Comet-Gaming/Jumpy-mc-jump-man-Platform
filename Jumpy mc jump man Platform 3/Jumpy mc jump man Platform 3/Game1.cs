using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using MonoGame.Extended;
using MonoGame.Extended.Maps.Tiled;
using MonoGame.Extended.ViewportAdapters;
using System;
using System.Collections.Generic;
using ParticleEffects;

namespace Jumpy_mc_jump_man_Platform_3
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Camera2D camera = null;

        Emitter fireEmitter = null;

        List<Enemy> enemies = new List<Enemy>();
        List<BronzeLootPile> bronzeLootPiles = new List<BronzeLootPile>();
        List<SilverLootPile> silverLootPiles = new List<SilverLootPile>();
        List<GoldLootPile> goldLootPiles = new List<GoldLootPile>();
        List<HealthPack> healthPacks = new List<HealthPack>();

        Player player = null;

        //playerSpawn = 

        Sprite goal = null;
        Sprite playerSpawn = null;
        //Sprite bronzeLoot = null;

        SpriteFont agency_FB;

        Song gameMusic;

        SoundEffect zombieDeathSound;
        SoundEffectInstance zombieDeathSoundInstance;

        Texture2D heart;
        Texture2D healthPack;
        //Texture2D bronzeLoot;
        Texture2D silverLoot;
        Texture2D goldLoot;
        Texture2D fireTexture = null;

        TiledMap map = null;
        TiledMap backgound = null;
        TiledTileLayer collisionLayer;
        TiledObject playerSpawn1;

        //Vector2 playerSpawnPos = TiledObject playerSpawn1;

        int score = 0;
        int lives = 3;
        int timer = 3;

        public int ScreenHeight
        {
            get
            {
                return graphics.GraphicsDevice.Viewport.Height;
            }
        }

        public int ScreenWidth
        {
            get
            {
                return graphics.GraphicsDevice.Viewport.Width;
            }
        }

        public static int tile = 64;
        //arbitrary choice for 1m (1 tile = 1 meter)
        public static float meter = tile;
        //very exaggeratted gravity (6x)
        public static float gravity = meter * 9.8f * 6.0f;
        //max vertical speed (10 tiles/sec horizontal, 15 tiles sec/vertical)
        public static Vector2 maxVelocity = new Vector2(meter * 10, meter * 15);
        //horizontal acceleration - takes 1/2 second/s to reach max velocity.
        public static float acceleration = maxVelocity.X * 2;
        //horizontal friction - takes 1/2 second/s to reach max velocity.
        public static float friction = maxVelocity.X * 6;
        //(A large) instantaneous jump impulse
        public static float jumpImpulse = meter * 1500;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            player = new Player(this);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here

            AIE.StateManager.CreateState("SPLASH", new SplashState());
            AIE.StateManager.CreateState("GAME", new GameState());
            AIE.StateManager.CreateState("GAMEOVER", new GameOverState());

            AIE.StateManager.PushState("SPLASH");

            player.Load(Content);

            agency_FB = Content.Load<SpriteFont>("Agency_FB");
            heart = Content.Load<Texture2D>("Heart");
            //healthPack = Content.Load<Texture2D>("Healthpack");
            //bronzeLoot = Content.Load<Texture2D>("Bronze-Loot-Pile");
            //silverLoot = Content.Load<Texture2D>("Silver-Loot-Pile");
            //goldLoot = Content.Load<Texture2D>("Gold-Loot-Pile");

            zombieDeathSound = Content.Load<SoundEffect>("ZombieDeath");
            zombieDeathSoundInstance = zombieDeathSound.CreateInstance();

            var viewportAdaptor = new BoxingViewportAdapter(Window, GraphicsDevice, ScreenWidth, ScreenHeight);

            camera = new Camera2D(viewportAdaptor);

            camera.Position = player.Position - new Vector2(ScreenWidth / 2, ScreenHeight / 2);

            backgound = Content.Load<TiledMap>("Level3-background");
            map = Content.Load<TiledMap>("Level3");
            foreach (TiledTileLayer layer in map.TileLayers)
            {
                if (layer.Name == "Collisions")
                    collisionLayer = layer;

            }

            foreach (TiledObjectGroup group in map.ObjectGroups)
            {
                if (group.Name == "Enemies")
                {
                    foreach (TiledObject obj in group.Objects)
                    {
                        Enemy enemy = new Enemy(this);
                        enemy.Load(Content);
                        enemy.Position = new Vector2(obj.X, obj.Y);
                        enemies.Add(enemy);
                    }
                }

                else if (group.Name == "Bronze")
                {
                    foreach (TiledObject obj in group.Objects)
                    {
                        BronzeLootPile bronzeLootPile = new BronzeLootPile(this);
                        bronzeLootPile.Load(Content);
                        bronzeLootPile.Position = new Vector2(obj.X, obj.Y);
                        bronzeLootPiles.Add(bronzeLootPile);
                    }
                }

                else if (group.Name == "Silver")
                {
                    foreach (TiledObject obj in group.Objects)
                    {
                        SilverLootPile silverLootPile = new SilverLootPile(this);
                        silverLootPile.Load(Content);
                        silverLootPile.Position = new Vector2(obj.X, obj.Y);
                        silverLootPiles.Add(silverLootPile);
                    }
                }

                else if (group.Name == "Gold")
                {
                    foreach (TiledObject obj in group.Objects)
                    {
                        GoldLootPile goldLootPile = new GoldLootPile(this);
                        goldLootPile.Load(Content);
                        goldLootPile.Position = new Vector2(obj.X, obj.Y);
                        goldLootPiles.Add(goldLootPile);
                    }
                }

                else if (group.Name == "Healthpacks")
                {
                    foreach (TiledObject obj in group.Objects)
                    {
                        HealthPack healthPack = new HealthPack(this);
                        healthPack.Load(Content);
                        healthPack.Position = new Vector2(obj.X, obj.Y);
                        healthPacks.Add(healthPack);
                    }
                }

                else if (group.Name == "Goal")
                {
                    TiledObject obj = group.Objects[0];
                    if (obj != null)
                    {
                        AnimatedTexture anim = new AnimatedTexture(Vector2.Zero, 0, 1, 1);
                        anim.Load(Content, "chest", 1, 1);
                        goal = new Sprite();
                        goal.Add(anim, 0, 5);
                        goal.position = new Vector2(obj.X, obj.Y);
                    }
                }

                else if (group.Name == "PlayerSpawn")
                {
                    TiledObject obj = group.Objects[0];
                    if (obj != null)
                    {
                        player.Position = new Vector2(obj.X, obj.Y);
                    }
                }
            }

            // load the game music
            gameMusic = Content.Load<Song>("SuperHero_original(edited)");
            MediaPlayer.Play(gameMusic);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            AIE.StateManager.Update(Content, gameTime);

            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            player.Update(deltaTime);
            CheckCollisions();
            //fireEmitter.Update(gameTime);
            foreach (Enemy e in enemies)
            {
                e.Update(deltaTime);
            }
            foreach (BronzeLootPile b in bronzeLootPiles)
            {
                b.Update(deltaTime);
            }
            foreach (SilverLootPile s in silverLootPiles)
            {
                s.Update(deltaTime);
            }
            foreach (GoldLootPile g in goldLootPiles)
            {
                g.Update(deltaTime);
            }
            foreach (HealthPack h in healthPacks)
            {
                h.Update(deltaTime);
            }
            ///Atempt at updating gamestates to work with the game code I have. 
            ///Currently updates and draw seperate to the main draw and I am unsure of how to link them to work together.
            /*if (lives < 1)
            {
                AIE.StateManager.ChangeState("GAMEOVER");
            }
            if (lives == 0 && timer < 0)
            {

            }*/


            ///player.Position = playerSpawn;
            camera.Position = player.Position - new Vector2(ScreenWidth / 2, ScreenHeight / 2);

            // update the fire partical emitter
            ///fireEmitter.position = playerPosition;
            ///fireEmitter.minVelocity = new Vector2((playerSpeed * s), (playerSpeed * c));
            ///fireEmitter.maxVelocity = new Vector2((playerSpeed*2 * s), (playerSpeed*2 * c));


            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            var transformMatrix = camera.GetViewMatrix();

            AIE.StateManager.Draw(spriteBatch);

            if (lives > 0)
            {
                spriteBatch.Begin(transformMatrix: transformMatrix);
                backgound.Draw(spriteBatch);
                player.Draw(spriteBatch);
                foreach (Enemy e in enemies)
                {
                    e.Draw(spriteBatch);
                }
                foreach (BronzeLootPile b in bronzeLootPiles)
                {
                    b.Draw(spriteBatch);
                }
                foreach (SilverLootPile s in silverLootPiles)
                {
                    s.Draw(spriteBatch);
                }
                foreach (GoldLootPile g in goldLootPiles)
                {
                    g.Draw(spriteBatch);
                }
                foreach (HealthPack h in healthPacks)
                {
                    h.Draw(spriteBatch);
                }
                goal.Draw(spriteBatch);
                map.Draw(spriteBatch);

                spriteBatch.End();
            }

            //draw all the GUI compoments in a seperate SpriteBatch section
            spriteBatch.Begin();

            spriteBatch.DrawString(agency_FB, "Score :" + score.ToString(), new Vector2(20, 20), Color.Gold);

            for (int i = 0; i < lives; i++)
            {
                spriteBatch.Draw(heart, new Vector2(ScreenWidth - 80 - i * 20, 20), Color.White);
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }

        public int PixelToTile(float pixelCoord)
        {
            return (int)Math.Floor(pixelCoord / tile);
        }
        public int TileToPixel(int tileCoord)
        {
            return tile * tileCoord;
        }
        public int CellAtPixelCoord(Vector2 pixelCoord)
        {
            if (pixelCoord.X < 0 || pixelCoord.X > map.WidthInPixels || pixelCoord.Y < 0)
                return 1;
            // let the player drop to the bottom of the screen (this means death)
            if (pixelCoord.Y > map.HeightInPixels)
                return 0;
            return CellAtPixelCoord(PixelToTile(pixelCoord.X), PixelToTile(pixelCoord.Y));
        }
        public int CellAtPixelCoord(int tx, int ty)
        {
            if (tx < 0 || tx >= map.Width || ty < 0)
                return 1;
            // let the player drop to the bottom of the screen (this means death)
            if (ty >= map.Height)
                return 0;

            TiledTile tile = collisionLayer.GetTile(tx, ty);
            return tile.Id;
        }
        private void CheckCollisions()
        {
            foreach (BronzeLootPile b in bronzeLootPiles)
            {
                if (IsColliding(player.Bounds, b.Bounds) == true)
                {
                    score += 5;
                    bronzeLootPiles.Remove(b);
                    break;
                }
            }
            foreach (SilverLootPile s in silverLootPiles)
            {
                if (IsColliding(player.Bounds, s.Bounds) == true)
                {
                    score += 10;
                    silverLootPiles.Remove(s);
                    break;
                }
            }
            foreach (GoldLootPile g in goldLootPiles)
            {
                if (IsColliding(player.Bounds, g.Bounds) == true)
                {
                    score += 20;
                    goldLootPiles.Remove(g);
                    break;
                }
            }
            foreach (HealthPack h in healthPacks)
            {
                if (IsColliding(player.Bounds, h.Bounds) == true && lives < 3)
                {
                    lives += 1;
                    healthPacks.Remove(h);
                    break;
                }
            }
            foreach (Enemy e in enemies)
            {
                if (IsColliding(player.Bounds, e.Bounds) == true)
                {
                    if (player.IsJumping && player.Velocity.Y > 0 || player.Velocity.Y > 0)
                    {
                        player.JumpOnCollision();
                        enemies.Remove(e);
                        zombieDeathSoundInstance.Play();
                        score += 5;
                        break;
                    }
                    else
                    {
                        lives -= 1;
                        enemies.Remove(e);
                        break;
                    }
                }
            }
            if (IsColliding(player.Bounds, goal.Bounds) == true)
            {
                score += 100;
                foreach (TiledObjectGroup group in map.ObjectGroups)
                {
                    if (group.Name == "PlayerSpawn")
                    {
                        TiledObject obj = group.Objects[0];
                        if (obj != null)
                        {
                            player.Position = new Vector2(obj.X, obj.Y);
                            break;
                        }
                    }
                }
            }
        }

        //I made multiple attempts to link the gamestates with the main code properly.
        /*private void GameReset(float deltaTime)
        {
            if (lives == 0)
            {
                AIE.StateManager.ChangeState("GAMEOVER");
            }

        }*/

        private bool IsColliding(Rectangle rect1, Rectangle rect2)
        {
            if (rect1.X + rect1.Width < rect2.X || rect1.X > rect2.X + rect2.Width || rect1.Y + rect1.Height < rect2.Y || rect1.Y > rect2.Y + rect2.Height)
            {
                //these two rectangles are not colliding
                return false;
            }
            //else, the two AABB rectangles overlap, therefore collision
            return true;
        }
    }
}
