using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Globals;

namespace Generators {
    internal interface IMazeGeneratorFactory {
        abstract IMazeGenerator Create(MazeGeneratorTypes type);
    }
}
