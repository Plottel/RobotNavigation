using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using System.IO;
using System;
using System.Collections.Generic;

namespace RobotNavigation
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        NodeGrid grid;
        ISearch search;
        Queue<SearchSnapshot> snapshots = new Queue<SearchSnapshot>();


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;

            // Restrict mouse to stay within the window.
            Input.SetMaxMouseX(graphics.PreferredBackBufferWidth);
            Input.SetMaxMouseY(graphics.PreferredBackBufferHeight);
        }

        protected override void Initialize()
        {
            IsMouseVisible = true;
            Mouse.WindowHandle = Window.Handle;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            Input.UpdateStates();

            if (Input.KeyTyped(Keys.Space))
            {
                snapshots = search.Search(grid);
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DimGray);

            spriteBatch.Begin();

            {
                if (snapshots.Count > 1)
                {
                    snapshots.Dequeue().Render(spriteBatch);
                    System.Threading.Thread.Sleep(200);
                }
                else if (snapshots.Count == 1) // Leave the final snapshot visible on screen
                {
                    snapshots.Peek().Render(spriteBatch);
                }

                // Render Start and Target Nodes
                spriteBatch.FillRectangle(grid.startNode.Bounds, Color.Red);
                spriteBatch.FillRectangle(grid.targetNode.Bounds, new Color(0, 255, 0));

                RenderGrid();
            }
            

            spriteBatch.End();

            base.Draw(gameTime);
        }

        public void Setup(string[] args)
        {
            grid = GridParser.LoadGridFrom(args[0]);

            //string searchType = args[1];

            search = new DepthFirstSearch();
        }        

        // Renders the nodes and then renders grid lines on top.
        private void RenderGrid()
        {
            // Render nodes.
            Color nodeColor;
            foreach (var node in grid)
            {
                if (node.isWall)
                    spriteBatch.FillRectangle(node.Bounds, Color.Black);
            }

            // For grid line rendering.
            Vector2 lineStart;
            Vector2 lineEnd;

            // Render vertical Column lines.
            for (int i = 0; i <= grid.Cols; ++i)
            {
                lineStart = grid.Pos + new Vector2(grid.TileSize * i, 0);
                lineEnd = lineStart + new Vector2(0, grid.Height);

                spriteBatch.DrawLine(lineStart, lineEnd, Color.Black, 1);
            }

            // Render horizontal Row lines;
            for (int i = 0; i <= grid.Rows; ++i)
            {
                lineStart = grid.Pos + new Vector2(0, grid.TileSize * i);
                lineEnd = lineStart + new Vector2(grid.Width, 0);

                spriteBatch.DrawLine(lineStart, lineEnd, Color.Black, 1);
            }            
        }
    }
}
