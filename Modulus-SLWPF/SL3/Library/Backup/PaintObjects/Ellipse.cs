using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Media;

namespace ModulusFE.PaintObjects
{
    internal partial class Ellipse : IPaintObject
    {
        public System.Windows.Shapes.Ellipse _ellipse;

        public void SetArgs(params object[] args)
        {
            if (args == null || args.Length == 0) return;
            Debug.Assert(args.Length == 4);
        }

        public void AddTo(Canvas canvas)
        {
            _ellipse = new System.Windows.Shapes.Ellipse();
            canvas.Children.Add(_ellipse);


        }

        public void RemoveFrom(Canvas canvas)
        {
            canvas.Children.Remove(_ellipse);
        }

        public int ZIndex
        {
            get { return Canvas.GetZIndex(_ellipse); }
            set { Canvas.SetZIndex(_ellipse, value); }
        }

        public Brush Stroke
        {
            get { return _ellipse.Stroke; }
            set { _ellipse.Stroke = value; }
        }

        public double StrokeThickness
        {
            get { return _ellipse.StrokeThickness; }
            set { _ellipse.StrokeThickness = value; }
        }

        public Brush Fill
        {
            get { return _ellipse.Fill; }
            set { _ellipse.Fill = value; }
        }
    }
}
