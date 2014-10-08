
using System.Collections.Generic;
using ModulusFE.Indicators;

namespace ModulusFE
{
    using System.Linq;

    public partial class ChartPanel
    {
        private void CalculateIndicators()
        {
            if (IsHeatMap || !_recalc)
            {
                return;
            }

            lock (_series)
            {
                List<Indicator> toBeRemoved = new List<Indicator>();
                List<Indicator> indicators = _series.Select(t => t as Indicator)
                  .Where(indicator => indicator != null && indicator._indicatorType != IndicatorType.Unknown)
                  .ToList();

                foreach (Indicator indicator in indicators)
                {
                    if (indicator._recycleFlag)
                    {
                        toBeRemoved.Add(indicator);
                        continue;
                    }

                    _chartX._updatingIndicator = true;
                    indicator.Calculate();
                    if (!indicator._calculateResult && !indicator._ignoreErrors)
                    {
                        toBeRemoved.Add(indicator);
                    }
                }

                foreach (Indicator indicator in toBeRemoved)
                {
                    RemoveSeries(indicator, true);
                }
            }
        }
    }
}