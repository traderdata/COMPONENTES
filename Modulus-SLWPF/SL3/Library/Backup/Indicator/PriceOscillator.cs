using System.Collections.Generic;
using ModulusFE.Indicators;
using ModulusFE.Tasdk;

namespace ModulusFE
{
    public static partial class StockChartX_IndicatorsParameters
    {
        internal static void Register_PriceOscillator()
        {
            /*  Required inputs for this indicator:
              1. paramStr0 = Source (eg "msft.close")
              2. paramInt1 = Long Cycle (eg 22)
              3. paramInt2 = Short Cycle (eg 14)
              4. paramInt3 = Moving Average Type (eg indSimpleMovingAverage)
            */
            RegisterIndicatorParameters(IndicatorType.PriceOscillator, typeof(PriceOscillator),
                                        "Price Oscillator",
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
                                          // Long Cycle (eg 22)
                                          Name = Indicator.GetParamName(ParameterType.ptLongCycle),
                                          ParameterType = ParameterType.ptLongCycle,
                                          DefaultValue = 22,
                                          ValueType = typeof (int)
                                        },
                                      new IndicatorParameter
                                        {
                                          // Short Cycle (eg 14)
                                          Name = Indicator.GetParamName(ParameterType.ptShortCycle),
                                          ParameterType = ParameterType.ptShortCycle,
                                          DefaultValue = 14,
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
    /// The Price Oscillator shows a spread of two moving averages.
    /// </summary>
    /// <remarks>The Price Oscillator is basically a moving average spread. Buying usually occurs when the oscillator rises, and conversely, selling usually occurs when the oscillator falls.
    /// <list type="table">
    /// <listheader>
    /// <term>Parameters</term>
    /// </listheader>
    /// <item><term>str Source</term></item>
    /// <item><term>int Long Cycle</term></item>
    /// <item><term>int Short Cycle</term></item>
    /// <item><term>int Moving Average Type</term></item>
    /// </list>
    /// </remarks>
    public class PriceOscillator : Indicator
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Indicator name</param>
        /// <param name="chartPanel">Reference to a panel where it will be placed</param>
        public PriceOscillator(string name, ChartPanel chartPanel)
            : base(name, chartPanel)
        {
            _indicatorType = IndicatorType.PriceOscillator;

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
            if (paramInt1 < 3 || paramInt1 > iSize / 2)
            {
                ProcessError("Invalid Long Cycle for indicator " + FullName, IndicatorErrorType.ShowErrorMessage);
                return false;
            }
            int paramInt2 = ParamInt(2);
            if (paramInt2 < 2 || paramInt2 > iSize / 2 || paramInt2 >= paramInt1)
            {
                ProcessError("Invalid Short Cycle for indicator " + FullName, IndicatorErrorType.ShowErrorMessage);
                return false;
            }
            IndicatorType param3 = (IndicatorType)ParamInt(3);
            if (param3 < Constants.MA_START || param3 > Constants.MA_END)
            {
                ProcessError("Invalid Moving Average Type for indicator " + FullName, IndicatorErrorType.ShowErrorMessage);
                return false;
            }

            // Get the data
            string paramStr0 = ParamStr(0);
            Field pSource = SeriesToField("Source", paramStr0, iSize);
            if (!EnsureField(pSource, paramStr0)) return false;

            Navigator pNav = new Navigator();
            Recordset pRS = new Recordset();

            pRS.AddField(pSource);
            pNav.Recordset_ = pRS;


            // Calculate the indicator
            Oscillator ta = new Oscillator();
            Recordset pInd = ta.PriceOscillator(pNav, pSource, paramInt1, paramInt2, param3, FullName);


            // Output the indicator values
            Clear();
            for (int n = 0; n < iSize; ++n)
            {
                AppendValue(DM.TS(n), n < paramInt1 ? null : pInd.Value(FullName, n + 1));
            }
            return _calculateResult = PostCalculate();
        }
    }
}

