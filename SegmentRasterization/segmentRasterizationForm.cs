using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SegmentRasterization
{
    public partial class segmentRasterizationForm : Form
    {
        public segmentRasterizationForm()
        {
            InitializeComponent();
        }
        private void segmentRasterizationForm_Load(object sender, EventArgs e)
        {
            timer1.Interval = 40;
            timer1.Start();
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            timer1.Stop();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {

        }

        private void DrawSegment(Point point1, Point point2, Color color1, Color color2)
        {

        }
    }
}
