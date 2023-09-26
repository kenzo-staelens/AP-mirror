namespace Globals {
    public class Cell {
        int x, y;
        public Cell[] Neighbours;

        public Cell(int x, int y){
            this.x = x;
            this.y = y;
        }

        public Cell(int x, int y, Cell[] Neighbours) : this(x,y){
            this.Neighbours = Neighbours;
            
        }
    }
}