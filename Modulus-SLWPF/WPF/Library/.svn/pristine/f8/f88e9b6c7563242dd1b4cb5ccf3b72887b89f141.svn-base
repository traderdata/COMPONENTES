using System.Windows;
using System.Windows.Controls;
using ModulusFE.Interfaces;

namespace ModulusFE
{
    ///<summary>
    /// Button used in various places of ChartPanel to create the ability to show
    /// more information about specific items from underlying panel
    ///</summary>
    public class ChartPanelMoreIndicator : Button
    {
#if WPF
    static ChartPanelMoreIndicator()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(ChartPanelMoreIndicator),
        new FrameworkPropertyMetadata(typeof(ChartPanelMoreIndicator)));
    }
#endif
#if SILVERLIGHT
        ///<summary>
        /// ctor
        ///</summary>
        public ChartPanelMoreIndicator()
        {
            DefaultStyleKey = typeof(ChartPanelMoreIndicator);
        }
#endif

        ///<summary>
        /// Gets the position of indicator. Set by internal code only
        ///</summary>
        public ChartPanelMoreIndicatorPosition Position { get; internal set; }

        ///<summary>
        /// Gets or sets the visibility of control
        ///</summary>
        public bool Visible
        {
            get { return Visibility == Visibility.Visible; }
            set { Visibility = value ? Visibility.Visible : Visibility.Collapsed; }
        }

        ///<summary>
        /// Gets or sets the left position
        ///</summary>
        public double Left
        {
            get { return Canvas.GetLeft(this); }
            set { Canvas.SetLeft(this, value); }
        }

        ///<summary>
        /// Gets or sets the Top position
        ///</summary>
        public double Top
        {
            get { return Canvas.GetTop(this); }
            set { Canvas.SetTop(this, value); }
        }
    }
}
