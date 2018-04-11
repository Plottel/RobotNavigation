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
        public NodeGrid grid;
        ISearch search;
        Queue<ISearchSnapshot> snapshots = new Queue<ISearchSnapshot>();
        public SpriteFont font;
        public SpriteFont smallFont;

        public static Game1 Instance;

        public Game1()
        {
            Instance = this;
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

            grid = GridParser.LoadGridFrom("RobotNav-test.txt");
            search = new DepthFirstSearch();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            font = Content.Load<SpriteFont>("font");
            smallFont = Content.Load<SpriteFont>("smallFont");
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            Input.UpdateStates();

            if (Input.KeyTyped(Keys.D1))
                search = new DepthFirstSearch();
            if (Input.KeyTyped(Keys.D2))
                search = new BreadthFirstSearch();
            if (Input.KeyTyped(Keys.D3))
                search = new GreedyBestFirstSearch();
            if (Input.KeyTyped(Keys.D4))
                search = new AStarSearch();
            if (Input.KeyTyped(Keys.D5))
                search = new BidirectionalSearch();
            if (Input.KeyTyped(Keys.N))
            {
                grid = GridParser.CreateGridToFitScreen(18, 18);
                snapshots.Clear();
            }

            if (Input.KeyTyped(Keys.Space))
                snapshots = search.Search(grid);

            HandleGridChangeInput();

            base.Update(gameTime);
        }

        private void HandleGridChangeInput()
        {
            var node = grid.NodeAt(Input.MousePos);
            if (node == null) return;

            if (Input.LeftMouseDown())
                node.isWall = true;
            if (Input.RightMouseDown())
                node.isWall = false;
            if (Input.KeyTyped(Keys.S))
                grid.startNode = node;
            if (Input.KeyTyped(Keys.T))
                grid.targetNode = node;


        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DimGray);

            spriteBatch.Begin();

            {
                if (snapshots.Count > 1)
                {
                    snapshots.Dequeue().Render(spriteBatch);
                    System.Threading.Thread.Sleep(20);
                }
                else if (snapshots.Count == 1) // Leave the final snapshot visible on screen
                {
                    snapshots.Peek().Render(spriteBatch);
                }

                // Render Start and Target Nodes
                spriteBatch.FillRectangle(grid.startNode.Bounds, Color.Red);
                spriteBatch.FillRectangle(grid.targetNode.Bounds, new Color(0, 255, 0));

                RenderGrid();

                RenderUI();
            }
            

            spriteBatch.End();

            base.Draw(gameTime);
        }  

        // Renders the nodes and then renders grid lines on top.
        private void RenderGrid()
        {
            // Render nodes.
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

        private void RenderUI()
        {
            Color clr = Color.White;
            var start = new Vector2(GridParser.MAX_GRID_WIDTH + 20, 20);

            spriteBatch.DrawString(font, " --- Instructions --- ", start, clr);
            spriteBatch.DrawString(font, "  Space - Run Search", start + new Vector2(0, 30), clr);
            spriteBatch.DrawString(font, "  1 - Depth First Search", start + new Vector2(0, 60), clr);
            spriteBatch.DrawString(font, "  2 - Breadth First Search", start + new Vector2(0, 90), clr);
            spriteBatch.DrawString(font, "  3 - Greedy Best First Search", start + new Vector2(0, 120), clr);
            spriteBatch.DrawString(font, "  4 - AStar Search", start + new Vector2(0, 150), clr);
            spriteBatch.DrawString(font, "  5 - Bidirectional Search", start + new Vector2(0, 180), clr);
            spriteBatch.DrawString(font, "  N - Create a Blank Grid", start + new Vector2(0, 210), clr);
            spriteBatch.DrawString(font, "  S - Change Start Node", start + new Vector2(0, 240), clr);
            spriteBatch.DrawString(font, "  T - Change Target Node", start + new Vector2(0, 270), clr);

            spriteBatch.DrawString(font, "Current Search Type : " + search.GetType().Name, start + new Vector2(0, 300), clr);

            spriteBatch.DrawString(font, " --- Search Results --- ", start + new Vector2(0,330), clr);
            RenderPathResults(start + new Vector2(0, 360), clr);
            
        }

        private void RenderPathResults(Vector2 start, Color clr)
        {
            const float MAX_LINE_WIDTH = 240;
            float lineWidth = 0;
            float lineHeight = smallFont.MeasureString("X").Y + 5;

            if (snapshots.Count == 0)
                return;

            spriteBatch.DrawString(font, "Path Length : " + snapshots.Peek().PathDirections.Count, start, clr);
            start.Y += 25;

            foreach (string move in snapshots.Peek().PathDirections)
            {
                string formattedMove = move + ", ";

                if (move == "FRONT HALF" || move == "BACK HALF")
                {
                    start.Y += lineHeight * 2;
                    lineWidth = 0;
                }
                else
                    lineWidth += 60;

                spriteBatch.DrawString(smallFont, formattedMove, start + new Vector2(lineWidth, 0), clr);

                if (move == "FRONT HALF" || move == "BACK HALF")
                    lineWidth = MAX_LINE_WIDTH + 1;

                if (lineWidth > MAX_LINE_WIDTH)
                {
                    start.Y += lineHeight;
                    lineWidth = 0;
                }
            }
        }
    }
}
