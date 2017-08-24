using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emux.GameBoy.Input
{
    
    public class IoManager
    {
        private static KeyboardState _keyboardState;
        public static void Update()
        {
            _keyboardState = Keyboard.GetState();

        }
    }
}
