using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ModulusFE.LineStudies;
using ModulusFE.PaintObjects;
using Line = System.Windows.Shapes.Line;

namespace ModulusFE
{
    public static partial class StockChartX_LineStudiesParams
    {
        internal static void Register_SpeedLines()
        {
            RegisterLineStudy(LineStudy.StudyTypeEnum.SpeedLines, typeof(SpeedLines), "Speed Lines");
        }
    }
}


namespace ModulusFE.LineStudies
{
    /// <summary>
    /// Speed Lines line study
    /// </summary>
    public partial class SpeedLines : LineStudy, IContextAbleLineStudy
    {
        private readonly Line[] _lines = new Line[3];
        private Line _handle;
        private ContextLine _contextLine;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="key">Unique key for line study</param>
        /// <param name="stroke">Stroke brush</param>
        /// <param name="chartPanel">Reference to chart panel where it will be placed.</param>
        public SpeedLines(string key, Brush stroke, ChartPanel chartPanel)
            : base(key, stroke, chartPanel)
        {
            _studyType = StudyTypeEnum.SpeedLines;
        }

        internal override void DrawLineStudy(Types.RectEx rect, LineStatus lineStatus)
        {
            if (lineStatus == LineStatus.StartPaint)
            {
                for (int i = 0; i < _lines.Length; i++)
                {
                    _lines[i] = new Line { Tag = this };
                    Canvas.SetZIndex(_lines[i], ZIndexConstants.LineStudies1);
                    C.Children.Add(_lines[i]);
                }
                _handle = new Line { Tag = this };
                Canvas.SetZIndex(_handle, ZIndexConstants.LineStudies1);
                C.Children.Add(_handle);

                if (_contextLine == null)
                    _contextLine = new ContextLine(this);

                _internalObjectCreated = true;

                _chartX.InvokeLineStudyCreated(new StockChartX.LineStudyCreatedEventArgs(this));

                return;
            }

            if (rect.IsZero)
                return;

            // Draw  three trend lines that go out from 
            // the three horizontal box sections to form the speed lines.
            int value = (int)((rect.Bottom - rect.Top) / 3);

            rect.Bottom += 15;
            double b = (rect.Top - rect.Bottom) / (rect.Left - rect.Right);
            double c = rect.Top - b * rect.Left;
            double cx = C.ActualWidth;
            double cy = (b * cx + c);
            double y = rect.Top;
            double x = rect.Left;
            Utils.DrawLine(x, y, cx, cy, Stroke, StrokeType, StrokeThickness, Opacity, _lines[0]);

            b = ((rect.Top + value * 1) - rect.Bottom) / (rect.Left - rect.Right);
            c = (rect.Top + value * 1) - b * rect.Left;
            cx = C.ActualWidth;
            cy = (b * cx + c);
            y = rect.Top;
            x = rect.Left;
            Utils.DrawLine(x, y, cx, cy, Stroke, StrokeType, StrokeThickness, Opacity, _lines[1]);

            b = (rect.Top + value * 2 - rect.Bottom) / (rect.Left - rect.Right);
            c = rect.Top + value * 2 - b * rect.Left;
            cx = C.ActualWidth;
            cy = (b * cx + c);
            y = rect.Top;
            x = rect.Left;
            Utils.DrawLine(x, y, cx, cy, Stroke, StrokeType, StrokeThickness, Opacity, _lines[2]);
            rect.Bottom -= 15;

            // Draw the handle
            Utils.DrawLine(rect.Left, rect.Top, rect.Right, rect.Bottom, Stroke, StrokeType, StrokeThickness, Opacity, _handle);
        }

        internal override void SetCursor()
        {
            if (_lines.Length == 0) return;
            Line line = _lines[0];
            if (_selectionVisible)
            {
                foreach (Line l in _lines)
                {
                    l.Cursor = Cursors.Hand;
                }
                return;
            }
            if (_selectionVisible || line.Cursor == Cursors.Arrow) return;
            foreach (Line l in _lines)
            {
                l.Cursor = Cursors.Arrow;
            }
        }

        internal override List<SelectionDotInfo> GetSelectionPoints()
        {
            return new List<SelectionDotInfo>
               {
                 new SelectionDotInfo {Corner = Types.Corner.TopLeft, Position = new Point(_x1, _y1)},
//                 new SelectionDotInfo {Corner = Types.Corner.TopRight, Position = new Point(_x2, _y1)},
//                 new SelectionDotInfo {Corner = Types.Corner.BottomLeft, Position = new Point(_x1, _y2)},
                 new SelectionDotInfo {Corner = Types.Corner.BottomRight, Position = new Point(_x2, _y2)},
               };
        }

        internal override void SetStrokeThickness()
        {
            foreach (Line l in _lines)
            {
                l.StrokeThickness = StrokeThickness;
            }
        }

        internal override void SetStroke()
        {
            foreach (Line l in _lines)
            {
                l.Stroke = Stroke;
            }
        }

        internal override void SetStrokeType()
        {
            foreach (Line l in _lines)
            {
                Types.SetShapePattern(l, StrokeType);
            }
        }

        internal override void RemoveLineStudy()
        {
            foreach (Line l in _lines)
            {
                C.Children.Remove(l);
            }
            C.Children.Remove(_handle);
        }

        internal override void SetOpacity()
        {
            foreach (var line in _lines)
            {
                line.Opacity = Opacity;
            }
            _handle.Opacity = Opacity;
        }

        #region Implementation of IContextAbleLineStudy

        /// <summary>
        /// Element to which context line is bound
        /// </summary>
        public UIElement Element
        {
            get { return _handle; }
        }

        /// <summary>
        /// Segment where context line shall be shown
        /// </summary>
        public Segment Segment
        {
            get { return new Segment(_handle.X1, _handle.Y1, _handle.X2, _handle.Y2).Inflate(-10); }
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

