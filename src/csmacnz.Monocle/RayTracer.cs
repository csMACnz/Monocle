using System;
using System.Drawing;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Color = System.Windows.Media.Color;
using static csmacnz.Monocle.DoubleHelpers;

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

                    var point = new Vector3D(((x + 0.5) - (output.Width / 2.0)) * scene.PixelsToUnits, ((y + 0.5) - (output.Height / 2.0)) * scene.PixelsToUnits, 0);
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
            Vector3D sphereCenter = new Vector3D(0, 0, 150);
            var sphereDiffuseColor = LightStrength.From(Colors.DeepSkyBlue);
            double sphereRadius = 150.0;
            int sphereMaterialSpecularConstant = 500;
            LightStrength sphereMaterialSpecularIntensity = new LightStrength(1, 1, 1);

            Vector3D lightPosition = new Vector3D(50, 250, -100);
            LightStrength lightIntensity = new LightStrength(1, 1, 1);
            var lightSpecularFactor = 1;
            LightStrength lightSpecularColor = lightIntensity*lightSpecularFactor;

            var lineToCircle = renderPoint - sphereCenter;
            var B = 2*Vector3D.DotProduct(direction, lineToCircle);
            var C = lineToCircle.LengthSquared - Math.Pow(sphereRadius, 2);

            var determinate = Math.Pow(B, 2) - 4*C;
            if (determinate >= 0)
            {
                var diff = Math.Sqrt(determinate);
                var tintersect1 = (-B - diff)/2;
                var tintersect2 = (-B + diff)/2;

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
                    var intersectionPoint = renderPoint + direction*tintersect1;

                    var surfaceNormal = intersectionPoint - sphereCenter;
                    surfaceNormal.Normalize();

                    var directionToLight = lightPosition - intersectionPoint;
                    directionToLight.Normalize();

                    var directionToEye = -direction;
                    directionToEye.Normalize();//should be anyway?

                    var ambiance = scene.AmbientLight*sphereDiffuseColor;
                    var ldotn = Clamp01(Vector3D.DotProduct(directionToLight, surfaceNormal));
                    var diffuse = sphereDiffuseColor * lightIntensity * ldotn;
                    var H = directionToLight + directionToEye;
                    H.Normalize();
                    var ldoth = Clamp01(Vector3D.DotProduct(surfaceNormal, H));
                    var specular = sphereMaterialSpecularIntensity* Math.Pow(ldoth, sphereMaterialSpecularConstant)*
                                   lightSpecularColor;
                    return (ambiance+diffuse+specular).Clamped().ToColor();
                }
            }

            return scene.DefaultColor;
        }
    }
}