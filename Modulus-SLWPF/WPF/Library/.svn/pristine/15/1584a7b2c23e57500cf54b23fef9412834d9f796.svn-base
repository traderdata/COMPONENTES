using System.Collections.Generic;
using ModulusFE.Indicators;
using ModulusFE.Tasdk;

namespace ModulusFE
{
    public static partial class StockChartX_IndicatorsParameters
    {
        internal static void Register_OnBalanceVolume()
        {
            /*  Required inputs for this indicator:
              1. paramStr0 = Source (eg "msft.close")
              2. paramStr1 = Volume (eg "msft.volume")
            */
            RegisterIndicatorParameters(IndicatorType.OnBalanceVolume, typeof(OnBalanceVolume),
                                        "On Balance Volume",
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
                                    });
        }
    }
}

namespace ModulusFE.Indicators
{
    /// <summary>
    /// The On Balance Volume indicator shows a relationship of price and volume as a momentum index.
    /// </summary>
    /// <remarks>On Balance Volume index generally precedes actual price movements. The premise is that well-informed investors are buying when the index rises and uninformed investors are buying when the index falls.
    /// <list type="table">
    /// <listheader>
    /// <term>Parameters</term>
    /// </listheader>
    /// <item><term>str Source</term></item>
    /// <item><term>str Volume</term></item>
    /// </list>
    /// </remarks>
    public class OnBalanceVolume : Indicator
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Indicator name</param>
        /// <param name="chartPanel">Reference to a panel where it will be placed</param>
        public OnBalanceVolume(string name, ChartPanel chartPanel)
            : base(name, chartPanel)
        {
            _indicatorType = IndicatorType.OnBalanceVolume;

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

            string paramStr0 = ParamStr(0);
            string paramStr1 = ParamStr(1);
            if (paramStr0 == paramStr1)
            {
                ProcessError("Source 1 cannot be the same as Source 2\nSource 2 must be a volume series", IndicatorErrorType.ShowErrorMessage);
                return false;
            }

            // Get the data
            Field pSource = SeriesToField("Source", paramStr0, iSize);
            if (!EnsureField(pSource, paramStr0)) return false;

            Field pVolume = SeriesToField("Volume", paramStr1, iSize);
            if (!EnsureField(pVolume, paramStr1)) return false;

            Navigator pNav = new Navigator();
            Recordset pRS = new Recordset();

            pRS.AddField(pSource);
            pRS.AddField(pVolume);

            pNav.Recordset_ = pRS;


            // Calculate the indicator
            Index ta = new Index();
            Recordset pInd = ta.OnBalanceVolume(pNav, pSource, pVolume, FullName);


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

