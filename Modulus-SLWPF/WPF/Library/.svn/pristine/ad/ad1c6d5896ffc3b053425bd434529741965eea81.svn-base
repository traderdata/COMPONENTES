using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using ModulusFE.Indicators;
using ModulusFE.LineStudies;
using ModulusFE.PaintObjects;
using FrameworkElement = System.Windows.FrameworkElement;
using Line = ModulusFE.PaintObjects.Line;
#if SILVERLIGHT
using ModulusFE.SL;
using ModulusFE.SL.Utils;
#endif

namespace ModulusFE
{
    /// <summary>
    /// ChartPanel - container for all series and line studies.
    /// </summary>
    [CLSCompliant(true)]
    public partial class ChartPanel : Control, INotifyPropertyChanged
    {
        internal static PropertyChangedEventArgs Property_YAxisResized = new PropertyChangedEventArgs("Property_YAxisResized");
        internal static PropertyChangedEventArgs Property_YAxisMoved = new PropertyChangedEventArgs("Property_YAxisMoved");
        internal static PropertyChangedEventArgs Property_PanelResized = new PropertyChangedEventArgs("Property_PanelResized");
        internal static PropertyChangedEventArgs Property_MinMaxChanged = new PropertyChangedEventArgs("Property_MinMax");
        internal static PropertyChangedEventArgs Property_SeriesIndexChanged = new PropertyChangedEventArgs("Property_SeriesIndex");


        /// <summary>
        /// Position type of the chart.
        /// </summary>
        public enum PositionType
        {
            /// <summary>
            /// Always top. Panel can't be moved under a panel that has no AlwaysTop type
            /// </summary>
            AlwaysTop,
            /// <summary>
            /// Always bottom - usually used for Volume panels
            /// </summary>
            AlwaysBottom,
            /// <summary>
            /// Arbitrary postion
            /// </summary>
            None
        }

        /// <summary>
        /// The current state of the panel. Usually used for Maximized, Minimized and Normal.
        /// </summary>
        public enum StateType
        {
            /// <summary>
            /// Standard State. A panel in the chart control
            /// </summary>
            Normal,

            /// <summary>
            /// When the panel takes over the entire control area
            /// </summary>
            Maximized,

            /// <summary>
            /// When the panel is minimized to the bottom bar
            /// </summary>
            Minimized,

            /// <summary>
            /// used when adding new panels, after this it becomes Normal
            /// </summary>
            New,

            /// <summary>
            /// Used internally
            /// </summary>
            Resizing,

            /// <summary>
            /// Used internally
            /// </summary>
            Moving
        }

        internal event EventHandler OnMinimizeClick;
        internal event EventHandler OnMaximizeClick;
        internal event EventHandler OnCloseClick;

        internal PositionType _position;
        internal StockChartX _chartX;
        internal bool _allowDelete;
        internal bool _allowMaxMin;
        internal int _index;
        internal bool _hasPrice;
        internal bool _hasVolume;
        internal bool _enforceSeriesSetMinMax;
        internal Canvas _rootCanvas;
        internal ChartPanelTitleBar _titleBar;
        internal PanelsContainer _panelsContainer;
        /// <summary>
        /// remembers the minimized rectangle, used for restoring panel with animation
        /// </summary>
        internal Rect _minimizedRect;

        internal double _normalTopPosition;//used when restoring panel from maximized state to normal
        internal double _normalHeight;
        internal double _normalHeightPct; //the height on percents, used when restoring panel from minimized state

        private readonly ObservableCollection<Series> _series = new ObservableCollection<Series>();

        internal readonly ObservableCollection<SeriesTitleLabel> _seriesTitle = new ObservableCollection<SeriesTitleLabel>();

        internal readonly List<LineStudy> _lineStudies = new List<LineStudy>();
        internal readonly List<TrendLine> _trendWatch = new List<TrendLine>();
        internal LineStudy _lineStudyToAdd;

        internal LineStudy _lineStudySelected;
        internal Series _seriesSelected;

        internal YAxisCanvas _leftYAxis;
        internal YAxisCanvas _rightYAxis;

        internal StateType _state;

        private Grid _rootGrid;

        /// <summary>
        /// Actual min of all series (visible records only)
        /// </summary>
        private double _min;
        /// <summary>
        /// Actual max of all series (visible records only)
        /// </summary>
        private double _max;
        /// <summary>
        /// Minimum from all series if panel is not resized with mouse, otherwise keeps th given values
        /// </summary>
        internal double _minChanged;
        /// <summary>
        /// Maximum from all series if panel is not resized with mouse, otherwise keeps th given values
        /// </summary>
        internal double _maxChanged;

        private readonly PaintObjectsManager<Line> _gridXLines = new PaintObjectsManager<Line>();
        private readonly PaintObjectsManager<Line> _gridYLines = new PaintObjectsManager<Line>();
        /// <summary>
        /// used when moving panel up and down
        /// </summary>
        internal double _yOffset;

        internal bool _staticYScale;

        internal TextBlock _betaReminder;
        internal bool _recalc;

        private const string TimerMoveYAxes = "TimerMoveYAxes";
        private const string TimerResizeYAxes = "TimerResizeYAxes";
        private const string TimerLineStudiesNotVisibleChanged = "TimerLineStudiesNotVisibleChanged";
        internal const string TimerSizeChanged = "TimerSizeChanged";

        internal readonly ChartTimers _timers = new ChartTimers();

        internal bool _templateLoaded;
        /// <summary>
        /// when a panel gets created and a series is added to it its Template may not be loaded
        /// so, set a flag in Paint method and when template is loaded RePaint the panel
        /// </summary>
        internal bool _needRePaint;
        internal bool _painting;
        internal bool _isHeatMap;

        /// <summary>
        /// will ahve references to series that owns(shares) the Y scale
        /// </summary>
        internal readonly List<Series> _shareScaleSeries = new List<Series>();
        static ChartPanel()
        {
#if WPF
      DefaultStyleKeyProperty.OverrideMetadata(typeof(ChartPanel), new FrameworkPropertyMetadata(typeof(ChartPanel)));
#endif
            YAxesBackgroundProperty = DependencyProperty.Register("YAxesBackground", typeof(Brush), typeof(ChartPanel),
                                                                  new PropertyMetadata(Brushes.Black, OnYAxesBackgroundChanged));
        }

        ///<summary>
        ///</summary>
        public ChartPanel()
        {
#if SILVERLIGHT
            DefaultStyleKey = typeof(ChartPanel);
#endif

            InitPanel(null, PositionType.None);

            //ApplyTemplate();
        }

        internal ChartPanel(StockChartX chartX, PositionType positionType)
        {
#if SILVERLIGHT
            DefaultStyleKey = typeof(ChartPanel);
#endif
            InitPanel(chartX, positionType);

            //ApplyTemplate();
        }

        /// <summary>
        /// Init
        /// </summary>
        /// <param name="chartX"></param>
        /// <param name="positionType"></param>
        private void InitPanel(StockChartX chartX, PositionType positionType)
        {
            _chartX = chartX;
            _position = positionType;
            _allowDelete = true;
            _allowMaxMin = true;
            _state = StateType.Normal;

            if (_chartX != null && _chartX.OptimizePainting)
            {
                Background = new SolidColorBrush(Colors.Black);
            }
            else
            {
                Background = new LinearGradientBrush
                               {
                                   StartPoint = new Point(0.5, 0),
                                   EndPoint = new Point(0.5, 1),
                                   GradientStops = new GradientStopCollection
                                           {
                                             new GradientStop
                                               {
                                                 Color = Color.FromArgb(0xFF, 0x7F, 0x7F, 0x7F),
                                                 Offset = 0
                                               },
                                             new GradientStop
                                               {
                                                 Color = Color.FromArgb(0xFF, 0xBF, 0xBF, 0xBF),
                                                 Offset = 1
                                               }
                                           }
                               };
            }

            _timers.RegisterTimer(TimerMoveYAxes, MoveUpDown, 50);
            _timers.RegisterTimer(TimerResizeYAxes, ResizeUpDown, 50);
            _timers.RegisterTimer(TimerSizeChanged, Paint, 50);
            _timers.RegisterTimer(TimerLineStudiesNotVisibleChanged, ShowMoreSeriesIndicator, 500);

            _series.CollectionChanged += Series_OnCollectionChanged;
            if (_chartX != null)
            {
                _chartX.PropertyChanged += ChartX_OnPropertyChanged;
            }
        }

        private void ChartX_OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case StockChartX.Property_NewRecord:
                    WatchTrendLines();
                    break;
            }
        }

        internal void UpdateYAxesFontInformation()
        {
            if (_leftYAxis != null)
                _leftYAxis.UpdateFontInformation();
            if (_rightYAxis != null)
                _rightYAxis.UpdateFontInformation();
        }

        internal void ClearAll()
        {
            _series.CollectionChanged -= Series_OnCollectionChanged;
            _chartX.PropertyChanged -= ChartX_OnPropertyChanged;

            foreach (Series series in _series)
            {
                series.UnSubscribe();
            }

            _series.Clear();

            foreach (SeriesTitleLabel seriesTitleLabel in _seriesTitle)
            {
                seriesTitleLabel.UnSubscribe();
            }

            _seriesTitle.Clear();
            _lineStudies.Clear();
            _trendWatch.Clear();
        }

        private void Series_OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Remove:
                    foreach (var o in e.OldItems)
                    {
                        for (int i = 0; i < _seriesTitle.Count; i++)
                            if (_seriesTitle[i].Series == o)
                            {
                                _seriesTitle[i].UnSubscribe();
                                _seriesTitle.RemoveAt(i);
                                break;
                            }
                    }

                    _panelsContainer.ResetInfoPanelContent();
                    return;
                case NotifyCollectionChangedAction.Add:
                    foreach (Series series in e.NewItems.Cast<Series>())
                    {
                        if ((series._seriesType == SeriesTypeEnum.stCandleChart ||
                             series._seriesType == SeriesTypeEnum.stStockBarChart ||
                             series._seriesType == SeriesTypeEnum.stStockBarChartHLC))
                        {
                            if (series.OHLCType == SeriesTypeOHLC.Close)
                            {
                                _seriesTitle.Add(new SeriesTitleLabel(series));
                            }
                        }
                        else if (_chartX.IndicatorTwinTitleVisibility == Visibility.Collapsed &&
                            series._seriesType == SeriesTypeEnum.stIndicator &&
                            ((Indicator)series).IsTwin)
                        {
                            // Do nothing. (Hate this style of logic)
                        }
                        else
                        {
                            _seriesTitle.Add(new SeriesTitleLabel(series));
                        }
                    }

                    _panelsContainer.ResetInfoPanelContent();
                    break;
            }
        }

        public StateType State
        {
            get { return _state; }
            internal set
            {
                _state = value;
                if (_titleBar != null)
                {
                    _titleBar.PanelGotNewState(this);
                }
            }
        }

        internal double Top
        {
            get { return Canvas.GetTop(this); }
            set { Canvas.SetTop(this, value); }
        }

        internal double Left
        {
            get { return Canvas.GetLeft(this); }
            set { Canvas.SetLeft(this, value); }
        }

        internal Rect Bounds
        {
            get
            {
                return new Rect((double)GetValue(Canvas.LeftProperty), (double)GetValue(Canvas.TopProperty), Width, Height);
            }
            set
            {
                SetValue(Canvas.LeftProperty, value.Left);
                SetValue(Canvas.TopProperty, value.Top);
                Height = value.Height;
                Width = value.Width;
            }
        }


        private const string _reasonCantBeDeleted = "";
        internal bool CanBeDeleted
        {
            get { return _reasonCantBeDeleted.Length == 0; }
        }
        internal string ReasonCantBeDeleted
        {
            get { return _reasonCantBeDeleted; }
        }

        internal Rect CanvasRect
        {
            get
            {
                return new Rect(CanvasLeft, Canvas.GetTop(_rootCanvas), _rootCanvas.ActualWidth, _rootCanvas.ActualHeight);
            }
        }

        private double CanvasLeft
        {
            get
            {
                return _chartX.ScaleAlignment == ScaleAlignmentTypeEnum.Both ||
                       _chartX.ScaleAlignment == ScaleAlignmentTypeEnum.Left
                         ? Constants.YAxisWidth
                         : 0;
            }
        }

        //    internal new double Height
        //    {
        //      get { return base.Height; }
        //      set { base.Height = value; }
        //    }


        internal void SetYAxes()
        {
            if (_chartX == null) return;

            if (_leftYAxis != null)//we do this, cause this function is called from OnYAxesChanged, and in design mode they are null at this time
            {
                _leftYAxis.Visibility = (_chartX.ScaleAlignment == ScaleAlignmentTypeEnum.Both || _chartX.ScaleAlignment == ScaleAlignmentTypeEnum.Left)
                                          ? Visibility.Visible
                                          : Visibility.Collapsed;
                _rootGrid.ColumnDefinitions[0].Width = new GridLength(_leftYAxis.Visibility == Visibility.Visible ? Constants.YAxisWidth : 0);
            }

            if (_rightYAxis == null) return;
            {
                _rightYAxis.Visibility = (_chartX.ScaleAlignment == ScaleAlignmentTypeEnum.Both || _chartX.ScaleAlignment == ScaleAlignmentTypeEnum.Right)
                                         ? Visibility.Visible
                                         : Visibility.Collapsed;
            }

            _rootGrid.ColumnDefinitions[2].Width =
              new GridLength(_rightYAxis.Visibility == Visibility.Visible ? Constants.YAxisWidth : 0);
        }

        #region Overrides
        /// <summary>
        /// Ovveride
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _rootCanvas = GetTemplateChild("PART_RootCanvas") as Canvas;
            if (_rootCanvas == null)
            {
                throw new NullReferenceException();
            }
#if SILVERLIGHT
            Mouse.RegisterMouseMoveAbleElement(_rootCanvas);
            _rootCanvas.MouseMove += (sender, e) => Mouse.UpdateMousePosition(_rootCanvas, e.GetPosition(_rootCanvas));
#endif

#if WPF
      ToggleEdgeMode();
#endif

            _rootGrid = GetTemplateChild("rootGrid") as Grid;
            if (_rootGrid == null) throw new NullReferenceException();

            SetupTitleBar();

            _leftYAxis = GetTemplateChild("leftYAxis") as YAxisCanvas;
            if (_leftYAxis == null) throw new NullReferenceException();
            _rightYAxis = GetTemplateChild("rightYAxis") as YAxisCanvas;
            if (_rightYAxis == null) throw new NullReferenceException();
            _leftYAxis._chartPanel = _rightYAxis._chartPanel = this;
            _leftYAxis._isLeftAligned = true;
            _rightYAxis._isLeftAligned = false;
            SetYAxes();

            _rootCanvas.SizeChanged += RootCanvas_OnSizeChanged;
            _rootCanvas.MouseLeftButtonDown += RootCanvas_OnMouseLeftButtonDown;
            _rootCanvas.MouseLeftButtonUp += RootCanvas_OnMouseLeftButtonUp;
            _rootCanvas.MouseMove += RootCanvas_OnMouseMove;
#if WPF
      _rootCanvas.MouseRightButtonDown += RootCanvas_OnMouseRightButtonDown;
#endif

            if (_chartX != null && _chartX._isBeta)
            {
                _betaReminder = new TextBlock { FontSize = 18 };
                Canvas.SetZIndex(_betaReminder, ZIndexConstants.CrossHairs);
                _rootCanvas.Children.Add(_betaReminder);
            }

            SetLineStiduesClip();

            _templateLoaded = true;
        }

        private void SetupTitleBar()
        {
            _titleBar = GetTemplateChild("PART_TitleBar") as ChartPanelTitleBar;
            if (_titleBar == null)
            {
                throw new NullReferenceException("Title bar is a required PART from ChartPanel.");
            }

            _titleBar.Background = TitleBarBackground;

            _titleBar.MinimizeClick += (sender, e) => { if (OnMinimizeClick != null) OnMinimizeClick(this, EventArgs.Empty); };
            _titleBar.MaximizeClick += (sender, e) => { if (OnMaximizeClick != null) OnMaximizeClick(this, EventArgs.Empty); };
            _titleBar.CloseClick += (sender, e) => { if (OnCloseClick != null) OnCloseClick(this, EventArgs.Empty); };
#if SILVERLIGHT
            _titleBar.ApplyTemplate();
#endif

            _titleBar.SetChartPanel(this);
            _titleBar.LabelsDataSource = _seriesTitle;
            _titleBar.Visibility = _chartX == null ? Visibility.Visible : _chartX.DisplayTitles ? Visibility.Visible : Visibility.Collapsed;
            if (_titleBar.Visibility == Visibility.Collapsed)
            {
                _rootGrid.RowDefinitions[0].Height = new GridLength(0);
            }
            _titleBar.MaximizeBox = MaximizeBox;
            _titleBar.MinimizeBox = MinimizeBox;
            _titleBar.CloseBox = CloseBox;
        }

#if WPF
    private void ToggleEdgeMode()
    {
      if (_rootCanvas == null)
      {
        return;
      }

      _rootCanvas.SetValue(RenderOptions.EdgeModeProperty, UseAliasedEdgeMode ? EdgeMode.Aliased : EdgeMode.Unspecified);
    }
#endif

        private void SetTickBoxes()
        {
            foreach (Series series in _series)
            {
                series.CheckTickBoxNeedCreated();
            }
        }

        #endregion

        #region Root Canvas events
#if WPF
    private void RootCanvas_OnMouseRightButtonDown(object sender, MouseButtonEventArgs e)
    {
      if (_chartX.Status != StockChartX.ChartStatus.Ready)
      {
        return;
      }

      object element = _rootCanvas.HitTest(e.GetPosition(_rootCanvas), new[] {typeof(Series), typeof(LineStudy)});
      if (element == null)
      {
        return;
      }

      Series series = element as Series;
      if (series != null)
      {
        if (e.ClickCount == 1)
        {
          _chartX.FireSeriesRightClick(series, e.GetPosition(this));
        }

        return;
      }

      LineStudy lineStudy = element as LineStudy;
      if (lineStudy == null)
      {
        return;
      }

      if (e.ClickCount == 1)
      {
        _chartX.FireLineStudyRightClick(lineStudy, e.GetPosition(this));
      }
    }
#endif

        private bool _leftMouseDown;
        private Point _currentPoint;

        private void RootCanvas_OnMouseMove(object sender, MouseEventArgs e)
        {
            Point p = e.GetPosition(_rootCanvas);
            if (!_leftMouseDown)
            {
                if (_chartX.IsChartPanelMouseMoveHandled)
                {
                    _chartX.InvokeChartPanelMouseMove(Index, p.Y, p.X, GetReverseY(p.Y), _chartX.GetReverseX(p.X));
                }

                return;
            }

            switch (_chartX.Status)
            {
                case StockChartX.ChartStatus.LineStudyPainting:
                    _lineStudyToAdd.Paint(p.X, p.Y, LineStudy.LineStatus.Painting);
                    break;
                case StockChartX.ChartStatus.LineStudyMoving:
                    _lineStudySelected.Paint(p.X, p.Y, LineStudy.LineStatus.Moving);

                    _chartX.InvokeDragDropMoving(new StockChartX.DragDropLineStudyEventArgs(_lineStudySelected,
                      StockChartX.DragDropLineStudyEventArgs.DragDropActionType.Moving, false));
                    break;
                case StockChartX.ChartStatus.MovingSelection:
                    SeriesMoving(e);
                    break;
                case StockChartX.ChartStatus.Ready:
                    if (_lineStudySelected != null && _lineStudySelected.Selected)
                    {
                        if (_currentPoint.Distance(p) > 5)
                        {
                            var dragDropArgs = new StockChartX.DragDropLineStudyEventArgs(_lineStudySelected,
                                                                         StockChartX.DragDropLineStudyEventArgs.DragDropActionType.Started, true);
                            _chartX.InvokeDragDropStarted(dragDropArgs);

                            if (dragDropArgs.CancelAction)
                            {
                                return;
                            }

                            _lineStudySelected.Paint(p.X, p.Y, LineStudy.LineStatus.StartMove);
                            _chartX.Status = StockChartX.ChartStatus.LineStudyMoving;
                        }
                    }
                    else if (_seriesSelected != null && _seriesSelected.Selected && !_chartX._locked)
                    {
                        if (_currentPoint.Distance(p) > 5)
                        {
                            _chartX.Status = StockChartX.ChartStatus.MovingSelection;
                        }
                    }
                    else
                    {

                    }
                    break;
            }
        }

        private void RootCanvas_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Point p;

            _leftMouseDown = false;
            _rootCanvas.ReleaseMouseCapture();

            switch (_chartX.Status)
            {
                case StockChartX.ChartStatus.LineStudyPainting:
                    p = e.GetPosition(_rootCanvas);
                    _lineStudyToAdd.Paint(p.X, p.Y, LineStudy.LineStatus.EndPaint);
                    _chartX.Status = StockChartX.ChartStatus.Ready;
                    _lineStudies.Add(_lineStudyToAdd);

                    _chartX.FireUserDrawingComplete(_lineStudyToAdd.StudyType, _lineStudyToAdd.Key);

                    _lineStudyToAdd = null;
                    break;
                case StockChartX.ChartStatus.LineStudyMoving:
                    p = e.GetPosition(_rootCanvas);
                    _lineStudySelected.Paint(p.X, p.Y, LineStudy.LineStatus.EndMove);
                    _chartX.Status = StockChartX.ChartStatus.Ready;

                    _chartX.FireLineStudyLeftClick(_lineStudySelected);
                    _chartX.InvokeDragDropEnded(new StockChartX.DragDropLineStudyEventArgs(
                      _lineStudySelected, StockChartX.DragDropLineStudyEventArgs.DragDropActionType.Ended, false));
                    break;
                case StockChartX.ChartStatus.MovingSelection:
                    MoveSeriesTo(_seriesSelected, _chartPanelToMoveTo, _moveStatusEnum);
                    break;
                case StockChartX.ChartStatus.Ready:
                    if (_chartX.InfoPanelPosition == InfoPanelPositionEnum.FollowMouse)
                    {
                        _chartX.StopShowingInfoPanel();
                        _rootCanvas.ReleaseMouseCapture();
                    }
                    break;
            }
        }


#if SILVERLIGHT
        private DateTime _lastClick = DateTime.Now;
#endif
        private void RootCanvas_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point p;
#if WPF
      int clickCount = e.ClickCount;
#endif
#if SILVERLIGHT
            int clickCount = 1;
            TimeSpan span = DateTime.Now - _lastClick;
            if (span.TotalMilliseconds < 300)
                clickCount = 2;
            _lastClick = DateTime.Now;
#endif
            if (_chartX == null) return;

            _leftMouseDown = true;
            _currentPoint = e.GetPosition(_rootCanvas);

            switch (_chartX.Status)
            {
                case StockChartX.ChartStatus.LineStudyPaintReady:  //begin paiting a line study
                    if (_lineStudyToAdd == null) //user paints
                        _chartX.GetLineStudyToAdd(this);

                    _chartX.Status = StockChartX.ChartStatus.LineStudyPainting;
                    p = e.GetPosition(_rootCanvas);
                    if (_lineStudyToAdd != null)
                        _lineStudyToAdd.Paint(p.X, p.Y, LineStudy.LineStatus.StartPaint);
                    if (_lineStudySelected != null)
                    {
                        _chartX.LineStudySelectedCount--;
                        _lineStudySelected.Selected = false;
                        _lineStudySelected = null;
                    }
                    _rootCanvas.CaptureMouse();
                    break;
                case StockChartX.ChartStatus.Ready: //check for hitting a series or line studies
                    object element = _rootCanvas.HitTest(e.GetPosition(_rootCanvas),
                                                                   new[]
                                                           {
                                                             typeof(ContextLine), typeof(LineStudy), typeof(Series)
                                                           });
                    //Debug.WriteLine("HitTest " + element);

                    if (element == null)
                    {
                        if (_lineStudySelected != null)
                        {
                            _chartX.LineStudySelectedCount--;
                            _lineStudySelected.Selected = false;
                            _lineStudySelected = null;
                        }
                        if (_seriesSelected != null)
                        {
                            _seriesSelected.HideSelection();
                            _seriesSelected = null;
                        }
                    }
                    else
                    {
                        LineStudy lineStudy = element as LineStudy;
                        if (lineStudy != null && lineStudy.Selectable)
                        {
                            if (_lineStudySelected != null && lineStudy != _lineStudySelected)
                            {
                                _chartX.LineStudySelectedCount--;
                                _lineStudySelected.Selected = false;
                                _lineStudySelected = null;
                            }

                            if (lineStudy != _lineStudySelected)
                                _chartX.LineStudySelectedCount++;

                            _lineStudySelected = lineStudy;
                            _lineStudySelected.Selected = true;

                            if (_seriesSelected != null)
                            {
                                _seriesSelected.HideSelection();
                                _seriesSelected = null;
                            }
                            if (clickCount == 2)
                            {
                                _leftMouseDown = false;
                                _rootCanvas.ReleaseMouseCapture();
                                _chartX.FireLineStudyDoubleClick(lineStudy);
                            }
                            break;
                        }
                        //series drag & drop
                        Series series = element as Series;
                        if (CanStartMoveSeries(series) != MoveSeriesIndicator.MoveStatusEnum.CantMove)
                        {
                            StartMoveSeries(series, clickCount);
                            break;
                        }

                        ContextLine contextLine = element as ContextLine;
                        if (contextLine != null)
                        {
                            break;
                        }
                    }

                    p = e.GetPosition(_rootCanvas);
                    int xIndex = _chartX.GetReverseX(p.X);
                    _chartX.InvokeChartPanelMouseLeftClick(new StockChartX.ChartPanelMouseLeftClickEventArgs(
                                                             this,
                                                             p.X, p.Y,
                                                             GetReverseY(p.Y),
                                                             xIndex != -1
                                                               ? _chartX.GetTimestampByIndex(_chartX.GetReverseX(p.X) + _chartX._startIndex)
                                                               : null));

                    //just mouse down);
                    if (_chartX.InfoPanelPosition == InfoPanelPositionEnum.FollowMouse) //start timer and show infopanel
                    {
                        _chartX.StartShowingInfoPanel();
                        _rootCanvas.CaptureMouse();
                    }

                    break;
            }
        }

        private static readonly SeriesTypeOHLC[] _ohlcTypes = new[]
                                      {
                                        SeriesTypeOHLC.Open, SeriesTypeOHLC.High,
                                        SeriesTypeOHLC.Low, SeriesTypeOHLC.Close,
                                      };

        private void RootCanvas_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            Rect clipRectangle = new Rect(0, 0, e.NewSize.Width, e.NewSize.Height);

            _rootCanvas.Clip = new RectangleGeometry
                                 {
                                     Rect = clipRectangle
                                 };

            var ps = _panelsContainer._chartX._priceStyle;

            //only for non-standard price styles
            if (ps != PriceStyleEnum.psStandard && ps != PriceStyleEnum.psHeikinAshi)
            {
                int count = SeriesCollection.Count(s => _ohlcTypes.Contains(s.OHLCType));
                if (count < 3)
                {
                    //a panel that does not have OHLC series
                    //queue it's painting until an OHLC panel is repainted

                    _panelsContainer._panelsToBeRepainted.Add(this);
                    return;
                }
            }

            InvokePropertyChanged(Property_PanelResized);

            _timers.StartTimerWork(TimerSizeChanged);
        }
        #endregion


        #region Series Related methods
        /// <summary>
        /// Gind a series with same name but different OHLCV type
        /// </summary>
        /// <param name="series">A series from OHLC group.</param>
        /// <param name="seriesTypeOHLC">Needed OHLCV type</param>
        /// <returns>Reference to a series or null</returns>
        public Series GetSeriesOHLCV(Series series, SeriesTypeOHLC seriesTypeOHLC)
        {
            return _series.FirstOrDefault(s => s.Name == series.Name && s.OHLCType == seriesTypeOHLC);
        }

        internal Series CreateSeries(string seriesName, SeriesTypeOHLC ohlcType, SeriesTypeEnum seriesType)
        {
            Series series = CreateSeries(seriesName, seriesType, ohlcType);

            return AddSeries(series);
        }

        internal Series FirstSeries
        {

            get
            {
                return _series.Count > 0 ? _series[0] : null;
            }
        }

        internal int SeriesCount
        {
            get { return _series.Count; }
        }

        /// <summary>
        /// Adds a series to internal collection
        /// </summary>
        /// <param name="series"></param>
        /// <returns></returns>
        internal Series AddSeries(Series series)
        {
            _series.Add(series);
            if (_series.Count == 1) //first series, it will own Y scale
            {
                _shareScaleSeries.Add(series);
            }
            else
            {
                //not sure why I had this logic. By default all series share scale, unless specified otherwise
                //if (series.Name != _shareScaleSeries[0].Name && !(series is Indicator)) //if different group it can't share scale, unless is an indicator
                //  series._shareScale = false;

                if (series._shareScale)
                {
                    _shareScaleSeries.Add(series);
                }
            }

            if (!_hasVolume)
            {
                _hasVolume = series.OHLCType == SeriesTypeOHLC.Volume;
            }

            return series;
        }

        /// <summary>
        /// Deletes a series from internal collection
        /// </summary>
        /// <param name="series"></param>
        internal void DeleteSeries(Series series)
        {
            _shareScaleSeries.Remove(series);
            _series.Remove(series);
            if (_shareScaleSeries.Count != 0 || _series.Count <= 0) return;
            _series[0]._shareScale = true;
            _shareScaleSeries.Add(_series[0]);
        }

        internal IEnumerable<Series> GetSeriesFromGroup(string groupName)
        {
            return _series.Where(series => series.Name == groupName);
        }

        internal IEnumerable<Series> AllSeriesCollection
        {
            get { return _series; }
        }

        internal void RemoveSeries(Series series)
        {
            RemoveSeries(series, false);
        }
        internal void RemoveSeries(Series series, bool delete)
        {
            if (series == null)
            {
                return;
            }

            series.RemovePaint();
            series.HideSelection();
            if (delete)
            {
                _series.Remove(series);
            }

            _chartX._dataManager.UnRegisterSeries(series.Name, series.OHLCType);

            _chartX.FireSeriesRemoved(series.FullName);

            foreach (Series child in series._linkedSeries)
            {
                RemoveSeries(child, true);
            }
        }
        #endregion

        #region Private & Internal Methods
        private DataManager.DataManager DM
        {
            get { return _chartX._dataManager; }
        }

        /*
            private void GetMinMax(IEnumerable<Series> series, out double min, out double max)
            {
              min = double.MaxValue;
              max = double.MinValue;

              foreach (Series s in series)
              {
                if (_enforceSeriesSetMinMax)
                  s.SeriesEntry._visibleDataChanged = true;

                DM.VisibleMinMax(s._seriesIndex, out s._min, out s._max);

                if (s._min < min) min = s._min;
                if (s._max > max) max = s._max;
              }
            }
        */

        private int _labelCount;
        /// <summary>
        /// Get the max/min for scaling
        /// </summary>
        internal void SetMaxMin()
        {
            //if (_staticYScale) return;

            if (!_staticYScale)
            {
                _min = double.MaxValue;
                _max = double.MinValue;


                _hasPrice = false;
                _hasVolume = false;
                List<Series> shareScaleSeries = new List<Series>();
                foreach (Series series in _shareScaleSeries.Where(series => series._shareScale))
                {
                    shareScaleSeries.AddRange(GetSeriesFromGroup(series.Name));
                }

                foreach (Series series in _series.Where(series => series._shareScale && shareScaleSeries.IndexOf(series) == -1))
                {
                    shareScaleSeries.Add(series);
                }

                foreach (Series series in AllSeriesCollection)
                {
                    if (_enforceSeriesSetMinMax)
                        series.SeriesEntry._visibleDataChanged = true;

                    DM.VisibleMinMax(series.SeriesIndex, out series._min, out series._max);

                    if (shareScaleSeries.Count == 0)
                    {
                        if (series._min < _min)
                            _min = series._min;

                        if (series._max > _max)
                            _max = series._max;
                    }
                    else
                    {
                        //analize just series that hold the Y scale
                        int seriesIndex = series.SeriesIndex;
                        if (shareScaleSeries.FindIndex(series1 => series1.SeriesIndex == seriesIndex) != -1)
                        {
                            if (series._min < _min)
                                _min = series._min;

                            if (series._max > _max)
                                _max = series._max;
                        }
                    }

                    switch (series.OHLCType)
                    {
                        case SeriesTypeOHLC.Close:
                            _hasPrice = true;
                            break;
                        case SeriesTypeOHLC.Volume:
                            _hasVolume = true;
                            break;
                    }
                }
                if (IncludeLineStudiesInSeriesMinMax)
                {
                    foreach (LineStudy lineStudy in LineStudiesCollection)
                    {
                        double min, max;
                        lineStudy.GetYMinMax(out min, out max);
                        if (min == double.MinValue)
                            continue;

                        if (min < _min)
                            _min = min;

                        if (max > _max)
                            _max = max;
                    }
                }

                if (_max == double.MinValue)
                {
                    _max = 1;
                    _min = 0;
                }

                _minChanged = _min;
                _maxChanged = _max;

                _labelCount = (int)(ActualHeight / (_chartX.GetTextHeight("0") * 2.5));
                if (_labelCount < 4)
                    _labelCount = 4;

                double realMin, realMax;
                double gridScale;

                switch (_chartX.YGridStepType)
                {
                    case YGridStepType.NiceStep:
                        Utils.GridScaleReal(_minChanged, _maxChanged, _labelCount, out realMin, out realMax, out _labelCount,
                                            out gridScale);
                        break;
                    case YGridStepType.MinimalGap:
                        Utils.GridScale1(_minChanged, _maxChanged, _labelCount, false, out realMin, out realMax, out gridScale);
                        break;
                    default:
                        throw new ArgumentException("Unsupported YGrid step type");
                }

                CurrentYScaleStep = gridScale;
                _minChanged = realMin;
                _maxChanged = realMax;

                if (Math.Abs(_minChanged - _maxChanged) <= 0.00001)
                {
                    if (_minChanged == 0)
                    {
                        _minChanged = -0.01;
                        _maxChanged = 0.01;
                    }
                    else
                    {
                        double median = (_minChanged + _maxChanged) / 2;
                        _maxChanged = median + Math.Abs(median) * 0.005;
                        _minChanged = median - Math.Abs(median) * 0.005;
                    }
                }
            }
            else
            {
                _labelCount = (int)(ActualHeight / (_chartX.GetTextHeight("0") * 2.5));
                if (_labelCount < 4)
                    _labelCount = 4;

                double realMin, realMax;
                double gridScale;

                switch (_chartX.YGridStepType)
                {
                    case YGridStepType.NiceStep:
                        Utils.GridScaleReal(_minChanged, _maxChanged, _labelCount, out realMin, out realMax, out _labelCount,
                                            out gridScale);
                        break;
                    case YGridStepType.MinimalGap:
                        Utils.GridScale1(_minChanged, _maxChanged, _labelCount, false, out realMin, out realMax, out gridScale);
                        break;
                    default:
                        throw new ArgumentException("Unsupported YGrid step type");
                }
            }

            InvokePropertyChanged(Property_MinMaxChanged);

            _enforceSeriesSetMinMax = false;
        }

        private void PostIndicatorCalculate()
        {
            _timers.StopTimerWork(TimerSizeChanged);

            //if (!_staticYScale)
            SetMaxMin();

            if (_leftYAxis.Visibility == Visibility.Visible)
            {
                _leftYAxis.GridStep = CurrentYScaleStep;
                _leftYAxis.LabelCount = _labelCount;
                _leftYAxis.SetMinMax(Min, Max);
            }

            if (_rightYAxis.Visibility == Visibility.Visible)
            {
                _rightYAxis.GridStep = CurrentYScaleStep;
                _rightYAxis.LabelCount = _labelCount;
                _rightYAxis.SetMinMax(Min, Max);
            }

            PaintSideVolumeDepthBars();

            foreach (Series series in _series)
            {
                series.Painted = false;
            }

            foreach (Series series in _series)
            {
                series.Paint();
            }

            _notVisibleLineStudiesAbove.Clear();
            _notVisibleLineStudiesBelow.Clear();
            System.Windows.Media.Geometry clipArea = ClipArea;
            _lineStudies.ForEach(ls =>
                                   {
                                       //ls.Clip = clipArea;
                                       ls.SetClipingArea(ClipAreaRect);
                                       ls.Paint(0, 0, LineStudy.LineStatus.RePaint);
                                       var v = ls.IsCurrentlyVisible();
                                       if (v == LineStudy.LSVisibility.NotVisible_Above)
                                       {
                                           _notVisibleLineStudiesAbove.Add(new WeakReference(ls));
                                       }
                                       else if (v == LineStudy.LSVisibility.NotVisible_Below)
                                       {
                                           _notVisibleLineStudiesBelow.Add(new WeakReference(ls));
                                       }
                                   });
            _timers.StartTimerWork(TimerLineStudiesNotVisibleChanged);

            //PaintXGrid();

            if (_chartX._isBeta)
            {
                _betaReminder.Text = "BETA VERSION - please report bugs to support@modulusfe.com";
                Canvas.SetLeft(_betaReminder, 100);
                Canvas.SetTop(_betaReminder, 50);
                _betaReminder.Foreground = Brushes.Red;
                Canvas.SetZIndex(_betaReminder, ZIndexConstants.PriceStyles1);
            }

            _painting = false;
            _recalc = false;

            if (_series.Count > 0) //only if panel has series left then it was actually painted, otherwise it will be deleted
            {
                _chartX.FireChartPanelPaint(this);
            }

            if (_needRePaint)
            {
                _needRePaint = false;
                if (!IsHeatMap)
                {
                    _panelsContainer.ResetHeatMapPanels();
                    _panelsContainer.RecyclePanels();

                    SetTickBoxes();
                }
            }
            //      Debug.WriteLine("After series paint end.");
        }

        internal void ResetZoom()
        {
            //AutoResetIncludeLineStudiesMinMax = true;
            IncludeLineStudiesInSeriesMinMax = false;
        }

        private bool PreIndicatorCalculate()
        {
            if (!_chartX.CheckRegistration())
                return false;

            if (!_templateLoaded)
                _needRePaint = true;
            if (!_templateLoaded || _chartX._locked)
                return false;
            if (_rootCanvas.ActualHeight == 0)
                return false;

            if (_painting)
            {
                _timers.StopTimerWork(TimerSizeChanged);
                return false;
            }

            _painting = true;
            return true;
        }


        internal Action _afterPaintAction;
        internal virtual void Paint()
        {
            if (!PreIndicatorCalculate())
                return;

            CalculateIndicators();
#if WPF
      PostIndicatorCalculate(); //for Silverlight we move this code to ProcessIndicators, cause of the non-modal behavior of pseudo-dialogs in SL
      if (_afterPaintAction != null)
        _afterPaintAction();
#endif

            //      Debug.WriteLine("Paint - end");

            if (!_panelsContainer._panelsToBeRepainted.Contains(this))
            {
                _panelsContainer._panelsToBeRepainted.ForEach(panel => panel.Paint());
                _panelsContainer._panelsToBeRepainted.Clear();
            }

            if (_chartX.XGrid)
                PaintXGrid();

            _chartX.InvokeChartPanelStatusChanged(
              new StockChartX.ChartPanelStatusChangedEventArgs(this,
                                                               StockChartX.ChartStatus.Building,
                                                               StockChartX.ChartStatus.Ready));
        }

        private YAxisCanvas _yaxisMovingInAction;
        private double _yMoveUpDownStart;
        internal void StartYMoveUpDown(YAxisCanvas axisCanvas)
        {
            if (_isHeatMap)
                return;

            _yaxisMovingInAction = axisCanvas;
            _yMoveUpDownStart = Mouse.GetPosition(_yaxisMovingInAction).Y;
            _timers.StartTimerWork(TimerMoveYAxes);

        }

        internal void StopYMoveUpDown(YAxisCanvas axisCanvas)
        {
            if (_isHeatMap)
                return;

            _yaxisMovingInAction = null;
            _timers.StopTimerWork(TimerMoveYAxes);
        }
        /// <summary>
        /// this method will be called from timer, when user will press in Y axes the right mouse button
        /// </summary>
        internal void MoveUpDown()
        {
            if (_isHeatMap)
                return;

            Point p = Mouse.GetPosition(_yaxisMovingInAction);
            _yOffset -= (_yMoveUpDownStart - p.Y);
            Paint();
            _yMoveUpDownStart = p.Y;

            InvokePropertyChanged(Property_YAxisMoved);
        }

        private YAxisCanvas _yaxisResizeInAction;
        private double _yResizeStart;
        internal void StartYResize(YAxisCanvas axisCanvas)
        {
            if (_isHeatMap)
                return;

            _yaxisResizeInAction = axisCanvas;
            _yResizeStart = Mouse.GetPosition(_yaxisResizeInAction).Y;

            //Debug.WriteLine(string.Format("ResizeUpDown start {0}", _y));
            _timers.StartTimerWork(TimerResizeYAxes);
        }

        internal void StopYResize(YAxisCanvas axisCanvas)
        {
            if (_isHeatMap)
                return;

            _yaxisResizeInAction = null;
            _timers.StopTimerWork(TimerResizeYAxes);
        }

        internal void ResizeUpDown()
        {
            if (_isHeatMap)
            {
                return;
            }

            Point p = Mouse.GetPosition(_yaxisResizeInAction);
            _staticYScale = true;

            if (_yResizeStart == p.Y)
            {
                return;
            }

            double diff = (_maxChanged - _minChanged) * 0.05; //5%
            if ((_yResizeStart - p.Y) > 0)
            {
                //by increasing max and decreasing min, chart will become "shorter" by Y
                _maxChanged += diff;
                _minChanged -= diff;
            }
            else
            {
                _maxChanged -= diff;
                _minChanged += diff;
            }

            Paint();
            _yResizeStart = p.Y;

            InvokePropertyChanged(Property_YAxisResized);
        }

        internal void StartPaintingYGridLines()
        {
            if (!_chartX.YGrid || _isHeatMap)
            {
                return;
            }

            _gridYLines.C = _rootCanvas;
            _gridYLines.Start();
            if (_gridYLines.NewObjectCreated == null)
            {
                _gridYLines.NewObjectCreated = line =>
                                                 {
                                                     var l = line._line;
                                                     l.SetBinding(Shape.StrokeProperty, _chartX.CreateOneWayBinding("GridStroke"));
#if WPF
                                           l.SetBinding(Shape.StrokeDashArrayProperty, _chartX.CreateOneWayBinding("HorizontalGridLinePattern"));
#endif
                                                     l.StrokeThickness = 1;
                                                     l.X1 = 0;
                                                 };
            }
        }

        internal void StopPaintingYGridLines()
        {
            if (!_chartX.YGrid || _isHeatMap)
            {
                return;
            }

            _gridYLines.Stop();
            _gridYLines.Do(l => l.ZIndex = ZIndexConstants.GridLines);
        }

        internal void PaintYGridLine(double y)
        {
            if (!_chartX.YGrid || _isHeatMap)
            {
                return;
            }

            var line = _gridYLines.GetPaintObject()._line;
            line.X2 = _rootCanvas.ActualWidth;
            line.Y1 = y;
            line.Y2 = y;
        }

        internal void ShowHideYGridLines()
        {
            bool linesVisible = _chartX.YGrid;
            _gridYLines.Do(l => l._line.Visibility = linesVisible ? Visibility.Visible : Visibility.Collapsed);
        }

        internal void StartPaintingXGridLines()
        {
            if (!_chartX.XGrid) return;

            _gridXLines.C = _rootCanvas;
            _gridXLines.Start();

            if (_gridXLines.NewObjectCreated == null)
            {
                _gridXLines.NewObjectCreated = line =>
                                                 {
                                                     var l = line._line;
                                                     l.SetBinding(Shape.StrokeProperty, _chartX.CreateOneWayBinding("GridStroke"));
#if WPF
                                           l.SetBinding(Shape.StrokeDashArrayProperty, _chartX.CreateOneWayBinding("VerticalGridLinePattern"));
#endif
                                                     l.StrokeThickness = 1;
                                                 };
            }
        }

        internal void StopPaintingXGridLines()
        {
            if (!_chartX.XGrid || _isHeatMap)
            {
                return;
            }

            _gridXLines.Stop();
            _gridXLines.Do(l => l.ZIndex = ZIndexConstants.GridLines);
        }

        internal void PaintXGridLine(double x)
        {
            if (!_chartX.XGrid || !_templateLoaded || _isHeatMap)
            {
                return;
            }

            foreach (var pair in _chartX._xGridMap)
            {
                var l = _gridXLines.GetPaintObject()._line;
                l.X1 = pair.Value;
                l.X2 = pair.Value;
                l.Y1 = 0;
                l.Y2 = _rootCanvas.ActualHeight;
                //Utils.DrawLine(pair.Value, 0, pair.Value, _rootCanvas.ActualHeight, _chartX.GridStroke, LinePattern.Solid, 1, _gridXLines);
            }
        }

        internal void ShowHideXGridLines()
        {
            if (_chartX.XGrid && _gridXLines.Count == 0)
            {
                PaintXGrid();
            }
            _gridXLines.Do(l => l._line.Visibility = _chartX.XGrid ? Visibility.Visible : Visibility.Collapsed);
        }

        /// <summary>
        /// Paint X Grid
        /// Mainly called fron Calendar OnPaint, after it paints itself and prepares xGridMap
        /// </summary>
        internal virtual void PaintXGrid()
        {
            StartPaintingXGridLines();
            PaintXGridLine(0);
            StopPaintingXGridLines();
        }

        internal virtual Series CreateSeries(string seriesName,
          SeriesTypeEnum seriesType, SeriesTypeOHLC seriesTypeOHLC)
        {
            switch (seriesType)
            {
                case SeriesTypeEnum.stLineChart:
                    return new Standard(seriesName, seriesType, seriesTypeOHLC, this);
                case SeriesTypeEnum.stStockBarChart:
                    return new Stock(seriesName, seriesType, seriesTypeOHLC, this);
                case SeriesTypeEnum.stStockBarChartHLC:
                    return new Stock(seriesName, seriesType, seriesTypeOHLC, this);
                case SeriesTypeEnum.stCandleChart:
                    return new Stock(seriesName, seriesType, seriesTypeOHLC, this);
                case SeriesTypeEnum.stVolumeChart:
                    return new Standard(seriesName, SeriesTypeEnum.stVolumeChart, SeriesTypeOHLC.Volume, this);
                default:
                    throw new ArgumentException("[AddNewSeriesType] SeriesType " + seriesType + " not supported.");
            }
        }

        internal static SeriesTypeEnum GetSeriesTypeBySeries(Series series)
        {
            if (series is Standard)
            {
                return SeriesTypeEnum.stLineChart;
            }

            if (series is Stock)
            {
                return SeriesTypeEnum.stCandleChart;
            }

            return SeriesTypeEnum.stUnknown;
        }

        /// <summary>
        /// updates only the visual presentation of he trendline
        /// actual penetration check happens within Series class
        /// </summary>
        internal void WatchTrendLines()
        {
            int recordCount = _chartX.RecordCount;

            foreach (TrendLine trendLine in _trendWatch)
            {
                // Automatically extend the trend line into the future
                double x1 = trendLine.X1Value;
                double y1 = trendLine.Y1Value;
                double x2 = trendLine.X2Value;
                double y2 = trendLine.Y2Value;

                if (x2 == x1) return;

                double incr = (y2 - y1) / (x2 - x1);

                trendLine.SetXYValues(x1, y1, x1 + (recordCount - x1), y1 + (incr * (recordCount - x1)));
            }
        }

        internal void RegisterWatchableTrendLine(TrendLine trendLine)
        {
            _trendWatch.Add(trendLine);
            WatchTrendLines();
            foreach (Series series in _series)
            {
                series.CheckTrendLinesPenetration();
            }
        }

        internal void UnRegisterWatchableTrendLine(TrendLine trendLine)
        {
            _trendWatch.Remove(trendLine);
        }

        internal IList<TrendLine> WatchableTrendLines
        {
            get { return _trendWatch; }
        }

        internal void ShowHideTitleBar()
        {
            if (!_templateLoaded) return;
            if (_chartX.DisplayTitles)
            {
                _titleBar.Visibility = Visibility.Visible;
                _rootGrid.RowDefinitions[0].Height = new GridLength(Constants.PanelTitleBarHeight);
            }
            else
            {
                _titleBar.Visibility = Visibility.Collapsed;
                _rootGrid.RowDefinitions[0].Height = new GridLength(0);
            }
        }

        internal void UnRegisterSeriesFromDataManager()
        {
            while (_series.Count > 0)
            {
                string seriesName = _series[0].Name;
                string seriesFullName = _series[0].FullName;
                SeriesTypeOHLC ohlcType = _series[0].OHLCType;
                _series.RemoveAt(0);
                _chartX._dataManager.UnRegisterSeries(seriesName, ohlcType);
                _chartX.FireSeriesRemoved(seriesFullName);
            }
        }

        internal void SetLineStiduesClip()
        {
            Rect? clipAreaRect = ClipAreaRect;
            foreach (LineStudy lineStudy in _lineStudies)
            {
                lineStudy.SetClipingArea(clipAreaRect);
            }
        }

        internal System.Windows.Media.Geometry ClipArea
        {
            get
            {
                double paintableWidth = _chartX.PaintableWidth;
                if (_chartX == null || _rootCanvas == null || paintableWidth <= 0)
                {
                    return null;
                }

                return new RectangleGeometry
                         {
                             Rect = new Rect(_chartX.LeftChartSpace, 0, paintableWidth, _rootCanvas.ActualHeight)
                         };
            }
        }

        internal Rect? ClipAreaRect
        {
            get
            {
                if (_chartX == null || _rootCanvas == null || _rootCanvas.ActualHeight < 0 || _chartX.PaintableWidth < 0)
                {
                    return null;
                }

                return new Rect(_chartX.LeftChartSpace, 0, _chartX.PaintableWidth, _rootCanvas.ActualHeight);
            }
        }
        #endregion

        #region Implementation of INotifyPropertyChanged

        ///<summary>
        /// Invoked when a property changes
        ///</summary>
        public event PropertyChangedEventHandler PropertyChanged;

        private void InvokePropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, e);
        }

        #endregion
    }
}
