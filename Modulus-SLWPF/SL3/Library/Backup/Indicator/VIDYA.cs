using System.Collections.Generic;
using ModulusFE.Indicators;
using ModulusFE.Tasdk;

namespace ModulusFE
{
    public static partial class StockChartX_IndicatorsParameters
    {
        internal static void Register_VIDYA()
        {
            /*  Required inputs for this indicator:
              1. paramStr0 = Source (eg "msft.close")
              2. paramInt1 = Periods (eg 14)
              3. paramDbl2 = R2 Scale (0.1 to 0.95)
            */
            RegisterIndicatorParameters(IndicatorType.VIDYA, typeof(VIDYA),
                                        "VIDYA",
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
                                          // R2 Scale (0.1 to 0.95)
                                          Name = Indicator.GetParamName(ParameterType.ptR2Scale),
                                          ParameterType = ParameterType.ptR2Scale,
                                          DefaultValue = 0.65,
                                          ValueType = typeof (double)
                                        },
                                    });
        }
    }
}

namespace ModulusFE.Indicators
{
    /// <summary>
    /// VIDYA (Volatility Index Dynamic Average), developed by Chande, is a moving average derived from linear regression R2.
    /// </summary>
    /// <remarks>A Moving Average is most often used to average values for a smoother representation of the underlying price or 
    /// indicator. Because VIDYA is a derivative of linear regression, it quickly adapts to volatility.
    /// R2Scale is a double value specifying the R-Squared scale to use in the linear regression calculations. 
    /// Chande recommends a value between 0.5 and 0.8 (default value is 0.65).
    /// <list type="table">
    /// <listheader>
    /// <term>Parameters</term>
    /// </listheader>
    /// <item><term>str Source</term></item>
    /// <item><term>int Periods</term></item>
    /// <item><term>dbl R2 Scale</term></item>
    /// </list>
    /// </remarks>
    public class VIDYA : Indicator
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Indicator name</param>
        /// <param name="chartPanel">Reference to a panel where it will be placed</param>
        public VIDYA(string name, ChartPanel chartPanel)
            : base(name, chartPanel)
        {
            _indicatorType = IndicatorType.VIDYA;

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
            if (paramInt1 < 1 || paramInt1 > iSize / 2)
            {
                ProcessError("Invalid Periods for indicator " + FullName, IndicatorErrorType.ShowErrorMessage);
                return false;
            }

            double paramDbl2 = ParamDbl(2);
            if (paramDbl2 < 0.1 || paramDbl2 > 0.95)
            {
                ProcessError("Invalid R2 Scale (0.1 to 0.95) for indicator " + FullName, IndicatorErrorType.ShowErrorMessage);
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
            MovingAverage ta = new MovingAverage();
            Recordset pInd = ta.VIDYA(pNav, pSource, paramInt1, paramDbl2, FullName);


            // Output the indicator values
            Clear();
            for (int n = 0; n < iSize; ++n)
            {
                AppendValue(DM.TS(n), n < paramInt1 * 2 ? null : pInd.Value(FullName, n + 1));
            }
            return _calculateResult = PostCalculate();
        }
    }
}
