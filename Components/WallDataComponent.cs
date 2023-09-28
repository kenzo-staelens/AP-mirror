namespace Components {
    public struct WallDataComponent : IComponent {
        public int Width {
            get;
            private set;
        }
        public WallDataComponent(int width) {
            this.Width = width;
        }
    }
}