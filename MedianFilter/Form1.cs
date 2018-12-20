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

namespace MedianFilter
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

        private void MedianFilterButton_Click(object sender, EventArgs e)
        {
            if (int.TryParse(textBox1.Text, out var pixelCount) && pixelCount % 2 != 0)
            {
                Bitmap bmpNew = new Bitmap(bitmap.Width, bitmap.Height);

                MedianFilter(bitmap, bmpNew, pixelCount);

                OpenPictureInNewWindow(bmpNew);
            }
            else
            {
                MessageBox.Show("Uncorrect value of window!");
            }
        }

        private void MedianFilter(Bitmap source, Bitmap destination, int pixelCount)
        {
            var initData = source.LockBits(new Rectangle(0, 0, source.Width, source.Height), ImageLockMode.ReadOnly, source.PixelFormat);
            var destData = destination.LockBits(new Rectangle(0, 0, destination.Width, destination.Height), ImageLockMode.WriteOnly, source.PixelFormat);
            int byteLength = 4;
            if (source.PixelFormat.ToString().Contains("24")) byteLength = 3;

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
                        byte[] pixel = GetMedian(Transform2Dto1DArray(window, byteLength *  pixelCount * pixelCount), byteLength, pixelCount);
                        PixelOperations.SetPixelUnsafe(destPtr, pixel, initData.Stride, i, j, byteLength);
                    }
                }
            }

            source.UnlockBits(initData);
            destination.UnlockBits(destData);
        }

        private byte[] GetMedian(byte[] values, int byteLength, int pixelCount)
        {
            List<Color> RGBColors = PixelOperations.TransformPixelArrayToColorArray(values, byteLength).ToList();
            RGBColors.Sort((x, y) => x.GetBrightness().CompareTo(y.GetBrightness()));
            Color color = RGBColors[pixelCount * pixelCount / 2];
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
