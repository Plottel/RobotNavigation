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
    public struct JPSCalculationSnapshot : ISearchSnapshot
    {
        public NodeGrid grid;
        public Point startIdx;
        public Point jumpIdx;
        public List<Point> openIndexes;

        public List<string> PathDirections
        {
            get
            {
                return new List<string>();
            }
        }

        public void Render(SpriteBatch spriteBatch)
        {
            foreach (var idx in openIndexes)
                spriteBatch.FillRectangle(grid[idx.Col(), idx.Row()].Bounds, Color.PowderBlue);

            spriteBatch.FillRectangle(grid[startIdx].Bounds, Color.Yellow);
            spriteBatch.FillRectangle(grid[jumpIdx].Bounds, Color.Orange);
            spriteBatch.DrawLine(grid[startIdx].Bounds.Center.ToVector2(),
                grid[jumpIdx].Bounds.Center.ToVector2(), Color.OliveDrab);
        }
    }
}
