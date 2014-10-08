using System.Collections.Generic;
using ModulusFE.Indicators;
using ModulusFE.Tasdk;

namespace ModulusFE
{
    public static partial class StockChartX_IndicatorsParameters
    {
        internal static void Register_HighMinusLow()
        {
            /*  Required inputs for this indicator:
              1. paramStr0 = Symbol (eg "msft")   
            */
            RegisterIndicatorParameters(IndicatorType.HighMinusLow, typeof(HighMinusLow),
                                        "High Minus Low",
                                        new List<IndicatorParameter>
                                    {
                                      new IndicatorParameter
                                        {
                                          // Source (eg "msft.close")
                                          Name = Indicator.GetParamName(ParameterType.ptSymbol),
                                          ParameterType = ParameterType.ptSymbol,
                                          DefaultValue = "",
                                          ValueType = typeof (string)
                                        }
                                    });
        }
    }
}

namespace ModulusFE.Indicators
{
    /// <summary>
    /// Returns the high price minus the low price.
    /// </summary>
    /// <remarks>
    /// <list type="table">
    /// <listheader>
    /// <term>Parameters</term>
    /// </listheader>
    /// <item><term>str Symbol</term></item>
    /// </list>
    /// </remarks>
    public class HighMinusLow : Indicator
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Indicator name</param>
        /// <param name="chartPanel">Reference to a panel where it will be placed</param>
        public HighMinusLow(string name, ChartPanel chartPanel)
            : base(name, chartPanel)
        {
            _indicatorType = IndicatorType.HighMinusLow;

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

            // Get the data
            string paramStr0 = ParamStr(0);
            Field pHigh = SeriesToField("High", paramStr0 + ".high", size);
            if (!EnsureField(pHigh, paramStr0 + ".high")) return false;
            Field pLow = SeriesToField("Low", paramStr0 + ".low", size);
            if (!EnsureField(pLow, paramStr0 + ".low")) return false;

            Navigator pNav = new Navigator();
            Recordset pRS = new Recordset();

            pRS.AddField(pHigh);
            pRS.AddField(pLow);


            pNav.Recordset_ = pRS;


            // Calculate the indicator
            General ta = new General();
            Recordset pInd = ta.HighMinusLow(pNav, pRS, FullName);


            // Output the indicator values
            Clear();
            for (int n = 0; n < size; ++n)
            {
                AppendValue(DM.GetTimeStampByIndex(n), pInd.Value(FullName, n + 1));
            }

            return _calculateResult = PostCalculate();
        }
    }
}
