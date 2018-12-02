using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CubeTransformations
{
    public class Cube
    {
        public int width = 0;
        public int height = 0;
        public int depth = 0;

        public double XRotation { get; set; }
        public double YRotation { get; set; }
        public double ZRotation { get; set; }

        Camera camera1 = new Camera();
        Point3D cubeOrigin;

        public Cube(int side)
        {
            width = side;
            height = side;
            depth = side;
            cubeOrigin = new Point3D(width / 2, height / 2, depth / 2);
            //cubeOrigin = new Point3D(0, 0, 0);

            XRotation = 0.0;
            YRotation = 0.0;
            ZRotation = 0.0;
        }

        //Finds the othermost points. Used so when the cube is drawn on a bitmap,
        //the bitmap will be the correct size
        public static Rectangle getBounds(PointF[] points)
        {
            double left = points[0].X;
            double right = points[0].X;
            double top = points[0].Y;
            double bottom = points[0].Y;
            for (int i = 1; i < points.Length; i++)
            {
                if (points[i].X < left)
                    left = points[i].X;
                if (points[i].X > right)
                    right = points[i].X;
                if (points[i].Y < top)
                    top = points[i].Y;
                if (points[i].Y > bottom)
                    bottom = points[i].Y;
            }

            return new Rectangle(0, 0, (int)Math.Round(right - left), (int)Math.Round(bottom - top));
        }

        public Bitmap drawCube(Point drawOrigin)
        {
            PointF[] point3D = new PointF[24]; //Will be actual 2D drawing points
            Point tmpOrigin = new Point(0, 0);

            // Коэффициент масштабирования задается с помощью ширины монитора, чтобы сохранить искажение куба
            double zoom = (double)Screen.PrimaryScreen.Bounds.Width / 1.5;
            
            Point3D[] cubePoints = FillCubeVertices(width, height, depth);

            //Calculate the camera Z position to stay constant despite rotation            
            Point3D anchorPoint = cubePoints[4]; //anchor point
            double cameraZ = -(((anchorPoint.X - cubeOrigin.X) * zoom) / cubeOrigin.X) + anchorPoint.Z;
            camera1.Position = new Point3D(cubeOrigin.X, cubeOrigin.Y, cameraZ);

            //Apply Rotations, moving the cube to a corner then back to middle
            cubePoints = Math3D.Translate(cubePoints, cubeOrigin, new Point3D(0, 0, 0));
            cubePoints = Math3D.RotateX(cubePoints, XRotation);
            cubePoints = Math3D.RotateY(cubePoints, YRotation);
            cubePoints = Math3D.RotateZ(cubePoints, ZRotation);
            cubePoints = Math3D.Translate(cubePoints, new Point3D(0, 0, 0), cubeOrigin);

            //Convert 3D Points to 2D
            Point3D vec;
            for (int i = 0; i < point3D.Length; i++)
            {
                vec = cubePoints[i];
                if (vec.Z - camera1.Position.Z >= 0)
                {
                    point3D[i].X = (int)(-(vec.X - camera1.Position.X) / (-0.1f) * zoom) + drawOrigin.X;
                    point3D[i].Y = (int)((vec.Y - camera1.Position.Y) / (-0.1f) * zoom) + drawOrigin.Y;
                }
                else
                {
                    tmpOrigin.X = (int)((cubeOrigin.X - camera1.Position.X) / (cubeOrigin.Z - camera1.Position.Z) * zoom) + drawOrigin.X;
                    tmpOrigin.Y = (int)(-(cubeOrigin.Y - camera1.Position.Y) / (cubeOrigin.Z - camera1.Position.Z) * zoom) + drawOrigin.Y;

                    point3D[i].X = (int)(float)((vec.X - camera1.Position.X) / (vec.Z - camera1.Position.Z) * zoom + drawOrigin.X);
                    point3D[i].Y = (int)(float)(-(vec.Y - camera1.Position.Y) / (vec.Z - camera1.Position.Z) * zoom + drawOrigin.Y);
                }
        }

            //Now to plot out the points
            Rectangle bounds = getBounds(point3D);
            bounds.Width += drawOrigin.X;
            bounds.Height += drawOrigin.Y;

            Bitmap tmpBmp = new Bitmap(bounds.Width, bounds.Height);
            Graphics g = Graphics.FromImage(tmpBmp);

            //Back Face
            g.DrawLine(Pens.Black, point3D[0], point3D[1]);
            g.DrawLine(Pens.Black, point3D[1], point3D[2]);
            g.DrawLine(Pens.Black, point3D[2], point3D[3]);
            g.DrawLine(Pens.Black, point3D[3], point3D[0]);

            //Front Face
            g.DrawLine(Pens.Black, point3D[4], point3D[5]);
            g.DrawLine(Pens.Black, point3D[5], point3D[6]);
            g.DrawLine(Pens.Black, point3D[6], point3D[7]);
            g.DrawLine(Pens.Black, point3D[7], point3D[4]);

            //Right Face
            g.DrawLine(Pens.Black, point3D[8], point3D[9]);
            g.DrawLine(Pens.Black, point3D[9], point3D[10]);
            g.DrawLine(Pens.Black, point3D[10], point3D[11]);
            g.DrawLine(Pens.Black, point3D[11], point3D[8]);

            //Left Face
            g.DrawLine(Pens.Black, point3D[12], point3D[13]);
            g.DrawLine(Pens.Black, point3D[13], point3D[14]);
            g.DrawLine(Pens.Black, point3D[14], point3D[15]);
            g.DrawLine(Pens.Black, point3D[15], point3D[12]);

            //Bottom Face
            g.DrawLine(Pens.Black, point3D[16], point3D[17]);
            g.DrawLine(Pens.Black, point3D[17], point3D[18]);
            g.DrawLine(Pens.Black, point3D[18], point3D[19]);
            g.DrawLine(Pens.Black, point3D[19], point3D[16]);

            //Top Face
            g.DrawLine(Pens.Black, point3D[20], point3D[21]);
            g.DrawLine(Pens.Black, point3D[21], point3D[22]);
            g.DrawLine(Pens.Black, point3D[22], point3D[23]);
            g.DrawLine(Pens.Black, point3D[23], point3D[20]);

            g.Dispose(); //Clean-up

            return tmpBmp;
        }

        public static Point3D[] FillCubeVertices(int width, int height, int depth)
        {
            Point3D[] verts = new Point3D[24];

            //front face
            verts[0] = new Point3D(0, 0, 0);
            verts[1] = new Point3D(0, height, 0);
            verts[2] = new Point3D(width, height, 0);
            verts[3] = new Point3D(width, 0, 0);

            //back face
            verts[4] = new Point3D(0, 0, depth);
            verts[5] = new Point3D(0, height, depth);
            verts[6] = new Point3D(width, height, depth);
            verts[7] = new Point3D(width, 0, depth);

            //left face
            verts[8] = new Point3D(0, 0, 0);
            verts[9] = new Point3D(0, 0, depth);
            verts[10] = new Point3D(0, height, depth);
            verts[11] = new Point3D(0, height, 0);

            //right face
            verts[12] = new Point3D(width, 0, 0);
            verts[13] = new Point3D(width, 0, depth);
            verts[14] = new Point3D(width, height, depth);
            verts[15] = new Point3D(width, height, 0);

            //top face
            verts[16] = new Point3D(0, height, 0);
            verts[17] = new Point3D(0, height, depth);
            verts[18] = new Point3D(width, height, depth);
            verts[19] = new Point3D(width, height, 0);

            //bottom face
            verts[20] = new Point3D(0, 0, 0);
            verts[21] = new Point3D(0, 0, depth);
            verts[22] = new Point3D(width, 0, depth);
            verts[23] = new Point3D(width, 0, 0);

            return verts;
        }
    }
}
