using System;
using System.Windows.Media.Imaging;

namespace GK3
{
    public class ColorQuantizerAverageDithering : ColorQuantizerIndeIndependentPalette
    {
        public ColorQuantizerAverageDithering(BitmapImage bitmapImage, int kr, int kg, int kb) : base(bitmapImage, kr, kg, kb)
        {
        }

        protected override void ProcessByChanel(Func<int, int, byte> getPixel, Action<int, int, byte> setPixel, Func<byte, byte> approximate, int k)
        {
            EachPixel((x, y) =>
            {
                setPixel(x, y, approximate(getPixel(x, y)));
            });
        }
    }
}