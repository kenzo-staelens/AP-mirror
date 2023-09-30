using Components;

namespace Globals {
    public class Cell {
        public int x, y;
        public Cell[] Neighbours;
        private readonly IComponent[] components;

        public readonly bool[] Walls;

        public Cell(int x, int y, bool isDefaultSet) : this(x, y, isDefaultSet, Array.Empty<Cell>(), Array.Empty<IComponent>()) {

        }

        public Cell(int x, int y, bool isDefaultSet, Cell[] neighbours) : this(x, y, isDefaultSet, neighbours, Array.Empty<IComponent>()) {}

        public Cell(int x, int y, bool isDefaultSet, IComponent[] components) : this(x, y, isDefaultSet, Array.Empty<Cell>(), components) {}

        public Cell(int x, int y, bool isDefaultSet, Cell[] neighbours, IComponent[] components) {
            this.Neighbours = neighbours;
            this.components = components;
            this.x = x;
            this.y = y;
            this.Walls = new bool[] { isDefaultSet, isDefaultSet, isDefaultSet, isDefaultSet }; // top, right, bottom, left
            this.components = new IComponent[components.Length];
            for (int i = 0; i < components.Length; i++) {
                this.components[i] = components[i].Clone();
            }
        }

        public void SetWall(int index, bool isSet) { this.SetWall(index, isSet, true); }
        public void SetWall(int index, bool isSet, bool cascades) {
            this.Walls[index] = isSet;
            if (cascades && this.Neighbours != null && this.Neighbours[index] != null) {
                //copy set to neighbouring cell if exists
                this.Neighbours[index].SetWall((index + 2) % 4, isSet, false);
            }
        }
        public IComponent? GetComponent(Type ComponentType) {
            if (!ComponentType.GetInterfaces().Contains(typeof(IComponent))) return null;
            foreach (IComponent c in this.components) {
                if (c.GetType() == ComponentType) {
                    return c;
                }
            }
            return null;
        }
    }
}