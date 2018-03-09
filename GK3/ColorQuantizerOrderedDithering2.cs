using System;
using System.Windows.Media.Imaging;

namespace GK3
{
    public class ColorQuantizerOrderedDithering2 : ColorQuantizerOrderedDithering1
    {
        public ColorQuantizerOrderedDithering2(BitmapImage bitmapImage, int kr, int kg, int kb) : base(bitmapImage, kr, kg, kb)
        {
        }

        protected override void ProcessByChanel(Func<int, int, byte> getPixel, Action<int, int, byte> setPixel, Func<byte, byte> approximate, int k)
        {
            int[,] pattern = PreparePattern(k);
            int n = pattern.GetLength(0);

            EachPixel((x, y) =>
            {
                int I = (int)(getPixel(x, y) / 255d * n * n * (k - 1));
                int col = I / (n * n);
                int re = I % (n * n);
                int i = x % n;
                int j = y % n;
                if (re > pattern[i, j])
                    col++;
                setPixel(x, y, (byte)(col * 255d / (k - 1)));

            });
        }
    }
}