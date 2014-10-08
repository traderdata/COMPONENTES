using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ModulusFE.LineStudies;
using ModulusFE.PaintObjects;

namespace ModulusFE
{
    public static partial class StockChartX_LineStudiesParams
    {
        internal static void Register_StaticText()
        {
            RegisterLineStudy(LineStudy.StudyTypeEnum.StaticText, typeof(StaticText), "Static Text");
        }
    }
}


namespace ModulusFE.LineStudies
{
    ///<summary>
    /// Static Text
    ///</summary>
    public partial class StaticText : LineStudy, IMouseAble, IContextAbleLineStudy
    {
        private TextBlock _txt;
        private string _text;
        private string _fontName;
        private ContextLine _contextLine;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="key">Unique key for line study</param>
        /// <param name="stroke">Stroke brush</param>
        /// <param name="chartPanel">Reference to chart panel where it will be placed.</param>
        public StaticText(string key, Brush stroke, ChartPanel chartPanel)
            : base(key, stroke, chartPanel)
        {
            _studyType = StudyTypeEnum.StaticText;
        }

        internal override void SetArgs(params object[] args)
        {
            _extraArgs = args;

            Debug.Assert(args.Length > 0);
            Text = args[0].ToString();
        }

        internal override void DrawLineStudy(Types.RectEx rect, LineStatus lineStatus)
        {
            if (_txt == null && lineStatus != LineStatus.StartPaint)
            {
                DrawLineStudy(rect, LineStatus.StartPaint);
            }

            if (lineStatus == LineStatus.StartPaint)
            {
                FontFamily = new FontFamily(_fontName = _chartX.FontFace);
                Foreground = Stroke;

                _txt = new TextBlock
                         {
                             Tag = this,
                             Visibility = Visibility.Collapsed
                         };
                C.Children.Add(_txt);
                _txt.SetBinding(TextBlock.TextProperty, this.CreateOneWayBinding("Text"));
                _txt.SetBinding(TextBlock.FontFamilyProperty, this.CreateOneWayBinding("FontFamily"));
                _txt.SetBinding(TextBlock.FontSizeProperty, this.CreateOneWayBinding("FontSize"));
                _txt.SetBinding(TextBlock.ForegroundProperty, this.CreateOneWayBinding("Foreground"));
                _txt.SetBinding(UIElement.OpacityProperty, this.CreateOneWayBinding("Opacity"));

                _txt.MouseLeftButtonDown += (sender, args) => MouseDown(sender, args);
                _txt.MouseEnter += (sender, args) => MouseEnter(sender, args);
                _txt.MouseLeave += (sender, args) => MouseLeave(sender, args);
                _txt.MouseMove += (sender, args) => MouseMove(sender, args);
                _txt.MouseLeftButtonUp += (sender, args) => MouseUp(sender, args);

                if (_contextLine == null)
                {
                    _contextLine = new ContextLine(this);

                    _chartX.InvokeLineStudyCreated(new StockChartX.LineStudyCreatedEventArgs(this));
                }

                _internalObjectCreated = true;
                _txt.SetBinding(Canvas.TopProperty, this.CreateOneWayBinding("CanvasTop"));
                _txt.SetBinding(Canvas.LeftProperty, this.CreateOneWayBinding("CanvasLeft"));
                _txt.SetBinding(Canvas.ZIndexProperty, this.CreateOneWayBinding("CanvasZIndex"));

                return;
            }

            if (_txt == null)
            {
                return;
            }

            if (_txt.Visibility == Visibility.Collapsed)
            {
                _txt.Visibility = Visibility.Visible;
                _txt.UpdateLayout();
            }

            //Canvas.SetLeft(_txt, rect.Right);
            //Canvas.SetTop(_txt, rect.Bottom);
            CanvasLeft = rect.Right;
            CanvasTop = rect.Bottom;
            CanvasZIndex = ZIndexConstants.LineStudies1;

            //Canvas.SetZIndex(_txt, ZIndexConstants.LineStudies1);
        }

        internal override List<SelectionDotInfo> GetSelectionPoints()
        {
            return new List<SelectionDotInfo>
               {
                 new SelectionDotInfo
                   {
                     Corner = Types.Corner.TopLeft,
                     Position = new Point(_x2, _y2),
                     Clickable = false
                   },
                 new SelectionDotInfo
                   {
                     Corner = Types.Corner.TopRight,
                     Position = new Point(_x2 + _txt.ActualWidth, _y2),
                     Clickable = false
                   },
                 new SelectionDotInfo
                   {
                     Corner = Types.Corner.BottomLeft,
                     Position = new Point(_x2, _y2 + _txt.ActualHeight),
                     Clickable = false
                   },
                 new SelectionDotInfo
                   {
                     Corner = Types.Corner.BottomRight,
                     Position = new Point(_x2 + _txt.ActualWidth, _y2 + _txt.ActualHeight),
                     Clickable = false
                   }
               };
        }

        internal override void SetCursor()
        {
            if (_selectionVisible)
            {
                _txt.Cursor = Cursors.Hand;
                return;
            }
            if (_selectionVisible || _txt.Cursor == Cursors.Arrow) return;
            _txt.Cursor = Cursors.Arrow;
        }

        internal override void RemoveLineStudy()
        {
            C.Children.Remove(_txt);
        }

        internal override void SetStrokeThickness()
        {
            FontSize = StrokeThickness;
        }

        internal override void SetStroke()
        {
            Foreground = Stroke;
        }

        internal override void SetStrokeType()
        {
        }

        internal override void SetOpacity()
        {
        }

        ///<summary>
        ///</summary>
        public static readonly PropertyChangedEventArgs TextChangedEventArgs = new PropertyChangedEventArgs("Text");
        ///<summary>
        /// Text that is shown 
        ///</summary>
        public string Text
        {
            get { return _text; }
            set
            {
                if (_text != value)
                {
                    _text = value;
                    InvokePropertyChanged(TextChangedEventArgs);
                }
            }
        }

        ///<summary>
        ///</summary>
        public static readonly PropertyChangedEventArgs FontFamilyChangedEventArgs = new PropertyChangedEventArgs("FontFamily");

        private FontFamily _fontFamily = new FontFamily("Verdana");
        ///<summary>
        /// Gets or sets the FontFamily for text
        ///</summary>
        public FontFamily FontFamily
        {
            get { return _fontFamily; }
            set
            {
                if (_fontFamily != value)
                {
                    _fontFamily = value;
                    InvokePropertyChanged(FontFamilyChangedEventArgs);
                }
            }
        }

        ///<summary>
        ///</summary>
        public static readonly PropertyChangedEventArgs FontSizeChangedEventArgs = new PropertyChangedEventArgs("FontSize");
        private double _fontSize = 12;
        ///<summary>
        /// Gets or sets the FontSize for text
        ///</summary>
        public double FontSize
        {
            get { return _fontSize; }
            set
            {
                if (_fontSize != value)
                {
                    _fontSize = value;
                    InvokePropertyChanged(FontSizeChangedEventArgs);
                }
            }
        }

        /// <summary>
        /// Font name used to show the text
        /// </summary>
        public string FontName
        {
            get { return _fontName; }
            set
            {
                if (_fontName != value)
                {
                    _fontName = value;
                    FontFamily = new FontFamily(_fontName);
                }
            }
        }

        ///<summary>
        ///</summary>
        public static readonly PropertyChangedEventArgs ForegroundChangedEventArgs = new PropertyChangedEventArgs("Foreground");
        private Brush _foreground;
        ///<summary>
        /// Gets or sets the foreground
        ///</summary>
        public Brush Foreground
        {
            get { return _foreground; }
            set
            {
                if (_foreground != value)
                {
                    _foreground = value;
                    InvokePropertyChanged(ForegroundChangedEventArgs);
                }
            }
        }

        #region Implementation of IMouseAble

        ///<summary>
        ///</summary>
        public event MouseButtonEventHandler MouseDown = delegate { };
        ///<summary>
        ///</summary>
        public event MouseEventHandler MouseEnter = delegate { };
        ///<summary>
        ///</summary>
        public event MouseEventHandler MouseLeave = delegate { };
        ///<summary>
        ///</summary>
        public event MouseEventHandler MouseMove = delegate { };
        ///<summary>
        ///</summary>
        public event MouseButtonEventHandler MouseUp = delegate { };

        #endregion

        #region Implementation of IContextAbleLineStudy

        /// <summary>
        /// Element to which context line is bound
        /// </summary>
        public UIElement Element
        {
            get { return _txt; }
        }

        /// <summary>
        /// Segment where context line shall be shown
        /// </summary>
        public Segment Segment
        {
            get { return new Segment(_x2, _y2, _x2 + _txt.ActualWidth, _y2); }
        }

        /// <summary>
        /// Parent where <see cref="IContextAbleLineStudy.Element"/> belongs
        /// </summary>
        public Canvas Parent
        {
            get { return C; }
        }

        /// <summary>
        /// Gets if <see cref="IContextAbleLineStudy.Element"/> is selected
        /// </summary>
        public bool IsSelected
        {
            get { return _selected; }
        }

        /// <summary>
        /// Z Index of <see cref="IContextAbleLineStudy.Element"/>
        /// </summary>
        public int ZIndex
        {
            get { return ZIndexConstants.LineStudies1; }
        }

        /// <summary>
        /// Gets the chart object associated with <see cref="IContextAbleLineStudy.Element"/> object
        /// </summary>
        public StockChartX Chart
        {
            get { return _chartX; }
        }

        /// <summary>
        /// Gets the reference to <see cref="LineStudies.LineStudy"/> 
        /// </summary>
        public LineStudy LineStudy
        {
            get { return this; }
        }

        /// <summary>
        /// Basic properties
        /// </summary>
        protected override IEnumerable<ChartElementProperties.IChartElementProperty> BaseProperties
        {
            get
            {
                GetChartElementColorProperty();
                yield return propertyStroke;

                GetChartElementStrokeThicknessProperty("Font Size");
                yield return propertyStrokeThickness;

                GetChartElementOpacityProperty();
                yield return propertyOpacity;
            }
        }

        #endregion

        #region CanvasLeft

        private double _canvasLeft;

        ///<summary>
        ///</summary>
        public static readonly PropertyChangedEventArgs CanvasLeftChangedEventsArgs =
          ObservableHelper.CreateArgs<StaticText>(_ => _.CanvasLeft);

        ///<summary>
        /// Gets or sets the left position of text object
        ///</summary>
        public double CanvasLeft
        {
            get { return _canvasLeft; }
            set
            {
                if (_canvasLeft != value)
                {
                    _canvasLeft = value;
                    InvokePropertyChanged(CanvasLeftChangedEventsArgs);
                }
            }
        }

        #endregion

        #region CanvasTop

        private double _canvasTop;

        ///<summary>
        ///</summary>
        public static readonly PropertyChangedEventArgs CanvasTopChangedEventsArgs =
          ObservableHelper.CreateArgs<StaticText>(_ => _.CanvasTop);

        ///<summary>
        /// Gets or sets the top position of text object
        ///</summary>
        public double CanvasTop
        {
            get { return _canvasTop; }
            set
            {
                if (_canvasTop != value)
                {
                    _canvasTop = value;
                    InvokePropertyChanged(CanvasTopChangedEventsArgs);
                }
            }
        }

        #endregion
    }
}

