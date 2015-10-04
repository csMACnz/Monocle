using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace csmacnz.Monocle
{
    public class Scene
    {
        public static Scene Default()
        {
            return new Scene
            {
                CameraPosition = new Vector3D(0, 0, -10.00),
                CameraFacingAt = new Vector3D(),
                PixelsToUnits = 1,
                DefaultColor = Colors.Firebrick,
                AmbientLight = new LightStrength(0.1, 0.1, 0.1),
                Spheres = new[]
                {
                    new Sphere(new Vector3D(3, 3, 3), 1.50),
                    new Sphere(new Vector3D(-3, -3, 3), 1.50),
                    new Sphere(new Vector3D(3, -3, 3), 1.50),
                    new Sphere(new Vector3D(-3, 3, 3), 1.50)
                },

                Lights = new[]
                {
                    new Light(new Vector3D(0, 0, 3.00), new LightStrength(0.5, 0.5, 0.5)),
                    new Light(new Vector3D(-6.00, 3.00, 3.00), new LightStrength(0.5, 0.5, 0.5)),
                    new Light(new Vector3D(4.00, 4.00, 0), new LightStrength(0.5, 0.5, 0.5)),
                    new Light(new Vector3D(-4.00, -4.00, 0), new LightStrength(0.5, 0.5, 0.5))
                }
            };
        }

        public Light[] Lights { get; set; }

        public Sphere[] Spheres { get; set; }

        public LightStrength AmbientLight { get; set; }

        public Vector3D CameraFacingAt { get; set; }

        public Vector3D CameraPosition { get; set; }

        public double PixelsToUnits { get; set; }
        public Color DefaultColor { get; set; }
    }
}