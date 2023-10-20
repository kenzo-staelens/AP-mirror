using Globals;
using HelixToolkit.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using System.Windows.Media;
using UtilityFunctions;
namespace WPF_physics_simulator {
    public class RenderingTools {
        public static Transform3DGroup GetTransformation(double AngleX, double AngleY, int cellsize, int Width, int Height) {
            var translateTransform = new TranslateTransform3D(-cellsize * Width / 2, cellsize * Height / 2, 0);

            RotateTransform3D RotateTransform3D_X = new RotateTransform3D();
            AxisAngleRotation3D AxisAngleRotation3d_X = new AxisAngleRotation3D();
            AxisAngleRotation3d_X.Axis = new Vector3D(0, 1, 0);
            AxisAngleRotation3d_X.Angle = Utility.RadiansToDegrees(AngleX);
            RotateTransform3D_X.Rotation = AxisAngleRotation3d_X;

            RotateTransform3D RotateTransform3D_Y = new RotateTransform3D();
            AxisAngleRotation3D AxisAngleRotation3d_Y = new AxisAngleRotation3D();
            AxisAngleRotation3d_Y.Axis = new Vector3D(1, 0, 0);
            AxisAngleRotation3d_Y.Angle = Utility.RadiansToDegrees(AngleY);
            RotateTransform3D_Y.Rotation = AxisAngleRotation3d_Y;

            var transformGroup = new Transform3DGroup();
            transformGroup.Children.Add(translateTransform);
            transformGroup.Children.Add(RotateTransform3D_X);
            transformGroup.Children.Add(RotateTransform3D_Y);
            return transformGroup;
        }

        public static GeometryModel3D CreateGeometry(double x1, double y1, double x2, double y2, double height) {
            return CreateGeometry(x1, y1, x2, y2, height, 0);
        }

        public static GeometryModel3D CreateGeometry(double x1, double y1, double x2, double y2, double height, double height_start) {
            GeometryModel3D model = new() {
                Material = Materials.White,
                Geometry = new MeshGeometry3D {
                    Positions = new Point3DCollection {
                        new Point3D(x1, -y1, height_start),
                        new Point3D(x2, -y1, height_start),
                        new Point3D(x1, -y1, height_start + height),
                        new Point3D(x2, -y1, height_start + height),
                        new Point3D(x1, -y2, height_start),
                        new Point3D(x2, -y2, height_start),
                        new Point3D(x1, -y2, height_start + height),
                        new Point3D(x2, -y2, height_start + height)
                    },
                    TriangleIndices = new Int32Collection {
                        1,3,2,
                        0,1,2,
                        3,1,7,
                        1,5,7,
                        7,5,6,
                        5,4,6,
                        0,2,6,
                        4,0,6,
                        3,7,2,
                        7,6,2,
                        5,1,0,
                        4,5,0
                    },

                },
            };
            return model;
        }
    }
}
