using System;
using System.Windows.Media.Imaging;

namespace GK3
{
    public class ColorQuantizerErrorDiffusionDithering : ColorQuantizerIndeIndependentPalette
    {
        private double[,] weights = 
        {
            {0,0, 7/16d},
            {3/16d, 5/16d, 1/16d }
        };

        public ColorQuantizerErrorDiffusionDithering(BitmapImage bitmapImage, int kr, int kg, int kb) : base(bitmapImage, kr, kg, kb)
        {
        }

        protected override void ProcessByChanel(Func<int, int, byte> getPixel, Action<int, int, byte> setPixel, Func<byte, byte> approximate, int k)
        {
            Double[,] copy = new double[Width,Height];
            EachPixel((x, y) => copy[x, y] = getPixel(x, y));
            EachPixel((x, y) =>
            {
                double G = copy[x, y];
                double K = approximate((byte)Math.Min(Math.Max(G, 0), 255));
                copy[x, y] = K;
                double err = G-K;
                for (int i = 0; i < 2; i++)
                {
                    for (int j = -1; j < 2; j++)
                    {
                        int nx = x + j;
                        int ny = y + i;

                        if(!(0 <= nx && nx < Width)) continue;
                        if(!(0 <= ny && ny < Height)) continue;

                        copy[nx, ny] += weights[i, j +1] * err;
                    }
                }
            });
            EachPixel((x, y) => setPixel(x, y, (byte) copy[x, y]));
        }
    }
}