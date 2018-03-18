using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace RobotNavigation
{
    public class DepthFirstSearch : ISearch
    {
        public List<Node> Path { get; set; }

        public DepthFirstSearch()
        {
            Path = new List<Node>();
        }

        public void Search(NodeGrid grid)
        {
            var parents = new Dictionary<Node, Node>();
            var open = new Stack<Node>();
            var closed = new List<Node>();
            Node current = null;

            open.Push(grid.startNode);

            // Search
            while (open.Count > 0)
            {
                current = open.Pop();

                if (current == grid.targetNode)
                    break;

                if (!closed.Contains(current))
                    closed.Add(current);

                foreach (var neighbour in current.neighbours)
                {
                    if (!closed.Contains(neighbour) && !neighbour.isWall)
                    {
                        open.Push(neighbour);
                        parents[neighbour] = current;
                    }
                }
            }

            // Retrace the path
            var path = new List<Node>();

            // Until we reach the source node...
            while (parents.ContainsKey(current))
            {
                path.Add(current);
                current = parents[current];
            }

            path.Add(grid.startNode);
            path.Reverse();

            Path = path;
        }
    }
}
