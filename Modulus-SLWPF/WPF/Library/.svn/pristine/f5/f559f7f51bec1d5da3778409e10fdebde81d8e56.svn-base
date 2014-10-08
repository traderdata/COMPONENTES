using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using ModulusFE.PaintObjects;

namespace ModulusFE.PriceStyles
{
    internal partial class HeikinAshi : Style
    {
        internal Brush _downBrush;
        internal Brush _upBrush;
        internal Brush _candleDownOutlineBrush;
        internal Brush _candleUpOutlineBrush;

        private readonly PaintObjectsManager<CandleHeikinAshi> _candles = new PaintObjectsManager<CandleHeikinAshi>();

        public HeikinAshi(ModulusFE.Stock stock)
            : base(stock)
        {
        }

        public override bool Paint()
        {
            //Find Series
            Series open = _series._chartPanel.GetSeriesOHLCV(_series, SeriesTypeOHLC.Open);
            if (open == null || open.RecordCount == 0 || open.Painted) return false;
            Series high = _series._chartPanel.GetSeriesOHLCV(_series, SeriesTypeOHLC.High);
            if (high == null || high.RecordCount == 0 || high.Painted) return false;
            Series low = _series._chartPanel.GetSeriesOHLCV(_series, SeriesTypeOHLC.Low);
            if (low == null || low.RecordCount == 0 || low.Painted) return false;
            Series close = _series._chartPanel.GetSeriesOHLCV(_series, SeriesTypeOHLC.Close);
            if (close == null || close.RecordCount == 0 || close.Painted) return false;

            open.Painted = high.Painted = low.Painted = close.Painted = true;

            StockChartX chartX = _series._chartPanel._chartX;
            chartX._xMap = new double[chartX._xCount = 0];

            const int iStep = 1;
            int rcnt = chartX.RecordCount;
            double x2 = chartX.GetXPixel(rcnt);
            double x1 = chartX.GetXPixel(rcnt - 1);
            double space = ((x2 - x1) / 2) - chartX._barWidth / 2;
            if (space > 20) space = 20;
            if (space > _series._chartPanel._chartX._barSpacing)
                _series._chartPanel._chartX._barSpacing = space;

            space = Math.Ceiling(space * 0.75);

            Color upColor = _series._upColor.HasValue ? _series._upColor.Value : chartX.UpColor;
            Color downColor = _series._downColor.HasValue ? _series._downColor.Value : chartX.DownColor;
            _upBrush = !chartX.ThreeDStyle
                         ? (Brush)new SolidColorBrush(upColor)
                         : new LinearGradientBrush
                             {
                                 StartPoint = new Point(0, 0.5),
                                 EndPoint = new Point(1, 0.5),
                                 GradientStops =
                           {
                             new GradientStop { Color = upColor, Offset = 0 },
                             new GradientStop { Color = Constants.FadeColor, Offset = 1.25 }
                           }
                             };
            _downBrush = !chartX.ThreeDStyle
                           ? (Brush)new SolidColorBrush(downColor)
                           : new LinearGradientBrush
                               {
                                   StartPoint = new Point(0, 0.5),
                                   EndPoint = new Point(1, 0.5),
                                   GradientStops =
                             {
                               new GradientStop { Color = downColor, Offset = 0 },
                               new GradientStop { Color = Constants.FadeColor, Offset = 1.25 }
                             }
                               };
            if (chartX._candleDownOutlineColor.HasValue)
                _candleDownOutlineBrush = new SolidColorBrush(chartX._candleDownOutlineColor.Value);
            if (chartX._candleUpOutlineColor.HasValue)
                _candleUpOutlineBrush = new SolidColorBrush(chartX._candleUpOutlineColor.Value);

            double wick = chartX._barWidth;
            double halfwick = wick / 2;
            if (halfwick < 1)
                halfwick = 1;

            int n;

            //int t = Environment.TickCount;
            _candles.C = _series._chartPanel._rootCanvas;
            _candles.Start();

            Debug.WriteLine("StartIndex " + chartX._startIndex + " EndIndex " + chartX._endIndex);

            for (n = chartX._startIndex; n < chartX._endIndex; n += iStep)
            {
                if (n == 0 && (!open[n].Value.HasValue || !high[n].Value.HasValue || !low[n].Value.HasValue || !close[n].Value.HasValue))
                    continue;
                if (n > 0 && (!open[n].Value.HasValue || !high[n].Value.HasValue || !low[n].Value.HasValue || !close[n].Value.HasValue ||
                  !open[n - 1].Value.HasValue || !close[n - 1].Value.HasValue))
                    continue;

                CandleHeikinAshi candle = _candles.GetPaintObject(_upBrush, _downBrush);
                candle.Init(_series);
                candle.SetBrushes(_upBrush, _downBrush, _candleUpOutlineBrush, _candleDownOutlineBrush);
                candle.SetValues(open[n], high[n], low[n], close[n], space, halfwick, n - chartX._startIndex);
            }
            _candles.Stop();

            _candles.Do(c => c.ZIndex = ZIndexConstants.PriceStyles1);

            Debug.WriteLine("Candles count " + _candles.Count);

            return true;
        }

        public override void RemovePaint()
        {
            _candles.RemoveAll();
        }
    }
}
