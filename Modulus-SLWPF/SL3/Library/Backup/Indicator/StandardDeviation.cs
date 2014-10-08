using System.Collections.Generic;
using ModulusFE.Indicators;
using ModulusFE.Tasdk;

namespace ModulusFE
{
    public static partial class StockChartX_IndicatorsParameters
    {
        internal static void Register_StandardDeviation()
        {
            /*  Required inputs for this indicator:
              1. paramStr0 = Source (eg "msft.close")
              2. paramInt1 = Periods (eg 14)
              3. paramInt2 = Standard Deviations (eg 2)
              4. paramInt3 = Moving Average Type (eg indSimpleMovingAverage)
            */
            RegisterIndicatorParameters(IndicatorType.StandardDeviation, typeof(StandardDeviation),
                                        "Standard Deviation",
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
                                          ValueType = typeof (double)
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
    /// Standard Deviation is a common statistical calculation that measures volatility. Other technical indicators are often calculated using standard deviations.
    /// </summary>
    /// <remarks>Major highs and lows often accompany extreme volatility. High values of standard deviations indicate that the price or indicator is more volatile than usual.
    /// <list type="table">
    /// <listheader>
    /// <term>Parameters</term>
    /// </listheader>
    /// <item><term>str Source</term></item>
    /// <item><term>int Periods</term></item>
    /// <item><term>int Standard Deviations</term></item>
    /// <item><term>int Moving Average Type</term></item>
    /// </list>
    /// </remarks>
    public class StandardDeviation : Indicator
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Indicator name</param>
        /// <param name="chartPanel">Reference to a panel where it will be placed</param>
        public StandardDeviation(string name, ChartPanel chartPanel)
            : base(name, chartPanel)
        {
            _indicatorType = IndicatorType.StandardDeviation;

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
                ProcessError("Invalid Periods (min 2) for indicator " + FullName, IndicatorErrorType.ShowErrorMessage);
                return false;
            }
            double paramDbl2 = ParamDbl(2);
            if (paramDbl2 < 0.0001 || paramDbl2 > 10)
            {
                ProcessError("Invalid Standard Deviations (min > 0) for indicator " + FullName, IndicatorErrorType.ShowErrorMessage);
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
            if (pSource == null) return false;

            Navigator pNav = new Navigator();
            Recordset pRS = new Recordset();

            pRS.AddField(pSource);
            pNav.Recordset_ = pRS;


            // Calculate the indicator  
            General ta = new General();
            Recordset pInd = ta.StandardDeviation(pNav, pSource, paramInt1, paramDbl2, param3, FullName);


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

