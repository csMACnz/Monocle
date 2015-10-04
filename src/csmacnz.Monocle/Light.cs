using System.Windows.Media.Media3D;

namespace csmacnz.Monocle
{
    public class Light
    {
        public Light(Vector3D centerPoint, LightStrength brightness)
        {
            CenterPoint = centerPoint;
            Brightness = brightness;
        }

        public LightStrength Brightness { get; set; }

        public Vector3D CenterPoint { get; set; }
    }
}