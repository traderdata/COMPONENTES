using System.Collections.Generic;
using ModulusFE.Indicators;
using ModulusFE.Tasdk;

namespace ModulusFE
{
    public static partial class StockChartX_IndicatorsParameters
    {
        internal static void Register_UltimateOscillator()
        {
            /*  Required inputs for this indicator:
              1. paramStr0 = Symbol (eg "msft")
              2. paramInt1 = Cycle 1 (eg 7)
              3. paramInt2 = Cycle 2 (eg 14)
              4. paramInt3 = Cycle 3 (eg 28)
            */
            RegisterIndicatorParameters(IndicatorType.UltimateOscillator, typeof(UltimateOscillator),
                                        "Ultimate Oscillator",
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
                                          // Cycle 1 (eg 7)
                                          Name = Indicator.GetParamName(ParameterType.ptCycle1),
                                          ParameterType = ParameterType.ptCycle1,
                                          DefaultValue = 7,
                                          ValueType = typeof (int)
                                        },
                                      new IndicatorParameter
                                        {
                                          // Cycle 2 (eg 14)
                                          Name = Indicator.GetParamName(ParameterType.ptCycle2),
                                          ParameterType = ParameterType.ptCycle2,
                                          DefaultValue = 14,
                                          ValueType = typeof (int)
                                        },
                                      new IndicatorParameter
                                        {
                                          // Cycle 1 (eg 28)
                                          Name = Indicator.GetParamName(ParameterType.ptCycle3),
                                          ParameterType = ParameterType.ptCycle3,
                                          DefaultValue = 28,
                                          ValueType = typeof (int)
                                        },
                                    });
        }
    }
}

namespace ModulusFE.Indicators
{
    /// <summary>
    /// The Ultimate Oscillator compares prices with three oscillators, using three different periods for calculations.
    /// </summary>
    /// <remarks>The most popular interpretation of the Ultimate Oscillator is price/indicator divergence.
    /// <list type="table">
    /// <listheader>
    /// <term>Parameters</term>
    /// </listheader>
    /// <item><term>str Symbol</term></item>
    /// <item><term>int Cycle 1</term></item>
    /// <item><term>int Cycle 2</term></item>
    /// <item><term>int Cycle 3</term></item>
    /// </list>
    /// </remarks>
    public class UltimateOscillator : Indicator
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Indicator name</param>
        /// <param name="chartPanel">Reference to a panel where it will be placed</param>
        public UltimateOscillator(string name, ChartPanel chartPanel)
            : base(name, chartPanel)
        {
            _indicatorType = IndicatorType.UltimateOscillator;

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
                ProcessError("Invalid Cycle 1 for indicator " + FullName, IndicatorErrorType.ShowErrorMessage);
                return false;
            }
            int paramInt2 = ParamInt(2);
            if (paramInt2 < 1)
            {
                ProcessError("Invalid Cycle 2 for indicator " + FullName, IndicatorErrorType.ShowErrorMessage);
                return false;
            }
            int paramInt3 = ParamInt(3);
            if (paramInt3 < 1)
            {
                ProcessError("Invalid Cycle 3 for indicator " + FullName, IndicatorErrorType.ShowErrorMessage);
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
            Recordset pInd = ta.UltimateOscillator(pNav, pRS, paramInt1, paramInt2, paramInt3, FullName);


            // Output the indicator values
            Clear();
            int max = paramInt1;
            if (paramInt2 > max) max = paramInt2;
            if (paramInt3 > max) max = paramInt3;
            for (int n = 0; n < iSize; ++n)
            {
                AppendValue(DM.TS(n), n < max ? null : pInd.Value(FullName, n + 1));
            }

            return _calculateResult = PostCalculate();
        }
    }
}

