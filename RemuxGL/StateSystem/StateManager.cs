using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace RemuxGL.StateSystem
{
    public class StateManager : IState
    {
        public IState CurrentState { get; set; }
        private Game game { get; set; }

        public StateManager(Game game)
        {
            this.game = game;
            TransitionTo(new AsciiFileBrowserScreen(game));
        }

        public void TransitionTo(IState nextState)
        {
            nextState.LoadContent(game.Content);
            // Transition effects?
            CurrentState = nextState;
            game.Window.Title = "Remux (State: " + CurrentState.GetType().Name + ")";
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            CurrentState.Draw(gameTime, spriteBatch);
            try
            {
            }
            catch (Exception e)
            {
                TransitionTo(new PanicScreen(game, "SOMETHING BROKE!!!! >:|\n\nRemux caught an exception: " + e.Message + $"\n\n    Previous State: {CurrentState.GetType().Name}\n\n    Method: \"{new StackTrace().GetFrame(1).GetMethod().Name}()\"", e));
            }
        }

        public void Update(GameTime gameTime)
        {
            try
            {
                CurrentState.Update(gameTime);
            }
            catch (Exception e)
            {
                TransitionTo(new PanicScreen(game, "SOMETHING BROKE!!!! >:|\n\nRemux caught an exception: " + e.Message + $"\n\n    Previous State: {CurrentState.GetType().Name}\n\n    Method: \"{new StackTrace().GetFrame(1).GetMethod().Name}()\"", e));
            }
        }

        public void LoadContent(ContentManager content) { }
        public void UnloadContent() { }
    }
}
