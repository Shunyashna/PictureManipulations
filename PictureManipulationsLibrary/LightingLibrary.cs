using CubeTransformations;
using PictureManipulationsLibrary;
using System;
using System.Collections.Generic;
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

            /*Point3D[] normals = new Point3D[8];

            normals[0] = new Point3D((frontNormal.X + leftNormal.X + bottomNormal.X)/3,
                                 (frontNormal.Y + leftNormal.Y + bottomNormal.Y) / 3,
                                 (frontNormal.Z + leftNormal.Z + bottomNormal.Z) / 3);

            normals[1] = new Point3D((frontNormal.X + leftNormal.X + topNormal.X) / 3,
                                 (frontNormal.Y + leftNormal.Y + topNormal.Y) / 3,
                                 (frontNormal.Z + leftNormal.Z + topNormal.Z) / 3);

            normals[2] = new Point3D((frontNormal.X + rightNormal.X + topNormal.X) / 3,
                                 (frontNormal.Y + rightNormal.Y + topNormal.Y) / 3,
                                 (frontNormal.Z + rightNormal.Z + topNormal.Z) / 3);

            normals[3] = new Point3D((frontNormal.X + rightNormal.X + bottomNormal.X) / 3,
                                 (frontNormal.Y + rightNormal.Y + bottomNormal.Y) / 3,
                                 (frontNormal.Z + rightNormal.Z + bottomNormal.Z) / 3);

            normals[4] = new Point3D((backNormal.X + leftNormal.X + bottomNormal.X) / 3,
                                 (backNormal.Y + leftNormal.Y + bottomNormal.Y) / 3,
                                 (backNormal.Z + leftNormal.Z + bottomNormal.Z) / 3);

            normals[5] = new Point3D((backNormal.X + leftNormal.X + topNormal.X) / 3,
                                 (backNormal.Y + leftNormal.Y + topNormal.Y) / 3,
                                 (backNormal.Z + leftNormal.Z + topNormal.Z) / 3);

            normals[6] = new Point3D((backNormal.X + rightNormal.X + topNormal.X) / 3,
                                 (backNormal.Y + rightNormal.Y + topNormal.Y) / 3,
                                 (backNormal.Z + rightNormal.Z + topNormal.Z) / 3);

            normals[7] = new Point3D((backNormal.X + rightNormal.X + bottomNormal.X) / 3,
                                 (backNormal.Y + rightNormal.Y + bottomNormal.Y) / 3,
                                 (backNormal.Z + rightNormal.Z + bottomNormal.Z) / 3);
            return normals;*/
            return normals;
        }

        public double CosQ(Point3D light, Point3D normal)
        {
            var cosQ = (normal.X * light.X + normal.Y * light.Y + normal.Z * light.Z) /
                (Math.Sqrt(Math.Pow(normal.X,2) + Math.Pow(normal.Y, 2) + Math.Pow(normal.X, 2)) *
                Math.Sqrt(Math.Pow(light.X, 2) + Math.Pow(light.Y, 2) + Math.Pow(light.X, 2)));
            return cosQ;
        }

        public double CosA(Point3D light, Point3D camera, Point3D normal)
        {
            var xR = (2 * normal.X * (normal.X * light.X + normal.Y * light.Y + normal.Z * light.Z) / Math.Sqrt(Math.Pow(normal.X, 2) + Math.Pow(normal.Y, 2) + Math.Pow(normal.X, 2))) - light.X;
            var yR = (2 * normal.Y * (normal.X * light.X + normal.Y * light.Y + normal.Z * light.Z) / Math.Sqrt(Math.Pow(normal.X, 2) + Math.Pow(normal.Y, 2) + Math.Pow(normal.X, 2))) - light.Y;
            var zR = (2 * normal.Z * (normal.X * light.X + normal.Y * light.Y + normal.Z * light.Z) / Math.Sqrt(Math.Pow(normal.X, 2) + Math.Pow(normal.Y, 2) + Math.Pow(normal.X, 2))) - light.Z;
        }


    }
}
