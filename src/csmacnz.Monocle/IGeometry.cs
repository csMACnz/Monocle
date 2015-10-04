using System.Windows.Media.Media3D;

namespace csmacnz.Monocle
{
    public interface IGeometry
    {
        Vector3D NormalAt(Vector3D intersectionPoint);
        double? Test(Vector3D renderPoint, Vector3D direction);
        Material Material { get; set; }
    }
}