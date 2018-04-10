using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotNavigation
{
    public class BDSearchHalf
    {
        public NodeGrid grid;
        public Node current;
        public Queue<Node> open = new Queue<Node>();
        public List<Node> closed = new List<Node>();
        public Dictionary<Node, Node> parents = new Dictionary<Node, Node>();

        public BDSearchHalf(Node start, NodeGrid grid)
        {
            open.Enqueue(start);
            this.grid = grid;
        }

        public SearchSnapshot ExpandOneNode()
        {
            // Other half may still be searching, return current snapshot.
            if (open.Count == 0)
                return SearchUtils.MakeSnapshot(grid, current, open, closed, parents);

            current = open.Dequeue();

            if (!closed.Contains(current))
                closed.Add(current);

            foreach (var neighbour in current.neighbours)
            {
                if (closed.Contains(neighbour) || open.Contains(neighbour) || neighbour.isWall)
                    continue;

                open.Enqueue(neighbour);
                parents[neighbour] = current;
            }

            // Return new snapshot.
            return SearchUtils.MakeSnapshot(grid, current, open, closed, parents);
        }

        public SearchSnapshot MakeSnapshot()
        {
            return SearchUtils.MakeSnapshot(grid, current, open, closed, parents);
        }
    }

    public class BidirectionalSearch : ISearch
    {
        public BDSearchHalf front;
        public BDSearchHalf back;

        public Queue<ISearchSnapshot> Search(NodeGrid grid)
        {
            var snapshots = new Queue<ISearchSnapshot>();
            Node intersect = null;

            front = new BDSearchHalf(grid.startNode, grid);
            back = new BDSearchHalf(grid.targetNode, grid);

            while (front.open.Count > 0 || back.open.Count > 0)
            {
                snapshots.Enqueue(new BidirectionalSearchSnapshot
                {
                    frontSnapshot = front.ExpandOneNode(),
                    backSnapshot = back.ExpandOneNode(),
                    isSearchFinished = false
                });

                intersect = GetIntersectingNode(front.open, back.open);
                if (intersect != null)
                    break;
            }

            // Link the two paths.
            front.current = intersect;
            back.current = intersect;

            snapshots.Enqueue(new BidirectionalSearchSnapshot
            {
                frontSnapshot = front.MakeSnapshot(),
                backSnapshot = back.MakeSnapshot(),
                isSearchFinished = true
            });


            return snapshots;
        }

        private Node GetIntersectingNode(Queue<Node> open1, Queue<Node> open2)
        {
            foreach (var n1 in open1)
            {
                foreach (var n2 in open2)
                {
                    if (n1 == n2)
                        return n1;
                }
            }

            return null;
        }
    }
}
