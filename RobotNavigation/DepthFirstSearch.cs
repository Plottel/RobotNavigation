using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace RobotNavigation
{
    public class DepthFirstSearch : ISearch
    {
        public void Search(NodeGrid grid)
        {
            //      DFS(G, v)(v is the vertex where the search starts)
            //   Stack S := { }; (start with an empty stack )
            //   for each vertex u, set visited[u] := false;
            //      push S, v;
            //      while (S is not empty) do
            //          u:= pop S;
            //      if (not visited[u]) then
            //         visited[u] := true;
            //      for each unvisited neighbour w of u
            //         push S, w;
            //      end if
            //   end while
            //END DFS()

            var open = new Stack<Node>();



        }
    }
}
