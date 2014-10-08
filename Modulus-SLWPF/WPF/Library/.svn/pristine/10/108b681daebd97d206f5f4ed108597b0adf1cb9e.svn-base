using System.Collections.Generic;
using ModulusFE.Indicators;
using ModulusFE.Tasdk;
using System.Windows.Media;

namespace ModulusFE
{
    public static partial class StockChartX_IndicatorsParameters
    {
        internal static void Register_StollerAverageRangeChannels()
        {
            /*  Required inputs for this indicator:
  
              1. paramStr0 = Symbol (eg "msft")
              ...

            */
            RegisterIndicatorParameters(IndicatorType.StollerAverageRangeChannels, typeof(KeltnerChannel),
                                        "Stoller Average Range Channels",
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

