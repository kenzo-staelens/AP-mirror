using Globals;
using Components;

namespace Generators {
    internal interface IMazeGeneratorFactory {
        abstract IMazeGenerator Create(MazeGeneratorTypes type, MazeConstructionComponent constructionData);
    }
}
