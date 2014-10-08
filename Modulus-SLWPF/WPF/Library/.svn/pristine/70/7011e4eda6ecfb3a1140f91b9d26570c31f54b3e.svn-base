using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ModulusFE.Exceptions;
using ModulusFE.Indicators;
using ModulusFE.LineStudies;
#if SILVERLIGHT
using ModulusFE.SL;
using ModulusFE.SL.Utils;
#endif
#if WPF
using System.Windows.Input;
using System.Globalization;
#endif

namespace ModulusFE
{
    /// <summary>
    /// Represents the chart. It works as a container for all panels.
    /// </summary>
    public partial class StockChartX
    {
        #region Properties
        /// <summary>
        /// Gets or sets the main symbol that is used in the chart. i.e. MSFT, DELL, ...
        /// </summary>
        public string Symbol { get; set; }

#if WPF
		/// <summary>
		/// Gets or sets the owner of the chart. Used for internal dialogs to be centered relative to the parent
		/// </summary>
		public Window OwnerWindow { get; set; }
#endif

        ///<summary>
        /// Gets font name used to paint Y grid, calendar text
        ///</summary>
        public string FontFace
        {
            get { return _fontFace; }
            set
            {
                if (_fontFace == value)
                    return;

                _fontFace = value;
                if (_calendar != null)
                    _calendar.UpdateFontInformation();
                UpdatePanelsFontInformation();
            }
        }

        ///<summary>
        /// Gets font size used to paint Y grid, calendar text
        ///</summary>
        public new double FontSize
        {
            get { return _fontSize; }
            set
            {
                if (_fontSize == value)
                    return;

                _fontSize = value;
                if (_calendar != null)
                    _calendar.UpdateFontInformation();
                UpdatePanelsFontInformation();
            }
        }


        ///<summary>
        /// Gets font foreground used to paint Y grid, calendar text
        ///</summary>
        public Brush FontForeground
        {
            get { return _fontForeground; }
            set
            {
                if (_fontForeground == value)
                    return;

                _fontForeground = value;
                if (_calendar != null)
                    _calendar.UpdateFontInformation();
                UpdatePanelsFontInformation();
            }
        }

        ///<summary>
        /// Gets or sets the maximum visible record count that are currently visible in the chart
        ///</summary>
        public int VisibleRecordCount
        {
            get { return (_endIndex - _startIndex) / _groupingInterval; }
        }

        ///<summary>
        /// Gets record count that are currently stored in the chart
        ///</summary>
        public int RecordCount
        {
            get { return _dataManager.RecordCount; }
        }

        ///<summary>
        /// Gets paintable width of panel that is used to paint series. 
        ///</summary>
        public double PaintableWidth
        {
            get
            {
                double width = ActualWidth;
                if (_scaleAlignement == ScaleAlignmentTypeEnum.Both || _scaleAlignement == ScaleAlignmentTypeEnum.Left)
                    width -= Constants.YAxisWidth;
                if (_scaleAlignement == ScaleAlignmentTypeEnum.Both || _scaleAlignement == ScaleAlignmentTypeEnum.Right)
                    width -= Constants.YAxisWidth;
                width -= _leftChartSpace;
                width -= _rightChartSpace;

                return width;
            }
        }

        /// <summary>
        /// Gets the left paintable side
        /// </summary>
        public double PaintableLeft
        {
            get
            {
                double left = _leftChartSpace;
                if (ScaleAlignment == ScaleAlignmentTypeEnum.Both || ScaleAlignment == ScaleAlignmentTypeEnum.Left)
                    left += Constants.YAxisWidth;

                return left;
            }
        }

        /// <summary>
        /// Gets the right paintable side
        /// </summary>
        public double PaintableRight
        {
            get
            {
                double right = ActualWidth - _rightChartSpace;
                if (ScaleAlignment == ScaleAlignmentTypeEnum.Both || ScaleAlignment == ScaleAlignmentTypeEnum.Right)
                    right -= Constants.YAxisWidth;

                return right;
            }
        }

        ///<summary>
        /// Gets or sets price style that is currently used in the chart
        ///</summary>
        public PriceStyleEnum PriceStyle
        {
            get { return _priceStyle; }
            set { _priceStyle = value; }
        }



        /// <summary>
        /// Gets or sets the YGrid step type calculation
        /// </summary>
        public YGridStepType YGridStepType { get; set; }

        ///<summary>
        /// Show or hide the Darvas boxes
        ///</summary>
        public bool DarvasBoxes
        {
            get { return _darwasBoxes; }
            set
            {
                if (value == _darwasBoxes) return;
                _darwasBoxes = value;
                Update();
            }
        }

        ///<summary>
        /// Gets or sets darvas boxes stop percent
        ///</summary>
        public double DarvasStopPercent
        {
            get { return _darvasPct; }
            set
            {
                if (value == _darvasPct) return;
                _darvasPct = value;
                Update();
            }
        }

        ///<summary>
        /// Gets panels count used in the chart
        ///</summary>
        public int PanelsCount
        {
            get { return _panelsContainer.Panels.Count; }
        }

        ///<summary>
        /// Gets reference to panel that is currently maximized, or null if there isn't such a panel
        ///</summary>
        public ChartPanel MaximizedPanel
        {
            get { return _panelsContainer._maximizedPanel; }
        }

        ///<summary>
        /// Gets the collection of all indicators from all panels
        ///</summary>
        public IEnumerable<Indicator> IndicatorsCollection
        {
            get
            {
                return _panelsContainer.Panels.SelectMany(chartPanel => chartPanel.IndicatorsCollection);
            }
        }

        /// <summary>
        /// Get the collection of all series from all panels 
        /// </summary>
        public IEnumerable<Series> SeriesCollection
        {
            get
            {
                return _panelsContainer.Panels.SelectMany(chartPanel => chartPanel.SeriesCollection);
            }
        }

        ///<summary>
        /// Gets the collection of all LineStudies from all panels
        ///</summary>
        public IEnumerable<LineStudy> LineStudiesCollection
        {
            get
            {
                return _panelsContainer.Panels.SelectMany(chartPanel => chartPanel._lineStudies);
            }
        }

        /// <summary>
        /// Gets the number of panels ignoring panels with HeatMap on them
        /// </summary>
        public int UseablePanelsCount
        {
            get
            {
                return _panelsContainer.Panels.Count(panel => !panel.IsHeatMap);
            }
        }

        ///<summary>
        /// Gets or sets chart type
        ///</summary>
        public ChartTypeEnum ChartType
        {
            get { return _chartType; }
            set
            {
                if (_chartType == value) return;
                _chartType = value;
            }
        }

        ///<summary>
        /// Gets or sets bar width used ti paint the wick of candles
        ///</summary>
        public double BarWidth
        {
            get { return _barWidth; }
            set
            {
                if (_barWidth == value || value < 1) return;
                _barWidth = value;
            }
        }

        /// <summary>
        /// Gets or sets whether user can create a zoom area using mouse
        /// </summary>
        public bool DisableZoomArea { get; set; }

        ///<summary>
        /// When UseLineSeriesUpDownColors is set to True, StockChartX will display the UpColor for values of oscillators that are above 0, and DownColor for values of oscillators that are below 0. 
        ///</summary>
        public bool UseLineSeriesUpDownColors
        {
            get { return _useLineSeriesColors; }
            set
            {
                if (value == _useLineSeriesColors) return;
                _useLineSeriesColors = value;
            }
        }

        ///<summary>
        /// When UseVolumeUpDownColors is set to True, StockChartX will display the UpColor of the symbol's up candle color, and DownColor for symbol's down candle color. The volume series must be named as part of a symbol group (e.g "MSFT.volume").
        ///</summary>
        public bool UseVolumeUpDownColors
        {
            get { return _useVolumeUpDownColors; }
            set
            {
                if (value == _useVolumeUpDownColors) return;
                _useVolumeUpDownColors = value;
            }
        }

        ///<summary>
        /// Gets or sets the candle outline color (for hollow 2D candles). When the close is lower than the previous close, this color will be used to paint the bar outline. 
        ///</summary>
        public Color? CandleUpOutlineColor
        {
            get { return _candleUpOutlineColor; }
            set
            {
                if (value == _candleUpOutlineColor) return;
                _candleUpOutlineColor = value;
            }
        }

        ///<summary>
        /// Gets or sets the candle outline color (for hollow 2D candles). When the close is lower than the previous close, this color will be used to paint the bar outline.
        ///</summary>
        public Color? CandleDownOutlineColor
        {
            get { return _candleDownOutlineColor; }
            set
            {
                if (value == _candleDownOutlineColor) return;
                _candleDownOutlineColor = value;
            }
        }

        ///<summary>
        /// Works with the <see cref="CandleUpOutlineColor"/> to set which color is used for the 'up' wick (very useful when using hollow candles). Default is to use the fill color, but the outline can be used too.
        ///</summary>
        public bool CandleUpWickMatchesOutlineColor
        {
            get { return _candleUpWickMatchesOutlineColor; }
            set
            {
                if (value == _candleUpWickMatchesOutlineColor) return;
                _candleUpWickMatchesOutlineColor = value;
            }
        }

        ///<summary>
        /// Works with the <see cref="CandleDownOutlineColor"/> to set which color is used for the 'down' wick (very useful when using hollow candles). Default is to use the fill color, but the outline can be used too.
        ///</summary>
        public bool CandleDownWickMatchesOutlineColor
        {
            get { return _candleDownWickMatchesOutlineColor; }
            set
            {
                if (value == _candleDownWickMatchesOutlineColor) return;
                _candleDownWickMatchesOutlineColor = value;
            }
        }

        /// <summary>
        /// Gets the record number of the first visible record on the chart. This value may change as the chart is zoomed or scrolled.
        /// </summary>
        public int FirstVisibleRecord
        {
            get { return _startIndex; }
            set
            {
                if (value == _startIndex)
                {
                    return;
                }

                if (value >= RecordCount || value >= _endIndex)
                {
                    return;
                }

                _startIndex = value;
                OnPropertyChanged(Property_StartIndex);
                Update();
            }
        }

        /// <summary>
        /// Gets the number of selected LineStudies on all panels from chart
        /// </summary>
        public int LineStudySelectedCount { get; internal set; }

        /// <summary>
        /// Gets the record number of the last visible record on the chart. This value may change as the chart is zoomed or scrolled. 
        /// </summary>
        public int LastVisibleRecord
        {
            get { return _endIndex; }
            set
            {
                if (value == _endIndex)
                {
                    return;
                }

                if (value <= _startIndex || value > RecordCount)
                {
                    return;
                }

                _endIndex = value;
                OnPropertyChanged(Property_EndIndex);
                Update();
            }
        }

        /// <summary>
        /// Gets the selected objects from all panels
        /// </summary>
        public List<object> SelectedObjectsCollection
        {
            get
            {
                List<object> ret = new List<object>();
                foreach (ChartPanel chartPanel in _panelsContainer.Panels)
                {
                    if (chartPanel._seriesSelected != null && chartPanel._seriesSelected.Selected)
                        ret.Add(chartPanel._seriesSelected);
                    if (chartPanel._lineStudySelected != null && chartPanel._lineStudySelected.Selected)
                        ret.Add(chartPanel._lineStudySelected);
                }

                return ret;
            }
        }

        ///<summary>
        /// Gets the collection of panels from the chart
        ///</summary>
        public IEnumerable<ChartPanel> PanelsCollection
        {
            get
            {
                if (_panelsContainer == null)
                    yield break;

                foreach (ChartPanel chartPanel in _panelsContainer.Panels)
                {
                    yield return chartPanel;
                }
            }
        }

        ///<summary>
        /// 
        ///</summary>
        public TextBlock LabelTitle
        {
            get
            {
                return _textLabelTitle;
            }
        }

        ///<summary>
        /// Returns a series from OHLC group. Series name is formed from <see cref="Symbol"/> + series type
        ///</summary>
        ///<param name="ohlc"></param>
        public Series this[SeriesTypeOHLC ohlc]
        {
            get
            {
                return SeriesCollection.FirstOrDefault(_ => _._seriesTypeOHLC == ohlc);
            }
        }

        ///<summary>
        /// Gets a reference to a <see cref="ChartPanel"/> by its index
        ///</summary>
        ///<param name="panelIndex"></param>
        public ChartPanel this[int panelIndex]
        {
            get
            {
                return PanelsCollection.FirstOrDefault(_ => _.Index == panelIndex);
            }
        }

        ///<summary>
        ///</summary>
        public bool OptimizePainting { get; set; }

        /// <summary>
        /// Gets the <see cref="ChartPanel"/> that is currently under mouse cursor
        /// </summary>
        public ChartPanel CurrentPanel
        {
            get { return _panelsContainer.PanelByY(Mouse.GetPosition(_panelsContainer).Y); }
        }

        /// <summary>
        /// The list of different scales that are able to be used as labels in the calendar (x axis)
        /// The user can play with which scales are available, and they can play with the string output
        /// formats that are used to display the timeStamps. 
        /// </summary>
        public List<CalendarScaleData> CalendarV2CalendarScaleDataList
        {
            get
            {
                if (_calendar == null) return null;
                return _calendar.CalendarScaleDataList;
            }
        }

        #endregion

        #region Methods

        private bool _frozen;

        ///<summary>
        /// Makes chart do not update itself when changing public visual properties, such as FirstVisibleRecord
        ///</summary>
        public void Freeze()
        {
            _frozen = true;
        }

        ///<summary>
        /// Makes chart to update itself
        ///</summary>
        public void Melt()
        {
            _frozen = false;
        }

        /// <summary>
        /// Forces to invalidate the chart
        /// </summary>
        public void Update()
        {
            _timers.StopTimerWork(TimerUpdate);

            //Debug.WriteLine("Chart.Update - Status " + _status);
            if (_frozen
                || RecordCount == 0
                || (_status != ChartStatus.Ready && _status != ChartStatus.LineStudyPaintReady))
                return;

            if (!CheckRegistration())
            {
                MessageBox.Show(
#if WPF
					OwnerWindow,
#endif
"Registration for current version expired or invalid." + Environment.NewLine +
                                                "Please contact support@modulusfe.com for help.",
                                                "Registration expired or invalid", MessageBoxButton.OK);
                return;
            }

            if (Symbol.Length == 0)
                throw new SymbolNotSetException();



            if (_startIndex > RecordCount - 1)
                _startIndex = 0;

            //SetGroupingValue();

            if (!_scrollerUpdating && _scroller != null)
            {
                //_chartScroller.Freeze();
                //_chartScroller.MaxValue = RecordCount;
                //_chartScroller.LeftValue = _startIndex;
                //_chartScroller.RightValue = _endIndex;
                //_chartScroller.Melt();
                _scroller.PaintTrend();
                _scroller.PaintSelection();
            }

            if (ReCalc)
                InvalidateIndicators();

#if WPF
#if DEMO
			_calendar._demoText = _demoText;
#endif
#endif
            _calendar.Paint();

            _panelsContainer.ResetPanels();
        }

        /// <summary>
        /// Adds an indicator to a specified panel
        /// </summary>
        /// <param name="indicatorType">Indicator type</param>
        /// <param name="key">An unique key for indicator</param>
        /// <param name="chartPanel">a valid reference to a panel</param>
        /// <param name="userParams">true - the indicator parameters will be set by code
        /// false - indicator will show a dialog where user will choose its parameters</param>
        /// <returns>a reference to an indicator object</returns>
        public Indicator AddIndicator(IndicatorType indicatorType, string key, ChartPanel chartPanel,
                bool userParams)
        {
            return AddIndicator(indicatorType, key, chartPanel, userParams, false);
        }

        /// <summary>
        /// Adds an indicator to a specified panel
        /// </summary>
        /// <param name="indicatorType">Indicator type</param>
        /// <param name="key">An unique key for indicator</param>
        /// <param name="chartPanel">a valid reference to a panel</param>
        /// <param name="userParams">true - the indicator parameters will be set by code
        /// false - indicator will show a dialog where user will choose its parameters</param>
        /// <param name="ignoreErrors"></param>
        /// <returns>a reference to an indicator object</returns>
        public Indicator AddIndicator(IndicatorType indicatorType, string key, ChartPanel chartPanel,
            bool userParams, bool ignoreErrors)
        {
            if (GetSeriesByName(key) != null)
                throw new KeyNotUniqueException(key);

            Indicator res =
                (Indicator)Activator.CreateInstance(StockChartX_IndicatorsParameters.GetIndicatorCLRType(indicatorType),
                                                                                        new object[] { key, chartPanel });

            res.UserParams = userParams;
            res._toBeAdded = true;
            res._ignoreErrors = ignoreErrors;

            ReCalc = true;
            _changed = true;
            chartPanel.AddSeries(res);
            return res;
        }

        private readonly string[] OverlayIndicatorNames
            = new[]
					{
						"PARABOLIC", "PSAR", "FORECAST", "INTERCEPT",
						"WEIGHTED CLOSE", "TYPICAL PRICE", "WEIGHTED PRICE",
						"MEDIAN PRICE", "SMOOTHING", "BOLLINGER",
						"MOVING AVERAGE", "BANDS"
					};

        ///<summary>
        /// By indicator name suggests eother to create a new panel or not. Some indicators have values way different then series and it's recomended to create different panels for them.
        ///</summary>
        ///<param name="indicatorType">Indicator type</param>
        ///<returns>true - it is recomended to create a new panel for indicator.</returns>
        public bool IsOverlayIndicator(IndicatorType indicatorType)
        {
            string indicatorName = StockChartX_IndicatorsParameters.GetIndicatorName(indicatorType).ToUpper();
            return OverlayIndicatorNames.Any(indicatorName.Contains);
        }

        /// <summary>
        /// Initiates a line study painted by code
        /// </summary>
        /// <param name="studyTypeEnum">Study type</param>
        /// <param name="key">Unique key</param>
        /// <param name="stroke">Brush used to paint the lines</param>
        /// <param name="panelIndex">Panel index where to place line study</param>
        /// <param name="args">mainly used for ImageObject when setting image path</param>
        /// <returns>Reference to newly created line study</returns>
        public LineStudy AddLineStudy(LineStudy.StudyTypeEnum studyTypeEnum, string key, Brush stroke, int panelIndex, params object[] args)
        {
            ChartPanel chartPanel = GetPanelByIndex(panelIndex);

            chartPanel._lineStudyToAdd =
                (LineStudy)Activator.CreateInstance(StockChartX_LineStudiesParams.GetLineStudyCLRType(studyTypeEnum),
                                                                                        new object[] { key, stroke, chartPanel });
            chartPanel._lineStudyToAdd.SetArgs(args);

            _currentPanel = chartPanel;
            Status = ChartStatus.LineStudyPaintReady;

            return chartPanel._lineStudyToAdd;
        }

        /// <summary>
        /// Initiates a line study painted by user
        /// </summary>
        /// <param name="studyTypeEnum">Study type</param>
        /// <param name="key">Unique key</param>
        /// <param name="stroke">Brush used to paint the lines</param>
        /// <param name="args">mainly used for ImageObject when setting image path</param>
        /// <returns>Reference to newly created line study</returns>
        public LineStudy AddLineStudy(LineStudy.StudyTypeEnum studyTypeEnum, string key, Brush stroke, params object[] args)
        {
            _currentPanel = null; //will set in panel where user will click

            _lineStudyToAdd = (LineStudy)Activator.CreateInstance(StockChartX_LineStudiesParams.GetLineStudyCLRType(studyTypeEnum),
                                                                                        new object[] { key, stroke, null });
            _lineStudyToAdd.SetArgs(args);

            Status = ChartStatus.LineStudyPaintReady;

            return _lineStudyToAdd;
        }

        /// <summary>
        /// Adds a static text and lets user position it at needed position
        /// </summary>
        /// <param name="staticText">A user defined text</param>
        /// <param name="key">Unique key</param>
        /// <param name="foreground">Foreground Brush</param>
        /// <param name="fontSize">Font size</param>
        /// <param name="panelIndex">Panel index where to place the text</param>
        /// <returns>Reference to <seealso cref="StaticText"/> object</returns>
        public StaticText AddStaticText(string staticText, string key, Brush foreground, double fontSize, int panelIndex)
        {
            ChartPanel chartPanel = GetPanelByIndex(panelIndex);

            StaticText lineStaticText =
                (StaticText)Activator.CreateInstance(typeof(StaticText), new object[] { key, foreground, chartPanel });
            lineStaticText.SetArgs(new object[] { staticText });
            lineStaticText.StrokeThickness = fontSize;

            _currentPanel = chartPanel;

            if (Status == ChartStatus.Ready)
                lineStaticText.Paint(0, 0, LineStudy.LineStatus.StartPaint);
            chartPanel._lineStudies.Add(lineStaticText);

            return lineStaticText;
        }

        /// <summary>
        /// Initiates a symbol object placed by user by  user
        /// </summary>
        /// <param name="symbolType">Symbol type</param>
        /// <param name="key">Unique key</param>
        /// <param name="panelIndex">Panel index where to place the symbol object</param>
        public void AddSymbolObject(SymbolType symbolType, string key, int panelIndex)
        {
            ChartPanel chartPanel = GetPanelByIndex(panelIndex);
            chartPanel._lineStudyToAdd =
                (LineStudy)Activator.CreateInstance(StockChartX_LineStudiesParams.GetLineStudyCLRType(LineStudy.StudyTypeEnum.ImageObject),
                                                                                        new object[] { key, Brushes.Transparent, chartPanel });
            chartPanel._lineStudyToAdd.SetArgs(symbolType);

            _currentPanel = chartPanel;
            Status = ChartStatus.LineStudyPaintReady;
        }

        ///<summary>
        /// Initiates a symbol object placed by user by  user
        ///</summary>
        /// <param name="symbolType">Symbol type</param>
        /// <param name="key">Unique key</param>
        public void AddSymbolObject(SymbolType symbolType, string key)
        {
            _currentPanel = null;

            _lineStudyToAdd =
                (LineStudy)Activator.CreateInstance(StockChartX_LineStudiesParams.GetLineStudyCLRType(LineStudy.StudyTypeEnum.ImageObject),
                                                                 new object[] { key, Brushes.Transparent, null });

            _lineStudyToAdd.SetArgs(symbolType);

            Status = ChartStatus.LineStudyPaintReady;
        }

        /// <summary>
        /// Adds a line study programmatically
        /// </summary>
        /// <param name="stroke">Brush used to paint line study</param>
        /// <param name="studyTypeEnum">Study type</param>
        /// <param name="key">Unique key</param>
        /// <param name="panelIndex">Panel index where to place line study</param>
        /// <returns>A reference to the line study created</returns>
        public LineStudy CreateLineStudy(LineStudy.StudyTypeEnum studyTypeEnum, string key, Brush stroke, int panelIndex)
        {
            return CreateLineStudy(studyTypeEnum, key, stroke, panelIndex, null);
        }

        /// <summary>
        /// Adds a line study programmatically
        /// </summary>
        /// <param name="stroke">Brush used to paint line study</param>
        /// <param name="studyTypeEnum">Study type</param>
        /// <param name="key">Unique key</param>
        /// <param name="panelIndex">Panel index where to place line study</param>
        /// <param name="args">Optional parameters to be passed to LineStudy</param>
        /// <returns>A reference to the line study created</returns>
        public LineStudy CreateLineStudy(LineStudy.StudyTypeEnum studyTypeEnum, string key, Brush stroke, int panelIndex, object[] args)
        {
            ChartPanel chartPanel = GetPanelByIndex(panelIndex);

            LineStudy lineStudy =
                (LineStudy)Activator.CreateInstance(StockChartX_LineStudiesParams.GetLineStudyCLRType(studyTypeEnum),
                                                                                        new object[] { key, stroke, chartPanel });
            if (args != null)
            {
                lineStudy.SetArgs(args);
            }

            lineStudy.SetChartPanel(chartPanel);

            if (Status == ChartStatus.Ready)
            {
                lineStudy.Paint(0, 0, LineStudy.LineStatus.StartPaint);
            }

            chartPanel._lineStudies.Add(lineStudy);

            return lineStudy;
        }

        /// <summary>
        /// Adds a symbol object programmatically
        /// </summary>
        /// <param name="symbolType">Symbol type</param>
        /// <param name="key">Unique key</param>
        /// <param name="panelIndex">Panel index where to place symbol object</param>
        /// <param name="size"></param>
        /// <returns>Reference to symbol object created.</returns>
        public LineStudy CreateSymbolObject(SymbolType symbolType, string key, int panelIndex, Size size)
        {
            ChartPanel chartPanel = GetPanelByIndex(panelIndex);

            LineStudy lineStudy =
                (LineStudy)Activator.CreateInstance(StockChartX_LineStudiesParams.GetLineStudyCLRType(LineStudy.StudyTypeEnum.ImageObject),
                                                                                        new object[] { key, Brushes.Transparent, chartPanel });
            lineStudy.SetArgs(symbolType, size);

            if (Status == ChartStatus.Ready)
                lineStudy.Paint(0, 0, LineStudy.LineStatus.StartPaint);
            chartPanel._lineStudies.Add(lineStudy);

            return lineStudy;
        }

        /// <summary>
        /// Gets the width of a text including white spaces using current font properties
        /// </summary>
        /// <param name="text">Text</param>
        /// <returns>Width</returns>
        public double GetTextWidth(string text)
        {
#if WPF
			return new FormattedText(text, CultureInfo.CurrentCulture,
					FlowDirection.LeftToRight, new Typeface(FontFace), FontSize, Brushes.Black).WidthIncludingTrailingWhitespace;
#endif
#if SILVERLIGHT
            TextBlock tb = new TextBlock { FontFamily = new FontFamily(FontFace), FontSize = FontSize, Text = text };
            //tb.UpdateLayout();
            return tb.ActualWidth;
#endif
        }


        /// <summary>
        /// Gets the height of a text using current font properties
        /// </summary>
        /// <param name="text">Text</param>
        /// <returns>Height</returns>
        public double GetTextHeight(string text)
        {
#if WPF
			return new FormattedText(text, CultureInfo.CurrentCulture,
					FlowDirection.LeftToRight, new Typeface(FontFace), FontSize, Brushes.Black).Height;
#endif
#if SILVERLIGHT
            TextBlock tb = new TextBlock { FontFamily = new FontFamily(FontFace), FontSize = FontSize, Text = text };
            //tb.UpdateLayout();
            return tb.ActualHeight;
#endif
        }

        ///<summary>
        /// Gets a series from an OHLC group by a given OHLC type and another series from this group.
        ///</summary>
        ///<param name="series">Series base</param>
        ///<param name="seriesTypeOHLC">OHLC type</param>
        ///<returns>Reference to needed series or null</returns>
        public Series GetSeriesOHLCV(Series series, SeriesTypeOHLC seriesTypeOHLC)
        {
            return _panelsContainer.Panels
                .Select(chartPanel => chartPanel.GetSeriesOHLCV(series, seriesTypeOHLC))
                .FirstOrDefault(s => s != null);
        }

        /// <summary>
        /// Gets maximum value for a series
        /// </summary>
        /// <param name="series">Reference to <seealso cref="Series"/></param>
        /// <returns>Maximum value</returns>
        public double? GetMaxValue(Series series)
        {
            return GetMax(series, false, false);
        }

        /// <summary>
        /// Gets maximum value for a series
        /// </summary>
        /// <param name="seriesName">Series name</param>
        /// <returns>Maximum value or null if series doesn't exists</returns>
        public double? GetMaxValue(string seriesName)
        {
            Series series = GetSeriesByName(seriesName);
            return series == null ? null : GetMaxValue(series);
        }

        /// <summary>
        /// Gets the maximum visible value for a series
        /// </summary>
        /// <param name="series">Reference to <seealso cref="Series"/></param>
        /// <returns>Maximum value from visible records</returns>
        public double? GetVisibleMaxValue(Series series)
        {
            return GetMax(series, false, true);
        }

        /// <summary>
        /// Gets the maximum visible value for a series
        /// </summary>
        /// <param name="seriesName">Series name</param>
        /// <returns>Maximum value from visible records></returns>
        public double? GetVisibleMaxValue(string seriesName)
        {
            Series series = GetSeriesByName(seriesName);
            return series == null ? null : GetVisibleMaxValue(series);
        }


        internal double GetMin(Series series, bool ignoreZero, bool onlyVisible)
        {
            double min = double.MaxValue;
            int start = 0;
            int count = series.RecordCount;
            if (onlyVisible)
            {
                start = _startIndex;
                count = _endIndex;
            }
            for (int i = start; i < count; i++)
            {
                if (!series[i].Value.HasValue)
                    continue;

                if (ignoreZero)
                {
                    if (series[i].Value < min && series[i].Value.Value != 0.0)
                        min = series[i].Value.Value;
                }
                else
                {
                    if (series[i].Value < min)
                        min = series[i].Value.Value;
                }
            }
            return min;
        }
        internal double GetMin(Series series, bool ignoreZero)
        {
            return GetMin(series, ignoreZero, false);
        }
        /// <summary>
        /// Gets minimum value for a series
        /// </summary>
        /// <param name="series">Reference to <seealso cref="Series"/></param>
        /// <returns>Miniumu value</returns>
        public double? GetMinValue(Series series)
        {
            return GetMin(series, false, false);
        }
        /// <summary>
        /// Gets minimum value for a series
        /// </summary>
        /// <param name="seriesName">Series name</param>
        /// <returns>Minimum value</returns>
        public double? GetMinValue(string seriesName)
        {
            Series series = GetSeriesByName(seriesName);
            return series == null ? null : GetMinValue(series);
        }
        /// <summary>
        /// Gets the minimum visible value for a series
        /// </summary>
        /// <param name="series">Reference to <seealso cref="Series"/></param>
        /// <returns>Minimum value from visible records</returns>
        public double? GetVisibleMinValue(Series series)
        {
            return GetMin(series, false, true);
        }
        /// <summary>
        /// Gets the minimum visible value for a series
        /// </summary>
        /// <param name="seriesName">Series name</param>
        /// <returns>Minimum value from visible records</returns>
        public double? GetVisibleMinValue(string seriesName)
        {
            Series series = GetSeriesByName(seriesName);
            return series == null ? null : GetVisibleMinValue(series);
        }

        ///<summary>
        /// Adds a new chart panel and Gets a reference to it. Position type is None
        ///</summary>
        ///<returns>Reference to the newly created chart panel</returns>
        public ChartPanel AddChartPanel()
        {
            return _panelsContainer.AddPanel(ChartPanel.PositionType.None);
        }
        ///<summary>
        /// Adds a chart panel with a specified type of positioning
        ///</summary>
        ///<param name="position">Position type</param>
        ///<returns>Reference to the newly created chart panel</returns>
        public ChartPanel AddChartPanel(ChartPanel.PositionType position)
        {
            return _panelsContainer.AddPanel(position);
        }
        ///<summary>
        /// Adds a panel that will show the heat map panel. Such a panel can't hold any series or line studies.
        /// Only one instance of heat map can exists
        ///</summary>
        ///<returns>Reference to the newly created chart panel</returns>
        public ChartPanel AddHeatMapPanel()
        {
            //make sure we have only one heat map
            ChartPanel heatMap = PanelsCollection.FirstOrDefault(p => p.IsHeatMap);
            if (heatMap != null)
                return heatMap;

            heatMap = _panelsContainer.AddPanel(ChartPanel.PositionType.None, true);
            Update();
            return heatMap;
        }

        ///<summary>
        /// Destroys the heap map panel
        ///</summary>
        public void DeleteHeatMap()
        {
            _panelsContainer.CloseHeatMap();
        }

        ///<summary>
        /// Gets a reference to a series by its name or null if such series doesn't exists
        ///</summary>
        ///<param name="seriesName">Series name</param>
        /// <example>
        /// <code>
        /// Series seriesOpen = _stockChartX.GetSeriesByName(_stockChartX.Symbol + ".open");
        /// </code>
        /// </example>
        ///<returns>Reference to series</returns>
        public Series GetSeriesByName(string seriesName)
        {
            return _panelsContainer.Panels
                .SelectMany(panel => panel.AllSeriesCollection)
                .FirstOrDefault(series => Utils.StrICmp(seriesName, series.FullName));
        }

        /// <summary>
        /// Gets a panel by its index. ignores panels that have heat map
        /// </summary>
        /// <param name="index">Panel Index</param>
        /// <returns>Reference to ChartPanel</returns>
        public ChartPanel GetPanelByIndex(int index)
        {
            return _panelsContainer.Panels[index];

            //      for (int i = 0; i < _panelsContainer.Panels.Count; i++)
            //      {
            //        if (_panelsContainer.Panels[i].IsHeatMap) continue;
            //        if (index-- == 0)
            //          return _panelsContainer.Panels[i];
            //      }
            //      throw new IndexOutOfRangeException("index");
        }

        /// <summary>
        /// Gets the total number of indicator of a specified type
        /// </summary>
        /// <param name="indicatorType">Indicator type</param>
        /// <returns>Number of indicator that matches given indicator type</returns>
        public int GetIndicatorCountByType(IndicatorType indicatorType)
        {
            return _panelsContainer.Panels
                .SelectMany(chartPanel => chartPanel.IndicatorsCollection)
                .Count(indicator => indicator.IndicatorType == indicatorType);
        }

        /// <summary>
        /// Gets the total number of line studies from the chart by its type
        /// </summary>
        /// <param name="studyTypeEnum">Study type</param>
        /// <returns>Number of line studies that matches the given study type</returns>
        public int GetLineStudyCountByType(LineStudy.StudyTypeEnum studyTypeEnum)
        {
            return _panelsContainer.Panels
                .SelectMany(chartPanel => chartPanel._lineStudies)
                .Count(study => study.StudyType == studyTypeEnum);
        }

        ///<summary>
        /// Sets the parameter for a price style.
        ///</summary>
        ///<param name="index">Index of parameter</param>
        ///<param name="value">New value</param>
        /// <example>
        /// <code>
        /// _stockChartX.SetPriceStyleParam(0, 0); //Reversal size
        /// _stockChartX.SetPriceStyleParam(1, (double)StockChartX.ChartDataType.Points);
        /// </code>
        /// </example>
        public void SetPriceStyleParam(int index, double value)
        {
            _priceStyleParams[index] = value;
        }

        /// <summary>
        /// Gets the value of a parameter for current price style
        /// </summary>
        /// <param name="index">Parameter index</param>
        /// <returns>Parameter's value</returns>
        public double GetPriceStyleParam(int index)
        {
            return _priceStyleParams[index];
        }

        ///<summary>
        /// Return price style value 1 by record index
        ///</summary>
        ///<param name="recordIndex">Record index</param>
        ///<returns>Value</returns>
        public double? GerPriceStyleValue1(int recordIndex)
        {
            return _psValues1[recordIndex].Value;
        }
        ///<summary>
        /// Return price style value 1 by time stamp
        ///</summary>
        ///<param name="timeStamp">Timestamp</param>
        ///<returns>Value</returns>
        public double? GerPriceStyleValue1(DateTime timeStamp)
        {
            int recordIndex = _dataManager.GetTimeStampIndex(timeStamp);
            if (recordIndex == -1) return null;
            return _psValues1[recordIndex].Value;
        }

        ///<summary>
        /// Return price style value 2 by record index
        ///</summary>
        ///<param name="recordIndex">Record Index</param>
        ///<returns>Value</returns>
        public double? GerPriceStyleValue2(int recordIndex)
        {
            return _psValues2[recordIndex].Value;
        }
        ///<summary>
        /// Return price style value 2 by time stamp
        ///</summary>
        ///<param name="timeStamp">TimeStamp</param>
        ///<returns>Value</returns>
        public double? GerPriceStyleValue2(DateTime timeStamp)
        {
            int recordIndex = _dataManager.GetTimeStampIndex(timeStamp);
            if (recordIndex == -1) return null;
            return _psValues2[recordIndex].Value;
        }

        ///<summary>
        /// Return price style value 3 by record index
        ///</summary>
        ///<param name="recordIndex">Record index</param>
        ///<returns>Value</returns>
        public double? GerPriceStyleValue3(int recordIndex)
        {
            return _psValues3[recordIndex].Value;
        }
        ///<summary>
        /// Return price style value 3 by time stamp
        ///</summary>
        ///<param name="timeStamp">Time stamp</param>
        ///<returns>Value</returns>
        public double? GerPriceStyleValue3(DateTime timeStamp)
        {
            int recordIndex = _dataManager.GetTimeStampIndex(timeStamp);
            if (recordIndex == -1) return null;
            return _psValues3[recordIndex].Value;
        }

        ///<summary>
        /// Gets an object type from current mouse position
        ///</summary>
        ///<param name="o">Reference to an object or null</param>
        ///<returns>Object's type</returns>
        public ObjectFromCursor GetObjectFromCursor(out object o)
        {
            o = null;
            Point pg = Mouse.GetPosition(this);
            if (AbsoluteRect(_calendar).Contains(pg))
            {
                o = _calendar;
                return ObjectFromCursor.Calendar;
            }

            if (AbsoluteRect(_panelsBar).Contains(pg))
            {
                o = _panelsBar;
                return ObjectFromCursor.PanelsBar;
            }

            Point p = Mouse.GetPosition(_panelsContainer);
            ChartPanel chartPanel = _panelsContainer.PanelByY(p.Y) ?? _panelsContainer._maximizedPanel;
            if (chartPanel == null)
                return ObjectFromCursor.NoObject;

            if (AbsoluteRect(chartPanel._leftYAxis).Contains(pg))
            {
                o = chartPanel._leftYAxis;
                return ObjectFromCursor.PanelLeftYAxis;
            }
            if (AbsoluteRect(chartPanel._rightYAxis).Contains(pg))
            {
                o = chartPanel._rightYAxis;
                return ObjectFromCursor.PanelRightYAxis;
            }
            if (AbsoluteRect(chartPanel._titleBar).Contains(pg))
            {
                o = chartPanel._titleBar;
                return ObjectFromCursor.PanelTitleBar;
            }

            GeneralTransform generalTransform =
#if WPF
				chartPanel._rootCanvas.TransformToAncestor(this);
#endif
#if SILVERLIGHT
 chartPanel._rootCanvas.TransformToVisual(this);
#endif
            Point location = generalTransform.Transform(new Point(0, 0));
            Rect rcCanvas = new Rect(location.X, location.Y, chartPanel._rootCanvas.ActualWidth, chartPanel._rootCanvas.ActualHeight);

            o = chartPanel;
            if (pg.X >= rcCanvas.Left && pg.X <= rcCanvas.Left + LeftChartSpace)
                return ObjectFromCursor.PanelLeftNonPaintableArea;
            if (pg.X >= rcCanvas.Right - RightChartSpace && pg.X <= rcCanvas.Right)
                return ObjectFromCursor.PanelRightNonPaintableArea;

            return ObjectFromCursor.PanelPaintableArea;
        }

        /// <summary>
        /// deletes all panels and everyting related to them
        /// </summary>
        public void ClearAll()
        {
            _status = ChartStatus.Building;
            _dataManager.ClearAll();
            if (_panelsContainer != null)
                _panelsContainer.ClearAll();
            _xMap = new double[0];
            _xCount = 0;
            _xGridMap.Clear();
            _startIndex = _endIndex = 0;
            _barBrushes.Clear();

            FireChartReseted();
            _status = ChartStatus.Ready;
        }

        /// <summary>
        /// Adds an OHLC group of series to the chart
        /// </summary>
        /// <param name="groupName">Group name</param>
        /// <param name="panelIndex">Panel index where to place OHLC group of series</param>
        /// <returns>An array with length = 4 that containes references to newly create series</returns>
        public Series[] AddOHLCSeries(string groupName, int panelIndex)
        {
            ChartPanel chartPanel = GetPanelByIndex(panelIndex);
            if (chartPanel == null)
                throw new IndexOutOfRangeException("panelIndex");
            _dataManager.AddOHLCSeries(groupName);

            Series[] series = new Series[4];
            series[0] = chartPanel.CreateSeries(groupName, SeriesTypeOHLC.Open, SeriesTypeEnum.stCandleChart);
            series[1] = chartPanel.CreateSeries(groupName, SeriesTypeOHLC.High, SeriesTypeEnum.stCandleChart);
            series[2] = chartPanel.CreateSeries(groupName, SeriesTypeOHLC.Low, SeriesTypeEnum.stCandleChart);
            series[3] = chartPanel.CreateSeries(groupName, SeriesTypeOHLC.Close, SeriesTypeEnum.stCandleChart);
            foreach (var series1 in series)
            {
                _dataManager.BindSeries(series1);
            }
            return series;
        }

        /// <summary>
        /// Adds an HLC group of series to the chart
        /// </summary>
        /// <param name="groupName">Group Name</param>
        /// <param name="panelIndex">Panel index where to place</param>
        /// <returns>An array with length = 3 that has references to all 3 series</returns>
        public Series[] AddHLCSeries(string groupName, int panelIndex)
        {
            ChartPanel chartPanel = GetPanelByIndex(panelIndex);
            if (chartPanel == null)
                throw new IndexOutOfRangeException("panelIndex");

            _dataManager.AddHLCSeries(groupName);

            Series[] series = new Series[3];
            series[0] = chartPanel.CreateSeries(groupName, SeriesTypeOHLC.High, SeriesTypeEnum.stCandleChart);
            series[1] = chartPanel.CreateSeries(groupName, SeriesTypeOHLC.Low, SeriesTypeEnum.stCandleChart);
            series[2] = chartPanel.CreateSeries(groupName, SeriesTypeOHLC.Close, SeriesTypeEnum.stCandleChart);
            foreach (var series1 in series)
            {
                _dataManager.BindSeries(series1);
            }

            return series;
        }

        /// <summary>
        /// Adds volume type of series to the chart
        /// </summary>
        /// <param name="groupName">Group name</param>
        /// <param name="panelIndex">Panel index where to place</param>
        /// <returns>Reference to volume series</returns>
        public Series AddVolumeSeries(string groupName, int panelIndex)
        {
            ChartPanel chartPanel = GetPanelByIndex(panelIndex);
            if (chartPanel == null)
                throw new IndexOutOfRangeException("panelIndex");

            _dataManager.AddSeries(groupName, SeriesTypeOHLC.Volume);

            Series series = chartPanel.CreateSeries(groupName, SeriesTypeOHLC.Volume, SeriesTypeEnum.stVolumeChart);
            _dataManager.BindSeries(series);
            return series;
        }

        /// <summary>
        /// Adds a line type of series to the chart
        /// </summary>
        /// <param name="symbolName">Symbol name. Series name get's created from symbol name and SeriesOHLCType</param>
        /// <param name="panelIndex">Panel index where to place</param>
        /// <param name="ohlcType">OHLC type</param>
        /// <returns>Reference to the newly created series</returns>
        public Series AddLineSeries(string symbolName, int panelIndex, SeriesTypeOHLC ohlcType)
        {
            ChartPanel chartPanel = GetPanelByIndex(panelIndex);
            if (chartPanel == null)
                throw new IndexOutOfRangeException("panelIndex");

            _dataManager.AddSeries(symbolName, ohlcType);

            Series series = chartPanel.CreateSeries(symbolName, ohlcType, SeriesTypeEnum.stLineChart);
            _dataManager.BindSeries(series);
            return series;
        }


        ///<summary>
        /// Adds a linear type of series with an arbitrary series name, not binded to the chart's symbol
        ///</summary>
        ///<param name="seriesName">Series name</param>
        ///<param name="panelIndex">Panel index where to place</param>
        ///<returns>Reference to the newly created series</returns>
        ///<exception cref="IndexOutOfRangeException"></exception>
        public Series AddSeries(string seriesName, int panelIndex)
        {
            ChartPanel chartPanel = GetPanelByIndex(panelIndex);
            if (chartPanel == null)
                throw new IndexOutOfRangeException("panelIndex");

            _dataManager.AddSeries(seriesName, SeriesTypeOHLC.Unknown);

            Series series = chartPanel.CreateSeries(seriesName, SeriesTypeOHLC.Unknown, SeriesTypeEnum.stLineChart);
            _dataManager.BindSeries(series);

            return series;
        }

        ///<summary>
        /// Appends values for an OHLC group
        ///</summary>
        ///<param name="symbolName">Symbol name. Usually main symbol</param>
        ///<param name="timeStamp">Value's timestamp</param>
        ///<param name="open">Open value</param>
        ///<param name="high">High value</param>
        ///<param name="low">Low value</param>
        ///<param name="close">Close value</param>
        public void AppendOHLCValues(string symbolName, DateTime timeStamp, double? open, double? high, double? low, double? close)
        {
            _dataManager.AppendOHLCValues(symbolName, timeStamp, open, high, low, close);
        }

        ///<summary>
        /// Appends values for an HLC group
        ///</summary>
        ///<param name="groupName">Group name. Usually main symbol</param>
        ///<param name="timeStamp">Value's timestamp</param>
        ///<param name="high">High value</param>
        ///<param name="low">Low value</param>
        ///<param name="close">Close value</param>
        public void AppendHLCValues(string groupName, DateTime timeStamp, double? high, double? low, double? close)
        {
            _dataManager.AppendHLCValues(groupName, timeStamp, high, low, close);
        }

        ///<summary>
        /// Appends Volume value to the chart
        ///</summary>
        ///<param name="groupName">Group name. Usually main symbol</param>
        ///<param name="timeStamp">Value's timestamp</param>
        ///<param name="volume">Volume value</param>
        public void AppendVolumeValue(string groupName, DateTime timeStamp, double? volume)
        {
            _dataManager.AppendValue(groupName, SeriesTypeOHLC.Volume, timeStamp, volume);
        }

        /// <summary>
        /// Appends value for linear type of series. 
        /// </summary>
        /// <param name="symbolName">Symbol name. Series name gets created from symbol name and SeriesTypeOHLC</param>
        /// <param name="ohlcType">OHLC type</param>
        /// <param name="timeStamp">Time stamp</param>
        /// <param name="value">Value</param>
        public void AppendValue(string symbolName, SeriesTypeOHLC ohlcType, DateTime timeStamp, double? value)
        {
            _dataManager.AppendValue(symbolName, ohlcType, timeStamp, value);
        }

        /// <summary>
        /// Appends value 
        /// </summary>
        /// <param name="seriesName">Full series name</param>
        /// <param name="timeStamp">Time stamp</param>
        /// <param name="value">Value</param>
        public void AppendValue(string seriesName, DateTime timeStamp, double? value)
        {
            _dataManager.AppendValue(seriesName, SeriesTypeOHLC.Unknown, timeStamp, value);
        }

        ///<summary>
        /// Adds a tick value to the chart. Make sure chart has Tick type
        ///</summary>
        ///<param name="symbolName">Symbol name</param>
        ///<param name="timeStamp">Time stamp</param>
        ///<param name="lastPrice">Last price value</param>
        ///<param name="lastVolume">Last Volume value</param>
        public void AppendTickValue(string symbolName, DateTime timeStamp, double lastPrice, double lastVolume)
        {
            _dataManager.AppendTickValue(symbolName, timeStamp, lastPrice, lastVolume);
        }

        ///<summary>
        /// Edit a value for a given series at a specified position
        ///</summary>
        ///<param name="seriesName">Series name</param>
        ///<param name="timeStamp">Time stamp where to edit</param>
        ///<param name="newValue">New value</param>
        public void EditValue(string seriesName, DateTime timeStamp, double? newValue)
        {
            EditValue(GetSeriesByName(seriesName), timeStamp, newValue);
        }

        ///<summary>
        /// Edit a value for a given series at a specified position
        ///</summary>
        ///<param name="series">Reference to a series</param>
        ///<param name="timeStamp">Time stamp where to edit</param>
        ///<param name="newValue">New value</param>
        public void EditValue(Series series, DateTime timeStamp, double? newValue)
        {
            int valueIndex = _dataManager.GetTimeStampIndex(timeStamp);
            if (valueIndex == -1) return;
            if (series != null)
                series[valueIndex].Value = newValue;
        }

        ///<summary>
        /// Edit a value for a given series at a specified by index position
        ///</summary>
        ///<param name="seriesName">Series name</param>
        ///<param name="valueIndex">Value index</param>
        ///<param name="newValue">New value</param>
        public void EditValueByRecord(string seriesName, int valueIndex, double? newValue)
        {
            Series series = GetSeriesByName(seriesName);
            if (series != null)
                series[valueIndex].Value = newValue;
        }

        ///<summary>
        /// Edit a value for a given series at a specified by index position
        ///</summary>
        ///<param name="series">Reference to a series</param>
        ///<param name="valueIndex">Value index</param>
        ///<param name="newValue">New value</param>
        public void EditValueByRecord(Series series, int valueIndex, double? newValue)
        {
            series[valueIndex].Value = newValue;
        }

        ///<summary>
        /// Gets a value from a series by index
        ///</summary>
        ///<param name="seriesName">Series name</param>
        ///<param name="valueIndex">Record index</param>
        ///<returns>Value</returns>
        public double? GetValue(string seriesName, int valueIndex)
        {
            Series series = GetSeriesByName(seriesName);
            return series != null ? series[valueIndex].Value : null;
        }

        ///<summary>
        /// Gets a value from a series by index
        ///</summary>
        ///<param name="series">Reference to a series</param>
        ///<param name="valueIndex">Record index</param>
        ///<returns>Value</returns>
        public double? GetValue(Series series, int valueIndex)
        {
            return series[valueIndex].Value;
        }

        ///<summary>
        /// Gets a value from a series by time stamp
        ///</summary>
        ///<param name="seriesName">Series name</param>
        ///<param name="timeStamp">Time stamp</param>
        ///<returns>Value</returns>
        public double? GetValue(string seriesName, DateTime timeStamp)
        {
            int valueIndex = _dataManager.GetTimeStampIndex(timeStamp);
            return valueIndex == -1 ? null : GetValue(seriesName, valueIndex);
        }

        ///<summary>
        /// Gets a value from a series by time stamp
        ///</summary>
        ///<param name="series">Reference to a series</param>
        ///<param name="timeStamp">Time stamp</param>
        ///<returns>Value</returns>
        public double? GetValue(Series series, DateTime timeStamp)
        {
            int valueIndex = _dataManager.GetTimeStampIndex(timeStamp);
            return valueIndex == -1 ? null : GetValue(series, valueIndex);
        }

        ///<summary>
        /// Gets the index of a given timeStamp
        ///</summary>
        ///<param name="timeStamp">Time stamp to check</param>
        ///<returns>Index (0-based)</returns>
        public int GetTimeStampIndex(DateTime timeStamp)
        {
            return _dataManager.GetTimeStampIndex(timeStamp);
        }

        ///<summary>
        /// Forces to recalculate indicators and their repainting
        ///</summary>
        public void RecalculateIndicators()
        {
            foreach (var indicator in _panelsContainer.Panels
                                                .SelectMany(panel => panel.IndicatorsCollection))
            {
                indicator.Painted = false;
                indicator._calculated = false;
                indicator.Calculate();
                indicator.Paint();
            }
        }

        /// <summary>
        /// Represents the periodicity used to compress tick data
        /// it is either value in 
        /// 1. number of ticks - when compression type is ticks
        /// 2. number of seconds - when compression type is time
        /// </summary>
        public int TickPeriodicity
        {
            get { return _dataManager.TickPeriodicity; }
            set { _dataManager.TickPeriodicity = value; }
        }

        /// <summary>
        /// type of compression used to compress ticks.
        /// </summary>
        public TickCompressionEnum TickCompressionType
        {
            get { return _dataManager.TickCompressionType; }
            set { _dataManager.TickCompressionType = value; }
        }

        /// <summary>
        /// Compress internally stored ticks, by taking into consideration the properties <see cref="TickPeriodicity"/> and <see cref="TickCompressionType"/>
        /// </summary>
        public void CompressTicks()
        {
            if (_dataManager != null)
                _dataManager.ReCompressTicks();
        }

        /// <summary>
        /// Removes a series object from the chart.
        /// </summary>
        /// <param name="series">Reference to a series to remove</param>
        public void RemoveSeries(Series series)
        {
            if (FireIndicatorBeforeDelete(series))
            {
                return;
            }

            if (series is TwinIndicator)
            {
                series = ((TwinIndicator)series)._indicatorParent;
            }

            foreach (Series linkedSeries in series._linkedSeries)
            {
                linkedSeries._recycleFlag = true;
            }

            series._recycleFlag = true;

            ReCalc = true;
            Update();
        }

        /// <summary>
        /// Removes an object from the chart
        /// </summary>
        /// <param name="lineStudy">Reference to linestudy to remove</param>
        public void RemoveObject(LineStudy lineStudy)
        {
            if (FireLineStudyBeforeDelete(lineStudy))
            {
                return;
            }

            lineStudy._chartPanel._lineStudies.Remove(lineStudy);
            lineStudy.Selected = false;
            lineStudy.RemoveLineStudy();
        }

        /// <summary>
        /// Removes an object from the chart with the specified object type and Key.
        /// </summary>
        /// <param name="objectKey">Object key</param>
        public void RemoveObject(string objectKey)
        {
            LineStudy lineStudy = GetLineStudy(objectKey);
            if (lineStudy == null)
            {
                return;
            }

            RemoveObject(lineStudy);
        }

        /// <summary>
        /// Sets the min and max values for the panel. this values will be ussed instead of min &amp; max
        /// of series from this panel
        /// </summary>
        /// <param name="panelIndex">Panel index</param>
        /// <param name="max">Max Y price value</param>
        /// <param name="min">Min Y price value</param>
        public void SetYScale(int panelIndex, double max, double min)
        {
            if (panelIndex >= PanelsCount)
                return;

            ChartPanel chartPanel = _panelsContainer.Panels[panelIndex];
            chartPanel._minChanged = min;
            chartPanel._maxChanged = max;
            chartPanel._staticYScale = true;
            //chartPanel.Paint();
        }

        /// <summary>
        /// Resets the min &amp; max values. 
        /// </summary>
        /// <param name="panelIndex">Panel idnex</param>
        public void ResetYScale(int panelIndex)
        {
            if (panelIndex >= PanelsCount)
                return;

            ChartPanel chartPanel = _panelsContainer.Panels[panelIndex];
            chartPanel.ResetYScale();
        }

#if WPF
		///<summary>
		/// A helper function to show an internal color dialog
		///</summary>
		///<returns>Color choosed, or null if user pressed cancel</returns>
		public Color? ShowColorDialog()
		{
			ColorPickerDialog dlg = new ColorPickerDialog
																{
																	Owner = OwnerWindow
																};

			if (dlg.ShowDialog() == false)
				return null;

			return dlg.SelectedColor;
		}
#endif

        /// <summary>
        /// Sets the height of a given panel.
        ///  </summary>
        /// <param name="panelIndex">0 based index of the panel</param>
        /// <param name="newHeight">New height</param>
        public void SetPanelHeight(int panelIndex, double newHeight)
        {
            if (panelIndex >= PanelsCount)
            {
                return;
            }

            ChartPanel chartPanel = _panelsContainer.Panels[panelIndex];
            if (chartPanel.State != ChartPanel.StateType.Normal)
            {
                return;
            }

            if (newHeight > _panelsContainer.ActualHeight)
            {
                return;
            }

            _panelsContainer.ResizePanelByHeight(chartPanel, newHeight);
        }

        /// <summary>
        /// Move series from its current location to a new panel
        /// if series is a part of OHLC or HLC group entire group will be moved
        /// </summary>
        /// <param name="seriesName">Series na,e</param>
        /// <param name="toPanelIndex">New panel index</param>
        public void MoveSeries(string seriesName, int toPanelIndex)
        {
            Series series = GetSeriesByName(seriesName);
            if (series == null) return;
            if (series._chartPanel.Index == toPanelIndex) return;

            ChartPanel fromPanel = series._chartPanel;
            ChartPanel toPanel = GetPanelByIndex(toPanelIndex);

            fromPanel.MoveSeriesTo(series, toPanel, MoveSeriesIndicator.MoveStatusEnum.MoveToExistingPanel);
        }

        /// <summary>
        /// Gets x-pixel coordinate by record index
        /// </summary>
        /// <param name="index">Record index</param>
        /// <returns>X pixel</returns>
        public double GetXPixel(double index)
        {
            return GetXPixel(index, false);
        }

        /// <summary>
        /// Gets the pixel location of a price at the specified record. Pixel will be located in panel with index 0
        /// </summary>
        /// <param name="priceValue">Price value</param>
        /// <returns>Record index</returns>
        public double? GetYPixel(double priceValue)
        {
            return GetYPixel(priceValue, 0);
        }

        /// <summary>
        /// Gets the pixel location of a price at the specified record. 
        /// </summary>
        /// <param name="priceValue">Price value</param>
        /// <param name="panelIndex">Panel Index</param>
        /// <returns>Pixel value</returns>
        public double? GetYPixel(double priceValue, int panelIndex)
        {
            if (_panelsContainer.Panels.Count == 0)
            {
                return null;
            }

            ChartPanel chartPanel = _panelsContainer.Panels[panelIndex];
            if (chartPanel._state != ChartPanel.StateType.Normal)
            {
                return null;
            }

            return chartPanel.GetY(priceValue);
        }

        /// <summary>
        /// Removes a series object from the chart
        /// </summary>
        /// <param name="seriesName">Series name</param>
        public void RemoveSeries(string seriesName)
        {
            ChartPanel chartPanel = GetPanelBySeriesName(seriesName);
            if (chartPanel == null) return;
            Series series = GetSeriesByName(seriesName);
            if (series == null) return;
            chartPanel.RemoveSeries(series, true);
            series._recycleFlag = true;
            Update();
        }

        /// <summary>
        /// This method Gets the panel reference that contains the specified series. This method is useful 
        /// because users may drag series from one chart panel to another, or delete the series entirely.
        /// </summary>
        /// <param name="seriesName">Series name</param>
        /// <returns>Reference to a series</returns>
        public ChartPanel GetPanelBySeriesName(string seriesName)
        {
            Series series = GetSeriesByName(seriesName);
            return series == null ? null : series._chartPanel;
        }

        /// <summary>
        /// Scrolls the chart to the left by the specified amount of Records. 
        /// If the chart is already scrolled to the maximum level, this method will have no effect. 
        /// </summary>
        /// <param name="records">Number of records</param>
        public void ScrollLeft(int records)
        {
            if (_status != ChartStatus.Ready)
            {
                return;
            }

            if (_startIndex - records > 0)
            {
                _startIndex -= records;
                _endIndex -= records;
            }
            else
            {
                int oldStartIndex = _startIndex;
                _startIndex = 0;
                _endIndex -= oldStartIndex;
            }

            OnPropertyChanged(Property_StartIndex);
            OnPropertyChanged(Property_EndIndex);

            Update();
            FireChartScroll();
        }

        /// <summary>
        /// Scrolls the chart to the right by the specified amount of Records. 
        /// If the chart is already scrolled to the maximum level, this method will have no effect.
        /// </summary>
        /// <param name="records">Number of records</param>
        public void ScrollRight(int records)
        {
            if (_status != ChartStatus.Ready)
            {
                return;
            }

            if (_endIndex + records <= RecordCount)
            {
                _startIndex += records;
                _endIndex += records;
            }
            else
            {
                _startIndex += RecordCount - _endIndex;
                _endIndex = RecordCount;
            }

            OnPropertyChanged(Property_StartIndex);
            OnPropertyChanged(Property_EndIndex);

            Update();
            FireChartScroll();
        }

        /// <summary>
        /// Clears all values for the series object specified by the Name argument. E.g. if you have a series named 
        /// "my series" and have previously inserted data into that series you can erase all data in that series and 
        /// start over by calling ClearValues. This is easier than calling RemoveSeries and AddSeries again, or RemoveAllSeries. 
        /// You can also clear ALL series values via the ClearAllSeries function.
        /// </summary>
        /// <param name="seriesName">Series name</param>
        public void ClearValues(string seriesName)
        {
            Series series = GetSeriesByName(seriesName);
            if (series == null) return;
            ClearValues(series);
        }

        /// <summary>
        /// Clears all values for the series object specified by the Name argument. E.g. if you have a series named 
        /// "my series" and have previously inserted data into that series you can erase all data in that series and 
        /// start over by calling ClearValues. This is easier than calling RemoveSeries and AddSeries again, or ClearAll(). 
        /// You can also clear ALL series values via the ClearAllSeries function.
        /// </summary>
        /// <param name="series">Reference to a series</param>
        public void ClearValues(Series series)
        {
            _dataManager.ClearValues(series.SeriesIndex);
            _recalc = true;
        }

        /// <summary>
        /// Clears all values from all series on the chart. To clear values from one series only, use the 
        /// ClearValues function instead. To remove series, use the ClearAll() function.
        /// </summary>
        public void ClearAllSeries()
        {
            _dataManager.ClearData();
            _startIndex = _endIndex = 0;
        }

        /// <summary>
        /// Gets a reference to a lineStudy (that also includes buy, sell, ... symbols)
        /// </summary>
        /// <param name="objectKey">Object key</param>
        /// <returns>Reference to a line study</returns>
        public LineStudy GetLineStudy(string objectKey)
        {
            return _panelsContainer.Panels
                .SelectMany(panel => panel._lineStudies)
                .FirstOrDefault(lineStudy => lineStudy.Key == objectKey);
        }

        /// <summary>
        /// Gets a symbol (bmp) object, line object, or text object's start record number.
        /// </summary>
        /// <param name="objectKey">Object key</param>
        /// <returns>Start records</returns>
        public double? GetObjectStartRecord(string objectKey)
        {
            LineStudy lineStudy = GetLineStudy(objectKey);
            return lineStudy != null ? lineStudy.X1Value : (double?)null;
        }

        /// <summary>
        /// Gets a symbol (bitmap) object, line object, or text object's end record number.
        /// </summary>
        /// <param name="objectKey">Object key</param>
        /// <returns>End Record</returns>
        public double? GetObjectEndRecord(string objectKey)
        {
            LineStudy lineStudy = GetLineStudy(objectKey);
            return lineStudy != null ? lineStudy.X2Value : (double?)null;
        }

        /// <summary>
        /// Gets a symbol (bmp) object, line object, or text object's start price value.
        /// </summary>
        /// <param name="objectKey">Object key</param>
        /// <returns>Start value</returns>
        public double? GetObjectStartValue(string objectKey)
        {
            LineStudy lineStudy = GetLineStudy(objectKey);
            return lineStudy != null ? lineStudy.Y1Value : (double?)null;
        }

        /// <summary>
        /// Gets a symbol (bmp) object, line object, or text object's end price value.
        /// </summary>
        /// <param name="objectKey">Object key</param>
        /// <returns>End value</returns>
        public double? GetObjectEndValue(string objectKey)
        {
            LineStudy lineStudy = GetLineStudy(objectKey);
            return lineStudy != null ? lineStudy.Y2Value : (double?)null;
        }

        /// <summary>
        /// Sets a symbol (bmp) object, line object, or text ojbect's start record, end record, start value, and end value. 
        /// </summary>
        /// <param name="objectKey">Object key</param>
        /// <param name="startRecord">Start record</param>
        /// <param name="startValue">Start value</param>
        /// <param name="endRecord">End Record</param>
        /// <param name="endValue">End Value</param>
        /// <returns>Reference to a line study</returns>
        public LineStudy SetObjectPosition(string objectKey, int startRecord, double startValue, int endRecord, double endValue)
        {
            LineStudy lineStudy = GetLineStudy(objectKey);
            if (lineStudy == null) return null;
            lineStudy.SetXYValues(startRecord, startValue, endRecord, endValue);
            return lineStudy;
        }

        /// <summary>
        /// Gets the total number of objects on the chart that match the specified type. 
        /// </summary>
        /// <param name="studyTypeEnum">Study type</param>
        /// <returns>Number</returns>
        public int GetObjectCount(LineStudy.StudyTypeEnum studyTypeEnum)
        {
            return _panelsContainer.Panels
                .SelectMany(panel => panel._lineStudies)
                .Count(study => study.StudyType == studyTypeEnum);
        }

        /// <summary>
        /// sets a custom background brush for a specified by index bar
        /// </summary>
        /// <param name="barIndex">Bar index</param>
        /// <param name="customBrush">A new background brush</param>
        public void BarBrush(int barIndex, Brush customBrush)
        {
            BarBrush(string.Empty, barIndex, customBrush);
        }

        /// <summary>
        /// sets a custom background brush for a specified by index bar
        /// </summary>
        /// <param name="seriesName"></param>
        /// <param name="barIndex">Bar index</param>
        /// <param name="customBrush">A new background brush</param>
        public void BarBrush(string seriesName, int barIndex, Brush customBrush)
        {
            Dictionary<int, BarBrushData> o;
            if (!_barBrushes.TryGetValue(seriesName, out o))
                _barBrushes[seriesName] = (o = new Dictionary<int, BarBrushData>());

            o[barIndex] = new BarBrushData
                                                                {
                                                                    Brush = customBrush != null ? (Brush)customBrush.GetAsFrozen() : null,
                                                                    Changed = true
                                                                };
            if (OnCandleCustomBrush != null)
                OnCandleCustomBrush(seriesName, barIndex - _startIndex, customBrush);
        }

        /// <summary>
        /// Gets the bar brush or null if bar has no user-defined brush
        /// </summary>
        /// <param name="barIndex">Bar index</param>
        /// <returns>Brush used</returns>
        public Brush BarBrush(int barIndex)
        {
            return BarBrush(string.Empty, barIndex);
        }

        /// <summary>
        /// Gets the bar brush or null if bar has no user-defined brush
        /// </summary>
        /// <param name="seriesName"></param>
        /// <param name="barIndex">Bar index</param>
        /// <returns>Brush used</returns>
        public Brush BarBrush(string seriesName, int barIndex)
        {
            Dictionary<int, BarBrushData> o;
            if (!_barBrushes.TryGetValue(seriesName, out o))
                _barBrushes[seriesName] = (o = new Dictionary<int, BarBrushData>());
            BarBrushData outBrush;
            return o.TryGetValue(barIndex, out outBrush) ? o[barIndex].Brush : null;
        }

        /// <summary>
        /// Sets the line color for the series as specified by the Name argument. This property may not apply to certain price styles. 
        /// </summary>
        /// <param name="seriesName">Series name</param>
        /// <param name="seriesColor">Series color</param>
        public void SeriesColor(string seriesName, Color seriesColor)
        {
            Series series = GetSeriesByName(seriesName);
            if (series == null) return;
            series.StrokeColor = seriesColor;
        }

        /// <summary>
        /// Sets the line color for the series as specified by the Name argument
        /// </summary>
        /// <param name="seriesName">Series name</param>
        /// <returns>Color used, or null if such series doesn't exist</returns>
        public Color? SeriesColor(string seriesName)
        {
            Series series = GetSeriesByName(seriesName);
            return series == null ? (Color?)null : series.StrokeColor;
        }

        /// <summary>
        /// sets the stroke color for a given object 
        /// </summary>
        /// <param name="objectKey">Object key</param>
        /// <param name="newColor">New color</param>
        public void ObjectColor(string objectKey, Color newColor)
        {
            LineStudy lineStudy = GetLineStudy(objectKey);
            if (lineStudy == null) return;
            lineStudy.Stroke = new SolidColorBrush(newColor);
        }

        /// <summary>
        /// Retrieves the object color 
        /// </summary>
        /// <param name="objectKey">Object key</param>
        /// <returns>Color or null if such object doesn't exist</returns>
        public Color? ObjectColor(string objectKey)
        {
            LineStudy lineStudy = GetLineStudy(objectKey);
            return lineStudy == null ? (Color?)null : ((SolidColorBrush)lineStudy.Stroke).Color;
        }

        /// <summary>
        /// If this property is set to False, the user will not be able to select the object with the mouse. The object may be of any object type. 
        /// </summary>
        /// <param name="objectKey">Object key</param>
        /// <param name="selectAble">true - selectable, false - otherwise</param>
        public void ObjectSelectable(string objectKey, bool selectAble)
        {
            LineStudy lineStudy = GetLineStudy(objectKey);
            if (lineStudy == null) return;
            lineStudy.Selectable = selectAble;
        }

        /// <summary>
        /// A property that Gets says if an object is selectable, or null if such an object doesn't exists
        /// </summary>
        /// <param name="objectKey">Object key</param>
        /// <returns>true or false if object is selectable or not, or null if such objects doesn't exists</returns>
        public bool? ObjectSelectable(string objectKey)
        {
            LineStudy lineStudy = GetLineStudy(objectKey);
            return lineStudy == null ? (bool?)null : lineStudy.Selectable;
        }

        /// <summary>
        /// Sets the pen style of the specified object. 
        /// </summary>
        /// <param name="objectKey">Object key</param>
        /// <param name="objectStyle">Line pattern</param>
        public void ObjectStyle(string objectKey, LinePattern objectStyle)
        {
            LineStudy lineStudy = GetLineStudy(objectKey);
            if (lineStudy == null) return;
            lineStudy.StrokeType = objectStyle;
        }

        /// <summary>
        /// Gets the pen style of the specified object. 
        /// </summary>
        /// <param name="objectKey">Object key</param>
        /// <returns>Line pattern used, or null if such object doesn't exists</returns>
        public LinePattern? ObjectStyle(string objectKey)
        {
            LineStudy lineStudy = GetLineStudy(objectKey);
            return lineStudy == null ? (LinePattern?)null : lineStudy.StrokeType;
        }

        /// <summary>
        /// Sets the line weight of the object as specified by the object Name argument. 
        /// </summary>
        /// <param name="objectKey">Object key</param>
        /// <param name="objectWeight">New line thickness</param>
        public void ObjectWeight(string objectKey, double objectWeight)
        {
            LineStudy lineStudy = GetLineStudy(objectKey);
            if (lineStudy == null) return;
            lineStudy.StrokeThickness = objectWeight;
        }

        /// <summary>
        /// Gets the line weight of the object as specified by the object Name argument. 
        /// </summary>
        /// <param name="objectKey">Object key</param>
        /// <returns>Line thickness used, or null if such object doesn't exists</returns>
        public double? ObjectWeight(string objectKey)
        {
            LineStudy lineStudy = GetLineStudy(objectKey);
            return lineStudy == null ? 0 : lineStudy.StrokeThickness;
        }

        /// <summary>
        /// Sets the Text value of the object specified by object Name. Works only for StaticText type of objects
        /// </summary>
        /// <param name="objectKey">Object key</param>
        /// <param name="objectText">New text</param>
        public void ObjectText(string objectKey, string objectText)
        {
            LineStudy lineStudy = GetLineStudy(objectKey);
            if (lineStudy == null) return;
            ((StaticText)lineStudy).Text = objectText;
        }

        /// <summary>
        /// Gets the Text value of the object specified by object Name. Works only for StaticText type of objects
        /// </summary>
        /// <param name="objectKey">Object key</param>
        /// <returns>Object's text or null if such object doesn't exists</returns>
        public string ObjectText(string objectKey)
        {
            LineStudy lineStudy = GetLineStudy(objectKey);
            if (lineStudy == null) return null;
            StaticText staticText = lineStudy as StaticText;
            return staticText == null ? null : staticText.Text;
        }

        /// <summary>
        /// Gets the fontname for a StaticText object
        /// </summary>
        /// <param name="objectKey">Object key</param>
        /// <param name="fontName">New font name</param>
        public void TextAreaFontName(string objectKey, string fontName)
        {
            LineStudy lineStudy = GetLineStudy(objectKey);
            if (lineStudy == null) return;
            StaticText staticText = lineStudy as StaticText;
            if (staticText != null)
                staticText.FontName = fontName;
        }

        /// <summary>
        /// Gets the fontname for a StaticText object
        /// </summary>
        /// <param name="objectKey">Object key</param>
        /// <returns>Font name used</returns>
        public string TextAreaFontName(string objectKey)
        {
            LineStudy lineStudy = GetLineStudy(objectKey);
            if (lineStudy == null) return null;
            StaticText staticText = lineStudy as StaticText;
            return staticText == null ? null : staticText.FontName;
        }

        /// <summary>
        /// This function Gets the Indicator type for any indicator series added via the AddIndicator method.
        /// </summary>
        /// <param name="indicatorKey">Indicator's unique key</param>
        /// <returns>Indicator type</returns>
        public IndicatorType GetIndicatorType(string indicatorKey)
        {
            Series indicator = GetSeriesByName(indicatorKey);
            if (indicator == null) return IndicatorType.Unknown;
            Indicator indicator1 = indicator as Indicator;
            return indicator1 == null ? IndicatorType.Unknown : indicator1.IndicatorType;
        }

        /// <summary>
        /// Zooms the chart in by the specified amount of Records. If the chart is zoomed in all the way (meaning only one bar is visible), this method will have no effect. 
        /// </summary>
        /// <param name="records">Records to zoom in</param>
        public void ZoomIn(int records)
        {
            if (_status != ChartStatus.Ready) return;
            if (_endIndex - _startIndex - records > records)
            {
                _startIndex += records;
                _endIndex -= records;
            }
            Update();
            FireZoom();
        }

        /// <summary>
        /// Zooms the chart out by the specified amount of Records. This method will have no effect if the chart is zoomed out all the way.
        /// </summary>
        /// <param name="records">Records to zoom out</param>
        public void ZoomOut(int records)
        {
            if (_status != ChartStatus.Ready) return;

            int recCnt = RecordCount;
            if (_startIndex - records > -1)
            {
                _startIndex -= records;
                _endIndex += records;
            }
            else if (_priceStyle != PriceStyleEnum.psStandard && _endIndex < recCnt)
            {
                _endIndex += records;
            }
            else
            {
                _startIndex = 0;
            }
            if (_endIndex >= recCnt)
                _endIndex = recCnt - 1;

            Update();
            FireZoom();
        }

        /// <summary>
        /// Resets FirstVisibleRecord to 0 and LastVisibleRecord to RecordCount - 1, making the first and last bars visible on the chart. 
        /// </summary>
        public void ResetZoom()
        {
            _startIndex = 0;
            _endIndex = RecordCount;
            foreach (ChartPanel panel in PanelsCollection)
            {
                panel.ResetZoom();
            }
            Update();
            FireZoom();
        }

        /// <summary>
        /// Returns the Timestamp by its index value
        /// </summary>
        /// <param name="index">Record index for which timestamp is needed. Index is 0 based.</param>
        /// <returns>Timestamp if index value is ok, or null</returns>
        public DateTime? GetTimestampByIndex(int index)
        {
            DateTime result = _dataManager.GetTimeStampByIndex(index);
            if (result == DateTime.MinValue)
                return null;
            return result;
        }

        /// <summary>
        /// Gets record by pixel value
        /// </summary>
        /// <param name="pixel"></param>
        /// <returns></returns>
        public int GetReverseX(double pixel)
        {
            return (int)GetReverseXInternal(pixel + _barSpacing + _barWidth / 2);
        }

        /// <summary>
        /// Returns an aproximated record index for a given timeStamp.
        /// Usefull when an exact timestamp is unknown
        /// </summary>
        /// <param name="timestamp"></param>
        /// <param name="roundUp">
        /// if true - returns the next recordCount that has a value greater then given timestamp
        /// if false - returns the previos recordCount that has a value less then given timestamp
        /// </param>
        /// <returns>Timestamp index, -1 if no such aproximate value</returns>
        public int GetReverseX(DateTime timestamp, bool roundUp)
        {
            if (_dataManager == null)
                return -1;
            return _dataManager.GetTimeStampIndex(timestamp, roundUp);
        }

        /// <summary>
        /// Appends OHLC values and updates/replaces the last bar on new ticks.
        /// </summary>
        /// <param name="symbolName"></param>
        /// <param name="timeStamp"></param>
        /// <param name="open"></param>
        /// <param name="high"></param>
        /// <param name="low"></param>
        /// <param name="close"></param>
        /// <param name="volume"></param>
        /// <param name="isPartial"></param>
        public void AppendOHLCVValues(string symbolName, DateTime timeStamp, double? open, double? high, double? low, double? close, double? volume, bool isPartial)
        {
            _dataManager.AppendOHLCVValues(symbolName, timeStamp, open, high, low, close, volume, isPartial);
        }


        /// <summary>
        /// Offsets all the timestamp entries in the chart by a given offset.
        /// </summary>
        /// <param name="offset"></param>
        public void OffsetTimeStamps(TimeSpan offset)
        {
            if (_dataManager != null)
            {
                _dataManager.OffsetTimestamps(offset);
                Update();
            }
        }

        /// <summary>
        /// Delete all the bars from the chart whose timestamp is contained in the given array of timestamps.
        /// </summary>
        /// <param name="timestamps">Array of timestamps</param>
        /// <returns>Number found and deleted records.</returns>
        public int DeleteTimestamps(IEnumerable<DateTime> timestamps)
        {
            int cnt = 0;
            if (_dataManager != null)
            {
                cnt = _dataManager.DeleteTimestamps(timestamps);
                _endIndex -= cnt;

                Update();
            }

            return cnt;
        }
        #endregion
    }
}

