using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace RobotNavigation
{
    public static class SearchUtils
    {
        public static List<Node> RetracePath(Node current, Dictionary<Node, Node> parents)
        {
            // Retrace the path
            var path = new List<Node>();

            // Until we reach the source node...
            while (parents.ContainsKey(current))
            {
                path.Add(current);
                current = parents[current];
            }

            path.Reverse();

            return path;
        }

        public static List<Point> NodesToNodeIndexes(IEnumerable<Node> nodes, NodeGrid grid)
        {
            var result = new List<Point>();

            foreach (var node in nodes)
                result.Add(grid.IndexAt(node.pos));

            return result;
        }
    }
}
