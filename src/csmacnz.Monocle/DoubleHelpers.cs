namespace csmacnz.Monocle
{
    public static class DoubleHelpers
    {
        public static double Clamp01(double value)
        {
            return value > 1 ? 1 : value < 0 ? 0 : value;
        }

        public static byte ToClamp255(double value)
        {
            value = value*255;
            if (value > 255) return 255;
            return (byte)value;
        }
    }
}