using System.Collections.Generic;
using ModulusFE.Indicators;
using ModulusFE.PaintObjects;
using ModulusFE.Tasdk;

namespace ModulusFE
{
    public static partial class StockChartX_IndicatorsParameters
    {
        internal static void Register_MovingAverageEnvelope()
        {
            /*  Required inputs for this indicator:
              1. paramStr0 = Source (eg "msft.close")
              2. paramInt1 = Periods (eg 14)
              3. paramInt2 = Moving Average Type (eg indSimpleMovingAverage)
              4. paramDbl[3] = Shift (eg 5%)
            */
            RegisterIndicatorParameters(IndicatorType.MovingAverageEnvelope, typeof(MovingAverageEnvelope),
                                        "Moving Average Envelope",
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
                                          // Periods (eg 14)
                                          Name = Indicator.GetParamName(ParameterType.ptPeriods),
                                          ParameterType = ParameterType.ptPeriods,
                                          DefaultValue = 14,
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
                                      new IndicatorParameter
                                        {
                                          // Shift (eg 14)
                                          Name = Indicator.GetParamName(ParameterType.ptShift),
                                          ParameterType = ParameterType.ptShift,
                                          DefaultValue = 5,
                                          ValueType = typeof (double)
                                        },
                                    });
        }
    }
}

namespace ModulusFE.Indicators
{
    /// <summary>
    /// Moving Average Envelopes consist of moving averages calculated from the underling price, shifted up and down by a fixed percentage.
    /// </summary>
    /// <remarks>Moving Average Envelopes (or trading bands) can be imposed over an actual price or another indicator.
    ///When prices rise above the upper band or fall below the lower band, a change in direction may occur when the price penetrates the band after a small reversal from the opposite direction.
    ///
    /// Shift is a double value specifying the percentage of shift for each moving average from the actual values.
    /// <list type="table">
    /// <listheader>
    /// <term>Parameters</term>
    /// </listheader>
    /// <item><term>str Source</term></item>
    /// <item><term>int Periods</term></item>
    /// <item><term>int Moving Average Type</term></item>
    /// <item><term>dbl Shift</term></item>
    /// </list>
    /// </remarks>
    public class MovingAverageEnvelope : Indicator
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Indicator name</param>
        /// <param name="chartPanel">Reference to a panel where it will be placed</param>
        public MovingAverageEnvelope(string name, ChartPanel chartPanel)
            : base(name, chartPanel)
        {
            _indicatorType = IndicatorType.MovingAverageEnvelope;

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
                ProcessError("Invalid Periods for indicator " + FullName, IndicatorErrorType.ShowErrorMessage);
                return false;
            }
            IndicatorType param2 = (IndicatorType)ParamInt(2);
            if (param2 < Constants.MA_START || param2 > Constants.MA_END)
            {
                ProcessError("Invalid Moving Average Type for indicator " + FullName, IndicatorErrorType.ShowErrorMessage);
                return false;
            }
            double paramDbl3 = ParamDbl(3);
            if (paramDbl3 < 0 || paramDbl3 > 100)
            {
                ProcessError("Invalid Band Shift Percentage for indicator " + FullName, IndicatorErrorType.ShowErrorMessage);
                return false;
            }


            // Get the data  
            string paramStr0 = ParamStr(0);
            Field pSource = SeriesToField("Source", paramStr0, iSize);
            if (!EnsureField(pSource, paramStr0)) return false;

            Navigator pNav = new Navigator();
            Recordset pRS = new Recordset();

            pRS.AddField(pSource);

            pNav.Recordset_ = pRS;


            // Calculate the indicator
            Bands ta = new Bands();
            Recordset pInd = ta.MovingAverageEnvelope(pNav, pSource, paramInt1, param2, paramDbl3);


            // Output the indicator values
            Clear();

            TwinIndicator sTop = (TwinIndicator)EnsureSeries(FullName + " Top");
            sTop.SetStrokeColor(StrokeColor.Invert(), false);
            sTop.SetStrokeThickness(StrokeThickness, false);

            _title = FullName + " Bottom";

            for (int n = 0; n < iSize; ++n)
            {
                double? top;
                double? bottom;
                if (n < paramInt1 * 2)
                {
                    top = null;
                    bottom = null;
                }
                else
                {
                    top = pInd.Value("Envelope Top", n + 1);
                    bottom = pInd.Value("Envelope Bottom", n + 1);
                }
                AppendValue(DM.TS(n), bottom);
                sTop.AppendValue(DM.TS(n), top);
            }

            return _calculateResult = PostCalculate();
        }
    }
}
