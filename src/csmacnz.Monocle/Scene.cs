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
                CameraPosition = new Vector3D(0, 0, -1000),
                CameraFacingAt = new Vector3D(),
                PixelsToUnits = 1,
                DefaultColor = Colors.Firebrick
            };
        }

        public Vector3D CameraFacingAt { get; set; }

        public Vector3D CameraPosition { get; set; }

        public double PixelsToUnits { get; set; }
        public Color DefaultColor { get; set; }
    }
}