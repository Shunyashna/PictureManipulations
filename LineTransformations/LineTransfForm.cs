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

namespace LineTransformations
{
    public partial class LineTransfForm : Form
    {
        
        Bitmap bitmap;
        double angleSpeed1 = 0;
        double angleSpeed2 = 0;
        double currentAngle1 = 0;
        double currentAngle2 = 0;
        int oldCenterX = 0;
        int oldCenterY = 0;
        

        public LineTransfForm()
        {
            InitializeComponent();
            bitmap = new Bitmap(pictureBox1.Width, pictureBox1.Height);
        }

        private void LineTransfForm_Load(object sender, EventArgs e)
        {
            timer1.Interval = 40;
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            if (double.TryParse(textBox1.Text, out var angleSpeed1) && double.TryParse(textBox2.Text, out var angleSpeed2))
            {
                this.angleSpeed1 = angleSpeed1;
                this.angleSpeed2 = angleSpeed2;
                timer1.Start();
            }
            else
            {
                MessageBox.Show("Uncorrect value of speed.");
            }
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            timer1.Stop();
        }
        
        private void timer1_Tick(object sender, EventArgs e)
        {
            int centerX = bitmap.Width / 2;
            int centerY = bitmap.Height / 2;

            var coords = LineTransformation(151, centerX, centerY, centerX, centerY, ref currentAngle1, angleSpeed1);
            LineTransformation(41, oldCenterX, oldCenterY, coords.x, coords.y, ref currentAngle2, -angleSpeed2);

            oldCenterX = coords.x;
            oldCenterY = coords.y;

            DrawAxis(centerX, centerY);
            pictureBox1.Image = bitmap;
        }

        private void DrawAxis(int centerX, int centerY)
        {
            for(int i = 0; i < bitmap.Width; i++)
            {
                bitmap.SetPixel(i, centerY, Color.Black);
            }
            for (int i = 0; i < bitmap.Height; i++)
            {
                bitmap.SetPixel(centerX, i, Color.Black);
            }
        }

        private (int x, int y) LineTransformation(int radius, int oldX0, int oldY0, int newX0, int newY0, ref double currentAngle, double angleSpeed)
        {
            int oldX = CalculateNewX(oldX0 + radius, oldY0, Math.Cos(currentAngle), Math.Sin(currentAngle), oldX0, oldY0);
            int oldY = CalculateNewY(oldX0 + radius, oldY0, Math.Cos(currentAngle), Math.Sin(currentAngle), oldX0, oldY0);
            DrawSegment(oldX0, oldX, oldY0, oldY, BackColor);

            currentAngle = CalculateNewAngle(currentAngle, angleSpeed, timer1.Interval);

            int newX = CalculateNewX(newX0 + radius, newY0, Math.Cos(currentAngle), Math.Sin(currentAngle), newX0, newY0);
            int newY = CalculateNewY(newX0 + radius, newY0, Math.Cos(currentAngle), Math.Sin(currentAngle), newX0, newY0);
            DrawSegment(newX0, newX, newY0, newY, Color.Black);

            return (newX, newY);
        }

        private int CalculateNewX(double x, int y, double cos, double sin, int x0, int y0)
        {
            return (int)Math.Round((x - x0) * cos + (y - y0) * sin + x0);
        }
        private int CalculateNewY(double x, int y, double cos, double sin, int x0, int y0)
        {
            return (int)Math.Round(-(x - x0) * sin + (y - y0) * cos + y0);
        }

        private double CalculateNewAngle(double previousAngle, double angleSpeed, int time)
        {
            return previousAngle + angleSpeed / time;
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

        
    }
}
