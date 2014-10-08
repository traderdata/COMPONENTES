using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using ModulusFE.PaintObjects;

namespace ModulusFE
{
    public partial class StockChartX
    {
        #region ThreeDStyle

        private bool _threeDStyle = true;

        /// <summary>
        /// 
        /// </summary>
        public static readonly PropertyChangedEventArgs ThreeDStyleChangedEventsArgs =
          ObservableHelper.CreateArgs<StockChartX>(_ => _.ThreeDStyle);

        ///<summary>
        /// Gets or sets the value indicating whether to have 2D or 3D  candles
        ///</summary>
        public bool ThreeDStyle
        {
            get { return _threeDStyle; }
            set
            {
                if (_threeDStyle != value)
                {
                    _threeDStyle = value;
                    InvokePropertyChanged(ThreeDStyleChangedEventsArgs);

                    Update();
                }
            }
        }

        #endregion

        #region ScalingType

        private ScalingTypeEnum _scalingType = ScalingTypeEnum.Linear;

        /// <summary>
        /// 
        /// </summary>
        public static readonly PropertyChangedEventArgs ScalingTypeChangedEventsArgs =
          ObservableHelper.CreateArgs<StockChartX>(_ => _.ScalingType);

        ///<summary>
        /// Gets or sets the scaling type, linear or semilog
        ///</summary>
        public ScalingTypeEnum ScalingType
        {
            get { return _scalingType; }
            set
            {
                if (_scalingType != value)
                {
                    _scalingType = value;
                    InvokePropertyChanged(ScalingTypeChangedEventsArgs);
                    PanelsCollection.ForEach(_ => _.ScalingType = value);
                }
            }
        }

        #endregion

        #region UpColor

        private Color _upColor = Color.FromArgb(0xFF, 0x00, 0xFF, 0x00);

        /// <summary>
        /// 
        /// </summary>
        public static readonly PropertyChangedEventArgs UpColorChangedEventsArgs =
          ObservableHelper.CreateArgs<StockChartX>(_ => _.UpColor);

        ///<summary>
        /// Gets or sets the <seealso cref="Color"/> used to paint up-tick bars.When the close is higher than the previous close, this color will be used to paint the bar.
        ///</summary>
        public Color UpColor
        {
            get { return _upColor; }
            set
            {
                if (_upColor != value)
                {
                    _upColor = value;
                    InvokePropertyChanged(UpColorChangedEventsArgs);
                    Update();
                }
            }
        }

        #endregion

        #region DownColor

        private Color _downColor = Colors.Red;

        /// <summary>
        /// 
        /// </summary>
        public static readonly PropertyChangedEventArgs DownColorChangedEventsArgs =
          ObservableHelper.CreateArgs<StockChartX>(_ => _.DownColor);

        ///<summary>
        /// Gets or sets the <seealso cref="Color"/> used to paint down-tick bars. When the close is lower than the previous close, this color will be used to paint the bar. 
        ///</summary>
        public Color DownColor
        {
            get { return _downColor; }
            set
            {
                if (_downColor != value)
                {
                    _downColor = value;
                    InvokePropertyChanged(DownColorChangedEventsArgs);
                    Update();
                }
            }
        }

        #endregion

        #region LeftChartSpace

        private double _leftChartSpace = 10.0;

        /// <summary>
        /// 
        /// </summary>
        public static readonly PropertyChangedEventArgs LeftChartSpaceChangedEventsArgs =
          ObservableHelper.CreateArgs<StockChartX>(_ => _.LeftChartSpace);

        ///<summary>
        /// Gets or sets the left non-paitable chart area.
        ///</summary>
        public double LeftChartSpace
        {
            get { return _leftChartSpace; }
            set
            {
                if (_leftChartSpace != value)
                {
                    _leftChartSpace = value;
                    InvokePropertyChanged(LeftChartSpaceChangedEventsArgs);
                    Update();
                }
            }
        }

        #endregion

        #region RightChartSpace

        private double _rightChartSpace = 50.0;

        /// <summary>
        /// 
        /// </summary>
        public static readonly PropertyChangedEventArgs RightChartSpaceChangedEventsArgs =
          ObservableHelper.CreateArgs<StockChartX>(_ => _.RightChartSpace);

        ///<summary>
        /// Gets or sets the right side non-paitable chart area.
        ///</summary>
        public double RightChartSpace
        {
            get { return _rightChartSpace; }
            set
            {
                if (_rightChartSpace != value)
                {
                    _rightChartSpace = value;
                    InvokePropertyChanged(RightChartSpaceChangedEventsArgs);
                }
            }
        }

        #endregion

        #region RealTimeXLabels

        private bool _realTimeXLabels;

        /// <summary>
        /// when true X axis will display data suitable for real-time mode
        /// NOTE: Only valid for Calendar Version 1
        /// </summary>
        public static readonly PropertyChangedEventArgs RealTimeXLabelsChangedEventsArgs =
          ObservableHelper.CreateArgs<StockChartX>(_ => _.RealTimeXLabels);

        /// <summary>
        /// when true X axis will display data suitable for real-time mode
        /// </summary>
        public bool RealTimeXLabels
        {
            get { return _realTimeXLabels; }
            set
            {
                if (_realTimeXLabels != value)
                {
                    _realTimeXLabels = value;
                    InvokePropertyChanged(RealTimeXLabelsChangedEventsArgs);
                }
            }
        }

        #endregion

        #region ScalePrecision

        private int _scalePrecision = 2;

        /// <summary>
        /// 
        /// </summary>
        public static readonly PropertyChangedEventArgs ScalePrecisionChangedEventsArgs =
          ObservableHelper.CreateArgs<StockChartX>(_ => _.ScalePrecision);

        ///<summary>
        /// Gets or sets the precision used to paint values on Y axes
        ///</summary>
        public int ScalePrecision
        {
            get { return _scalePrecision; }
            set
            {
                if (_scalePrecision != value)
                {
                    _scalePrecision = value;
                    InvokePropertyChanged(ScalePrecisionChangedEventsArgs);

                    Update();
                }
            }
        }

        #endregion

        #region KeepZoomLevel

        private bool _keepZoomLevel;

        /// <summary>
        /// 
        /// </summary>
        public static readonly PropertyChangedEventArgs KeepZoomLevelChangedEventsArgs =
          ObservableHelper.CreateArgs<StockChartX>(_ => _.KeepZoomLevel);

        /// <summary>
        /// Get or sets the value that indicates if chart must keep current user's zoom level. 
        /// </summary>
        /// <value>
        /// true - anytime a new bar is added the chart will be scrolled to the left, this way last bar always will be seen, and the number of visible bars will be kept.
        /// false - when a new bar is added the chart will be compressed to show last bar.
        /// </value>
        public bool KeepZoomLevel
        {
            get { return _keepZoomLevel; }
            set
            {
                if (_keepZoomLevel != value)
                {
                    _keepZoomLevel = value;
                    InvokePropertyChanged(KeepZoomLevelChangedEventsArgs);
                }
            }
        }

        #endregion

        #region VolumeDivisor

        private int _volumeDivisor = 1000000;

        ///<summary>
        /// Gets or sets the divider for Volume values
        ///</summary>
        public static readonly PropertyChangedEventArgs VolumeDivisorChangedEventsArgs =
          ObservableHelper.CreateArgs<StockChartX>(_ => _.VolumeDivisor);

        ///<summary>
        /// Gets or sets the divider for Volume values
        ///</summary>
        public int VolumeDivisor
        {
            get { return _volumeDivisor; }
            set
            {
                if (_volumeDivisor != value)
                {
                    _volumeDivisor = value;
                    InvokePropertyChanged(VolumeDivisorChangedEventsArgs);

                    foreach (ChartPanel panel in _panelsContainer.Panels)
                    {
                        if (panel._leftYAxis != null && panel._leftYAxis.Visibility == Visibility.Visible)
                            panel._leftYAxis.Render();
                        if (panel._rightYAxis != null && panel._rightYAxis.Visibility == Visibility.Visible)
                            panel._rightYAxis.Render();
                    }
                }
            }
        }

        #endregion

    }
}