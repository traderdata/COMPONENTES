using System.Windows.Input;
using ModulusFE.Indicators;

namespace ModulusFE
{
    public partial class ChartPanel
    {
        private MoveSeriesIndicator.MoveStatusEnum CanStartMoveSeries(Series series)
        {
            //a series can be moved if
            //1. single series on a panel - only to an existing panel
            //2. if a panel has more series - then to a new panel, or to an existing one
            if (series == null || !series.Selectable) return MoveSeriesIndicator.MoveStatusEnum.CantMove;

            if (series._seriesType == SeriesTypeEnum.stCandleChart)
            {
                if (_series.Count > 4) return MoveSeriesIndicator.MoveStatusEnum.MoveToNewPanel;
                //candle OHLC group, must have at least 5 series
                return _series.Count == 4 ? MoveSeriesIndicator.MoveStatusEnum.MoveToExistingPanel : MoveSeriesIndicator.MoveStatusEnum.CantMove;
            }
            if (series._seriesType == SeriesTypeEnum.stStockBarChart ||
              series._seriesType == SeriesTypeEnum.stStockBarChartHLC)
            {
                if (_series.Count > 3) return MoveSeriesIndicator.MoveStatusEnum.MoveToNewPanel;
                return _series.Count == 3 ? MoveSeriesIndicator.MoveStatusEnum.MoveToExistingPanel : MoveSeriesIndicator.MoveStatusEnum.CantMove; //HLC group
            }
            //otherwise is single line series
            if (_series.Count > 1) return MoveSeriesIndicator.MoveStatusEnum.MoveToNewPanel;
            return _series.Count > 0 ? MoveSeriesIndicator.MoveStatusEnum.MoveToExistingPanel : MoveSeriesIndicator.MoveStatusEnum.CantMove;
        }

        private void StartMoveSeries(Series series, int clickCount)
        {
            if (_seriesSelected != null && _seriesSelected._recycleFlag)
            {
                return;
            }

            if (_seriesSelected != null && _seriesSelected != series)
            {
                _seriesSelected.HideSelection();
            }

            _seriesSelected = series;
            _seriesSelected.ShowSelection();

            if (_lineStudySelected != null)
            {
                _chartX.LineStudySelectedCount--;
                _lineStudySelected.Selected = false;
            }

            if (clickCount == 2)
            {
                Indicator indicator = series as Indicator;
                if (indicator != null)
                {
                    _leftMouseDown = false;
                    if (!_chartX.FireIndicatorDoubleClick(indicator)) //show series prop dialog
                    {
                        indicator.ShowParametersDialog();
                    }
                }
                else
                {
                    _chartX.FireSeriesDoubleClick(series);
                }
            }
            else
            {
                _rootCanvas.CaptureMouse();
            }
        }

        private MoveSeriesIndicator.MoveStatusEnum _moveStatusEnum;
        private ChartPanel _chartPanelToMoveTo;
        private void SeriesMoving(MouseEventArgs e)
        {
            MoveSeriesIndicator.MoveStatusEnum moveStatusEnum = CanStartMoveSeries(_seriesSelected);

            object o;
            ObjectFromCursor objectFromCursor = _chartX.GetObjectFromCursor(out o);

            _chartPanelToMoveTo = null;

            if (moveStatusEnum == MoveSeriesIndicator.MoveStatusEnum.CantMove)
                _moveStatusEnum = MoveSeriesIndicator.MoveStatusEnum.CantMove;
            else switch (objectFromCursor)
                {
                    case ObjectFromCursor.PanelRightYAxis:
                    case ObjectFromCursor.PanelLeftYAxis:
                        if (_chartX.MaximizedPanel != null)
                            _moveStatusEnum = MoveSeriesIndicator.MoveStatusEnum.CantMove;
                        else
                            _moveStatusEnum = moveStatusEnum == MoveSeriesIndicator.MoveStatusEnum.MoveToNewPanel
                                                ? moveStatusEnum
                                                : MoveSeriesIndicator.MoveStatusEnum.CantMove;
                        break;
                    case ObjectFromCursor.PanelRightNonPaintableArea:
                    case ObjectFromCursor.PanelPaintableArea:
                    case ObjectFromCursor.PanelLeftNonPaintableArea:
                        _chartPanelToMoveTo = (ChartPanel)o;
                        _moveStatusEnum = _chartPanelToMoveTo._index != _index
                                            ? MoveSeriesIndicator.MoveStatusEnum.MoveToExistingPanel
                                            : MoveSeriesIndicator.MoveStatusEnum.CantMove;
                        break;
                }

            _panelsContainer.ShowMoveSeriesIndicator(_panelsContainer.ToPanelsHolder(e), _moveStatusEnum);
        }

        internal void MoveSeriesTo(Series seriesToMove, ChartPanel chartPanelToMoveTo, MoveSeriesIndicator.MoveStatusEnum moveStatusEnum)
        {
            _panelsContainer.HideMoveSeriesIndicator();
            _chartX.Status = StockChartX.ChartStatus.Ready;
            seriesToMove.HideSelection();
            _rootCanvas.ReleaseMouseCapture();

            if (moveStatusEnum == MoveSeriesIndicator.MoveStatusEnum.CantMove) return;

            ChartPanel chartPanel = (moveStatusEnum == MoveSeriesIndicator.MoveStatusEnum.MoveToNewPanel
                                       ? null
                                       : chartPanelToMoveTo) ?? _chartX.AddChartPanel();
            chartPanel._enforceSeriesSetMinMax = true;

            seriesToMove.Painted = false;
            seriesToMove.MoveToPanel(chartPanel);
            if ((seriesToMove._seriesType != SeriesTypeEnum.stCandleChart &&
                 seriesToMove._seriesType != SeriesTypeEnum.stStockBarChart) &&
                seriesToMove._seriesType != SeriesTypeEnum.stStockBarChartHLC)
            {
                _chartX.FireSeriesMoved(seriesToMove, _series.Count == 0 ? -1 : Index, chartPanel.Index);

                _chartX.UpdateByTimer();
                return;
            }
            //series is a part from (O)|HLC group, move the others too
            Series seriesRelated;
            if (seriesToMove.OHLCType != SeriesTypeOHLC.Open &&
                (seriesRelated = GetSeriesOHLCV(seriesToMove, SeriesTypeOHLC.Open)) != null)
            {
                seriesRelated.Painted = false;
                seriesRelated.MoveToPanel(chartPanel);
            }
            if (seriesToMove.OHLCType != SeriesTypeOHLC.High &&
                (seriesRelated = GetSeriesOHLCV(seriesToMove, SeriesTypeOHLC.High)) != null)
            {
                seriesRelated.Painted = false;
                seriesRelated.MoveToPanel(chartPanel);
            }
            if (seriesToMove.OHLCType != SeriesTypeOHLC.Low &&
                (seriesRelated = GetSeriesOHLCV(seriesToMove, SeriesTypeOHLC.Low)) != null)
            {
                seriesRelated.Painted = false;
                seriesRelated.MoveToPanel(chartPanel);
            }
            if (seriesToMove.OHLCType != SeriesTypeOHLC.Close &&
                (seriesRelated = GetSeriesOHLCV(seriesToMove, SeriesTypeOHLC.Close)) != null)
            {
                seriesRelated.Painted = false;
                seriesRelated.MoveToPanel(chartPanel);
            }

            _chartX.FireSeriesMoved(seriesToMove, _series.Count == 0 ? -1 : Index, chartPanel.Index);

            _chartX.UpdateByTimer();
        }
    }
}


