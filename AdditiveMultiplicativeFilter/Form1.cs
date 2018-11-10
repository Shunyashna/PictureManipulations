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

namespace AdditiveMultiplicativeFilter
{
    public partial class Form1 : Form
    {
        Bitmap bitmap { get; set; }
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

        private void button2_Click(object sender, EventArgs e)
        {
            matrix1.Text = "0,1";
            matrix2.Text = "0,1";
            matrix3.Text = "0,1";
            matrix4.Text = "0,1";
            matrix5.Text = "0,2";
            matrix6.Text = "0,1";
            matrix7.Text = "0,1";
            matrix8.Text = "0,1";
            matrix9.Text = "0,1";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            matrix1.Text = "-0,1";
            matrix2.Text = "-0,1";
            matrix3.Text = "-0,1";
            matrix4.Text = "-0,1";
            matrix5.Text = "2";
            matrix6.Text = "-0,1";
            matrix7.Text = "-0,1";
            matrix8.Text = "-0,1";
            matrix9.Text = "-0,1";
        }

        private void button4_Click(object sender, EventArgs e)
        {
            matrix1.Text = "-0,1";
            matrix2.Text = "0,2";
            matrix3.Text = "-0,1";
            matrix4.Text = "0,2";
            matrix5.Text = "3";
            matrix6.Text = "0,2";
            matrix7.Text = "-0,1";
            matrix8.Text = "0,2";
            matrix9.Text = "-0,1";
        }
        private void button1_Click(object sender, EventArgs e)
        {
            matrix1.Text = "-0,1";
            matrix2.Text = "0,1";
            matrix3.Text = "-0,1";
            matrix4.Text = "0,1";
            matrix5.Text = "0,5";
            matrix6.Text = "0,1";
            matrix7.Text = "-0,1";
            matrix8.Text = "0,1";
            matrix9.Text = "-0,1";

        }
        private void AdditiveMultiplicativeFilterButton_Click(object sender, EventArgs e)
        {
            double[] matrix = new double[9];
            if (double.TryParse(matrix1.Text, out matrix[0]) &&
                double.TryParse(matrix2.Text, out matrix[1]) &&
                double.TryParse(matrix3.Text, out matrix[2]) &&
                double.TryParse(matrix4.Text, out matrix[3]) &&
                double.TryParse(matrix5.Text, out matrix[4]) &&
                double.TryParse(matrix6.Text, out matrix[5]) &&
                double.TryParse(matrix7.Text, out matrix[6]) && 
                double.TryParse(matrix8.Text, out matrix[7]) &&
                double.TryParse(matrix9.Text, out matrix[8]))
            {
                Bitmap bmpNew = new Bitmap(bitmap.Width, bitmap.Height);

                AdditiveMultiplicativeFilter(bitmap, bmpNew, matrix);

                OpenPictureInNewWindow(bmpNew);
            }
            else
            {
                MessageBox.Show("Uncorrect value of matrix!");
            }
        }

        private void AdditiveMultiplicativeFilter(Bitmap source, Bitmap destination, double[] matrix)
        {
            var initData = source.LockBits(new Rectangle(0, 0, source.Width, source.Height), ImageLockMode.ReadOnly, source.PixelFormat);
            var destData = destination.LockBits(new Rectangle(0, 0, destination.Width, destination.Height), ImageLockMode.WriteOnly, source.PixelFormat);
            int byteLength = 4;
            if (source.PixelFormat.ToString().Contains("24")) byteLength = 3;


            var pixelCount = 3;
            unsafe
            {
                byte* sourcePtr = (byte*)initData.Scan0.ToPointer();
                byte* destPtr = (byte*)destData.Scan0.ToPointer();

                int pixelOffset = pixelCount / 2;
                int byteOffset = pixelOffset * byteLength;
                for (int i = byteOffset; i < (initData.Stride - byteOffset); i += byteLength)
                {
                    for (int j = pixelOffset; j < source.Height - pixelOffset; j++)
                    {
                        byte[][] window = PixelOperations.GetWindowOfPixels(sourcePtr, initData.Stride, i, j, byteLength, pixelCount);
                        byte[] pixel = GetAdditiveMultiplicatedPixel(Transform2Dto1DArray(window, byteLength * pixelCount * pixelCount), byteLength, matrix);
                        PixelOperations.SetPixelUnsafe(destPtr, pixel, initData.Stride, i, j, byteLength);
                    }
                }
            }

            source.UnlockBits(initData);
            destination.UnlockBits(destData);
        }

        private byte[] GetAdditiveMultiplicatedPixel(byte[] values, int byteLength, double[] matrix)
        {
            var RGBColors = PixelOperations.TransformPixelArrayToColorArray(values, byteLength);

            double red = 0;
            double green = 0;
            double blue = 0;
            double sum = 0;

            for (int i = 0; i < 9; i++) 
            {
                red += RGBColors[i].R * matrix[i];
                green += RGBColors[i].G * matrix[i];
                blue += RGBColors[i].B * matrix[i];
                sum += matrix[i];
            }

            if (sum <= 0) sum = 1;

            red /= sum;
            if (red < 0) red = 0;
            if (red > 255) red = 255;

            green /= sum;
            if (green < 0) green = 0;
            if (green > 255) green = 255;

            blue /= sum;
            if (blue < 0) blue = 0;
            if (blue > 255) blue = 255;
            
            Color color = Color.FromArgb(RGBColors[4].A, (int)red, (int)green, (int)blue);
            byte[] pixel = PixelOperations.TransformColorToPixel(color, byteLength);
            return pixel;
        }
        private byte[] Transform2Dto1DArray(byte[][] window, int elementCount)
        {
            byte[] lineWindow = new byte[elementCount];
            for (int i = 0; i < window.Length; i++)
            {
                for (int j = 0; j < window[i].Length; j++)
                {
                    lineWindow[i * window[i].Length + j] = window[i][j];
                }
            }
            return lineWindow;
        }
        private void OpenPictureInNewWindow(Bitmap newBitmap)
        {
            Form form = new Form();
            form.WindowState = FormWindowState.Maximized;
            form.Name = "Zoomed Picture";
            Panel panel = new Panel();
            panel.Width = 1200;
            panel.Height = 700;
            var p1 = new PictureBox();
            p1.Image = newBitmap;
            p1.SizeMode = PictureBoxSizeMode.AutoSize;
            panel.AutoScroll = true;
            panel.Controls.Add(p1);
            form.Controls.Add(panel);
            form.Show();
        }

        
    }

}
