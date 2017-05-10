using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Jumpy_mc_jump_man_Platform_3
{
    public class GameOverState : AIE.State
    {
        bool isLoaded = false;
        SpriteFont font = null;

        KeyboardState oldState;

        public GameOverState() : base()
        {

        }

        public override void Update(ContentManager content, GameTime gameTime)
        {
            if (isLoaded == false)
            {
                isLoaded = true;
                font = content.Load<SpriteFont>("Agency_FB");
                oldState = Keyboard.GetState();
            }

            KeyboardState newState = Keyboard.GetState();

            if (newState.IsKeyDown(Keys.Enter) == true)
            {
                //checking the oldstate ensures that we're only checking the new key
                //presses (incase there was one key held down from the last state
                if (oldState.IsKeyDown(Keys.Enter) == false)
                {
                    AIE.StateManager.ChangeState("SPLASH");
                }
            }
            oldState = newState;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.DrawString(font, "Game Over...", new Vector2(200, 200), Color.White);
            spriteBatch.End();
        }

        public override void CleanUp()
        {
            font = null;
            isLoaded = false;
        }
    }
}
