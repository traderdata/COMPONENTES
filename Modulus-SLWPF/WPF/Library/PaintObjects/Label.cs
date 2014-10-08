using System.Windows.Controls;

namespace ModulusFE.PaintObjects
{
    internal partial class Label : IPaintObject
    {
        internal TextBlock _textBlock;

        public void SetArgs(params object[] args)
        {

        }

        public void AddTo(Canvas canvas)
        {
            _textBlock = new TextBlock();
            canvas.Children.Add(_textBlock);
        }

        public void RemoveFrom(Canvas canvas)
        {
            canvas.Children.Remove(_textBlock);
        }

        public int ZIndex
        {
            get { return Canvas.GetZIndex(_textBlock); }
            set
            {
                Canvas.SetZIndex(_textBlock, value);
            }
        }

        public string Text
        {
            get { return _textBlock.Text; }
            set { _textBlock.Text = value; }
        }

        public double Left
        {
            get { return Canvas.GetLeft(_textBlock); }
            set { Canvas.SetLeft(_textBlock, value); }
        }

        public double Top
        {
            get { return Canvas.GetTop(_textBlock); }
            set { Canvas.SetTop(_textBlock, value); }
        }
    }
}
