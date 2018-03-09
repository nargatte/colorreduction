using System;
using System.Windows.Media.Imaging;

namespace GK3
{
    public abstract class ColorQuantizerIndeIndependentPalette : ColorQuantizer
    {
        protected readonly int Kr;
        protected readonly int Kg;
        protected readonly int Kb;

        protected ColorQuantizerIndeIndependentPalette(BitmapImage bitmapImage, int kr, int kg, int kb) : base(bitmapImage)
        {
            Kr = kr;
            Kg = kg;
            Kb = kb;
        }

        protected byte Approximate(byte v, int k)
        {
            double range = 255d / ((k-1) * 2 );
            int n = (int)(v / range);
            if (n == 0) return 0;
            n--;
            n /= 2;
            n++;
            return (byte) (255d / (k-1) * n);
        }

        protected abstract void ProcessByChanel(Func<int, int, byte> getPixel, Action<int, int, byte> setPixel, Func<byte, byte> approximate, int k);

        public override WriteableBitmap Process()
        {
            ProcessByChanel((x, y) => GetPixel(x, y).Red, (x, y, v) =>
            {
                if (!(0 <= x && x < Width)) return;
                if (!(0 <= y && y < Height)) return;
                int j = y * Stride + x * 4;
                Pixels[j] = v;
            },
            v => Approximate(v, Kr), Kr);
            ProcessByChanel((x, y) => GetPixel(x, y).Green, (x, y, v) =>
            {
                if (!(0 <= x && x < Width)) return;
                if (!(0 <= y && y < Height)) return;
                int j = y * Stride + x * 4;
                Pixels[j + 1] = v;
            },
            v => Approximate(v, Kg), Kg);
            ProcessByChanel((x, y) => GetPixel(x, y).Blue, (x, y, v) =>
            {
                if (!(0 <= x && x < Width)) return;
                if (!(0 <= y && y < Height)) return;
                int j = y * Stride + x * 4;
                Pixels[j + 2] = v;
            },
            v => Approximate(v, Kb), Kb);
            return base.Process();
        }
    }
}