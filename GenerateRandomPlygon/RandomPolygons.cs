using MathNet.Symbolics;
using PictureManipulationsLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Expr = MathNet.Symbolics.Expression;


namespace GenerateRandomPlygon
{
    public partial class RandomPolygons : Form
    {
        Bitmap bitmap { get; set; }
        public RandomPolygons()
        {
            InitializeComponent();
        }

        private void RandomPolygons_Load(object sender, EventArgs e)
        {
            timer1.Interval = 1000;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Random rnd = new Random();
            bitmap = new Bitmap(pictureBox1.Width, pictureBox1.Height);

            /*List<Point> angles = new List<Point>();
            int angleCount = rnd.Next(2, 6);
            List<int> xs = new List<int>();
            for(int i = 0; i< angleCount; i++)
            {
                xs.Add(rnd.Next(0, pictureBox1.Width - 1)/2);
            }

            xs = xs.OrderBy(x => x).ToList();

            int firstX = xs[0];
            int firstY = rnd.Next(0, pictureBox1.Height - 1);
            int previousX = firstX;
            int previousY = firstY;
            angles.Add(new Point(firstX, firstY));
            
            for (int i = 1; i < xs.Count; i++)
            {
                int y = rnd.Next(0, firstY);
                angles.Add(new Point(xs[i], y));
                previousX = xs[i];
                previousY = y;
            }
            for (int i = xs.Count-1; i > 0 ; i--)
            {
                int y = rnd.Next(firstY, pictureBox1.Height - 1);
                angles.Add(new Point(xs[i], y));
                previousX = xs[i];
                previousY = y;
            }
            angles.Add(new Point(firstX, firstY));*/

            List<Pixel> angles = new List<Pixel>();
            angles.Add(new Pixel(20, 20, Color.Red));
            angles.Add(new Pixel(300, 100, Color.Purple));
            angles.Add(new Pixel(400, 400, Color.Blue));
            angles.Add(new Pixel(200, 450, Color.Green));
            angles.Add(new Pixel(20, 20, Color.Red));
            //FillPolygon(angles);
            Random random = new Random();
            //Methods2D.Polygon(bitmap, random.Next(3, 10));
            FillPolygon(bitmap, angles);
            pictureBox1.Image = bitmap;

            timer1.Stop();
        }

        public void FillPolygon(Bitmap bitmap, List<Pixel> polygon)
        {
            List<Pixel> pixels = new List<Pixel>();
            for(int i = 0; i < polygon.Count - 1; i++)
            {
                var p1 = polygon[i];
                var p2 = polygon[i + 1];
                var linePixels = Methods2D.DrawLine(bitmap, p1.X, p1.Y, p1.Color, p2.X, p2.Y, p2.Color);
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
                    Methods2D.DrawLine(bitmap, ys[j].X, ys[j].Y, ys[j].Color, ys[j + 1].X, ys[j + 1].Y, ys[j + 1].Color);
                }
            }
        }
        

        //private void FillPolygon(List<Point> polygon)
        //{
        //    Random rnd = new Random();
        //    var ys = new List<int>();
        //    polygon.ForEach(i => ys.Add(i.X));
        //    ys = ys.OrderBy(x => x).ToList();

        //    Dictionary<Point, Color> mapping = new Dictionary<Point, Color>();
        //    for(int i = 0; i< ys.Count - 1; i++)
        //    {
        //        var color = Color.FromArgb(rnd.Next(0, 255), rnd.Next(0, 255), rnd.Next(0, 255));
        //        mapping.Add(polygon[i], color);
        //    }
        //    for(float j = ys[0]/* + 0.001f*/; j <= ys[ys.Count - 1]; j += /*0.97f*/1)
        //    {
        //        ScanLine(mapping, j);
        //    }

        //    for(int i = 0; i < polygon.Count - 1; i++)
        //    {
        //        DrawSegment(polygon[i].X, polygon[i + 1].X, polygon[i].Y, polygon[i + 1].Y, Color.Red, Color.Red);
        //    }
        //}
        //public void ScanLine(Dictionary<Point, Color> mapping, float y)
        //{
        //    var lineX = new Dictionary<float, Color>();
        //    for(int i = 0; i < mapping.Count; i++)
        //    {
        //        int next = 0;
        //        if (i == mapping.Count - 1) next = 0;
        //        else next = i + 1;

        //        var p1 = mapping.ElementAt(i).Key;
        //        var p2 = mapping.ElementAt(next).Key;
        //        var x1 = mapping.ElementAt(i).Key.Y;
        //        var x2 = mapping.ElementAt(next).Key.Y;
        //        var low = x1 > x2 ? x2 : x1;
        //        var high = x1 > x2 ? x1 : x2;
        //        var intersection = Methods2D.GetIntersectionPointOfTwoLines(p1, p2, new PointF(y, 0),
        //                                                                    new PointF(y, bitmap.Height-1),
        //                                                                    out var status);

        //        if (intersection != null && status == 1 && intersection.Y > low && intersection.Y <= high 
        //            && !lineX.Keys.Contains(intersection.Y))
        //        {
        //            double progress = 0;
        //            if (x1 != x2)
        //            {
        //                progress = (intersection.Y - low) / Math.Abs(x2 - x1);
        //            }
        //            var color = GetInterpolateColor(mapping.ElementAt(i).Value, mapping.ElementAt(next).Value, progress);
        //            lineX.Add(intersection.Y, color);
        //        }
        //    }

        //    var xs = lineX.OrderBy(x => x.Key);
        //    for (int i = 0; i < xs.Count(); i += 2)
        //    {
        //        if (i + 1 != xs.Count())
        //        {
        //            var color1 = xs.ElementAt(i).Value;
        //            var color2 = xs.ElementAt(i + 1).Value;
        //            DrawSegment((int)Math.Round(y), (int)Math.Round(y),
        //                (int)Math.Round(xs.ElementAt(i).Key),
        //                (int)Math.Round(xs.ElementAt(i + 1).Key),

        //            color1, color2);
        //        }
        //    }
        //}

        private Color GetInterpolateColor(Color color1, Color color2, double interpolation)
        {
            double dr = (color2.R - color1.R) * interpolation;
            double dg = (color2.G - color1.G) * interpolation;
            double db = (color2.B - color1.B) * interpolation;
            int R = Clip((int)(color1.R + dr));
            int G = Clip((int)(color1.G + dg));
            int B = Clip((int)(color1.B + db));
            var color = Color.FromArgb(R, G, B);
            return color;
        }
        private int Clip(int num)
        {
            return num <= 0 ? 0 : (num >= 255 ? 255 : num);
        }

        private void DrawSegment(int x0, int x1, int y0, int y1, Color color1, Color color2)
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
                    var color = GetInterpolateColor(color1, color2, 1);
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
                        color = GetInterpolateColor(color1, color2, i * 1.0 / dx);
                        PixelOperations.SetPixelUnsafe(ptr, new byte[] { color.B, color.G, color.R, 255 }, data.Stride, x * 4, y, 4);
                        x += sx;
                    }
                }
                else
                {
                    int d = (dx << 1) - dy;
                    int d1 = dx << 1;
                    int d2 = (dx - dy) << 1;
                    var color = GetInterpolateColor(color1, color2, 1);
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
                        color = GetInterpolateColor(color1, color2, i * 1.0 / dy);
                        PixelOperations.SetPixelUnsafe(ptr, new byte[] { color.B, color.G, color.R, 255 }, data.Stride, x * 4, y, 4);
                        y += sy;
                    }
                }
            }

            bitmap.UnlockBits(data);
            pictureBox1.Image = bitmap;
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            timer1.Start();
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            timer1.Stop();
        }
    }

}
