namespace Globals {
    public class Cell {
        int x, y;
        public Cell[] Neighbours;

        private bool[] _walls;
        public bool[] walls {
            get { return _walls; }
        }

        public Cell(int x, int y, bool isDefaultSet){
            this.x = x;
            this.y = y;
            this._walls = new bool[]{ isDefaultSet, isDefaultSet, isDefaultSet, isDefaultSet}; // top, right, bottom, left
        }

        public Cell(int x, int y, bool isDefaultSet, Cell[] Neighbours) : this(x, y, isDefaultSet) {
            this.Neighbours = Neighbours;
            
        }

        public void SetWall(int index, bool isSet) { this.SetWall(index, isSet, true); }
        public void SetWall(int index, bool isSet, bool cascades) {
            this.walls[index] = isSet;
            if(cascades && this.Neighbours != null && this.Neighbours[index]!=null){
                //copy set to neighbouring cell if exists
                this.Neighbours[index].SetWall((index + 2) % 4, isSet, false);
            }
        }
    }
}