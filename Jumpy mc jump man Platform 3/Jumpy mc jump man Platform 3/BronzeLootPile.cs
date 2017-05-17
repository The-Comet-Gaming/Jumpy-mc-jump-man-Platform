using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Jumpy_mc_jump_man_Platform_3
{
    class BronzeLootPile
    {       
        //keep a reference to the Game object to check for collisions on the map
        Game1 game = null;

        Sprite sprite = new Sprite();

        Texture2D bronzeLoot;

        Vector2 position = Vector2.Zero;
        Vector2 velocity = Vector2.Zero;

        public Vector2 Position
        {
            get { return sprite.position; }
            set { sprite.position = value; }
        }

        public Rectangle Bounds
        {
            get { return sprite.Bounds; }
        }

        public BronzeLootPile(Game1 game)
        {
            this.game = game;
            position = Vector2.Zero;
            velocity = Vector2.Zero;
        }

        public void Load(ContentManager content)
        {
            AnimatedTexture animation = new AnimatedTexture(Vector2.Zero, 0, 1, 1);
            animation.Load(content, "Bronze-Loot-Pile", 1, 5);

            sprite.Add(animation, 0, 0);
        }

        public void Update(float deltaTime)
        {
            sprite.Update(deltaTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            sprite.Draw(spriteBatch);
        }
    }
}
