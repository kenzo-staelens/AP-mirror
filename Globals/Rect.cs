using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Globals {
    public struct Rect {
        public readonly int x1;
        public readonly int x2;
        public readonly int y1;
        public readonly int y2;
        public readonly int source_cell;
        public Rect(int x1, int y1, int x2, int y2, int source_cell) {
            this.x1 = x1;
            this.y1 = y1;
            this.x2 = x2;
            this.y2 = y2;
            this.source_cell = source_cell;
        }
    }
}
