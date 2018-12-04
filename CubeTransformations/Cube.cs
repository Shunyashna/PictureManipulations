using PictureManipulationsLibrary;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CubeTransformations
{
    public class Cube
    {
        public int Width { get; }
        public int Height { get; }
        public int Length { get; }

        public double XRotation { get; set; }
        public double YRotation { get; set; }
        public double ZRotation { get; set; }

        
        Point3D cubeOrigin;

        public Cube(int side)
        {
            Width = side;
            Height = side;
            Length = side;
            cubeOrigin = new Point3D(Width / 2, Height / 2, Length / 2);

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

        public Bitmap drawCube(Point drawOrigin, ProectionType type)
        {
            PointF[] point2D = new PointF[24];
            
            double zoom = Screen.PrimaryScreen.Bounds.Width / 1.5;
            
            Point3D[] cubePoints = FillCubeVertices(Width, Height, Length);
            Camera camera1 = new Camera();
         
            Point3D anchorPoint = cubePoints[4]; //anchor point
            double cameraZ = -(((anchorPoint.X - cubeOrigin.X) * zoom) / cubeOrigin.X) + anchorPoint.Z;
            camera1.Position = new Point3D(cubeOrigin.X, cubeOrigin.Y, cameraZ);
            

            cubePoints = Transformation.Translate(cubePoints, cubeOrigin, new Point3D(0, 0, 0));
            cubePoints = Transformation.RotateX(cubePoints, XRotation);
            cubePoints = Transformation.RotateY(cubePoints, YRotation);
            cubePoints = Transformation.RotateZ(cubePoints, ZRotation);
            cubePoints = Transformation.Translate(cubePoints, new Point3D(0, 0, 0), cubeOrigin);

            if (type == ProectionType.Perspective)
            {
                PerspectiveProection(cubePoints, point2D, drawOrigin, camera1, zoom);
            }
            else
            {
                ParallelProection(cubePoints, point2D, drawOrigin, zoom);
            }

            var tmpBmp = draw2DCube(point2D, drawOrigin);
            return tmpBmp;
        }

        private void PerspectiveProection(Point3D[] cubePoints, PointF[] point2D, Point drawOrigin, Camera camera1, double zoom)
        {
            Point3D vec;
            for (int i = 0; i < point2D.Length; i++)
            {
                vec = cubePoints[i];
                if (vec.Z - camera1.Position.Z >= 0)
                {
                    point2D[i].X = (int)(-(vec.X - camera1.Position.X) / (-0.1f) * zoom) + drawOrigin.X;
                    point2D[i].Y = (int)((vec.Y - camera1.Position.Y) / (-0.1f) * zoom) + drawOrigin.Y;
                }
                else
                {
                    point2D[i].X = (int)(float)((vec.X - camera1.Position.X) / (vec.Z - camera1.Position.Z) * zoom + drawOrigin.X);
                    point2D[i].Y = (int)(float)(-(vec.Y - camera1.Position.Y) / (vec.Z - camera1.Position.Z) * zoom + drawOrigin.Y);
                }
            }
        }

        private void ParallelProection(Point3D[] cubePoints, PointF[] point2D, Point drawOrigin, double zoom)
        {
            Point3D vec;
            for(int i = 0; i < point2D.Length; i++)
            {
                vec = cubePoints[i];
                if (vec.Z - 10>= 0)
                {
                    point2D[i].X = (int)(float)((vec.X) + drawOrigin.X / 1.5);
                    point2D[i].Y = (int)(float)((vec.Y) + drawOrigin.Y / 1.5);
                }
                else
                {
                    point2D[i].X = (int)(float)((vec.X) + drawOrigin.X / 1.5);
                    point2D[i].Y = (int)(float)((vec.Y) + drawOrigin.Y / 1.5);
                }
            }
        }

        private static Bitmap draw2DCube(PointF[] point2D, Point drawOrigin)
        {
            Rectangle bounds = getBounds(point2D);
            bounds.Width += drawOrigin.X;
            bounds.Height += drawOrigin.Y;

            Bitmap tmpBmp = new Bitmap(bounds.Width, bounds.Height);
            
            //Back Face
            DrawLine(tmpBmp, point2D[0], point2D[1], Color.Black);
            DrawLine(tmpBmp, point2D[1], point2D[2], Color.Black);
            DrawLine(tmpBmp, point2D[2], point2D[3], Color.Black);
            DrawLine(tmpBmp, point2D[3], point2D[0], Color.Black);

            //Front Face
            DrawLine(tmpBmp, point2D[4], point2D[5], Color.Black);
            DrawLine(tmpBmp, point2D[5], point2D[6], Color.Black);
            DrawLine(tmpBmp, point2D[6], point2D[7], Color.Black);
            DrawLine(tmpBmp, point2D[7], point2D[4], Color.Black);

            //Right Face
            DrawLine(tmpBmp, point2D[8], point2D[9], Color.Black);
            DrawLine(tmpBmp, point2D[9], point2D[10], Color.Black);
            DrawLine(tmpBmp, point2D[10], point2D[11], Color.Black);
            DrawLine(tmpBmp, point2D[11], point2D[8], Color.Black);

            //Left Face
            DrawLine(tmpBmp, point2D[12], point2D[13], Color.Black);
            DrawLine(tmpBmp, point2D[13], point2D[14], Color.Black);
            DrawLine(tmpBmp, point2D[14], point2D[15], Color.Black);
            DrawLine(tmpBmp, point2D[15], point2D[12], Color.Black);

            //Bottom Face
            DrawLine(tmpBmp, point2D[16], point2D[17], Color.Black);
            DrawLine(tmpBmp, point2D[17], point2D[18], Color.Black);
            DrawLine(tmpBmp, point2D[18], point2D[19], Color.Black);
            DrawLine(tmpBmp, point2D[19], point2D[16], Color.Black);

            //Top Face
            DrawLine(tmpBmp, point2D[20], point2D[21], Color.Black);
            DrawLine(tmpBmp, point2D[21], point2D[22], Color.Black);
            DrawLine(tmpBmp, point2D[22], point2D[23], Color.Black);
            DrawLine(tmpBmp, point2D[23], point2D[20], Color.Black);
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

        private static void DrawLine(Bitmap bitmap, PointF point1, PointF point2, Color color)
        {
            int x0 = (int)point1.X;
            int x1 = (int)point2.X;
            int y0 = (int)point1.Y;
            int y1 = (int)point2.Y;
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
                    PixelOperations.SetPixelUnsafe(ptr, new byte[] { 0, 0, 0, 255 }, data.Stride, x0 * 4, y0, 4);
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
                        PixelOperations.SetPixelUnsafe(ptr, new byte[] { 0, 0, 0, 255 }, data.Stride, x * 4, y, 4);
                        x += sx;
                    }
                }
                else
                {
                    int d = (dx << 1) - dy;
                    int d1 = dx << 1;
                    int d2 = (dx - dy) << 1;
                    PixelOperations.SetPixelUnsafe(ptr, new byte[] { 0, 0, 0, 255 }, data.Stride, x0 * 4, y0, 4);
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
                        PixelOperations.SetPixelUnsafe(ptr, new byte[] { 0, 0, 0, 255 }, data.Stride, x * 4, y, 4);
                        y += sy;
                    }
                }
            }

            bitmap.UnlockBits(data);
        }
    }
}
