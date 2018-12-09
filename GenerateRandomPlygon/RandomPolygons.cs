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

            List<Point> angles = new List<Point>();
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
            angles.Add(new Point(firstX, firstY));

            FillPolygon(angles, firstX, xs[xs.Count - 1]);
        }

        private void FillPolygon(List<Point> polygon, int minX, int maxX)
        {
            Random rnd = new Random();
            var ys = new List<int>();
            polygon.ForEach(i => ys.Add(i.Y));
            ys = ys.OrderBy(x => x).ToList();
            
            Dictionary<Point, Color> mapping = new Dictionary<Point, Color>();
            for(int i = 0; i< ys.Count - 1; i++)
            {
                var color = Color.FromArgb(rnd.Next(0, 255), rnd.Next(0, 255), rnd.Next(0, 255));
                mapping.Add(polygon[i], color);
            }
            for(float j = ys[0] + 0.001f; j <= ys[ys.Count - 1]; j += 0.7f)
            {
                ScanLine(mapping, j);
            }

            for(int i = 0; i < polygon.Count - 1; i++)
            {
                DrawSegment(polygon[i].X, polygon[i + 1].X, polygon[i].Y, polygon[i + 1].Y, Color.Red, Color.Red);
            }
        }
        public void ScanLine(Dictionary<Point, Color> mapping, float y)
        {
            var lineX = new Dictionary<float, Color>();
            for(int i = 0; i < mapping.Count; i++)
            {
                Point p1 = new Point();
                var p2 = new Point();
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
                x1 = mapping.ElementAt(i).Key.X;
                x2 = mapping.ElementAt(next).Key.X;
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
                    //var color1 = xs.ElementAt(i).Value;
                    //var color2 = xs.ElementAt(i + 1).Value;
                    var color1 = Color.Red;
                    var color2 = Color.Green;
                    DrawSegment((int)Math.Round(xs.ElementAt(i).Key), (int)Math.Round(xs.ElementAt(i + 1).Key), (int)Math.Round(y), (int)Math.Round(y),
                    color1, color2);
                }
            }
        }

        private Color GetInterpolateColor(Color color1, Color color2, double interpolation)
        {
            Color newColor;
            //int A = Clip((int)(color1.A * interpolation + color2.A * (1 - interpolation)));
            int R = Clip((int)(color1.R * (1-interpolation) + color2.R * interpolation));
            int G = Clip((int)(color1.G * (1-interpolation) + color2.G * interpolation));
            int B = Clip((int)(color1.B * (1-interpolation) + color2.B * interpolation));
            newColor = Color.FromArgb( R, G, B);
            return newColor;
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
