using System.Collections.Generic;
using ModulusFE.Indicators;
using ModulusFE.Tasdk;

namespace ModulusFE
{
    public static partial class StockChartX_IndicatorsParameters
    {
        internal static void Register_EaseOfMovement()
        {
            /*  Required inputs for this indicator:
              1. paramStr0 = Symbol (eg "msft")
              2. paramStr[1] = Volume (eg "msft.volume")
              3. paramInt2 = Periods (eg 14)
              4. param3 = Moving Average Type (eg indSimpleMovingAverage)
            */
            RegisterIndicatorParameters(IndicatorType.EaseOfMovement, typeof(EaseOfMovement),
                                        "Ease Of Movement",
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
                                          // Volume (eg "msft.volume")
                                          Name = Indicator.GetParamName(ParameterType.ptVolume),
                                          ParameterType = ParameterType.ptVolume,
                                          DefaultValue = "",
                                          ValueType = typeof (string)
                                        },
                                      new IndicatorParameter
                                        {
                                          // Periods (eg 14)
                                          Name = Indicator.GetParamName(ParameterType.ptPeriods),
                                          ParameterType = ParameterType.ptPeriods,
                                          DefaultValue = 8,
                                          ValueType = typeof (int)
                                        },
                                      new IndicatorParameter
                                        {
                                          // Moving Average Type (eg indSimpleMovingAverage)
                                          Name = Indicator.GetParamName(ParameterType.ptMAType),
                                          ParameterType = ParameterType.ptMAType,
                                          DefaultValue = IndicatorType.SimpleMovingAverage,
                                          ValueType = typeof (IndicatorType)
                                        },
                                    });
        }
    }
}

namespace ModulusFE.Indicators
{
    /// <summary>
    /// The Ease of Movement oscillator shows a unique relationship between price change and volume.
    /// </summary>
    /// <remarks>The Ease of Movement oscillator rises when prices are trending upwards under low volume, and likewise, the Ease of Movement oscillator falls when prices are trending downwards under low volume.
    /// <list type="table">
    /// <listheader>
    /// <term>Parameters</term>
    /// </listheader>
    /// <item><term>str Symbol</term></item>
    /// <item><term>str Volume</term></item>
    /// <item><term>int Periods</term></item>
    /// <item><term>int Moving Average Type</term></item>
    /// </list>
    /// </remarks>
    public class EaseOfMovement : Indicator
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Indicator name</param>
        /// <param name="chartPanel">Reference to a panel where it will be placed</param>
        public EaseOfMovement(string name, ChartPanel chartPanel)
            : base(name, chartPanel)
        {
            _indicatorType = IndicatorType.EaseOfMovement;

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

            int paramInt2 = ParamInt(2);
            if (paramInt2 < 1 || paramInt2 > size / 2)
            {
                ProcessError("Invalid Periods for indicator " + FullName, IndicatorErrorType.ShowErrorMessage);
                return false;
            }
            IndicatorType param3 = (IndicatorType)ParamInt(3);
            if (param3 < Constants.MA_START || param3 > Constants.MA_END)
            {
                ProcessError("Invalid Moving Average Type for indicator " + FullName, IndicatorErrorType.ShowErrorMessage);
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
            Field pVolume = SeriesToField("Volume", paramStr0 + ".close", size);
            if (!EnsureField(pVolume, ParamStr(1))) return false;

            Navigator pNav = new Navigator();
            Recordset pRS = new Recordset();

            pRS.AddField(pHigh);
            pRS.AddField(pLow);
            pRS.AddField(pClose);
            pRS.AddField(pVolume);

            pNav.Recordset_ = pRS;


            // Calculate the indicator
            Oscillator ta = new Oscillator();
            Recordset pInd = ta.EaseOfMovement(pNav, pRS, paramInt2, param3, FullName);


            // Output the indicator values
            Clear();
            for (int n = 0; n < size; ++n)
            {
                AppendValue(DM.GetTimeStampByIndex(n), n < paramInt2 ? null : pInd.Value(FullName, n + 1));
            }

            return _calculateResult = PostCalculate();
        }
    }
}

