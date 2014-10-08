using System.Collections.Generic;
using ModulusFE.Indicators;
using ModulusFE.Tasdk;

namespace ModulusFE
{
    public static partial class StockChartX_IndicatorsParameters
    {
        internal static void Register_TradeVolumeIndex()
        {
            /*  Required inputs for this indicator:
              1. paramStr0 = Source (eg "msft.close")
              2. paramStr1 = Volume (eg "msft.volume")
              3. paramDbl2 = Minimum Tick Value (eg 0.25)
            */
            RegisterIndicatorParameters(IndicatorType.TradeVolumeIndex, typeof(TradeVolumeIndex),
                                        "Trade Volume Index",
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
                                          // Volume (eg "msft.volume")
                                          Name = Indicator.GetParamName(ParameterType.ptVolume),
                                          ParameterType = ParameterType.ptVolume,
                                          DefaultValue = "",
                                          ValueType = typeof (string)
                                        },
                                      new IndicatorParameter
                                        {
                                          // Minimum Tick Value (eg 0.25)
                                          Name = Indicator.GetParamName(ParameterType.ptMinTickVal),
                                          ParameterType = ParameterType.ptMinTickVal,
                                          DefaultValue = 0.25,
                                          ValueType = typeof (double)
                                        },
                                    });
        }
    }
}

namespace ModulusFE.Indicators
{
    /// <summary>
    /// The Trade Volume index shows whether a security is being accumulated or distribute (similar to the Accumulation/Distribution index).
    /// </summary>
    /// <remarks>When the indicator is rising, the security is said to be accumulating. Conversely, when the indicator is falling, the security is said to being distributing. Prices may reverse when the indicator converges with price.
    /// <list type="table">
    /// <listheader>
    /// <term>Parameters</term>
    /// </listheader>
    /// <item><term>str Source</term></item>
    /// <item><term>str Volume</term></item>
    /// <item><term>dbl Minimum Tick Value</term></item>
    /// </list>
    /// </remarks>
    public class TradeVolumeIndex : Indicator
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Indicator name</param>
        /// <param name="chartPanel">Reference to a panel where it will be placed</param>
        public TradeVolumeIndex(string name, ChartPanel chartPanel)
            : base(name, chartPanel)
        {
            _indicatorType = IndicatorType.TradeVolumeIndex;

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

            double paramDbl2 = ParamDbl(2);
            if (paramDbl2 <= 0.01 || paramDbl2 >= 0.9)
            {
                ProcessError("Invalid Minimum Tick Value (0.01 to 0.9) for indicator " + FullName, IndicatorErrorType.ShowErrorMessage);
                return false;
            }


            // Get the data
            string paramStr0 = ParamStr(0);
            Field pSource = SeriesToField("Source", paramStr0, iSize);
            if (!EnsureField(pSource, paramStr0)) return false;
            string paramStr1 = ParamStr(1);
            Field pVolume = SeriesToField("Volume", paramStr1, iSize);
            if (!EnsureField(pVolume, paramStr0)) return false;

            Navigator pNav = new Navigator();
            Recordset pRS = new Recordset();

            pRS.AddField(pSource);
            pRS.AddField(pVolume);

            pNav.Recordset_ = pRS;


            // Calculate the indicator
            Index ta = new Index();
            Recordset pInd = ta.TradeVolumeIndex(pNav, pSource, pVolume, paramDbl2, FullName);

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

