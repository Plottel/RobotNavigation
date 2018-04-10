using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RobotNavigation
{
    public struct BidirectionalSearchSnapshot : ISearchSnapshot
    {
        public SearchSnapshot frontSnapshot;
        public SearchSnapshot backSnapshot;

        public List<string> PathDirections
        {
            get
            {
                var result = new List<string>();
                result.Add("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ FRONT HALF ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
                result.AddRange(frontSnapshot.pathDirections);
                result.Add("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ BACK HALF ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
                result.AddRange(backSnapshot.pathDirections);

                return result;
            }
        }

        public void Render(SpriteBatch spriteBatch)
        {
            frontSnapshot.Render(spriteBatch);
            backSnapshot.Render(spriteBatch);
        }
    }
}
