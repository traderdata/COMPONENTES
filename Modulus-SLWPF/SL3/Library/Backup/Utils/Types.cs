using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
#if SILVERLIGHT
using ModulusFE.SL;
#endif

namespace ModulusFE.PaintObjects
{
    ///<summary>
    /// Extra types used in library
    ///</summary>
    public static class Types
    {

        internal static void SetShapePattern(Shape shape, LinePattern pattern)
        {
            switch (pattern)
            {
                case LinePattern.Dash:
                    shape.StrokeDashArray = new DoubleCollection { 4, 3 };
                    break;
                case LinePattern.Dot:
                    shape.StrokeDashArray = new DoubleCollection { 1, 2 };
                    break;
                case LinePattern.DashDot:
                    shape.StrokeDashArray = new DoubleCollection { 4, 2, 1, 2 };
                    break;
                case LinePattern.None:
                    shape.Stroke = Brushes.Transparent;
                    break;
                case LinePattern.Solid:
                    shape.StrokeDashArray = null;
                    break;
            }
        }

        internal enum Corner
        {
            None,
            MoveAll,
            TopLeft,
            TopRight,
            BottomLeft,
            BottomRight,
            TopCenter,
            BottomCenter,
            MiddleLeft,
            MiddleRight
        }

        internal struct Location
        {
            public double X1;
            public double X2;
            public double Y1;
            public double Y2;

            public Location(double x1, double y1, double x2, double y2)
            {
                X1 = x1;
                X2 = x2;
                Y1 = y1;
                Y2 = y2;
            }

            public Location(Point p1, Point p2)
            {
                X1 = p1.X;
                Y1 = p1.Y;
                X2 = p2.X;
                Y2 = p2.Y;
            }

            public Point P1
            {
                get { return new Point(X1, Y1); }
            }

            public Point P2
            {
                get { return new Point(X2, Y2); }
            }
        }

        /// <summary>
        /// Used instead of standard Rectangle, cause standard one has too many checks against negative Width &amp; Height
        /// </summary>
        public struct RectEx
        {
            ///<summary>
            ///</summary>
            public double Left;
            ///<summary>
            ///</summary>
            public double Top;
            ///<summary>
            ///</summary>
            public double Right;
            ///<summary>
            ///</summary>
            public double Bottom;

            ///<summary>
            /// Ctor
            ///</summary>
            ///<param name="left"></param>
            ///<param name="top"></param>
            ///<param name="right"></param>
            ///<param name="bottom"></param>
            public RectEx(double left, double top, double right, double bottom)
            {
                Left = left;
                Top = top;
                Right = right;
                Bottom = bottom;
            }

            ///<summary>
            /// Empty rectangle
            ///</summary>
            public static RectEx Empty
            {
                get { return new RectEx(0, 0, 0, 0); }
            }

            ///<summary>
            ///</summary>
            public double Width
            {
                get { return Right - Left; }
            }

            ///<summary>
            ///</summary>
            public double Height
            {
                get { return Bottom - Top; }
            }

            ///<summary>
            ///</summary>
            public bool IsZero
            {
                get { return Width == 0 || Height == 0; }
            }

            ///<summary>
            ///</summary>
            ///<returns></returns>
            public RectEx Normalize()
            {
                if (Left > Right)
                    Utils.Swap(ref Left, ref Right);
                if (Top > Bottom)
                    Utils.Swap(ref Top, ref Bottom);

                return this;
            }

            ///<summary>
            ///</summary>
            public Point TopLeft
            {
                get { return new Point(Left, Top); }
            }

            ///<summary>
            ///</summary>
            public Point TopRight
            {
                get { return new Point(Right, Top); }
            }

            ///<summary>
            ///</summary>
            public Point BottomLeft
            {
                get { return new Point(Left, Bottom); }
            }

            ///<summary>
            ///</summary>
            public Point BottomRight
            {
                get { return new Point(Right, Bottom); }
            }

            ///<summary>
            ///</summary>
            public Point TopCenter
            {
                get { return new Point((Left + Right) / 2, Top); }
            }

            ///<summary>
            ///</summary>
            public Point BottomCenter
            {
                get { return new Point((Left + Right) / 2, Bottom); }
            }

            ///<summary>
            ///</summary>
            public Point MiddleLeft
            {
                get { return new Point(Left, (Top + Bottom) / 2); }
            }

            ///<summary>
            ///</summary>
            public Point MiddleRight
            {
                get { return new Point(Right, (Top + Bottom) / 2); }
            }

            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                return string.Format("Left: {0}, Top: {1}, Width: {2}, Height: {3}",
                  Left, Top, Width, Height);
            }

            ///<summary>
            ///</summary>
            ///<param name="thickness"></param>
            ///<returns></returns>
            public Point[] MainDiagonalPolygon(double thickness)
            {
                return new Segment(Left, Top, Right, Bottom).SurroundRectangle(thickness);
            }

            ///<summary>
            ///</summary>
            ///<param name="p"></param>
            ///<param name="strokeThickness"></param>
            ///<returns></returns>
            public bool PointOnMainDiagonal(Point p, int strokeThickness)
            {
                return p.InPolygon(MainDiagonalPolygon(strokeThickness));
            }
        }
    }
}

namespace ModulusFE
{
    ///<summary>
    ///</summary>
    ///<typeparam name="T"></typeparam>
    public class EventArgs<T> : EventArgs
    {
        ///<summary>
        ///</summary>
        public T Data { get; private set; }

        ///<summary>
        ///</summary>
        ///<param name="data"></param>
        public EventArgs(T data)
        {
            Data = data;
        }
    }

#if FW3
  ///<summary>
  /// Represents an ordered collection of members from different hierarchies. 
  ///</summary>
  ///<typeparam name="T1"></typeparam>
  ///<typeparam name="T2"></typeparam>
  public struct Tuple<T1, T2>
  {
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="field1"></param>
    /// <param name="field2"></param>
    public Tuple(T1 field1, T2 field2)
    {
      Field1 = field1;
      Field2 = field2;
    }

    /// <summary>
    /// Field1
    /// </summary>
    public readonly T1 Field1;
    /// <summary>
    /// Field2
    /// </summary>
    public readonly T2 Field2;

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
      int hash1 = Field1 == null ? 0 : Field1.GetHashCode();
      int hash2 = Field2 == null ? 0 : Field2.GetHashCode();

      return ((hash1 << 5) + hash1) ^ hash2;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object obj)
    {
      if (obj is Tuple<T1, T2>)
      {
        Tuple<T1, T2> t = (Tuple<T1, T2>)obj;

        return
          (Field1 == null ? t.Field1 == null : Field1.Equals(t.Field1)) &&
          (Field2 == null ? t.Field2 == null : Field2.Equals(t.Field2));
      }

      return false;
    }
  }

#endif
}


