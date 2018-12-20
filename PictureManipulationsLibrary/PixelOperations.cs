using System;
using System.Drawing;

namespace PictureManipulationsLibrary
{
    public static class PixelOperations
    {
        public static byte[] GetPixel(byte[] source, int stride, int x, int y, int bytelength)
        {
            byte[] pixel = new byte[bytelength];
            for(int i = 0; i < bytelength; i++)
            {
                pixel[i] = source[y * stride + x + i];
            }
            return pixel;
        }
        public static void SetPixel(byte[] destination, byte[] pixel, int stride, int x, int y, int bytelength)
        {
            for (int i = 0; i < bytelength; i++)
            {
                destination[y * stride + x + i] = pixel[i];
            }
        }
        unsafe public static byte[] GetPixelUnsafe(byte* source, int stride, int x, int y, int bytelength)
        {
            byte[] pixel = new byte[bytelength];
            for (int i = 0; i < bytelength; i++)
            {
                var pixelByte = source + (y * stride + x + i);
                pixel[i] = *pixelByte;
            }
            return pixel;
        }
        unsafe public static void SetPixelUnsafe(byte* destination, byte[] pixel, int stride, int x, int y, int bytelength)
        {
            for (int i = 0; i < bytelength; i++)
            {
                *(destination + (y * stride + x + i)) = pixel[i];
            }
        }

        unsafe private static byte[] GetLineOfPixels(byte* currentPixel, int byteLength, int pixelCount)
        {
            byte* lineBeginning = currentPixel - (pixelCount / 2 * byteLength);
            byte[] line = new byte[byteLength * pixelCount];

            for (int i = 0; i < pixelCount * byteLength; i+=byteLength)
            {
                for(int c = 0; c < byteLength; c++)
                {
                    line[i + c] = *(lineBeginning + i + c);
                }
            }
            return line;
        }

        unsafe public static byte[][] GetWindowOfPixels(byte* source, int stride, int x, int y, int byteLength, int pixelCount)
        {
            byte* currentPixel = source + (y * stride + x);
            byte[][] window = new byte[pixelCount][];
            byte* upperLinePixel = currentPixel - (stride * (pixelCount / 2));
            for(int i = 0; i < pixelCount; i++)
            {
                window[i] = GetLineOfPixels(upperLinePixel + (i * stride), byteLength, pixelCount);
            }
            return window;
        }


        public static (double hue, double saturation, double value) ColorToHSV(Color color)
        {
            int max = Math.Max(color.R, Math.Max(color.G, color.B));
            int min = Math.Min(color.R, Math.Min(color.G, color.B));

            var hue = color.GetHue();
            var saturation = (max == 0) ? 0 : 1d - (1d * min / max);
            var value = max / 255d;
            return ( hue, saturation, value);
        }

        public static Color ColorFromHSV(double hue, double saturation, double value)
        {
            int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            double f = hue / 60 - Math.Floor(hue / 60);

            value = value * 255;
            int v = Clip(Convert.ToInt32(value));
            int p = Clip(Convert.ToInt32(value * (1 - saturation)));
            int q = Clip(Convert.ToInt32(value * (1 - f * saturation)));
            int t = Clip(Convert.ToInt32(value * (1 - (1 - f) * saturation)));

            if (hi == 0)
                return Color.FromArgb(255, v, t, p);
            else if (hi == 1)
                return Color.FromArgb(255, q, v, p);
            else if (hi == 2) 
                return Color.FromArgb(255, p, v, t);
            else if (hi == 3) 
                return Color.FromArgb(255, p, q, v);
            else if (hi == 4)
                return Color.FromArgb(255, t, p, v);
            else    
                return Color.FromArgb(255, v, p, q);
        }
        private static int Clip(int num)
        {
            return num <= 0 ? 0 : (num >= 255 ? 255 : num);
        }

        public static Color[] TransformPixelArrayToColorArray(byte[] pixels, int byteLength)
        {
            Color[] colors = new Color[pixels.Length / byteLength];
            for(int i = 0; i < pixels.Length; i += byteLength)
            {
                Color color;
                if (byteLength == 4) color = Color.FromArgb(pixels[i + 3], pixels[i + 2], pixels[i + 1], pixels[i]);
                else color = Color.FromArgb(pixels[i + 2], pixels[i + 1], pixels[i]);
                colors[i / byteLength] = color;
            }
            return colors;
        }

        public static byte[] TransformColorToPixel(Color color, int byteLength)
        {
            byte[] pixel = new byte[byteLength];
            if(byteLength == 4)
            {
                pixel[0] = color.B;
                pixel[1] = color.G;
                pixel[2] = color.R;
                pixel[3] = color.A;
            }
            else
            {
                pixel[0] = color.B;
                pixel[1] = color.G;
                pixel[2] = color.R;
            }
            return pixel;
        }

        public static Color TransformPixelToColor(byte[] pixel, int byteLength)
        {
            Color color;
            if (byteLength == 4)
            {
                var B = pixel[0];
                var G = pixel[1];
                var R = pixel[2];
                var A = pixel[3];
                color = Color.FromArgb(A, R, G, B);
            }
            else
            {
                var B = pixel[0];
                var G = pixel[1];
                var R = pixel[2];
                color = Color.FromArgb(R, G, B);
            }
            return color;
        }
    }
}
