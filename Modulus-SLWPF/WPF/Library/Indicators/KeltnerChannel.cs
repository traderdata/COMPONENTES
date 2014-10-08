using System.Collections.Generic;
using ModulusFE.Indicators;
using ModulusFE.Tasdk;
using System.Windows.Media;

namespace ModulusFE
{
    public static partial class StockChartX_IndicatorsParameters
    {
        internal static void Register_KeltnerChannel()
        {
            /*  Required inputs for this indicator:
  
              1. paramStr0 = Symbol (eg "msft")
              ...

            */
            RegisterIndicatorParameters(IndicatorType.KeltnerChannel, typeof(KeltnerChannel),
                                        "Keltner Channel",
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
                                        },
                                      new IndicatorParameter
                                        {
                                          // Shift (eg 14)
                                          Name = Indicator.GetParamName(ParameterType.ptShift),
                                          ParameterType = ParameterType.ptShift,
                                          DefaultValue = 1.1,
                                          ValueType = typeof (double)
                                        },
                                      new IndicatorParameter
                                        {
                                          // Moving Average Type (eg indSimpleMovingAverage)
                                          Name = Indicator.GetParamName(ParameterType.ptMAType),
                                          ParameterType = ParameterType.ptMAType,
                                          DefaultValue = IndicatorType.SimpleMovingAverage,
                                          ValueType = typeof (IndicatorType)
                                        }
                                    });
        }
    }
}

namespace ModulusFE.Indicators
{
    /// <summary>
    /// Keltner Channel is a volatility based moving average envelope that shifts a moving average of the True Range indicator by a certain percentage upwards and downwards.
    /// </summary>
    /// <remarks>Prices may reverse sharply after exiting and re-entering either the top or bottom band.
    /// <list type="table">
    /// <listheader>
    /// <term>Parameters</term>
    /// </listheader>
    /// <item><term>str Symbol</term></item>
    /// <item><term>int Periods</term></item>
    /// <item><term>dbl Shift</term></item>
    /// <item><term>int Moving Average Type</term></item>
    /// </list>
    /// </remarks>
    public class KeltnerChannel : Indicator
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Indicator name</param>
        /// <param name="chartPanel">Reference to a panel where it will be placed</param>
        public KeltnerChannel(string name, ChartPanel chartPanel)
            : base(name, chartPanel)
        {
            _indicatorType = IndicatorType.KeltnerChannel;

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

            if (ParamInt(1) < 1 || ParamInt(1) > iSize / 2)
            {
                ProcessError("Invalid Periods for indicator " + FullName, IndicatorErrorType.ShowErrorMessage);
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
            Bands ta = new Bands();
            Recordset pInd = ta.Keltner(pNav, pRS, ParamInt(1), ParamDbl(2), (IndicatorType)ParamInt(3), FullName);

            string sTopLine = FullName + " Top";
            string sBottomLine = FullName + " Bottom";

            this._title = sTopLine;

            TwinIndicator indBottomLine = (TwinIndicator)EnsureSeries(sBottomLine);
            indBottomLine.SetStrokeColor(Colors.Blue, false);
            indBottomLine.SetStrokeThickness(StrokeThickness, false);

            ForceLinearChart = true;
            indBottomLine.ForceLinearChart = true;

            // Output the indicator values
            Clear();
            for (int n = 0; n < iSize; ++n)
            {
                AppendValue(DM.TS(n), n < ParamInt(1) + 1 ? null : pInd.Value(sTopLine, n + 1));
                indBottomLine.AppendValue(DM.TS(n), n < ParamInt(1) + 1 ? null : pInd.Value(sBottomLine, n + 1));
            }

            return _calculateResult = PostCalculate();
        }
    }
}

