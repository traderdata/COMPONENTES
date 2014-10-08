using System;
using System.Windows.Controls;
using System.Windows.Media;
using ModulusFE.Data;
using ModulusFE.PaintObjects;

namespace ModulusFE.PriceStyles
{
    internal partial class PointAndFigure : Style
    {
        private readonly PaintObjectsManager<Line> _lines = new PaintObjectsManager<Line>();
        private readonly PaintObjectsManager<Ellipse> _ellipses = new PaintObjectsManager<Ellipse>();

        public PointAndFigure(Series stock)
            : base(stock)
        {
        }

        public override bool Paint()
        {
            if (_series.OHLCType != SeriesTypeOHLC.High && _series.OHLCType != SeriesTypeOHLC.Low) return false;
            StockChartX chartX = _series._chartPanel._chartX;
            Series high = chartX.GetSeriesOHLCV(_series, SeriesTypeOHLC.High);
            if (high == null || high.Painted || high.RecordCount == 0) return false;
            Series low = chartX.GetSeriesOHLCV(_series, SeriesTypeOHLC.Low);
            if (low == null || low.Painted || low.RecordCount == 0) return false;

            high.Painted = low.Painted = true;

            Brush upBrush = new SolidColorBrush(_series._upColor.HasValue ? _series.UpColor.Value : chartX.UpColor);
            Brush downBrush = new SolidColorBrush(_series._downColor.HasValue ? _series._downColor.Value : chartX.DownColor);

            double width = chartX.PaintableWidth;

            double max = chartX.GetMax(high, true);
            double min = chartX.GetMin(low, true);

            double boxSize = chartX._priceStyleParams[0];
            if (boxSize > 50 || boxSize < 0.00000000000000000000001)
                boxSize = (max - min) / 25;
            double reversalSize = chartX._priceStyleParams[1];
            if (reversalSize > 50 || reversalSize < 1)
                reversalSize = 3;

            chartX._priceStyleParams[0] = boxSize;
            chartX._priceStyleParams[1] = reversalSize;

            double nHigh, nLow, nLastHigh = 0, nLastLow = 0;
            int column = 0; // X=1 O=2
            int columnHeight = 0;
            int totalColumns = 0;
            int boxes;
            const int Xs = 1;
            const int Os = 2;

            chartX._psValues1.Clear();
            chartX._psValues2.Clear();
            chartX._psValues3.Clear();

            chartX._xMap = new double[chartX._endIndex - chartX._startIndex + 1];
            int cnt = 0;
            // Count columns
            int n;
            for (n = chartX._startIndex; n < chartX._endIndex; n++)
            {
                if (!high[n].Value.HasValue || !low[n].Value.HasValue) continue;
                // Calculate Point and Figure
                nHigh = high[n].Value.Value;
                nLow = low[n].Value.Value;

                switch (column)
                {
                    case Xs:
                        boxes = (int)((nHigh - nLastHigh) / boxSize);
                        if (boxes >= boxSize)
                        {
                            // Add one X box
                            columnHeight += 1;
                            nLastHigh += boxSize;
                            if (nLastHigh > max) max = nLastHigh;
                        }
                        else
                        {
                            // Check for O's reversal
                            boxes = (int)((nLastHigh - nLow) / boxSize);
                            if (boxes >= reversalSize)
                            {
                                column = Os;
                                columnHeight = boxes;
                                totalColumns++;
                                nLastLow = nLastHigh - (boxes * boxSize);
                                if (nLastLow < min && min != 0) min = nLastLow;
                            }
                        }
                        break;
                    case Os:
                        boxes = (int)((nLastLow - nLow) / boxSize);
                        if (boxes >= boxSize)
                        {
                            // Add one O box
                            columnHeight += 1;
                            nLastLow -= boxSize;
                            if (nLastLow < min && min != 0) min = nLastLow;
                        }
                        else
                        {
                            // Check for X's reversal
                            boxes = (int)((nHigh - nLastLow) / boxSize);
                            if (boxes >= reversalSize)
                            {
                                column = Xs;
                                columnHeight = boxes;
                                totalColumns++;
                                nLastHigh = nLastLow + (boxes * boxSize);
                                if (nLastHigh > max) max = nLastHigh;
                            }
                        }
                        break;
                }

                if (column != 0) continue; // Prime first column        
                column = Xs;
                boxes = (int)Math.Floor(((nHigh - (nLow + (boxSize * reversalSize))) / boxSize) + 0.5);
                columnHeight = boxes;
                nLastHigh = nHigh;
                nLastLow = nHigh - (boxes * boxSize);
                totalColumns = 1;
            }

            chartX._xCount = totalColumns;

            column = 0;
            double x = chartX.LeftChartSpace;
            if (totalColumns == 0) return false;
            double space = width / totalColumns;
            totalColumns = 0;

            _lines.C = _ellipses.C = _series._chartPanel._rootCanvas;
            _lines.Start();
            _ellipses.Start();

            // Calculate from beginning, but only show between startIndex and endIndex
            for (n = 0; n < chartX._endIndex; n++)
            {
                if (high[n].Value.HasValue && low[n].Value.HasValue)
                {
                    // Calculate Point and Figure
                    nHigh = high[n].Value.Value;
                    nLow = low[n].Value.Value;

                    double y1;
                    double y2;
                    switch (column)
                    {
                        case Xs:
                            boxes = (int)((nHigh - nLastHigh) / boxSize);
                            if (boxes >= boxSize)
                            {
                                // Add one X box
                                columnHeight += 1;
                                nLastHigh += boxSize;
                            }
                            else
                            {
                                // Check for O's reversal
                                boxes = (int)((nLastHigh - nLow) / boxSize);
                                if (boxes >= reversalSize)
                                {

                                    // Paint the previous X column
                                    if (n >= chartX._startIndex && n <= chartX._endIndex)
                                    {
                                        double y = nLastHigh;
                                        if (columnHeight > 0)
                                        {
                                            for (int col = 0; col < columnHeight; ++col)
                                            {
                                                y1 = _series._chartPanel.GetY(y);
                                                y -= boxSize;
                                                y2 = _series._chartPanel.GetY(y);
                                                double x1 = x;
                                                double x2 = x + space;
                                                Utils.DrawLine(x1, y1, x2, y2, upBrush, _series._strokePattern, _series._strokeThickness, _series._opacity, _lines);
                                                Utils.DrawLine(x2, y1, x1, y2, upBrush, _series._strokePattern, _series._strokeThickness, _series._opacity, _lines);
                                            }
                                        }
                                    }

                                    // Create new O column
                                    column = Os;
                                    columnHeight = boxes;

                                    if (n >= chartX._startIndex && n <= chartX._endIndex)
                                    {
                                        totalColumns++;
                                        x += space;
                                    }

                                    nLastLow = nLastHigh - (boxes * boxSize);
                                }
                            }
                            break;
                        case Os:
                            boxes = (int)((nLastLow - nLow) / boxSize);
                            if (boxes >= boxSize)
                            {
                                // Add one O box
                                columnHeight += 1;
                                nLastLow -= boxSize;
                            }
                            else
                            {
                                // Check for X's reversal
                                boxes = (int)((nHigh - nLastLow) / boxSize);
                                if (boxes >= reversalSize)
                                {

                                    // Paint the previous O's column
                                    if (n >= chartX._startIndex && n <= chartX._endIndex)
                                    {
                                        double y = nLastLow - boxSize;
                                        if (columnHeight > 0)
                                        {
                                            for (int col = 0; col < columnHeight; ++col)
                                            {
                                                y2 = _series._chartPanel.GetY(y);
                                                y += boxSize;
                                                y1 = _series._chartPanel.GetY(y);
                                                System.Windows.Shapes.Ellipse ellipse = _ellipses.GetPaintObject()._ellipse;
                                                Types.RectEx bounds = new Types.RectEx(x, y1, x + space, y2);
                                                Canvas.SetLeft(ellipse, bounds.Left);
                                                Canvas.SetTop(ellipse, bounds.Top);
                                                ellipse.Width = bounds.Width;
                                                ellipse.Height = bounds.Height;
                                                ellipse.Stroke = downBrush;
                                                ellipse.StrokeThickness = _series._strokeThickness;
                                                ellipse.Fill = _series._chartPanel.Background;
                                            }
                                        }
                                    }

                                    // Create new X column
                                    column = Xs;
                                    columnHeight = boxes;

                                    if (n >= chartX._startIndex && n <= chartX._endIndex)
                                    {
                                        totalColumns++;
                                        x += space;
                                    }

                                    nLastHigh = nLastLow + (boxes * boxSize);
                                }
                            }
                            break;
                    }

                    if (column == 0)
                    { // Prime first column        
                        column = Xs;
                        boxes = (int)Math.Floor(((nHigh - (nLow + (boxSize * reversalSize))) / boxSize) + 0.5);
                        columnHeight = boxes;
                        nLastHigh = nHigh;
                        nLastLow = nHigh - (boxes * boxSize);

                        if (n >= chartX._startIndex && n <= chartX._endIndex)
                            totalColumns = 1;

                        x = chartX.LeftChartSpace;
                    }

                    // Record the x value
                    if (n >= chartX._startIndex && n <= chartX._endIndex)
                    {
                        chartX._xMap[cnt] = x + (space / 2);
                        cnt++;
                    }
                }

                chartX._psValues3.Add(new PriceStyleValue(high[n].TimeStamp, column == 1 ? 1 : -1));
                chartX._psValues1.Add(new PriceStyleValue(high[n].TimeStamp, columnHeight));

                // Once the direction changes, we need to
                // go backwards until the previous change
                // and fill in the values.
                if (chartX._psValues3.Count <= 1) continue;
                for (int prev = chartX._psValues3.Count - 1; prev >= 0; --prev)
                {
                    if (chartX._psValues3[prev].Value != column) break;
                    chartX._psValues1[prev].Value = columnHeight;
                }
            }

            _lines.Stop();
            _ellipses.Stop();

            _lines.Do(l => l.ZIndex = ZIndexConstants.PriceStyles1);
            _ellipses.Do(e => e.ZIndex = ZIndexConstants.PriceStyles1);

            return true;
        }

        public override void RemovePaint()
        {
            _lines.RemoveAll();
            _ellipses.RemoveAll();
        }
    }
}
