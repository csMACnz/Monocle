using System.Linq;
using System.Windows.Media;

namespace csmacnz.Monocle
{
    public class RayTracer
    {
        public static void RenderSection(int minVerticalPixel, int verticalCount, int minHorizontalPixel, int horizontalCount, SceneOutput output)
        {
            foreach (var y in Enumerable.Range(minVerticalPixel, verticalCount))
            {
                foreach (var x in Enumerable.Range(minHorizontalPixel, horizontalCount))
                {
                    System.Threading.Thread.Sleep(0);
                    System.Threading.Thread.Sleep(0);
                    System.Threading.Thread.Sleep(0);

                    output.SetPixel(x, y, Colors.Firebrick);

                    System.Threading.Thread.Sleep(0);
                    System.Threading.Thread.Sleep(0);
                    System.Threading.Thread.Sleep(0);
                }
            }
        }
    }
}