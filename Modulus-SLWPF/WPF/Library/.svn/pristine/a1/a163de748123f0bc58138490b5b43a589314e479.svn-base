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
        internal static void Register_FibonacciFan()
        {
            RegisterLineStudy(LineStudy.StudyTypeEnum.FibonacciFan, typeof(FibonacciFan), "Fibonacci Fan");
        }
    }
}


namespace ModulusFE.LineStudies
{
    /// <summary>
    /// Fibonacci Fan line Study
    /// </summary>
    public partial class FibonacciFan : LineStudy, IContextAbleLineStudy
    {
        private readonly List<Line> _lines = new List<Line>();

        private ContextLine _contextLine;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="key">Unique key for line study</param>
        /// <param name="stroke">Stroke brush</param>
        /// <param name="chartPanel">Reference to chart panel where it will be placed.</param>
        public FibonacciFan(string key, Brush stroke, ChartPanel chartPanel)
            : base(key, stroke, chartPanel)
        {
            _studyType = StudyTypeEnum.FibonacciFan;
        }

        internal override void DrawLineStudy(Types.RectEx rect, LineStatus lineStatus)
        {
            double cy = 0;

            if (rect.IsZero) return;

            if (_lines.Count == 0)
            {
                for (int i = 0; i < 4; i++)
                {
                    Line line = new Line { Tag = this };
                    Canvas.SetZIndex(line, ZIndexConstants.LineStudies1);
                    _lines.Add(line);

                    C.Children.Add(line);
                }

                _internalObjectCreated = true;

                _chartX.InvokeLineStudyCreated(new StockChartX.LineStudyCreatedEventArgs(this));
            }

            double value = (rect.Bottom - rect.Top) / 3;

            rect.Bottom += 15;

            double b = (rect.Top - rect.Bottom) / (rect.Left - rect.Right);
            double c = rect.Top - b * rect.Left;
            double cx = C.ActualWidth;
            cy = (b * cx + c) + cy * 0.3;
            double y = rect.Top;
            double x = rect.Left;
            Utils.DrawLine(x, y, cx, cy, Stroke, StrokeType, StrokeThickness, Opacity, _lines[0]);

            b = ((rect.Top + value * 1) - rect.Bottom) / (rect.Left - rect.Right);
            c = (rect.Top + value * 1) - b * rect.Left;
            cx = C.ActualWidth;
            cy = (b * cx + c) + cy * 0.5;
            y = rect.Top;
            x = rect.Left;
            Utils.DrawLine(x, y, cx, cy, Stroke, StrokeType, StrokeThickness, Opacity, _lines[1]);

            b = (rect.Top + value * 2 - rect.Bottom) / (rect.Left - rect.Right);
            c = rect.Top + value * 2 - b * rect.Left;
            cx = C.ActualWidth;
            cy = (b * cx + c) + cy * 0.8;
            y = rect.Top;
            x = rect.Left;
            Utils.DrawLine(x, y, cx, cy, Stroke, StrokeType, StrokeThickness, Opacity, _lines[2]);
            rect.Bottom -= 15;

            Utils.DrawLine(rect.Left, rect.Top, rect.Right, rect.Bottom, Stroke, StrokeType, StrokeThickness, Opacity, _lines[3]);

            if (_contextLine == null)
                _contextLine = new ContextLine(this);
        }

        internal override void SetCursor()
        {
            if (_lines.Count < 2) return;
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
                 new SelectionDotInfo {Corner = Types.Corner.TopLeft, Position = _newRect.TopLeft},
                 new SelectionDotInfo {Corner = Types.Corner.BottomRight, Position = _newRect.BottomRight},
                 /*new SelectionDotInfo {Corner = Types.Corner.BottomLeft, Position = _newRect.BottomLeft},
                 new SelectionDotInfo {Corner = Types.Corner.BottomRight, Position = _newRect.BottomRight}, */
               };
        }

        internal override void Reset()
        {
            if (_x1 < 1 || _x2 < 1 || _lines.Count == 0) return;

            Line line = _lines[_lines.Count - 1];

            _x1Value = _chartX.GetReverseXInternal(line.X1) + _chartX._startIndex;
            _y1Value = _chartPanel.GetReverseY(line.Y1);
            _x2Value = _chartX.GetReverseXInternal(line.X2) + _chartX._startIndex;
            _y2Value = _chartPanel.GetReverseY(line.Y2);

            _x1 = _chartX.GetXPixel(_x1Value - _chartX._startIndex);
            _y1 = _chartPanel.GetY(_y1Value);
            _x2 = _chartX.GetXPixel(_x2Value - _chartX._startIndex);
            _y2 = _chartPanel.GetY(_y2Value);
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
        }

        internal override void SetOpacity()
        {
            foreach (var line in _lines)
            {
                line.Opacity = Opacity;
            }
        }

        #region Implementation of IContextAbleLineStudy

        /// <summary>
        /// Element to which context line is bound
        /// </summary>
        public UIElement Element
        {
            get { return _lines[3]; }
        }

        /// <summary>
        /// Segment where context line shall be shown
        /// </summary>
        public Segment Segment
        {
            get
            {
                return new Segment(_lines[3].X1, _lines[3].Y1, _lines[3].X2, _lines[3].Y2).Inflate(-10);
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
    }
}
