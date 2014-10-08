using System.Windows.Media;
using ModulusFE.PaintObjects;

namespace ModulusFE.PriceStyles
{
    internal partial class EquiVolume : Style
    {
        private readonly PaintObjectsManager<Line> _lines = new PaintObjectsManager<Line>();
        private readonly PaintObjectsManager<Rectangle> _rects = new PaintObjectsManager<Rectangle>();
        private readonly PaintObjectsManager<Rectangle3D> _rects3D = new PaintObjectsManager<Rectangle3D>();

        public EquiVolume(ModulusFE.Stock stock)
            : base(stock)
        {
        }

        public override bool Paint()
        {
            /*
            highestVolume = highest volume up to current record (not after)

            x = volume / highestVolume
            if x = 0 then x = 1

            1,0.25,0.5,0.75,0.25,0.5,1
            total=4.25

            /4.25 total
            0.235,0.058,0.117,0.176,0.058,0.117,0.235=1

            300* pixels
            70.5,17.4,35.1,52.8,17.4,35.1,70.5=300

            */
            if (_series.OHLCType == SeriesTypeOHLC.Volume) return false;
            StockChartX chartX = _series._chartPanel._chartX;

            Series open = chartX.GetSeriesOHLCV(_series, SeriesTypeOHLC.Open);
            if (open == null || open.Painted || open.RecordCount == 0) return false;
            Series high = chartX.GetSeriesOHLCV(_series, SeriesTypeOHLC.High);
            if (high == null || high.Painted || high.RecordCount == 0) return false;
            Series low = chartX.GetSeriesOHLCV(_series, SeriesTypeOHLC.Low);
            if (low == null || low.Painted || low.RecordCount == 0) return false;
            Series close = chartX.GetSeriesOHLCV(_series, SeriesTypeOHLC.Close);
            if (close == null || close.Painted || close.RecordCount == 0) return false;
            Series volume = chartX.GetSeriesOHLCV(_series, SeriesTypeOHLC.Volume);
            if (volume == null || volume.RecordCount == 0) return false;

            open.Painted = high.Painted = low.Painted = close.Painted = true;

            double width = chartX.PaintableWidth;

            const int up = 1;
            const int down = 2;
            double highestVolume;
            double x;
            double equiVol;
            double total = 0;
            Types.RectEx box;

            Brush customBrush = new SolidColorBrush(_series._strokeColor);
            Brush upBrush = new SolidColorBrush(_series._upColor.HasValue ? _series._upColor.Value : chartX.UpColor);
            Brush downBrush = new SolidColorBrush(_series._downColor.HasValue ? _series._downColor.Value : chartX.DownColor);
            Brush upGradBrush = Utils.CreateFadeVertBrush(_series._upColor.HasValue ? _series._upColor.Value : chartX.UpColor,
                                                          Colors.Black);
            Brush downGradBrush = Utils.CreateFadeVertBrush(Colors.Black,
                                                            _series._downColor.HasValue
                                                              ? _series._downColor.Value
                                                              : chartX.DownColor);

            chartX._xMap = new double[chartX._endIndex - chartX._startIndex + 1];
            int cnt = 0;

            _lines.C = _rects.C = _rects3D.C = _series._chartPanel._rootCanvas;
            _lines.Start();
            _rects.Start();
            _rects3D.Start();

            // Count total Equi-Volume
            int n;
            for (n = chartX._startIndex; n < chartX._endIndex; n++)
            {
                if (!open[n].Value.HasValue || !high[n].Value.HasValue || !low[n].Value.HasValue || !close[n].Value.HasValue ||
                    !volume[n].Value.HasValue)
                {
                    continue;
                }

                // Calculate Equi-Volume
                highestVolume = HighestVolumeToRecord(n, volume);
                x = highestVolume != 0.0 ? volume[n].Value.Value / highestVolume : 0;
                total += x;

                if (!chartX._darwasBoxes)
                {
                    continue;
                }

                equiVol = volume[n].Value.Value / highestVolume;
                equiVol = equiVol / total;
                equiVol = width * equiVol;
                x += equiVol;
                // Record the x value
                chartX._xMap[cnt] = x;
                cnt++;
            }

            cnt = 0;
            x = chartX.LeftChartSpace;
            double px = x;
            for (n = chartX._startIndex; n < chartX._endIndex; n++)
            {
                if (!open[n].Value.HasValue || !high[n].Value.HasValue || !low[n].Value.HasValue || !close[n].Value.HasValue ||
                      !volume[n].Value.HasValue)
                {
                    continue;
                }

                // Calculate Equi-Volume
                highestVolume = HighestVolumeToRecord(n, volume);
                equiVol = highestVolume != 0 ? volume[n].Value.Value / highestVolume : 0.0;
                equiVol = equiVol / total;
                equiVol = width * equiVol;
                x += equiVol;

                // Record the x value
                chartX._xMap[cnt] = x;
                cnt++;

                double x1 = px;
                double x2 = x;
                double y1 = _series._chartPanel.GetY(high[n].Value.Value);
                double y2 = _series._chartPanel.GetY(low[n].Value.Value);
                if (y2 == y1)
                    y1 = y2 - 2;

                box = new Types.RectEx(x1, y1, x2, y2);
                Types.RectEx frame = box;

                if (chartX._priceStyle == PriceStyleEnum.psEquiVolumeShadow)
                {
                    if (close[n].Value > open[n].Value)
                        box.Top = _series._chartPanel.GetY(close[n].Value.Value);
                    else
                        box.Bottom = _series._chartPanel.GetY(close[n].Value.Value);
                }

                double wx = x1 + (x2 - x1) / 2;
                if (chartX._priceStyle == PriceStyleEnum.psCandleVolume)
                {
                    if (close[n].Value > open[n].Value)
                    {
                        box.Top = _series._chartPanel.GetY(close[n].Value.Value);
                        box.Bottom = _series._chartPanel.GetY(open[n].Value.Value);
                    }
                    else
                    {
                        box.Top = _series._chartPanel.GetY(open[n].Value.Value);
                        box.Bottom = _series._chartPanel.GetY(close[n].Value.Value);
                    }
                    if (box.Bottom == box.Top)
                        box.Top = box.Bottom - 2;

                    Utils.DrawLine(wx, y1, wx, y2, customBrush, _series.StrokePattern, _series._strokeThickness, _series._opacity, _lines);

                    frame = box;
                }

                // Is the bar up or down?
                int direction;
                if (n == 0)
                    direction = close[n].Value > open[n].Value ? up : down;
                else
                    direction = close[n].Value > close[n - 1].Value ? up : down;

                if (chartX.ThreeDStyle)
                {
                    if (direction == up)
                    {
                        Utils.DrawRectangle(box, upGradBrush, _rects);
                        if (chartX._priceStyle == PriceStyleEnum.psEquiVolumeShadow)
                            Utils.Draw3DRectangle(frame, upBrush, downBrush, _rects3D);
                    }
                    else
                    {
                        Utils.DrawRectangle(box, downGradBrush, _rects);
                        if (chartX._priceStyle == PriceStyleEnum.psEquiVolumeShadow)
                            Utils.Draw3DRectangle(frame, upBrush, downBrush, _rects3D);
                    }
                }
                else
                {
                    if (direction == up)
                    {
                        Utils.DrawRectangle(box, upBrush, _rects);
                        if (chartX._priceStyle == PriceStyleEnum.psEquiVolumeShadow)
                            Utils.Draw3DRectangle(frame, upBrush, upBrush, _rects3D);
                    }
                    else
                    {
                        Utils.DrawRectangle(box, downBrush, _rects);
                        if (chartX._priceStyle == PriceStyleEnum.psEquiVolumeShadow)
                            Utils.Draw3DRectangle(frame, downBrush, downBrush, _rects3D);
                    }
                }

                px = x;
            }

            _lines.Stop();
            _rects.Stop();
            _rects3D.Stop();

            _lines.Do(l => l.ZIndex = ZIndexConstants.PriceStyles1);
            _rects.Do(r => r.ZIndex = ZIndexConstants.PriceStyles2);
            _rects3D.Do(r => r.ZIndex = ZIndexConstants.PriceStyles3);

            return true;
        }

        private static int HighestVolumeToRecord(int record, Series volume)
        {
            int startIndex = 0;
            record++;
            return (int)volume.MaxFromInterval(ref startIndex, ref record);
        }

        public override void RemovePaint()
        {
            _lines.RemoveAll();
            _rects.RemoveAll();
            _rects3D.RemoveAll();
        }
    }
}
