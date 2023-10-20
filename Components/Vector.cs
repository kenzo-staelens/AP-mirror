using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components {
    public class Vector {
        public double X { get; set; }
        public double Y { get; set; }
        public double Length {
            get {
                return Math.Sqrt(X*X+ Y*Y);
            }
        }

        public Vector() : this(0, 0) { }

        public Vector(double X, double Y) {
            this.X = X;
            this.Y = Y;
        }
    }
}
