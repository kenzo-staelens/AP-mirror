namespace Components {
    public struct WallDataComponent {
        public int Width {
            get;
            private set;
        }
        public WallDataComponent(int width) {
            this.Width = width;
        }
    }
}