using System.Windows.Media;

namespace ModulusFE.PriceStyles
{
    internal partial class Linear : Style
    {
        private readonly PaintObjects.PaintObjectsManager<PaintObjects.Line> _lines = new PaintObjects.PaintObjectsManager<PaintObjects.Line>();

        public Linear(Series series)
            : base(series)
        {

        }

        private Series _closeSeries;
        public override bool Paint()
        {
            _closeSeries = _series._chartPanel.GetSeriesOHLCV(_series, SeriesTypeOHLC.Close);
            if (_closeSeries == null)
            {
                return false;
            }

            if (_closeSeries.Painted || !_closeSeries._visible || _closeSeries.RecordCount < 0 ||
                _closeSeries.RecordCount < _closeSeries._chartPanel._chartX._startIndex)
            {
                return false;
            }

            _closeSeries.Painted = true;

            _lines.C = _closeSeries._chartPanel._rootCanvas;
            _lines.Start();

            Brush strokeUpBrush =
              new SolidColorBrush(_closeSeries.UpColor == null
                                    ? _closeSeries._chartPanel._chartX.UpColor
                                    : _closeSeries.UpColor.Value);
            Brush strokeDownBrush =
              new SolidColorBrush(_closeSeries.DownColor == null
                                    ? _closeSeries._chartPanel._chartX.DownColor
                                    : _closeSeries.DownColor.Value);
            Brush strokeNormalBrush = new SolidColorBrush(_closeSeries._strokeColor);
            Brush currentBrush = strokeNormalBrush;

            double x2 = _closeSeries._chartPanel._chartX.GetXPixel(0);
            double? y1;
            double? y2 = null;

            int cnt = 0;
            for (int i = _closeSeries._chartPanel._chartX._startIndex;
                 i < _closeSeries._chartPanel._chartX._endIndex;
                 i++, cnt++)
            {
                //cnt++;
                double x1 = _closeSeries._chartPanel._chartX.GetXPixel(cnt);
                y1 = _closeSeries[i].Value;
                if (!y1.HasValue)
                {
                    continue;
                }

                y1 = _closeSeries.GetY(y1.Value);
                if (i == _closeSeries._chartPanel._chartX._startIndex)
                {
                    y2 = y1.Value;
                }

                // +/- change colors
                if ((_closeSeries._chartPanel._chartX._useLineSeriesColors || _closeSeries._upColor.HasValue))
                {
                    if (i > 0)
                    {
                        if (_closeSeries[i].Value > _closeSeries[i - 1].Value)
                        {
                            currentBrush = strokeUpBrush;
                        }
                        else
                        {
                            currentBrush = _closeSeries[i].Value < _closeSeries[i - 1].Value ? strokeDownBrush : strokeNormalBrush;
                        }
                    }
                    else
                    {
                        currentBrush = strokeNormalBrush;
                    }
                }

                if (i > _closeSeries._chartPanel._chartX._startIndex)
                {
                    if (_closeSeries[i].Value.HasValue && y2.HasValue)
                    {
                        DrawLine(x1, y1.Value, x2, y2.Value, currentBrush);
                    }
                }

                y2 = y1;
                x2 = x1;
            }

            _lines.Stop();

            _lines.Do(l =>
                        {
                            l.ZIndex = ZIndexConstants.PriceStyles1;
                            l._line.Tag = _closeSeries;
                        });

            if (_closeSeries.Selected)
            {
                _closeSeries.ShowSelection();
            }

            return true;
        }

        private void DrawLine(double x1, double y1, double x2, double y2, Brush strokeBrush)
        {
            Utils.DrawLine(x1, y1, x2, y2, strokeBrush, _closeSeries._strokePattern, _closeSeries._strokeThickness, _closeSeries._opacity, _lines);
        }

        public override void RemovePaint()
        {
            _lines.RemoveAll();
        }
    }
}

