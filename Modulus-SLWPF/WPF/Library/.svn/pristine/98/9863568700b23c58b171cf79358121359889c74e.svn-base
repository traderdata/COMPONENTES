using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;

namespace ModulusFE.PriceStyles.Models
{
    ///<summary>
    ///</summary>
    internal partial class PriceStyleStandardModel : PriceStyleModeBase
    {
        public PriceStyleStandardModel(int startIndex, int endIndex, Series[] series)
            : base(startIndex, endIndex, series)
        {
        }

        public double Space { get; set; }
        public double HalfWick { get; set; }

        public class WickValueType
        {
            public double X;
            public double Y1;
            public double Y2;
        }

        public enum CandleType
        {
            Up,
            Down,
            NoChange
        }

        public class CandleValueType
        {
            public double X1, X2;
            public double Y1, Y2;
            public CandleType Type;

            public Rect Rect
            {
                get
                {
                    return new Rect(Math.Min(X1, X2), Math.Min(Y1, Y2),
                                    Math.Abs(X1 - X2), Math.Abs(Y1 - Y2));
                }
            }
        }

        public class Values
        {
            public WickValueType WickValue;
            public CandleValueType CandleValue;
        }

        public IEnumerable<Values> WickValues
        {
            get
            {
                ChartPanel panel = Series[0]._chartPanel;
                StockChartX chart = panel._chartX;
                Series openSeries = Series[0]; //open
                Series highSeries = Series[1]; //high
                Series lowSeries = Series[2]; //low
                Series closeSeries = Series[3]; //close

                int cnt = 0;
                for (int i = StartIndex; i < EndIndex; i++)
                {
                    double? openValue = openSeries[i].Value;
                    if (!openValue.HasValue) continue;
                    double? highValue = highSeries[i].Value;
                    if (!highValue.HasValue) continue;
                    double? lowValue = lowSeries[i].Value;
                    if (!lowValue.HasValue) continue;
                    double? closeValue = closeSeries[i].Value;
                    if (!closeValue.HasValue) continue;

                    highValue = highSeries.GetY(highValue.Value);
                    lowValue = lowSeries.GetY(lowValue.Value);
                    if (highValue == lowValue)
                        highValue = lowValue - 2;

                    CandleType candleType = openValue > closeValue ? CandleType.Down :
                      (openValue < closeValue) ? CandleType.Up : CandleType.NoChange;

                    double Y1 = 0;
                    double Y2 = 0;
                    switch (candleType)
                    {
                        case CandleType.Down:
                            Y1 = openSeries.GetY(openValue.Value);
                            Y2 = closeSeries.GetY(closeValue.Value);
                            break;
                        case CandleType.Up:
                        case CandleType.NoChange:
                            Y1 = openSeries.GetY(closeValue.Value);
                            Y2 = closeSeries.GetY(openValue.Value);
                            break;
                    }
                    if (Math.Abs(Y2 - Y1) < 1)
                        Y2 = Y1 + 1;

                    double x = chart.GetXPixel(cnt++);

                    yield return new Values
                                   {
                                       WickValue = (openValue.Value == highValue.Value) &&
                                                    (highValue.Value == lowValue.Value) &&
                                                    (lowValue.Value == closeValue.Value)
                                                     ? null
                                                     : new WickValueType
                                                         {
                                                             X = x,
                                                             Y1 = highValue.Value,
                                                             Y2 = lowValue.Value
                                                         },
                                       CandleValue = new CandleValueType
                                                       {
                                                           Type = candleType,
                                                           X1 = x - Space - HalfWick,
                                                           Y1 = Y1,
                                                           X2 = x + Space + HalfWick,
                                                           Y2 = Y2
                                                       }
                                   };
                }
            }
        }

        public IEnumerable<Values> WickValuesG
        {
            get
            {
                ChartPanel panel = Series[0]._chartPanel;
                StockChartX chart = panel._chartX;

                List<double?>[] v = chart._dataManager.GetGroupedOhlcValues(new[]
                                                                      {
                                                                        Series[0].SeriesIndex,
                                                                        Series[1].SeriesIndex,
                                                                        Series[2].SeriesIndex,
                                                                        Series[3].SeriesIndex,
                                                                      });
                //List<double?> openSeries = v[0];
                //List<double?> highSeries = v[1];
                //List<double?> lowSeries = v[2];
                //List<double?> closeSeries = v[3];

                //        if (openSeries == null || highSeries == null 
                //          || lowSeries == null || closeSeries == null) yield break;

                Series oSeries = Series[0];
                Series hSeries = Series[1];
                Series lSeries = Series[2];
                Series cSeries = Series[3];


                int cnt = 0;
                //int totalLength = openSeries.Count;
                for (int i = 0; i < chart.RecordCount; i++)
                {
                    double? openValue = oSeries[i].Value;
                    if (!openValue.HasValue) continue;
                    double? highValue = hSeries[i].Value;
                    if (!highValue.HasValue) continue;
                    double? lowValue = lSeries[i].Value;
                    if (!lowValue.HasValue) continue;
                    double? closeValue = cSeries[i].Value;
                    if (!closeValue.HasValue) continue;

                    highValue = hSeries.GetY(highValue.Value);
                    lowValue = lSeries.GetY(lowValue.Value);
                    if (highValue == lowValue)
                        highValue = lowValue - 2;

                    CandleType candleType = openValue > closeValue
                                              ? CandleType.Down
                                              :
                                                (openValue < closeValue) ? CandleType.Up : CandleType.NoChange;

                    double Y1 = 0;
                    double Y2 = 0;
                    switch (candleType)
                    {
                        case CandleType.Down:
                            Y1 = oSeries.GetY(openValue.Value);
                            Y2 = cSeries.GetY(closeValue.Value);
                            break;
                        case CandleType.Up:
                        case CandleType.NoChange:
                            Y1 = oSeries.GetY(closeValue.Value);
                            Y2 = cSeries.GetY(openValue.Value);
                            break;
                    }

                    double x = chart.GetXPixel(cnt++);

                    yield return new Values
                                   {
                                       WickValue = (openValue.Value == highValue.Value) &&
                                                   (highValue.Value == lowValue.Value) &&
                                                   (lowValue.Value == closeValue.Value)
                                                     ? null
                                                     : new WickValueType
                                                         {
                                                             X = x,
                                                             Y1 = highValue.Value,
                                                             Y2 = lowValue.Value
                                                         },
                                       CandleValue = new CandleValueType
                                                       {
                                                           Type = candleType,
                                                           X1 = x - Space - HalfWick,
                                                           Y1 = Y1,
                                                           X2 = x + Space + HalfWick,
                                                           Y2 = Y2
                                                       }
                                   };
                }
            }
        }
    }
}
