using System.Collections.Generic;
using System.Windows.Media;
using ModulusFE.Indicators;
using ModulusFE.Tasdk;

namespace ModulusFE
{
    public static partial class StockChartX_IndicatorsParameters
    {
        internal static void Register_StochasticOscillator()
        {
            /*  Required inputs for this indicator:
              1. paramStr0 = Symbol (eg "msft")
              2. paramInt1 = %K Periods (eg 14)
              3. paramInt2 = %K Slowing (eg 3)
              4. paramInt3 = %D Periods (eg 5)
              5. paramInt[4] = Moving Average Type (eg indSimpleMovingAverage)
            */
            RegisterIndicatorParameters(IndicatorType.StochasticOscillator, typeof(StochasticOscillator),
                                        "Stochastic Oscillator",
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
                                          // %K Periods (eg 14)
                                          Name = Indicator.GetParamName(ParameterType.ptPctKPeriods),
                                          ParameterType = ParameterType.ptPctKPeriods,
                                          DefaultValue = 9,
                                          ValueType = typeof (int)
                                        },
                                      new IndicatorParameter
                                        {
                                          // %K Slowing (eg 3)
                                          Name = Indicator.GetParamName(ParameterType.ptPctKSlowing),
                                          ParameterType = ParameterType.ptPctKSlowing,
                                          DefaultValue = 3,
                                          ValueType = typeof (int)
                                        },
                                      new IndicatorParameter
                                        {
                                          // %D Periods (eg 5)
                                          Name = Indicator.GetParamName(ParameterType.ptPctDPeriods),
                                          ParameterType = ParameterType.ptPctDPeriods,
                                          DefaultValue = 9,
                                          ValueType = typeof (int)
                                        },
                                      new IndicatorParameter
                                        {
                                          // Moving Average Type (eg indSimpleMovingAverage)
                                          Name = Indicator.GetParamName(ParameterType.ptMAType),
                                          ParameterType = ParameterType.ptMAType,
                                          DefaultValue = IndicatorType.SimpleMovingAverage,
                                          ValueType = typeof (IndicatorType)
                                        },
                                    });
        }
    }
}

namespace ModulusFE.Indicators
{
    /// <summary>
    /// The Stochastic Oscillator is a popular indicator that shows where a security’s price has closed in proportion to its closing price range over a specified period of time.
    /// </summary>
    /// <remarks>The Stochastic Oscillator has two components: %K and %D. %K is most often displayed as a solid line and %D is often shown as a dotted line. The most widely used method for interpreting the Stochastic Oscillator is to buy when either component rises above 80 or sell when either component falls below 20. Another way to interpret the Stochastic Oscillator is to buy when %K rises above %D, and conversely, sell when %K falls below %D.
    /// 
    /// The most commonly used arguments are 9 for %K periods, 3 for %K slowing periods and 3 for %D smoothing.
    /// <list type="table">
    /// <listheader>
    /// <term>Parameters</term>
    /// </listheader>
    /// <item><term>str Symbol</term></item>
    /// <item><term>int %K Periods</term></item>
    /// <item><term>int %K Slowing</term></item>
    /// <item><term>int %D Periods</term></item>
    /// <item><term>int Moving Average Type</term></item>
    /// </list>
    /// </remarks>
    public class StochasticOscillator : Indicator
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Indicator name</param>
        /// <param name="chartPanel">Reference to a panel where it will be placed</param>
        public StochasticOscillator(string name, ChartPanel chartPanel)
            : base(name, chartPanel)
        {
            _indicatorType = IndicatorType.StochasticOscillator;

            ForceLinearChart = false;
            ForceOscilatorPaint = true;

            Init();
        }

        /// <summary>
        /// Action to be executd for calculating indicator
        /// </summary>
        /// <returns>for future usage. Must be ignored at this time.</returns>
        protected override bool TrueAction()
        {
            // Validate
            int iSize = _chartPanel._chartX.RecordCount;
            if (iSize == 0)
                return false;

            int paramInt1 = ParamInt(1);
            if (paramInt1 < 1)
            {
                ProcessError("Invalid %K Periods for indicator " + FullName, IndicatorErrorType.ShowErrorMessage);
                return false;
            }
            int paramInt2 = ParamInt(2);
            if (paramInt2 < 2 || paramInt2 > iSize / 2)
            {
                ProcessError("Invalid %K Slowing Periods (min 2) for indicator " + FullName, IndicatorErrorType.ShowErrorMessage);
                return false;
            }
            int paramInt3 = ParamInt(3);
            if (paramInt3 <= 0 || paramInt3 > iSize / 2)
            {
                ProcessError("Invalid %D Periods for indicator " + FullName, IndicatorErrorType.ShowErrorMessage);
                return false;
            }
            IndicatorType param4 = (IndicatorType)ParamInt(4);
            if (param4 < Constants.MA_START || param4 > Constants.MA_END)
            {
                ProcessError("Invalid Moving Average Type for indicator " + FullName, IndicatorErrorType.ShowErrorMessage);
                return false;
            }

            // Get the data
            string paramStr0 = ParamStr(0);
            Field pHigh = SeriesToField("High", paramStr0 + ".high", iSize);
            if (!EnsureField(pHigh, paramStr0 + ".high")) return false;
            Field pLow = SeriesToField("Low", paramStr0 + ".low", iSize);
            if (!EnsureField(pLow, paramStr0 + ".low")) return false;
            Field pClose = SeriesToField("Close", paramStr0 + ".close", iSize);
            if (!EnsureField(pClose, paramStr0 + ".close")) return false;

            Navigator pNav = new Navigator();
            Recordset pRS = new Recordset();

            pRS.AddField(pHigh);
            pRS.AddField(pLow);
            pRS.AddField(pClose);

            pNav.Recordset_ = pRS;


            // Calculate the indicator
            Oscillator ta = new Oscillator();

            Recordset pInd = ta.StochasticOscillator(pNav, pRS, paramInt1, paramInt2, paramInt3, param4);


            // Output the indicator values
            Clear();

            TwinIndicator sPctK = (TwinIndicator)EnsureSeries(FullName + " %K");
            sPctK.SetStrokeColor(Colors.Blue, false);
            sPctK.SetStrokeThickness(StrokeThickness, false);

            ForceLinearChart = true;
            sPctK.ForceLinearChart = true;

            _strokePattern = LinePattern.Dot;
            _title = FullName + " %D";

            for (int n = 0; n < iSize; ++n)
            {
                double? pctd = n < paramInt3 ? null : pInd.Value("%D", n + 1);
                double? pctk = n < paramInt1 ? null : pInd.Value("%K", n + 1);

                AppendValue(DM.TS(n), pctd);
                sPctK.AppendValue(DM.TS(n), pctk);
            }

            return _calculateResult = PostCalculate();
        }
    }
}

