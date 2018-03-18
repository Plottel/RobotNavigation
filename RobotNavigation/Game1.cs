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
        ISearch search;


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
                search.Search(grid);
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DimGray);

            spriteBatch.Begin();

            {
                RenderGrid();
                RenderPath();

                // Render Start and Target Nodes
                spriteBatch.FillRectangle(grid.startNode.Bounds, Color.Red);
                spriteBatch.FillRectangle(grid.targetNode.Bounds, new Color(0, 255, 0));

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

        private void RenderPath()
        {
            if (search.Path.Count < 2)
                return;

            // Render Path
            var path = search.Path;
            for (int i = 0; i < search.Path.Count - 1; ++i)
            {
                // Render line between two center points
                Vector2 start = path[i].Bounds.Center.ToVector2();
                Vector2 end = path[i + 1].Bounds.Center.ToVector2();

                spriteBatch.DrawLine(start, end, Color.Yellow, 2);
            }

            // Render final node in Path
            spriteBatch.DrawLine(path[path.Count - 2].Bounds.Center.ToVector2(),
                                path[path.Count - 1].Bounds.Center.ToVector2(),
                                Color.Yellow,
                                2);
        }
    }
}
