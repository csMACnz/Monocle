using System.Windows.Media;
using static csmacnz.Monocle.DoubleHelpers;

namespace csmacnz.Monocle
{
    public struct LightStrength
    {
        public LightStrength(double r, double g, double b)
        {
            R = r;
            G = g;
            B = b;
        }

        public double R { get; set; }
        public double G { get; set; }
        public double B { get; set; }
        public static LightStrength Zero => new LightStrength(0, 0, 0);

        public static LightStrength From(Color color)
        {
            return new LightStrength(color.R/255.0, color.G/255.0, color.B/255.0);
        }

        public Color ToColor()
        {
            return Color.FromRgb(ToClamp255(R), ToClamp255(G), ToClamp255(B));
        }

        public LightStrength ScaledBy(double scale)
        {
            return new LightStrength(scale * R, scale * G, scale * B);
        }

        public LightStrength Clamped()
        {
            return new LightStrength(Clamp01(R), Clamp01(G), Clamp01(B));
        }

        public static LightStrength operator +(LightStrength first, LightStrength second)
        {
            return new LightStrength(first.R + second.R, first.G + second.G, first.B + second.B);
        }

        public static LightStrength operator *(LightStrength first, LightStrength second)
        {
            return new LightStrength(first.R*second.R, first.G*second.G, first.B*second.B);
        }

        public static LightStrength operator *(LightStrength first, double second)
        {
            return first.ScaledBy(second);
        }

        public static LightStrength operator *(double first, LightStrength second)
        {
            return second.ScaledBy(first);
        }
    }
}