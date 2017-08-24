using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace RemuxGL.StateSystem
{
    public class PanicScreen : IState
    {

        private Texture2D blueRectangle;
        private SpriteFont font;
        private string msg;
        private Texture2D bugTexture;
        private const int BugCount = 0; 
        private Vector2[] positions = new Vector2[BugCount];
        private Vector2[] velocities = new Vector2[BugCount];
        private RemuxWindow wnd;
        private Random r = new Random();

        public PanicScreen(Game g, string msg, Exception e)
        {
            blueRectangle = new Texture2D(g.GraphicsDevice, 3000, 3000);
            Color[] color = new Color[blueRectangle.Width * blueRectangle.Height];
            for (int i = 0; i < color.Length; i++) color[i] = Color.Blue;
            blueRectangle.SetData(color);
            wnd = (RemuxWindow)g;
            this.msg = msg;
            for (int i = 0; i < positions.Length; i++)
            {
                positions[i] = new Vector2(r.Next(wnd.WndWidth), r.Next(wnd.WndHeight));
                int num1 = r.Next(6) + 3;
                int num2 = r.Next(6) + 3;
                if (r.Next(2) == 1) num1 *= -1;
                if (r.Next(2) == 1) num2 *= -1;
                velocities[i] = new Vector2(num1, num2);
            }
            Console.WriteLine(e.Message);
            Console.WriteLine(e.StackTrace);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(blueRectangle, new Rectangle(0, 0, blueRectangle.Width, blueRectangle.Height), Color.White);
            for (int i = 0; i < positions.Length; i++)
                spriteBatch.Draw(bugTexture, positions[i], null, Color.White, 0.0f, new Vector2(0, 0), 0.1f, SpriteEffects.None, 0.0f);
            spriteBatch.DrawString(font, msg, new Vector2(5, 5), Color.White);
        }

        public void LoadContent(ContentManager content)
        {
            font = content.Load<SpriteFont>("MainFont");
            bugTexture = content.Load<Texture2D>("animal");
        }

        public void UnloadContent()
        {
        }

        public void Update(GameTime gameTime)
        {
            for (int i = 0; i < positions.Length; i++)
            {
                Vector2 pos = positions[i];
                if (pos.X < 0 || pos.X >= wnd.WndWidth - (bugTexture.Width*0.1) - 1) velocities[i].X *= -1;
                if (pos.Y < 0 || pos.Y >= wnd.WndHeight - (bugTexture.Height*0.1) -1) velocities[i].Y *= -1;
                positions[i].X += velocities[i].X;
                positions[i].Y += velocities[i].Y;
            }
        }
    }
}
