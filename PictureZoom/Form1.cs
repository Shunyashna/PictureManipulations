using PictureManipulationsLibrary;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;


namespace PictureZoom
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
        private void button1_Click(object sender, EventArgs e)
        {
            if(double.TryParse(textBox1.Text, out var xZoom) && double.TryParse(textBox2.Text, out var yZoom) && xZoom > 0 && yZoom > 0)
            {
                double newWidth =  Math.Ceiling(bitmap.Width * xZoom);
                double newHeight =  Math.Ceiling(bitmap.Height * yZoom);
                Bitmap bmpNew = new Bitmap((int)newWidth, (int)newHeight);

                ZoomPicture(bitmap, bmpNew, xZoom, yZoom);

                OpenPictureInNewWindow(bmpNew);
            }
            else
            {
                MessageBox.Show("Uncorrect values of zoom!");
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

        private void button2_Click(object sender, EventArgs e)
        {
            var picture = panel1.Controls[0] as PictureBox;
            if (picture.Image != null)
            {
                picture.Image.Dispose();
                picture.Image = null;
            }
            Form1_Load(sender, e);
        }

        private void ZoomPicture(Bitmap source, Bitmap destination, double xZoom, double yZoom)
        {
            var initData = source.LockBits(new Rectangle(0, 0, source.Width, source.Height), ImageLockMode.ReadOnly, source.PixelFormat);
            var destData = destination.LockBits(new Rectangle(0, 0, destination.Width, destination.Height), ImageLockMode.WriteOnly, source.PixelFormat);
            int byteLength = 4;
            if (source.PixelFormat.ToString().Contains("24")) byteLength = 3;
            else if (source.PixelFormat.ToString().Contains("16")) byteLength = 2;
            // Get the address of the first line
            IntPtr ptr1 = initData.Scan0;
            IntPtr ptr2 = destData.Scan0;

            unsafe
            {
                byte* sourcePtr = (byte*)ptr1.ToPointer();
                byte* destPtr = (byte*)ptr2.ToPointer();

                for (int i = 0; i < destData.Stride; i += 4)
                {
                    for (int j = 0; j < destination.Height; j++)
                    {
                        int newX = (int)(i / xZoom);
                        int newY = (int)(j / yZoom);
                        if (newX % byteLength != 0) newX -= newX % byteLength; //offset to the first byte of four

                        var pixel = PixelOperations.GetPixelUnsafe(sourcePtr, initData.Stride, newX, newY, byteLength);
                        PixelOperations.SetPixelUnsafe(destPtr, pixel, destData.Stride, i, j, byteLength);
                    }
                }
            }

            source.UnlockBits(initData);
            destination.UnlockBits(destData);
        }
    }
}
