using System.Collections.Generic;

namespace ModulusFE.Models
{
    internal partial class SeriesStandardModel : SeriesModelBase
    {
        public bool VolumeUpDown { get; private set; }

        public SeriesStandardModel(int startIndex, int endIndex, bool isOscillator, SeriesTypeEnum seriesType,
          Series series, bool volumeUpDown) :
            base(startIndex, endIndex, isOscillator, seriesType, series)
        {
            VolumeUpDown = volumeUpDown;
        }

        public Series CloseSeries { get; set; }

        public SeriesTypeOHLC SeriesTypeOHLC { get; set; }

        public bool ForceOscilatorPaint { get; set; }

        public enum BrushType
        {
            Down,
            Up,
            Normal
        }

        public class SeriesValueType
        {
            public double X1;
            public double Y1;
            public double X2;
            public double Y2;
            public BrushType Brush;
        }

        public IEnumerable<SeriesValueType> Values
        {
            get
            {
                int cnt = 0;
                double? y2 = null;
                double? y1;
                double x2 = Series._chartPanel._chartX.GetXPixel(0);
                double yZero = Series.GetY(0);
                double minY = Series.GetY(Series.SeriesEntry._min);

                for (int i = StartIndex; i < EndIndex; i++, cnt++)
                {
                    y1 = Series[i].Value;
                    if (!y1.HasValue) continue;
                    y1 = Series.GetY(y1.Value);
                    if (i == StartIndex)
                        y2 = y1;

                    BrushType brushType = BrushType.Normal;
                    if (VolumeUpDown)
                    {
                        if (i > 0)
                        {
                            if (CloseSeries[i].Value > CloseSeries[i - 1].Value)
                                brushType = SeriesTypeOHLC == SeriesTypeOHLC.Volume ? BrushType.Up : BrushType.Down; //up
                            else if (CloseSeries[i].Value < CloseSeries[i - 1].Value)
                                brushType = SeriesTypeOHLC == SeriesTypeOHLC.Volume ? BrushType.Down : BrushType.Up; //down
                            else
                                brushType = BrushType.Normal;
                        }
                        else
                        {
                            brushType = BrushType.Normal;
                        }
                    }
                    else if (Series._chartPanel._chartX._useLineSeriesColors || Series._upColor.HasValue)
                    {
                        if (!IsOscillator)
                        {
                            if (i > 0)
                            {
                                if (Series[i].Value > Series[i - 1].Value)
                                    brushType = BrushType.Up;
                                else
                                    brushType = Series[i].Value < Series[i - 1].Value ? BrushType.Down : BrushType.Normal;
                            }
                            else
                                brushType = BrushType.Normal;
                        }
                        else
                        {
                            if (Series[i].Value > 0)
                                brushType = BrushType.Up;
                            else
                                brushType = Series[i].Value < 0 ? BrushType.Down : BrushType.Normal;
                        }
                    }

                    double x1 = Series._chartPanel._chartX.GetXPixel(cnt);
                    if (SeriesType == SeriesTypeEnum.stVolumeChart || ForceOscilatorPaint)
                    {
                        if (y1.HasValue)
                        {
                            // Make sure at least 2 or 3 pixels show
                            // if the value is the same as the min Y.
                            double nY1 = y1.Value;
                            if (minY - 3 < nY1)
                                nY1 -= 3;
                            if (IsOscillator)
                                yield return new SeriesValueType
                                               {
                                                   Brush = brushType,
                                                   X1 = x1,
                                                   Y1 = nY1,
                                                   X2 = x1,
                                                   Y2 = yZero
                                               };
                            else
                                yield return new SeriesValueType
                                               {
                                                   Brush = brushType,
                                                   X1 = x1,
                                                   Y1 = nY1,
                                                   X2 = x1,
                                                   Y2 = minY,
                                               };
                        }
                    }
                    else if ((SeriesType == SeriesTypeEnum.stLineChart || SeriesType == SeriesTypeEnum.stIndicator) && i > StartIndex)
                    {
                        if (y2.HasValue)
                            yield return new SeriesValueType
                                           {
                                               Brush = brushType,
                                               X1 = x1,
                                               Y1 = y1.Value,
                                               X2 = x2,
                                               Y2 = y2.Value
                                           };
                    }

                    y2 = y1;
                    x2 = x1;
                }
            }
        }
    }
}
