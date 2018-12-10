using CubeTransformations;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
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
                verticeColor[i] = Methods2D.GetInterpolateColor(Color.Yellow, Color.Black, ((distances[i] - minDist) / (maxDist - minDist)));
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
            polygon.ForEach(i => ys.Add((int)i.Y));
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
                ScanLine(mapping, j, bitmap);
            }

            /*for (int i = 0; i < polygon.Count - 1; i++)
            {
                DrawLine(bitmap, polygon[i], polygon[i + 1], Color.Red, Color.Red);
            }*/
        }
        public static void ScanLine(Dictionary<PointF, Color> mapping, float y, Bitmap bitmap)
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
        }
    }
}
