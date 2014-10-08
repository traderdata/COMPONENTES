using System.Collections.Generic;
using ModulusFE.Indicators;
using ModulusFE.Tasdk;

namespace ModulusFE
{
    public static partial class StockChartX_IndicatorsParameters
    {
        internal static void Register_RainbowOscillator()
        {
            /*  Required inputs for this indicator:
              1. paramStr0 = Source (eg "msft.close")
              2. paramInt1 = Levels (eg 9)
              3. paramInt2 = Moving Average Type (eg indSimpleMovingAverage)
            */
            RegisterIndicatorParameters(IndicatorType.RainbowOscillator, typeof(RainbowOscillator),
                                        "Rainbow Oscillator",
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
                                          // Levels (eg 9)
                                          Name = Indicator.GetParamName(ParameterType.ptLevels),
                                          ParameterType = ParameterType.ptLevels,
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
    /// The rainbow oscillator is based upon multiple time frames of a moving average. 
    /// </summary>
    /// <remarks>When values stay above 80 or below 20, the trend may reverse suddenly.
    /// <list type="table">
    /// <listheader>
    /// <term>Parameters</term>
    /// </listheader>
    /// <item><term>str Source</term></item>
    /// <item><term>int Levels</term></item>
    /// <item><term>int Moving Average Type</term></item>
    /// </list>
    /// </remarks>
    public class RainbowOscillator : Indicator
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Indicator name</param>
        /// <param name="chartPanel">Reference to a panel where it will be placed</param>
        public RainbowOscillator(string name, ChartPanel chartPanel)
            : base(name, chartPanel)
        {
            _indicatorType = IndicatorType.RainbowOscillator;

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
            if (paramInt1 < 2 || paramInt1 > iSize / 2)
            {
                ProcessError("Invalid Level (min 2) for indicator " + FullName, IndicatorErrorType.ShowErrorMessage);
                return false;
            }
            IndicatorType param2 = (IndicatorType)ParamInt(2);
            if (param2 < Constants.MA_START || param2 > Constants.MA_END)
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
            Recordset pInd = ta.RainbowOscillator(pNav, pSource, paramInt1, param2, FullName);


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

