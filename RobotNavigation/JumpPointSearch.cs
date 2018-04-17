using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RobotNavigation
{
    public class JumpPointSearch : ISearch
    {
        private NodeGrid grid;

        public Queue<ISearchSnapshot> Search(NodeGrid grid)
        {
            this.grid = grid;
            var parents = new Dictionary<Node, Node>();
            var open = new List<Node>();
            var closed = new List<Node>();
            var scores = new Dictionary<Node, NodeScore>();

            Node current = null;

            open.Add(grid.startNode);
            scores[grid.startNode] = new NodeScore();

            while (open.Count > 0)
            {
                open = open.OrderBy(n => scores[n].f).ToList(); // Priority queue
                current = open[0];
                open.RemoveAt(0);

                if (current == grid.targetNode)
                    break;

                if (!closed.Contains(current))
                    closed.Add(current);

                for (int i = current.neighbours.Count - 1; i >= 0; --i)
                {
                    Node neighbour = current.neighbours[i];

                    if (neighbour.isWall | closed.Contains(neighbour))
                        continue;

                    // Flesh this out properly with revisiting code.
                    current.neighbours[i] = GetJumpPointForNeighbour(current, neighbour);
                    neighbour = current.neighbours[i]; // Update since may have been overwritten.

                    if (!open.Contains(neighbour))
                    {
                        open.Add(neighbour);
                        parents[neighbour] = current;
                        scores[neighbour] = SearchUtils.GetScore(neighbour, grid.targetNode, parents, scores);
                    }                    
                }
            } 

            return new Queue<ISearchSnapshot>();
        }

        private Node GetJumpPointForNeighbour(Node node, Node neighbour)
        {
            string direction = SearchUtils.NodeIndexesToMoveDirection(grid.IndexOf(node), grid.IndexOf(neighbour));
            Node jumpPoint = null;

            var idx = grid.IndexOf(neighbour);

            if (direction == "RIGHT")
                jumpPoint = GetJumpPointHorizontal(idx.Col(), idx.Row(), 1);
            else if (direction == "LEFT")
                jumpPoint = GetJumpPointHorizontal(idx.Col(), idx.Row(), -1);
            else if (direction == "UP")
                jumpPoint = GetJumpPointVertical(idx.Col(), idx.Row(), -1);
            else if (direction == "DOWN")
                jumpPoint = GetJumpPointVertical(idx.Col(), idx.Row(), 1);

            if (jumpPoint == null) // No jump point found, return original neighbour.
                return neighbour;
            return jumpPoint;
        }

        private Node GetJumpPointHorizontal(int col, int row, int dCol)
        {
            Node jumpPoint = null;

            while (true) // Each if statement will triggger a return, while true loop is fine.
            {
                if (grid[col + dCol, row].isWall)
                    return jumpPoint; // End of projection, return current jumpPoint. 

                if (!grid[col, row + 1].isWall && grid[col - dCol, row + 1].isWall)
                {
                    // End of projection, set this to jumpPoint and return;
                }

                if (!grid[col, row - 1].isWall && grid[col - dCol, row - 1].isWall)
                {
                    // End of projection, set this to jumpPoint and return;
                }

                col += dCol;
            }
        }

        private Node GetJumpPointVertical(int col, int row, int dRow)
        {

        }
    }
}
