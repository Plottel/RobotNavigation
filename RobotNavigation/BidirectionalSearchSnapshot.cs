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
    public struct BidirectionalSearchSnapshot : ISearchSnapshot
    {
        public SearchSnapshot frontSnapshot;
        public SearchSnapshot backSnapshot;
        public bool isSearchFinished;

        public List<string> PathDirections
        {
            get
            {
                var result = new List<string>();
                result.Add("FRONT HALF");
                result.AddRange(frontSnapshot.pathDirections);
                result.Add("BACK HALF");
                result.AddRange(backSnapshot.pathDirections);

                return result;
            }
        }

        public void Render(SpriteBatch spriteBatch)
        {
            frontSnapshot.Render(spriteBatch);
            backSnapshot.Render(spriteBatch);

            if (!isSearchFinished)
                return;

            // Draw final line connecting the two paths
            var grid = frontSnapshot.grid;
            var idx1 = frontSnapshot.pathIndexes[frontSnapshot.pathIndexes.Count - 2];
            var idx2 = backSnapshot.pathIndexes[backSnapshot.pathIndexes.Count - 1];

            spriteBatch.DrawLine(grid[idx1.Col(), idx1.Row()].Bounds.Center.ToVector2(),
                                grid[idx2.Col(), idx2.Row()].Bounds.Center.ToVector2(),
                                Color.Green,
                                2);
        }
    }
}
