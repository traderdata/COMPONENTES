
using System.Collections.Generic;
using ModulusFE.Indicators;

namespace ModulusFE
{
  public partial class ChartPanel
  {
    private void CalculateIndicators()
    {
      if (!IsHeatMap && _recalc)
      {
        lock (_series)
        {
          List<Indicator> toBeRemoved = new List<Indicator>();
          for (int i = 0; i < _series.Count; i++)
          {
            Indicator indicator = _series[i] as Indicator;
            if (indicator == null || indicator._indicatorType == IndicatorType.Unknown) continue;
            if (indicator._recycleFlag)
            {
              toBeRemoved.Add(indicator);
              continue;
            }
            _chartX._updatingIndicator = true;
            indicator.Calculate();
            if (!indicator._calculateResult)
            {
              toBeRemoved.Add(indicator);
            }
          }
          foreach (Indicator indicator in toBeRemoved)
          {
            _chartX.FireSeriesRemoved(indicator.FullName);
            RemoveSeries(indicator, true);
          }
        }
      }
    }
  }
}