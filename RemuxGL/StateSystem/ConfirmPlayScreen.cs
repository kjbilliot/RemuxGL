using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace RemuxGL.StateSystem
{
    public class ConfirmPlayScreen : IState
    {
        private string romPath, romName;
        private SpriteFont font;
        private RemuxWindow wnd;
        private Vector2[] possiblePointerPositions =
        {
            new Vector2(300, 242),
            new Vector2(460, 242)
        };
        private int pointerPosition = 0;
        private bool canUseEnter = false;

        public ConfirmPlayScreen(string fname, RemuxWindow wnd)
        {
            this.wnd = wnd;
            romPath = fname;
            string fileSeparator = OsUtil.IsWindows() ? "\\" : "/";
            romName = fname.Substring(fname.LastIndexOf(fileSeparator));
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(font, "This may not a GameBoy game!", new Vector2(275, 150), Color.Red);
            spriteBatch.DrawString(font, "Are you sure you want to try playing this file?", new Vector2(230, 175), Color.Red);
            int fifthX = wnd.WndWidth / 5;
            int fifthY = wnd.WndHeight / 5;
            spriteBatch.DrawString(font, "No", new Vector2(fifthX*2, fifthY*2), Color.White, 0f, new Vector2(0, 0), 1.2f, SpriteEffects.None, 0f);
            spriteBatch.DrawString(font, "Yes", new Vector2(fifthX*3, fifthY*2), Color.White, 0f, new Vector2(0, 0), 1.2f, SpriteEffects.None, 0f);
            spriteBatch.DrawString(font, ">", possiblePointerPositions[pointerPosition], Color.White);
        }

        public void LoadContent(ContentManager content)
        {
            font = content.Load<SpriteFont>("MainFont");
        }

        public void UnloadContent()
        {

        }

        public void Update(GameTime gameTime)
        {
            if (IsPressed(Keys.Right)) pointerPosition++;
            if (IsPressed(Keys.Left)) pointerPosition--;
            if (!IsPressed(Keys.Enter)) canUseEnter = true;
            if      (pointerPosition > 1) pointerPosition = 1;
            else if (pointerPosition < 0) pointerPosition = 0;
            if (IsPressed(Keys.Enter) && canUseEnter)
            {
                if (pointerPosition == 1) wnd.StateManager.TransitionTo(new GameboyPlayScreen(romPath, wnd));
                else wnd.StateManager.TransitionBack();
            }
        }

        private bool IsPressed(params Keys[] keys)
        {
            bool pressed = true;
            foreach (Keys k in keys)
            {
                if (!Keyboard.GetState().IsKeyDown(k))
                {
                    pressed = false;
                    break;
                }
            }
            return pressed;
        }
    }
}
