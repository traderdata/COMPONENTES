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
        internal static void Register_ErrorChannel()
        {
            RegisterLineStudy(LineStudy.StudyTypeEnum.ErrorChannel, typeof(ErrorChannel), "Error Channel");
        }
    }
}


namespace ModulusFE.LineStudies
{
    /// <summary>
    /// Error channel line study
    /// </summary>
    public partial class ErrorChannel : LineStudy, IContextAbleLineStudy
    {
        private readonly Types.Location[] _errorLines = new Types.Location[3];
        private readonly PaintObjectsManager<Line> _lines = new PaintObjectsManager<Line>();
        private readonly List<System.Windows.Shapes.Line> _linesError = new List<System.Windows.Shapes.Line>(3);
        private ContextLine _contextLine;
        private double? _rangeScale;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="key">Unique key for line study</param>
        /// <param name="stroke">Stroke brush</param>
        /// <param name="chartPanel">Reference to chart panel where it will be placed.</param>
        public ErrorChannel(string key, Brush stroke, ChartPanel chartPanel)
            : base(key, stroke, chartPanel)
        {
            _studyType = StudyTypeEnum.ErrorChannel;
        }

        internal override void SetArgs(params object[] args)
        {
            if (args == null) return;

            if (args.Length > 0)
                _rangeScale = Convert.ToDouble(args[0]);
        }

        internal override void DrawLineStudy(Types.RectEx rect, LineStatus lineStatus)
        {
            if (lineStatus == LineStatus.StartPaint) return;

            rect.Normalize();

            //if (rect.Width == 0 || rect.Left < 0)
            //{
            //    return;
            //}

            int revX1 = (int)(_chartX.GetReverseXInternal(rect.Left) + _chartX._startIndex);
            int revX2 = (int)(_chartX.GetReverseXInternal(rect.Right) + _chartX._startIndex);
            if (revX1 < 0) revX1 = 0;
            if (revX2 < 0) revX2 = 0;

            if (revX1 == revX2) return;

            // Note: this code makes the vague assumption
            // that only one symbol exists on this panel.
            // Get the close series
            Series sClose = GetSeriesOHLC(SeriesTypeOHLC.Close);
            if (sClose == null) return;
            //Debug.Assert(sClose.RecordCount == _chartX.RecordCount);
            if (revX1 >= _chartX.RecordCount) revX1 = _chartX.RecordCount - 1;
            if (revX2 >= _chartX.RecordCount) revX2 = _chartX.RecordCount - 1;

            // Get the highest high of the high series.
            double highestHigh = 0;
            Series sHigh = GetSeriesOHLC(SeriesTypeOHLC.High);
            if (sHigh == null) return;
            //Debug.Assert(revX1 <= revX2);
            for (int i = revX1; i <= revX2; i++)
            {
                if (sHigh[i].Value > highestHigh)
                    highestHigh = sHigh[i].Value.Value;
            }

            //Get the lowest low of the low series.
            double lowestLow = highestHigh;
            Series sLow = GetSeriesOHLC(SeriesTypeOHLC.Low);
            if (sLow == null) return;
            for (int i = revX1; i <= revX2; i++)
            {
                if (sLow[i].Value < lowestLow)
                    lowestLow = sLow[i].Value.Value;
            }

            double range = (highestHigh - lowestLow) * 0.5;

            if (_rangeScale.HasValue)
                range = (highestHigh - lowestLow) * _rangeScale.Value;

            // Perform linear regression on the data
            double xSum = 0, ySum = 0, xSquaredSum = 0, xYSum = 0;
            int x = revX2 - revX1;
            int j, n;
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
            double inc = (x - 1) != 0 ? (rightValue - leftValue) / (x - 1) : 0;

            j = 0;
            double prevVal = 0;
            double lX1 = rect.Left;//_chartX.GetX(revX1 - _chartX._startIndex);
            double lX2 = rect.Right;// _chartX.GetX((revX2 - 1) - _chartX._startIndex + 1);

            if (_linesError.Count == 0)
            {
                for (int i = 0; i < 3; i++)
                {
                    System.Windows.Shapes.Line line = new System.Windows.Shapes.Line { Tag = this };
                    Canvas.SetZIndex(line, ZIndexConstants.LineStudies1);
                    C.Children.Add(line);

                    _linesError.Add(line);
                }

                _contextLine = new ContextLine(this);

                _chartX.InvokeLineStudyCreated(new StockChartX.LineStudyCreatedEventArgs(this));
            }

            _internalObjectCreated = true;

            _lines.C = C;
            _lines.Start();
            for (n = revX1; n <= revX2; n++, j++)
            {
                double val = leftValue + inc * (j - 1);
                //double lX1 = _chartX.GetX(n - _chartX._startIndex);
                //double lX2 = _chartX.GetX(n - _chartX._startIndex + 1);
                double lY1 = _chartPanel.GetY(prevVal + range);
                double lY2 = _chartPanel.GetY(val + range);
                if (prevVal != 0.0)
                {
                    _errorLines[2].Y1 = lY1;
                    _errorLines[2].Y2 = lY2;
                    Utils.DrawLine(lX1, lY1, lX2, lY2, Stroke, StrokeType, StrokeThickness, Opacity, _linesError[0]);
                }

                lY1 = _chartPanel.GetY(prevVal - range);
                lY2 = _chartPanel.GetY(val - range);
                if (prevVal != 0.0)
                {
                    _errorLines[0].Y1 = lY1;
                    _errorLines[0].Y2 = lY2;
                    Utils.DrawLine(lX1, lY1, lX2, lY2, Stroke, StrokeType, StrokeThickness, Opacity, _linesError[1]);
                }

                lY1 = _chartPanel.GetY(prevVal);
                lY2 = _chartPanel.GetY(val);
                if (prevVal != 0.0)
                {
                    _errorLines[1].Y1 = lY1;
                    _errorLines[1].Y2 = lY2;
                    Utils.DrawLine(lX1, lY1, lX2, lY2, Stroke, StrokeType, StrokeThickness, Opacity, _linesError[2]);
                }

                prevVal = val;

            }
            if (lineStatus == LineStatus.Moving || lineStatus == LineStatus.Painting)
            {
                Utils.DrawLine(rect.Left, 0, rect.Left, _chartPanel.Height, Stroke, StrokeType, StrokeThickness, Opacity, _lines);
                Utils.DrawLine(rect.Right, 0, rect.Right, _chartPanel.Height, Stroke, StrokeType, StrokeThickness, Opacity, _lines);
            }

            _lines.Stop();

            _lines.Do(l => l.ZIndex = ZIndexConstants.LineStudies1);

            _errorLines[0].X1 = rect.Left;
            _errorLines[0].X2 = rect.Right;
            _errorLines[1].X1 = rect.Left;
            _errorLines[1].X2 = rect.Right;
            _errorLines[2].X1 = rect.Left;
            _errorLines[2].X2 = rect.Right;

            if (_contextLine == null && _linesError.Count > 0)
                _contextLine = new ContextLine(this);
        }

        internal override void SetCursor()
        {
            if (_linesError.Count == 0) return;
            System.Windows.Shapes.Line line = _linesError[0];

            if (_selectionVisible && line.Cursor != Cursors.Hand)
            {
                foreach (System.Windows.Shapes.Line l in _linesError)
                {
                    l.Cursor = Cursors.Hand;
                }
                return;
            }
            if (_selectionVisible || line.Cursor == Cursors.Arrow) return;
            foreach (System.Windows.Shapes.Line l in _linesError)
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

            if (_errorLines.Length > 0)
            {
                if (_errorLines[0].Y1 < _errorLines[2].Y1)
                {
                    _y1 = _errorLines[0].Y1;
                    _y2 = _errorLines[2].Y2;
                }
                else
                {
                    _y1 = _errorLines[2].Y1;
                    _y2 = _errorLines[1].Y2;
                }
            }
        }

        internal override List<SelectionDotInfo> GetSelectionPoints()
        {
            return new List<SelectionDotInfo>
               {
                 new SelectionDotInfo {Corner = Types.Corner.MiddleLeft, Position = _errorLines[1].P1},
                 new SelectionDotInfo {Corner = Types.Corner.MiddleRight, Position = _errorLines[1].P2},
               };
        }

        internal override void SetStrokeThickness()
        {
            foreach (System.Windows.Shapes.Line l in _linesError)
            {
                l.StrokeThickness = StrokeThickness;
            }
        }

        internal override void SetStroke()
        {
            foreach (System.Windows.Shapes.Line line in _linesError)
            {
                line.Stroke = Stroke;
            }
        }

        internal override void SetStrokeType()
        {
            foreach (System.Windows.Shapes.Line line in _linesError)
            {
                Types.SetShapePattern(line, StrokeType);
            }
        }

        internal override void SetOpacity()
        {
            foreach (var line in _linesError)
            {
                line.Opacity = Opacity;
            }
        }

        internal override void RemoveLineStudy()
        {
            _lines.RemoveAll();
            foreach (var line in _linesError)
            {
                C.Children.Remove(line);
            }
        }

        #region Implementation of IContextAbleLineStudy

        /// <summary>
        /// Element to which context line is bound
        /// </summary>
        public UIElement Element
        {
            get { return _linesError[2]; }
        }

        /// <summary>
        /// Segment where context line shall be shown
        /// </summary>
        public Segment Segment
        {
            get
            {
                return new Segment(_errorLines[1].X1, _errorLines[1].Y1,
                                   _errorLines[1].X2, _errorLines[1].Y2).Inflate(-20);
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
