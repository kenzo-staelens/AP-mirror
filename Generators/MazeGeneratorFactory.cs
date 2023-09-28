using Globals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generators {
    public class MazeGeneratorFactory : IMazeGeneratorFactory {
        public IMazeGenerator Create(MazeGeneratorTypes type) {
            switch(type){
                case MazeGeneratorTypes.Static:
                default:
                    return new StaticGenerator("./testmaze.txt");//automatically breaks on return
            }
        }
    }
}
