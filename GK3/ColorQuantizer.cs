using System;
using System.Windows;
using System.Windows.Media.Imaging;

namespace GK3
{
    public abstract class ColorQuantizer
    {
        protected byte[] Pixels;
        private readonly WriteableBitmap _writeableBitmap;
        protected readonly int Width;
        protected readonly int Height;
        protected readonly int Stride;

        protected class PixelColor
        {
            public byte Red { get; set; }
            public byte Green { get; set; }
            public byte Blue { get; set; }

            protected bool Equals(PixelColor other)
            {
                return Red == other.Red && Green == other.Green && Blue == other.Blue;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((PixelColor) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    var hashCode = Red.GetHashCode();
                    hashCode = (hashCode * 397) ^ Green.GetHashCode();
                    hashCode = (hashCode * 397) ^ Blue.GetHashCode();
                    return hashCode;
                }
            }
        }

        public ColorQuantizer(BitmapImage bitmapImage)
        {
            _writeableBitmap = new WriteableBitmap(bitmapImage);
            Width = (int) bitmapImage.Width;
            Height = (int) bitmapImage.Height;
            Stride = Width * ((this._writeableBitmap.Format.BitsPerPixel + 7) / 8);
            int arraySize = Stride * Height;
            Pixels = new byte[arraySize];
            _writeableBitmap.CopyPixels(Pixels, Stride, 0);
        }

        protected PixelColor GetPixel(int x, int y)
        {
            if(!( 0 <= x && x < Width)) return new PixelColor();
            if(!( 0 <= y && y < Height)) return new PixelColor();
            int j = y * Stride + x * 4;
            return new PixelColor
            {
                Red = Pixels[j],
                Green = Pixels[j + 1],
                Blue = Pixels[j + 2]
            };
        }

        protected void SetPixel(int x, int y, PixelColor pixelColor)
        {
            if (!(0 <= x && x < Width)) return;
            if (!(0 <= y && y < Height)) return;
            int j = y * Stride + x * 4;
            Pixels[j] = pixelColor.Red;
            Pixels[j + 1] = pixelColor.Green;
            Pixels[j + 2] = pixelColor.Blue;
        }

        protected void EachPixel(Action<int, int> action)
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    action(x, y);
                }
            }
        }

        public virtual WriteableBitmap Process()
        {
            Int32Rect rect = new Int32Rect(0, 0, Width, Height);
            _writeableBitmap.WritePixels(rect, Pixels, Stride, 0);
            Pixels = null;
            return _writeableBitmap;
        }
    }
}