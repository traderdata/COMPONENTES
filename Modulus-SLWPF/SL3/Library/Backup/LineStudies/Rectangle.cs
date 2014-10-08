using System.Collections.Generic;
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
        internal static void Register_Rectangle()
        {
            RegisterLineStudy(LineStudy.StudyTypeEnum.Rectangle, typeof(LineStudies.Rectangle), "Rectangle");
        }
    }
}


namespace ModulusFE.LineStudies
{
    /// <summary>
    /// Rectangle
    /// </summary>
    public partial class Rectangle : LineStudy, IShapeAble, IMouseAble, IContextAbleLineStudy
    {
        private readonly System.Windows.Shapes.Rectangle _rectangle;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="key">Unique key for line study</param>
        /// <param name="stroke">Stroke brush</param>
        /// <param name="chartPanel">Reference to chart panel where it will be placed.</param>
        public Rectangle(string key, Brush stroke, ChartPanel chartPanel)
            : base(key, stroke, chartPanel)
        {
            _studyType = StudyTypeEnum.Rectangle;
            _rectangle = new System.Windows.Shapes.Rectangle();
        }

        ///<summary>
        /// Gets or sets the Brush that specifies how the shape's interior is filled
        ///</summary>
        public Brush Fill
        {
            get { return _rectangle.Fill; }
            set { _rectangle.Fill = value; }
        }

        internal override void DrawLineStudy(Types.RectEx rect, LineStatus lineStatus)
        {
            if (!_internalObjectCreated && lineStatus != LineStatus.StartPaint)
                DrawLineStudy(rect, LineStatus.StartPaint);
            if (lineStatus == LineStatus.StartPaint)
            {
                _rectangle.StrokeThickness = StrokeThickness;
                _rectangle.Stroke = Stroke;
                if (_rectangle.Fill == null)
                    _rectangle.Fill = new SolidColorBrush(Colors.Transparent);
                _rectangle.Tag = this;
                C.Children.Add(_rectangle);
                Canvas.SetZIndex(_rectangle, ZIndexConstants.LineStudies1);

                _rectangle.MouseLeftButtonDown += (sender, args) => MouseDown(sender, args);
                _rectangle.MouseEnter += (sender, args) => MouseEnter(sender, args);
                _rectangle.MouseLeave += (sender, args) => MouseLeave(sender, args);
                _rectangle.MouseMove += (sender, args) => MouseMove(sender, args);
                _rectangle.MouseLeftButtonUp += (sender, args) => MouseUp(sender, args);

                new ContextLine(this);

                _internalObjectCreated = true;

                _chartX.InvokeLineStudyCreated(new StockChartX.LineStudyCreatedEventArgs(this));

                return;
            }
            rect.Normalize();
            Canvas.SetLeft(_rectangle, rect.Left);
            Canvas.SetTop(_rectangle, rect.Top);
            _rectangle.Width = rect.Width;
            _rectangle.Height = rect.Height;
        }

        internal override List<SelectionDotInfo> GetSelectionPoints()
        {
            List<SelectionDotInfo> res =
              new List<SelectionDotInfo>
          {
            new SelectionDotInfo {Corner = Types.Corner.TopLeft, Position = _newRect.TopLeft},
            new SelectionDotInfo {Corner = Types.Corner.TopRight, Position = _newRect.TopRight},
            new SelectionDotInfo {Corner = Types.Corner.BottomLeft, Position = _newRect.BottomLeft},
            new SelectionDotInfo {Corner = Types.Corner.BottomRight, Position = _newRect.BottomRight},
          };
            if (_rectangle.ActualWidth > Constants.SelectionDotSize * 4)
            {
                res.Add(new SelectionDotInfo { Corner = Types.Corner.TopCenter, Position = _newRect.TopCenter });
                res.Add(new SelectionDotInfo { Corner = Types.Corner.BottomCenter, Position = _newRect.BottomCenter });
            }

            if (_rectangle.ActualHeight > Constants.SelectionDotSize * 4)
            {
                res.Add(new SelectionDotInfo { Corner = Types.Corner.MiddleLeft, Position = _newRect.MiddleLeft });
                res.Add(new SelectionDotInfo { Corner = Types.Corner.MiddleRight, Position = _newRect.MiddleRight });
            }

            return res;
        }

        internal override void SetCursor()
        {
            if (_selectionVisible && _rectangle.Cursor != Cursors.Hand)
            {
                _rectangle.Cursor = Cursors.Hand;
                return;
            }
            if (_selectionVisible || _rectangle.Cursor == Cursors.Arrow) return;
            _rectangle.Cursor = Cursors.Arrow;
        }

        internal override void SetStrokeThickness()
        {
            _rectangle.StrokeThickness = StrokeThickness;
        }

        internal override void SetStroke()
        {
            _rectangle.Stroke = Stroke;
        }

        internal override void SetStrokeType()
        {
            Types.SetShapePattern(_rectangle, StrokeType);
        }

        internal override void RemoveLineStudy()
        {
            C.Children.Remove(_rectangle);
        }

        internal override void SetOpacity()
        {
            _rectangle.Opacity = Opacity;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="paintableRect"></param>
        protected override void SetClipingAreaInternal(Rect? paintableRect)
        {
            //      if (_rectangle != null)
            //      {
            //        _rectangle.Clip = GetClip(Canvas.GetLeft(_rectangle), Canvas.GetTop(_rectangle), paintableRect);
            //      }
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
            get { return _rectangle; }
        }

        /// <summary>
        /// Segment where context line shall be shown
        /// </summary>
        public Segment Segment
        {
            get { return new Segment(_newRect.Left, _newRect.Top, _newRect.Right, _newRect.Bottom).Normalize(); }
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
    }
}
