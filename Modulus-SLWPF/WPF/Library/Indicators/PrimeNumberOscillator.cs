using System.Collections.Generic;
using ModulusFE.Indicators;
using ModulusFE.Tasdk;

namespace ModulusFE
{
    public static partial class StockChartX_IndicatorsParameters
    {
        internal static void Register_PrimeNumberOscillator()
        {
            /*  Required inputs for this indicator:
              1. paramStr0 = Source (eg "msft.close")
            */
            RegisterIndicatorParameters(IndicatorType.PrimeNumberOscillator, typeof(PrimeNumberOscillator),
                                        "Prime Number Oscillator",
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
                                    });
        }
    }
}

namespace ModulusFE.Indicators
{
    /// <summary>
    /// The prime numbers oscillator was developed by Modulus Financial Engineering, Inc. This indicator finds the nearest prime number from either the top or bottom of the series, and plots the difference between that prime number and the series itself.
    /// </summary>
    /// <remarks>This indicator can be used to spot market turning points. When the oscillator remains at the same high point for two consecutive periods in the positive range, consider selling. Conversely, when the oscillator remains at a low point for two consecutive periods in the negative range, consider buying.
    /// <list type="table">
    /// <listheader>
    /// <term>Parameters</term>
    /// </listheader>
    /// <item><term>str Source</term></item>
    /// </list>
    /// </remarks>
    public class PrimeNumberOscillator : Indicator
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Indicator name</param>
        /// <param name="chartPanel">Reference to a panel where it will be placed</param>
        public PrimeNumberOscillator(string name, ChartPanel chartPanel)
            : base(name, chartPanel)
        {
            _indicatorType = IndicatorType.PrimeNumberOscillator;

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
            Recordset pInd = ta.PrimeNumberOscillator(pNav, pSource, FullName);


            // Output the indicator values
            Clear();
            for (int n = 0; n < iSize; ++n)
            {
                AppendValue(DM.TS(n), pInd.Value(FullName, n + 1));
            }
            return _calculateResult = PostCalculate();
        }
    }
}
