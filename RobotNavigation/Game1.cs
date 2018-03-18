using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using System.IO;
using System;

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

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DimGray);

            spriteBatch.Begin();

            {
                RenderGrid();
            }
            
            spriteBatch.End();

            base.Draw(gameTime);
        }

        public void LoadGrid(string filePath)
        {
            grid = GridParser.LoadGridFrom(filePath);           
        }        

        // Renders the nodes and then renders grid lines on top.
        private void RenderGrid()
        {
            // Render nodes.
            Color nodeColor;
            foreach (var node in grid)
            {
                if (node.isWall)
                    nodeColor = Color.Black;
                else
                    nodeColor = Color.LightSlateGray;

                spriteBatch.FillRectangle(node.Bounds, nodeColor);
            }

            // For grid line rendering.
            Vector2 lineStart;
            Vector2 lineEnd;

            // Render vertical Column lines.
            for (int i = 0; i < grid.Cols; ++i)
            {
                lineStart = grid.Pos + new Vector2(grid.TileSize * i, 0);
                lineEnd = lineStart + new Vector2(0, grid.Height);

                spriteBatch.DrawLine(lineStart, lineEnd, Color.Black, 1);
            }

            // Render Start and Target Nodes
            spriteBatch.FillRectangle(grid.startNode.Bounds, Color.Red);
            spriteBatch.FillRectangle(grid.targetNode.Bounds, new Color(0, 255, 0));

            // Render horizontal Row lines;
            for (int i = 0; i < grid.Rows; ++i)
            {
                lineStart = grid.Pos + new Vector2(0, grid.TileSize * i);
                lineEnd = lineStart + new Vector2(grid.Width, 0);

                spriteBatch.DrawLine(lineStart, lineEnd, Color.Black, 1);
            }            
        }
    }
}
