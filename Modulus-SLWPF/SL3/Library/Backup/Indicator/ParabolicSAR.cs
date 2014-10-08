using System.Collections.Generic;
using ModulusFE.Indicators;
using ModulusFE.Tasdk;

namespace ModulusFE
{
    public static partial class StockChartX_IndicatorsParameters
    {
        internal static void Register_ParabolicSAR()
        {
            /*  Required inputs for this indicator:
              1. paramStr0 = Symbol (eg "msft")   
              2. paramDbl1 = MinAF (eg 0.02)
              3. paramDbl2 = MaxAF (eg 0.2)
            */
            RegisterIndicatorParameters(IndicatorType.ParabolicSAR, typeof(ParabolicSAR),
                                        "Parabolic SAR",
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
                                          // MinAF (eg 0.02)
                                          Name = Indicator.GetParamName(ParameterType.ptMinAF),
                                          ParameterType = ParameterType.ptMinAF,
                                          DefaultValue = 0.02,
                                          ValueType = typeof (double)
                                        },
                                      new IndicatorParameter
                                        {
                                          // MaxAF (eg 0.2)
                                          Name = Indicator.GetParamName(ParameterType.ptMaxAF),
                                          ParameterType = ParameterType.ptMaxAF,
                                          DefaultValue = 0.2,
                                          ValueType = typeof (double)
                                        },
                                    });
        }
    }
}

namespace ModulusFE.Indicators
{
    /// <summary>
    /// The Parabolic SAR was  developed by Welles Wilder. This indicator is always in the market (whenever a position is closed, an opposing position is taken).  The Parabolic SAR indicator is most often used to set trailing price stops. A stop and reversal (SAR) occurs when the price penetrates a  Parabolic SAR level.
    /// </summary>
    /// <remarks>
    /// <list type="table">
    /// <listheader>
    /// <term>Parameters</term>
    /// </listheader>
    /// <item><term>str Symbol</term></item>
    /// <item><term>dbl Min AF (accumulation factor)</term></item>
    /// <item><term>dbl Max AF (accumulation factor)</term></item>
    /// </list>
    /// </remarks>
    public class ParabolicSAR : Indicator
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Indicator name</param>
        /// <param name="chartPanel">Reference to a panel where it will be placed</param>
        public ParabolicSAR(string name, ChartPanel chartPanel)
            : base(name, chartPanel)
        {
            _indicatorType = IndicatorType.ParabolicSAR;

            Init();

            _strokePattern = LinePattern.Dot;
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

            double paramDbl1 = ParamDbl(1);
            if (paramDbl1 <= 0)
            {
                ProcessError("Invalid Max AF for indicator " + FullName, IndicatorErrorType.ShowErrorMessage);
                return false;
            }
            double paramDbl2 = ParamDbl(2);
            if (paramDbl2 <= 0)
            {
                ProcessError("Invalid Min AF for indicator " + FullName, IndicatorErrorType.ShowErrorMessage);
                return false;
            }


            // Get the data
            string paramStr0 = ParamStr(0);
            Field pHigh = SeriesToField("High", paramStr0 + ".high", iSize);
            if (!EnsureField(pHigh, paramStr0 + ".high")) return false;
            Field pLow = SeriesToField("Low", paramStr0 + ".low", iSize);
            if (!EnsureField(pLow, paramStr0 + ".low")) return false;

            Navigator pNav = new Navigator();
            Recordset pRS = new Recordset();

            pRS.AddField(pHigh);
            pRS.AddField(pLow);

            pNav.Recordset_ = pRS;


            // Calculate the indicator
            Oscillator ta = new Oscillator();
            Recordset pInd = ta.ParabolicSAR(pNav, pHigh, pLow, paramDbl1, paramDbl2, FullName);


            // Output the indicator values
            Clear();
            for (int n = 0; n < iSize; ++n)
            {
                AppendValue(DM.TS(n), n < 2 ? null : pInd.Value(FullName, n + 1));
            }
            return _calculateResult = PostCalculate();
        }
    }
}

