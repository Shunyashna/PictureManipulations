using CubeTransformations;
using PictureManipulationsLibrary;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubeTransformations
{
    public class LightingLibrary
    {
        public static Point3D GetNormalVector(Point3D p1, Point3D p2, Point3D p3)
        {
            Point3D p = new Point3D(p1.X - p2.X, p1.Y - p2.Y, p1.Z - p2.Z);
            Point3D q = new Point3D(p3.X - p2.X, p3.Y - p2.Y, p3.Z - p2.Z);

            Point3D normal = new Point3D(p.Y * q.Z - q.Y * p.Z,
                                         p.Z * q.X - q.Z * p.X,
                                         p.X * q.Y - q.X * p.Y);
            var normalLength = Math.Sqrt(Math.Pow(normal.X, 2) + Math.Pow(normal.Y, 2) + Math.Pow(normal.Z, 2));
            normal = new Point3D(normal.X / normalLength, normal.Y / normalLength, normal.Z / normalLength);
            return normal;
        }

        public static PolygonNormals CalculateNormals(Point3D[] cubePoints)
        {
            PolygonNormals normals = new PolygonNormals();
            normals.Front = GetNormalVector(cubePoints[7], cubePoints[4], cubePoints[5]);
            normals.Back = GetNormalVector(cubePoints[2], cubePoints[1], cubePoints[0]);
            normals.Top = GetNormalVector(cubePoints[6], cubePoints[5], cubePoints[1]);
            normals.Bottom = GetNormalVector(cubePoints[3], cubePoints[0], cubePoints[4]);
            normals.Left = GetNormalVector(cubePoints[3], cubePoints[7], cubePoints[6]);
            normals.Right = GetNormalVector(cubePoints[4], cubePoints[0], cubePoints[1]);

            return normals;
        }

        public static double CosQ(Point3D light, Point3D normal, Point3D vertice)
        {
            light = new Point3D(light.X - vertice.X, light.Y - vertice.Y, light.Z - vertice.Z);
            var cosQ = (normal.X * light.X + normal.Y * light.Y + normal.Z * light.Z) /
                (Math.Sqrt(Math.Pow(normal.X,2) + Math.Pow(normal.Y, 2) + Math.Pow(normal.X, 2)) *
                Math.Sqrt(Math.Pow(light.X, 2) + Math.Pow(light.Y, 2) + Math.Pow(light.X, 2)));
            return cosQ;
        }

        public static double CosA(Point3D light, Point3D camera, Point3D normal, Point3D vertice)
        {
            light = new Point3D(light.X - vertice.X, light.Y - vertice.Y, light.Z - vertice.Z);
            camera = new Point3D(camera.X - vertice.X, camera.Y - vertice.Y, camera.Z - vertice.Z);
            var xR = (2 * normal.X * (normal.X * light.X + normal.Y * light.Y + normal.Z * light.Z) / Math.Sqrt(Math.Pow(normal.X, 2) + Math.Pow(normal.Y, 2) + Math.Pow(normal.X, 2))) - light.X;
            var yR = (2 * normal.Y * (normal.X * light.X + normal.Y * light.Y + normal.Z * light.Z) / Math.Sqrt(Math.Pow(normal.X, 2) + Math.Pow(normal.Y, 2) + Math.Pow(normal.X, 2))) - light.Y;
            var zR = (2 * normal.Z * (normal.X * light.X + normal.Y * light.Y + normal.Z * light.Z) / Math.Sqrt(Math.Pow(normal.X, 2) + Math.Pow(normal.Y, 2) + Math.Pow(normal.X, 2))) - light.Z;


            var cosA = (xR * camera.X + yR * camera.Y + xR * camera.Z) /
                (Math.Sqrt(Math.Pow(xR, 2) + Math.Pow(yR, 2) + Math.Pow(zR, 2)) *
                Math.Sqrt(Math.Pow(camera.X, 2) + Math.Pow(camera.Y, 2) + Math.Pow(camera.Z, 2)));
            return cosA;
        }

        public static List<double> GetIntense(Point3D[] cubePoints, Point3D camera, Point3D light)
        {
            var normals = CalculateNormals(cubePoints);
            Point3D[] verticeNormals = new Point3D[8];

            verticeNormals[0] = new Point3D((normals.Front.X + normals.Left.X + normals.Bottom.X) / 3,
                                            (normals.Front.Y + normals.Left.Y + normals.Bottom.Y) / 3,
                                            (normals.Front.Z + normals.Left.Z + normals.Bottom.Z) / 3);

            verticeNormals[1] = new Point3D((normals.Front.X + normals.Left.X + normals.Top.X) / 3,
                                            (normals.Front.Y + normals.Left.Y + normals.Top.Y) / 3,
                                            (normals.Front.Z + normals.Left.Z + normals.Top.Z) / 3);

            verticeNormals[2] = new Point3D((normals.Front.X + normals.Right.X + normals.Top.X) / 3,
                                            (normals.Front.Y + normals.Right.Y + normals.Top.Y) / 3,
                                            (normals.Front.Z + normals.Right.Z + normals.Top.Z) / 3);

            verticeNormals[3] = new Point3D((normals.Front.X + normals.Right.X + normals.Bottom.X) / 3,
                                            (normals.Front.Y + normals.Right.Y + normals.Bottom.Y) / 3,
                                            (normals.Front.Z + normals.Right.Z + normals.Bottom.Z) / 3);

            verticeNormals[4] = new Point3D((normals.Back.X + normals.Left.X + normals.Bottom.X) / 3,
                                            (normals.Back.Y + normals.Left.Y + normals.Bottom.Y) / 3,
                                            (normals.Back.Z + normals.Left.Z + normals.Bottom.Z) / 3);

            verticeNormals[5] = new Point3D((normals.Back.X + normals.Left.X + normals.Top.X) / 3,
                                            (normals.Back.Y + normals.Left.Y + normals.Top.Y) / 3,
                                            (normals.Back.Z + normals.Left.Z + normals.Top.Z) / 3);

            verticeNormals[6] = new Point3D((normals.Back.X + normals.Right.X + normals.Top.X) / 3,
                                            (normals.Back.Y + normals.Right.Y + normals.Top.Y) / 3,
                                            (normals.Back.Z + normals.Right.Z + normals.Top.Z) / 3);

            verticeNormals[7] = new Point3D((normals.Back.X + normals.Right.X + normals.Bottom.X) / 3,
                                            (normals.Back.Y + normals.Right.Y + normals.Bottom.Y) / 3,
                                            (normals.Back.Z + normals.Right.Z + normals.Bottom.Z) / 3);

            List<double> verticeIntences = new List<double>();
            for(int i = 0; i < verticeNormals.Length; i++)
            {
                verticeIntences.Add(GetVerticeIntence(camera, light, verticeNormals[i], cubePoints[i]));
            }
            return verticeIntences;
        }

        public static double GetVerticeIntence(Point3D camera, Point3D light, Point3D normal, Point3D vertice)
        {
            double Kd = 0.5, Ks = 0.7, Ka = 0.4;
            int p = 3;
            double Ia = 0.7, I = 0.7;
            return Ia * Ka + I / (Methods2D.DistanceFromPointToCamera(vertice, camera)) * (Ks * Math.Pow(CosA(light, camera, normal, vertice), 30) + Kd * CosQ(light, normal, vertice));
        }



        private static double I = 200;
        private static double I_A = 1;
        private static double K_D = 0.3;
        private static double K_S = 0.7;
        private static double K_A = 2;
        private static double P = 3;

        public static Color[] getIntence(Point3D[] cubePoints, Point3D[] normals,
                                         Point3D camera1, Point3D light)
        {
            Color[] verticesColor = new Color[8];
            verticesColor[0] = getIntenceColor(cubePoints[0],
                    new Point3D[] { normals[1], normals[3], normals[5] },
                    camera1, light);

            verticesColor[1] = getIntenceColor(cubePoints[1],
                   new Point3D[] { normals[1], normals[2], normals[5] },
                    camera1, light);

            verticesColor[2] = getIntenceColor(cubePoints[2],
                    new Point3D[] { normals[1], normals[2], normals[4] },
                    camera1, light);

            verticesColor[3] = getIntenceColor(cubePoints[3],
                    new Point3D[] { normals[1], normals[3], normals[4] },
                    camera1, light);

            verticesColor[4] = getIntenceColor(cubePoints[4],
                    new Point3D[] { normals[0], normals[3], normals[5] },
                    camera1, light);

            verticesColor[5] = getIntenceColor(cubePoints[5],
                    new Point3D[] { normals[0], normals[2], normals[5] },
                    camera1, light);

            verticesColor[6] = getIntenceColor(cubePoints[6],
                    new Point3D[] { normals[0], normals[2], normals[4] },
                    camera1, light);

            verticesColor[7] = getIntenceColor(cubePoints[7],
                    new Point3D[] { normals[0], normals[3], normals[4] },
                    camera1, light);

            return verticesColor;
        }

        private static Color getIntenceColor(Point3D cubePoint, Point3D[] normals,
                                             Point3D camera1, Point3D light)
        {
            Point3D verticeNormal = calculateVerticeNormal(normals[0], normals[1], normals[2]);
            double distance = distanceFromPointToCamera(cubePoint, camera1);
            double interpolation = getIReflection(light, verticeNormal, camera1, distance);
            return getInterpolateColor(Color.White, Color.DarkGray, interpolation);
        }

        private static Point3D calculateVerticeNormal(Point3D normal1, Point3D normal2, Point3D normal3)
        {
            double x = (normal1.X + normal2.X + normal3.X) / 3;
            double y = (normal1.Y + normal2.Y + normal3.Y) / 3;
            double z = (normal1.Z + normal2.Z + normal3.Z) / 3;
            return new Point3D(x, y, z);
        }

        public static Color getInterpolateColor(Color color1, Color color2, double interpolation)
        {
            Color newColor;
            int R = Clip((int)(color1.R * (1 - interpolation) + color2.R * interpolation));
            int G = Clip((int)(color1.G * (1 - interpolation) + color2.G * interpolation));
            int B = Clip((int)(color1.B * (1 - interpolation) + color2.B * interpolation));
            newColor = Color.FromArgb(R, G, B);
            return newColor;
        }

        private static int Clip(int num)
        {
            return num <= 0 ? 0 : (num >= 255 ? 255 : num);
        }

        public static double distanceFromPointToCamera(Point3D a, Point3D b)
        {
            return Math.Sqrt(Math.Pow((b.X - a.X), 2) + Math.Pow((b.Y - a.Y), 2) + Math.Pow((b.Z - a.Z), 2));
        }

        private static double getIReflection(Point3D light, Point3D normal, Point3D camera, double distance)
        {
            double cosA = generateCosA(light, normal, camera);
            double cosQ = generateCosQ(light, normal);
            return I_A * K_A + (I / distance + 15) * (K_D * cosQ + K_S * Math.Pow(cosA, P));
        }

        private static double generateCosQ(Point3D light, Point3D normal)
        {
            return (normal.X * light.X+ normal.Y * light.Y + normal.Z * light.Z)
                    / (Math.Sqrt(Math.Pow(normal.X, 2) + Math.Pow(normal.Y, 2) + Math.Pow(normal.Z, 2))
                    * Math.Sqrt(Math.Pow(light.X, 2) + Math.Pow(light.Y, 2) + Math.Pow(light.Z, 2)));
        }

        private static double generateCosA(Point3D light, Point3D normal, Point3D camera)
        {
            double commonCalc = (normal.X * light.X + normal.Y * light.Y + normal.Z * light.Z)
                    / (Math.Pow(normal.X, 2) + Math.Pow(normal.Y, 2) + Math.Pow(normal.Z, 2));

            double xR = 2 * normal.X * commonCalc - light.X;

            double yR = 2 * normal.Y * commonCalc - light.Y;

            double zR = 2 * commonCalc - light.Z;

            return (xR * camera.X + yR * camera.Y + zR * camera.Z)
                    / (Math.Sqrt(Math.Pow(camera.X, 2) + Math.Pow(camera.Y, 2) + Math.Pow(camera.Z, 2))
                    * (Math.Sqrt(Math.Pow(xR, 2) + Math.Pow(yR, 2) + Math.Pow(zR, 2))));
        }

        //    public static Point3d[] calculateNormals(Point3d[] cubePoints){
        //        Point3d[] normals = new Point3d[6];
        //        //frontNormal
        //        normals[0] = getNormalVector(cubePoints[3], cubePoints[7], cubePoints[6]);
        //        //backNormal
        //        normals[1] = getNormalVector(cubePoints[4], cubePoints[0], cubePoints[1]);
        //        //topNormal
        //        normals[2] = getNormalVector(cubePoints[7], cubePoints[4], cubePoints[5]);
        //        //bottomNormal
        //        normals[3] = getNormalVector(cubePoints[0], cubePoints[3], cubePoints[2]);
        //        //rightNormal
        //        normals[4]= getNormalVector(cubePoints[0], cubePoints[4], cubePoints[7]);
        //        //leftNormal
        //        normals[5]= getNormalVector(cubePoints[2], cubePoints[6], cubePoints[5]);
        //        return normals;
        //    }

        public static Point3D[] calculateNormals(Point3D[] cubePoints)
        {
            Point3D[] normals = new Point3D[6];
            //frontNormal
            normals[0] = GetNormalVector(cubePoints[7], cubePoints[4], cubePoints[5]);
            //backNormal
            normals[1] = GetNormalVector(cubePoints[0], cubePoints[3], cubePoints[2]);
            //topNormal
            normals[2] = GetNormalVector(cubePoints[2], cubePoints[6], cubePoints[5]);
            //bottomNormal
            normals[3] = GetNormalVector(cubePoints[0], cubePoints[4], cubePoints[7]);
            //rightNormal
            normals[4] = GetNormalVector(cubePoints[3], cubePoints[7], cubePoints[6]);
            //leftNormal
            normals[5] = GetNormalVector(cubePoints[4], cubePoints[0], cubePoints[1]);
            return normals;
        }
    }
}
