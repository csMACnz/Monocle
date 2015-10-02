using System;
using System.Drawing;
using System.Linq;
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
            var sphere = new Sphere(new Vector3D(0, 0, 300), 150);

            var light = new Light(new Vector3D(400, 400, 0));

            double? t = sphere.Test(renderPoint, direction);

            if (t.HasValue)
            {
                var intersectionPoint = renderPoint + direction * t.Value;

                var ambiance = scene.AmbientLight * sphere.DiffuseColor;

                var directionToEye = -direction;
                directionToEye.Normalize(); //should be anyway?

                var additionalLight = CalculateLighting(directionToEye, sphere, intersectionPoint, light);
                return (ambiance + additionalLight).Clamped().ToColor();
            }

            return scene.DefaultColor;
        }

        private static LightStrength CalculateLighting(
            Vector3D directionToEye,
            Sphere sphere,
            Vector3D intersectionPoint,
            Light light)
        {
            var surfaceNormal = sphere.NormalAt(intersectionPoint);

            var directionToLight = light.CenterPoint - intersectionPoint;
            directionToLight.Normalize();

            var ldotn = Clamp01(Vector3D.DotProduct(directionToLight, surfaceNormal));
            var diffuse = sphere.DiffuseColor*light.Intensity*ldotn;
            var H = directionToLight + directionToEye;
            H.Normalize();
            var ldoth = Clamp01(Vector3D.DotProduct(surfaceNormal, H));
            var specular = sphere.MaterialSpecularIntensity*Math.Pow(ldoth, sphere.MaterialSpecularConstant)*
                           light.SpecularColor;
            var x = diffuse + specular;
            return x;
        }
    }
}