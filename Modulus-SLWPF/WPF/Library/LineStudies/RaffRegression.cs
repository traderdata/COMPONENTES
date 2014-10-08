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
        internal static void Register_RaffRegression()
        {
            RegisterLineStudy(LineStudy.StudyTypeEnum.RaffRegression, typeof(RaffRegression), "Raff Regression");
        }
    }
}


namespace ModulusFE.LineStudies
{
    /// <summary>
    /// Raff Regression line study
    /// </summary>
    public partial class RaffRegression : LineStudy, IContextAbleLineStudy
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
        public RaffRegression(string key, Brush stroke, ChartPanel chartPanel)
            : base(key, stroke, chartPanel)
        {
            _studyType = StudyTypeEnum.RaffRegression;
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
            Series sClose = GetSeriesOHLC(SeriesTypeOHLC.Close);

            if (revX1 >= sHigh.RecordCount)
                revX1 = sHigh.RecordCount;
            if (revX2 >= sHigh.RecordCount)
                revX2 = sHigh.RecordCount;

            //**********
            // Perform linear regression on the data
            double xSum = 0, ySum = 0, xSquaredSum = 0, xYSum = 0;
            int n, j;

            if (revX1 > revX2)
                revX2 = revX1;
            int x = revX2 - revX1;
            for (n = 1; n != x + 1; ++n)
            {
                j = revX1 + n - 1;
                xSum += n;
                ySum += sClose[j].Value.Value;
                xSquaredSum += (n * n);
                xYSum += (sClose[j].Value.Value * n);
            }
            n = x;
            double q1 = n != 0 ? (xYSum - ((xSum * ySum) / n)) : 0;
            double q2 = n != 0 ? (xSquaredSum - ((xSum * xSum) / n)) : 0;
            double slope = q2 != 0 ? (q1 / q2) : 0;
            double leftValue = slope != 0 ? (((1 / (double)n) * ySum) - (((int)((double)n / 2)) * slope)) : 0.0;
            double rightValue = ((n * slope) + leftValue);
            double right = (x - 1);
            double inc = 0;
            if (right != 0)
                inc = (rightValue - leftValue) / right;

            double prevVal = 0.0;
            j = 0;
            // Find max distance from linear regression line
            double lowestLow = sHigh[0].Value.Value;
            double highestHigh = 0;
            for (n = revX1; n < revX2; ++n)
            {
                j++;
                double val = leftValue + inc * (j - 1);
                if (prevVal != 0)
                {
                    if (sHigh[n].Value.Value - val > highestHigh)
                    {
                        highestHigh = sHigh[n].Value.Value - val;
                    }
                    if (val - sLow[n].Value.Value < lowestLow &&
                     val - sLow[n].Value.Value > 0)
                    {
                        lowestLow = val - sLow[n].Value.Value;
                    }
                }
                prevVal = val;
            }
            if (highestHigh > lowestLow) lowestLow = highestHigh;
            if (lowestLow > highestHigh) highestHigh = lowestLow;

            _linesSel.C = C;

            double lX1 = _chartX.GetXPixel(revX1 - _chartX._startIndex);
            double lX2 = _chartX.GetXPixel(revX2 - _chartX._startIndex + 1);

            j = 0;
            for (n = revX1; n < revX2; n++, j++)
            {
                double val = leftValue + inc * (j - 1);
                double lY1 = _chartPanel.GetY(prevVal - lowestLow);
                double lY2 = _chartPanel.GetY(val - lowestLow);
                if (prevVal != 0)
                    Utils.DrawLine(lX1, lY1, lX2, lY2, Stroke, StrokeType, StrokeThickness, Opacity, _lines[0]);

                lY1 = _chartPanel.GetY(prevVal);
                lY2 = _chartPanel.GetY(val);
                if (prevVal != 0)
                    Utils.DrawLine(lX1, lY1, lX2, lY2, Stroke, StrokeType, StrokeThickness, Opacity, _lines[1]);

                lY1 = _chartPanel.GetY(prevVal + highestHigh);
                lY2 = _chartPanel.GetY(val + highestHigh);
                if (prevVal != 0)
                    Utils.DrawLine(lX1, lY1, lX2, lY2, Stroke, StrokeType, StrokeThickness, Opacity, _lines[2]);

                prevVal = val;
            }

            _linesSel.Start();
            if (lineStatus == LineStatus.Moving || lineStatus == LineStatus.Painting)
            {
                Utils.DrawLine(lX1, 0, lX1, C.ActualHeight, Stroke, LinePattern.Dot, StrokeThickness, Opacity, _linesSel);
                Utils.DrawLine(lX2, 0, lX2, C.ActualHeight, Stroke, LinePattern.Dot, StrokeThickness, Opacity, _linesSel);
            }
            _linesSel.Stop();
            _linesSel.Do(l => l.ZIndex = ZIndexConstants.LineStudies1);
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
                if (_lines[0].Y1 < _lines[2].Y1)
                {
                    _y1 = _lines[0].Y1;
                    _y2 = _lines[2].Y2;
                }
                else
                {
                    _y1 = _lines[2].Y1;
                    _y2 = _lines[1].Y2;
                }
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
