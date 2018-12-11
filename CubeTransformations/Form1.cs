using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CubeTransformations
{
    public partial class Form1 : Form
    {
        Cube cube;

        double rotationX = 0;
        double rotationY = 0;
        double rotationZ = 0;

        double speedX = 0;
        double speedY = 0;
        double speedZ = 0;

        ProectionType type = ProectionType.Perspective;

        //Cube is positioned based on center
        Point Origin { get; set; }
        public Form1()
        {
            InitializeComponent();
        }
        private void Render(double rotateX, double rotateY, double rotateZ)
        {
            //Set the rotation values
            cube.XRotation = rotateX;
            cube.YRotation = rotateY;
            cube.ZRotation = rotateZ;
            
            pictureBox1.Image = cube.drawCube(Origin, type);
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            double.TryParse(textBox1.Text, out speedX);
            double.TryParse(textBox2.Text, out speedY);
            double.TryParse(textBox3.Text, out speedZ);
            timer1.Start();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Origin = new Point(pictureBox1.Width / 2, pictureBox1.Height / 2);
            cube = new Cube(200);
            timer1.Interval = 20;
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            timer1.Stop();
        }

        private void resetButton_Click(object sender, EventArgs e)
        {
            rotationX = 0;
            rotationY = 0;
            rotationZ = 0;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            rotationX = CalculateNewAngle(rotationX, speedX/100);
            rotationY = CalculateNewAngle(rotationY, speedY/100);
            rotationZ = CalculateNewAngle(rotationZ, speedZ/100);

            Render(rotationX, rotationY, rotationZ);
        }

        private double CalculateNewAngle(double previousRotation, double speed)
        {
            double a = speed * timer1.Interval;
            return previousRotation += a;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            resetButton_Click(sender, e);
            type = ProectionType.Parallel;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            resetButton_Click(sender, e);
            type = ProectionType.Perspective;
        }
    }
}
