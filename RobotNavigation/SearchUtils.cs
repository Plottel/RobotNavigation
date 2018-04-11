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
        public const int MIN_G_SCORE = 2;

        public static float GetGScore(Node current, Dictionary<Node, Node> parents, Dictionary<Node, AStarSearch.NodeScore> scores)
        {
            return scores[parents[current]].g + MIN_G_SCORE;

            var path = RetracePath(current, parents);
            path.Remove(current);

            float score = 0;

            foreach (var node in path)
                score += scores[node].g;

            return score + MIN_G_SCORE;
        }

        public static AStarSearch.NodeScore GetScore(Node current, Node target, Dictionary<Node, Node> parents, Dictionary<Node, AStarSearch.NodeScore> scores)
        {
            var result = new AStarSearch.NodeScore();
            result.g = GetGScore(current, parents, scores);

            // Euclidian

            //result.h = Vector2.Distance(
            //                current.Bounds.Center.ToVector2(),
            //                target.Bounds.Center.ToVector2()
            //                );

            // Manhattan
            var curIdx = Game1.Instance.grid.IndexOf(current);
            var targetIdx = Game1.Instance.grid.IndexOf(target);
            int colOffset = Math.Abs(curIdx.Col() - targetIdx.Col());
            int rowOffset = Math.Abs(curIdx.Row() - targetIdx.Row());

            result.h = colOffset + rowOffset;
            result.h *= 0.9f;

            result.f = result.g + result.h;

            return result;
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

        public static AStarSearchSnapshot MakeAStarSnapshot(NodeGrid grid,
                                                            Node current,
                                                            IEnumerable<Node> open,
                                                            IEnumerable<Node> closed,
                                                            Dictionary<Node, Node> parents,
                                                            Dictionary<Node, AStarSearch.NodeScore> scores)
        {
            var snapshot = new AStarSearchSnapshot();
            snapshot.grid = grid;
            snapshot.openIndexes = NodesToNodeIndexes(open, grid);
            snapshot.closedIndexes = NodesToNodeIndexes(closed, grid);
            snapshot.pathIndexes = NodesToNodeIndexes(RetracePath(current, parents), grid);
            snapshot.pathDirections = PathIndexesToPathDirections(snapshot.pathIndexes);

            snapshot.scoreIndexes = new Dictionary<Point, AStarSearch.NodeScore>();
            // Add scores
            foreach (var nodeScoreMap in scores)
                snapshot.scoreIndexes[grid.IndexOf(nodeScoreMap.Key)] = nodeScoreMap.Value;

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