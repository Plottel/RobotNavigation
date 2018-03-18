using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace RobotNavigation
{
    public class Node
    {
        public Vector2 pos;
        public Vector2 size;
        public bool isWall;

        public Rectangle Bounds
        {
            get => new Rectangle(pos.ToPoint(), size.ToPoint());
        }
    }
}
