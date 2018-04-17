using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace RobotNavigation
{
    public class GreedyBestFirstSearch : ISearch
    {
        public List<Node> Path { get; set; }

        public GreedyBestFirstSearch()
        {
            Path = new List<Node>();
        }

        public Stack<ISearchSnapshot> Search(NodeGrid grid)
        {
            var snapshots = new Stack<ISearchSnapshot>();

            var parents = new Dictionary<Node, Node>();
            var open = new List<Node>();
            var closed = new List<Node>();
            var scores = new Dictionary<Node, float>();
            Node current = null;

            open.Add(grid.startNode);
            scores[grid.startNode] = 0;

            // Search
            while (open.Count > 0)
            {
                open = open.OrderBy(n => scores[n]).ToList(); // Essentially turns it into a Priority Queue
                current = open[0];
                open.RemoveAt(0);

                // Create new SearchSnapshot
                snapshots.Push(SearchUtils.MakeSnapshot(grid, current, open, closed, parents));

                if (current == grid.targetNode)
                    break;

                if (!closed.Contains(current))
                    closed.Add(current);

                foreach (var neighbour in current.neighbours)
                {
                    if (!closed.Contains(neighbour) && !open.Contains(neighbour) && !neighbour.isWall)
                    {
                        open.Add(neighbour);
                        parents[neighbour] = current;
                        scores[neighbour] = Vector2.Distance(
                            neighbour.Bounds.Center.ToVector2(),
                            grid.targetNode.Bounds.Center.ToVector2()
                            );
                    }
                }
            }

            Path = SearchUtils.RetracePath(current, parents);

            return snapshots;
        }
    }
}
