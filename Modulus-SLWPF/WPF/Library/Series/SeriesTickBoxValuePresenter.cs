using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using ModulusFE.Data;
using ModulusFE.Interfaces;

namespace ModulusFE
{
    ///<summary>
    ///</summary>
    public partial class SeriesTickBoxValuePresenter : ContentControl
    {
        private readonly Series _series;
        private double localMin = double.MaxValue;
        private double localMax = double.MinValue;

#if WPF
    static SeriesTickBoxValuePresenter()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(SeriesTickBoxValuePresenter),
                                               new FrameworkPropertyMetadata(typeof(SeriesTickBoxValuePresenter)));
    }
#endif
        ///<summary>
        /// ctor
        ///</summary>
        public SeriesTickBoxValuePresenter()
        {
#if SILVERLIGHT
            DefaultStyleKey = typeof(SeriesTickBoxValuePresenter);
#endif
        }

        private IValueBridge<Series> ValueBridge
        {
            get { return (IValueBridge<Series>)DataContext; }
        }

        ///<summary>
        ///</summary>
        ///<param name="series"></param>
        public SeriesTickBoxValuePresenter(Series series)
        {
#if SILVERLIGHT
            DefaultStyleKey = typeof(SeriesTickBoxValuePresenter);
#endif

            _series = series;
            _series.PropertyChanged += Series_OnPropertyChanged;
            StockChartX chartX = _series._chartPanel._chartX;
            chartX.PropertyChanged += ChartX_OnPropertyChanged;
            _series._chartPanel.PropertyChanged += ChartPanelOnPropertyChanged;

            IsHitTestVisible = false;
        }

        internal void UnSubscribe()
        {
            _series.PropertyChanged -= Series_OnPropertyChanged;
            StockChartX chartX = _series._chartPanel._chartX;
            chartX.PropertyChanged -= ChartX_OnPropertyChanged;
            _series._chartPanel.PropertyChanged -= ChartPanelOnPropertyChanged;
        }

        private void ChartPanelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == ChartPanel.Property_YAxisMoved.PropertyName ||
              e.PropertyName == ChartPanel.Property_YAxisResized.PropertyName ||
              e.PropertyName == ChartPanel.Property_PanelResized.PropertyName)
            {
                Show();
            }

            // Note to other developers...
            // This is not the best solution for this, but it gets a job done. When the y-axis scale changes the event
            // was not being fired properly. The issue is that the MinMaxChanged property actually gets fired even when the values do
            // not actually change in value. Therefore, we store a local value and only react when the values actually change.
            // It might be better to fix when the value is firing, but that is a level of complexity beyond a bug fix...
            if (e.PropertyName == ChartPanel.Property_MinMaxChanged.PropertyName
                && (_series._chartPanel.Min != localMin || _series._chartPanel.Max != localMax))
            {
                localMin = _series._chartPanel.Min;
                localMax = _series._chartPanel.Max;
                Show();
            }
        }

        private void ChartX_OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == StockChartX.Property_EndIndex ||
              e.PropertyName == StockChartX.Property_Zoomed)
            {
                Show();
            }
        }

        private void Series_OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (Visibility != Visibility.Visible)
                return;

            switch (e.PropertyName)
            {
                case Series.PropertyLastValue:
                case Series.PropertyLastTick:
                    Show();
                    break;
            }
        }

        internal void Show()
        {
            if (Visibility != Visibility.Visible)
                return;

            DataEntry dataEntry = _series.DM.LastVisibleDataEntry(_series.SeriesIndex);
            if (dataEntry == null || !dataEntry.Value.HasValue)
                return;

            double y = _series.GetY(dataEntry.Value.Value);

            if (y < 0)
                y = 0;

            Canvas.SetTop(this, y - ActualHeight / 2);
            Canvas.SetLeft(this, 0);
            Canvas.SetZIndex(this, ZIndexConstants.SelectionPoint2);

            ValueBridge.NotifyDataChanged(dataEntry.Value.Value);
        }
    }
}

