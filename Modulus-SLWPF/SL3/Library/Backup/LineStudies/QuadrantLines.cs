using System;
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
        internal static void Register_QuadrantLines()
        {
            RegisterLineStudy(LineStudy.StudyTypeEnum.QuadrantLines, typeof(QuadrantLines), "Quadrant Lines");
        }
    }
}


namespace ModulusFE.LineStudies
{
    /// <summary>
    /// Quandrant lines line study
    /// </summary>
    public partial class QuadrantLines : LineStudy, IContextAbleLineStudy
    {
        private readonly Line[] _lines = new Line[5];
        private readonly PaintObjectsManager<PaintObjects.Line> _linesSel = new PaintObjectsManager<PaintObjects.Line>();
        private ContextLine _contextLine;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="key">Unique key for line study</param>
        /// <param name="stroke">Stroke brush</param>
        /// <param name="chartPanel">Reference to chart panel where it will be placed.</param>
        public QuadrantLines(string key, Brush stroke, ChartPanel chartPanel)
            : base(key, stroke, chartPanel)
        {
            _studyType = StudyTypeEnum.QuadrantLines;
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

                if (_contextLine == null)
                {
                    _contextLine = new ContextLine(this);
                    _chartX.InvokeLineStudyCreated(new StockChartX.LineStudyCreatedEventArgs(this));
                }

                return;
            }
            // ****************************************************************
            // *Note: This line study requires OHLC series in the owner panel!*
            // ****************************************************************

            rect.Normalize();
            if (rect.Width == 0) return;

            int revX1 = (int)(_chartX.GetReverseXInternal(rect.Left) + _chartX._startIndex);
            int revX2 = (int)(_chartX.GetReverseXInternal(rect.Right) + _chartX._startIndex);
            if (revX1 < 0) revX1 = 0;
            if (revX2 < 0) revX2 = 0;

            if (revX1 == revX2)
            {
                return;
            }

            // Get the highest high of the high series.
            // Note: this code makes the vague assumption
            // that only one symbol exists on this panel.
            Series sHigh = GetSeriesOHLC(SeriesTypeOHLC.High);
            if (sHigh == null) return;
            Series sLow = GetSeriesOHLC(SeriesTypeOHLC.Low);
            if (sLow == null) return;

            double highestHigh = sHigh.MaxFromInterval(ref revX1, ref revX2);
            double lowestLow = sLow.MinFromInterval(ref revX1, ref revX2);

            double value = highestHigh + ((highestHigh - lowestLow) / 4);

            _linesSel.C = C;
            _linesSel.Start();
            for (int i = 0; i < _lines.Length; i++)
            {
                value -= ((highestHigh - lowestLow) / 4);
                rect.Top = _chartPanel.GetY(value);
                Utils.DrawLine(rect.Left, rect.Top, rect.Right, rect.Top,
                               Stroke, i == 2 ? LinePattern.Dot : StrokeType, StrokeThickness, Opacity, _lines[i]);
            }

            if (lineStatus == LineStatus.Moving || lineStatus == LineStatus.Painting)
            {
                Utils.DrawLine(rect.Left, 0, rect.Left, C.ActualHeight, Stroke, LinePattern.Dot, StrokeThickness, Opacity, _linesSel);
                Utils.DrawLine(rect.Right, 0, rect.Right, C.ActualHeight, Stroke, LinePattern.Dot, StrokeThickness, Opacity, _linesSel);
            }

            _linesSel.Stop();

            _internalObjectCreated = true;

            _linesSel.Do(l => l.ZIndex = ZIndexConstants.LineStudies1);
        }

        internal override void SetCursor()
        {
            if (_lines.Length == 0) return;
            Line l = _lines[0];
            if (_selectionVisible)
            {
                foreach (Line line in _lines)
                {
                    line.Cursor = Cursors.Hand;
                }
                return;
            }
            if (_selectionVisible || l.Cursor == Cursors.Arrow) return;
            foreach (Line line in _lines)
            {
                line.Cursor = Cursors.Arrow;
            }
        }

        internal override void Reset()
        {
            _x1Value = _chartX.GetReverseXInternal(_x1) + _chartX._startIndex;
            _x2Value = _chartX.GetReverseXInternal(_x2) + _chartX._startIndex;
            _y1Value = _chartPanel.GetReverseY(_y1);
            _y2Value = _chartPanel.GetReverseY(_y2);
            _x1 = _chartX.GetXPixel(_x1Value - _chartX._startIndex);
            _x2 = _chartX.GetXPixel(_x2Value - _chartX._startIndex);
            if (_lines.Length > 0)
            {
                _y1 = _lines[0].Y1;
                _y2 = _lines[4].Y2;
            }
        }

        internal override List<SelectionDotInfo> GetSelectionPoints()
        {
            return new List<SelectionDotInfo>
               {
                 new SelectionDotInfo {Corner = Types.Corner.MiddleLeft, Position = new Point(_x1, _lines[2].Y1)},
                 new SelectionDotInfo {Corner = Types.Corner.MiddleRight, Position = new Point(_x2, _lines[2].Y1)},
               };
        }

        internal override void SetStrokeThickness()
        {
            foreach (Line line in _lines)
            {
                line.StrokeThickness = StrokeThickness;
            }
        }

        internal override void SetStroke()
        {
            foreach (Line line in _lines)
            {
                line.Stroke = Stroke;
            }
        }

        internal override void SetStrokeType()
        {
            foreach (Line line in _lines)
            {
                Types.SetShapePattern(line, StrokeType);
            }
        }

        internal override void RemoveLineStudy()
        {
            foreach (Line line in _lines)
            {
                C.Children.Remove(line);
            }
            _linesSel.RemoveAll();
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
            get { return _lines[2]; }
        }

        /// <summary>
        /// Segment where context line shall be shown
        /// </summary>
        public Segment Segment
        {
            get { return new Segment(_lines[2].X1, _lines[2].Y1, _lines[2].X2, _lines[2].Y2).Inflate(-10); }
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
