using Components;

namespace Globals {
    public class Ball {
        public int X, Y;
        public readonly int Size;
        private readonly IComponent[] components;
        public Ball(int x, int y, int size) : this(x, y, size, Array.Empty<IComponent>()) { }

        public Ball(int x, int y, int size, IComponent[] components) {
            this.X = x;
            this.Y = y;
            this.Size = size;
            this.components = components;
        }

        public IComponent? GetComponent(Type ComponentType) {
            if (!ComponentType.IsSubclassOf(typeof(IComponent))) return null;
            foreach (IComponent c in this.components) {
                if (c.GetType() == ComponentType) {
                    return c;
                }
            }
            return null;
        }
    }
}
