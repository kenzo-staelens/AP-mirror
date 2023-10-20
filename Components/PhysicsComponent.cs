using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components {
    public class PhysicsComponent : IComponent {
        public Vector Velocity;
        public Vector Acceleration;
        public Vector Force;
        public PhysicsComponent() {
            this.Velocity = new();
            this.Acceleration = new();
            this.Force = new();
        }

        public IComponent Clone() {
            PhysicsComponent pc = new();
            pc.Velocity.X = this.Velocity.X;
            pc.Velocity.Y = this.Velocity.Y;
            pc.Acceleration.X = this.Acceleration.X;
            pc.Acceleration.Y = this.Acceleration.Y;
            pc.Force.X = this.Force.X;
            pc.Force.Y = this.Force.Y;
            return pc;
        }
    }
}
