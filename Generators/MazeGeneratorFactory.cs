using Globals;
using Components;

namespace Generators {
    public class MazeGeneratorFactory : IMazeGeneratorFactory {

        private readonly IComponent[] CellComponents;
        public MazeGeneratorFactory() : this(new IComponent[0]){}

        public MazeGeneratorFactory(IComponent[] CellComponents) {
            this.CellComponents = CellComponents;
        }
        public IMazeGenerator Create(MazeGeneratorTypes type, MazeConstructionComponent constructionData) {
            switch(type) {
                case MazeGeneratorTypes.Static:
                    return new StaticGenerator(constructionData.Filename, this.CellComponents);//automatically breaks on return
                case MazeGeneratorTypes.Additive:
                    return new RecursiveDivisionGenerator(constructionData.Width, constructionData.Height, this.CellComponents);
                case MazeGeneratorTypes.Destructive:
                    return new RecursiveBackgrackingGenerator(constructionData.Width, constructionData.Height, this.CellComponents);
                default:
                    throw new NotImplementedException();
            };
        }
    }
}
