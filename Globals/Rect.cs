using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Globals {
    public struct Rect {
        public readonly double x1;
        public readonly double x2;
        public readonly double y1;
        public readonly double y2;
        public readonly int source_cell;
        public bool Mark;
        public bool Collides;
        public Rect(double x1, double y1, double x2, double y2, int source_cell) {
            this.x1 = x1;
            this.y1 = y1;
            this.x2 = x2;
            this.y2 = y2;
            this.source_cell = source_cell;
            this.Mark = false;
        }

        public void mark(bool val) { this.Mark = val; }
        public void collides(bool val) { this.Collides = val; }
    }
}
