using System;
using System.Windows;

namespace ModulusFE
{
    /// <summary>
    /// A help class that deails with Geometry math
    /// </summary>
    public static class Geometry
    {
        /// <summary>
        /// Represents a <see cref="Point"/> with 0 coordinates
        /// </summary>
        public static readonly Point ZeroPoint = new Point(0, 0);

        /// <summary>
        /// Represents the multiplier used to transform degrees to radians
        /// </summary>
        public static readonly double AngleToRadK = Math.PI / 180.0;

        /// <summary>
        /// Represents the multiplier used to transform radians to degrees
        /// </summary>
        public static readonly double RadToAngleK = 180.0 / Math.PI;

        /// <summary>
        /// Calculates distance between 2 <see cref="Point"/>s
        /// </summary>
        /// <param name="self"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        public static double Distance(this Point self, Point other)
        {
            double x = other.X - self.X;
            double y = other.Y - self.Y;
            return Math.Sqrt(x * x + y * y);
        }

        /// <summary>
        /// Gets a point located at distance from center placed 
        /// at angle
        /// </summary>
        /// <param name="angle"></param>
        /// <param name="distance"></param>
        /// <param name="center"></param>
        /// <returns></returns>
        public static Point FindPoint(double angle, double distance, Point center)
        {
            Point p = new Point();
            double theta = angle * AngleToRadK;
            p.X = center.X + distance * Math.Cos(theta);
            p.Y = center.Y + distance * Math.Sin(theta);
            return p;
        }

        /// <summary>
        /// Gets a point located at intersection of line starting from center at given angle
        /// and one of the edges of given rectangle
        /// </summary>
        /// <param name="center"></param>
        /// <param name="angle"></param>
        /// <param name="rectangle"></param>
        /// <returns>Coordinates of point or <see cref="ZeroPoint"/> in case when center
        /// is outside of given rectangle
        /// </returns>
        public static Point FindPoint(Point center, double angle, Rect rectangle)
        {
            if (!rectangle.Contains(center))
            {
                return ZeroPoint;
            }

            Point p2 = FindPoint(angle, Math.Max(rectangle.Width, rectangle.Height), center);
            bool code;

            Point p = Intersects(center, p2, new Point(rectangle.Left, rectangle.Top), new Point(rectangle.Right, rectangle.Top), out code);
            if (code)
            {
                return p;
            }

            p = Intersects(center, p2, new Point(rectangle.Right, rectangle.Top), new Point(rectangle.Right, rectangle.Bottom), out code);
            if (code)
            {
                return p;
            }

            p = Intersects(center, p2, new Point(rectangle.Right, rectangle.Bottom), new Point(rectangle.Left, rectangle.Bottom), out code);
            if (code)
            {
                return p;
            }

            return Intersects(center, p2, new Point(rectangle.Left, rectangle.Bottom), new Point(rectangle.Left, rectangle.Top), out code);
        }

        /// <summary>
        /// Verifies if 2 segments have an intersection point
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        /// <param name="p4"></param>
        /// <param name="ok"></param>
        /// <returns></returns>
        public static Point Intersects(Point p1, Point p2, Point p3, Point p4, out bool ok)
        {
            Point p = Intersects(new Segment { Start = p1, End = p2 }, new Segment { Start = p3, End = p4 });
            ok = p != ZeroPoint;
            return p;
        }

        /// <summary>
        /// Verifies if 2 segments have an intersection point
        /// </summary>
        /// <param name="AB"></param>
        /// <param name="CD"></param>
        /// <returns></returns>
        public static Point Intersects(Segment AB, Segment CD)
        {
            double deltaACy = AB.Start.Y - CD.Start.Y;
            double deltaDCx = CD.End.X - CD.Start.X;
            double deltaACx = AB.Start.X - CD.Start.X;
            double deltaDCy = CD.End.Y - CD.Start.Y;
            double deltaBAx = AB.End.X - AB.Start.X;
            double deltaBAy = AB.End.Y - AB.Start.Y;

            double denominator = deltaBAx * deltaDCy - deltaBAy * deltaDCx;
            double numerator = deltaACy * deltaDCx - deltaACx * deltaDCy;

            if (denominator == 0)
            {
                if (numerator == 0)
                {
                    // collinear. Potentially infinite intersection points.
                    // Check and return one of them.
                    if (AB.Start.X >= CD.Start.X && AB.Start.X <= CD.End.X)
                    {
                        return AB.Start;
                    }
                    if (CD.Start.X >= AB.Start.X && CD.Start.X <= AB.End.X)
                    {
                        return CD.Start;
                    }
                    return ZeroPoint;
                }

                // parallel
                return ZeroPoint;
            }

            double r = numerator / denominator;
            if (r < 0 || r > 1)
            {
                return ZeroPoint;
            }

            double s = (deltaACy * deltaBAx - deltaACx * deltaBAy) / denominator;
            if (s < 0 || s > 1)
            {
                return ZeroPoint;
            }

            return new Point(AB.Start.X + r * deltaBAx, AB.Start.Y + r * deltaBAy);
        }

        /// <summary>
        /// Calculates the distance between a line and a point
        /// </summary>
        /// <param name="cx">Point - X</param>
        /// <param name="cy">Point - Y</param>
        /// <param name="ax"></param>
        /// <param name="ay"></param>
        /// <param name="bx"></param>
        /// <param name="by"></param>
        /// <param name="distanceSegment"></param>
        /// <param name="distanceLine"></param>
        public static void DistanceFromLine(double cx, double cy, double ax, double ay,
                double bx, double by, out double distanceSegment,
                out double distanceLine)
        {

            //
            // find the distance from the point (cx,cy) to the line
            // determined by the points (ax,ay) and (bx,by)
            //
            // distanceSegment = distance from the point to the line segment
            // distanceLine = distance from the point to the line (assuming
            //					infinite extent in both directions
            //

            /*

            Subject 1.02: How do I find the distance from a point to a line?


                Let the point be C (Cx,Cy) and the line be AB (Ax,Ay) to (Bx,By).
                Let P be the point of perpendicular projection of C on AB.  The parameter
                r, which indicates P's position along AB, is computed by the dot product 
                of AC and AB divided by the square of the length of AB:
    
                (1)     AC dot AB
                    r = ---------  
                        ||AB||^2
    
                r has the following meaning:
    
                    r=0      P = A
                    r=1      P = B
                    r<0      P is on the backward extension of AB
                    r>1      P is on the forward extension of AB
                    0<r<1    P is interior to AB
    
                The length of a line segment in d dimensions, AB is computed by:
    
                    L = sqrt( (Bx-Ax)^2 + (By-Ay)^2 + ... + (Bd-Ad)^2)

                so in 2D:   
    
                    L = sqrt( (Bx-Ax)^2 + (By-Ay)^2 )
    
                and the dot product of two vectors in d dimensions, U dot V is computed:
    
                    D = (Ux * Vx) + (Uy * Vy) + ... + (Ud * Vd)
    
                so in 2D:   
    
                    D = (Ux * Vx) + (Uy * Vy) 
    
                So (1) expands to:
    
                        (Cx-Ax)(Bx-Ax) + (Cy-Ay)(By-Ay)
                    r = -------------------------------
                                      L^2

                The point P can then be found:

                    Px = Ax + r(Bx-Ax)
                    Py = Ay + r(By-Ay)

                And the distance from A to P = r*L.

                Use another parameter s to indicate the location along PC, with the 
                following meaning:
                       s<0      C is left of AB
                       s>0      C is right of AB
                       s=0      C is on AB

                Compute s as follows:

                        (Ay-Cy)(Bx-Ax)-(Ax-Cx)(By-Ay)
                    s = -----------------------------
                                    L^2


                Then the distance from C to P = |s|*L.

            */


            double r_numerator = (cx - ax) * (bx - ax) + (cy - ay) * (by - ay);
            double r_denomenator = (bx - ax) * (bx - ax) + (by - ay) * (by - ay);
            double r = r_numerator / r_denomenator;
            //
            //     
            double s = ((ay - cy) * (bx - ax) - (ax - cx) * (by - ay)) / r_denomenator;

            distanceLine = Math.Abs(s) * Math.Sqrt(r_denomenator);

            //
            // (xx,yy) is the point on the lineSegment closest to (cx,cy)
            //

            if ((r >= 0) && (r <= 1))
            {
                distanceSegment = distanceLine;
            }
            else
            {

                double dist1 = (cx - ax) * (cx - ax) + (cy - ay) * (cy - ay);
                double dist2 = (cx - bx) * (cx - bx) + (cy - by) * (cy - by);
                distanceSegment = Math.Sqrt(dist1 < dist2 ? dist1 : dist2);
            }
        }
    }
}
