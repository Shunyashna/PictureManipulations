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
        public int Depth { get; }

        public double XRotation { get; set; }
        public double YRotation { get; set; }
        public double ZRotation { get; set; }

        
        Point3D cubeOrigin;

        public Cube(int side)
        {
            Width = side;
            Height = side;
            Depth = side;
            cubeOrigin = new Point3D(Width / 2, Height / 2, Depth / 2);

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
            
            Point3D[] cubePoints = FillCubeVertices(Width, Height, Depth);
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
                ParallelProection(cubePoints, point2D, drawOrigin);
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

        private void ParallelProection(Point3D[] cubePoints, PointF[] point2D, Point drawOrigin)
        {
            Point3D vec;
            for (int i = 0; i < point2D.Length; i++)
            {
                vec = cubePoints[i];
                point2D[i].X = (int)(float)((vec.X) + drawOrigin.X / 1.5);
                point2D[i].Y = (int)(float)((vec.Y) + drawOrigin.Y / 1.5);
            }
        }
        private void ParallelPointProection(Point3D point, ref Point point2D, Point drawOrigin)
        {
                point2D.X = (int)(float)((point.X) + drawOrigin.X / 1.5);
                point2D.Y = (int)(float)((point.Y) + drawOrigin.Y / 1.5);
        }

        private Bitmap draw2DCube(PointF[] point2D, Point drawOrigin, Point3D[] cubePoints, Camera camera1, ProectionType type)
        {
            Rectangle bounds = getBounds(point2D);
            bounds.Width += drawOrigin.X;
            bounds.Height += drawOrigin.Y;
            Camera light = new Camera();
            light.Position = new Point3D(Width / 2, Height / 2 + 175, Depth / 2);
            
            Bitmap tmpBmp = new Bitmap(bounds.Width, bounds.Height);

            Point light2D = new Point();
            ParallelPointProection(light.Position, ref light2D, drawOrigin);

            tmpBmp.SetPixel(light2D.X, light2D.Y, Color.OrangeRed);
            tmpBmp.SetPixel(light2D.X + 1, light2D.Y, Color.OrangeRed);
            tmpBmp.SetPixel(light2D.X - 1, light2D.Y, Color.OrangeRed);
            tmpBmp.SetPixel(light2D.X + 1, light2D.Y + 1, Color.OrangeRed);
            tmpBmp.SetPixel(light2D.X + 1, light2D.Y - 1, Color.OrangeRed);
            tmpBmp.SetPixel(light2D.X - 1, light2D.Y - 1, Color.OrangeRed);
            tmpBmp.SetPixel(light2D.X - 1, light2D.Y + 1, Color.OrangeRed);
            tmpBmp.SetPixel(light2D.X, light2D.Y + 1, Color.OrangeRed);
            tmpBmp.SetPixel(light2D.X, light2D.Y - 1, Color.OrangeRed);

            Color[] verticeColor = /*Methods2D.GetIntence(cubePoints, light);*/ GetGuroColors(cubePoints, camera1.Position, light.Position, Color.Black);

            DrawGuro(tmpBmp, point2D, verticeColor, cubePoints);

            /*//Back Face
            Methods2D.DrawLine(tmpBmp, point2D[0], point2D[1], verticeColor[0], verticeColor[1]);
            Methods2D.DrawLine(tmpBmp, point2D[1], point2D[5], verticeColor[1], verticeColor[5]);
            Methods2D.DrawLine(tmpBmp, point2D[5], point2D[4], verticeColor[5], verticeColor[4]);
            Methods2D.DrawLine(tmpBmp, point2D[4], point2D[0], verticeColor[4], verticeColor[0]);

            //Front Face
            Methods2D.DrawLine(tmpBmp, point2D[3], point2D[2], verticeColor[3], verticeColor[2]);
            Methods2D.DrawLine(tmpBmp, point2D[2], point2D[6], verticeColor[2], verticeColor[6]);
            Methods2D.DrawLine(tmpBmp, point2D[6], point2D[7], verticeColor[6], verticeColor[7]);
            Methods2D.DrawLine(tmpBmp, point2D[7], point2D[3], verticeColor[7], verticeColor[3]);

            //Right Face
            Methods2D.DrawLine(tmpBmp, point2D[7], point2D[4], verticeColor[7], verticeColor[4]);
            Methods2D.DrawLine(tmpBmp, point2D[3], point2D[0], verticeColor[3], verticeColor[0]);

            //Left Face
            Methods2D.DrawLine(tmpBmp, point2D[6], point2D[5], verticeColor[6], verticeColor[5]);
            Methods2D.DrawLine(tmpBmp, point2D[2], point2D[1], verticeColor[2], verticeColor[1]);*/

            

            return tmpBmp;
        }

        public Point3D[] FillCubeVertices(int width, int height, int depth)
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

        /*public Bitmap DrawPhong(Bitmap tmpBmp, PointF[] point2D, Color[] colors, Point3D[] cubePoints)
        {
            var normals = LightingLibrary.CalculateNormals(cubePoints);

            const float4 diffColor = vec4(0.5, 0.0, 0.0, 1.0); //рассеянное освещение
            const vec4 specColor = vec4(0.7, 0.7, 0.0, 1.0);
            const float specPower = 30.0;

            return tmpBmp;
        }*/

        public static Color[] GetGuroColors(Point3D[] cubePoints, Point3D camera, Point3D light, Color baseColor)
        {
            Color[] colors = new Color[8];
            var intences = LightingLibrary.GetIntense(cubePoints, camera, light);
            for (int i = 0; i < colors.Length; i++)
            {
                colors[i] = Color.FromArgb(Clip((int)(baseColor.R * intences[i])), Clip((int)(baseColor.G * intences[i])), Clip((int)(baseColor.B * intences[i])));
            }
            return colors;
        }
        private static int Clip(int num)
        {
            return num <= 0 ? 0 : (num >= 255 ? 255 : num);
        }

        public Bitmap DrawGuro(Bitmap tmpBmp, PointF[] point2D, Color[] colors, Point3D[] cubePoints)
        {

            var normals = LightingLibrary.CalculateNormals(cubePoints);

                DrawWatchablePolygons(tmpBmp, normals.Front,
                new List<PointF> { point2D[3], point2D[7], point2D[6], point2D[2], point2D[3] },
                new List<Color>() { colors[3], colors[7], colors[6], colors[2], colors[3] });

                DrawWatchablePolygons(tmpBmp, normals.Right,
                new List<PointF> { point2D[0], point2D[4], point2D[7], point2D[3], point2D[0] },
                new List<Color>() { colors[0], colors[4], colors[7], colors[3], colors[0] });

                DrawWatchablePolygons(tmpBmp, normals.Back,
                new List<PointF> { point2D[1], point2D[5], point2D[4], point2D[0], point2D[1] },
                new List<Color>() { colors[1], colors[5], colors[4], colors[0], colors[1] });
            
                DrawWatchablePolygons(tmpBmp, normals.Left,
                new List<PointF> { point2D[2], point2D[6], point2D[5], point2D[1], point2D[2] },
                new List<Color>() { colors[2], colors[6], colors[5], colors[1], colors[2] });
            
                DrawWatchablePolygons(tmpBmp, normals.Top,
                new List<PointF> { point2D[7], point2D[4], point2D[5], point2D[6], point2D[7] },
                new List<Color>() { colors[7], colors[4], colors[5], colors[6], colors[7] });

                DrawWatchablePolygons(tmpBmp, normals.Bottom,
                new List<PointF> { point2D[0], point2D[3], point2D[2], point2D[1], point2D[0] },
                new List<Color>() { colors[0], colors[3], colors[2], colors[1], colors[0] });

            return tmpBmp;
        }

        public void DrawWatchablePolygons(Bitmap tmpBmp, Point3D normal, List<PointF> points, List<Color> colors)
        {
            var cos = normal.Z / Math.Sqrt(Math.Pow(normal.X,2) + Math.Pow(normal.Y, 2) + Math.Pow(normal.Z, 2));
            if (cos >= 0 && cos <= 1)
            {
                List<Pixel> pixels = new List<Pixel>();
                for(int i = 0; i < points.Count; i++)
                {
                    pixels.Add(new Pixel((int)points[i].X, (int)points[i].Y, colors[i]));
                }
                Methods2D.FillPolygon(tmpBmp, pixels);
            }
        }
        
    }
}
