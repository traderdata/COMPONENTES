using System.Windows;
using System.Windows.Controls;

namespace ModulusFE
{
    ///<summary>
    ///</summary>
    public partial class ChartPanelMovePlaceholder : Control
    {
#if WPF
    static ChartPanelMovePlaceholder()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(ChartPanelMovePlaceholder), new FrameworkPropertyMetadata(typeof(ChartPanelMovePlaceholder)));
    }
#endif

#if SILVERLIGHT
        ///<summary>
        /// ctor
        ///</summary>
        public ChartPanelMovePlaceholder()
        {
            DefaultStyleKey = typeof(ChartPanelMovePlaceholder);
        }
#endif

        internal bool Visible
        {
            get { return Visibility == Visibility.Visible; }
            set { Visibility = value ? Visibility.Visible : Visibility.Collapsed; }
        }

        internal void ShowOnPanel(ChartPanel chartPanel)
        {
            Rect rcPanelPaintBounds = chartPanel.CanvasRect;
            Canvas.SetTop(this, chartPanel.Top);
            Canvas.SetLeft(this, rcPanelPaintBounds.Left);
            Width = rcPanelPaintBounds.Width;
        }
    }

}
