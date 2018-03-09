using System;
using System.Linq;
using System.Windows.Media.Imaging;

namespace GK3
{
    public class ColorQuantizerOrderedDithering1 : ColorQuantizerIndeIndependentPalette
    {
        public ColorQuantizerOrderedDithering1(BitmapImage bitmapImage, int kr, int kg, int kb) : base(bitmapImage, kr, kg, kb)
        { 
        }

        protected override void ProcessByChanel(Func<int, int, byte> getPixel, Action<int, int, byte> setPixel, Func<byte, byte> approximate, int k)
        {
            int[,] pattern = PreparePattern(k);
            int n = pattern.GetLength(0);

            Random r = new Random();

            EachPixel((x, y) =>
            {
                int I = (int) (getPixel(x, y) / 255d * n * n * (k - 1));
                int col = I / (n * n);
                int re = I % (n * n);
                int i = r.Next(n - 1);
                int j = r.Next(n - 1);
                if (re > pattern[i, j])
                    col++;
                setPixel(x, y, (byte) (col * 255d / (k-1)));

            });
        }

        protected int[,] PreparePattern(int k)
        {
            int n = (int)Math.Ceiling(Math.Sqrt(256d / (k - 1)));
            int[] rounds = {2, 3, 4, 6, 8, 12, 16};
            return GeneratePattern(rounds.First(x => n <= x));
        }

        protected int[,] GeneratePattern(int n)
        {
            if (n == 2)
                return new[,]
                {
                    {0, 2},
                    {3, 1}
                };

            if (n == 3)
                return new[,]
                {
                    {6, 8, 4 },
                    {1, 0, 3 },
                    {5, 2, 7 }
                };

            if(n%2 == 1)
                throw new Exception("Size not supported");

            int[,] nPattern = GeneratePattern(n / 2);
            int[,] n2Pattern = new int[n, n];

            for (int y = 0; y < n; y++)
            {
                for (int x = 0; x < n; x++)
                {
                    if (x < n / 2 && y < n / 2)
                    {
                        n2Pattern[x, y] = 4 * nPattern[x, y];
                    }
                    else if (x < n / 2)
                    {
                        n2Pattern[x, y] = 4 * nPattern[x, y - n / 2] + 3;
                    }
                    else if (y < n / 2)
                    {
                        n2Pattern[x, y] = 4 * nPattern[x - n / 2, y] + 2;
                    }
                    else
                    {
                        n2Pattern[x, y] = 4 * nPattern[x - n / 2, y - n / 2] + 1;
                    }
                }
            }

            return n2Pattern;
        }
    }
}