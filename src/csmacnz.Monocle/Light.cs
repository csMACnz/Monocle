using System.Windows.Media.Media3D;

namespace csmacnz.Monocle
{
    public class Light
    {
        public Light(Vector3D centerPoint)
        {
            CenterPoint = centerPoint;
            Intensity = new LightStrength(1, 1, 1);
            SpecularFactor = 1;
        }

        public LightStrength SpecularColor => Intensity*SpecularFactor;

        public int SpecularFactor { get; set; }

        public LightStrength Intensity { get; set; }

        public Vector3D CenterPoint { get; set; }
    }
}