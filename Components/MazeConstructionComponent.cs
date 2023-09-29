﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components {
    public class MazeConstructionComponent {
        public int Width { get; }
        public int Height { get; }
        public string Filename { get; }
        public MazeConstructionComponent(int width, int height, string filename) {
            Width = width;
            Height = height;
            Filename = filename;
        }
    }
}