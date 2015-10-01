using System;
using System.Drawing;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Color = System.Windows.Media.Color;

namespace csmacnz.Monocle
{
    public class RayTracer
    {
        public static void RenderSection(Rectangle region, Scene scene, SceneOutput output)
        {
            Vector3D directionUnit = (scene.CameraFacingAt - scene.CameraPosition);
            directionUnit.Normalize();

            foreach (var y in Enumerable.Range(region.Y, region.Height))
            {
                foreach (var x in Enumerable.Range(region.X, region.Width))
                {
                    System.Threading.Thread.Sleep(0);
                    System.Threading.Thread.Sleep(0);
                    System.Threading.Thread.Sleep(0);

                    var point = new Vector3D(((y + 0.5) - (output.Height / 2.0)) * scene.PixelsToUnits, ((x + 0.5) - (output.Width / 2.0)) * scene.PixelsToUnits, 0);
                    var direction = point - scene.CameraPosition;
                    direction.Normalize();
                    var pixelColor = RenderPoint(scene, point, direction);
                    output.SetPixel(x, y, pixelColor);

                    System.Threading.Thread.Sleep(0);
                    System.Threading.Thread.Sleep(0);
                    System.Threading.Thread.Sleep(0);
                }
            }
        }

        private static Color RenderPoint(Scene scene, Vector3D renderPoint, Vector3D direction)
        {
            Vector3D sphereCenter = new Vector3D(350, 350, 150);
            double sphereRadius = 50.0;

            var lineToCircle = renderPoint - sphereCenter;
            var B = 2*Vector3D.DotProduct(direction, lineToCircle);
            var C = lineToCircle.LengthSquared - Math.Pow(sphereRadius, 2);

            var determinate = Math.Pow(B, 2) - 4*C;
            if (determinate >= 0)
            {
                var diff = Math.Sqrt(determinate)/2;
                var tintersect1 = -B - diff;
                var tintersect2 = -B + diff;

                if (tintersect1 > tintersect2)
                {
                    var temp = tintersect2;
                    tintersect2 = tintersect1;
                    tintersect1 = temp;
                }
                if (tintersect1 < 0)
                {
                    tintersect1 = tintersect2;
                }
                if (tintersect1 >= 0)
                {
                    //intersectionPoint = tintersect1;
                    return Colors.DeepSkyBlue;
                }
            }

            return scene.DefaultColor;
        }
    }
}