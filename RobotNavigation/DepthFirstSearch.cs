﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace RobotNavigation
{
    public class DepthFirstSearch : ISearch
    {
        public List<Node> Path { get; set; }

        public DepthFirstSearch()
        {
            Path = new List<Node>();
        }

        public Stack<ISearchSnapshot> Search(NodeGrid grid)
        {
            var snapshots = new Stack<ISearchSnapshot>();

            var parents = new Dictionary<Node, Node>();
            var open = new Stack<Node>();
            var closed = new List<Node>();
            Node current = null;

            open.Push(grid.startNode);

            // Search
            while (open.Count > 0)
            {
                current = open.Pop();

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
                        open.Push(neighbour);
                        parents[neighbour] = current;
                    }
                }
            }

            Path = SearchUtils.RetracePath(current, parents);

            return snapshots;
        }        
    }
}
