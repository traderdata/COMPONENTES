using System.Collections.Generic;
using System.Windows.Media;
using ModulusFE.Indicators;
using ModulusFE.Tasdk;

namespace ModulusFE
{
    public static partial class StockChartX_IndicatorsParameters
    {
        internal static void Register_RAVI()
        {
            RegisterIndicatorParameters(IndicatorType.RAVI, typeof(RAVI),
                                        "RAVI",
                                        new List<IndicatorParameter>
                                    {
                                      new IndicatorParameter
                                        {
                                          // Symbol (eg "msft")
                                          Name = Indicator.GetParamName(ParameterType.ptSource),
                                          ParameterType = ParameterType.ptSource,
                                          DefaultValue = "",
                                          ValueType = typeof (string)
                                        },
                                      new IndicatorParameter
                                        {
                                          // Short Cycle (eg 13)
                                          Name = Indicator.GetParamName(ParameterType.ptShortCycle),
                                          ParameterType = ParameterType.ptShortCycle,
                                          DefaultValue = 8,
                                          ValueType = typeof (int)
                                        },
                                      new IndicatorParameter
                                        {
                                          // Long cycle (eg 26)
                                          Name = Indicator.GetParamName(ParameterType.ptLongCycle),
                                          ParameterType = ParameterType.ptLongCycle,
                                          DefaultValue = 14,
                                          ValueType = typeof (int)
                                        },
                                    });
        }
    }
}

namespace ModulusFE.Indicators
{
    /// <summary>
    /// RAVI (Rapid Adaptive Variance Indicator) by Tushar Chande, measures trend intensity. The formula is based on VIDYA (Volatility Based Index Dynamic Average), also by Tushar Chande.
    /// </summary>
    /// <remarks>Increasing values of RAVI indicate that a trend is forming, whereas decreasing values of RAVI indicate that the current trend is ending.
    /// <list type="table">
    /// <listheader>
    /// <term>Parameters</term>
    /// </listheader>
    /// <item><term>str Symbol</term></item>
    /// <item><term>int Short Cycle</term></item>
    /// <item><term>int Long Cycle</term></item>
    /// <item><term>int Signal Periods</term></item>
    /// </list>
    /// </remarks>
    public class RAVI : Indicator
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Indicator name</param>
        /// <param name="chartPanel">Reference to a panel where it will be placed</param>
        public RAVI(string name, ChartPanel chartPanel)
            : base(name, chartPanel)
        {
            _indicatorType = IndicatorType.RAVI;

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
            int paramInt2 = ParamInt(2);
            if (paramInt1 > 500) paramInt1 = 500;
            if (paramInt2 > 500) paramInt2 = 500;
            if (paramInt1 < 0) paramInt1 = 0;
            if (paramInt2 < 0) paramInt2 = 0;

            if (paramInt1 < 1 || paramInt1 > iSize / 2)
            {
                ProcessError("Invalid Short Cycle for indicator " + FullName, IndicatorErrorType.ShowErrorMessage);
                return false;
            }

            if (paramInt2 < 1 || paramInt2 > iSize / 2 || paramInt2 < paramInt1)
            {
                ProcessError("Invalid Long Cycle for indicator " + FullName, IndicatorErrorType.ShowErrorMessage);
                return false;
            }

            // Get the data
            Field pSource = SeriesToField("Source", ParamStr(0), iSize);
            if (!EnsureField(pSource, ParamStr(0))) return false;

            Navigator pNav = new Navigator();
            Recordset pRS = new Recordset();

            pRS.AddField(pSource);

            pNav.Recordset_ = pRS;

            // Calculate the indicator
            Index ta = new Index();
            Recordset pInd = ta.RAVI(pNav, pSource, paramInt1, paramInt2, FullName);


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
