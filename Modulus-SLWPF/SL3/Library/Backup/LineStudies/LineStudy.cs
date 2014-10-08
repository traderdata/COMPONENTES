using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using ModulusFE.ChartElementProperties;
using ModulusFE.Interfaces;
using ModulusFE.PaintObjects;
#if SILVERLIGHT
using ModulusFE.SL;
#endif

namespace ModulusFE.LineStudies
{
    /// <summary>
    /// Base class for all line studies used in the chart
    /// </summary>
    public partial class LineStudy : IChartElementPropertyAble, INotifyPropertyChanged
    {
        /// <summary>
        /// Line Study Types
        /// </summary>
        public enum StudyTypeEnum
        {
            /// <summary>
            /// Ellipse
            /// </summary>
            Ellipse,
            /// <summary>                        
            /// Rectangle
            /// </summary>
            Rectangle,
            /// <summary>
            /// Trend Line
            /// </summary>
            TrendLine,
            /// <summary>
            /// Speed Lines
            /// </summary>
            SpeedLines,
            /// <summary>
            /// Gann Fan
            /// </summary>
            GannFan,
            /// <summary>
            /// Fibonacci Arcs
            /// </summary>
            FibonacciArcs,
            /// <summary>
            /// Fibonacci Fan
            /// </summary>
            FibonacciFan,
            /// <summary>
            /// Fibonacci Retracements
            /// </summary>
            FibonacciRetracements,
            /// <summary>
            /// Fibonacci Time Zones
            /// </summary>
            FibonacciTimeZones,
            /// <summary>
            /// Tirone Levels
            /// </summary>
            TironeLevels,
            /// <summary>
            /// Quadrant Lines
            /// </summary>
            QuadrantLines,
            /// <summary>
            /// Raff Regression
            /// </summary>
            RaffRegression,
            /// <summary>
            /// Error Channel
            /// </summary>
            ErrorChannel,
            /// <summary>
            /// used for buy, sell and exit Symbols - images
            /// </summary>
            SymbolObject,
            /// <summary>
            /// Horizontal line
            /// </summary>
            HorizontalLine,
            /// <summary>
            /// Vertical Line
            /// </summary>
            VerticalLine,
            /// <summary>
            /// Image Object
            /// </summary>
            ImageObject,
            /// <summary>
            /// Static Text
            /// </summary>
            StaticText,
            /// <summary>
            /// User defined text (not yet supported)
            /// </summary>
            UserDefinedText,
            /// <summary>
            /// WPF UI Element
            /// </summary>
            FrameworkElement,

            /// <summary>
            /// Unknown
            /// </summary>
            Unknown
        }

        ///<summary>
        /// Specifies the alignment of the value presenter relative to chart
        ///</summary>
        public enum ValuePresenterAlignmentType
        {
            /// <summary>
            /// Left
            /// </summary>
            Left,

            ///<summary>
            /// Right
            ///</summary>
            Right
        }

        ///<summary>
        /// Values that indicates the visibility of current LineStudy
        ///</summary>
        public enum LSVisibility
        {
            ///<summary>
            /// Not visible, is located above <see cref="ChartPanel.Max"/>
            ///</summary>
            NotVisible_Above,
            ///<summary>
            /// Not visible, is located below <see cref="ChartPanel.Min"/>
            ///</summary>
            NotVisible_Below,
            ///<summary>
            /// Visible
            ///</summary>
            Visible,
            /// <summary>
            /// In case when LineStudy is not created yet, or its position is not yet defined
            /// </summary>
            Unknown
        }

        ///<summary>
        /// Makes the LienStudy visible, by scrolling or zooming the chart to needed position
        ///</summary>
        public enum EnsureVisibilityPosition
        {
            ///<summary>
            ///</summary>
            Left,
            ///<summary>
            ///</summary>
            Middle,
            ///<summary>
            ///</summary>
            Right
        }


        /// <summary>
        /// for internal usage only
        /// </summary>
        internal enum LineStatus
        {
            StartPaint, Painting, EndPaint,
            StartMove, Moving, EndMove,
            Selected, Normal,
            RePaint
        }

        /// <summary>
        /// Has a reference to <see cref="Interfaces.IValuePresenter"/> for a <see cref="LineStudy"/>
        /// </summary>
        protected Interfaces.IValuePresenter _valuePresenter;
        /// <summary>
        /// Shows or hides a LineStudy
        /// </summary>
        /// <param name="show"></param>
        protected virtual void ShowInternal(bool show) { }

        internal bool _selectable;

        private double _strokeThickness;
        private Brush _stroke;
        private LinePattern _strokeType;
        private double _opacity = 1.0;
        private Rect? _paintableRect; // used for clipping region

        //internal int _zOrder;
        internal bool _selected;
        internal bool _drawing;
        internal bool _drawn;
        internal bool _internalObjectCreated; //gets if the internal object used to paint the lineStudy was created
        internal String _key;
        internal bool _selectionVisible;
        internal object _extraArgs;
        internal double _x1;
        internal double _y1;
        internal double _x2;
        internal double _y2;
        internal double _x1Value; //Chart values
        internal double _y1Value;
        internal double _x2Value;
        internal double _y2Value;
        //internal double? []_params = new double?[Constants.MaxParams];
        internal StudyTypeEnum _studyType;
        internal StockChartX _chartX;

        internal Types.Corner _resizingCorner;
        internal ChartPanel _chartPanel;
        internal PaintObjectsManager<SelectionDot> _selectionDots = new PaintObjectsManager<SelectionDot>();
        internal class SelectionDotInfo
        {
            public Types.Corner Corner;
            public Point Position;
            public bool Clickable = true;
        }

        internal LineStudy()
        {
            _stroke = Brushes.Red;

            Initialize();
        }

        internal LineStudy(string key, Brush stroke, ChartPanel chartPanel)
        {
            _key = key;
            _stroke = stroke;

            if (chartPanel != null)
            {
                SetChartPanel(chartPanel);
            }

            Initialize();
        }

        internal void SetChartPanel(ChartPanel chartPanel)
        {
            _chartPanel = chartPanel;
            _chartX = _chartPanel._chartX;
        }

        internal void Initialize()
        {
            _x1 = _y1 = _x2 = _y2 = 0.0;
            _x1Value = _y1Value = 0.0;
            _x2Value = _y2Value = 0.0;
            _strokeThickness = 1;
            _strokeType = LinePattern.Solid;
            _selectable = true;
            _selected = false;
            _drawn = false;
            _drawing = false;
            _selectionVisible = false;

            ValuePresenterAlignment = ValuePresenterAlignmentType.Right;
        }

        /// <summary>
        /// Sets chart value lookup based on actual pixel position
        /// </summary>
        internal void Update()
        {
            if (_chartPanel == null)
            {
                return;
            }

            _x1 = _chartX.GetXPixel(_x1Value - _chartX._startIndex, true);
            _x2 = _chartX.GetXPixel(_x2Value - _chartX._startIndex, true);
            _y1 = _chartPanel.GetY(_y1Value);
            _y2 = _chartPanel.GetY(_y2Value);

            if (_y1 < 0) _y1 = 0;
            if (_y2 < 0) _y2 = 0;
        }

        internal double _dStartX, _dStartY;
        internal Types.RectEx _newRect;
        internal Types.RectEx _oldRect;
        internal void Paint(double x, double y, LineStatus lineStatus)
        {
            if (C == null)
            {
                return;
            }

            if (lineStatus == LineStatus.RePaint)
            {
                Update();
            }

            if (lineStatus == LineStatus.Moving)
            {
                Paint(x, y, lineStatus, _resizingCorner == Types.Corner.None ? Types.Corner.MoveAll : _resizingCorner);
            }
            else
            {
                Paint(x, y, lineStatus, Types.Corner.None);
            }
        }
        internal void Paint(double x, double y, LineStatus lineStatus, Types.Corner corner)
        {
            if (!_drawn && lineStatus == LineStatus.RePaint)
            {
                _drawn = true;
                Update();
                DrawLineStudy(new Types.RectEx(), LineStatus.StartPaint);
            }
            switch (lineStatus)
            {
                case LineStatus.StartPaint:
                    _dStartX = x;
                    _dStartY = y;
                    DrawLineStudy(new Types.RectEx(), lineStatus);
                    break;
                case LineStatus.Painting:
                    SetRect(x, y);
                    DrawLineStudy(_newRect, lineStatus);
                    break;
                case LineStatus.EndPaint:
                    SetXY(x, y, lineStatus);
                    _dStartX = _dStartY = 0.0;
                    Reset();

                    _newRect = new Types.RectEx(_x1, _y1, _x2, _y2);
                    DrawLineStudy(_newRect, lineStatus);
                    break;
                case LineStatus.RePaint:
                    DrawLineStudy(_newRect = new Types.RectEx(_x1, _y1, _x2, _y2), lineStatus);
                    break;
                case LineStatus.StartMove:
                    _dStartX = x;
                    _dStartY = y;
                    break;
                case LineStatus.Moving:
                    switch (corner)
                    {
                        case Types.Corner.BottomRight:
                            _x2 = x;
                            _y2 = y;
                            break;
                        case Types.Corner.MiddleRight:
                            _x2 = x;
                            break;
                        case Types.Corner.TopRight:
                            _x2 = x;
                            _y1 = y;
                            break;
                        case Types.Corner.TopCenter:
                            _y1 = y;
                            break;
                        case Types.Corner.TopLeft:
                            _x1 = x;
                            _y1 = y;
                            break;
                        case Types.Corner.MiddleLeft:
                            _x1 = x;
                            break;
                        case Types.Corner.BottomLeft:
                            _x1 = x;
                            _y2 = y;
                            break;
                        case Types.Corner.BottomCenter:
                            _y2 = y;
                            break;
                        case Types.Corner.MoveAll:
                            _x1 -= (_dStartX - x);
                            _y1 -= (_dStartY - y);
                            _x2 -= (_dStartX - x);
                            _y2 -= (_dStartY - y);
                            _dStartX = x;
                            _dStartY = y;
                            break;
                    }
                    _newRect = new Types.RectEx(_x1, _y1, _x2, _y2);
                    DrawLineStudy(_newRect, lineStatus);
                    ShowSelection(true);
                    _oldRect = _newRect;
                    break;
                case LineStatus.EndMove:
                    SetXY(x, y, lineStatus);
                    Reset();

                    _newRect = new Types.RectEx(_x1, _y1, _x2, _y2);
                    DrawLineStudy(_newRect, lineStatus);
                    _resizingCorner = Types.Corner.None;
                    break;
            }

            if (lineStatus == LineStatus.RePaint)
            {
                ShowSelection(_selected);
            }

            _drawn = true;
        }

        internal void SetXY(double x, double y, LineStatus lineStatus)
        {
            switch (lineStatus)
            {
                case LineStatus.EndPaint:
                    _x1 = _dStartX;
                    _y1 = _dStartY;
                    _x2 = x;
                    _y2 = y;
                    if (_x1 > _x2 && _studyType != StudyTypeEnum.ImageObject && _studyType != StudyTypeEnum.FibonacciRetracements)
                    {
                        Utils.Swap(ref _x1, ref _x2);
                        Utils.Swap(ref _y1, ref _y2);
                    }
                    break;
                case LineStatus.EndMove:
                    _x1 = _newRect.Left;
                    _y1 = _newRect.Top;
                    _x2 = _newRect.Right;
                    _y2 = _newRect.Bottom;
                    break;
            }
        }

        internal virtual void Reset()
        {
            //if (_x1 < 1 || _x2 < 1) return;

            if (_x1 > 0)
            {
                _x1Value = _chartX.GetReverseXInternal(_x1) + _chartX._startIndex;
                _x1 = _chartX.GetXPixel(_x1Value - _chartX._startIndex);
            }
            if (_x2 > 0)
            {
                _x2Value = _chartX.GetReverseXInternal(_x2) + _chartX._startIndex;
                _x2 = _chartX.GetXPixel(_x2Value - _chartX._startIndex);
            }

            _y1Value = _chartPanel.GetReverseY(_y1);
            _y2Value = _chartPanel.GetReverseY(_y2);
            _y1 = _chartPanel.GetY(_y1Value);
            _y2 = _chartPanel.GetY(_y2Value);
        }

        internal void SetRect(double x, double y)
        {
            _newRect.Left = _dStartX;
            _newRect.Right = x;
            _newRect.Top = _dStartY;
            _newRect.Bottom = y;

            if (_newRect.Right < _x1)
            {
                _newRect.Left = _x1 + 5;
            }
        }

        #region Line Study Value
        private IValueBridge<LineStudy> _lineStudyValue;

        /// <summary>
        /// 
        /// </summary>
        protected virtual void ReSetLineStudyValue() { }
        ///<summary>
        /// Gets or sets a reference to a <see cref="IValueBridge{T}"/> derived object
        ///</summary>
        public IValueBridge<LineStudy> LineStudyValue
        {
            get { return _lineStudyValue; }
            set
            {
                if (_lineStudyValue == value)
                    return;
                _lineStudyValue = value;
                ReSetLineStudyValue();
            }
        }

        #endregion

        ///<summary>
        /// Gets or sets the alignment of Value Presenter
        ///</summary>
        public ValuePresenterAlignmentType ValuePresenterAlignment { get; set; }

        /// <summary>
        /// Selects or diselects a line study
        /// </summary>
        public bool Selected
        {
            get { return _selected; }
            set
            {
                if (!_selectable)
                {
                    return;
                }

                if (_selected == value || !_internalObjectCreated)
                {
                    return;
                }

                ShowSelection(_selected = value);
            }
        }

        /// <summary>
        /// Gets the unique key that is associated with current line study
        /// </summary>
        public string Key
        {
            get { return _key; }
        }

        /// <summary>
        /// Gets either the line study is selectable or not
        /// </summary>
        public bool Selectable
        {
            get { return _selectable; }
            set { _selectable = value; }
        }

        /// <summary>
        /// Gets the study type
        /// </summary>
        public StudyTypeEnum StudyType
        {
            get { return _studyType; }
        }

        /// <summary>
        /// Start record
        /// </summary>
        public double X1Value
        {
            get { return _x1Value; }
        }

        /// <summary>
        /// Start price value
        /// </summary>
        public double Y1Value
        {
            get { return _y1Value; }
        }

        /// <summary>
        /// End Record
        /// </summary>
        public double X2Value
        {
            get { return _x2Value; }
        }

        /// <summary>
        /// End price value
        /// </summary>
        public double Y2Value
        {
            get { return _y2Value; }
        }

        /// <summary>
        /// Sets the color of lineStudy, when LineStudy is a text object it will set the Foreground color
        /// </summary>
        public Brush Stroke
        {
            get { return _stroke; }
            set
            {
                if (_stroke == value)
                {
                    return;
                }

                _stroke = value;
                if (_internalObjectCreated)
                {
                    SetStroke();
                }
            }
        }

        /// <summary>
        /// Sets the thicknes if lines used to paint the lineStudy. In case of a text object it sets the font size
        /// </summary>
        public double StrokeThickness
        {
            get { return _strokeThickness; }
            set
            {
                if (value == _strokeThickness)
                {
                    return;
                }

                _strokeThickness = value;
                if (_internalObjectCreated)
                {
                    SetStrokeThickness();
                }
            }
        }

        /// <summary>
        /// Sets the stroke type (solid, dash, dot, ...) for lines used to paint the <see cref="LineStudy"/>
        /// </summary>
        public LinePattern StrokeType
        {
            get { return _strokeType; }
            set
            {
                if (_strokeType == value)
                {
                    return;
                }

                _strokeType = value;
                if (_internalObjectCreated)
                {
                    SetStrokeType();
                }
            }
        }

        ///<summary>
        /// Gets or sets the opacity of <see cref="LineStudy"/>
        ///</summary>
        public double Opacity
        {
            get { return _opacity; }
            set
            {
                if (_opacity == value)
                {
                    return;
                }

                _opacity = value;
                if (_internalObjectCreated)
                {
                    SetOpacity();
                }
                InvokePropertyChanged(OpacityChangedEventArgs);
            }
        }

        /// <summary>
        /// Gets the panel that has ownership on this <see cref="LineStudy"/>
        /// </summary>
        public ChartPanel Panel
        {
            get { return _chartPanel; }
        }

        /// <summary>
        /// Gets or sets whether the <see cref="LineStudy"/> will have the default functionality for context menu.
        /// </summary>
        public bool IsContextMenuDisabled { get; set; }

        /// <summary>
        /// extra parameters that were passed when creating the <see cref="LineStudy"/>. 
        /// Used for Image object to set image path and for text object to set the text
        /// </summary>
        public object ExtraArgs
        {
            get { return _extraArgs; }
        }

        internal Canvas C
        {
            get { return _chartPanel._rootCanvas; }
        }

        internal virtual void DrawLineStudy(Types.RectEx rect, LineStatus lineStatus)
        {
            throw new NotImplementedException();
        }
        internal virtual List<SelectionDotInfo> GetSelectionPoints()
        {
            throw new NotImplementedException();
        }
        internal virtual void SetCursor()
        {
            throw new NotImplementedException();
        }
        internal virtual void SetStrokeThickness()
        {
            throw new NotImplementedException();
        }
        internal virtual void SetStroke()
        {
            throw new NotImplementedException();
        }
        internal virtual void SetStrokeType()
        {
            throw new NotImplementedException();
        }
        internal virtual void RemoveLineStudy()
        {
            throw new NotImplementedException();
        }
        internal virtual void SetOpacity()
        {
            throw new NotImplementedException();
        }

        internal void ShowSelection(bool bShow)
        {
            _selectionVisible = bShow;

            SetCursor();

            if (!_selectionVisible)
            {
                _selectionDots.RemoveAll();
                return;
            }

            _selectionDots.C = C;
            _selectionDots.Start();

            foreach (SelectionDotInfo point in GetSelectionPoints())
            {
                SelectionDot dot = _selectionDots.GetPaintObject(point.Corner, point.Clickable);
                dot.SetPos(point.Position);
                dot.Tag = this;
                //dot.SetClip(Clip);
                Shape shape = dot.Shape;
                shape.Clip = GetClip(Canvas.GetLeft(shape), Canvas.GetTop(shape), _paintableRect);

                dot.ZIndex = ZIndexConstants.SelectionPoint1;
            }

            _selectionDots.Stop();
        }

        internal Series GetSeriesOHLC(SeriesTypeOHLC ohlc)
        {
            return _chartPanel.SeriesCount == 0 ? null : _chartPanel.GetSeriesOHLCV(_chartPanel.FirstSeries, ohlc);
        }

        internal void StartResize(SelectionDot selectionDot, MouseButtonEventArgs e)
        {
            _resizingCorner = selectionDot.Corner;
            _chartX.Status = StockChartX.ChartStatus.LineStudyMoving;
            C.CaptureMouse();
        }

        /// <summary>
        /// Called mainly by DataManager when a new record gets inserted before _x1 index value, in this case
        /// we must update X1 value so it will keep up with the needed record
        /// </summary>
        /// <param name="step"></param>
        internal void UpdatePosition(int step)
        {
            if (step >= _x1Value)
            {
                return; //value was appended after
            }

            //shift line study
            _x1Value += 1;
            _x2Value += 1;
        }

        internal virtual void SetArgs(params object[] args) { }

        /// <summary>
        /// Called after user set XYValues.
        /// </summary>
        protected virtual void XYValuesChanged() { }

        /// <summary>
        /// Sets programmatically the logical coordinates of a <see cref="LineStudy"/>. Internally they are transformed to canvas coordinates
        /// every line study has its own logic for seting canvas coordinates, you must know very well what every line study does
        /// and what are its rules of paiting.
        /// </summary>
        /// <param name="x1Value">X1 record index</param>
        /// <param name="y1Value">Y1 price value</param>
        /// <param name="x2Value">X2 record index</param>
        /// <param name="y2Value">Y2 price value</param>
        public virtual void SetXYValues(double x1Value, double y1Value, double x2Value, double y2Value)
        {
            _x1Value = x1Value;
            _x2Value = x2Value;
            _y1Value = y1Value;
            _y2Value = y2Value;

            Update();

            _newRect.Left = _x1;
            _newRect.Right = _x2;
            _newRect.Top = _y1;
            _newRect.Bottom = Double.IsNaN(_y2) ? _y1 + 1 : _y2;

            if (_chartX.Status == StockChartX.ChartStatus.Ready && C != null)
            {
                DrawLineStudy(_newRect, LineStatus.RePaint);
            }

            XYValuesChanged();
        }

        ///<summary>
        /// Returns the Y min &amp; max values that are needed to show the current <see cref="LineStudy"/>
        /// It will return <see cref="double.MinValue"/> in case <see cref="LineStudy"/> doesn't support such features, such as 
        /// <see cref="VerticalLine"/> and others.
        /// The values returned are price values rather than pixels.
        ///</summary>
        ///<param name="min"></param>
        ///<param name="max"></param>
        public virtual void GetYMinMax(out double min, out double max)
        {
            min = max = double.MinValue;
        }

        #region Implementation of IChartElementPropertyAble

        ///<summary>
        /// Gets <see cref="LineStudy"/>'s title
        ///</summary>
        public string Title
        {
            get { return StockChartX_LineStudiesParams.GetLineStudyFriendlyName(_studyType) + " Properties"; }
        }

        ///<summary>
        /// Gets <see cref="LineStudy"/>'s properties
        ///</summary>
        public IEnumerable<IChartElementProperty> Properties
        {
            get { return BaseProperties; }
        }

        #endregion

        #region ChartElementPropertyAble Propperties

        internal ChartElementColorProperty propertyStroke;
        internal ChartElementStrokeThicknessProperty propertyStrokeThickness;
        internal ChartElementStrokeTypeProperty propertyStrokeType;
        internal ChartElementColorProperty propertyFill;
        internal ChartElementOpacityProperty propertyOpacity;

        /// <summary>
        /// Gets the basic properties that are common for all LineStudies
        /// </summary>
        protected virtual IEnumerable<IChartElementProperty> BaseProperties
        {
            get
            {
                GetChartElementColorProperty();
                yield return propertyStroke;

                GetChartElementStrokeThicknessProperty("Stroke Thickness");
                yield return propertyStrokeThickness;

                GetChartElementStrokeTypeProperty();
                yield return propertyStrokeType;

                GetChartElementOpacityProperty();
                yield return propertyOpacity;

                GetChartElementFillProperty();
                if (propertyFill != null)
                    yield return propertyFill;
            }
        }

        /// <summary>
        /// Fill Propeerty
        /// </summary>
        protected void GetChartElementFillProperty()
        {
            if (!(this is IShapeAble)) return;

            IShapeAble shapeAble = (IShapeAble)this;
            propertyFill = new ChartElementColorProperty("Fill Color");
            if (shapeAble.Fill is SolidColorBrush)
                propertyFill.ValuePresenter.Value = shapeAble.Fill;
            propertyFill.SetChartElementPropertyValue
              += presenter =>
                   {
                       shapeAble.Fill = (SolidColorBrush)presenter.Value;
                   };
        }

        /// <summary>
        /// Opacity Property
        /// </summary>
        protected void GetChartElementOpacityProperty()
        {
            propertyOpacity = new ChartElementOpacityProperty("Opacity");
            propertyOpacity.ValuePresenter.Value = _opacity;
            propertyOpacity.SetChartElementPropertyValue
              += presenter =>
                   {
                       Opacity = Convert.ToDouble(presenter.Value);
                   };
        }

        /// <summary>
        /// Stroke Property
        /// </summary>
        protected void GetChartElementStrokeTypeProperty()
        {
            propertyStrokeType = new ChartElementStrokeTypeProperty("Stroke Type");
            propertyStrokeType.ValuePresenter.Value = StrokeType.ToString();
            propertyStrokeType.SetChartElementPropertyValue
              += presenter =>
                   {
                       StrokeType = (LinePattern)System.Enum.Parse(typeof(LinePattern), presenter.Value.ToString()
#if SILVERLIGHT
, true
#endif
);
                       SetStrokeType();
                   };
        }

        /// <summary>
        /// Stroke Thickness property
        /// </summary>
        /// <param name="propertyName"></param>
        protected void GetChartElementStrokeThicknessProperty(string propertyName)
        {
            propertyStrokeThickness = new ChartElementStrokeThicknessProperty(propertyName);
            propertyStrokeThickness.ValuePresenter.Value = StrokeThickness;
            propertyStrokeThickness.SetChartElementPropertyValue
              += presenter =>
                   {
                       StrokeThickness = Convert.ToDouble(presenter.Value);
                       SetStrokeThickness();
                   };
        }

        /// <summary>
        /// Color property
        /// </summary>
        protected void GetChartElementColorProperty()
        {
            propertyStroke = new ChartElementColorProperty("Border Stroke Color");
            propertyStroke.ValuePresenter.Value = Stroke;
            propertyStroke.SetChartElementPropertyValue
              += presenter =>
                   {
                       Stroke = (SolidColorBrush)presenter.Value;
                       SetStroke();
                   };
        }

        #endregion

        ///<summary>
        /// Occurs when a property changes
        ///</summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Invokes the PropertyChanged handler
        /// </summary>
        /// <param name="e"></param>
        protected void InvokePropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        ///<summary>
        ///</summary>
        public static readonly PropertyChangedEventArgs OpacityChangedEventArgs = new PropertyChangedEventArgs("Opacity");

        internal void SetClipingArea(Rect? paintableRect)
        {
            _paintableRect = paintableRect;
            SetClipingAreaInternal(_paintableRect);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="paintableRect"></param>
        protected virtual void SetClipingAreaInternal(Rect? paintableRect) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="top"></param>
        /// <param name="paintableRect"></param>
        /// <returns></returns>
        protected System.Windows.Media.Geometry GetClip(double left, double top, Rect? paintableRect)
        {
            if (paintableRect == null)
            {
                return null;
            }

            left = Math.Abs(left);
            top = Math.Abs(top);

            double width = Math.Max(0, paintableRect.Value.Right - left);
            double height = Math.Max(0, paintableRect.Value.Bottom - top);

            return new RectangleGeometry
                     {
                         Rect = new Rect(0, 0, width, height)
                     };
        }

        ///<summary>
        /// Shows the internal properties dialog
        ///</summary>
        public void ShowPropertiesWindow()
        {
            IChartElementPropertyAble propertyAble = this;

            List<IChartElementProperty> properties = new List<IChartElementProperty>(propertyAble.Properties);
            PropertiesDialog dialog = new PropertiesDialog(propertyAble.Title, properties)
                                        {
#if WPF
                                    Owner = _chartX.OwnerWindow,
#endif
                                            Background = _chartX.LineStudyPropertyDialogBackground
                                        };

#if SILVERLIGHT
            //dialog.Show(Dialog.DialogStyle.ModalDimmed);
            dialog.Show();
#endif
#if WPF
      dialog.ShowDialog();
#endif
        }


        #region CanvasZIndex

        private int _canvasZIndex;

        ///<summary>
        ///</summary>
        public static readonly PropertyChangedEventArgs CanvasZIndexChangedEventsArgs =
          ObservableHelper.CreateArgs<StaticText>(_ => _.CanvasZIndex);

        ///<summary>
        /// Gets or sets the ZIndex order on canvas. 
        /// Not to be used public
        ///</summary>
        public int CanvasZIndex
        {
            get { return _canvasZIndex; }
            set
            {
                if (_canvasZIndex != value)
                {
                    _canvasZIndex = value;
                    InvokePropertyChanged(CanvasZIndexChangedEventsArgs);
                }
            }
        }

        #endregion

    }
}

