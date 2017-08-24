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
        public RemuxWindow Game { get; set; }
        private Stack<IState> backStack = new Stack<IState>();

        public StateManager(Game game)
        {
            Game = (RemuxWindow)game;
            TransitionTo(new AsciiFileBrowserScreen(Game));
        }

        public void TransitionTo(IState nextState)
        {
            nextState.LoadContent(Game.Content);
            backStack.Push(CurrentState);
            CurrentState = nextState;
            Game.Window.Title = "Remux (State: " + CurrentState.GetType().Name + ")";
        }

        public void ClearBackstack()
        {
            backStack.Clear();
        }

        public void TransitionBack()
        {
            IState lastState = backStack.Pop();
            IState currentState = CurrentState;
            currentState.UnloadContent();
            CurrentState = lastState;
        }

        public void TransitionToIgnoreCurrent(IState nextState)
        {
            nextState.LoadContent(Game.Content);
            CurrentState = nextState;
            Game.Window.Title = "Remux (State: " + CurrentState.GetType().Name + ")";
        }

        public void Draw(GameTime GameTime, SpriteBatch spriteBatch)
        {
            try
            {
                CurrentState.Draw(GameTime, spriteBatch);
            }
            catch (Exception e)
            {
                TransitionTo(new PanicScreen(Game, "SOMETHING BROKE!!!! >:|\n\nRemux caught an exception: " + e.Message + $"\n\n    Previous State: {CurrentState.GetType().Name}\n\n    Method: \"{new StackTrace().GetFrame(1).GetMethod().Name}()\"", e));
            }
        }

        public void Update(GameTime GameTime)
        {
            try
            {
                CurrentState.Update(GameTime);
            }
            catch (Exception e)
            {
                TransitionTo(new PanicScreen(Game, "SOMETHING BROKE!!!! >:|\n\nRemux caught an exception: " + e.Message + $"\n\n    Previous State: {CurrentState.GetType().Name}\n\n    Method: \"{new StackTrace().GetFrame(1).GetMethod().Name}()\"", e));
            }
        }

        public void LoadContent(ContentManager content) { }
        public void UnloadContent() { }
    }
}
