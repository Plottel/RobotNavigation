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
    }

    public class BidirectionalSearch : ISearch
    {
        public BDSearchHalf front;
        public BDSearchHalf back;

        public Queue<ISearchSnapshot> Search(NodeGrid grid)
        {
            var snapshots = new Queue<ISearchSnapshot>();

            front = new BDSearchHalf(grid.startNode, grid);
            back = new BDSearchHalf(grid.targetNode, grid);

            while (front.open.Count > 0 || back.open.Count > 0)
            {
                snapshots.Enqueue(new BidirectionalSearchSnapshot
                {
                    frontSnapshot = front.ExpandOneNode(),
                    backSnapshot = back.ExpandOneNode()
                });                

                if (front.current == back.current)
                    break;
            }

            return snapshots;
        }
    }
}
