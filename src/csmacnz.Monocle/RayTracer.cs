using System;
using System.Drawing;
using System.Linq;
using System.Threading;
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
                    Thread.Sleep(0);
                    Thread.Sleep(0);
                    Thread.Sleep(0);

                    var point = new Vector3D(((x + 0.5) - (output.Width / 2.0)) * scene.PixelsToUnits, ((y + 0.5) - (output.Height / 2.0)) * scene.PixelsToUnits, 0);
                    point = point*0.01D;
                    var direction = point - scene.CameraPosition;
                    direction.Normalize();
                    var pixelColor = RenderPoint(scene, point, direction);
                    output.SetPixel(x, y, pixelColor);

                    Thread.Sleep(0);
                    Thread.Sleep(0);
                    Thread.Sleep(0);
                }
            }
        }

        private static Color RenderPoint(Scene scene, Vector3D renderPoint, Vector3D direction)
        {
            var firstHit = scene.Spheres
                .Select(s=>Tuple.Create(s,s.Test(renderPoint, direction)))
                .Where(h=>h.Item2.HasValue)
                .OrderBy(h=>h.Item2.Value)
                .FirstOrDefault();

            if (firstHit?.Item2 != null)
            {
                var sphere = firstHit.Item1;
                var t = firstHit.Item2.Value;
                var intersectionPoint = renderPoint + direction * t;

                var lighting = scene.AmbientLight * sphere.DiffuseColor;

                var directionToEye = -direction;
                directionToEye.Normalize(); //should be anyway?

                LightStrength additionalLighting = LightStrength.Zero;
                foreach (var light in scene.Lights)
                {
                    additionalLighting += CalculateLighting(scene.Spheres, directionToEye, sphere, intersectionPoint, light);
                }
                return (lighting+ additionalLighting*sphere.DiffuseColor).Clamped().ToColor();

            }

            return scene.DefaultColor;
        }

        private static LightStrength CalculateLighting(Sphere[] spheres, Vector3D directionToEye, Sphere sphere, Vector3D intersectionPoint, Light light)
        {
            var directionToLight = light.CenterPoint - intersectionPoint;
            directionToLight.Normalize();
            var lightT = (light.CenterPoint.X - intersectionPoint.X)/directionToLight.X;
            var blocked = spheres.Any(s => ObscuresLight(s, intersectionPoint, directionToLight, lightT));
            if (blocked)
            {
                return LightStrength.Zero;
            }

            var surfaceNormal = sphere.NormalAt(intersectionPoint);

            var ldotn = Math.Max(0.0,Vector3D.DotProduct(surfaceNormal, directionToLight));
            var diffuse = light.Brightness*ldotn;
            var H = directionToLight + directionToEye;
            H.Normalize();
            var ndoth = Math.Max(0.0,Vector3D.DotProduct(surfaceNormal, H));
            var specular = sphere.SpecularColor*Math.Pow(ndoth, sphere.Shinyness);
            var x = diffuse + specular;
            return x.Clamped();
        }

        private static bool ObscuresLight(Sphere s, Vector3D intersectionPoint, Vector3D directionToLight, double lightT)
        {
            var test = s.Test(intersectionPoint+(directionToLight*0.001), directionToLight);
            return test.HasValue && test > 0 && test <= lightT;
        }
    }
}