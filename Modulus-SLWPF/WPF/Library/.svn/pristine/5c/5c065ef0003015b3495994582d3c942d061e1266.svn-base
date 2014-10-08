using System;
using ModulusFE.PriceStyles;

namespace ModulusFE
{
    internal partial class Stock : Series
    {
        internal PriceStyles.Stock _priceStyleStock;

        internal Stock(string name, SeriesTypeEnum seriesType, SeriesTypeOHLC seriesTypeOHLC, ChartPanel chartPanel)
            : base(name, seriesType, seriesTypeOHLC, chartPanel)
        {
            Init();
            _chartPanel._chartX.ChartReseted += ChartX_OnChartReseted;
            _priceStyleStock = null;
        }

        internal override void UnSubscribe()
        {
            base.UnSubscribe();

            _chartPanel._chartX.ChartReseted -= ChartX_OnChartReseted;
        }

        private void ChartX_OnChartReseted(object sender, EventArgs e)
        {
            _priceStyle = null;
            _darvasBoxes = null;
            _priceStyleType = PriceStyleEnum.psUnknown;
            _seriesTypeType = SeriesTypeEnum.stUnknown;
        }

        private Style _priceStyle;
        private PriceStyleEnum _priceStyleType = PriceStyleEnum.psUnknown;
        private SeriesTypeEnum _seriesTypeType = SeriesTypeEnum.stUnknown;
        private DarvasBoxes _darvasBoxes;

        internal override void Paint()
        {
            Paint(null);
        }

        internal override void Paint(object drawingContext)
        {
            //if (_painted) return;

            Style ps = null;
            if (_priceStyleType != _chartPanel._chartX._priceStyle || _seriesTypeType != _seriesType)
            {
                if (_chartPanel._chartX._priceStyle != PriceStyleEnum.psStandard)
                {
                    switch (_chartPanel._chartX._priceStyle)
                    {
                        case PriceStyleEnum.psKagi:
                            ps = new Kagi(this);
                            break;
                        case PriceStyleEnum.psCandleVolume:
                        case PriceStyleEnum.psEquiVolume:
                        case PriceStyleEnum.psEquiVolumeShadow:
                            ps = new EquiVolume(this);
                            break;
                        case PriceStyleEnum.psPointAndFigure:
                            ps = new PointAndFigure(this);
                            break;
                        case PriceStyleEnum.psRenko:
                            ps = new Renko(this);
                            break;
                        case PriceStyleEnum.psThreeLineBreak:
                            ps = new ThreeLineBreak(this);
                            break;
                        case PriceStyleEnum.psHeikinAshi:
                            ps = new HeikinAshi(this);
                            break;
                    }
                }
                else
                {
                    switch (_seriesType)
                    {
                        case SeriesTypeEnum.stCandleChart:
                            ps = new Candles(this);
                            break;
                        case SeriesTypeEnum.stStockBarChartHLC:
                        case SeriesTypeEnum.stStockBarChart:
                            ps = new PriceStyles.Stock(this);
                            break;
                        case SeriesTypeEnum.stLineChart:
                            ps = new Linear(this);
                            break;
                    }
                }
                if (_priceStyle != null)
                {
                    _priceStyle.RemovePaint();
                }
            }

            if (_darvasBoxes != null)
            {
                _darvasBoxes.RemovePaint();
            }

            if (_chartPanel._chartX._priceStyle == PriceStyleEnum.psStandard || _chartPanel._chartX._priceStyle == PriceStyleEnum.psHeikinAshi)
            {
                if (_darvasBoxes == null)
                {
                    _darvasBoxes = new DarvasBoxes(this);
                }

                _darvasBoxes.SetSeriesStock(this);
                if (_chartPanel._chartX._darwasBoxes)
                {
                    _darvasBoxes.Paint();
                }
            }

            if (_priceStyle != null || ps != null)
            {
                (ps ?? _priceStyle).SetStockSeries(this);
                Style psToPaint = ps ?? _priceStyle;
                bool res;
                if (psToPaint is Candles && drawingContext != null)
                {
                    res = psToPaint.Paint(drawingContext);
                }
                else
                {
                    res = psToPaint.Paint();
                }
                //if (!(ps ?? _priceStyle).Paint()) return;
                if (!res)
                {
                    return;
                }
            }

            if (Selected)
            {
                ShowSelection();
            }

            if (ps == null)
            {
                return;
            }

            _priceStyle = ps;
            _priceStyleType = _chartPanel._chartX._priceStyle;
            _seriesTypeType = _seriesType;
        }

        internal override void MoveToPanel(ChartPanel chartPanel)
        {
            if (_priceStyle != null)
            {
                _priceStyle.RemovePaint();
            }

            //Debug.Assert(_priceStyleType == PriceStyleEnum. || _priceStyleType == PriceStyleEnum.psHeikinAshi);
            _chartPanel.DeleteSeries(this);
            _chartPanel = chartPanel;
            _chartPanel.AddSeries(this);

            base.MoveToPanel(chartPanel);
        }

        internal override void SetStrokeColor()
        {
        }

        internal override void SetStrokeThickness()
        {
            if (_priceStyleStock != null)
            {
                _priceStyleStock.SetStrokeThickness(_strokeThickness);
            }
        }

        internal override void RemovePaint()
        {
            if (_priceStyle != null)
            {
                _priceStyle.RemovePaint();
            }
        }
    }
}
