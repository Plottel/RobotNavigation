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
        public static float GetGScore(Node current, Dictionary<Node, Node> parents, Dictionary<Node, AStarSearch.NodeScore> scores)
        {
            var path = RetracePath(current, parents);
            path.Remove(current);

            float score = 0;

            foreach (var node in path)
                score += scores[node].g;

            return score;
        }

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

            path.Add(current);
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

        public static SearchSnapshot MakeSnapshot(NodeGrid grid, 
                                                Node current, 
                                                IEnumerable<Node> open, 
                                                IEnumerable<Node> closed, 
                                                Dictionary<Node, Node> parents)
        {
            var snapshot = new SearchSnapshot();
            snapshot.grid = grid;
            snapshot.openIndexes = NodesToNodeIndexes(open, grid);
            snapshot.closedIndexes = NodesToNodeIndexes(closed, grid);
            snapshot.pathIndexes = NodesToNodeIndexes(RetracePath(current, parents), grid);
            snapshot.pathDirections = PathIndexesToPathDirections(snapshot.pathIndexes);

            return snapshot;
        }

        private static List<string> PathIndexesToPathDirections(List<Point> pathIndexes)
        {
            var result = new List<string>();

            for (int i = 1; i < pathIndexes.Count; ++i)
                result.Add(NodeIndexesToMoveDirection(pathIndexes[i - 1], pathIndexes[i]));

            return result;
        }

        private static string NodeIndexesToMoveDirection(Point from, Point to)
        {
            // Only orthogonal movement allowed, don't need to worry about 8 directions
            if (from.Col() < to.Col())
                return "RIGHT";
            else if (from.Col() > to.Col())
                return "LEFT";
            else if (from.Row() < to.Row())
                return "DOWN";
            return "UP";
        }
    }
}