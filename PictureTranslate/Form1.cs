using PictureManipulationsLibrary;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace PictureTranslate
{
    public partial class Form1 : Form
    {
        Bitmap bitmap;
        double currentAngle = 0;
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            OpenFileDialog myFile = new OpenFileDialog();
            myFile.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png, *.bmp) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png; *.bmp";
            if (myFile.ShowDialog() == DialogResult.OK)
            {
                Bitmap bmp = new Bitmap(Image.FromFile(myFile.FileName));
                bitmap = (Bitmap)bmp.Clone();
                panel1.AutoScroll = true;
                PictureBox pictureBox1 = new PictureBox();

                pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;
                this.Controls.Add(pictureBox1);
                panel1.Controls.Add(pictureBox1);
                pictureBox1.Image = bmp;
            }
        }
        private void AbsoluteRotateButton_Click(object sender, EventArgs e)
        {
            if (double.TryParse(textBox1.Text, out var angle))
            {
                currentAngle = angle;
                Bitmap newBitmap = RotateImage(bitmap, angle);
                var picBox = panel1.Controls[0] as PictureBox;
                picBox.Image = newBitmap;

            }
            else
            {
                MessageBox.Show("Uncorrect value of angle!");
            }
        }
        private void RelativeRotateButton_Click(object sender, EventArgs e)
        {
            if (double.TryParse(textBox2.Text, out var angle))
            {
                double newAngle = currentAngle + angle;
                currentAngle = newAngle;

                Bitmap newBitmap = RotateImage(bitmap, newAngle);

                var picBox = panel1.Controls[0] as PictureBox;
                picBox.Image = newBitmap;
            }
            else
            {
                MessageBox.Show("Uncorrect value of angle!");
            }
        }

        private Bitmap RotateImage(Bitmap source, double angle)
        {
            double radAngle = angle * Math.PI / 180;
            var cos = Math.Cos(radAngle);
            var sin = Math.Sin(radAngle);

            var widthAndHeight = CalculateNewBoundaries(bitmap.Width, bitmap.Height, radAngle);
            int newWidth = widthAndHeight.width;
            int newHeight = widthAndHeight.height;

            Bitmap destination = new Bitmap((int)newWidth, (int)newHeight);

            var SrcData = source.LockBits(new Rectangle(0, 0, source.Width, source.Height), ImageLockMode.ReadOnly, source.PixelFormat);
            var DestData = destination.LockBits(new Rectangle(0, 0, destination.Width, destination.Height), ImageLockMode.WriteOnly, source.PixelFormat);

            int bytelength = 4;
            if (source.PixelFormat.ToString().Contains("24")) bytelength = 3;

            int x0 = (int)(newWidth / 2.0);
            int y0 = (int)(newHeight / 2.0);
            unsafe
            {

                byte* SrcPtr = (byte*)SrcData.Scan0.ToPointer();
                byte* DestPtr = (byte*)DestData.Scan0.ToPointer();

                var offset = GetOffsets(source.Width, source.Height, cos, sin, x0, y0, angle);

                for (double x = 0; x < source.Width; x += 1)
                {
                    for (int y = 0; y < source.Height; y++)
                    {
                        var X = CalculateNewX((int)x, y, cos, sin, x0, y0);
                        var Y = CalculateNewY((int)x, y, cos, sin, x0, y0);
                        X += offset.xOffset;
                        Y += offset.yOffset;

                        if (Y >= 0 && Y < newHeight && X >= 0 && X < newWidth)
                        {
                            var pixel = PixelOperations.GetPixelUnsafe(SrcPtr, SrcData.Stride, (int)Math.Round(x * 4), y, bytelength);
                            PixelOperations.SetPixelUnsafe(DestPtr, pixel, DestData.Stride, X * 4, Y, bytelength);
                            PixelOperations.SetPixelUnsafe(DestPtr, pixel, DestData.Stride, (X + 1) * 4, Y, bytelength);
                        }
                    }
                }

            }
            source.UnlockBits(SrcData);
            destination.UnlockBits(DestData);
            return destination;
        }
        private (int xOffset, int yOffset) GetOffsets(int width, int height, double cos, double sin, int x0, int y0, double angle)
        {
            var xOffset = 0;
            var yOffset = 0;

            int valueX;
            int valueY;
            if (angle >= 0 && angle <= 90)
            {
                valueX = CalculateNewX(0, 0, cos, sin, x0, y0);
                valueY = CalculateNewY(width - 1, 0, cos, sin, x0, y0);

            }
            else if (angle > 90 && angle <= 180)
            {
                valueX = CalculateNewX(width - 1, 0, cos, sin, x0, y0);
                valueY = CalculateNewY(width - 1, height - 1, cos, sin, x0, y0);
            }
            else if (angle > 180 && angle < 270)
            {
                valueX = CalculateNewX(width - 1, height - 1, cos, sin, x0, y0);
                valueY = CalculateNewY(0, height - 1, cos, sin, x0, y0);
            }
            else
            {
                valueX = CalculateNewX(0, height - 1, cos, sin, x0, y0);
                valueY = CalculateNewY(0, 0, cos, sin, x0, y0);
            }
            xOffset = -1 * valueX;
            yOffset = -1 * valueY;
            return (xOffset, yOffset);
        }
        private int CalculateNewX(double x, int y, double cos, double sin, int x0, int y0)
        {
            return (int)Math.Round((x - x0) * cos + (y - y0) * sin + x0);
        }
        private int CalculateNewY(double x, int y, double cos, double sin, int x0, int y0)
        {
            return (int)Math.Round(-(x - x0) * sin + (y - y0) * cos + y0);
        }
        private (int width, int height) CalculateNewBoundaries(int oldWidth, int oldHeight, double radAngle)
        {
            var atan1 = Math.Atan2(oldHeight, oldWidth);
            var atan2 = Math.Atan2(oldWidth, oldHeight);
            double diagonal = Math.Sqrt(Math.Abs(Math.Pow(oldWidth, 2) + Math.Pow(oldHeight, 2)));
            double newWidth = diagonal;
            double newHeight = diagonal;
            if (radAngle >= 0 && radAngle <= Math.PI / 2 || radAngle > Math.PI && radAngle <= Math.PI * 3 / 2)
            {
                newWidth = diagonal * Math.Sin(atan2 + radAngle);
                newHeight = diagonal * Math.Sin(atan1 + radAngle);
            }
            else
            {
                newWidth = diagonal * Math.Sin(atan2 + Math.PI - radAngle);
                newHeight = diagonal * Math.Sin(atan1 + Math.PI - radAngle);
            }
            if (newWidth < 0) newWidth *= -1;
            if (newHeight < 0) newHeight *= -1;
            return ((int)Math.Round(newWidth), (int)Math.Round(newHeight));
            //return ((int)diagonal, (int)diagonal);
        }

        
    }
}
