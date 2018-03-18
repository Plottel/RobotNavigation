﻿using Microsoft.Xna.Framework;
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

        const int MAX_GRID_WIDTH = 800;
        const int MAX_GRID_HEIGHT = 700;

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
            using (StreamReader reader = new StreamReader(File.Open(filePath, FileMode.Open)))
            {
                LoadGridDimensions(reader.ReadLine());
            }
        }

        // Extracts the cols and rows from the line in format [rows, cols]
        private void LoadGridDimensions(string input)
        {
            // Strip square braces
            input = input.Replace("[", "");
            input = input.Replace("]", "");

            // Separate into two numbers, rows and cols
            int commaIndex = input.IndexOf(",");

            int rows = Convert.ToInt32(input.Substring(0, commaIndex));
            int cols = Convert.ToInt32(input.Substring(commaIndex + 1));

            // Determine appropriate TileSize
            int tileWidth = MAX_GRID_WIDTH / cols;
            int tileHeight = MAX_GRID_HEIGHT / rows;

            // Make TileSize square
            int tileSize = Math.Min(tileWidth, tileHeight);

            // Create Grid
            grid = new NodeGrid(new Vector2(5, 5), cols, rows, tileSize);
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
