using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Jumpy_mc_jump_man_Platform_3
{
    class Enemy
    {
        Sprite sprite = new Sprite();

        KeyboardState state;

        public Enemy()
        {

        }

        public void Load(ContentManager content)
        {
            sprite.Load(content, "hero");
        }

        public void Update(float deltaTime)
        {
            state = Keyboard.GetState();

            /*  if (state.IsKeyDown(Keys.Right))
                  sprite.position.X += 5;
              if (state.IsKeyDown(Keys.Left))
                  sprite.position.X -= 5;
              if (state.IsKeyDown(Keys.Up))
                  sprite.position.Y -= 5;
              if (state.IsKeyDown(Keys.Down))
                  sprite.position.Y += 5; */

            sprite.Update(deltaTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            sprite.Draw(spriteBatch);
        }
    }
}
