using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Emux.GameBoy.Input.GameBoyPadButton;

namespace Emux.GameBoy.Input
{
    
    public class IoManager
    {
        public static Dictionary<GameBoyPadButton, GameBoyInputDefinition> InputMap = new Dictionary<GameBoyPadButton, GameBoyInputDefinition>()
        {
            {Up,     new GameBoyInputDefinition(Keys.Up)        },
            {Down,   new GameBoyInputDefinition(Keys.Down)      },
            {Left,   new GameBoyInputDefinition(Keys.Left)      },
            {Right,  new GameBoyInputDefinition(Keys.Right)     },
            {A,      new GameBoyInputDefinition(Keys.X)         },
            {B,      new GameBoyInputDefinition(Keys.Z)         },
            {Start,  new GameBoyInputDefinition(Keys.Enter)     },
            {Select, new GameBoyInputDefinition(Keys.LeftShift) },
        };
        private GameBoy vm;
        private GamePadCapabilities capabilities;
        public IoManager(GameBoy vm)
        {
            capabilities = GamePad.GetCapabilities(Microsoft.Xna.Framework.PlayerIndex.One);
            if (capabilities.IsConnected)
                AddGamePadBindings();
            this.vm = vm;
        }
        private void AddGamePadBindings()
        {
            InputMap[Up].AddGamepadBinding(Buttons.DPadUp);
            InputMap[Down].AddGamepadBinding(Buttons.DPadDown);
            InputMap[Left].AddGamepadBinding(Buttons.DPadLeft);
            InputMap[Right].AddGamepadBinding(Buttons.DPadRight);
            InputMap[A].AddGamepadBinding(Buttons.A);
            InputMap[B].AddGamepadBinding(Buttons.B);
            InputMap[Start].AddGamepadBinding(Buttons.Start);
            InputMap[Select].AddGamepadBinding(Buttons.Back);
        }

        public void Update()
        {
            foreach (GameBoyPadButton k in InputMap.Keys)
            {
                if (InputMap[k].IsPressed) vm.KeyPad.PressedButtons |= k;
                else vm.KeyPad.PressedButtons &= ~k;
            }
        }
    }
}
