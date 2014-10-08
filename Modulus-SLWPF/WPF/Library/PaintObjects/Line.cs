using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ModulusFE.PaintObjects
{
    internal partial class Line : IPaintObject
    {
        public System.Windows.Shapes.Line _line;

        public void SetArgs(params object[] args)
        {
        }

        public void AddTo(Canvas canvas)
        {
            _line = new System.Windows.Shapes.Line { StrokeEndLineCap = PenLineCap.Round };
            canvas.Children.Add(_line);
        }

        public void RemoveFrom(Canvas canvas)
        {
            canvas.Children.Remove(_line);
        }

        public int ZIndex
        {
            get { return Canvas.GetZIndex(_line); }
            set { Canvas.SetZIndex(_line, value); }
        }

        public Brush Stroke
        {
            get { return _line.Stroke; }
            set { _line.Stroke = value; }
        }

        public double StrokeThickness
        {
            get { return _line.StrokeThickness; }
            set { _line.StrokeThickness = value; }
        }

        public double Opacity
        {
            get { return _line.Opacity; }
            set { _line.Opacity = value; }
        }

        public bool Visible
        {
            get { return _line.Visibility == Visibility.Visible; }
            set
            {
                _line.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
                //_line.IsHitTestVisible = value;
            }
        }
    }
}
