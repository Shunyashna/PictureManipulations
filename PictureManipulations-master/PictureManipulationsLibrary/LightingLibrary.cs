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
            normals.Front = GetNormalVector(cubePoints[3], cubePoints[7], cubePoints[6]);
            normals.Back = GetNormalVector(cubePoints[4], cubePoints[0], cubePoints[1]);
            normals.Top = GetNormalVector(cubePoints[7], cubePoints[4], cubePoints[5]);
            normals.Bottom = GetNormalVector(cubePoints[0], cubePoints[3], cubePoints[2]);
            normals.Right = GetNormalVector(cubePoints[0], cubePoints[4], cubePoints[7]);
            normals.Left = GetNormalVector(cubePoints[2], cubePoints[6], cubePoints[5]);
            
            return normals;
        }

        public static double CosQ(Point3D light, Point3D normal, Point3D vertice)
        {
            //light = new Point3D(light.X - vertice.X, light.Y - vertice.Y, light.Z - vertice.Z);
            var cosQ = (normal.X * light.X + normal.Y * light.Y + normal.Z * light.Z) /
                (Math.Sqrt(Math.Pow(normal.X,2) + Math.Pow(normal.Y, 2) + Math.Pow(normal.X, 2)) *
                Math.Sqrt(Math.Pow(light.X, 2) + Math.Pow(light.Y, 2) + Math.Pow(light.X, 2)));
            return cosQ;
        }

        public static double CosA(Point3D light, Point3D camera, Point3D normal, Point3D vertice)
        {
            //light = new Point3D(light.X - vertice.X, light.Y - vertice.Y, light.Z - vertice.Z);
            //camera = new Point3D(camera.X - vertice.X, camera.Y - vertice.Y, camera.Z - vertice.Z);
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
            double Kd = 0.7, Ks = 0.2, Ka = 0.4;
            int p = 3;
            double Ia = 1, I = 1;
            return Ia * Ka + I * (Ks * Math.Pow(CosA(light, camera, normal, vertice), p) + Kd * CosQ(light, normal, vertice));
        }

        


    }
}
