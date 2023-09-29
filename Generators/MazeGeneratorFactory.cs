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
            return type switch {
                MazeGeneratorTypes.Static => new StaticGenerator(constructionData.Filename),//automatically breaks on return
                MazeGeneratorTypes.Additive => new RecursiveDivisionGenerator(constructionData.Width, constructionData.Height),
                _ => throw new NotImplementedException(),
            };
        }
    }
}
