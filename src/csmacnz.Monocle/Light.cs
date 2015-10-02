using System.Windows.Media.Media3D;

namespace csmacnz.Monocle
{
    public class Light
    {
        public Light(Vector3D centerPoint)
        {
            CenterPoint = centerPoint;
            Color = new LightStrength(0.8, 0.8, 0.8);
        }

        public LightStrength Color { get; set; }

        public Vector3D CenterPoint { get; set; }
    }
}