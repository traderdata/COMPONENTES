using System.Windows;

namespace ModulusFE
{
    public partial class ChartPanel
    {
        ///<summary>
        /// Gets or sets a value indicating whether the Maximize button is displayed in the caption bar of the panel.
        ///</summary>
        public static DependencyProperty MaximizeBoxProperty =
          DependencyProperty.Register("MaximizeBox", typeof(bool), typeof(ChartPanel),
                                      new PropertyMetadata(true, MaximizeBoxPropertyChangedCallback));
        ///<summary>
        /// Gets or sets a value indicating whether the Maximize button is displayed in the caption bar of the panel.
        ///</summary>
        public bool MaximizeBox
        {
            get { return (bool)GetValue(MaximizeBoxProperty); }
            set { SetValue(MaximizeBoxProperty, value); }
        }

        private static void MaximizeBoxPropertyChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs args)
        {
            ChartPanel chartPanel = (ChartPanel)o;
            if (chartPanel._titleBar != null)
                chartPanel._titleBar.MaximizeBox = (bool)args.NewValue;
        }

        ///<summary>
        /// Gets or sets a value indicating whether the Minimize button is displayed in the caption bar of the panel.
        ///</summary>
        public static DependencyProperty MinimizeBoxProperty =
          DependencyProperty.Register("MinimizeBox", typeof(bool), typeof(ChartPanel),
                                      new PropertyMetadata(true, MinimizeBoxPropertyChangedCallback));
        ///<summary>
        /// Gets or sets a value indicating whether the Minimize button is displayed in the caption bar of the panel.
        ///</summary>
        public bool MinimizeBox
        {
            get { return (bool)GetValue(MinimizeBoxProperty); }
            set { SetValue(MinimizeBoxProperty, value); }
        }

        private static void MinimizeBoxPropertyChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs args)
        {
            ChartPanel chartPanel = (ChartPanel)o;
            if (chartPanel._titleBar != null)
                chartPanel._titleBar.MinimizeBox = (bool)args.NewValue;
        }

        ///<summary>
        /// Gets or sets a value indicating whether the Close button is displayed in the caption bar of the panel.
        ///</summary>
        public static DependencyProperty CloseBoxProperty =
          DependencyProperty.Register("CloseBox", typeof(bool), typeof(ChartPanel),
                                      new PropertyMetadata(true, CloseBoxPropertyChangedCallback));
        ///<summary>
        /// Gets or sets a value indicating whether the Close button is displayed in the caption bar of the panel.
        ///</summary>
        public bool CloseBox
        {
            get { return (bool)GetValue(CloseBoxProperty); }
            set { SetValue(CloseBoxProperty, value); }
        }

        private static void CloseBoxPropertyChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs args)
        {
            ChartPanel chartPanel = (ChartPanel)o;
            if (chartPanel._titleBar != null)
                chartPanel._titleBar.CloseBox = (bool)args.NewValue;
        }
    }
}