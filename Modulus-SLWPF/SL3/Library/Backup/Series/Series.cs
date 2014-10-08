using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using ModulusFE.Data;
using ModulusFE.LineStudies;
using ModulusFE.PaintObjects;

namespace ModulusFE
{
    ///<summary>
    /// Base class for all series types used in the chart
    ///</summary>
    [CLSCompliant(true)]
    public partial class Series : INotifyPropertyChanged
    {
        internal const string PropertyStrokeBrush = "StrokeBrush";
        internal const string PropertyLastValue = "LastValue";
        internal const string PropertyTitleBrush = "TitleBrush";
        internal const string PropertyLastTick = "LastTick";
        internal const string PropertySeriesIndex = "SeriesIndex";

        internal ChartPanel _chartPanel;
        internal Color? _upColor;
        internal Color? _downColor;
        internal Color _strokeColor;
        internal double _strokeThickness;
        internal LinePattern _strokePattern;
        internal double _opacity = 1.0;
        internal List<Series> _linkedSeries = new List<Series>();
        internal bool _recycleFlag;
        internal SeriesTypeEnum _seriesType;
        internal bool _selectable;
        internal bool _shareScale;
        internal PaintObjectsManager<SelectionDot> _selectionDots = new PaintObjectsManager<SelectionDot>();

        internal string _title = string.Empty;
        internal SeriesTypeOHLC _seriesTypeOHLC = SeriesTypeOHLC.Unknown;
        internal string _name;
        internal bool _selected;
        internal bool _visible;

        /// <summary>
        /// has minimum value from all visible records. Set in ChartPanel.SetMaxMin()
        /// </summary>
        internal double _min;
        /// <summary>
        /// has maximum value from all visible records. Set in ChartPanel.SetMaxMin()
        /// </summary>  
        internal double _max;

        /// <summary>
        /// Keeps the index of the series in DataManager
        /// </summary>
        private int _seriesIndex;

        internal Series(string name, SeriesTypeEnum seriesType, SeriesTypeOHLC seriesTypeOHLC, ChartPanel chartPanel)
        {
            _name = name;
            _seriesType = seriesType;
            _chartPanel = chartPanel;
            _seriesTypeOHLC = seriesTypeOHLC;

            ShowInHeatMap = false;

            UseEnhancedColoring = false;
            WickUpStroke = new SolidColorBrush(Color.FromArgb(0xFF, 0x00, 0xFF, 0x00)); // Lime
            WickDownStroke = new SolidColorBrush(Colors.Red);
            WickStrokeThickness = 1.0;
            CandleUpFill = new SolidColorBrush(Color.FromArgb(0xFF, 0x00, 0xFF, 0x00)); // Lime
            CandleDownFill = new SolidColorBrush(Colors.Red);
            CandleUpStroke = null;
            CandleDownStroke = null;
            CandleStrokeThickness = 0;
        }

        #region Public properties
        ///<summary>
        /// Series Name
        ///</summary>
        public string Name
        {
            get { return _name; }
        }

        ///<summary>
        ///</summary>
        public bool ShareScale
        {
            get { return _shareScale; }
            set { _shareScale = value; }
        }

        /// <summary>
        /// Gets or sets whether the series will be used in heat map.
        /// By default only indicators are shown in heat map.
        /// </summary>
        public bool ShowInHeatMap { get; set; }

        /// <summary>
        /// Gets the reference to the panel where series is hosted.
        /// </summary>
        public ChartPanel Panel
        {
            get { return _chartPanel; }
        }

        /// <summary>
        /// Includes Name + OHLCV postfix
        /// </summary>
        public string FullName
        {
            get
            {
                if (_seriesType == SeriesTypeEnum.stIndicator) return Name;
                switch (_seriesTypeOHLC)
                {
                    case SeriesTypeOHLC.Open:
                        return Name + ".open";
                    case SeriesTypeOHLC.High:
                        return Name + ".high";
                    case SeriesTypeOHLC.Low:
                        return Name + ".low";
                    case SeriesTypeOHLC.Close:
                        return Name + ".close";
                    case SeriesTypeOHLC.Volume:
                        return Name + ".volume";
                    default:
                        return Name;
                }
            }
        }

        ///<summary>
        /// Custom title if present. Otherwise FullName
        ///</summary>
        public string Title
        {
            get { return _title.Length == 0 ? FullName : _title; }
        }

        ///<summary>
        /// OHLC type of the series
        ///</summary>
        public SeriesTypeOHLC OHLCType
        {
            get { return _seriesTypeOHLC; }
        }

        /// <summary>
        /// Series Type
        /// </summary>
        public SeriesTypeEnum SeriesType
        {
            get { return _seriesType; }
            set { _seriesType = value; }
        }

        ///<summary>
        /// Sets the chart's up-tick bar color. When the close is higher than the previous close, this color will be used to paint the bar. 
        ///</summary>
        public Color? UpColor
        {
            get { return _upColor; }
            set
            {
                _upColor = value;
            }
        }

        ///<summary>
        /// Sets the chart's down-tick bar color. When the close is lower than the previous close, this color will be used to paint the bar. 
        ///</summary>
        public Color? DownColor
        {
            get { return _downColor; }
            set
            {
                _downColor = value;
            }
        }

        ///<summary>
        /// Line color
        ///</summary>
        public Color StrokeColor
        {
            get { return _strokeColor; }
            set
            {
                if (_strokeColor == value) return;
                _strokeColor = value;
                SetStrokeColor();
                InvokePropertyChanged(PropertyStrokeBrush);
            }
        }

        private Brush _titleBrush;
        ///<summary>
        /// Gets or sets the foreground for series' text displayed in panel's title bar
        ///</summary>
        public Brush TitleBrush
        {
            get { return _titleBrush; }
            set
            {
                if (_titleBrush == value) return;
                _titleBrush = value;
                InvokePropertyChanged(PropertyTitleBrush);
            }
        }

        ///<summary>
        /// Gets the line stroke brush
        ///</summary>
        public Brush StrokeColorBrush
        {
            get { return new SolidColorBrush(_strokeColor); }
        }

        /// <summary>
        /// Gets or sets the Brush for up-trend wick
        /// </summary>
        public Brush WickUpStroke { get; set; }

        /// <summary>
        /// Gets or sets the Brush for down-trend wick
        /// </summary>
        public Brush WickDownStroke { get; set; }

        ///<summary>
        /// Gets or sets the stroke thickness for wick
        ///</summary>
        public double WickStrokeThickness { get; set; }

        ///<summary>
        /// Gets or sets the Brush used to fill the up-trend of candles
        ///</summary>
        public Brush CandleUpFill { get; set; }

        ///<summary>
        /// Gets or sets the Brush used to fill the down-trend of candles
        ///</summary>
        public Brush CandleDownFill { get; set; }

        ///<summary>
        /// Gets or sets the stroke Brush for the up-trend of candles
        ///</summary>
        public Brush CandleUpStroke { get; set; }

        ///<summary>
        /// Gets or sets the stroke Brush for the down-trend of candles
        ///</summary>
        public Brush CandleDownStroke { get; set; }

        /// <summary>
        /// Gets or sets the stroke thickness for Candles
        /// </summary>
        public double CandleStrokeThickness { get; set; }

        ///<summary>
        /// Gets ir sets whether the chart needs to use the an enhanced coloring.
        /// When set to true the following properties will be considerd
        /// <seealso cref="WickUpStroke"/>
        /// <seealso cref="WickDownStroke"/>
        /// <seealso cref="WickStrokeThickness"/>
        /// <seealso cref="CandleUpFill"/>
        /// <seealso cref="CandleDownFill"/>
        /// <seealso cref="CandleUpStroke"/>
        /// <seealso cref="CandleDownStroke"/>
        /// <seealso cref="CandleStrokeThickness"/>
        ///</summary>
        public bool UseEnhancedColoring { get; set; }

        ///<summary>
        /// Stroke thickness of lines used. It is used as FontSize for StaticText objects
        ///</summary>
        public double StrokeThickness
        {
            get
            {
                return _strokeThickness;
            }
            set
            {
                if (_strokeThickness == value) return;
                _strokeThickness = value;
                SetStrokeThickness();
            }
        }

        ///<summary>
        /// Stroke pattern
        ///</summary>
        public LinePattern StrokePattern
        {
            get
            {
                return _strokePattern;
            }
            set
            {
                if (_strokePattern == value) return;
                _strokePattern = value;
                SetStrokeType();
            }
        }


        ///<summary> 
        /// Stroke Opacity 
        ///</summary> 
        public double Opacity
        {
            get { return _opacity; }
            set
            {
                if (_opacity == value) return;
                _opacity = value;
                SetOpacity();
            }
        }

        ///<summary>
        /// Is series selectable or not. If not, the user won't be able to select it with the mouse
        ///</summary>
        public bool Selectable
        {
            get { return _selectable; }
            set
            {
                if (_selectable == value) return;
                _selectable = value;
                foreach (Series s in _linkedSeries)
                    s.Selectable = value;
                if (!_selected || _selectable) return;
                _selected = false;
                ShowSelection();
            }
        }

        ///<summary>
        /// Hides or shows the series.
        ///</summary>
        public bool Visible
        {
            get { return _visible; }
            set
            {
                if (_visible == value) return;
                _visible = value;
                ShowHide();
            }
        }

        ///<summary>
        /// Minimum value that series has
        ///</summary>
        public double Min
        {
            get { return DM.Min(_seriesIndex); }
        }

        ///<summary>
        /// Maximum value that series has
        ///</summary>
        public double Max
        {
            get { return DM.Max(_seriesIndex); }
        }

        ///<summary>
        /// Series index. All series internally have an index, that is used to access their value in internal DataManager object
        ///</summary>
        public int SeriesIndex
        {
            get { return _seriesIndex; }
            set
            {
                if (_seriesIndex != value)
                {
                    _seriesIndex = value;
                    InvokePropertyChanged(PropertySeriesIndex);
                }
            }
        }

        ///<summary>
        /// Is series selected by the user with mouse or not.
        ///</summary>
        public bool Selected
        {
            get { return _selected; }
        }

        /// <summary>
        /// Returns a series value by index
        /// </summary>
        /// <param name="recordIndex"></param>
        /// <returns></returns>
        public DataEntry this[int recordIndex]
        {
            get { return DM[_seriesIndex].Data[recordIndex]; }
        }
        #endregion

        #region Virtual methods

        internal virtual void Paint()
        {
            throw new NotImplementedException();
        }
        internal virtual void Paint(object drawingContext)
        {
            throw new NotImplementedException();
        }
        internal virtual void SetStrokeColor()
        {
            throw new NotImplementedException();
        }
        internal virtual void SetStrokeThickness()
        {
            throw new NotImplementedException();
        }
        internal virtual void SetStrokeType()
        {
            throw new NotImplementedException();
        }
        internal virtual void SetOpacity()
        {
            throw new NotImplementedException();
        }
        internal virtual void RemovePaint()
        {
            throw new NotImplementedException();
        }
        internal virtual void ShowHide()
        {
            throw new NotImplementedException();
        }

        internal virtual void Init()
        {
            _strokeThickness = 1;
            _strokePattern = LinePattern.Solid;
            _strokeColor = Color.FromArgb(0xFF, 0x00, 0xFF, 0x00); //Lime

            _selectable = true;
            _selected = false;
            _visible = true;
            Painted = false;
            _upColor = null;
            _downColor = null;
            _shareScale = true;
        }

        internal virtual void UnSubscribe()
        {
            if (_seriesTickBoxValuePresenterLeft != null)
                _seriesTickBoxValuePresenterLeft.UnSubscribe();
            if (_seriesTickBoxValuePresenterRight != null)
                _seriesTickBoxValuePresenterRight.UnSubscribe();

            RemovePaint();
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        protected void UpdateTitle()
        {
            InvokePropertyChanged(PropertyLastValue);
        }

        /// <summary>
        /// Unscales a value and restores between max and min
        /// </summary>
        /// <param name="value">Value to be unnormalized</param>
        /// <returns>UnNormalized value</returns>
        public double UnNormalize(double value)
        {
            if (_chartPanel.ScalingType == ScalingTypeEnum.Semilog && Min > 0 && _chartPanel._hasPrice)
                return Math.Log10(_min) + (value * (Math.Log10(_max) - Math.Log10(_min)));
            return Min + (value * (Max - Min));
        }

        /// <summary>
        /// Normalizes a value between 1 and 0
        /// </summary>
        /// <param name="value">Value to be normalized</param>
        /// <returns>Normalized value</returns>
        public double Normalize(double value)
        {
            if (_chartPanel.ScalingType == ScalingTypeEnum.Semilog && _min > 0 && _chartPanel._hasPrice)
                return (value - Math.Log10(_min)) / (Math.Log10(_max) - Math.Log10(_min));
            return (value - _min) / (_max - _min);
        }

        /// <summary>
        /// Gets the Y pixel by price value
        /// </summary>
        /// <param name="seriesValue">Prive value</param>
        /// <returns>Y pixel</returns>
        public double GetY(double seriesValue)
        {
            if (_shareScale)
                return _chartPanel.GetY(seriesValue);

            double panelHeight = _chartPanel.PaintableHeight;
            if (panelHeight == 0)
                return 0.0;

            if (_chartPanel.ScalingType == ScalingTypeEnum.Semilog && _min > 0/* && _chartPanel._hasPrice*/)
                return (panelHeight - (panelHeight * Normalize(Math.Log10(seriesValue)))) + _chartPanel._yOffset;

            return (panelHeight - (panelHeight * Normalize(seriesValue))) + _chartPanel._yOffset;
        }

        /// <summary>
        /// Returns series value by Y pixel
        /// </summary>
        /// <param name="pixelValue">Pixel value</param>
        /// <returns>Price value</returns>
        public double GetReverseY(double pixelValue)
        {
            if (_shareScale)
                return _chartPanel.GetReverseY(pixelValue);

            double realHeight = _chartPanel.PaintableHeight;
            if (realHeight == 0)
                return 0.0;

            if (_chartPanel.ScalingType == ScalingTypeEnum.Semilog && Min > 0/* && _chartPanel._hasPrice*/)
            {
                pixelValue = UnNormalize(1 - ((pixelValue - _chartPanel._yOffset) / realHeight));
                if (pixelValue > 0 && Max > 0)
                    return Math.Pow(10, pixelValue);
            }

            return UnNormalize(1 - (pixelValue - _chartPanel._yOffset) / realHeight);
        }

        internal bool Painted { get; set; }

        internal DataManager.DataManager DM
        {
            get { return _chartPanel._chartX._dataManager; }
        }

        internal int RecordCount
        {
            get { return DM.RecordCount; }
        }

        internal DataManager.SeriesEntry SeriesEntry
        {
            get { return DM[_seriesIndex]; }
        }

        internal double MaxFromInterval(ref int startIndex, ref int endIndex)
        {
            return DM.MaxFromInterval(_seriesIndex, ref startIndex, ref endIndex);
        }

        internal double MinFromInterval(ref int startIndex, ref int endIndex)
        {
            return DM.MinFromInterval(_seriesIndex, ref startIndex, ref endIndex);
        }

        internal void ShowSelection()
        {
            if (!_selectable)
                return;

            _selected = true;

            double dx = 0;
            DataEntryCollection data = _chartPanel._chartX._dataManager[_seriesIndex].Data;

            _selectionDots.C = _chartPanel._rootCanvas;
            _selectionDots.Start();
            for (int i = _chartPanel._chartX._startIndex; i < _chartPanel._chartX._endIndex; i++)
            {
                if (!data[i].Value.HasValue)
                {
                    continue;
                }

                double x = _chartPanel._chartX.GetXPixel(i - _chartPanel._chartX._startIndex);
                if (x - dx <= 50)
                {
                    continue;
                }

                dx = x;
                SelectionDot dot = _selectionDots.GetPaintObject(Types.Corner.MoveAll);
                dot.SetPos(new Point(dx, GetY(data[i].Value.Value)));
                dot.Tag = this;
            }
            _selectionDots.Stop();
            _selectionDots.Do(dot => dot.ZIndex = ZIndexConstants.SelectionPoint1);
        }

        internal void HideSelection()
        {
            _selected = false;
            _selectionDots.RemoveAll();
        }

        internal virtual void MoveToPanel(ChartPanel chartPanel)
        {
            //each type of series has its own implementation
            //they will call base at the end to move linked series
            foreach (Series series in _linkedSeries)
            {
                series.MoveToPanel(chartPanel);
            }
        }

        /// <summary>
        /// Occurs when an internal property changes. For internal usage only
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        internal void InvokePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));

            if (propertyName == PropertyLastValue)
                CheckTrendLinesPenetration();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        protected void InvokePropertyChanged(PropertyChangedEventArgs args)
        {
            InvokePropertyChanged(args.PropertyName);
        }

        internal void CheckTrendLinesPenetration()
        {
            // go over each trendline present in current panel and see if it
            // penetrates the current series
            int recordCount = RecordCount;
            if (recordCount < 2)
            {
                return; //works for record > 2
            }

            double? value1 = this[recordCount - 1].Value;
            double? value2 = this[recordCount - 2].Value;
            if (!value1.HasValue || !value2.HasValue)
            {
                return; //need both values
            }

            foreach (TrendLine trendLine in _chartPanel.WatchableTrendLines)
            {
                double x1 = trendLine.X1Value;
                double y1 = trendLine.Y1Value;
                double x2 = trendLine.X2Value;
                double y2 = trendLine.Y2Value;

                if (x2 == x1)
                {
                    continue;
                }

                double incr = (y2 - y1) / (x2 - x1);
                double pointB = trendLine.Y2Value;
                double pointA = trendLine.Y2Value - incr;

                if (value1.Value > pointB && value2.Value < pointA)
                {
                    _chartPanel._chartX.FireTrendLinePenetration(trendLine, StockChartX.TrendLinePenetrationEnum.Above, this);
                }
                else if (value1.Value < pointB && value2.Value > pointA)
                {
                    _chartPanel._chartX.FireTrendLinePenetration(trendLine, StockChartX.TrendLinePenetrationEnum.Below, this);
                }
            }
        }

        /// <summary>
        /// Returns a string that represents this <see cref="Series"/> object.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("Name: {0}, SeriesType: {1}, SeriesTypeOhlc: {2}", FullName, _seriesType, _seriesTypeOHLC);
        }

        /// <summary>
        /// Gets all values
        /// </summary>
        public IEnumerable<double?> AllValues
        {
            get
            {
                for (int i = 0; i < _chartPanel._chartX.RecordCount; i++)
                    yield return this[i].Value;
            }
        }
    }
}

