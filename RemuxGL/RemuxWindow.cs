using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.IO;
using RemuxGL.StateSystem;
using Emux.GameBoy.Input;

namespace RemuxGL
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class RemuxWindow : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        public StateManager StateManager { get; private set; }

        public int WndWidth
        {
            get => graphics.PreferredBackBufferWidth;
        }

        public int WndHeight
        {
            get => graphics.PreferredBackBufferHeight;
        }
        
        public RemuxWindow()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 600;
        }

        public void SetWindowSize(int w, int h)
        {
            graphics.PreferredBackBufferWidth = w;
            graphics.PreferredBackBufferHeight = h;
        }

        protected override void Initialize()
        {
            base.Initialize();
            StateManager = new StateManager(this);
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            StateManager.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();
            StateManager.Draw(gameTime, spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
