using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Emux.GameBoy;
using Emux.GameBoy.Cartridge;
using System.IO;
using Emux.GameBoy.Graphics;
using System.Diagnostics;
using Color = Microsoft.Xna.Framework.Color;
using Emux.GameBoy.Input;

namespace RemuxGL.StateSystem
{
    public class GameboyPlayScreen : IState, IVideoOutput
    {
        private string romFile;
        private GameBoy vm;
        private byte[] frameBuffer;
        private Stopwatch sw = new Stopwatch();
        private SpriteFont font;
        private RemuxWindow wnd;
        private IoManager ioman;

        public GameboyPlayScreen(string fname, RemuxWindow wnd)
        {
            romFile = fname;
            vm = new GameBoy(new EmulatedCartridge(File.ReadAllBytes(fname)));
            vm.Gpu.VideoOutput = this;
            vm.Cpu.Run();
            this.wnd = wnd;
            ioman = new IoManager(vm);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            sw.Reset();
            sw.Start();
            string fpsOutput = $"Render Time: {sw.ElapsedMilliseconds}ms\n";
            if (frameBuffer != null)
            {
                fpsOutput += $"VM GPU Render Speed: {vm.Cpu.FramesPerSecond:0.00} FPS\n";
                fpsOutput += $"Speed Accuracy: {((vm.Cpu.FramesPerSecond/60)*100):0.00}%";
                Texture2D bufferImage = new Texture2D(wnd.GraphicsDevice, 160, 144);
                bufferImage.SetData(GetArgbFrameBuffer());
                spriteBatch.Draw(bufferImage, new Rectangle(0, 0, bufferImage.Width*2, bufferImage.Height*2), Color.Olive);
            }
            spriteBatch.DrawString(font, fpsOutput, new Vector2(5, 5), Color.White, 0f, new Vector2(0, 0), 0.8f, SpriteEffects.None, 0.0f);
            sw.Stop();
        }

        private byte[] GetArgbFrameBuffer()
        {
            List<byte> data = new List<byte>();
            for (int i = 0; i < frameBuffer.Length; i += 3)
            {
                data.Add(frameBuffer[i]);
                data.Add(frameBuffer[i+1]);
                data.Add(frameBuffer[i+2]);
                data.Add(0xff);
            }
            return data.ToArray();
        }

        public void LoadContent(ContentManager content)
        {
            font = content.Load<SpriteFont>("MainFont");
        }

        public void RenderFrame(byte[] pixelData)
        {
            frameBuffer = pixelData;
        }

        public void UnloadContent()
        {
        }

        public void Update(GameTime gameTime)
        {
        }
    }
}
