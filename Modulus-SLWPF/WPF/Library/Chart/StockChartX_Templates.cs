using System.Windows;
using System.Windows.Controls;

namespace ModulusFE
{
    /// <summary>
    /// Represents the chart. It works as a container for all panels.
    /// </summary>
    public partial class StockChartX
    {
        #region HorizontalLineValuePresenterTemplateProperty (DependencyProperty)

        /// <summary>
        /// HorizontalLineValuePresenterTemplate summary
        /// </summary>
        public ControlTemplate HorizontalLineValuePresenterTemplate
        {
            get { return (ControlTemplate)GetValue(HorizontalLineValuePresenterTemplateProperty); }
            set { SetValue(HorizontalLineValuePresenterTemplateProperty, value); }
        }

        /// <summary>
        /// HorizontalLineValuePresenterTemplate
        /// </summary>
        public static readonly DependencyProperty HorizontalLineValuePresenterTemplateProperty =
          DependencyProperty.Register("HorizontalLineValuePresenterTemplate", typeof(ControlTemplate), typeof(StockChartX),
                                      new PropertyMetadata(null, OnHorizontalLineValuePresenterControlTemplateChanged));

        private static void OnHorizontalLineValuePresenterControlTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((StockChartX)d).OnHorizontalLineValuePresenterControlTemplateChanged(e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnHorizontalLineValuePresenterControlTemplateChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        #endregion

        #region SeriesTickBoxValuePresenterTemplateProperty (DependencyProperty)

        /// <summary>
        /// Template for the tick box that are shown on Y axis for any series
        /// </summary>
        public ControlTemplate SeriesTickBoxValuePresenterTemplate
        {
            get { return (ControlTemplate)GetValue(SeriesTickBoxValuePresenterTemplateProperty); }
            set { SetValue(SeriesTickBoxValuePresenterTemplateProperty, value); }
        }

        /// <summary>
        /// SeriesTickBoxValuePresenterTemplate
        /// </summary>
        public static readonly DependencyProperty SeriesTickBoxValuePresenterTemplateProperty =
          DependencyProperty.Register("SeriesTickBoxValuePresenterTemplate", typeof(ControlTemplate), typeof(StockChartX),
                                      new PropertyMetadata(null, OnSeriesTickBoxValuePresenterTemplateChanged));

        private static void OnSeriesTickBoxValuePresenterTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((StockChartX)d).OnSeriesTickBoxValuePresenterTemplateChanged(e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnSeriesTickBoxValuePresenterTemplateChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        #endregion

        #region ChartPanelMoreIndicatorTemplateProperty (DependencyProperty)

        /// <summary>
        /// ChartPanelMoreIndicatorTemplate summary
        /// </summary>
        public ControlTemplate ChartPanelMoreIndicatorTemplate
        {
            get { return (ControlTemplate)GetValue(ChartPanelMoreIndicatorTemplateProperty); }
            set { SetValue(ChartPanelMoreIndicatorTemplateProperty, value); }
        }

        /// <summary>
        /// ChartPanelMoreIndicatorTemplate
        /// </summary>
        public static readonly DependencyProperty ChartPanelMoreIndicatorTemplateProperty =
          DependencyProperty.Register("ChartPanelMoreIndicatorTemplate", typeof(ControlTemplate), typeof(StockChartX),
                                      new PropertyMetadata(null, OnChartPanelMoreIndicatorTemplateChanged));

        private static void OnChartPanelMoreIndicatorTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((StockChartX)d).OnChartPanelMoreIndicatorTemplateChanged(e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnChartPanelMoreIndicatorTemplateChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        #endregion

        #region AddHolderProperty (DependencyProperty)

        /// <summary>
        /// AddHolder summary
        /// </summary>
        public object AddHolder
        {
            get { return (object)GetValue(AddHolderProperty); }
            set { SetValue(AddHolderProperty, value); }
        }

        /// <summary>
        /// AddHolder
        /// </summary>
        public static readonly DependencyProperty AddHolderProperty =
          DependencyProperty.Register("AddHolder", typeof(object), typeof(StockChartX),
                                      new PropertyMetadata(null, OnAddHolderChanged));

        private static void OnAddHolderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((StockChartX)d).OnAddHolderChanged(e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnAddHolderChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        #endregion

        #region AddHolderTemplateProperty (DependencyProperty)

        /// <summary>
        /// AddHolderTemplate summary
        /// </summary>
        public DataTemplate AddHolderTemplate
        {
            get { return (DataTemplate)GetValue(AddHolderTemplateProperty); }
            set { SetValue(AddHolderTemplateProperty, value); }
        }

        /// <summary>
        /// AddHolderTemplate
        /// </summary>
        public static readonly DependencyProperty AddHolderTemplateProperty =
          DependencyProperty.Register("AddHolderTemplate", typeof(DataTemplate), typeof(StockChartX),
                                      new PropertyMetadata(null, OnAddHolderTemplateChanged));

        private static void OnAddHolderTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((StockChartX)d).OnAddHolderTemplateChanged(e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnAddHolderTemplateChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        #endregion

        #region AddHolderMarginProperty (DependencyProperty)

        /// <summary>
        /// AddHolderMargin summary
        /// </summary>
        public Thickness AddHolderMargin
        {
            get { return (Thickness)GetValue(AddHolderMarginProperty); }
            set { SetValue(AddHolderMarginProperty, value); }
        }

        /// <summary>
        /// AddHolderMargin
        /// </summary>
        public static readonly DependencyProperty AddHolderMarginProperty =
          DependencyProperty.Register("AddHolderMargin", typeof(Thickness), typeof(StockChartX),
                                      new PropertyMetadata(new Thickness(), OnAddHolderMarginChanged));

        private static void OnAddHolderMarginChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((StockChartX)d).OnAddHolderMarginChanged(e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnAddHolderMarginChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        #endregion

    }
}