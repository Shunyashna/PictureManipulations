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
using System.Threading;
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

            List <Pixel> angles = new List<Pixel>();
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
            var color = GetRandomColor();
            angles.Add(new Pixel(firstX, firstY, color));
            
            for (int i = 1; i < xs.Count; i++)
            {
                int y = rnd.Next(0, firstY);
                angles.Add(new Pixel(xs[i], y, GetRandomColor()));
                previousX = xs[i];
                previousY = y;
            }
            for (int i = xs.Count-1; i > 0 ; i--)
            {
                int y = rnd.Next(firstY, pictureBox1.Height - 1);
                angles.Add(new Pixel(xs[i], y, GetRandomColor()));
                previousX = xs[i];
                previousY = y;
            }
            angles.Add(new Pixel(firstX, firstY, color));

            /*List<Pixel> angles = new List<Pixel>();
            angles.Add(new Pixel(20, 20, Color.Red));
            angles.Add(new Pixel(300, 100, Color.Purple));
            angles.Add(new Pixel(400, 400, Color.Blue));
            angles.Add(new Pixel(200, 450, Color.Green));
            angles.Add(new Pixel(20, 20, Color.Red));*/

            Random random = new Random();
            Methods2D.FillPolygon(bitmap, angles);
            pictureBox1.Image = bitmap;

            timer1.Stop();
        }

        public Color GetRandomColor()
        {
            Random rnd = new Random();
            int r = rnd.Next(0, 255);
            Thread.Sleep(1);
            int g = rnd.Next(0, 255);
            Thread.Sleep(1);
            int b = rnd.Next(0, 255);
            return Color.FromArgb(r, g, b);
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
