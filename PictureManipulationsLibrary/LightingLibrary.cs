using CubeTransformations;
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

        public static (Point3D front, Point3D back, Point3D top, Point3D bottom, Point3D right, Point3D left) CalculateNormals(Point3D[] cubePoints)
        {
            var frontNormal = GetNormalVector(cubePoints[3], cubePoints[7], cubePoints[6]);
            var backNormal = GetNormalVector(cubePoints[4], cubePoints[0], cubePoints[1]);
            var topNormal = GetNormalVector(cubePoints[7], cubePoints[4], cubePoints[5]);
            var bottomNormal = GetNormalVector(cubePoints[0], cubePoints[3], cubePoints[2]);
            var rightNormal = GetNormalVector(cubePoints[0], cubePoints[4], cubePoints[7]);
            var leftNormal = GetNormalVector(cubePoints[2], cubePoints[6], cubePoints[5]);

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
            return (frontNormal, backNormal, topNormal, bottomNormal, rightNormal, leftNormal);
        }
    }
}
