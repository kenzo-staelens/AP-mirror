using Components;

namespace Globals {
    public class Cell {
        public int x, y;
        public Cell[] Neighbours;
        public IComponent[] components;

        private bool[] _walls;
        public bool[] walls {
            get { return _walls; }
        }

        public Cell(int x, int y, bool isDefaultSet){
            this.x = x;
            this.y = y;
            this._walls = new bool[]{ isDefaultSet, isDefaultSet, isDefaultSet, isDefaultSet}; // top, right, bottom, left
        }

        public Cell(int x, int y, bool isDefaultSet, Cell[] neighbours) : this(x, y, isDefaultSet) {
            this.Neighbours = neighbours;
        }

        public Cell(int x, int y, bool isDefaultSet, IComponent[] components) : this(x,y,isDefaultSet){
            this.components = components;
        }

        public Cell(int x, int y, bool isDefaultSet, Cell[] neighbours, IComponent[] components) : this(x, y, isDefaultSet){
            this.Neighbours = neighbours;
            this.components = components;
        }

        public void SetWall(int index, bool isSet) { this.SetWall(index, isSet, true); }
        public void SetWall(int index, bool isSet, bool cascades) {
            this.walls[index] = isSet;
            if(cascades && this.Neighbours != null && this.Neighbours[index]!=null){
                //copy set to neighbouring cell if exists
                this.Neighbours[index].SetWall((index + 2) % 4, isSet, false);
            }
        }
        public IComponent? GetComponent(Type ComponentType) {
            foreach(IComponent c in this.components) {
                if(c.GetType() == ComponentType) {
                    return c;
                }
            }
            return null;
        }
    }
}