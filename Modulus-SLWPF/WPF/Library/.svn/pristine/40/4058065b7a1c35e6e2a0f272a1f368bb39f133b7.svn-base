using System.Collections.Generic;
using System.Windows.Media;
using ModulusFE.Indicators;
using ModulusFE.Tasdk;

namespace ModulusFE
{
    public static partial class StockChartX_IndicatorsParameters
    {
        internal static void Register_StochasticMomentumIndex()
        {
            /*  Required inputs for this indicator:
              1. paramStr0 = Symbol (eg "msft")
              2. paramInt1 = %KPeriods (eg 5)
              3. paramInt2 = %KSmooth (eg 3)
              4. paramInt3 = %ptPctKDblSmooth (eg 3)
              5. paramInt[4] = %DPeriods (eg 3)
              6. paramInt[5] = MAType (eg indSimpleMovingAverage)
              7. paramInt[6] = PctD_MAType (eg indSimpleMovingAverage)
            */
            RegisterIndicatorParameters(IndicatorType.StochasticMomentumIndex, typeof(StochasticMomentumIndex),
                                        "Stochastic Momentum Index",
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
                                          // %KPeriods (eg 5)
                                          Name = Indicator.GetParamName(ParameterType.ptPctKPeriods),
                                          ParameterType = ParameterType.ptPctKPeriods,
                                          DefaultValue = 13,
                                          ValueType = typeof (int)
                                        },
                                      new IndicatorParameter
                                        {
                                          // %KSmooth (eg 3)
                                          Name = Indicator.GetParamName(ParameterType.ptPctKSmooth),
                                          ParameterType = ParameterType.ptPctKSmooth,
                                          DefaultValue = 25,
                                          ValueType = typeof (int)
                                        },
                                      new IndicatorParameter
                                        {
                                          // %ptPctKDblSmooth (eg 3)
                                          Name = Indicator.GetParamName(ParameterType.ptPctKDblSmooth),
                                          ParameterType = ParameterType.ptPctKDblSmooth,
                                          DefaultValue = 2,
                                          ValueType = typeof (int)
                                        },
                                      new IndicatorParameter
                                        {  
                                          // %DPeriods (eg 3)
                                          Name = Indicator.GetParamName(ParameterType.ptPctDPeriods),
                                          ParameterType = ParameterType.ptPctDPeriods,
                                          DefaultValue = 9,
                                          ValueType = typeof (int)
                                        },
                                      new IndicatorParameter
                                        {
                                          // MAType (eg indSimpleMovingAverage)
                                          Name = Indicator.GetParamName(ParameterType.ptMAType),
                                          ParameterType = ParameterType.ptMAType,
                                          DefaultValue = IndicatorType.ExponentialMovingAverage,
                                          ValueType = typeof (IndicatorType)
                                        },
                                      new IndicatorParameter
                                        {
                                          // %DPeriods (eg 3)
                                          Name = Indicator.GetParamName(ParameterType.ptPctDMAType),
                                          ParameterType = ParameterType.ptPctDMAType,
                                          DefaultValue = IndicatorType.ExponentialMovingAverage,
                                          ValueType = typeof (IndicatorType)
                                        },
                                    });
        }
    }
}

namespace ModulusFE.Indicators
{
    /// <summary>
    /// The Stochastic Momentum Index, developed by William Blau, first appeared in the January 1993 issue of Stocks &amp; Commodities magazine. This indicator plots the closeness relative to the midpoint of the recent high/low range.
    /// </summary>
    /// <remarks>The Stochastic Momentum Index has two components: %K and %D. %K is most often displayed as a solid line and %D is often shown as a dotted line. 
    /// The most widely used method for interpreting the Stochastic Momentum Index is to buy when either component rises above 40 or sell 
    /// when either component falls below 40. Another way to interpret the Stochastic Momentum Index is to buy when %K rises above %D, 
    /// and conversely, sell when %K falls below %D.
    /// The most commonly used arguments are 13 for %K periods, 25 for %K smoothing, 2 for %K double smoothing, and 9 for %D periods.
    /// <list type="table">
    /// <listheader>
    /// <term>Parameters</term>
    /// </listheader>
    /// <item><term>str Symbol</term></item>
    /// <item><term>int %K Periods</term></item>
    /// <item><term>int %K Smoothing</term></item>
    /// <item><term>int %K Double Smoothing</term></item>
    /// <item><term>int %D Periods</term></item>
    /// <item><term>int Moving Average Type</term></item>
    /// <item><term>int %D Moving Average Type</term></item>
    /// </list>
    /// </remarks>
    public class StochasticMomentumIndex : Indicator
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Indicator name</param>
        /// <param name="chartPanel">Reference to a panel where it will be placed</param>
        public StochasticMomentumIndex(string name, ChartPanel chartPanel)
            : base(name, chartPanel)
        {
            _indicatorType = IndicatorType.StochasticMomentumIndex;

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

            int paramInt1 = ParamInt(1);
            if (paramInt1 < 1 || paramInt1 > iSize / 2)
            {
                ProcessError("Invalid %K Periods for indicator " + FullName, IndicatorErrorType.ShowErrorMessage);
                return false;
            }
            int paramInt2 = ParamInt(2);
            if (paramInt2 < 1 || paramInt2 > iSize / 2)
            {
                ProcessError("Invalid %K Smoothing Periods for indicator " + FullName, IndicatorErrorType.ShowErrorMessage);
                return false;
            }
            int paramInt3 = ParamInt(3);
            if (paramInt3 < 1)
            {
                ProcessError("Invalid %K Double Smoothing Periods for indicator " + FullName, IndicatorErrorType.ShowErrorMessage);
                return false;
            }
            int paramInt4 = ParamInt(4);
            if (paramInt4 < 1)
            {
                ProcessError("Invalid %D Periods for indicator " + FullName, IndicatorErrorType.ShowErrorMessage);
                return false;
            }

            // Get the data
            string paramStr0 = ParamStr(0);
            Field pHigh = SeriesToField("High", paramStr0 + ".high", iSize);
            if (!EnsureField(pHigh, paramStr0 + ".high")) return false;
            Field pLow = SeriesToField("Low", paramStr0 + ".low", iSize);
            if (!EnsureField(pLow, paramStr0 + ".low")) return false;
            Field pClose = SeriesToField("Close", paramStr0 + ".close", iSize);
            if (!EnsureField(pClose, paramStr0 + ".close")) return false;

            Navigator pNav = new Navigator();
            Recordset pRS = new Recordset();

            pRS.AddField(pHigh);
            pRS.AddField(pLow);
            pRS.AddField(pClose);

            pNav.Recordset_ = pRS;



            // Calculate the indicator
            Index ta = new Index();
            Recordset pInd = ta.StochasticMomentumIndex(pNav, pRS, paramInt1, paramInt2, paramInt3,
                                                        paramInt4, (IndicatorType)ParamInt(5),
                                                        (IndicatorType)ParamInt(6));

            // Output the indicator values
            Clear();

            _strokeThickness = 2;
            _strokePattern = LinePattern.Dot;

            TwinIndicator sPctK = (TwinIndicator)EnsureSeries(FullName + " %K");
            sPctK.SetStrokeColor(Colors.Blue, false);
            sPctK.SetStrokeThickness(_strokeThickness > 1 ? _strokeThickness - 1 : _strokeThickness, false);

            _title = FullName + " %D";

            for (int n = 0; n < iSize; ++n)
            {
                double? pctd = null;
                double? pctk = null;
                if (n >= paramInt1 * 2)
                {
                    pctd = pInd.Value("%D", n + 1);
                    pctk = pInd.Value("%K", n + 1);
                }

                AppendValue(DM.TS(n), pctd);
                sPctK.AppendValue(DM.TS(n), pctk);
            }
            return _calculateResult = PostCalculate();
        }
    }
}

