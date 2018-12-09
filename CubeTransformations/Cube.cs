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
            PointF[] point2D = new PointF[8];
            
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
                //camera1.Position = new Point3D(cubeOrigin.X, cubeOrigin.Y, 1000);
                PerspectiveProection(cubePoints, point2D, drawOrigin, camera1, zoom);
            }
            else
            {
                camera1.Position = new Point3D(cubeOrigin.X, cubeOrigin.Y, 1000);
                ParallelProection(cubePoints, point2D, drawOrigin, zoom);
            }

            var tmpBmp = draw2DCube(point2D, drawOrigin, cubePoints, camera1, type);
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
            for (int i = 0; i < point2D.Length; i++)
            {
                vec = cubePoints[i];
                point2D[i].X = (int)(float)((vec.X) + drawOrigin.X / 1.5);
                point2D[i].Y = (int)(float)((vec.Y) + drawOrigin.Y / 1.5);
            }
        }

        private static Bitmap draw2DCube(PointF[] point2D, Point drawOrigin, Point3D[] cubePoints, Camera camera1, ProectionType type)
        {
            Rectangle bounds = getBounds(point2D);
            bounds.Width += drawOrigin.X;
            bounds.Height += drawOrigin.Y;

            Bitmap tmpBmp = new Bitmap(bounds.Width, bounds.Height);

            float[] distances = new float[8];
            for (int i = 0; i < distances.Length; i++)
            {
                distances[i] = DistanceFromPointToCamera(cubePoints[i], camera1.Position);
            }

            Color[] verticeColor = new Color[8];
            float minDist = distances[0];
            float maxDist = distances[0];
            for (int i = 0; i < distances.Length; i++)
            {
                if (maxDist < distances[i]) maxDist = distances[i];
                if (minDist > distances[i]) minDist = distances[i];
            }

            for (int i = 0; i < verticeColor.Length; i++)
            {
                verticeColor[i] = Methods2D.GetInterpolateColor(Color.Black, Color.Yellow, ((distances[i] - minDist) / (maxDist - minDist)));
            }

            //Back Face
            Methods2D.DrawLine(tmpBmp, point2D[0], point2D[1], verticeColor[0], verticeColor[1]);
            Methods2D.DrawLine(tmpBmp, point2D[1], point2D[2], verticeColor[1], verticeColor[2]);
            Methods2D.DrawLine(tmpBmp, point2D[2], point2D[3], verticeColor[2], verticeColor[3]);
            Methods2D.DrawLine(tmpBmp, point2D[3], point2D[0], verticeColor[3], verticeColor[0]);

            //Front Face
            Methods2D.DrawLine(tmpBmp, point2D[4], point2D[5], verticeColor[4], verticeColor[5]);
            Methods2D.DrawLine(tmpBmp, point2D[5], point2D[6], verticeColor[5], verticeColor[6]);
            Methods2D.DrawLine(tmpBmp, point2D[6], point2D[7], verticeColor[6], verticeColor[7]);
            Methods2D.DrawLine(tmpBmp, point2D[7], point2D[4], verticeColor[7], verticeColor[4]);

            //Right Face
            Methods2D.DrawLine(tmpBmp, point2D[0], point2D[4], verticeColor[0], verticeColor[4]);
            Methods2D.DrawLine(tmpBmp, point2D[5], point2D[1], verticeColor[5], verticeColor[1]);

            //Left Face
            Methods2D.DrawLine(tmpBmp, point2D[3], point2D[7], verticeColor[3], verticeColor[7]);
            Methods2D.DrawLine(tmpBmp, point2D[6], point2D[2], verticeColor[6], verticeColor[2]);

            return tmpBmp;
        }
        public static float DistanceFromPointToCamera(Point3D a, Point3D b)
        {
            return (float)Math.Sqrt(Math.Pow((b.X - a.X), 2) + Math.Pow((b.Y - a.Y), 2) + Math.Pow((b.Z - a.Z), 2));
        }

        public static Point3D[] FillCubeVertices(int width, int height, int depth)
        {
            Point3D[] verts = new Point3D[8];

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

            return verts;
        }

        
    }
}
