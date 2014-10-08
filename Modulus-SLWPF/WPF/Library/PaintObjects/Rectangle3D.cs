using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Media;
#if SILVERLIGHT
using ModulusFE.SL;
#endif

namespace ModulusFE.PaintObjects
{
    internal partial class Rectangle3D : IPaintObject
    {
        private System.Windows.Shapes.Rectangle _rect;
        private readonly System.Windows.Shapes.Line[] _lines = new System.Windows.Shapes.Line[4];

        public void SetArgs(params object[] args)
        {
            if (args == null || args.Length == 0) return;
            Debug.Assert(args.Length == 3);//Rect bounds, Color _topLeft, Color BottomRight
        }

        public void AddTo(Canvas canvas)
        {
            _rect = new System.Windows.Shapes.Rectangle
                      {
                          Stroke = Brushes.Transparent,
                      };
            canvas.Children.Add(_rect);
            for (int i = 0; i < _lines.Length; i++)
            {
                _lines[i] = new System.Windows.Shapes.Line { StrokeThickness = 1 };
                canvas.Children.Add(_lines[i]);
            }
        }

        public void SetPos(Types.RectEx bounds, Brush topLeft, Brush bottomRight)
        {
            Canvas.SetLeft(_rect, bounds.Left);
            Canvas.SetTop(_rect, bounds.Top);
            _rect.Width = bounds.Width;
            _rect.Height = bounds.Height;
            _rect.Fill = Brushes.Transparent;

            //top
            _lines[0].X1 = bounds.Left;
            _lines[0].Y1 = _lines[0].Y2 = bounds.Top;
            _lines[0].X2 = bounds.Right - 1;
            _lines[0].Stroke = topLeft;
            //left
            _lines[1].X1 = _lines[1].X2 = bounds.Left;
            _lines[1].Y1 = bounds.Top;
            _lines[1].Y2 = bounds.Bottom;
            _lines[1].Stroke = topLeft;
            //bottom
            _lines[2].X1 = bounds.Left;
            _lines[2].Y1 = _lines[2].Y2 = bounds.Bottom;
            _lines[2].X2 = bounds.Right - 1;
            _lines[2].Stroke = bottomRight;
            //right
            _lines[3].X1 = _lines[3].X2 = bounds.Right - 1;
            _lines[3].Y1 = bounds.Top;
            _lines[3].Y2 = bounds.Bottom;
            _lines[3].Stroke = bottomRight;
        }

        public void RemoveFrom(Canvas canvas)
        {
            canvas.Children.Remove(_rect);
            foreach (var line in _lines)
            {
                canvas.Children.Remove(line);
            }
        }

        public int ZIndex
        {
            get { return Canvas.GetZIndex(_rect); }
            set
            {
                Canvas.SetZIndex(_rect, value);
                foreach (var line in _lines)
                {
                    Canvas.SetZIndex(line, value + 1);
                }
            }
        }
    }
}
