using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Globals {
    public class Vector {
        public int X { get; set; }
        public int Y { get; set; }
        public double Length {
            get {
                return Math.Sqrt(X*X+ Y*Y);
            }
        }
    }
}
