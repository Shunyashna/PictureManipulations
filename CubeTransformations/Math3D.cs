using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubeTransformations
{
    public class Math3D
    {

        private static Point3D RotateX(Point3D point3D, double degrees)
        {
            //Here we use Euler's matrix formula for rotating a 3D point x degrees around the x-axis

            //[ a  b  c ] [ x ]   [ x*a + y*b + z*c ]
            //[ d  e  f ] [ y ] = [ x*d + y*e + z*f ]
            //[ g  h  i ] [ z ]   [ x*g + y*h + z*i ]

            //[ 1    0        0   ]
            //[ 0   cos(x)  sin(x)]
            //[ 0   -sin(x) cos(x)]

            double cDegrees = (Math.PI * degrees) / 180.0f; //Convert degrees to radian for .Net Cos/Sin functions
            double cosDegrees = Math.Cos(cDegrees);
            double sinDegrees = Math.Sin(cDegrees);

            double y = (point3D.Y * cosDegrees) + (point3D.Z * sinDegrees);
            double z = (point3D.Y * -sinDegrees) + (point3D.Z * cosDegrees);

            return new Point3D(point3D.X, y, z);
        }

        private static Point3D RotateY(Point3D point3D, double degrees)
        {
            //Y-axis

            //[ cos(x)   0    sin(x)]
            //[   0      1      0   ]
            //[-sin(x)   0    cos(x)]

            double cDegrees = (Math.PI * degrees) / 180.0; //Radians
            double cosDegrees = Math.Cos(cDegrees);
            double sinDegrees = Math.Sin(cDegrees);

            double x = (point3D.X * cosDegrees) + (point3D.Z * sinDegrees);
            double z = (point3D.X * -sinDegrees) + (point3D.Z * cosDegrees);

            return new Point3D(x, point3D.Y, z);
        }

        private static Point3D RotateZ(Point3D point3D, double degrees)
        {
            //Z-axis

            //[ cos(x)  sin(x) 0]
            //[ -sin(x) cos(x) 0]
            //[    0     0     1]

            double radianDegrees = (Math.PI * degrees) / 180.0;
            double cosDegrees = Math.Cos(radianDegrees);
            double sinDegrees = Math.Sin(radianDegrees);

            double x = (point3D.X * cosDegrees) + (point3D.Y * sinDegrees);
            double y = (point3D.X * -sinDegrees) + (point3D.Y * cosDegrees);

            return new Point3D(x, y, point3D.Z);
        }

        public static Point3D Translate(Point3D points3D, Point3D oldOrigin, Point3D newOrigin)
        {
            //Moves a 3D point based on a moved reference point
            Point3D difference = new Point3D(newOrigin.X - oldOrigin.X, newOrigin.Y - oldOrigin.Y, newOrigin.Z - oldOrigin.Z);
            points3D.X += difference.X;
            points3D.Y += difference.Y;
            points3D.Z += difference.Z;
            return points3D;
        }

        //These are to make the above functions workable with arrays of 3D points
        public static Point3D[] RotateX(Point3D[] points3D, double degrees)
        {
            for (int i = 0; i < points3D.Length; i++)
            {
                points3D[i] = RotateX(points3D[i], degrees);
            }
            return points3D;
        }

        public static Point3D[] RotateY(Point3D[] points3D, double degrees)
        {
            for (int i = 0; i < points3D.Length; i++)
            {
                points3D[i] = RotateY(points3D[i], degrees);
            }
            return points3D;
        }

        public static Point3D[] RotateZ(Point3D[] points3D, double degrees)
        {
            for (int i = 0; i < points3D.Length; i++)
            {
                points3D[i] = RotateZ(points3D[i], degrees);
            }
            return points3D;
        }

        public static Point3D[] Translate(Point3D[] points3D, Point3D oldOrigin, Point3D newOrigin)
        {
            for (int i = 0; i < points3D.Length; i++)
            {
                points3D[i] = Translate(points3D[i], oldOrigin, newOrigin);
            }
            return points3D;
        }


    }
}
