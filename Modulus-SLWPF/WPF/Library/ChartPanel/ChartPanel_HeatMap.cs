using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
#if WPF
using System.Windows;
#endif
using ModulusFE.PaintObjects;
#if SILVERLIGHT
using ModulusFE.SL;
#endif

namespace ModulusFE
{
    internal class ChartPanel_HeatMap : ChartPanel
    {
        #region Predefined colors constansts for heat map

        private readonly SolidColorBrush[] _brushes =
          new[]
        {
          new SolidColorBrush(Color.FromArgb(0xFF, 255, 0, 0)), 
          new SolidColorBrush(Color.FromArgb(0xFF, 255, 11, 11)),
          new SolidColorBrush(Color.FromArgb(0xFF, 255, 22, 22)),
          new SolidColorBrush(Color.FromArgb(0xFF, 255, 33, 33)),
          new SolidColorBrush(Color.FromArgb(0xFF, 255, 44, 44)),
          new SolidColorBrush(Color.FromArgb(0xFF, 255, 55, 55)),
          new SolidColorBrush(Color.FromArgb(0xFF, 255, 66, 66)),
          new SolidColorBrush(Color.FromArgb(0xFF, 255, 77, 77)),
          new SolidColorBrush(Color.FromArgb(0xFF, 255, 88, 88)),
          new SolidColorBrush(Color.FromArgb(0xFF, 255, 99, 99)),
          new SolidColorBrush(Color.FromArgb(0xFF, 255, 110, 110)),
          new SolidColorBrush(Color.FromArgb(0xFF, 255, 121, 121)),
          new SolidColorBrush(Color.FromArgb(0xFF, 255, 132, 132)),
          new SolidColorBrush(Color.FromArgb(0xFF, 255, 143, 143)),
          new SolidColorBrush(Color.FromArgb(0xFF, 255, 154, 154)),
          new SolidColorBrush(Color.FromArgb(0xFF, 255, 165, 165)),
          new SolidColorBrush(Color.FromArgb(0xFF, 255, 176, 176)),
          new SolidColorBrush(Color.FromArgb(0xFF, 255, 187, 187)),
          new SolidColorBrush(Color.FromArgb(0xFF, 255, 198, 198)),
          new SolidColorBrush(Color.FromArgb(0xFF, 255, 209, 209)),
          new SolidColorBrush(Color.FromArgb(0xFF, 255, 220, 220)),
          new SolidColorBrush(Color.FromArgb(0xFF, 255, 231, 231)),
          new SolidColorBrush(Color.FromArgb(0xFF, 255, 242, 242)),
          new SolidColorBrush(Color.FromArgb(0xFF, 255, 255, 255)),
          new SolidColorBrush(Color.FromArgb(0xFF, 253, 255, 253)),
          new SolidColorBrush(Color.FromArgb(0xFF, 242, 255, 242)),
          new SolidColorBrush(Color.FromArgb(0xFF, 231, 255, 231)),
          new SolidColorBrush(Color.FromArgb(0xFF, 220, 255, 220)),
          new SolidColorBrush(Color.FromArgb(0xFF, 209, 255, 209)),
          new SolidColorBrush(Color.FromArgb(0xFF, 198, 255, 198)),
          new SolidColorBrush(Color.FromArgb(0xFF, 187, 255, 187)),
          new SolidColorBrush(Color.FromArgb(0xFF, 176, 255, 176)),
          new SolidColorBrush(Color.FromArgb(0xFF, 165, 255, 165)),
          new SolidColorBrush(Color.FromArgb(0xFF, 154, 255, 154)),
          new SolidColorBrush(Color.FromArgb(0xFF, 143, 255, 143)),
          new SolidColorBrush(Color.FromArgb(0xFF, 132, 255, 132)),
          new SolidColorBrush(Color.FromArgb(0xFF, 121, 255, 121)),
          new SolidColorBrush(Color.FromArgb(0xFF, 110, 255, 110)),
          new SolidColorBrush(Color.FromArgb(0xFF, 99, 255, 99)),
          new SolidColorBrush(Color.FromArgb(0xFF, 88, 255, 88)),
          new SolidColorBrush(Color.FromArgb(0xFF, 77, 255, 77)),
          new SolidColorBrush(Color.FromArgb(0xFF, 66, 255, 66)),
          new SolidColorBrush(Color.FromArgb(0xFF, 55, 255, 55)),
          new SolidColorBrush(Color.FromArgb(0xFF, 44, 255, 44)),
          new SolidColorBrush(Color.FromArgb(0xFF, 33, 255, 33)),
          new SolidColorBrush(Color.FromArgb(0xFF, 22, 255, 22)),
          new SolidColorBrush(Color.FromArgb(0xFF, 11, 255, 11)),
          new SolidColorBrush(Color.FromArgb(0xFF, 0, 255, 0)),
        };
        #endregion

        private readonly PaintObjectsManager<Rectangle> _rects = new PaintObjectsManager<Rectangle>();
        private readonly PaintObjectsManager<Label> _labels = new PaintObjectsManager<Label>();

        internal ChartPanel_HeatMap(StockChartX chartX)
        {
            _chartX = chartX;

            _isHeatMap = true;
#if WPF
      ResourceDictionary resourceDictionary = new ResourceDictionary
                                                {
                                                  Source =
                                                    new Uri("/ModulusFE.StockChartX;component/Themes/Brushes.xaml",
                                                            UriKind.RelativeOrAbsolute)
                                                };

      Background = (Brush)resourceDictionary["HatchBrush"];
#else
            Background = Brushes.White;
#endif

        }

        internal override void Paint()
        {
            //Debug.WriteLine("ChartPanel heat map paint");
            if (!_templateLoaded || _painting)
            {
                _timers.StopTimerWork(TimerSizeChanged);
                return;
            }
            _rects.C = _rootCanvas;
            _rects.Start();
            _labels.C = _rootCanvas;
            _labels.Start();

            List<Series> indicators = _chartX.IndicatorsCollection.Cast<Series>().ToList();
            var seriesForHeatMap = _chartX.SeriesCollection.Where(s => s.ShowInHeatMap);
            indicators.AddRange(seriesForHeatMap);

            //IEnumerable<Indicator> indicators = _chartX.IndicatorsCollection;
            if (indicators.Count() == 0)
            {
                _timers.StopTimerWork(TimerSizeChanged);
                _rects.Stop();
                _labels.Stop();
                return;
            }
            _painting = true;

            double panelHeight = _rootCanvas.ActualHeight / indicators.Count;
            double rectWidth = _chartX.PaintableWidth / (_chartX._endIndex - _chartX._startIndex);

            int indicatorIndex = 0;
            foreach (Series indicator in indicators)
            {
                double min;
                double max;

                indicator.DM.VisibleMinMax(indicator.SeriesIndex, out min, out max);
                if (indicator.RecordCount == 0) continue;

                int cnt = 0;
                for (int i = _chartX._startIndex; i < _chartX._endIndex; i++, cnt++)
                {
                    double? value = indicator[i].Value;

                    if (!value.HasValue || min == max) continue;

                    Brush brush = _brushes[(int)((_brushes.Length - 1) * (value.Value - min) / (max - min))];

                    double x = cnt * rectWidth + _chartX.LeftChartSpace;
                    double y = indicatorIndex * panelHeight;
                    Rectangle rectangle = Utils.DrawRectangle(x, y, x + rectWidth + 1, y + panelHeight, brush, _rects);
                    rectangle._rectangle.Stroke = brush;
                }
                Label lbl = Utils.DrawText(_chartX.LeftChartSpace + 10, indicatorIndex * panelHeight + 2,
                                           indicator.Title, _chartX.HeatPanelLabelsForeground,
                                           _chartX.HeatPanelLabelsFontSize, _chartX.FontFamily, _labels);
#if WPF
        lbl._textBlock.Background = _chartX.HeatPanelLabelsBackground;
#endif

                indicatorIndex++;
            }
            _rects.Stop();
            _labels.Stop();

            _rects.Do(r => r.ZIndex = ZIndexConstants.DarvasBoxes1);
            _labels.Do(l => l.ZIndex = ZIndexConstants.DarvasBoxes2);

            _painting = false;
            _timers.StopTimerWork(TimerSizeChanged);
        }

        internal override void PaintXGrid()
        { }
    }
}
