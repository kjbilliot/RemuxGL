using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;

namespace Emux.GameBoy.Input
{
    public class GameBoyInputDefinition
    {
        public bool IsPressed
        {
            get
            { 
                if (_button != 0)
                {
                    return Keyboard.GetState().IsKeyDown(_key) || GamePad.GetState(Microsoft.Xna.Framework.PlayerIndex.One).IsButtonDown(_button);
                }
                else
                {
                    return Keyboard.GetState().IsKeyDown(_key);
                }
            }
        }
            
        private Keys _key;        
        private Buttons _button;

        public GameBoyInputDefinition(Keys key) => _key = key;
        public void AddGamepadBinding(Buttons button) => _button = button;
       
        public override string ToString()
        {
            return _key != Keys.None ? _key.ToString() : _button.ToString();
        }
    }
}
