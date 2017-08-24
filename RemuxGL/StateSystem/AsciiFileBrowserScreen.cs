using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

namespace RemuxGL.StateSystem
{
    public class AsciiFileBrowserScreen : IState
    {
        private const int LineSpacing = 25;
        private Game game;
        private SpriteFont font;
        private string currentDirectory = Directory.GetCurrentDirectory();
        private List<string> currentDirectoryContents;
        private List<string> subDirectories;
        private string pointerCharacter = ">";
        private List<string> displayList = new List<string>();
        private List<int> gbFiles = new List<int>();
        private int pointerIndex = 0;
        private Texture2D blueRectangle;
        private const int MaxPointerIndex = 22;
        private float gbGameScalar = 1.0f;
        private float gbGameScalarVelocity = 0.005f;
        private const float MaxGbScalar = 1.1f;
        private const float MinGbScalar = 0.98f;

        public AsciiFileBrowserScreen(Game game)
        {
            this.game = game;
            game.IsMouseVisible = true;
            blueRectangle = new Texture2D(game.GraphicsDevice, 3000, 3000);
            Color[] blueData = new Color[blueRectangle.Width * blueRectangle.Height];
            for (int i = 0; i < blueData.Length; i++) blueData[i] = Color.Blue;
            blueRectangle.SetData(blueData);
            RefreshDirectory();
        }

        public void RefreshDirectory()
        {
            if (subDirectories == null) subDirectories = new List<string>();
            if (currentDirectoryContents == null) currentDirectoryContents = new List<string>();
            gbFiles.Clear();
            currentDirectoryContents = Directory.GetFiles(currentDirectory).ToList();
            List<string> subdirs = Directory.GetDirectories(currentDirectory).ToList();
            subDirectories.Clear();
            subDirectories.Add("..");
            subDirectories.AddRange(subdirs);
            DetectGbGames();
        }

        private void DetectGbGames()
        {
            for (int i = 0; i < currentDirectoryContents.Count; i++)
                if (currentDirectoryContents[i].EndsWith(".gb"))
                    gbFiles.Add(i);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(font, currentDirectory, new Vector2(5, 5), Color.Blue);
            int clampedPointerIndex = Clamp(pointerIndex, 0, 11);
            int totalEntries = subDirectories.Count + currentDirectoryContents.Count;
            if (totalEntries >= MaxPointerIndex)
            {
                if (pointerIndex >= MaxPointerIndex/2)
                {
                    List<(string, bool)> tenBefore = new List<(string, bool)>();
                    List<(string, bool)> tenAfter =  new List<(string, bool)>();
                    tenBefore.Add(("..", true));
                    for (int i = pointerIndex - 10; i < pointerIndex; i++) tenBefore.Add(GetFileBrowserIndex(i));
                    for (int i = pointerIndex; i < pointerIndex + 12; i++) tenAfter.Add(GetFileBrowserIndex(i));
                    List<(string, bool)> allLines = new List<(string, bool)>();
                    allLines.AddRange(tenBefore);
                    allLines.AddRange(tenAfter);
                    for (int i = 0; i < allLines.Count; i++)
                    {
                        if (!allLines[i].Item2 && allLines[i].Item1.EndsWith(".gb"))
                        {
                            spriteBatch.DrawString(font, allLines[i].Item1.Replace(currentDirectory, ""), new Vector2(25, LineSpacing * (i + 1) + 5),
                                allLines[i].Item2 ? Color.Gold : Color.Cyan, 0.0f, new Vector2(0, 0), gbGameScalar, SpriteEffects.None, 0.0f);
                        }
                        else
                        {
                            spriteBatch.DrawString(font, allLines[i].Item1.Replace(currentDirectory, ""), new Vector2(25, LineSpacing * (i + 1) + 5), allLines[i].Item2 ? Color.Gold : Color.Cyan);
                        }

                    }
                    spriteBatch.DrawString(font, pointerCharacter, new Vector2(5, LineSpacing * (MaxPointerIndex/2 + 1) + 5), Color.White);

                }
                else
                {
                    for (int i = 0; i < subDirectories.Count; i++)
                        spriteBatch.DrawString(font, subDirectories[i].Replace(currentDirectory, ""), new Vector2(25, LineSpacing * (i + 1) + 5), Color.Gold);
                    for (int i = 0; i < currentDirectoryContents.Count; i++)
                    {
                        if (currentDirectoryContents[i].EndsWith(".gb"))
                        {
                            spriteBatch.DrawString(font, currentDirectoryContents[i].Replace(currentDirectory, ""), new Vector2(25, LineSpacing * (i + subDirectories.Count + 1) + 5),
                                Color.Cyan, 0.0f, new Vector2(0, 0), gbGameScalar, SpriteEffects.None, 0.0f);
                        }
                        else
                        {
                            spriteBatch.DrawString(font, currentDirectoryContents[i].Replace(currentDirectory, ""), new Vector2(25, LineSpacing * (i + subDirectories.Count + 1) + 5), Color.Cyan);
                        }
                    }
                    spriteBatch.DrawString(font, pointerCharacter, new Vector2(5, LineSpacing * (pointerIndex + 1) + 5), Color.White);
                }
            }
            else
            {
                for (int i = 0; i < subDirectories.Count; i++)
                    spriteBatch.DrawString(font, subDirectories[i].Replace(currentDirectory, ""), new Vector2(25, LineSpacing * (i + 1) + 5), Color.Gold);
                for (int i = 0; i < currentDirectoryContents.Count; i++)
                {
                    if (currentDirectoryContents[i].EndsWith(".gb"))
                    {
                        spriteBatch.DrawString(font, currentDirectoryContents[i].Replace(currentDirectory, ""), new Vector2(25, LineSpacing * (i + subDirectories.Count + 1) + 5),
                            Color.Chartreuse, 0.0f, new Vector2(0, 0), gbGameScalar, SpriteEffects.None, 0.0f);
                    }
                    else
                    {
                        spriteBatch.DrawString(font, currentDirectoryContents[i].Replace(currentDirectory, ""), new Vector2(25, LineSpacing * (i + subDirectories.Count + 1) + 5), Color.Cyan);
                    }
                }
                spriteBatch.DrawString(font, pointerCharacter, new Vector2(5, LineSpacing * (pointerIndex + 1) + 5), Color.White);
            }
        }

        private (string, bool) GetFileBrowserIndex(int index)
        {
            bool isDirectory = false;
            string fname = "";
            if (index < subDirectories.Count)
            {
                isDirectory = true;
                fname = subDirectories[index];
            }
            else if (index >= subDirectories.Count + currentDirectoryContents.Count)
            {
                
            }
            else
            {
                fname = currentDirectoryContents[index - subDirectories.Count];
            }
            return (fname, isDirectory);
        }

        private int Clamp(int val, int min, int max)
        {
            int returnVal = val;
            if (returnVal < min) returnVal = min;
            else if (returnVal > max) returnVal = max;
            return returnVal;
        }

        public void LoadContent(ContentManager content)
        {
            font = content.Load<SpriteFont>("MainFont");
        }

        public void UnloadContent()
        {
        }

        private bool pressedUpLastFrame = false, pressedDownLastFrame = false, pressedEnterLastFrame = false;

        public void Update(GameTime gameTime)
        {
            if (currentDirectory == null || subDirectories == null)
                return;

            if (IsPressed(Keys.Down) && !pressedDownLastFrame)
            {
                pointerIndex++;
                pressedDownLastFrame = true;
            }
            else if (!IsPressed(Keys.Down))
            {
                pressedDownLastFrame = false;
            }

            if (IsPressed(Keys.Up) && !pressedUpLastFrame)
            {
                pointerIndex--;
                pressedUpLastFrame = true;
            }
            else if (!IsPressed(Keys.Up))
            {
                pressedUpLastFrame = false;
            }

            if (pointerIndex >= currentDirectoryContents.Count + subDirectories.Count)
                pointerIndex = (currentDirectoryContents.Count+subDirectories.Count) - 1;

            if (pointerIndex < 0)
                pointerIndex = 0;

            if (IsPressed(Keys.Enter) && !pressedEnterLastFrame)
            {
                EnterPressed();
                pressedEnterLastFrame = true;
            }
            else if (!IsPressed(Keys.Enter))
            {
                pressedEnterLastFrame = false;
            }

            gbGameScalar += gbGameScalarVelocity;

            if (gbGameScalar >= MaxGbScalar || gbGameScalar <= MinGbScalar) gbGameScalarVelocity *= -1;

        }

        private void EnterPressed()
        {
            if (pointerIndex < subDirectories.Count)
            {
                string chosenDir = subDirectories[pointerIndex];

                if (chosenDir == ".." && currentDirectory.Length > 3)
                {
                    bool flag = OsUtil.IsWindows() ? currentDirectory.LastIndexOf("\\") != 2 : currentDirectory != "/";
                    if (flag)
                    {   
                        if (OsUtil.IsWindows())
                        {
                            currentDirectory = currentDirectory.Substring(0, currentDirectory.LastIndexOf("\\"));
                        }
                        else
                        {
                            currentDirectory = currentDirectory.Substring(0, currentDirectory.LastIndexOf("/"));
                            if (currentDirectory == "") currentDirectory = "/";
                        }
                    }
                    else
                    {
                        currentDirectory = currentDirectory.Substring(0, 3);
                    }
                }
                else if (pointerIndex != 0)
                {
                    currentDirectory = subDirectories[pointerIndex];
                }
                RefreshDirectory();
            }
            else
            {
                string fname = GetFileBrowserIndex(pointerIndex).Item1;
                RemuxWindow wnd = (RemuxWindow)game;
                wnd.StateManager.TransitionTo(fname.EndsWith(".gb") ? (IState)new GameboyPlayScreen(fname, wnd) : new ConfirmPlayScreen(fname, wnd));
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
