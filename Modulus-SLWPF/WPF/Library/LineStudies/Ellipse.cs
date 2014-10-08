using System;
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
        internal static void Register_Ellipse()
        {
            RegisterLineStudy(LineStudy.StudyTypeEnum.Ellipse, typeof(LineStudies.Ellipse), "Ellipse");
        }
    }
}

namespace ModulusFE.LineStudies
{
    /// <summary>
    /// Ellipse line study
    /// </summary>
    public partial class Ellipse : LineStudy, IShapeAble, IMouseAble, IContextAbleLineStudy
    {
        private readonly System.Windows.Shapes.Ellipse _ellipse;

        private ContextLine _contextLine;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="key">Unique key for line study</param>
        /// <param name="stroke">Stroke brush</param>
        /// <param name="chartPanel">Reference to chart panel where it will be placed.</param>
        public Ellipse(string key, Brush stroke, ChartPanel chartPanel)
            : base(key, stroke, chartPanel)
        {
            _studyType = StudyTypeEnum.Ellipse;
            _ellipse = new System.Windows.Shapes.Ellipse();
            _internalObjectCreated = true;
        }

        internal override void DrawLineStudy(Types.RectEx rect, LineStatus lineStatus)
        {
            //      if (_ellipse == null && lineStatus != LineStatus.StartPaint)
            //        DrawLineStudy(rect, LineStatus.StartPaint);
            //
            //      if (_ellipse == null)
            //        throw new NullReferenceException();
            //
            if (lineStatus == LineStatus.StartPaint)
            {
                _ellipse.StrokeThickness = StrokeThickness;
                _ellipse.Stroke = Stroke;
                if (_ellipse.Fill == null)
                    _ellipse.Fill = new SolidColorBrush(Colors.Transparent);
                _ellipse.Tag = this;
                C.Children.Add(_ellipse);

                if (_contextLine == null)
                    _contextLine = new ContextLine(this);

                _ellipse.MouseLeftButtonDown += (sender, args) => MouseDown(sender, args);
                _ellipse.MouseEnter += (sender, args) => MouseEnter(sender, args);
                _ellipse.MouseLeave += (sender, args) => MouseLeave(sender, args);
                _ellipse.MouseMove += (sender, args) => MouseMove(sender, args);
                _ellipse.MouseLeftButtonUp += (sender, args) => MouseUp(sender, args);

                Canvas.SetZIndex(_ellipse, ZIndexConstants.LineStudies1);

                _chartX.InvokeLineStudyCreated(new StockChartX.LineStudyCreatedEventArgs(this));

                return;
            }
            rect.Normalize();
            Canvas.SetLeft(_ellipse, rect.Left);
            Canvas.SetTop(_ellipse, rect.Top);
            _ellipse.Width = rect.Width;
            _ellipse.Height = rect.Height;


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
            if (_ellipse.ActualWidth > Constants.SelectionDotSize * 4)
            {
                res.Add(new SelectionDotInfo { Corner = Types.Corner.TopCenter, Position = _newRect.TopCenter });
                res.Add(new SelectionDotInfo { Corner = Types.Corner.BottomCenter, Position = _newRect.BottomCenter });
            }

            if (_ellipse.ActualHeight > Constants.SelectionDotSize * 4)
            {
                res.Add(new SelectionDotInfo { Corner = Types.Corner.MiddleLeft, Position = _newRect.MiddleLeft });
                res.Add(new SelectionDotInfo { Corner = Types.Corner.MiddleRight, Position = _newRect.MiddleRight });
            }

            return res;
        }

        internal override void SetCursor()
        {
            if (_selectionVisible && _ellipse.Cursor != Cursors.Hand)
            {
                _ellipse.Cursor = Cursors.Hand;
                return;
            }
            if (_selectionVisible || _ellipse.Cursor == Cursors.Arrow) return;
            _ellipse.Cursor = Cursors.Arrow;
        }

        internal override void SetStrokeThickness()
        {
            _ellipse.StrokeThickness = StrokeThickness;
        }

        internal override void SetStroke()
        {
            _ellipse.Stroke = Stroke;
        }

        internal override void SetStrokeType()
        {
            Types.SetShapePattern(_ellipse, StrokeType);
        }

        internal override void RemoveLineStudy()
        {
            C.Children.Remove(_ellipse);
        }

        internal override void SetOpacity()
        {
            _ellipse.Opacity = Opacity;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="paintableRect"></param>
        protected override void SetClipingAreaInternal(Rect? paintableRect)
        {
            //      if (_ellipse != null)
            //      {
            //        _ellipse.Clip = GetClip(Canvas.GetLeft(_ellipse), Canvas.GetTop(_ellipse), paintableRect);
            //      }
        }

        /// <summary>
        /// Gets or sets the background the object's background
        /// </summary>
        public Brush Fill
        {
            get { return _ellipse.Fill; }
            set { _ellipse.Fill = value; }
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

        ///<summary>
        ///</summary>
        public UIElement Element
        {
            get { return _ellipse; }
        }

        ///<summary>
        ///</summary>
        public Segment Segment
        {
            get { return new Segment(_newRect.Left, _newRect.Top, _newRect.Right, _newRect.Bottom).Normalize(); }
        }

        ///<summary>
        ///</summary>
        public Canvas Parent
        {
            get { return C; }
        }

        ///<summary>
        ///</summary>
        public bool IsSelected
        {
            get { return _selected; }
        }

        ///<summary>
        ///</summary>
        public int ZIndex
        {
            get { return ZIndexConstants.LineStudies1; }
        }

        /// <summary>
        /// 
        /// </summary>
        public StockChartX Chart
        {
            get { return _chartX; }
        }

        /// <summary>
        /// 
        /// </summary>
        public LineStudy LineStudy
        {
            get { return this; }
        }
        #endregion
    }
}
