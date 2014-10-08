using System.Collections.Generic;
using ModulusFE.Indicators;
using ModulusFE.Tasdk;

namespace ModulusFE
{
    public static partial class StockChartX_IndicatorsParameters
    {
        internal static void Register_FractalChaosBands()
        {
            /*  Required inputs for this indicator:
              1. paramStr0 = Symbol (eg "msft")
              2. paramInt1 = Periods (eg 14)
            */
            RegisterIndicatorParameters(IndicatorType.FractalChaosBands, typeof(FractalChaosBands),
                                        "Fractal Chaos Bands",
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
    /// The chaotic nature of stock market movements explains why it is sometimes difficult to distinguish hourly charts from monthly charts if the time scale is not given. The patterns are similar regardless of the time resolution. Like the chambers of the nautilus, each level is like the one before it, but the size is different. To determine what is happening in the current level of resolution, the fractal chaos band indicator can be used to examine these patterns.
    /// </summary>
    /// <remarks>
    /// <list type="table">
    /// <listheader>
    /// <term>Parameters</term>
    /// </listheader>
    /// <item><term>str Symbol</term></item>
    /// <item><term>int periods</term></item>
    /// </list>
    /// </remarks>
    public class FractalChaosBands : Indicator
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Indicator name</param>
        /// <param name="chartPanel">Reference to a panel where it will be placed</param>
        public FractalChaosBands(string name, ChartPanel chartPanel)
            : base(name, chartPanel)
        {
            _indicatorType = IndicatorType.FractalChaosBands;

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
            if (paramInt1 < 1 || paramInt1 > size)
            {
                ProcessError("Invalid Periods for indicator " + FullName, IndicatorErrorType.ShowErrorMessage);
                return false;
            }

            // Get the data
            string paramStr0 = ParamStr(0);
            Field pHigh = SeriesToField("High", paramStr0 + ".high", size);
            if (!EnsureField(pHigh, paramStr0 + ".high")) return false;
            Field pLow = SeriesToField("Low", paramStr0 + ".low", size);
            if (!EnsureField(pLow, paramStr0 + ".low")) return false;

            Navigator pNav = new Navigator();
            Recordset pRS = new Recordset();

            pRS.AddField(pHigh);
            pRS.AddField(pLow);

            pNav.Recordset_ = pRS;


            // Calculate the indicator
            Bands ta = new Bands();
            Recordset pInd = ta.FractalChaosBands(pNav, pRS, paramInt1);


            // Output the indicator values
            Clear();

            TwinIndicator sHigh = (TwinIndicator)EnsureSeries(FullName + " High");
            sHigh.SetStrokeColor(StrokeColor, false);
            sHigh.SetStrokeThickness(StrokeThickness, false);

            _title = FullName + " Low";

            for (int n = 0; n < size; ++n)
            {
                double? low;
                double? high;
                if (n < paramInt1)
                {
                    high = null;
                    low = null;
                }
                else
                {
                    high = pInd.Value("Fractal High", n + 1);
                    low = pInd.Value("Fractal Low", n + 1);
                }
                AppendValue(DM.GetTimeStampByIndex(n), low);
                sHigh.AppendValue(DM.GetTimeStampByIndex(n), high);
            }

            return _calculateResult = PostCalculate();
        }
    }
}

