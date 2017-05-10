using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Jumpy_mc_jump_man_Platform_3
{
    public class GameState : AIE.State
    {
        bool isLoaded = false;
        SpriteFont font = null;

        public GameState() : base()
        {
        }

        public override void Update(ContentManager content, GameTime gameTime)
        {
            if (isLoaded == false)
            {
                isLoaded = true;
                font = content.Load<SpriteFont>("Agency_FB");
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Enter) == true)
            {
                AIE.StateManager.ChangeState("GAMEOVER");
            }
        }

        public override void Draw(SpriteBatch spritebatch)
        {
            spritebatch.Begin();
            spritebatch.DrawString(font, "Game State", new Vector2(200, 200), Color.White);
            spritebatch.End();
        }

        public override void CleanUp()
        {
            font = null;
            isLoaded = false;
        }
    }
}
