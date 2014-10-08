using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Shapes;

namespace ModulusFE
{
    ///<summary>
    /// Segment
    ///</summary>
    public class Segment : INotifyPropertyChanged
    {
        ///<summary>
        /// Creates an empty segment
        ///</summary>
        public Segment()
        {
            Start = Geometry.ZeroPoint;
            End = Geometry.ZeroPoint;
        }

        ///<summary>
        /// 
        ///</summary>
        ///<param name="start"></param>
        ///<param name="end"></param>
        public Segment(Point start, Point end)
        {
            Start = start;
            End = end;
        }

        ///<summary>
        ///</summary>
        ///<param name="x1"></param>
        ///<param name="y1"></param>
        ///<param name="x2"></param>
        ///<param name="y2"></param>
        public Segment(double x1, double y1, double x2, double y2)
        {
            Start = new Point(x1, y1);
            End = new Point(x2, y2);
        }

        ///<summary>
        /// Ctor
        ///</summary>
        ///<param name="origin"></param>
        ///<param name="length"></param>
        ///<param name="angle"></param>
        public Segment(Point origin, double length, double angle)
        {
            X1 = origin.X;
            Y1 = origin.Y;

            angle *= Math.PI / 180.0;

            X2 = X1 + length * Math.Cos(angle);
            Y2 = Y1 + length * Math.Sin(angle);
        }

        #region Implementation of INotifyPropertyChanged

        ///<summary>
        ///</summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected virtual void InvokePropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        #endregion

        #region X1

        private double _x1;

        ///<summary>
        ///</summary>
        public static readonly PropertyChangedEventArgs X1ChangedEventsArgs =
          ObservableHelper.CreateArgs<Segment>(_ => _.X1);

        ///<summary>
        ///</summary>
        public double X1
        {
            get { return _x1; }
            set
            {
                if (_x1 != value)
                {
                    _x1 = value;
                    InvokePropertyChanged(X1ChangedEventsArgs);
                }
            }
        }

        #endregion

        #region Y1

        private double _y1;

        ///<summary>
        ///</summary>
        public static readonly PropertyChangedEventArgs Y1ChangedEventsArgs =
          ObservableHelper.CreateArgs<Segment>(_ => _.Y1);

        ///<summary>
        ///</summary>
        public double Y1
        {
            get { return _y1; }
            set
            {
                if (_y1 != value)
                {
                    _y1 = value;
                    InvokePropertyChanged(Y1ChangedEventsArgs);
                }
            }
        }

        #endregion

        #region X2

        private double _x2;

        ///<summary>
        ///</summary>
        public static readonly PropertyChangedEventArgs X2ChangedEventsArgs =
          ObservableHelper.CreateArgs<Segment>(_ => _.X2);

        ///<summary>
        ///</summary>
        public double X2
        {
            get { return _x2; }
            set
            {
                if (_x2 != value)
                {
                    _x2 = value;
                    InvokePropertyChanged(X2ChangedEventsArgs);
                }
            }
        }

        #endregion

        #region Y2

        private double _y2;

        ///<summary>
        ///</summary>
        public static readonly PropertyChangedEventArgs Y2ChangedEventsArgs =
          ObservableHelper.CreateArgs<Segment>(_ => _.Y2);

        ///<summary>
        ///</summary>
        public double Y2
        {
            get { return _y2; }
            set
            {
                if (_y2 != value)
                {
                    _y2 = value;
                    InvokePropertyChanged(Y2ChangedEventsArgs);
                }
            }
        }

        #endregion

        ///<summary>
        ///</summary>
        public Point Start
        {
            get { return new Point(X1, Y1); }
            set
            {
                X1 = value.X;
                Y1 = value.Y;
            }
        }

        ///<summary>
        ///</summary>
        public Point End
        {
            get { return new Point(X2, Y2); }
            set
            {
                X2 = value.X;
                Y2 = value.Y;
            }
        }

        ///<summary>
        ///</summary>
        ///<param name="line"></param>
        public void BindToLine(Line line)
        {
            line.SetBinding(Line.X1Property, this.CreateOneWayBinding("X1"));
            line.SetBinding(Line.X2Property, this.CreateOneWayBinding("X2"));
            line.SetBinding(Line.Y1Property, this.CreateOneWayBinding("Y1"));
            line.SetBinding(Line.Y2Property, this.CreateOneWayBinding("Y2"));
        }

        ///<summary>
        ///</summary>
        ///<param name="x1"></param>
        ///<param name="y1"></param>
        ///<param name="x2"></param>
        ///<param name="y2"></param>
        public void Set(double x1, double y1, double x2, double y2)
        {
            X1 = x1;
            X2 = x2;
            Y1 = y1;
            Y2 = y2;
        }

        ///<summary>
        ///</summary>
        ///<param name="start"></param>
        ///<param name="end"></param>
        public void Set(Point start, Point end)
        {
            Start = start;
            End = end;
        }

        ///<summary>
        /// Gets segment's angle
        ///</summary>
        public double Angle
        {
            get
            {
                return Math.Atan2(Y2 - Y1, X2 - X1) * Geometry.RadToAngleK;
            }
        }

        ///<summary>
        /// Postion segment with start in X1, Y1 and end that is border of bounding rectangle
        ///</summary>
        ///<param name="x1"></param>
        ///<param name="y1"></param>
        ///<param name="angle"></param>
        ///<param name="boundRectangle"></param>
        public void Set(double x1, double y1, double angle, Rect boundRectangle)
        {
            X1 = x1;
            Y1 = y1;
            End = Geometry.FindPoint(new Point(x1, y1), angle, boundRectangle);
        }

        ///<summary>
        ///</summary>
        ///<param name="x1"></param>
        ///<param name="y1"></param>
        ///<param name="x2"></param>
        ///<param name="y2"></param>
        ///<param name="boundRectangle"></param>
        public void SetThroughPoint(double x1, double y1, double x2, double y2, Rect boundRectangle)
        {
            X1 = x1;
            Y1 = y1;
            SetEnd(Math.Atan2(y2 - Y1, x2 - X1) * Geometry.RadToAngleK, boundRectangle);
        }

        ///<summary>
        ///</summary>
        ///<param name="x2"></param>
        ///<param name="y2"></param>
        ///<param name="boundRectangle"></param>
        public void SetEndThroughPoint(double x2, double y2, Rect boundRectangle)
        {
            SetEnd(Math.Atan2(y2 - Y1, x2 - X1) * Geometry.RadToAngleK, boundRectangle);
        }

        ///<summary>
        ///</summary>
        ///<param name="end"></param>
        ///<param name="boundRectangle"></param>
        public void SetEndThroughPoint(Point end, Rect boundRectangle)
        {
            SetEnd(Math.Atan2(end.Y - Y1, end.X - X1) * Geometry.RadToAngleK, boundRectangle);
        }

        ///<summary>
        ///</summary>
        ///<param name="angle"></param>
        ///<param name="boundRectangle"></param>
        public void SetEnd(double angle, Rect boundRectangle)
        {
            End = Geometry.FindPoint(Start, angle, boundRectangle);
        }

        ///<summary>
        ///</summary>
        ///<param name="p"></param>
        ///<param name="delta"></param>
        ///<returns></returns>
        public bool HasPoint(Point p, short delta)
        {
            double distanceSegment, distanceLine;
            Geometry.DistanceFromLine(p.X, p.Y, X1, Y1, X2, Y2, out distanceSegment, out distanceLine);

            return Math.Abs(distanceSegment) <= delta;
        }

        ///<summary>
        /// Gets the segment length
        ///</summary>
        public double Length
        {
            get
            {
                double x = End.X - Start.X;
                double y = End.Y - Start.Y;
                return Math.Sqrt(x * x + y * y);
            }
        }

        ///<summary>
        /// Gets the slope of segment
        ///</summary>
        public double Slope
        {
            get { return (Start.Y - End.Y) / (Start.X - End.X); }
        }

        ///<summary>
        /// Returns a polygon that surrounds the given segment
        ///</summary>
        ///<param name="thickness"></param>
        ///<returns></returns>
        public Point[] SurroundRectangle(double thickness)
        {
            double dx = X2 - X1;
            double dy = Y2 - Y1;

            if ((dx != 0) || (dy != 0))
            {
                double norm = 1.0 / Math.Sqrt(dx * dx + dy * dy);
                double uDX = dx * norm;
                double uDY = dy * norm;
                double tSum = (thickness / 2.0f) * (uDX + uDY);
                double tDiff = (thickness / 2.0f) * (uDX - uDY);

                return new[]
                 {
                   new Point(X2 + tDiff, Y2 + tSum),
                   new Point(X2 + tSum, Y2 - tDiff),
                   new Point(X1 - tDiff, Y1 - tSum),
                   new Point(X1 - tSum, Y1 + tDiff),
                 };
            }
            return new Point[0];
        }

        ///<summary>
        /// Inflates the current segment
        ///</summary>
        ///<param name="pct"></param>
        ///<returns></returns>
        public Segment Inflate(double pct)
        {
            int signX = Math.Sign(X2 - X1);
            int signY = Math.Sign(Y2 - Y1);

            double l = Length;
            double d = (l - (l * (1 + pct / 100.0))) / 2;

            if (X2 == X1)
            {
                Y1 += signY * d;
                Y2 -= signY * d;
                return this;
            }
            if (Y2 == Y1)
            {
                X1 += signX * d;
                X2 -= signX * d;
                return this;
            }


            double slope = (Y1 - Y2) / (X1 - X2);
            double angle = Math.Atan(slope);

            X1 += signX * Math.Cos(angle) * d;
            Y1 += signX * Math.Sin(angle) * d;
            X2 += -signX * Math.Cos(angle) * d;
            Y2 += -signX * Math.Sin(angle) * d;

            return this;
        }

        ///<summary>
        /// Returns a point that will be used to draw a normal-line to current segment
        ///</summary>
        ///<returns></returns>
        public Point Normal()
        {
            double nx, ny;

            if (X1 == X2)
            {
                nx = Math.Sign(Y2 - Y1);
                ny = 0;
            }
            else
            {
                double f = (Y2 - Y1) / (X2 - X1);
                nx = f * Math.Sign(X2 - X1) / Math.Sqrt(1 + f * f);
                ny = -1 * Math.Sign(X2 - X1) / Math.Sqrt(1 + f * f);
            }
            return new Point(nx, ny);
        }

        ///<summary>
        /// Gets the distance between current segment and the specified point
        ///</summary>
        ///<param name="p"></param>
        ///<returns></returns>
        public double DistanceToPoint(Point p)
        {
            double normalLength = Math.Sqrt((X2 - X1) * (X2 - X1) + (Y2 - Y1) * (Y2 - Y1));
            return Math.Abs((p.X - X1) * (Y2 - Y1) - (p.Y - Y1) * (X2 - X1)) / normalLength;
        }

        ///<summary>
        ///</summary>
        public Segment Normalize()
        {
            if (X1 > X2)
            {
                double t = X1;
                X1 = X2;
                X2 = t;
            }

            if (Y1 > Y2)
            {
                double t = Y1;
                Y1 = Y2;
                Y2 = t;
            }

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("[{0},{1}]-[{2}{3}]", X1, Y1, X2, Y2);
        }
    }
}
