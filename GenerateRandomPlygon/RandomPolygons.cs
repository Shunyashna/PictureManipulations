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
            int angleCount = rnd.Next(3,10);
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
                DrawSegment(previousX, xs[i], previousY, y, Color.Black);
                previousX = xs[i];
                previousY = y;
            }
            for (int i = xs.Count-1; i > 0 ; i--)
            {

                int y = rnd.Next(firstY, pictureBox1.Height - 1);
                angles.Add(new Point(xs[i], y));
                DrawSegment(previousX, xs[i], previousY, y, Color.Black);
                previousX = xs[i];
                previousY = y;
            }
            DrawSegment(firstX, previousX, firstY, previousY, Color.Black);
            angles.Add(new Point(firstX, firstY));

            FillPolygon(angles, firstX, xs[xs.Count - 1]);
            timer1.Stop();
        }

        private void FillPolygon(List<Point> polygon, int minX, int maxX)
        {
            Random rnd = new Random();
            var ys = new List<int>();
            polygon.ForEach(i => ys.Add(i.Y));
            ys = ys.OrderBy(x => x).ToList();
            List<Color> colors = new List<Color>();
            for(int i = 0; i< ys.Count - 1; i++)
            {
                colors.Add(Color.FromArgb(rnd.Next(0, 255), rnd.Next(0, 255), rnd.Next(0, 255)));
            }
            for(int i = minX; i <= maxX; i++)
            {
                for(int j = ys[0]; j <= ys[ys.Count - 1]; j++)
                {
                    if(IsPointInsidePolygon(polygon, i, j) == 1)
                    {
                        Color color = GetInterpolateColor(Color.Yellow, Color.Red, (double)j * 1.5/ ys[ys.Count - 1]);
                        bitmap.SetPixel(i, j, color);
                    }
                }
            }
        }
        int IsPointInsidePolygon(List<Point> p, int x, int y)
        {
            int i1, i2, n, N, S, S1, S2, S3, flag = 0;
            N = p.Count - 1;
            for (n = 0; n < N; n++)
            {
                flag = 0;
                i1 = n < N - 1 ? n + 1 : 0;
                while (flag == 0)
                {
                    i2 = i1 + 1;
                    if (i2 >= N)
                        i2 = 0;
                    if (i2 == (n < N - 1 ? n + 1 : 0))
                        break;
                    S = Math.Abs(p[i1].X * (p[i2].Y - p[n].Y) + p[i2].X * (p[n].Y - p[i1].Y) + p[n].X * (p[i1].Y - p[i2].Y));
                    S1 = Math.Abs(p[i1].X * (p[i2].Y - y) + p[i2].X * (y - p[i1].Y) + x * (p[i1].Y - p[i2].Y));
                    S2 = Math.Abs(p[n].X * (p[i2].Y - y) + p[i2].X * (y - p[n].Y) + x * (p[n].Y - p[i2].Y));
                    S3 = Math.Abs(p[i1].X * (p[n].Y - y) + p[n].X * (y - p[i1].Y) + x * (p[i1].Y - p[n].Y));
                    if (S == S1 + S2 + S3)
                    {
                        flag = 1;
                        break;
                    }
                    i1 = i1 + 1;
                    if (i1 >= N)
                        i1 = 0;
                }
                if (flag == 0)
                    break;
            }
            return flag;
        }

        private Color GetInterpolateColor(Color color1, Color color2, double interpolation)
        {
            var col1 = PixelOperations.ColorToHSV(color1);
            var col2 = PixelOperations.ColorToHSV(color2);
            Color newColor;
            int H = Clip((int)(col1.hue * interpolation + col2.hue * (1 - interpolation)));
            int S = Clip((int)(col1.saturation * interpolation + col2.saturation * (1 - interpolation)));
            int V = Clip((int)(col1.value * interpolation + col2.value * (1 - interpolation)));
            newColor = PixelOperations.ColorFromHSV(H, S, V);
            return newColor;
        }
        private int Clip(int num)
        {
            return num <= 0 ? 0 : (num >= 255 ? 255 : num);
        }

        private void DrawSegment(int x0, int x1, int y0, int y1, Color color)
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
                        PixelOperations.SetPixelUnsafe(ptr, new byte[] { color.B, color.G, color.R, 255 }, data.Stride, x * 4, y, 4);
                        x += sx;
                    }
                }
                else
                {
                    int d = (dx << 1) - dy;
                    int d1 = dx << 1;
                    int d2 = (dx - dy) << 1;
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
