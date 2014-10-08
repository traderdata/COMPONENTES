using System.Collections.Generic;
using ModulusFE.Indicators;
using ModulusFE.Tasdk;

namespace ModulusFE
{
    public static partial class StockChartX_IndicatorsParameters
    {
        internal static void Register_MACDHistogram()
        {
            /*  Required inputs for this indicator:
  
              1. paramStr0 = Symbol (eg "msft")
              2. paramInt1 = Long Cycle (eg 26)
              ...

            */
            RegisterIndicatorParameters(IndicatorType.MACDHistogram, typeof(MACDHistogram),
                                        "MACD Histogram",
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
                                          // Long cycle (eg 26)
                                          Name = Indicator.GetParamName(ParameterType.ptLongCycle),
                                          ParameterType = ParameterType.ptLongCycle,
                                          DefaultValue = 26,
                                          ValueType = typeof (int)
                                        },
                                      new IndicatorParameter
                                        {
                                          // Short Cycle (eg 13)
                                          Name = Indicator.GetParamName(ParameterType.ptShortCycle),
                                          ParameterType = ParameterType.ptShortCycle,
                                          DefaultValue = 13,
                                          ValueType = typeof (int)
                                        },
                                      new IndicatorParameter
                                        {
                                          // Singal Periods (eg 9)
                                          Name = Indicator.GetParamName(ParameterType.ptSignalPeriods),
                                          ParameterType = ParameterType.ptSignalPeriods,
                                          DefaultValue = 9,
                                          ValueType = typeof (int)
                                        },
                                    });
        }
    }
}

namespace ModulusFE.Indicators
{
    /// <summary>
    /// The MACD is a moving average oscillator that shows potential overbought/oversold phases of market fluctuation. The MACD is a calculation of two moving averages of the underlying price/indicator. The histogram is simply a bar graph of the MACD minus the MACD Signal line.
    /// </summary>
    /// <remarks>Buy/Sell interpretations may be derived from crossovers (calculated from the Signal Periods parameter), overbought/oversold levels of the MACD and divergences between MACD and actual price.
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
    public class MACDHistogram : Indicator
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Indicator name</param>
        /// <param name="chartPanel">Reference to a panel where it will be placed</param>
        public MACDHistogram(string name, ChartPanel chartPanel)
            : base(name, chartPanel)
        {
            _indicatorType = IndicatorType.MACDHistogram;

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
                ProcessError("Invalid Signal Periods for indicator " + FullName, IndicatorErrorType.ShowErrorMessage);
                return false;
            }

            if (paramInt2 == paramInt3)
            {
                ProcessError("Signal Periods cannot be the same as Short Cycle", IndicatorErrorType.ShowErrorMessage);
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

            Navigator pNav = new Navigator();
            Recordset pRS = new Recordset();

            pRS.AddField(pOpen);
            pRS.AddField(pHigh);
            pRS.AddField(pLow);
            pRS.AddField(pClose);

            pNav.Recordset_ = pRS;


            // Calculate the indicator
            Oscillator ta = new Oscillator();
            Recordset pInd = ta.MACDHistogram(pNav, pRS, paramInt3, paramInt1, paramInt2, FullName);

            // Output the indicator values
            Clear();
            for (int n = 0; n < iSize; ++n)
            {
                AppendValue(DM.TS(n), n < (paramInt1 + paramInt2) + 1 ? null : pInd.Value(FullName, n + 1));
            }

            return _calculateResult = PostCalculate();
        }
    }
}

