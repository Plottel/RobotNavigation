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
        private Queue<ISearchSnapshot> snapshots;
        private List<Node> open;

        public Queue<ISearchSnapshot> Search(NodeGrid grid)
        {
            this.grid = grid;
            snapshots = new Queue<ISearchSnapshot>();

            var parents = new Dictionary<Node, Node>();
            open = new List<Node>();
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

            return snapshots;
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
            Point startIdx = new Point(col, row);

            while (true)
            {
                AddCalculationSnapshot(startIdx, new Point(col, row));

                if (grid[col + dCol, row] == null || grid[col + dCol, row].isWall)
                    break;

                var beNotWall = grid[col, row + 1];
                var beWall = grid[col - dCol, row + 1];

                if (beWall != null & beNotWall != null && beWall.isWall && !beNotWall.isWall)
                    break;

                beNotWall = grid[col, row - 1];
                beWall = grid[col - dCol, row - 1];

                if (beWall != null & beNotWall != null && beWall.isWall && !beNotWall.isWall)
                    break;

                col += dCol;
            }

            return grid[col, row];
        }

        private Node GetJumpPointVertical(int col, int row, int dRow)
        {
            Point startIdx = new Point(col, row);

            while (true)
            {
                AddCalculationSnapshot(startIdx, new Point(col, row));

                if (grid[col, row + dRow] == null || grid[col, row + dRow].isWall)
                    break;

                var beNotWall = grid[col + 1, row];
                var beWall = grid[col + 1, row - dRow];

                if (beWall != null & beNotWall != null && beWall.isWall && !beNotWall.isWall)
                    break;

                beNotWall = grid[col - 1, row];
                beWall = grid[col - 1, row - dRow];

                if (beWall != null & beNotWall != null && beWall.isWall && !beNotWall.isWall)
                    break;

                row += dRow;
            }

            return grid[col, row];
        }

        private void AddCalculationSnapshot(Point nodeIdx, Point jumpIdx)
        {
            snapshots.Enqueue
            (
                new JPSCalculationSnapshot
                {
                    startIdx = nodeIdx,
                    jumpIdx = jumpIdx,
                    grid = grid,
                    openIndexes = SearchUtils.NodesToNodeIndexes(open, grid)
                }
            );
        }
    }
}
