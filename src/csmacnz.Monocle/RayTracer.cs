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
            var spheres = new[]
            {
                new Sphere(new Vector3D(300, 300, 300), 150),
                new Sphere(new Vector3D(-300, -300, 300), 150),
                new Sphere(new Vector3D(300, -300, 300), 150),
                new Sphere(new Vector3D(-300, 300, 300), 150)
            };

            var lights = new[]
            {
                new Light(new Vector3D(-600, 0, 300)),
                new Light(new Vector3D(400, 400, 0)),
                new Light(new Vector3D(-400, -400, 0))
            };
            var firstHit = spheres
                .Select(s=>Tuple.Create(s,s.Test(renderPoint, direction)))
                .Where(h=>h.Item2.HasValue)
                .OrderBy(h=>h.Item2.Value)
                .FirstOrDefault();
            
            if (firstHit != null && firstHit.Item2.HasValue)
            {
                var sphere = firstHit.Item1;
                var t = firstHit.Item2.Value;
                var intersectionPoint = renderPoint + direction * t;

                var lighting = scene.AmbientLight * sphere.DiffuseColor;

                var directionToEye = -direction;
                directionToEye.Normalize(); //should be anyway?

                foreach (var light in lights)
                {
                    lighting += CalculateLighting(directionToEye, sphere, intersectionPoint, light);
                }
                return (lighting).Clamped().ToColor();
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