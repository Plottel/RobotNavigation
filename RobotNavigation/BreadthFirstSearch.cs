using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotNavigation
{
    public class BreadthFirstSearch : ISearch
    {
        public List<Node> Path { get; set; }

        public BreadthFirstSearch()
        {
            Path = new List<Node>();
        }

        public Queue<SearchSnapshot> Search(NodeGrid grid)
        {
            var snapshots = new Queue<SearchSnapshot>();

            var parents = new Dictionary<Node, Node>();
            var open = new Queue<Node>();
            var closed = new List<Node>();
            Node current = null;

            open.Enqueue(grid.startNode);

            // Search
            while (open.Count > 0)
            {
                current = open.Dequeue();

                // Create new SearchSnapshot
                snapshots.Enqueue(SearchUtils.MakeSnapshot(grid, current, open, closed, parents));

                if (current == grid.targetNode)
                    break;

                if (!closed.Contains(current))
                    closed.Add(current);

                foreach (var neighbour in current.neighbours)
                {
                    if (!closed.Contains(neighbour) && !open.Contains(neighbour) && !neighbour.isWall)
                    {
                        open.Enqueue(neighbour);
                        parents[neighbour] = current;
                    }
                }
            }

            Path = SearchUtils.RetracePath(current, parents);

            return snapshots;
        }
    }
}
