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
        internal static void Register_FibonacciTimeZones()
        {
            RegisterLineStudy(LineStudy.StudyTypeEnum.FibonacciTimeZones, typeof(FibonacciTimeZones), "Fibonacci Time Zones");
        }
    }
}


namespace ModulusFE.LineStudies
{
    /// <summary>
    /// Fibonacci Time Zones line study
    /// </summary>
    public partial class FibonacciTimeZones : LineStudy, IContextAbleLineStudy
    {
        private readonly PaintObjectsManager<Line> _lines = new PaintObjectsManager<Line>();
        private readonly double[] _fib = new[] { 0.0557, 0.0902, 0.118, 0.1459, 0.2361, 0.381, 0.618 };
        private ContextLine _contextLine;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="key">Unique key for line study</param>
        /// <param name="stroke">Stroke brush</param>
        /// <param name="chartPanel">Reference to chart panel where it will be placed.</param>
        public FibonacciTimeZones(string key, Brush stroke, ChartPanel chartPanel)
            : base(key, stroke, chartPanel)
        {
            _studyType = StudyTypeEnum.FibonacciTimeZones;
        }

        private bool _firstPaint = true;
        internal override void DrawLineStudy(Types.RectEx rect, LineStatus lineStatus)
        {
            rect.Normalize();

            _lines.C = C;
            _lines.Start();

            Utils.DrawLine(rect.Left, rect.Top, rect.Left, rect.Bottom, Stroke, StrokeType, StrokeThickness, Opacity, _lines);
            Utils.DrawLine(rect.Right, rect.Top, rect.Right, rect.Bottom, Stroke, StrokeType, StrokeThickness, Opacity, _lines);

            foreach (double d in _fib)
            {
                double x = rect.Left + rect.Width * d;
                Utils.DrawLine(x, rect.Top, x, rect.Bottom, Stroke, StrokeType, StrokeThickness, Opacity, _lines);
            }

            _lines.Stop();

            _internalObjectCreated = true;

            if (_firstPaint)
            {
                _firstPaint = false;

                _lines.Do(l =>
                            {
                                l._line.Tag = this;
                                l.ZIndex = ZIndexConstants.LineStudies1;
                            });

                _contextLine = new ContextLine(this);
                _chartX.InvokeLineStudyCreated(new StockChartX.LineStudyCreatedEventArgs(this));
            }
        }

        internal override void SetCursor()
        {
            if (_lines.Count == 0) return;
            if (_selectionVisible)
            {
                _lines.Do(l => l._line.Cursor = Cursors.Hand);
                return;
            }
            if (_selectionVisible || _lines.GetPaintObject()._line.Cursor == Cursors.Arrow) return;
            _lines.Do(l => l._line.Cursor = Cursors.Arrow);
        }

        internal override List<SelectionDotInfo> GetSelectionPoints()
        {
            return new List<SelectionDotInfo>
               {
                 new SelectionDotInfo {Corner = Types.Corner.TopLeft, Position = _newRect.TopLeft},
                 new SelectionDotInfo {Corner = Types.Corner.TopRight, Position = _newRect.TopRight},
                 new SelectionDotInfo {Corner = Types.Corner.BottomLeft, Position = _newRect.BottomLeft},
                 new SelectionDotInfo {Corner = Types.Corner.BottomRight, Position = _newRect.BottomRight},
               };
        }

        internal override void SetStrokeThickness()
        {
            _lines.Do(l => l._line.StrokeThickness = StrokeThickness);
        }

        internal override void SetStroke()
        {
            _lines.Do(l => l._line.Stroke = Stroke);
        }

        internal override void SetStrokeType()
        {
            _lines.Do(l => Types.SetShapePattern(l._line, StrokeType));
        }

        internal override void RemoveLineStudy()
        {
            _lines.RemoveAll();
        }

        internal override void SetOpacity()
        {
            _lines.Do(line => line._line.Opacity = Opacity);
        }

        #region Implementation of IContextAbleLineStudy

        /// <summary>
        /// Element to which context line is bound
        /// </summary>
        public UIElement Element
        {
            get { return _lines[0]._line; }
        }

        /// <summary>
        /// Segment where context line shall be shown
        /// </summary>
        public Segment Segment
        {
            get { return new Segment(_newRect.Left, _newRect.Top, _newRect.Left, _newRect.Bottom).Normalize().Inflate(-10); }
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

