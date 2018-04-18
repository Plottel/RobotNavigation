using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace RobotNavigation
{
    public struct JPSSnapshot : ISearchSnapshot
    {
        public NodeGrid grid;
        public Point startIdx;
        public Point jumpIdx;
        public List<Point> openIndexes;
        public List<Point> pathIndexes;

        public List<string> PathDirections
        {
            get
            {
                var result = new List<string>();

                for (int i = 1; i < pathIndexes.Count; ++i)
                {
                    Point from = pathIndexes[i - 1];
                    Point to = pathIndexes[i];
                    int colDist = Math.Abs(from.Col() - to.Col());
                    int rowDist = Math.Abs(from.Row() - to.Row());
                    string dir = SearchUtils.NodeIndexesToMoveDirection(from, to);

                    if (dir == "LEFT" || dir == "RIGHT")
                    {
                        for (int j = 0; j < colDist; j++)
                            result.Add(dir);
                    }
                    else if (dir == "UP" || dir == "DOWN")
                    {
                        for (int j = 0; j < rowDist; j++)
                            result.Add(dir);
                    }
                }

                return result;
            }
        }

        public void Render(SpriteBatch spriteBatch)
        {
            foreach (var idx in openIndexes)
                spriteBatch.FillRectangle(grid[idx.Col(), idx.Row()].Bounds, Color.PowderBlue);

            // Render Jump Point line
            spriteBatch.FillRectangle(grid[startIdx].Bounds, Color.Yellow);
            spriteBatch.FillRectangle(grid[jumpIdx].Bounds, Color.Orange);
            spriteBatch.DrawLine(grid[startIdx].Bounds.Center.ToVector2(),
                grid[jumpIdx].Bounds.Center.ToVector2(), Color.Yellow, 2);

            if (pathIndexes.Count < 2)
                return;

            Point idx1, idx2;
            // Render Path
            for (int i = 0; i < pathIndexes.Count - 1; ++i)
            {
                idx1 = pathIndexes[i];
                idx2 = pathIndexes[i + 1];

                // Render line between two center points
                Vector2 start = grid[idx1.Col(), idx1.Row()].Bounds.Center.ToVector2();
                Vector2 end = grid[idx2.Col(), idx2.Row()].Bounds.Center.ToVector2();

                spriteBatch.DrawLine(start, end, Color.LawnGreen, 5);
            }

            // Render final node in Path
            idx1 = pathIndexes[pathIndexes.Count - 2];
            idx2 = pathIndexes[pathIndexes.Count - 1];

            spriteBatch.DrawLine(grid[idx1.Col(), idx1.Row()].Bounds.Center.ToVector2(),
                                grid[idx2.Col(), idx2.Row()].Bounds.Center.ToVector2(),
                                Color.LawnGreen,
                                5);            
        }
    }
}
