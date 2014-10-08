using System.Windows.Controls;
using System.Windows.Media;

namespace ModulusFE.PaintObjects
{
    internal partial class Rectangle : IPaintObject
    {
        public System.Windows.Shapes.Rectangle _rectangle;
        public void SetArgs(params object[] args)
        {
        }

        public void AddTo(Canvas canvas)
        {
            _rectangle = new System.Windows.Shapes.Rectangle();
            canvas.Children.Add(_rectangle);
        }

        public void RemoveFrom(Canvas canvas)
        {
            canvas.Children.Remove(_rectangle);
        }

        public int ZIndex
        {
            get { return Canvas.GetZIndex(_rectangle); }
            set { Canvas.SetZIndex(_rectangle, value); }
        }

        public Brush Stroke
        {
            get { return _rectangle.Stroke; }
            set { _rectangle.Stroke = value; }
        }

        public Brush Fill
        {
            get { return _rectangle.Fill; }
            set { _rectangle.Fill = value; }
        }
    }
}
