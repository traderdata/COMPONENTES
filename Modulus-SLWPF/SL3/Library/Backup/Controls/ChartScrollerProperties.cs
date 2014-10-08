using System.Windows;
using System.Windows.Media;

namespace ModulusFE.Controls
{
    /// <summary>
    /// Properties for chart scroller
    /// </summary>
    public class ChartScrollerProperties : DependencyObject
    {
        /// <summary>
        /// Gets the reference to the chart holding the scroller
        /// </summary>
        public StockChartX Chart { get; internal set; }

        /// <summary>
        /// Creates default properties for chart scroller
        /// </summary>
        /// <param name="chart"></param>
        /// <returns></returns>
        public static ChartScrollerProperties CreateDefault(StockChartX chart)
        {
            return new ChartScrollerProperties
                     {
                         Chart = chart,
                         Background = new SolidColorBrush(Color.FromArgb(0xFF, 0x28, 0x28, 0x28)),
                         ThumbBackground = new SolidColorBrush(Color.FromArgb(0xFF, 0x00, 0x00, 0xFF)), // SkyBlue
                         HandleBackground = new SolidColorBrush(Color.FromArgb(0xFF, 0xA9, 0xA9, 0xA9)), // Silver
                         HandleBackgroundProhibited = new SolidColorBrush(Color.FromArgb(0xFF, 0xA0, 0x52, 0x2D)),
                         TrendBackground = new SolidColorBrush(Color.FromArgb(0xFF, 0x63, 0x63, 0x63)),
                         HandleStroke = new SolidColorBrush(Color.FromArgb(0xFF, 0x82, 0x82, 0x82))
                     };
        }

        #region HeightProperty (DependencyProperty)

        /// <summary>
        /// </summary>
        public double Height
        {
            get { return (double)GetValue(HeightProperty); }
            set { SetValue(HeightProperty, value); }
        }

        /// <summary>
        /// Height
        /// </summary>
        public static readonly DependencyProperty HeightProperty =
          DependencyProperty.Register("Height", typeof(double), typeof(ChartScrollerProperties),
                                      new PropertyMetadata(50.0, OnHeightChanged));

        private static void OnHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ChartScrollerProperties)d).OnHeightChanged(e);
        }

        /// <summary>
        /// Method called when <see cref="Height"/> property changes
        /// </summary>
        /// <param name="e">Arguments</param>
        protected virtual void OnHeightChanged(DependencyPropertyChangedEventArgs e)
        {
            if (Chart != null)
            {
                Chart.UpdateScrollerVisual();
                Chart.Update();
            }
        }

        #endregion

        #region IsVisibleProperty (DependencyProperty)

        /// <summary>
        /// </summary>
        public bool IsVisible
        {
            get { return (bool)GetValue(IsVisibleProperty); }
            set { SetValue(IsVisibleProperty, value); }
        }

        /// <summary>
        /// IsVisible
        /// </summary>
        public static readonly DependencyProperty IsVisibleProperty =
          DependencyProperty.Register("IsVisible", typeof(bool), typeof(ChartScrollerProperties),
                                      new PropertyMetadata(true, OnIsVisibleChanged));

        private static void OnIsVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ChartScrollerProperties)d).OnIsVisibleChanged(e);
        }

        /// <summary>
        /// Method called when <see cref="IsVisible"/> property changes
        /// </summary>
        /// <param name="e">Arguments</param>
        protected virtual void OnIsVisibleChanged(DependencyPropertyChangedEventArgs e)
        {
            if (Chart != null)
            {
                Chart.Update();
            }
        }

        #endregion

        #region TrendBackgroundProperty (DependencyProperty)

        /// <summary>
        /// </summary>
        public Brush TrendBackground
        {
            get { return (Brush)GetValue(TrendBackgroundProperty); }
            set { SetValue(TrendBackgroundProperty, value); }
        }

        /// <summary>
        /// TrendBackground
        /// </summary>
        public static readonly DependencyProperty TrendBackgroundProperty =
          DependencyProperty.Register("TrendBackground", typeof(Brush), typeof(ChartScrollerProperties),
                                      new PropertyMetadata(null, OnTrendBackgroundChanged));

        private static void OnTrendBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ChartScrollerProperties)d).OnTrendBackgroundChanged(e);
        }

        /// <summary>
        /// Method called when <see cref="TrendBackground"/> property changes
        /// </summary>
        /// <param name="e">Arguments</param>
        protected virtual void OnTrendBackgroundChanged(DependencyPropertyChangedEventArgs e)
        {
            Chart.UpdateScrollerVisual();
        }

        #endregion

        #region TrendStrokeProperty (DependencyProperty)

        /// <summary>
        /// </summary>
        public Brush TrendStroke
        {
            get { return (Brush)GetValue(TrendStrokeProperty); }
            set { SetValue(TrendStrokeProperty, value); }
        }

        /// <summary>
        /// TrendStroke
        /// </summary>
        public static readonly DependencyProperty TrendStrokeProperty =
          DependencyProperty.Register("TrendStroke", typeof(Brush), typeof(ChartScrollerProperties),
                                      new PropertyMetadata(null, OnTrendStrokeChanged));

        private static void OnTrendStrokeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ChartScrollerProperties)d).OnTrendStrokeChanged(e);
        }

        /// <summary>
        /// Method called when <see cref="TrendStroke"/> property changes
        /// </summary>
        /// <param name="e">Arguments</param>
        protected virtual void OnTrendStrokeChanged(DependencyPropertyChangedEventArgs e)
        {
            Chart.UpdateScrollerVisual();
        }

        #endregion

        #region ThumbBackgroundProperty (DependencyProperty)

        /// <summary>
        /// </summary>
        public Brush ThumbBackground
        {
            get { return (Brush)GetValue(ThumbBackgroundProperty); }
            set { SetValue(ThumbBackgroundProperty, value); }
        }

        /// <summary>
        /// ThumbBackground
        /// </summary>
        public static readonly DependencyProperty ThumbBackgroundProperty =
          DependencyProperty.Register("ThumbBackground", typeof(Brush), typeof(ChartScrollerProperties),
                                      new PropertyMetadata(null, OnThumbBackgroundChanged));

        private static void OnThumbBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ChartScrollerProperties)d).OnThumbBackgroundChanged(e);
        }

        /// <summary>
        /// Method called when <see cref="ThumbBackground"/> property changes
        /// </summary>
        /// <param name="e">Arguments</param>
        protected virtual void OnThumbBackgroundChanged(DependencyPropertyChangedEventArgs e)
        {
            Chart.UpdateScrollerVisual();
        }

        #endregion

        #region ThumbStrokeProperty (DependencyProperty)

        /// <summary>
        /// </summary>
        public Brush ThumbStroke
        {
            get { return (Brush)GetValue(ThumbStrokeProperty); }
            set { SetValue(ThumbStrokeProperty, value); }
        }

        /// <summary>
        /// ThumbStroke
        /// </summary>
        public static readonly DependencyProperty ThumbStrokeProperty =
          DependencyProperty.Register("ThumbStroke", typeof(Brush), typeof(ChartScrollerProperties),
                                      new PropertyMetadata(null, OnThumbStrokeChanged));

        private static void OnThumbStrokeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ChartScrollerProperties)d).OnThumbStrokeChanged(e);
        }

        /// <summary>
        /// Method called when <see cref="ThumbStroke"/> property changes
        /// </summary>
        /// <param name="e">Arguments</param>
        protected virtual void OnThumbStrokeChanged(DependencyPropertyChangedEventArgs e)
        {
            Chart.UpdateScrollerVisual();
        }

        #endregion

        #region BackgroundProperty (DependencyProperty)

        /// <summary>
        /// </summary>
        public Brush Background
        {
            get { return (Brush)GetValue(BackgroundProperty); }
            set { SetValue(BackgroundProperty, value); }
        }

        /// <summary>
        /// Background
        /// </summary>
        public static readonly DependencyProperty BackgroundProperty =
          DependencyProperty.Register("Background", typeof(Brush), typeof(ChartScrollerProperties),
                                      new PropertyMetadata(null, OnBackgroundChanged));

        private static void OnBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ChartScrollerProperties)d).OnBackgroundChanged(e);
        }

        /// <summary>
        /// Method called when <see cref="Background"/> property changes
        /// </summary>
        /// <param name="e">Arguments</param>
        protected virtual void OnBackgroundChanged(DependencyPropertyChangedEventArgs e)
        {
            Chart.UpdateScrollerVisual();
        }

        #endregion

        #region HandleBackgroundProperty

        /// <summary>
        /// Identifies the <see cref="HandleBackground"/> dependency property 
        /// </summary>
        public static readonly DependencyProperty HandleBackgroundProperty =
          DependencyProperty.Register("HandleBackground", typeof(Brush), typeof(ChartScrollerProperties),
                                      new PropertyMetadata(null, (o, args) => ((ChartScrollerProperties)o).HandleBackgroundChanged(args)));

        /// <summary>
        /// Gets or sets the background for handles when zooming is allowed
        /// </summary>
        public Brush HandleBackground
        {
            get { return (Brush)GetValue(HandleBackgroundProperty); }
            set { SetValue(HandleBackgroundProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        protected void HandleBackgroundChanged(DependencyPropertyChangedEventArgs args)
        {
            Chart.UpdateScrollerVisual();
        }

        #endregion

        #region HandleBackgroundProhibitedProperty

        /// <summary>
        /// Identifies the <see cref="HandleBackgroundProhibited"/> dependency property 
        /// </summary>
        public static readonly DependencyProperty HandleBackgroundProhibitedProperty =
          DependencyProperty.Register("HandleBackgroundProhibited", typeof(Brush), typeof(ChartScrollerProperties),
                                      new PropertyMetadata(null, (o, args) => ((ChartScrollerProperties)o).HandleBackgroundProhibitedChanged(args)));

        /// <summary>
        /// Gets or sets the background for handles when zooming is prohibited. This happens in case when
        /// number if visible records in chart is greater than <see cref="StockChartX.MaxVisibleRecords"/>
        /// </summary>
        public Brush HandleBackgroundProhibited
        {
            get { return (Brush)GetValue(HandleBackgroundProhibitedProperty); }
            set { SetValue(HandleBackgroundProhibitedProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        protected void HandleBackgroundProhibitedChanged(DependencyPropertyChangedEventArgs args)
        {
            Chart.UpdateScrollerVisual();
        }

        #endregion

        #region HandleStrokeProperty

        /// <summary>
        /// Identifies the <see cref="HandleStroke"/> dependency property 
        /// </summary>
        public static readonly DependencyProperty HandleStrokeProperty =
          DependencyProperty.Register("HandleStroke", typeof(Brush), typeof(ChartScrollerProperties),
                                      new PropertyMetadata(null, (o, args) => ((ChartScrollerProperties)o).HandleStrokeChanged(args)));

        /// <summary>
        /// Gets or sets the stroke for handles. This is a  dependency property.
        /// </summary>
        public Brush HandleStroke
        {
            get { return (Brush)GetValue(HandleStrokeProperty); }
            set { SetValue(HandleStrokeProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        protected void HandleStrokeChanged(DependencyPropertyChangedEventArgs args)
        {
            Chart.UpdateScrollerVisual();
        }

        #endregion
    }
}
