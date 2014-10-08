using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using ModulusFE.LineStudies;
#if SILVERLIGHT
using ModulusFE.SL;
#endif

namespace ModulusFE.PaintObjects
{
    internal partial class SelectionDot : IPaintObject
    {
        public Types.Corner Corner { get; private set; }
        public bool ClickAble { get; set; }

        private System.Windows.Shapes.Rectangle _rectangle;
        private System.Windows.Shapes.Ellipse _ellipse;

        public object Tag { get; set; }

        public void SetArgs(params object[] args)
        {
            Debug.Assert(args.Length > 0);
            Debug.Assert(args[0] is Types.Corner);
            Corner = (Types.Corner)args[0];
            if (args.Length > 1)
                ClickAble = (bool)args[1];
        }

        public void AddTo(Canvas canvas)
        {
            switch (Corner)
            {
                case Types.Corner.TopLeft:
                case Types.Corner.TopRight:
                case Types.Corner.BottomLeft:
                case Types.Corner.BottomRight:
                case Types.Corner.MoveAll:
                    _ellipse = new System.Windows.Shapes.Ellipse();
                    canvas.Children.Add(_ellipse);
                    _ellipse.Stroke = Brushes.Navy;
                    _ellipse.Fill = Brushes.SkyBlue;
                    break;
                case Types.Corner.TopCenter:
                case Types.Corner.BottomCenter:
                case Types.Corner.MiddleLeft:
                case Types.Corner.MiddleRight:
                    _rectangle = new System.Windows.Shapes.Rectangle();
                    canvas.Children.Add(_rectangle);
                    _rectangle.Stroke = Brushes.Navy;
                    _rectangle.Fill = Brushes.SkyBlue;
                    break;
            }

            if (ClickAble)
            {
                Shape shape = (Shape)_rectangle ?? _ellipse;
                switch (Corner)
                {
                    case Types.Corner.TopLeft:
                    case Types.Corner.BottomRight:
                        shape.Cursor =
#if WPF 
              Cursors.SizeNWSE; 
#else
 Cursors.SizeNS;
#endif
                        break;
                    case Types.Corner.TopRight:
                    case Types.Corner.BottomLeft:
                        shape.Cursor =
#if WPF
              Cursors.SizeNESW;
#else
 Cursors.SizeNS;
#endif
                        break;
                    case Types.Corner.TopCenter:
                    case Types.Corner.BottomCenter:
                        shape.Cursor = Cursors.SizeNS;
                        break;
                    case Types.Corner.MiddleLeft:
                    case Types.Corner.MiddleRight:
                        shape.Cursor = Cursors.SizeWE;
                        break;
                    case Types.Corner.MoveAll:
                        shape.Cursor = Cursors.Hand;
                        break;
                }
                shape.MouseLeftButtonDown += Shape_OnMouseLeftButtonDown;
            }
        }

        private void Shape_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Tag == null) return;
            LineStudy lineStudy = Tag as LineStudy;
            if (lineStudy != null)
            {
                lineStudy.StartResize(this, e);
                return;
            }
        }

        public void RemoveFrom(Canvas canvas)
        {
            Shape shape = (Shape)_rectangle ?? _ellipse;
            if (shape == null) return;
            shape.Visibility = Visibility.Collapsed;
            canvas.Children.Remove(shape);
        }

        public int ZIndex
        {
            get { return Canvas.GetZIndex((UIElement)_rectangle ?? _ellipse); }
            set
            {
                Canvas.SetZIndex((UIElement)_rectangle ?? _ellipse, value);
            }
        }

        public void SetPos(Point p)
        {
            Shape shape = (Shape)_rectangle ?? _ellipse;
            if (shape == null) return;

            Canvas.SetLeft(shape, p.X - Constants.SelectionDotSize / 2);
            Canvas.SetTop(shape, p.Y - Constants.SelectionDotSize / 2);
            shape.Width = Constants.SelectionDotSize;
            shape.Height = Constants.SelectionDotSize;
        }

        public Shape Shape
        {
            get { return _rectangle ?? (Shape)_ellipse; }
        }

        public void SetClip(System.Windows.Media.Geometry geometry)
        {
            if (_rectangle != null)
            {
                _rectangle.Clip = geometry;
            }

            if (_ellipse != null)
            {
                _ellipse.Clip = geometry;
            }
        }
    }
}
