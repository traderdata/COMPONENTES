using System.Collections.Generic;
using ModulusFE.Indicators;
using ModulusFE.PaintObjects;
using ModulusFE.Tasdk;

namespace ModulusFE
{
    public static partial class StockChartX_IndicatorsParameters
    {
        internal static void Register_PrimeNumberBands()
        {
            /*  Required inputs for this indicator:
              1. paramStr0 = Symbol (eg "msft")
            */
            RegisterIndicatorParameters(IndicatorType.PrimeNumberBands, typeof(PrimeNumberBands),
                                        "Prime Number Bands",
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
                                    });
        }
    }
}

namespace ModulusFE.Indicators
{
    /// <summary>
    /// Similar to the Prime Numbers Oscillator, the prime numbers oscillator was developed by Modulus Financial Engineering, Inc. This indicator finds the nearest prime number for the high and low, and plots the two series as bands.
    /// </summary>
    /// <remarks>This indicator can be used to spot market trading ranges.
    /// <list type="table">
    /// <listheader>
    /// <term>Parameters</term>
    /// </listheader>
    /// <item><term>str Symbol</term></item>
    /// </list>
    /// </remarks>
    public class PrimeNumberBands : Indicator
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Indicator name</param>
        /// <param name="chartPanel">Reference to a panel where it will be placed</param>
        public PrimeNumberBands(string name, ChartPanel chartPanel)
            : base(name, chartPanel)
        {
            _indicatorType = IndicatorType.PrimeNumberBands;

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
            Field pHigh = SeriesToField("High", paramStr0 + ".high", iSize);
            if (!EnsureField(pHigh, paramStr0 + ".high")) return false;
            Field pLow = SeriesToField("Low", paramStr0 + ".low", iSize);
            if (!EnsureField(pLow, paramStr0 + ".low")) return false;

            Navigator pNav = new Navigator();
            Recordset pRS = new Recordset();

            pRS.AddField(pHigh);
            pRS.AddField(pLow);

            pNav.Recordset_ = pRS;


            // Calculate the indicator
            Bands ta = new Bands();
            Recordset pInd = ta.PrimeNumberBands(pNav, pHigh, pLow);

            // Output the indicator values
            Clear();

            TwinIndicator sBottom = (TwinIndicator)EnsureSeries(FullName + " Bottom");
            sBottom.SetStrokeColor(StrokeColor.Invert(), false);
            sBottom.SetStrokeThickness(StrokeThickness, false);

            _title = FullName + " Top";

            for (int n = 0; n < iSize; ++n)
            {
                AppendValue(DM.TS(n), pInd.Value("Prime Bands Top", n + 1));
                sBottom.AppendValue(DM.TS(n), pInd.Value("Prime Bands Bottom", n + 1));
            }
            return _calculateResult = PostCalculate();
        }
    }
}
