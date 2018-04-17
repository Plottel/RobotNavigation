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

        public Queue<ISearchSnapshot> Search(NodeGrid grid)
        {
            var snapshots = new Queue<ISearchSnapshot>();

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
                snapshots.Enqueue(SearchUtils.MakeAStarSnapshot(grid, current, open, closed, parents, scores));

                if (current == grid.targetNode)
                    break;

                if (!closed.Contains(current))
                    closed.Add(current);

                foreach (var neighbour in current.neighbours)
                {
                    if (neighbour.isWall || closed.Contains(neighbour))
                        continue;

                    // Node has been examined
                    if (open.Contains(neighbour))
                    {
                        // If this will be a shorter path, update parent.

                        // Temporarily change parent to check new score.
                        var oldParent = parents[neighbour];
                        parents[neighbour] = current;

                        var newScore = SearchUtils.GetScore(neighbour, grid.targetNode, parents, scores);

                        if (newScore.g < scores[neighbour].g)
                            scores[neighbour] = newScore;
                        else // New path was not shorter, reset original parent.
                            parents[neighbour] = oldParent;

                    }
                    else // Node needs to be added to frontier
                    {
                        open.Add(neighbour);
                        parents[neighbour] = current;
                        scores[neighbour] = SearchUtils.GetScore(neighbour, grid.targetNode, parents, scores);
                    }
                }
            }

            Path = SearchUtils.RetracePath(current, parents);

            return snapshots;
        }
    }
}
