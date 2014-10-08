using System.Collections.Generic;
using ModulusFE.Indicators;
using ModulusFE.Tasdk;

namespace ModulusFE
{
    public static partial class StockChartX_IndicatorsParameters
    {
        internal static void Register_BollingerBands()
        {
            /*  Required inputs for this indicator:
              1. paramStr[0] = Source (eg "msft.close")
              2. paramInt[1] = Periods (eg 14)
              3. paramInt[2] = Standard Deviations (eg 2)
              4. paramInt[3] = Moving Average Type (eg indSimpleMovingAverage)
            */
            RegisterIndicatorParameters(IndicatorType.BollingerBands, typeof(BollingerBands),
                                        "Bollinger Bands",
                                        new List<IndicatorParameter>
                                    {
                                      new IndicatorParameter
                                        {
                                          // Source (eg "msft.close")
                                          Name = Indicator.GetParamName(ParameterType.ptSource),
                                          ParameterType = ParameterType.ptSource,
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
                                      new IndicatorParameter
                                        {
                                          // Standard Deviations (eg 2)
                                          Name = Indicator.GetParamName(ParameterType.ptStandardDeviations),
                                          ParameterType = ParameterType.ptStandardDeviations,
                                          DefaultValue = 2,
                                          ValueType = typeof (int)
                                        },
                                      new IndicatorParameter
                                        {
                                          // Moving Average Type (eg indSimpleMovingAverage)
                                          Name = Indicator.GetParamName(ParameterType.ptMAType),
                                          ParameterType = ParameterType.ptMAType,
                                          DefaultValue = IndicatorType.SimpleMovingAverage,
                                          ValueType = typeof (IndicatorType)
                                        }
                                    });
        }
    }
}

namespace ModulusFE.Indicators
{
    /// <summary>
    /// Bollinger Bands are similar in comparison to moving average envelopes. Bollinger Bands are calculated using standard deviations instead of shifting bands by a fixed percentage.
    /// </summary>
    /// <remarks>Bollinger Bands (as with most bands) can be imposed over an actual price or another indicator.
    /// When prices rise above the upper band or fall below the lower band, a change in direction may occur when the price penetrates the band after a small reversal from the opposite direction.
    /// <list type="table">
    /// <listheader>
    /// <term>Parameters</term>
    /// </listheader>
    /// <item><term>str Source</term></item>
    /// <item><term>int Periods</term></item>
    /// <item><term>int Standard Deviations</term></item>
    /// <item><term>int Moving Average Type </term></item>
    /// </list>
    /// </remarks>

    public class BollingerBands : Indicator
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Indicator name</param>
        /// <param name="chartPanel">Reference to a panel where it will be placed</param>
        public BollingerBands(string name, ChartPanel chartPanel)
            : base(name, chartPanel)
        {
            _indicatorType = IndicatorType.BollingerBands;

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
            if (size == 0) return false;

            if (ParamInt(1) < 1 || ParamInt(1) > size / 2)
            {
                ProcessError("Invalid Periods for indicator " + FullName, IndicatorErrorType.ShowErrorMessage);
                return false;
            }
            if (ParamInt(2) < 0 || ParamInt(2) > 10)
            {
                ProcessError("Invalid Standard Deviations for indicator " + FullName, IndicatorErrorType.ShowErrorMessage);
                return false;
            }
            if (ParamInt(3) < (int)Constants.MA_START || ParamInt(3) > (int)Constants.MA_END)
            {
                ProcessError("Invalid Moving Average Type for indicator " + FullName, IndicatorErrorType.ShowErrorMessage);
                return false;
            }

            Field pSource = SeriesToField("Source", ParamStr(0), size);
            if (!EnsureField(pSource, ParamStr(0))) return false;

            Navigator pNav = new Navigator();
            Recordset pRS = new Recordset();

            pRS.AddField(pSource);
            pNav.Recordset_ = pRS;

            // Calculate the indicator
            Bands ta = new Bands();
            Recordset pInd = ta.BollingerBands(pNav, pSource, ParamInt(1), ParamInt(2), (IndicatorType)ParamInt(3));

            // Output the indicator values
            Clear();
            Series series = _chartPanel._chartX.GetSeriesByName(ParamStr(0));

            TwinIndicator pTop = (TwinIndicator)EnsureSeries(FullName + " Top");
            pTop.SetStrokeColor(StrokeColor, false);
            pTop.SetStrokeThickness(StrokeThickness, false);
            pTop.SetStrokePattern(_strokePattern, false);

            TwinIndicator pBottom = (TwinIndicator)EnsureSeries(FullName + " Bottom");
            pBottom.SetStrokeColor(StrokeColor, false);
            pBottom.SetStrokeThickness(StrokeThickness, false);
            pBottom.SetStrokePattern(_strokePattern, false);

            int paramInt1 = ParamInt(1);
            for (int n = 0; n < size; ++n)
            {
                double? top;
                double? median;
                double? bottom;
                if (n < paramInt1)
                {
                    top = null;
                    median = null;
                    bottom = null;
                }
                else
                {
                    top = pInd.Value("Bollinger Band Top", n + 1);
                    median = pInd.Value("Bollinger Band Median", n + 1);
                    bottom = pInd.Value("Bollinger Band Bottom", n + 1);
                }
                AppendValue(series[n].TimeStamp, median);
                pTop.AppendValue(series[n].TimeStamp, top);
                pBottom.AppendValue(series[n].TimeStamp, bottom);
            }

            return _calculateResult = PostCalculate();
        }
    }
}
