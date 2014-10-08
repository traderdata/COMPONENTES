using System.Collections.Generic;
using System.Windows.Media;
using ModulusFE.Indicators;
using ModulusFE.Tasdk;

namespace ModulusFE
{
    public static partial class StockChartX_IndicatorsParameters
    {
        internal static void Register_SchaffTrendCycle()
        {
            RegisterIndicatorParameters(IndicatorType.SchaffTrendCycle, typeof(SchaffTrendCycle),
                                        "Schaff Trend Cycle",
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
                                          // Periods (eg 9)
                                          Name = Indicator.GetParamName(ParameterType.ptPeriods),
                                          ParameterType = ParameterType.ptSignalPeriods,
                                          DefaultValue = 14,
                                          ValueType = typeof (int)
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
                                          DefaultValue = 16,
                                          ValueType = typeof (int)
                                        },
                                    });
        }
    }
}

namespace ModulusFE.Indicators
{
    /// <summary>
    /// The Schaff Trend Cycle, by Doug Schaff, combines both Slow Stochastics and the Moving Average Convergence/Divergence (MACD).
    /// </summary>
    /// <remarks>Schaff Trend Cycle is interpreted similar to MACD. Buy and sell signals are generated whenever the indicator crosses a signal line, the zero mark line or when the indicator diverges from price.
    /// <list type="table">
    /// <listheader>
    /// <term>Parameters</term>
    /// </listheader>
    /// <item><term>str Symbol</term></item>
    /// <item><term>int Periods</term></item>
    /// <item><term>int Short Cycle</term></item>
    /// <item><term>int Long Cycle</term></item>
    /// </list>
    /// </remarks>
    public class SchaffTrendCycle : Indicator
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Indicator name</param>
        /// <param name="chartPanel">Reference to a panel where it will be placed</param>
        public SchaffTrendCycle(string name, ChartPanel chartPanel)
            : base(name, chartPanel)
        {
            _indicatorType = IndicatorType.SchaffTrendCycle;

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
            int paramInt3 = ParamInt(3);
            if (paramInt1 > 500) paramInt1 = 500;
            if (paramInt2 > 500) paramInt2 = 500;
            if (paramInt1 < 0) paramInt1 = 0;
            if (paramInt2 < 0) paramInt2 = 0;
            if (paramInt3 > 500) paramInt3 = 500;
            if (paramInt3 < 0) paramInt3 = 0;

            if (paramInt1 < 1 || paramInt1 > iSize / 2)
            {
                ProcessError("Invalid Periods for indicator " + FullName, IndicatorErrorType.ShowErrorMessage);
                return false;
            }

            if (paramInt2 < 1 || paramInt2 > iSize / 2)
            {
                ProcessError("Invalid Short Cycle for indicator " + FullName, IndicatorErrorType.ShowErrorMessage);
                return false;
            }

            if (paramInt3 < 1 || paramInt3 > iSize / 2 || paramInt3 < paramInt2)
            {
                ProcessError("Invalid Long Cycle for indicator " + FullName, IndicatorErrorType.ShowErrorMessage);
                return false;
            }


            // Get the data
            string paramStr0 = ParamStr(0);
            Field pOpen = SeriesToField("Open", paramStr0 + ".open", iSize);
            if (!EnsureField(pOpen, paramStr0 + ".open")) return false;
            Field pHigh = SeriesToField("High", paramStr0 + ".high", iSize);
            if (!EnsureField(pHigh, paramStr0 + ".high")) return false;
            Field pLow = SeriesToField("Low", paramStr0 + ".low", iSize);
            if (!EnsureField(pLow, paramStr0 + ".low")) return false;
            Field pClose = SeriesToField("Close", paramStr0 + ".close", iSize);
            if (!EnsureField(pClose, paramStr0 + ".close")) return false;
            Field pVolume = SeriesToField("Volume", paramStr0 + ".volume", iSize);
            if (!EnsureField(pVolume, paramStr0 + ".volume")) return false;

            Navigator pNav = new Navigator();
            Recordset pRS = new Recordset();

            pRS.AddField(pOpen);
            pRS.AddField(pHigh);
            pRS.AddField(pLow);
            pRS.AddField(pClose);
            pRS.AddField(pVolume);

            pNav.Recordset_ = pRS;


            // Calculate the indicator
            Oscillator ta = new Oscillator();
            Recordset pInd = ta.SchaffTrendCycle(pNav, pClose, paramInt1, paramInt2, paramInt3, FullName);


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
