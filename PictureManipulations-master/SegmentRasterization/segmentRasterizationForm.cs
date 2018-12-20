using PictureManipulationsLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SegmentRasterization
{
    public partial class segmentRasterizationForm : Form
    {

        Bitmap bitmap;
        public segmentRasterizationForm()
        {
            InitializeComponent();
        }
        private void segmentRasterizationForm_Load(object sender, EventArgs e)
        {
            bitmap = new Bitmap(pictureBox1.Width-1, pictureBox1.Height-1);
            timer1.Interval = 40;
            //timer1.Interval = 1000;
            timer1.Start();
            //timer1_Tick(sender, e);

        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            timer1.Stop();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Random rnd = new Random();
            Color color1 = Color.FromArgb(255, rnd.Next(0, 255), rnd.Next(0, 255), rnd.Next(0, 255));
            Color color2 = Color.FromArgb(255, rnd.Next(0, 255), rnd.Next(0, 255), rnd.Next(0, 255));
            DrawSegment(rnd.Next(0, pictureBox1.Width-5), rnd.Next(0, pictureBox1.Width-5), rnd.Next(0, pictureBox1.Height-5), rnd.Next(0, pictureBox1.Height-5), color1, color2);

            //DrawSegment(471, 259, 535, 38, color1, color2);
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
                    Color color = GetInterpolateColor(color1, color2, 0);
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
                        color = GetInterpolateColor(color1, color2, (i * 1.0 / dx));
                        PixelOperations.SetPixelUnsafe(ptr, new byte[] { color.B, color.G, color.R, 255 }, data.Stride, x * 4, y, 4);
                        x += sx;
                    }
                }
                else
                {
                    int d = (dx << 1) - dy;
                    int d1 = dx << 1;
                    int d2 = (dx - dy) << 1;
                    Color color = GetInterpolateColor(color1, color2, 0);
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
                        color = GetInterpolateColor(color1, color2, (i * 1.0 / dy ));
                        PixelOperations.SetPixelUnsafe(ptr, new byte[] { color.B, color.G, color.R, 255 }, data.Stride, x * 4, y, 4);
                        y += sy;
                    }
                }
            }

            bitmap.UnlockBits(data);
            pictureBox1.Image = bitmap;
        }

        private Color GetInterpolateColor(Color color1, Color color2, double interpolation)
        {
            Color newColor;
            int A = Clip((int)(color1.A * interpolation + color2.A * (1 - interpolation)));
            int R = Clip((int)(color1.R * interpolation + color2.R * (1 - interpolation)));
            int G = Clip((int)(color1.G * interpolation + color2.G * (1 - interpolation)));
            int B = Clip((int)(color1.B * interpolation + color2.B * (1 - interpolation)));
            newColor = Color.FromArgb(A, R, G, B);
            return newColor;
        }
        private int Clip(int num)
        {
            return num <= 0 ? 0 : (num >= 255 ? 255 : num);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            timer1.Start();
        }
    }
}
