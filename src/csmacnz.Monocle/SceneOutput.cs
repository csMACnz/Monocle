using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace csmacnz.Monocle
{
    public class SceneOutput
    {
        public SceneOutput(int width, int height)
        {
            Width = width;
            Height = height;
            PixelFormat = PixelFormats.Rgb24;
            var bitsPerPixel = PixelFormat.BitsPerPixel;

            RawStride = (Width * bitsPerPixel + 7) / 8;
            PixelData = new byte[RawStride * Height];
        }

        public byte[] PixelData { get; }

        public int RawStride { get; }

        public PixelFormat PixelFormat { get; }

        public int Height { get; }

        public int Width { get; }

        public ImageSource CreateBitmap()
        {
            return BitmapSource.Create(Width, Height,
                96, 96, PixelFormat, null, PixelData, RawStride);
        }

        public void SetPixel(int x, int y, Color pixelColor)
        {
            int xIndex = x * 3;
            int yIndex = y * RawStride;
            PixelData[xIndex + yIndex] = pixelColor.R;
            PixelData[xIndex + yIndex + 1] = pixelColor.G;
            PixelData[xIndex + yIndex + 2] = pixelColor.B;
        }
    }
}