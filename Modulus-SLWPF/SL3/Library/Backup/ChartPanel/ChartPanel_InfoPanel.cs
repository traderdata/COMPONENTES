using System.Collections.Generic;
using System.Diagnostics;
#if WPF
using System.Windows.Input;
#endif
#if SILVERLIGHT
using ModulusFE.SL.Utils;
#endif

namespace ModulusFE
{
    internal class ChartPanelInfoPanelAble : IInfoPanelAble
    {
        internal ChartPanel ChartPanel { get; set; }
        public IEnumerable<InfoPanelItem> InfoPanelItems
        {
            get
            {
                yield return new ChartPanelInfoPanelItemX(ChartPanel);
                yield return new ChartPanelInfoPanelItemY(ChartPanel);

                foreach (Series series in ChartPanel.AllSeriesCollection)
                {
                    yield return new SeriesInfoPanelItem(series);
                }
            }
        }
    }

    internal class ChartPanelInfoPanelItemX : InfoPanelItem
    {
        public ChartPanel ChartPanelOwner { get; private set; }

        public ChartPanelInfoPanelItemX(ChartPanel chartPanel)
        {
            ChartPanelOwner = chartPanel;
        }

        public override string Caption
        {
            get { return "X"; }
        }

        public override string Value
        {
            get
            {
                int index = (int)_infoPanel.GetReverseX();
                if (index < 0)
                {
                    return "N/A";
                }

                return (index + _infoPanel._chartX._startIndex).ToString();
            }
        }
    }

    internal class ChartPanelInfoPanelItemY : InfoPanelItem
    {
        public ChartPanel ChartPanelOwner { get; private set; }

        public ChartPanelInfoPanelItemY(ChartPanel chartPanel)
        {
            ChartPanelOwner = chartPanel;
        }

        public override string Caption
        {
            get { return "Y"; }
        }

        public override string Value
        {
            get
            {
                double y = Mouse.GetPosition(ChartPanelOwner._rootCanvas).Y;
                //Debug.WriteLine("Y Info " + y);
                return string.Format(ChartPanelOwner.FormatYValueString, ChartPanelOwner.GetReverseY(y));
            }
        }
    }

    internal class SeriesInfoPanelItem : InfoPanelItem
    {
        public Series Series { get; private set; }
        public SeriesInfoPanelItem(Series series)
        {
            Series = series;
        }

        public override string Caption
        {
            get { return Series.Title; }
        }

        public override string Value
        {
            get
            {
                int index = (int)_infoPanel.GetReverseX();
                if (index < 0 || index >= _infoPanel._chartX._endIndex)
                {
                    return null;
                }

                index += _infoPanel._chartX._startIndex;
                if (index >= _infoPanel._chartX._endIndex)
                {
                    return null;
                }

                double? value = Series[index].Value;
                return !value.HasValue ? null : string.Format(Series._chartPanel.FormatYValueString, value.Value);
            }
        }
    }
}

