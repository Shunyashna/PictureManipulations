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
                verticeColor[i] = Methods2D.GetInterpolateColor(Color.White, Color.Black, interpolation);
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
        public static void FillPolygon(Bitmap bitmap, List<Pixel> polygon)
        {
            List<Pixel> pixels = new List<Pixel>();
            for (int i = 0; i < polygon.Count - 1; i++)
            {
                var p1 = polygon[i];
                var p2 = polygon[i + 1];
                var linePixels = DrawLine(bitmap, p1.X, p1.Y, p1.Color, p2.X, p2.Y, p2.Color);
                pixels.AddRange(linePixels);
            }
            polygon.Sort((i, j) => i.X.CompareTo(j.X));
            int minX = polygon[0].X;
            int maxX = polygon[polygon.Count - 1].X;
            for (int i = minX; i < maxX; i++)
            {
                var ys = pixels.Where(p => p.X == i).ToList();

                for (int j = 0; j < ys.Count - 1; j++)
                {
                    DrawLine(bitmap, ys[j].X, ys[j].Y, ys[j].Color, ys[j + 1].X, ys[j + 1].Y, ys[j + 1].Color);
                }
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
