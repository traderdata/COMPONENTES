using System.Collections.Generic;
using ModulusFE.Indicators;
using ModulusFE.Tasdk;

namespace ModulusFE
{
    public static partial class StockChartX_IndicatorsParameters
    {
        internal static void Register_CoppockCurve()
        {
            /*  Required inputs for this indicator:
  
              1. paramStr0 = Symbol (eg "msft")
              ...

            */
            RegisterIndicatorParameters(IndicatorType.CoppockCurve, typeof(CoppockCurve),
                                        "Coppock Curve",
                                        new List<IndicatorParameter>
                                    {
                                      new IndicatorParameter
                                        {
                                          // Symbol (eg "msft")
                                          Name = Indicator.GetParamName(ParameterType.ptSource),
                                          ParameterType = ParameterType.ptSource,
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
    /// The Coppock Curve, developed by Edwin Coppock and published in Barron's Magazine in 1962, is based on a 14-month and 11-month rate of change, smoothed by a 10-period weighted moving average.
    /// </summary>
    /// <remarks>The Coppock Curve generates buy signals when the value falls below zero and turns upwards from a low point.
    /// <list type="table">
    /// <listheader>
    /// <term>Parameters</term>
    /// </listheader>
    /// <item><term>str Symbol</term></item>
    /// <item><term>int Periods</term></item>
    /// </list>
    /// </remarks>
    public class CoppockCurve : Indicator
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Indicator name</param>
        /// <param name="chartPanel">Reference to a panel where it will be placed</param>
        public CoppockCurve(string name, ChartPanel chartPanel)
            : base(name, chartPanel)
        {
            _indicatorType = IndicatorType.CoppockCurve;

            Init();
        }

        /// <summary>
        /// Action to be executed for calculating indicator
        /// </summary>
        /// <returns>for future usage. Must be ignored at this time.</returns>
        protected override bool TrueAction()
        {
            // Validate
            int iSize = _chartPanel._chartX.RecordCount;
            if (iSize == 0)
                return false;

            // Get the data
            Field pSource = SeriesToField("Source", ParamStr(0), iSize);
            if (!EnsureField(pSource, ParamStr(0))) return false;

            Navigator pNav = new Navigator();
            Recordset pRS = new Recordset();

            pRS.AddField(pSource);

            pNav.Recordset_ = pRS;

            // Calculate the indicator
            Oscillator ta = new Oscillator();
            Recordset pInd = ta.CoppockCurve(pNav, pSource, FullName);

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

