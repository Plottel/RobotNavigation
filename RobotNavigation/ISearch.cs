using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotNavigation
{
    public interface ISearch
    {
        Queue<ISearchSnapshot> Search(NodeGrid grid);
    }
}
