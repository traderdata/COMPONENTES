using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ModulusFE.LineStudies;
using ModulusFE.PaintObjects;

namespace ModulusFE
{
    public static partial class StockChartX_LineStudiesParams
    {
        internal static void Register_ImageObject()
        {
            RegisterLineStudy(LineStudy.StudyTypeEnum.ImageObject, typeof(ImageObject), "Image Object");
        }
    }
}

namespace ModulusFE.LineStudies
{
    /// <summary>
    /// Image type line study. It can show external image, image from resource
    /// </summary>
    public partial class ImageObject : LineStudy, IMouseAble, IContextAbleLineStudy
    {
        ///<summary>
        /// Specifies where the image hotspot (current X &amp; Y coordinates) is located
        ///</summary>
        public enum ImageAlign
        {
            ///<summary>
            /// Top Left
            ///</summary>
            TopLeft,
            ///<summary>
            /// Top Middle
            ///</summary>
            TopMiddle,
            ///<summary>
            /// Top Right
            ///</summary>
            TopRight,
            ///<summary>
            /// Bottom Left
            ///</summary>
            BottomLeft,
            ///<summary>
            /// Bottom Middle
            ///</summary>
            BottomMiddle,
            ///<summary>
            /// Bottom Right
            ///</summary>
            BottomRight,
            ///<summary>
            /// Left Middle
            ///</summary>
            LeftMiddle,
            ///<summary>
            /// Right Middle
            ///</summary>
            RightMiddle,
            ///<summary>
            /// Image center
            ///</summary>
            Center
        }

        private Image _image;
        private string _imagePath;
        private ContextLine _contextLine;
        private Size? _actualSize;
        private bool _firstResize;
        private Types.RectEx _firstRect;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="key">Unique key for line study</param>
        /// <param name="stroke">Stroke brush</param>
        /// <param name="chartPanel">Reference to chart panel where it will be placed.</param>
        public ImageObject(string key, Brush stroke, ChartPanel chartPanel)
            : base(key, stroke, chartPanel)
        {
            _studyType = StudyTypeEnum.ImageObject;

            Align = ImageAlign.Center;
        }

        ///<summary>
        /// Gets or sets image' hot spot position
        ///</summary>
        public ImageAlign Align { get; set; }

        internal override void SetArgs(params object[] args)
        {
            Debug.Assert(args.Length > 0);
            _extraArgs = args;

            if (args[0].GetType() == typeof(string))
            {
                _imagePath = args[0].ToString();
            }
            else
            {
                SymbolType symbolType = (SymbolType)args[0];
#if WPF
        const string PackString = "pack://application:,,,/ModulusFE.StockChartX;component/Images/";
#endif
#if SILVERLIGHT
                const string PackString = "/ModulusFE.StockChartX.SL;component/Images/";
#endif
                switch (symbolType)
                {
                    case SymbolType.Buy:
                        _imagePath = PackString + "buy.png";
                        break;
                    case SymbolType.Sell:
                        _imagePath = PackString + "sell.png";
                        break;
                    case SymbolType.ExitLong:
                        _imagePath = PackString + "ExitLong.png";
                        break;
                    case SymbolType.ExitShort:
                        _imagePath = PackString + "ExitShort.png";
                        break;
                    case SymbolType.Signal:
                        _imagePath = PackString + "SignalPrice.png";
                        break;
                }
            }

            if (args.Length > 1)
            {
                if (args[1].GetType() == typeof(Size))
                {
                    _actualSize = (Size)args[1];
                }
            }
        }

        internal override void DrawLineStudy(Types.RectEx rect, LineStatus lineStatus)
        {
            if (lineStatus == LineStatus.StartPaint)
            {
                Debug.Assert(_imagePath.Length > 0);
                _image = new Image { Tag = this, Visibility = Visibility.Collapsed, Stretch = Stretch.None };
                _image.SetBinding(Canvas.TopProperty, this.CreateOneWayBinding("CanvasTop"));
                _image.SetBinding(Canvas.LeftProperty, this.CreateOneWayBinding("CanvasLeft"));
                _image.SetBinding(ToolTipService.ToolTipProperty, this.CreateOneWayBinding("Tooltip"));

#if WPF
        BitmapImage bi = new BitmapImage();
        bi.BeginInit();
        bi.CacheOption = BitmapCacheOption.OnLoad;
        bi.UriSource = new Uri(_imagePath, UriKind.RelativeOrAbsolute);
        bi.EndInit();
        _image.Source = bi;
#endif
#if SILVERLIGHT
                _image.SetValue(Image.SourceProperty, new BitmapImage(new Uri(_imagePath, UriKind.RelativeOrAbsolute)));
#endif

                _image.MouseLeftButtonDown += (sender, args) => MouseDown(sender, args);
                _image.MouseEnter += (sender, args) => MouseEnter(sender, args);
                _image.MouseLeave += (sender, args) => MouseLeave(sender, args);
                _image.MouseMove += (sender, args) => MouseMove(sender, args);
                _image.MouseLeftButtonUp += (sender, args) => MouseUp(sender, args);
                _firstResize = true;
                _firstRect = rect;
                _image.SizeChanged += (o, eventArgs) =>
                                        {
                                            if (_firstResize)
                                            {
                                                _firstResize = false;
                                                SetImagePos(_firstRect);
                                            }
                                        };

                C.Children.Add(_image);

                Canvas.SetZIndex(_image, ZIndexConstants.LineStudies1);

                if (_contextLine == null)
                    _contextLine = new ContextLine(this);

                _internalObjectCreated = true;
                _chartX.InvokeLineStudyCreated(new StockChartX.LineStudyCreatedEventArgs(this));

                return;
            }

            if (_firstResize)
                _firstRect = rect;

            SetImagePos(rect);
        }

        private void SetImagePos(Types.RectEx rect)
        {
            double top;
            double left;
            int x = (int)rect.Right;
            int y = (int)rect.Bottom;
            double height = _actualSize.HasValue ? _actualSize.Value.Height : _image.ActualHeight;
            double width = _actualSize.HasValue ? _actualSize.Value.Width : _image.ActualWidth;

            if (_image.Visibility == Visibility.Collapsed)
                _image.Visibility = Visibility.Visible;

            switch (Align)
            {
                case ImageAlign.TopLeft:
                    top = y;
                    left = x;
                    break;
                case ImageAlign.TopMiddle:
                    top = y;
                    left = x - width / 2;
                    break;
                case ImageAlign.TopRight:
                    top = y;
                    left = x - width;
                    break;
                case ImageAlign.BottomLeft:
                    top = y - height;
                    left = x;
                    break;
                case ImageAlign.BottomMiddle:
                    top = y - height;
                    left = x - width / 2;
                    break;
                case ImageAlign.BottomRight:
                    top = y - height;
                    left = x - width;
                    break;
                case ImageAlign.LeftMiddle:
                    top = y - height / 2;
                    left = x;
                    break;
                case ImageAlign.RightMiddle:
                    top = y - height / 2;
                    left = x - width;
                    break;
                case ImageAlign.Center:
                    top = y - height / 2;
                    left = x - width / 2;
                    break;
                default:
                    throw new ArgumentException("Align property must be set.");
            }
            //Canvas.SetTop(_image, top);
            //Canvas.SetLeft(_image, left);
            CanvasLeft = left;
            CanvasTop = top;
        }

        internal override void SetCursor()
        {
            if (_selectionVisible && _image.Cursor != Cursors.Hand)
            {
                _image.Cursor = Cursors.Hand;
                return;
            }
            if (_selectionVisible || _image.Cursor == Cursors.Arrow) return;
            _image.Cursor = Cursors.Arrow;
        }

        internal override void SetStrokeThickness()
        {
        }

        internal override void SetStroke()
        {
        }

        internal override void SetStrokeType()
        {
        }

        internal override void SetOpacity()
        {
            _image.Opacity = Opacity;
        }

        internal override List<SelectionDotInfo> GetSelectionPoints()
        {
            double left = CanvasLeft;
            double top = CanvasTop;

            return new List<SelectionDotInfo>
               {
                 new SelectionDotInfo
                   {
                     Corner = Types.Corner.TopLeft,
                     Clickable = false,
                     Position = new Point(left, top)
                   },
                 new SelectionDotInfo
                   {
                     Clickable = false,
                     Corner = Types.Corner.TopRight,
                     Position = new Point(left + _image.ActualWidth, _y2)
                   },
                 new SelectionDotInfo
                   {
                     Clickable = false,
                     Corner = Types.Corner.BottomLeft,
                     Position = new Point(left, top + _image.ActualHeight)
                   },
                 new SelectionDotInfo
                   {
                     Clickable = false,
                     Corner = Types.Corner.BottomRight,
                     Position = new Point(left + _image.ActualWidth, top + _image.ActualHeight)
                   }
               };
        }

        internal override void RemoveLineStudy()
        {
            C.Children.Remove(_image);
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
            get { return _image; }
        }

        /// <summary>
        /// Segment where context line shall be shown
        /// </summary>
        public Segment Segment
        {
            get
            {
                double left = Canvas.GetLeft(_image);
                double top = Canvas.GetTop(_image);
                return new Segment(new Point(left, top), 20, 45);
            }
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

        #endregion

        #region CanvasLeft

        private double _canvasLeft;

        ///<summary>
        ///</summary>
        public static readonly PropertyChangedEventArgs CanvasLeftChangedEventsArgs =
          ObservableHelper.CreateArgs<ImageObject>(_ => _.CanvasLeft);

        ///<summary>
        /// Gets or sets the Left position of image
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

        /// <summary>
        /// 
        /// </summary>
        public static readonly PropertyChangedEventArgs CanvasTopChangedEventsArgs =
          ObservableHelper.CreateArgs<ImageObject>(_ => _.CanvasTop);

        /// <summary>
        /// Gets or sets the top position of image
        /// </summary>
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

        #region Tooltip

        private string _tooltip = string.Empty;

        ///<summary>
        ///</summary>
        public static readonly PropertyChangedEventArgs TooltipChangedEventsArgs =
          ObservableHelper.CreateArgs<ImageObject>(_ => _.Tooltip);

        ///<summary>
        /// Gets or sets the tooltip for this image object
        ///</summary>
        public string Tooltip
        {
            get { return _tooltip; }
            set
            {
                if (_tooltip != value)
                {
                    _tooltip = value;
                    InvokePropertyChanged(TooltipChangedEventsArgs);
                }
            }
        }

        #endregion
    }
}
