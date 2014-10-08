using System.Collections.Generic;
using System.Windows.Media;
using ModulusFE.Indicators;
using ModulusFE.Tasdk;

namespace ModulusFE
{
    public static partial class StockChartX_IndicatorsParameters
    {
        internal static void Register_DirectionalMovementSystem()
        {
            /*  Required inputs for this indicator:
              1. paramStr0 = Symbol (eg "msft")
              2. paramInt1 = Periods (eg 14)
            */
            RegisterIndicatorParameters(IndicatorType.DirectionalMovementSystem, typeof(DirectionalMovementSystem),
                                        "Directional Movement System",
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
                                    });
        }
    }
}

namespace ModulusFE.Indicators
{
    /// <summary>
    /// The Welles Wilder's Directional Movement System contains five indicators; ADX, DI+, DI-, DX, and ADXR.
    ///
    ///The ADX (Average Directional Movement Index) is an indicator of how much the market is trending, either up or down: the higher the ADX line, the more the market is trending and the more suitable it becomes for a trend-following system. This indicator consists of two lines: DI+ and DI-, the first one being a measure of uptrend and the second one a measure of downtrend.
    ///
    ///Detailed information about this indicator and formulas can be found in Welles Wilder's book, "New Concepts in Technical Trading Systems".
    ///The standard Directional Movement System draws a 14 period DI+ and a 14 period DI- in the same chart panel. ADX is also sometimes shown in the same chart panel.
    /// </summary>
    /// <remarks>A buy signal is given when DI+ crosses over DI-, a sell signal is given when DI- crosses over DI+.
    /// <list type="table">
    /// <listheader>
    /// <term>Parameters</term>
    /// </listheader>
    /// <item><term>str Symbol</term></item>
    /// <item><term>int Periods</term></item>
    /// </list>
    /// </remarks>
    public class DirectionalMovementSystem : Indicator
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Indicator name</param>
        /// <param name="chartPanel">Reference to a panel where it will be placed</param>
        public DirectionalMovementSystem(string name, ChartPanel chartPanel)
            : base(name, chartPanel)
        {
            _indicatorType = IndicatorType.DirectionalMovementSystem;

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
            Recordset pInd = ta.DirectionalMovementSystem(pNav, pRS, paramInt1);


            // Output the indicator values
            Clear();

            TwinIndicator sDIPlus = (TwinIndicator)EnsureSeries(FullName + " DI+");
            TwinIndicator sDIMinus = (TwinIndicator)EnsureSeries(FullName + " DI-");

            sDIPlus.SetStrokeColor(Colors.Red, false);
            sDIMinus.SetStrokeColor(Colors.Blue, false);

            _title = "ADX";


            for (int n = 0; n < size; ++n)
            {
                double? adx;
                double? din;
                double? dip;
                if (n < paramInt1)
                {
                    adx = null;
                    dip = null;
                    din = null;
                }
                else
                {
                    adx = pInd.Value("ADX", n + 1);
                    dip = pInd.Value("DI+", n + 1);
                    din = pInd.Value("DI-", n + 1);
                }
                AppendValue(DM.GetTimeStampByIndex(n), adx);
                sDIPlus.AppendValue(DM.GetTimeStampByIndex(n), dip);
                sDIMinus.AppendValue(DM.GetTimeStampByIndex(n), din);
            }

            return _calculateResult = PostCalculate();
        }
    }
}
