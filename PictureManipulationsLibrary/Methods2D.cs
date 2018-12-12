using CubeTransformations;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PictureManipulationsLibrary
{
    public static class Methods2D
    {
        public static Color GetInterpolateColor(Color color1, Color color2, double interpolation)
        {
            Color newColor;
            //int A = Clip((int)(color1.A * interpolation + color2.A * (1 - interpolation)));
            int R = Clip((int)(color1.R * (1 - interpolation) + color2.R * interpolation));
            int G = Clip((int)(color1.G * (1 - interpolation) + color2.G * interpolation));
            int B = Clip((int)(color1.B * (1 - interpolation) + color2.B * interpolation));
            newColor = Color.FromArgb(R, G, B);
            return newColor;
        }

        private static int Clip(int num)
        {
            return num <= 0 ? 0 : (num >= 255 ? 255 : num);
        }

        public static Color[] GetIntence(Point3D[] cubePoints, Camera camera1)
        {
            float[] distances = new float[8];
            for (int i = 0; i < distances.Length; i++)
            {
                distances[i] = DistanceFromPointToCamera(cubePoints[i], camera1.Position);
            }

            Color[] verticeColor = new Color[8];
            float minDist = distances[0];
            float maxDist = distances[0];
            for (int i = 0; i < distances.Length; i++)
            {
                if (maxDist < distances[i]) maxDist = distances[i];
                if (minDist > distances[i]) minDist = distances[i];
            }

            for (int i = 0; i < verticeColor.Length; i++)
            {
                var interpolation = (distances[i] - minDist) / (maxDist + 0.00001f - minDist);
                verticeColor[i] = Methods2D.GetInterpolateColor(Color.Yellow, Color.Black, interpolation);
            }
            return verticeColor;
        }

        public static float DistanceFromPointToCamera(Point3D a, Point3D b)
        {
            return (float)Math.Sqrt(Math.Pow((b.X - a.X), 2) + Math.Pow((b.Y - a.Y), 2) + Math.Pow((b.Z - a.Z), 2));
        }

        public static void DrawSegment(Bitmap bitmap, int x0, int x1, int y0, int y1, Color color1, Color color2)
        {
            var data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            unsafe
            {
                byte* ptr = (byte*)data.Scan0.ToPointer();


                int dx = (x1 > x0) ? (x1 - x0) : (x0 - x1);
                int dy = (y1 > y0) ? (y1 - y0) : (y0 - y1);
                //Направление приращения
                int sx = (x1 >= x0) ? (1) : (-1);
                int sy = (y1 >= y0) ? (1) : (-1);

                if (dy < dx)
                {
                    int d = (dy << 1) - dx;
                    int d1 = dy << 1;
                    int d2 = (dy - dx) << 1;
                    var color = GetInterpolateColor(color1, color2, 0);
                    PixelOperations.SetPixelUnsafe(ptr, new byte[] { color.B, color.G, color.R, 255 }, data.Stride, x0 * 4, y0, 4);
                    int x = x0 + sx;
                    int y = y0;
                    for (int i = 1; i <= dx; i++)
                    {
                        if (d > 0)
                        {
                            d += d2;
                            y += sy;
                        }
                        else
                        {
                            d += d1;
                        }
                        color = GetInterpolateColor(color1, color2, i / dx);
                        PixelOperations.SetPixelUnsafe(ptr, new byte[] { color.B, color.G, color.R, 255 }, data.Stride, x * 4, y, 4);
                        x += sx;
                    }
                }
                else
                {
                    int d = (dx << 1) - dy;
                    int d1 = dx << 1;
                    int d2 = (dx - dy) << 1;
                    var color = GetInterpolateColor(color1, color2, 0);
                    PixelOperations.SetPixelUnsafe(ptr, new byte[] { color.B, color.G, color.R, 255 }, data.Stride, x0 * 4, y0, 4);
                    int x = x0;
                    int y = y0 + sy;
                    for (int i = 1; i <= dy; i++)
                    {
                        if (d > 0)
                        {
                            d += d2;
                            x += sx;
                        }
                        else
                        {
                            d += d1;
                        }
                        color = GetInterpolateColor(color1, color2, i / dy);
                        PixelOperations.SetPixelUnsafe(ptr, new byte[] { color.B, color.G, color.R, 255 }, data.Stride, x * 4, y, 4);
                        y += sy;
                    }
                }
            }

            bitmap.UnlockBits(data);
        }

        public static PointF GetIntersectionPointOfTwoLines(PointF p1_1, PointF p1_2, PointF p2_1, PointF p2_2, out int state)
        {
            state = -2;
            PointF result = new PointF();
            //Если знаменатель (n) равен нулю, то прямые параллельны.
            //Если и числитель (m или w) и знаменатель (n) равны нулю, то прямые совпадают.
            //Если нужно найти пересечение отрезков, то нужно лишь проверить, лежат ли ua и ub на промежутке [0,1].
            //Если какая-нибудь из этих двух переменных 0 <= ui <= 1, то соответствующий отрезок содержит точку пересечения.
            //Если обе переменные приняли значения из [0,1], то точка пересечения прямых лежит внутри обоих отрезков.
            float m = ((p2_2.X - p2_1.X) * (p1_1.Y - p2_1.Y) - (p2_2.Y - p2_1.Y) * (p1_1.X - p2_1.X));
            float w = ((p1_2.X - p1_1.X) * (p1_1.Y - p2_1.Y) - (p1_2.Y - p1_1.Y) * (p1_1.X - p2_1.X));
            float n = ((p2_2.Y - p2_1.Y) * (p1_2.X - p1_1.X) - (p2_2.X - p2_1.X) * (p1_2.Y - p1_1.Y));

            float Ua = m / n;
            float Ub = w / n;

            if ((n == 0) && (m != 0))
            {
                state = -1; //Прямые параллельны (не имеют пересечения)
            }
            else if ((m == 0) && (n == 0))
            {
                state = 0; //Прямые совпадают
            }
            else
            {
                //Прямые имеют точку пересечения
                result.X = p1_1.X + Ua * (p1_2.X - p1_1.X);
                result.Y = p1_1.Y + Ua * (p1_2.Y - p1_1.Y);

                // Проверка попадания в интервал
                bool a = result.X >= p1_1.X; bool b = result.X <= p1_1.X; bool c = result.X >= p2_1.X; bool d = result.X <= p2_1.X;
                bool e = result.Y >= p1_1.Y; bool f = result.Y <= p1_1.Y; bool g = result.Y >= p2_1.Y; bool h = result.Y <= p2_1.Y;

                if (((a || b) && (c || d)) && ((e || f) && (g || h)))
                {
                    state = 1; //Прямые имеют точку пересечения
                }
            }
            return result;
        }

        public static void DrawLine(Bitmap bitmap, PointF point1, PointF point2, Color color1, Color color2)
        {
            int x0 = (int)point1.X;
            int x1 = (int)point2.X;
            int y0 = (int)point1.Y;
            int y1 = (int)point2.Y;
            var data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            unsafe
            {
                byte* ptr = (byte*)data.Scan0.ToPointer();


                int dx = (x1 > x0) ? (x1 - x0) : (x0 - x1);
                int dy = (y1 > y0) ? (y1 - y0) : (y0 - y1);
                //Направление приращения
                int sx = (x1 >= x0) ? (1) : (-1);
                int sy = (y1 >= y0) ? (1) : (-1);

                if (dy < dx)
                {
                    int d = (dy << 1) - dx;
                    int d1 = dy << 1;
                    int d2 = (dy - dx) << 1;
                    var color = GetInterpolateColor(color1, color2, 0);
                    PixelOperations.SetPixelUnsafe(ptr, new byte[] { color.B, color.G, color.R, 255 }, data.Stride, x0 * 4, y0, 4);
                    int x = x0 + sx;
                    int y = y0;
                    for (int i = 1; i <= dx; i++)
                    {
                        if (d > 0)
                        {
                            d += d2;
                            y += sy;
                        }
                        else
                        {
                            d += d1;
                        }
                        color = GetInterpolateColor(color1, color2, (double)i / dx);
                        PixelOperations.SetPixelUnsafe(ptr, new byte[] { color.B, color.G, color.R, 255 }, data.Stride, x * 4, y, 4);
                        x += sx;
                    }
                }
                else
                {
                    int d = (dx << 1) - dy;
                    int d1 = dx << 1;
                    int d2 = (dx - dy) << 1;
                    var color = GetInterpolateColor(color1, color2, 0);
                    PixelOperations.SetPixelUnsafe(ptr, new byte[] { color.B, color.G, color.R, 255 }, data.Stride, x0 * 4, y0, 4);
                    int x = x0;
                    int y = y0 + sy;
                    for (int i = 1; i <= dy; i++)
                    {
                        if (d > 0)
                        {
                            d += d2;
                            x += sx;
                        }
                        else
                        {
                            d += d1;
                        }
                        color = GetInterpolateColor(color1, color2, (double)i/dy);
                        PixelOperations.SetPixelUnsafe(ptr, new byte[] { color.B, color.G, color.R, 255 }, data.Stride, x * 4, y, 4);
                        y += sy;
                    }
                }
            }

            bitmap.UnlockBits(data);
        }

        public static void FillPolygon(Bitmap bitmap, List<PointF> polygon, List<Color> colors)
        {
            Random rnd = new Random();
            var ys = new List<int>();
            polygon.ForEach(i => ys.Add((int)i.X));
            ys = ys.OrderBy(x => x).ToList();

            Dictionary<PointF, Color> mapping = new Dictionary<PointF, Color>();
            for (int i = 0; i < ys.Count - 1; i++)
            {
                if (!mapping.Keys.Contains(polygon[i]))
                {
                    //var color = Color.FromArgb(rnd.Next(0, 255), rnd.Next(0, 255), rnd.Next(0, 255));
                    mapping.Add(polygon[i], colors[i]);
                }
            }
            for (float j = ys[0] + 0.001f; j <= ys[ys.Count - 1]; j += 0.7f)
            {
                ScanLine(bitmap, mapping, j);
            }
        }
        public static void ScanLine(Bitmap bitmap,Dictionary<PointF, Color> mapping, float x)
        {
            var lineX = new Dictionary<float, Color>();
            for (int i = 0; i < mapping.Count; i++)
            {
                PointF p1 = new PointF();
                var p2 = new PointF();
                int x1 = 0;
                int x2 = 0;
                int next = 0;
                if (i == mapping.Count - 1)
                {
                    next = 0;
                }
                else
                {
                    next = i + 1;
                }
                p1 = mapping.ElementAt(i).Key;
                p2 = mapping.ElementAt(next).Key;
                x1 = (int)mapping.ElementAt(i).Key.Y;
                x2 = (int)mapping.ElementAt(next).Key.Y;
                var low = x1 > x2 ? x2 : x1;
                var high = x1 > x2 ? x1 : x2;
                var intersection = Methods2D.GetIntersectionPointOfTwoLines(p1, p2, new PointF(x, 0),
                                                                            new PointF(x, bitmap.Height - 1),
                                                                            out var status);

                if (intersection != null && status == 1 && intersection.Y >= low && intersection.Y <= high
                    && !lineX.Keys.Contains(intersection.Y))
                {
                    double progress = 0;
                    if (x1 != x2)
                    {
                        progress = (intersection.Y - low) / Math.Abs(x2 - x1);
                    }
                    var color = GetInterpolateColor(mapping.ElementAt(i).Value, mapping.ElementAt(next).Value, progress);
                    lineX.Add(intersection.Y, color);
                }
            }

            var xs = lineX.OrderBy(y => y.Key);
            for (int i = 0; i < xs.Count(); i += 2)
            {
                if (i + 1 != xs.Count())
                {
                    var color1 = xs.ElementAt(i).Value;
                    var color2 = xs.ElementAt(i + 1).Value;
                    DrawSegment(bitmap, (int)Math.Round(x), (int)Math.Round(x), (int)Math.Round(xs.ElementAt(i).Key), (int)Math.Round(xs.ElementAt(i + 1).Key),
                    color1, color2);
                }
            }
        }
        /*public static void ScanLine(Dictionary<PointF, Color> mapping, float y, Bitmap bitmap)
        {
            var lineX = new Dictionary<float, Color>();
            for (int i = 0; i < mapping.Count; i++)
            {
                PointF p1 = new PointF();
                var p2 = new PointF();
                int x1 = 0;
                int x2 = 0;
                int next = 0;
                if (i == mapping.Count - 1)
                {
                    next = 0;
                }
                else
                {
                    next = i + 1;
                }
                p1 = mapping.ElementAt(i).Key;
                p2 = mapping.ElementAt(next).Key;
                x1 = (int)mapping.ElementAt(i).Key.X;
                x2 = (int)mapping.ElementAt(next).Key.X;
                var low = x1 > x2 ? x2 : x1;
                var high = x1 > x2 ? x1 : x2;
                var intersection = Methods2D.GetIntersectionPointOfTwoLines(p1, p2, new PointF(0, y),
                                                                            new PointF(bitmap.Width - 1, y),
                                                                            out var status);

                if (intersection != null && status == 1 && intersection.X >= low && intersection.X <= high
                    && !lineX.Keys.Contains(intersection.X))
                {
                    double progress = 0;
                    if (x1 != x2)
                    {
                        progress = (intersection.X - low) / Math.Abs(x2 - x1);
                    }
                    var color = GetInterpolateColor(mapping.ElementAt(i).Value, mapping.ElementAt(next).Value, progress);
                    lineX.Add(intersection.X, color);
                }
            }

            var xs = lineX.OrderBy(x => x.Key);
            for (int i = 0; i < xs.Count(); i += 2)
            {
                if (i + 1 != xs.Count())
                {
                    var color1 = xs.ElementAt(i).Value;
                    var color2 = xs.ElementAt(i + 1).Value;
                    //var color1 = Color.Red;
                    //var color2 = Color.Black;
                    DrawSegment(bitmap, (int)Math.Round(xs.ElementAt(i).Key), (int)Math.Round(xs.ElementAt(i + 1).Key), (int)Math.Round(y), (int)Math.Round(y),
                    color1, color2);
                }
            }
        }*/

        private static int GetRandomX(int rangeX)
        {
            Random random = new Random();
            return random.Next(2, rangeX - 2);
        }

        //У будет генерировать от 2 до границы бокса-2
        private static int GetRandomY(int rangeY)
        {
            Random random = new Random();
            return random.Next(2, rangeY - 2);
        }

        //рандомная генерация цвета
        private static Color GetRandomColor()
        {
            Random random = new Random();
            Thread.Sleep(1);
            return Color.FromArgb(random.Next(0, 255), random.Next(0, 255), random.Next(0, 255));
        }

        //проверка
        private static int Sign(int x)
        {
            return (x > 0) ? 1 : (x < 0) ? -1 : 0;
        }


        //линии
        public static List<Pixel> DrawLine(Bitmap bitmap, int x1, int y1, Color color1, int x2, int y2, Color color2)
        {
            List<Point> points = new List<Point>();
            int pdx, pdy;
            int element, pelement;

            int dx = x2 - x1;
            int dy = y2 - y1;

            if (Math.Abs(dx) > Math.Abs(dy))
            {
                pdx = Sign(dx);
                pdy = 0;
                element = Math.Abs(dx);
                pelement = Math.Abs(dy);
            }
            else
            {
                pdx = 0;
                pdy = Sign(dy);
                element = Math.Abs(dy);
                pelement = Math.Abs(dx);
            }

            int x = x1;
            int y = y1;
            int e = element / 2;
            points.Add(new Point(x, y));

            for (int i = 0; i < element; i++)
            {
                e -= pelement;
                if (e < 0)
                {
                    e += element;
                    x += Sign(dx);
                    y += Sign(dy);
                }
                else
                {
                    x += pdx;
                    y += pdy;
                }

                points.Add(new Point(x, y));
            }
            List<Pixel> pixels = new List<Pixel>();

            //градиент
            double dr = (double)(color2.R - color1.R) / points.Count;
            double dg = (double)(color2.G - color1.G) / points.Count;
            double db = (double)(color2.B - color1.B) / points.Count;
            Color color = color1;
            for (int i = 0; i < points.Count; i++)
            {
                color = Color.FromArgb(Convert.ToInt32(color1.R + dr * i), Convert.ToInt32(color1.G + dg * i), Convert.ToInt32(color1.B + db * i));
                pixels.Add(new Pixel(points[i].X, points[i].Y, color));
                bitmap.SetPixel(points[i].X, points[i].Y, color);
            }
            return pixels;
        }

        //многоугольник
        public static void Polygon(Bitmap bitmap, int pointNum)
        {
            Random random = new Random();
            //начальная точка
            Pixel pixelFirst = new Pixel(GetRandomX(bitmap.Width - 2), GetRandomY(bitmap.Height - 2), GetRandomColor());
            //конечная точка
            Pixel pixelEnd = new Pixel(random.Next(pixelFirst.X + 1, bitmap.Width-2), pixelFirst.Y, GetRandomColor());
            //точки, которые нужно сгенерировать - начальная и конечная точки
            pointNum -= 2;

            //расстояние от начальной до конечной точки
            int line = pixelEnd.X - pixelFirst.X;
            int range;

            if ((pointNum - 1) == 0)
            {
                range = line;
            }
            else
            {
                range = line / pointNum;
            }

            List<Pixel> pixels = new List<Pixel>();
            List<Pixel> abovePixels = new List<Pixel>();
            List<Pixel> underPixels = new List<Pixel>();

            int curRang = pixelFirst.X;
            for (int i = 0; i < pointNum; i++)
            {
                //генерация x
                int x = random.Next(curRang, curRang + range);
                int y;
                //генерация у
                if (i % 2 == 0)
                {
                    y = random.Next(1, pixelFirst.Y - 1);
                    abovePixels.Add(new Pixel(x, y, GetRandomColor()));
                }
                else
                {
                    y = random.Next(pixelFirst.Y - 1, bitmap.Height-2 - 1);
                    underPixels.Add(new Pixel(x, y, GetRandomColor()));
                }
                curRang += range;
            }

            //все точки 
            pixels.AddRange(abovePixels);
            pixels.Add(pixelEnd);
            underPixels.Sort((x,y) => x.X.CompareTo(y.X)*-1);
            pixels.AddRange(underPixels);
            pixels.Add(pixelFirst);


            List<Pixel> pixelSet = new List<Pixel>();
            List<Pixel> pixelAboveSet = new List<Pixel>();
            List<Pixel> pixelUnderSet = new List<Pixel>();
            Pixel curPoint = pixelFirst;
            for (int i = 0; i < pixels.Count; i++)
            {
                if (i < abovePixels.Count + 1)
                {

                    foreach (Pixel p in DrawLine(bitmap, curPoint.X, curPoint.Y, curPoint.Color, pixels[i].X, pixels[i].Y, pixels[i].Color))
                    {
                        pixelSet.Add(p);
                        //точки выше линии
                        pixelAboveSet.Add(p);
                    }
                }
                else
                {

                    foreach (Pixel p in DrawLine(bitmap, curPoint.X, curPoint.Y, curPoint.Color, pixels[i].X, pixels[i].Y, pixels[i].Color))
                    {
                        pixelSet.Add(p);
                        //точки ниже линии
                        pixelUnderSet.Add(p);
                    }
                }
                curPoint = pixels[i];
            }

            pixelUnderSet.Sort((x, y) => x.X.CompareTo(y.X)*-1);
            for (int i = pixelFirst.X; i < pixelEnd.X; i++)
            {
                Pixel pixel1 = pixelAboveSet.FirstOrDefault(x => x.X == i);
                Pixel pixel2 = pixelUnderSet.FirstOrDefault(x => x.X == i);
                DrawLine(bitmap, pixel1.X, pixel1.Y, pixel1.Color, pixel2.X, pixel2.Y, pixel2.Color);
            }

        }
    }

    public class Pixel
    {
        public int X;
        public int Y;
        public Color Color;

        public Pixel(int x, int y, Color color)
        {
            X = x;
            Y = y;
            Color = color;
        }
    }
}
