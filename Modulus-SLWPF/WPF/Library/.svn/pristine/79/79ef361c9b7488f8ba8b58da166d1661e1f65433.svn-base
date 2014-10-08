using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using ModulusFE.ChartElementProperties;

namespace ModulusFE
{
    /// <summary>
    /// internal usage
    /// </summary>
    public partial class SeriesTitleLabel : DependencyObject
    {
        private readonly Series _series;

        /// <summary>
        /// internal usage
        /// </summary>
        public SeriesTitleLabel(Series series)
        {
            _series = series;
            _series.PropertyChanged += Series_OnPropertyChanged;
            _series._chartPanel._chartX.PropertyChanged += ChartX_OnPropertyChanged;

            SetTitle();
            SetSeriesStroke();
        }

        internal void UnSubscribe()
        {
            _series.PropertyChanged -= Series_OnPropertyChanged;
            _series._chartPanel._chartX.PropertyChanged -= ChartX_OnPropertyChanged;
        }

        internal Series Series
        {
            get { return _series; }
        }

        private void ChartX_OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == StockChartX.Property_EndIndex)
            {
                SetTitle();
            }
        }

        private void Series_OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case Series.PropertyStrokeBrush:
                case Series.PropertyTitleBrush:
                    SetSeriesStroke();
                    break;
                case Series.PropertyLastValue:
                case Series.PropertyLastTick:
                case Series.PropertySeriesIndex:
                    SetTitle();
                    break;
            }
        }

        internal void SetTitle()
        {
            Title = _series.Title + " = " +
                        string.Format(_series._chartPanel.FormatYValueString,
                                      _series.DM.LastVisibleDataEntry(_series.SeriesIndex, true).Value);
        }

        //Description - Dependency Property with changed event and  no validation

        #region TitleProperty

        ///<summary>
        ///</summary>
        public static readonly DependencyProperty TitleProperty =
          DependencyProperty.Register("Title", typeof(string), typeof(SeriesTitleLabel),
                                      new PropertyMetadata(string.Empty, (o, args) => ((SeriesTitleLabel)o).TitleChanged(args)));

        ///<summary>
        /// Gets the title displayed
        ///</summary>
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            private set { SetValue(TitleProperty, value); }
        }

        /// <summary>
        /// Title Changed event
        /// </summary>
        /// <param name="args"></param>
        protected void TitleChanged(DependencyPropertyChangedEventArgs args)
        {

        }

        #endregion

        //Description - Dependency Property with changed event and  no validation

        #region SeriesStrokeProperty

        ///<summary>
        ///</summary>
        public static readonly DependencyProperty SeriesStrokeProperty =
          DependencyProperty.Register("SeriesStroke", typeof(Brush), typeof(SeriesTitleLabel),
                                      new PropertyMetadata(null));

        ///<summary>
        /// Gets the series stroke brush
        ///</summary>
        public Brush SeriesStroke
        {
            get { return (Brush)GetValue(SeriesStrokeProperty); }
            private set { SetValue(SeriesStrokeProperty, value); }
        }

        #endregion

        private void SetSeriesStroke()
        {
            SeriesStroke = _series.TitleBrush ?? _series.StrokeColorBrush;
        }

        ///<summary>
        /// Gets whether to show the frame arround title
        ///</summary>
        public Visibility ShowFrame
        {
            get
            {
                return _series is IChartElementPropertyAble ? Visibility.Visible : Visibility.Collapsed;
            }
        }
    }
}
