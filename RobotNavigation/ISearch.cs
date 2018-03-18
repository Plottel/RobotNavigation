using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotNavigation
{
    public interface ISearch
    {
        List<Node> Open { get; }
        List<Node> Closed { get; }
        List<Node> Path { get; }

        void Search(NodeGrid grid);

    }
}
