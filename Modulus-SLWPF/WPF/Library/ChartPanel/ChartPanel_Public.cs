using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ModulusFE.Indicators;
using ModulusFE.LineStudies;
#if SILVERLIGHT
using ModulusFE.SL.Utils;
#endif

namespace ModulusFE
{
    public partial class ChartPanel
    {
        #region Public Methods
        /// <summary>
        /// Normalizes a value between 1 and 0
        /// </summary>
        /// <param name="value">Value to be normalized</param>
        /// <returns>Normalized value</returns>
        public double Normalize(double value)
        {
            
            if (_scalingType == ScalingTypeEnum.Semilog && _minChanged > 0 && _hasPrice)
                return (value - SLMin) / (SLMax - SLMin);

            return (value - _minChanged) / (_maxChanged - _minChanged);
        }

        /// <summary>
        /// Unscales a value and restores between max and min
        /// </summary>
        /// <param name="value">Value to be unnormalized</param>
        /// <returns>UnNormalized value</returns>
        public double UnNormalize(double value)
        {
            //Alterado por Felipe - 09-09-2014
            if (_scalingType == ScalingTypeEnum.Semilog && _minChanged > 0 && _hasPrice)
                return SLMin + (value * (SLMax - SLMin));

            return _minChanged + (value * (_maxChanged - _minChanged));
        }

        /// <summary>
        /// Returns Y pixel value by value from a series
        /// </summary>
        /// <param name="seriesValue">Price value</param>
        /// <returns>Y pixel</returns>
        public double GetY(double seriesValue)
        {
            double realHeight = PaintableHeight;
            if (realHeight == 0)
                return 0.0;

            if (_scalingType == ScalingTypeEnum.Semilog && _minChanged > 0 && _hasPrice)
                return (realHeight - (realHeight * Normalize(Math.Log10(seriesValue)))) + _yOffset;

            return (realHeight - (realHeight * Normalize(seriesValue))) + _yOffset;
        }

        /// <summary>
        /// Gets the paitable height of the panel or zero if template was not loaded yet.
        /// </summary>
        public double PaintableHeight
        {
            get { return !_templateLoaded ? 0 : _rootCanvas.ActualHeight; }
        }

        /// <summary>
        /// Returns series value by Y pixel
        /// </summary>
        /// <param name="pixelValue">Pixel value</param>
        /// <returns>Price value</returns>
        public double GetReverseY(double pixelValue)
        {
            double realHeight = _rootCanvas.ActualHeight;

            if (_scalingType == ScalingTypeEnum.Semilog && Min > 0 && _hasPrice)
            {
                pixelValue = UnNormalize(1 - (pixelValue - _yOffset) / realHeight);
                if (pixelValue > 0 && Max > 0)
                    return Math.Pow(10, pixelValue);
            }

            return UnNormalize(1 - (pixelValue - _yOffset) / realHeight);
        }

        /// <summary>
        /// Returns OHLC group of series from current panel
        /// Returns false if the group does not exist
        /// </summary>
        /// <param name="open">Reference to open series</param>
        /// <param name="high">Reference to high series</param>
        /// <param name="low">Reference to low series</param>
        /// <param name="close">Reference to close series</param>
        /// <returns>true - if OHLC group exists
        /// false - if OHLC doesn't exists</returns>
        public bool GetOHLCSeries(out Series open, out Series high,
          out Series low, out Series close)
        {
            open = high = low = close = null;
            if (_series.Count < 4) return false;
            open = _series.FirstOrDefault(_ => _.OHLCType == SeriesTypeOHLC.Open);
            if (open == null) return false;
            high = GetSeriesOHLCV(open, SeriesTypeOHLC.High);
            if (high == null) return false;
            low = GetSeriesOHLCV(open, SeriesTypeOHLC.Low);
            if (low == null) return false;
            close = GetSeriesOHLCV(open, SeriesTypeOHLC.Close);
            return close != null;
        }

        ///<summary>
        /// Resets the Y scale
        ///</summary>
        public void ResetYScale()
        {
            _staticYScale = false;
            _enforceSeriesSetMinMax = true;
            _yOffset = 0;
            Paint();
        }

        /// <summary>
        /// Minimize the panel will only work if the panel is currently in the 'Normal' state
        /// </summary>
        public void Minimize()
        {
            if (_state == StateType.Normal)
                _panelsContainer.MinimizePanel(this);
        }

        /// <summary>
        /// Maximise the panel will only work if the panel is currently in the 'Normal' state
        /// </summary>
        public void Maximize()
        {
            if (_state == StateType.Normal)
                _panelsContainer.MaxMinPanel(this);
        }

        /// <summary>
        /// Return a minimized or maximized panel to the normal state.
        /// </summary>
        public void Restore()
        {
            if (_state == StateType.Normal) return;
            if (_state == StateType.Maximized) _panelsContainer.MaxMinPanel(this);
            if (_state == StateType.Minimized) _panelsContainer.RestorePanel(this);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the Y value from current mouse position
        /// </summary>
        public double CurrentPrice
        {
            get { return GetReverseY(Mouse.GetPosition(_rootCanvas).Y); }
        }

        /// <summary>
        /// Gets the title of the panel. The title is composed from names all all series &amp; indicators
        /// located on current panel
        /// </summary>
        public string Title
        {
            get
            {
                StringBuilder sb = new StringBuilder(128);
                foreach (SeriesTitleLabel label in _seriesTitle)
                {
                    sb.Append(">").Append(label.Title).Append(" ");
                }
                return sb.ToString();
            }
        }

        /// <summary>
        /// Gets or sets the visibility of the panel
        /// </summary>
        public bool Visible
        {
            get { return Visibility == Visibility.Visible; }
            internal set
            {
                if (value == Visible || _state == StateType.Minimized)
                {
                    return;
                }

                Visibility = value ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Gets the height of the panel's title bar
        /// </summary>
        public double TitleBarHeight
        {
            get { return _titleBar.ActualHeight; }
        }

        /// <summary>
        /// Gets a collection of all indicators from current panel
        /// </summary>
        public IEnumerable<Indicator> IndicatorsCollection
        {
            get
            {
                return _series.
                  Where(series => series._seriesType == SeriesTypeEnum.stIndicator && !series._recycleFlag)
                  .Cast<Indicator>();
            }
        }

        /// <summary>
        /// Gets a collection of all series from panel
        /// </summary>
        public IEnumerable<Series> SeriesCollection
        {
            get
            {
                return _series
                  .Where(series => series._seriesType != SeriesTypeEnum.stIndicator && series._seriesType != SeriesTypeEnum.stUnknown);
            }
        }

        /// <summary>
        /// Gets index of the panel
        /// </summary>
        public int Index
        {
            get { return _index; }
        }

        /// <summary>
        /// Gets minimum value from all series located in current panel
        /// </summary>
        public double Min
        {
            get { return _minChanged; }
        }

        /// <summary>
        /// Gets maximum value from all series located in current panel
        /// </summary>
        public double Max
        {
            get { return _maxChanged; }
        }

        /// <summary>
        /// Gets minimum-logarithmic value from all series located in current panel
        /// </summary>
        public double SLMin
        {
            get { return Math.Log10(_minChanged); }
        }

        /// <summary>
        /// Gets maximum-logarithmic value from all series located in current panel
        /// </summary>
        public double SLMax
        {
            get { return Math.Log10(_maxChanged); }
        }

        /// <summary>
        /// Gets if panel has staic Y scale. 
        /// static Y scale set to true means that min &amp; max values from all series
        /// will be ignored, instead users' min &amp; max values will be used to paint series.
        /// </summary>
        public bool StaticYScale
        {
            get { return _staticYScale; }
        }

        ///<summary>
        /// Gets or sets the number of decimal digits used to paint the Y values. If set to null then
        /// the number of decimals are set to 0 if panel has a volume series, or number of decimals
        /// is taken from <see cref="StockChartX.ScalePrecision"/>
        ///</summary>
        public int? YAxisScalePrecision
        {
            get;
            set;
        }

        /// <summary>
        /// Gets formating string that will be used to format values on Y axis
        /// </summary>
        public string FormatYValueString
        {
            get
            {
                int decimals = YAxisScalePrecision.HasValue
                  ? YAxisScalePrecision.Value
                  : (_hasVolume ? 0 : _chartX.ScalePrecision);
                return "{0:f" + decimals + "}";
            }
        }

        /// <summary>
        /// Gets if current panel has an OHLC group of series
        /// </summary>
        public bool HasOHLC
        {
            get
            {
                if (_series.Count < 4) return false;

                Series series = _series.FirstOrDefault(_ => _.OHLCType == SeriesTypeOHLC.Close);
                if (series == null)
                {
                    return false;
                }

                series = GetSeriesOHLCV(series, SeriesTypeOHLC.Open);
                if (series == null) return false;
                series = GetSeriesOHLCV(series, SeriesTypeOHLC.High);
                if (series == null) return false;
                series = GetSeriesOHLCV(series, SeriesTypeOHLC.Low);
                if (series == null) return false;
                series = GetSeriesOHLCV(series, SeriesTypeOHLC.Close);

                return series != null;
            }
        }

        /// <summary>
        /// Gets true if current panel is a heat-map
        /// </summary>
        public bool IsHeatMap
        {
            get { return _isHeatMap; }
        }

        ///<summary>
        /// Gets the collection of LineStudies from current panel
        ///</summary>
        public IEnumerable<LineStudy> LineStudiesCollection
        {
            get { return _lineStudies; }
        }

        /// <summary>
        /// Gets or sets the background of Y axis
        /// </summary>
        public static DependencyProperty YAxesBackgroundProperty;
        /// <summary>
        /// Gets or sets the background of Y axis
        /// </summary>
        public Brush YAxesBackground
        {
            get { return (Brush)GetValue(YAxesBackgroundProperty); }
            set { SetValue(YAxesBackgroundProperty, value); }
        }

        ///<summary>
        /// Gets or sets the control that will show the elements when user clicks on more indicator
        ///</summary>
        public Interfaces.IChartPanelMoreIndicatorPanel MoreIndicatorPanel { get; set; }

        ///<summary>
        /// Gets the current Y scale step
        ///</summary>
        public double CurrentYScaleStep { get; internal set; }


        private static void OnYAxesBackgroundChanged(DependencyObject sender, DependencyPropertyChangedEventArgs eventArgs)
        {
            ChartPanel chartPanel = (ChartPanel)sender;
            if (chartPanel == null)
            {
                return;
            }

            if (chartPanel._leftYAxis != null)
            {
                chartPanel._leftYAxis.Background = (Brush)eventArgs.NewValue;
            }

            if (chartPanel._rightYAxis != null)
            {
                chartPanel._rightYAxis.Background = (Brush)eventArgs.NewValue;
            }
        }

        /// <summary>
        /// Gets or sets the post text for each value in Y axis
        /// </summary>
        public string YAxisPostFix { get; set; }
        #endregion
    }
}

