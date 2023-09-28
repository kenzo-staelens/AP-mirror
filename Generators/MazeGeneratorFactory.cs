using Globals;
using Components;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generators {
    public class MazeGeneratorFactory : IMazeGeneratorFactory {
        public IMazeGenerator Create(MazeGeneratorTypes type, MazeConstructionComponent constructionData) {
            switch(type){
                case MazeGeneratorTypes.Static:
                    return new StaticGenerator(constructionData.Filename);//automatically breaks on return
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
