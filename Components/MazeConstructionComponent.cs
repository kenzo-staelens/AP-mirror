namespace Components {
    public class MazeConstructionComponent : IComponent {
        public int Width { get; }
        public int Height { get; }
        public string Filename { get; }
        public MazeConstructionComponent(int width, int height, string filename) {
            Width = width;
            Height = height;
            Filename = filename;
        }

        public IComponent Clone() {
            return new MazeConstructionComponent(Width, Height, Filename);
        }
    }
}
