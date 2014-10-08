using System.Collections.Generic;
using ModulusFE.Indicators;
using ModulusFE.Tasdk;

namespace ModulusFE
{
    public static partial class StockChartX_IndicatorsParameters
    {
        internal static void Register_ElderThermometer()
        {
            /*  Required inputs for this indicator:
  
              1. paramStr0 = Symbol (eg "msft")
              ...

            */
            RegisterIndicatorParameters(IndicatorType.ElderThermometer, typeof(ElderThermometer),
                                        "Elder Thermometer",
                                        new List<IndicatorParameter>
                                    {
                                      new IndicatorParameter
                                        {
                                          // Symbol (eg "msft")
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
    /// The Elder Force Index is calculated by the change in price from the previous to the current day, multiplied by volume.
    /// </summary>
    /// <remarks>Buy signals are generated when the two-day EMA of the Elder Force Index is negative and sell signals are generated when it is positive.
    /// <list type="table">
    /// <listheader>
    /// <term>Parameters</term>
    /// </listheader>
    /// <item><term>str Symbol</term></item>
    /// </list>
    /// </remarks>
    public class ElderThermometer : Indicator
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Indicator name</param>
        /// <param name="chartPanel">Reference to a panel where it will be placed</param>
        public ElderThermometer(string name, ChartPanel chartPanel)
            : base(name, chartPanel)
        {
            _indicatorType = IndicatorType.ElderThermometer;

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
            Index ta = new Index();
            Recordset pInd = ta.ElderThermometer(pRS, FullName);

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

