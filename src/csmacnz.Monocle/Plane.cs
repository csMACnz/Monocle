using System.Windows.Media.Media3D;

namespace csmacnz.Monocle
{
    public class Plane : IGeometry
    {

        public Plane(Vector3D pointOnPlane, Vector3D normal, Material material)
        {
            Material = material;
            Point = pointOnPlane;
            Normal = normal;
        }

        public Vector3D Point { get; set; }

        public Vector3D Normal { get; set; }

        public Vector3D NormalAt(Vector3D intersectionPoint)
        {
            return Normal;
        }

        public double? Test(Vector3D renderPoint, Vector3D direction)
        {
            var ndotd = Vector3D.DotProduct(Normal, direction);
            if (ndotd != 0)
            {
                var ndoteq = Vector3D.DotProduct(Normal, (Point - renderPoint));
                var t = ndoteq/ndotd;
                if (t >= 0) return t;

                return null;
            }

            return null;
        }

        public Material Material { get; set; }
    }
}