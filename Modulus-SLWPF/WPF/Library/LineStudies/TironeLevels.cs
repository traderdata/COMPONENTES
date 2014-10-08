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
        internal static void Register_TironeLevels()
        {
            RegisterLineStudy(LineStudy.StudyTypeEnum.TironeLevels, typeof(TironeLevels), "Tirone Levels");
        }
    }
}


namespace ModulusFE.LineStudies
{
    ///<summary>
    /// Tirone Levels line study
    ///</summary>
    public partial class TironeLevels : LineStudy, IContextAbleLineStudy
    {
        private readonly System.Windows.Shapes.Line[] _lines = new System.Windows.Shapes.Line[3];
        private readonly PaintObjectsManager<Line> _linesSel = new PaintObjectsManager<Line>();
        private ContextLine _contextLine;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="key">Unique key for line study</param>
        /// <param name="stroke">Stroke brush</param>
        /// <param name="chartPanel">Reference to chart panel where it will be placed.</param>
        public TironeLevels(string key, Brush stroke, ChartPanel chartPanel)
            : base(key, stroke, chartPanel)
        {
            _studyType = StudyTypeEnum.TironeLevels;
        }

        internal override void DrawLineStudy(Types.RectEx rect, LineStatus lineStatus)
        {
            if (lineStatus == LineStatus.StartPaint)
            {
                for (int i = 0; i < _lines.Length; i++)
                {
                    _lines[i] = new System.Windows.Shapes.Line { Tag = this };
                    Canvas.SetZIndex(_lines[i], ZIndexConstants.LineStudies1);
                    C.Children.Add(_lines[i]);
                }

                if (_contextLine == null)
                {
                    _contextLine = new ContextLine(this);
                    _chartX.InvokeLineStudyCreated(new StockChartX.LineStudyCreatedEventArgs(this));
                }

                _internalObjectCreated = true;

                return;
            }

            rect.Normalize();

            if (rect.Width == 0) return;

            int revX1 = (int)(_chartX.GetReverseXInternal(rect.Left) + _chartX._startIndex);
            int revX2 = (int)(_chartX.GetReverseXInternal(rect.Right) + _chartX._startIndex);
            if (revX1 < 0) revX1 = 0;
            if (revX2 < 0) revX2 = 0;

            if (revX1 == revX2) return;

            // Get the highest high of the high series.
            // Note: this code makes the vague assumption
            // that only one symbol exists on this panel.
            Series sHigh = GetSeriesOHLC(SeriesTypeOHLC.High);
            if (sHigh == null) return;
            Series sLow = GetSeriesOHLC(SeriesTypeOHLC.Low);
            if (sLow == null) return;

            double highestHigh = sHigh.MaxFromInterval(ref revX1, ref revX2);
            double lowestLow = sLow.MinFromInterval(ref revX1, ref revX2);

            _linesSel.C = C;

            double value = highestHigh - ((highestHigh - lowestLow) / 3);

            rect.Top = _chartPanel.GetY(value);
            Utils.DrawLine(rect.Left, rect.Top, rect.Right, rect.Top, Stroke, LinePattern.Dot, StrokeThickness, Opacity, _lines[0]);

            value = lowestLow + (highestHigh - lowestLow) / 2;
            rect.Top = _chartPanel.GetY(value);
            Utils.DrawLine(rect.Left, rect.Top, rect.Right, rect.Top, Stroke, LinePattern.Dot, StrokeThickness, Opacity, _lines[1]);

            value = lowestLow + (highestHigh - lowestLow) / 3;
            rect.Top = _chartPanel.GetY(value);
            Utils.DrawLine(rect.Left, rect.Top, rect.Right, rect.Top, Stroke, LinePattern.Dot, StrokeThickness, Opacity, _lines[2]);

            _linesSel.Start();
            if (lineStatus == LineStatus.Painting || lineStatus == LineStatus.Moving)
            {
                Utils.DrawLine(rect.Left, 0, rect.Left, C.ActualHeight, Stroke, LinePattern.Solid, StrokeThickness, Opacity, _linesSel);
                Utils.DrawLine(rect.Right, 0, rect.Right, C.ActualHeight, Stroke, LinePattern.Solid, StrokeThickness, Opacity, _linesSel);
            }

            _linesSel.Stop();
            _linesSel.Do(l => l.ZIndex = ZIndexConstants.LineStudies1);
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
                _y2 = _lines[2].Y2;
            }
        }

        internal override List<SelectionDotInfo> GetSelectionPoints()
        {
            return new List<SelectionDotInfo>
               {
                 new SelectionDotInfo {Corner = Types.Corner.MiddleLeft, Position = new Point(_x1, _lines[1].Y1)},
                 new SelectionDotInfo {Corner = Types.Corner.MiddleRight, Position = new Point(_x2, _lines[1].Y2)},
               };

        }

        internal override void SetCursor()
        {
            if (_lines.Length == 0) return;
            System.Windows.Shapes.Line line = _lines[0];
            if (_selectionVisible)
            {
                foreach (System.Windows.Shapes.Line l in _lines)
                {
                    l.Cursor = Cursors.Hand;
                }
                return;
            }
            if (_selectionVisible || line.Cursor == Cursors.Arrow) return;
            foreach (System.Windows.Shapes.Line l in _lines)
            {
                l.Cursor = Cursors.Arrow;
            }
        }

        internal override void SetStrokeThickness()
        {
            foreach (System.Windows.Shapes.Line l in _lines)
            {
                l.StrokeThickness = StrokeThickness;
            }
        }

        internal override void SetStroke()
        {
            foreach (System.Windows.Shapes.Line l in _lines)
            {
                l.Stroke = Stroke;
            }
        }

        internal override void SetStrokeType()
        {
            foreach (System.Windows.Shapes.Line l in _lines)
            {
                Types.SetShapePattern(l, StrokeType);
            }
        }

        internal override void RemoveLineStudy()
        {
            foreach (System.Windows.Shapes.Line l in _lines)
            {
                C.Children.Remove(l);
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
            get { return _lines[1]; }
        }

        /// <summary>
        /// Segment where context line shall be shown
        /// </summary>
        public Segment Segment
        {
            get { return new Segment(_lines[1].X1, _lines[1].Y1, _lines[1].X2, _lines[1].Y2).Inflate(-10); }
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
