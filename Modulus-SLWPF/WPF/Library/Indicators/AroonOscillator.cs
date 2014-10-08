using System.Collections.Generic;
using ModulusFE.Indicators;
using ModulusFE.Tasdk;

namespace ModulusFE
{
    public static partial class StockChartX_IndicatorsParameters
    {
        internal static void Register_AroonOscillator()
        {
            /*  Required inputs for this indicator:
              1. paramStr[0] = Symbol (eg "msft")
              2. paramInt[1] = Periods (eg 14)
            */
            RegisterIndicatorParameters(IndicatorType.AroonOscillator, typeof(AroonOscillator),
                                        "Aroon Oscillator",
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
                                          // Periods (eg 14)
                                          Name = Indicator.GetParamName(ParameterType.ptPeriods),
                                          ParameterType = ParameterType.ptPeriods,
                                          DefaultValue = 14,
                                          ValueType = typeof (int)
                                        }
                                    });
        }
    }
}

namespace ModulusFE.Indicators
{
    /// <summary>
    /// Aroon Oscillator
    /// </summary>
    /// <remarks>
    /// <list type="table">
    /// <listheader>
    /// <term>Parameters</term>
    /// </listheader>
    /// </list>
    /// </remarks>
    public class AroonOscillator : Indicator
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Indicator name</param>
        /// <param name="chartPanel">Reference to a panel where it will be placed</param>
        public AroonOscillator(string name, ChartPanel chartPanel)
            : base(name, chartPanel)
        {
            _indicatorType = IndicatorType.AroonOscillator;

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
            if (size == 0) return false;

            int paramInt1 = ParamInt(1);
            if (paramInt1 < 1 || paramInt1 > size / 2)
            {
                ProcessError("Invalid Periods for indicator " + FullName, IndicatorErrorType.ShowErrorMessage);
                return false;
            }

            // Get the data
            string paramStr0 = ParamStr(0);
            Field pOpen = SeriesToField("Open", paramStr0 + ".open", size);
            if (!EnsureField(pOpen, paramStr0 + ".open")) return false;
            Field pHigh = SeriesToField("High", paramStr0 + ".high", size);
            if (!EnsureField(pHigh, paramStr0 + ".high")) return false;
            Field pLow = SeriesToField("Low", paramStr0 + ".low", size);
            if (!EnsureField(pLow, paramStr0 + ".low")) return false;
            Field pClose = SeriesToField("Close", paramStr0 + ".close", size);
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
            Recordset pInd = ta.Aroon(pNav, pRS, paramInt1);

            // Output the indicator values
            Clear();

            for (int n = 0; n < size; ++n)
            {
                double? value = n < paramInt1 ? null : pInd.Value("Aroon Oscillator", n + 1);
                AppendValue(DM.GetTimeStampByIndex(n), value);
            }

            return _calculateResult = PostCalculate();
        }
    }
}

