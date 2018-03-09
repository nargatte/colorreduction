using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Media.Imaging;

namespace GK3
{
    public class ColorQuantizerPopularityAlgorithm : ColorQuantizer
    {
        private readonly int _k;
        private readonly byte cube;

        public ColorQuantizerPopularityAlgorithm(BitmapImage bitmapImage, int numberOfColors, int cubeSize) : base(bitmapImage)
        {
            _k = numberOfColors;
            cube = (byte)cubeSize;
        }

        public override WriteableBitmap Process()
        {
            Dictionary<PixelColor, int> colorsCounter = new Dictionary<PixelColor, int>();
            EachPixel((x, y) =>
            {
                PixelColor pixelColor = GetPixel(x, y);
                pixelColor.Red /= cube;
                pixelColor.Green /= cube;
                pixelColor.Blue /= cube;
                colorsCounter.TryGetValue(pixelColor, out int c);
                if(c == 0) colorsCounter.Add(pixelColor, 1);
                else colorsCounter[pixelColor] = c + 1;
            });
            List<PixelColor> colors = new List<PixelColor>(colorsCounter.OrderByDescending(x => x.Value).Take(_k).Select(x => x.Key));
            colors.ForEach(c =>
            {
                c.Red *= cube;
                c.Green *= cube;
                c.Blue *= cube;

                c.Red += (byte)(cube / 2);
                c.Green += (byte)(cube / 2);
                c.Blue += (byte)(cube / 2);
            });
            Dictionary<PixelColor, PixelColor> colorCache = new Dictionary<PixelColor, PixelColor>();
            EachPixel((x, y) =>
            {
                PixelColor pixelColor = GetPixel(x, y);
                if (colorCache.TryGetValue(pixelColor, out PixelColor c))
                    SetPixel(x, y, c);
                else
                {
                    PixelColor newColor = colors.OrderBy(color =>
                    {
                        byte r = (byte) Math.Abs(pixelColor.Red - color.Red);
                        byte g = (byte) Math.Abs(pixelColor.Green - color.Green);
                        byte b = (byte) Math.Abs(pixelColor.Blue - color.Blue);
                        return r*r + g*g + b*b;
                    }).FirstOrDefault();
                    colorCache.Add(pixelColor, newColor);
                    SetPixel(x, y, newColor);
                }
            });
            return base.Process();
        }
    }
}