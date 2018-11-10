using PictureManipulationsLibrary;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace MonohromeFilter
{
    public partial class Form1 : Form
    {
        Bitmap bitmap { get; set; }
        Color firstColor = Color.White;
        Color secondColor = Color.Black;
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

            pictureBox1.BackColor = firstColor;
            pictureBox2.BackColor = secondColor;
        }

        private void MonohromeFilterButton_Click(object sender, EventArgs e)
        {
            if (int.TryParse(textBox1.Text, out var coefficient) || coefficient < 0 || coefficient > 255)
            {
                
                Bitmap bmpNew = new Bitmap(bitmap.Width, bitmap.Height);

                MonohromeFilter(bitmap, bmpNew, firstColor, secondColor, coefficient);

                OpenPictureInNewWindow(bmpNew);
            }
            else MessageBox.Show("Value mast be in 0 - 255 range");
        }

        private void MonohromeFilter(Bitmap source, Bitmap destination, Color firstColor, Color secondColor, int coefficient)
        {
            var initData = source.LockBits(new Rectangle(0, 0, source.Width, source.Height), ImageLockMode.ReadOnly, source.PixelFormat);
            var destData = destination.LockBits(new Rectangle(0, 0, destination.Width, destination.Height), ImageLockMode.WriteOnly, source.PixelFormat);
            int byteLength = 4;
            if (source.PixelFormat.ToString().Contains("24")) byteLength = 3;

            unsafe
            {
                byte* sourcePtr = (byte*)initData.Scan0.ToPointer();
                byte* destPtr = (byte*)destData.Scan0.ToPointer();
                
                for (int i = 0; i < initData.Stride; i += byteLength)
                {
                    for (int j = 0; j < source.Height; j++)
                    {
                        var pixel = PixelOperations.GetPixelUnsafe(sourcePtr, initData.Stride, i, j, byteLength);
                        pixel = GetMonohromePixel(pixel, byteLength, coefficient);
                        PixelOperations.SetPixelUnsafe(destPtr, pixel, initData.Stride, i, j, byteLength);
                    }
                }
            }

            source.UnlockBits(initData);
            destination.UnlockBits(destData);
        }

        private byte[] GetMonohromePixel(byte[] pixel, int byteLength, int coefficient)
        {

            Color color = PixelOperations.TransformPixelToColor(pixel, byteLength);
            Color newColor;
            if ((color.B + color.G + color.R) / 3 < coefficient) 
            {
                newColor = SetNewColor(color, secondColor);
            }
            else
            {
                newColor = SetNewColor(color, firstColor);
            }
            pixel = PixelOperations.TransformColorToPixel(newColor, byteLength);
            return pixel;
        }

        private Color SetNewColor(Color color, Color drawingColor)
        {
            var b = drawingColor.B;
            var g = drawingColor.G;
            var r = drawingColor.R;
            Color newColor = Color.FromArgb(r, g, b);
            var newColorHSV = PixelOperations.ColorToHSV(newColor);
            newColorHSV.value = color.GetBrightness();
            newColor = PixelOperations.ColorFromHSV(newColorHSV.hue, newColorHSV.saturation, newColorHSV.value);
            return newColor;
        }

        private void secondColorButton_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                secondColor = colorDialog1.Color;
                pictureBox2.BackColor = secondColor;
            }
        }

        private void firstColorButton_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                firstColor = colorDialog1.Color;
                pictureBox1.BackColor = firstColor;
            }
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
