using Components;
using Globals;

namespace Generators {
    internal interface IMazeGeneratorFactory {
        abstract IMazeGenerator Create(MazeGeneratorTypes type, MazeConstructionComponent constructionData);
    }
}
