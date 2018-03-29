using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace RobotNavigation
{
    public class AStarSearch : ISearch
    {
        public List<Node> Path { get; set; }

        public AStarSearch()
        {
            Path = new List<Node>();
        }

        public struct NodeScore
        {
            public float f;
            public float g;
            public float h;
        }

        public Queue<SearchSnapshot> Search(NodeGrid grid)
        {
            var snapshots = new Queue<SearchSnapshot>();

            var parents = new Dictionary<Node, Node>();
            var open = new List<Node>();
            var closed = new List<Node>();
            var scores = new Dictionary<Node, NodeScore>();

            Node current = null;

            open.Add(grid.startNode);
            scores[grid.startNode] = new NodeScore();

            // Search
            while (open.Count > 0)
            {
                open = open.OrderBy(n => scores[n].f).ToList(); // Essentially turns it into a Priority Queue
                current = open[0];
                open.RemoveAt(0);

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
                        open.Add(neighbour);
                        parents[neighbour] = current;

                        // TODO: Stuff about revisiting node. Can't just go if !closed and !open. We can revisit.

                        var newScore = new NodeScore();
                        newScore.h = Vector2.Distance(
                            neighbour.Bounds.Center.ToVector2(),
                            grid.targetNode.Bounds.Center.ToVector2()
                            );
                        newScore.g = SearchUtils.GetGScore(neighbour, parents, scores);
                        newScore.f = newScore.g + newScore.h;

                        scores[neighbour] = newScore;
                    }
                }
            }

            Path = SearchUtils.RetracePath(current, parents);

            return snapshots;
        }
    }
}
