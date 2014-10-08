using System;
using ModulusFE.PaintObjects;
#if SILVERLIGHT
using ModulusFE.SL;
#endif
#if WPF
using System.Windows.Media;
#endif

namespace ModulusFE
{
    public partial class ChartPanel
    {
        private int _sideVolumeDepthBars;
        readonly PaintObjectsManager<Rectangle> _rectsSideVolumeDepth = new PaintObjectsManager<Rectangle>();
        /// <summary>
        /// Gets or sets the number of side volume depths bars. 
        /// </summary>
        public int SideVolumeDepthBars
        {
            get { return _sideVolumeDepthBars; }
            set
            {
                if (_sideVolumeDepthBars == value) return;
                _sideVolumeDepthBars = value;

                if (_rootCanvas == null) return;

                PaintSideVolumeDepthBars();
            }
        }

        private void PaintSideVolumeDepthBars()
        {
            if (_sideVolumeDepthBars == 0) //remove
            {
                _rectsSideVolumeDepth.RemoveAll();
                return;
            }
            if (_chartX._endIndex == 0) return;
            if (_series.Count < 3) return;
            Series open = null;
            foreach (Series series in _series)
            {
                if (series.OHLCType != SeriesTypeOHLC.Open) continue;
                open = series;
                break;
            }
            if (open == null) return;
            Series close = GetSeriesOHLCV(open, SeriesTypeOHLC.Close);
            if (close == null) return;
            Series volume = _chartX.GetSeriesOHLCV(open, SeriesTypeOHLC.Volume);
            if (volume == null) return;

            double maxVolume = double.MinValue, minVolume = double.MaxValue;
            int i;
            for (i = _chartX._startIndex; i < _chartX._endIndex; i++)
            {
                if (!volume[i].Value.HasValue) continue;
                if (volume[i].Value.Value > maxVolume) maxVolume = volume[i].Value.Value;
                else if (volume[i].Value.Value < minVolume) minVolume = volume[i].Value.Value;
            }

            double range = maxVolume - minVolume;
            double barVolumeIncrement = range / _sideVolumeDepthBars;
            double barHeight = _rootCanvas.ActualHeight / _sideVolumeDepthBars;

            double volBar = minVolume;
            int[] volBarsPos = new int[_sideVolumeDepthBars];
            int[] volBarsNeg = new int[_sideVolumeDepthBars];

            for (int n = 0; n < _sideVolumeDepthBars; n++)
            {
                for (int j = _chartX._startIndex; j < _chartX._endIndex; j++)
                {
                    if (!volume[j].Value.HasValue) continue;
                    double v = volume[j].Value.Value;
                    if (v < volBar || v > volBar + barVolumeIncrement) continue;

                    if (close[j].Value > open[j].Value)
                        volBarsPos[n]++;
                    else if (close[j].Value < open[j].Value)
                        volBarsNeg[n]++;
                }
                volBar += barVolumeIncrement;
            }

            double[] volBarsWidthPos = new double[_sideVolumeDepthBars];
            double[] volBarsWidthNeg = new double[_sideVolumeDepthBars];

            double maxVolBars = Math.Max(Algorithms.Maximum(volBarsPos), Algorithms.Maximum(volBarsNeg));
            double minVolBars = Math.Min(Algorithms.Minimum(volBarsPos), Algorithms.Minimum(volBarsNeg));


            double volumeBarScaleWidth = _rootCanvas.ActualWidth * 0.15; //15% for positive values and 15% for negative values
            _rectsSideVolumeDepth.C = _rootCanvas;
            _rectsSideVolumeDepth.Start();

            for (int n = 0; n < _sideVolumeDepthBars; n++)
            {
                volBarsWidthPos[n] = (volBarsPos[n] - minVolBars) / (maxVolBars - minVolBars);
                volBarsWidthNeg[n] = (volBarsNeg[n] - minVolBars) / (maxVolBars - minVolBars);

                volBarsWidthPos[n] *= volumeBarScaleWidth;
                volBarsWidthNeg[n] *= volumeBarScaleWidth;

                double x1 = _chartX.LeftChartSpace;
                double y1 = n * barHeight;
                double x2 = _chartX.LeftChartSpace + volBarsWidthPos[n];
                double y2 = (n + 1) * barHeight;
                Utils.DrawRectangle(x1, y1, x2, y2, Brushes.Blue, _rectsSideVolumeDepth);

                x1 = x2;
                x2 = x1 + volBarsWidthNeg[n];
                Utils.DrawRectangle(x1, y1, x2, y2, Brushes.Red, _rectsSideVolumeDepth);
            }

            _rectsSideVolumeDepth.Stop();
            _rectsSideVolumeDepth.Do(r =>
                                       {
                                           r.ZIndex = ZIndexConstants.VolumeDepthBars;
                                           r._rectangle.Opacity = 0.5;
                                           r._rectangle.IsHitTestVisible = false;
                                       });
        }
    }
}
