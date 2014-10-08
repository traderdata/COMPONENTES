using System.Collections.Generic;
using ModulusFE.Indicators;
using ModulusFE.Tasdk;

namespace ModulusFE
{
    public static partial class StockChartX_IndicatorsParameters
    {
        internal static void Register_HighLowBands()
        {
            /*  Required inputs for this indicator:
              1. paramStr0 = Symbol (eg "msft")
              2. paramInt1 = Periods (eg 14)
            */
            RegisterIndicatorParameters(IndicatorType.HighLowBands, typeof(HighLowBands),
                                        "High Low Bands",
                                        new List<IndicatorParameter>
                                    {
                                      new IndicatorParameter
                                        {
                                          // Symbol (eg "msft")
                                          Name = Indicator.GetParamName(ParameterType.ptSymbol),
                                          ParameterType = ParameterType.ptSymbol,
                                          DefaultValue = "",
                                          ValueType = typeof (string)
                                        },
                                      new IndicatorParameter
                                        {
                                          // Periods (eg 14)
                                          Name = Indicator.GetParamName(ParameterType.ptPeriods),
                                          ParameterType = ParameterType.ptPeriods,
                                          DefaultValue = 14,
                                          ValueType = typeof (int)
                                        },
                                    });
        }
    }
}

namespace ModulusFE.Indicators
{
    /// <summary>
    /// High Low Bands consist of triangular moving averages calculated from the underling price, shifted up and down by a fixed percentage, and include a median value.
    /// </summary>
    /// <remarks>When prices rise above the upper band or fall below the lower band, a change in direction may occur when the price penetrates the band after a small reversal from the opposite direction.
    /// <list type="table">
    /// <listheader>
    /// <term>Parameters</term>
    /// </listheader>
    /// <item><term>str Symbol</term></item>
    /// <item><term>int Periods</term></item>
    /// </list>
    /// </remarks>
    public class HighLowBands : Indicator
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Indicator name</param>
        /// <param name="chartPanel">Reference to a panel where it will be placed</param>
        public HighLowBands(string name, ChartPanel chartPanel)
            : base(name, chartPanel)
        {
            _indicatorType = IndicatorType.HighLowBands;

            Init();
        }

        /// <summary>
        /// Action to be executd for calculating indicator
        /// </summary>
        /// <returns>for future usage. Must be ignored at this time.</returns>
        protected override bool TrueAction()
        {
            // Validate
            int size = _chartPanel._chartX.RecordCount;
            if (size == 0)
                return false;

            int paramInt1 = ParamInt(1);
            if (paramInt1 < 6 || paramInt1 > size / 2)
            {
                ProcessError("Invalid Periods (min 6) for indicator " + FullName, IndicatorErrorType.ShowErrorMessage);
                return false;
            }


            // Get the data
            string paramStr0 = ParamStr(0);
            Field pHigh = SeriesToField("High", paramStr0 + ".high", size);
            if (!EnsureField(pHigh, paramStr0 + ".high")) return false;
            Field pLow = SeriesToField("Low", paramStr0 + ".low", size);
            if (!EnsureField(pLow, paramStr0 + ".low")) return false;
            Field pClose = SeriesToField("Close", paramStr0 + ".close", size);

            if (!EnsureField(pClose, paramStr0 + ".close")) return false;
            Navigator pNav = new Navigator();
            Recordset pRS = new Recordset();

            pRS.AddField(pHigh);
            pRS.AddField(pLow);
            pRS.AddField(pClose);

            pNav.Recordset_ = pRS;


            // Calculate the indicator
            Bands ta = new Bands();
            Recordset pInd = ta.HighLowBands(pNav, pHigh, pLow, pClose, paramInt1);


            // Output the indicator values
            Clear();

            TwinIndicator sTop = (TwinIndicator)EnsureSeries(FullName + " Top");
            sTop.SetStrokeColor(StrokeColor, false);
            sTop.SetStrokeThickness(StrokeThickness, false);

            TwinIndicator sBottom = (TwinIndicator)EnsureSeries(FullName + " Bottom");
            sBottom.SetStrokeColor(StrokeColor, false);
            sBottom.SetStrokeThickness(StrokeThickness, false);

            for (int n = 0; n < size; ++n)
            {
                double? top;
                double? bottom;
                double? median;
                if (n < paramInt1)
                {
                    top = null;
                    median = null;
                    bottom = null;
                }
                else
                {
                    top = pInd.Value("High Low Bands Top", n + 1);
                    median = pInd.Value("High Low Bands Median", n + 1);
                    bottom = pInd.Value("High Low Bands Bottom", n + 1);
                }

                AppendValue(DM.GetTimeStampByIndex(n), median);
                sTop.AppendValue(DM.GetTimeStampByIndex(n), top);
                sBottom.AppendValue(DM.GetTimeStampByIndex(n), bottom);
            }

            return _calculateResult = PostCalculate();
        }
    }
}

