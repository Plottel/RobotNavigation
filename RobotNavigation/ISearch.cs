using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotNavigation
{
    public interface ISearch
    {
        List<Node> Path { get; set; }
        Queue<ISearchSnapshot> Search(NodeGrid grid);
    }
}
