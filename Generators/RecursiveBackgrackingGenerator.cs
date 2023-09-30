using Components;
using Globals;

namespace Generators {
    internal class RecursiveBackgrackingGenerator : IMazeGenerator {
        private readonly int width, height;
        private readonly IComponent[] extraComponents;
        private readonly Random random;
        public RecursiveBackgrackingGenerator(int width, int height, IComponent[] extraComponents) {
            this.width = width;
            this.height = height;
            var temp = extraComponents.ToList();
            temp.Add(new GeneratorRequirementComponent());
            this.extraComponents = temp.ToArray();
            this.random = new Random();
        }

        public Maze Generate() {
            Maze maze = new(this.width, this.height, true, this.extraComponents);
            Stack<Cell> stack = new();
            Cell randomCell = maze.maze.OfType<Cell>().Skip(random.Next(maze.maze.Length)).First();
            //Cell randomCell = maze.maze[0, 0];
            GeneratorRequirementComponent? cellDataStore = (GeneratorRequirementComponent?)randomCell.GetComponent(typeof(GeneratorRequirementComponent));
            if (cellDataStore != null) ((GeneratorRequirementComponent)cellDataStore)["visited"] = true;
            stack.Push(randomCell);
            while (stack.Count > 0) {
#pragma warning disable CS8605 // Unboxing a possibly null value.
                //get unvisited neighbour list
                randomCell = stack.Peek();//last item on stack
                var randomCellValidNeighbours = randomCell.Neighbours.Where(x => x != null)
                    .Where(x => ((GeneratorRequirementComponent?)x.GetComponent(typeof(GeneratorRequirementComponent))) != null)
                    .Where(x => (bool?)((GeneratorRequirementComponent)x.GetComponent(typeof(GeneratorRequirementComponent)))["visited"] == null ||
                    (bool)((GeneratorRequirementComponent)x.GetComponent(typeof(GeneratorRequirementComponent)))["visited"] == false);
#pragma warning restore CS8605 // Unboxing a possibly null value.
                if (randomCellValidNeighbours.Any()) {//if such neighbour exists, select a random neighbour and set as visited
                    var memory = randomCell;
                    randomCell = randomCellValidNeighbours.Skip(random.Next(randomCellValidNeighbours.Count())).Take(1).First();
                    cellDataStore = (GeneratorRequirementComponent?)randomCell.GetComponent(typeof(GeneratorRequirementComponent));
                    if (cellDataStore != null) ((GeneratorRequirementComponent)cellDataStore)["visited"] = true;
                    int wallIndex = Array.IndexOf(memory.Neighbours, randomCell);
                    memory.SetWall(wallIndex, false);
                    stack.Push(randomCell);
                }
                else {
                    stack.Pop();
                }
            }
            return maze;
        }
    }
}
