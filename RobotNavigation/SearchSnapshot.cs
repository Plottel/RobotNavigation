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
    public struct SearchSnapshot
    {
        public NodeGrid grid;
        public List<Point> openIndexes;
        public List<Point> closedIndexes;
        public List<Point> pathIndexes;
        public List<string> pathDirections;

        public void Render(SpriteBatch spriteBatch)
        {
            foreach (var idx in openIndexes)
                spriteBatch.FillRectangle(grid[idx.Col(), idx.Row()].Bounds, Color.PowderBlue);

            foreach (var idx in closedIndexes)
                spriteBatch.FillRectangle(grid[idx.Col(), idx.Row()].Bounds, Color.LightGoldenrodYellow);

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

                spriteBatch.DrawLine(start, end, Color.Green, 2);
            }

            // Render final node in Path
            idx1 = pathIndexes[pathIndexes.Count - 2];
            idx2 = pathIndexes[pathIndexes.Count - 1];

            spriteBatch.DrawLine(grid[idx1.Col(), idx1.Row()].Bounds.Center.ToVector2(),
                                grid[idx2.Col(), idx2.Row()].Bounds.Center.ToVector2(),
                                Color.Green,
                                2);
        }
    }
}
