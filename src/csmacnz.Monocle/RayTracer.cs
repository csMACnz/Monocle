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

            var pixelHeight = scene.ViewPortHeight / output.Height;
            var pixelWidth = pixelHeight;
            var halfWidth = (output.Width / 2.0);
            var halfHeight = (output.Height / 2.0);

            foreach (var y in Enumerable.Range(region.Y, region.Height))
            {
                foreach (var x in Enumerable.Range(region.X, region.Width))
                {
                    Thread.Sleep(0);
                    Thread.Sleep(0);
                    Thread.Sleep(0);

                    var point = new Vector3D(
                        ((x + 0.5) - halfWidth)*pixelWidth,
                        ((y + 0.5) - halfHeight)*pixelHeight, 0);

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
            var firstHit = scene.Objects
                .Select(s=>Tuple.Create(s,s.Test(renderPoint, direction)))
                .Where(h=>h.Item2.HasValue)
                .OrderBy(h=>h.Item2.Value)
                .FirstOrDefault();

            if (firstHit?.Item2 != null)
            {
                var sphere = firstHit.Item1;
                var t = firstHit.Item2.Value;
                var intersectionPoint = renderPoint + direction * t;

                var directionToEye = -direction;
                directionToEye.Normalize(); //should be anyway?


                var material = sphere.Material;
                var surfaceNormal = sphere.NormalAt(intersectionPoint);

                return CalculateColor(scene, directionToEye, surfaceNormal, material, intersectionPoint);
            }

            return scene.DefaultColor;
        }

        private static Color CalculateColor(Scene scene, Vector3D directionToEye, Vector3D surfaceNormal, Material material,
            Vector3D intersectionPoint)
        {
            double specular = 0.0;
            LightStrength additionalLighting = LightStrength.Zero;
            foreach (var light in scene.Lights)
            {
                var lighting = CalculateLighting(scene.Objects, directionToEye, surfaceNormal, material, intersectionPoint,
                    light);
                additionalLighting += lighting.Item1;
                specular += lighting.Item2;
            }
            var litColor = ((scene.AmbientLight + additionalLighting).Clamped()*material.DiffuseColor);
            var totalSpecular = (Math.Min(1.0, specular)*material.SpecularColor);
            return (litColor + totalSpecular).ToColor();
        }

        private static Tuple<LightStrength, double> CalculateLighting(IGeometry[] spheres, Vector3D directionToEye, Vector3D surfaceNormal, Material sphere, Vector3D intersectionPoint, Light light)
        {
            var directionToLight = light.CenterPoint - intersectionPoint;
            directionToLight.Normalize();
            var lightT = (light.CenterPoint.X - intersectionPoint.X)/directionToLight.X;
            var blocked = spheres.Any(s => ObscuresLight(s, intersectionPoint, directionToLight, lightT));
            if (blocked)
            {
                return Tuple.Create(LightStrength.Zero,0.0);
            }

            var ldotn = Math.Max(0.0,Vector3D.DotProduct(surfaceNormal, directionToLight));
            var diffuse = light.Brightness*ldotn;
            var H = directionToLight + directionToEye;
            H.Normalize();
            var ndoth = Math.Max(0.0,Vector3D.DotProduct(surfaceNormal, H));
            var specular = Math.Pow(ndoth, sphere.Shinyness);
            return Tuple.Create(diffuse, specular);
        }

        private static bool ObscuresLight(IGeometry s, Vector3D intersectionPoint, Vector3D directionToLight, double lightT)
        {
            var test = s.Test(intersectionPoint+(directionToLight*0.001), directionToLight);
            return test.HasValue && test > 0 && test <= lightT;
        }
    }
}