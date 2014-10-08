using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using ModulusFE.PaintObjects;

namespace ModulusFE.LineStudies
{
    public partial class LineStudy
    {
        ///<summary>
        /// Checks whether the lineStudy should pe painted at the current zoom level
        ///</summary>
        ///<returns></returns>
        public virtual LSVisibility IsCurrentlyVisible() { return LSVisibility.Visible; }
        ///<summary>
        /// Ensures the LineStudy is visible
        ///</summary>
        ///<param name="position"></param>
        public virtual void EnsureVisible(EnsureVisibilityPosition position) { }

        ///<summary>
        /// Returns a rectangle that will ensure the underlying LineStudy will fit into.
        /// Left &amp; Right values have the indexes from X axes
        /// Top &amp; Bottom values have the prices from Y axes
        ///</summary>
        public virtual Types.RectEx NeededVisibleBounds { get { return Types.RectEx.Empty; } }

        ///<summary>
        /// Makes visible all the supplied LineStudies
        ///</summary>
        ///<param name="chartPanel"></param>
        public static void EnsureVisibleMultiple(ChartPanel chartPanel)
        {
            bool any = false;
            double minPrice = double.MaxValue;
            double maxPrice = double.MinValue;

            foreach (Types.RectEx rc in
              chartPanel.LineStudiesCollection.Where(_ => _.IsCurrentlyVisible() != LSVisibility.Visible)
              .Where(_ => !_.NeededVisibleBounds.IsZero)
              .Select(ls => ls.NeededVisibleBounds))
            {
                any = true;

                if (rc.Bottom < minPrice)
                    minPrice = rc.Bottom;
                if (rc.Top > maxPrice)
                    maxPrice = rc.Top;
            }

            if (!any) return;

            var c = chartPanel._chartX;

            int startMinIndex = -1, startMaxIndex = -1;
            int endMinIndex = -1, endMaxIndex = -1;
            int[] seriesIndexes = chartPanel.SeriesCollection.Select(_ => _.SeriesIndex).ToArray();

            int idx = c._startIndex;
            while (idx >= 0 && (startMinIndex == -1 || endMinIndex == -1))
            {
                double min, max;
                c._dataManager.MinMaxFromIndex(seriesIndexes, idx, out min, out max);

                if (min < minPrice && startMinIndex == -1)
                    startMinIndex = idx;
                if (max > maxPrice && startMaxIndex == -1)
                    startMaxIndex = idx;

                idx--;
            }
            idx = c._endIndex;
            int cnt = c.RecordCount;
            while (idx < cnt && (endMaxIndex == -1 || endMinIndex == -1))
            {
                double min, max;
                c._dataManager.MinMaxFromIndex(seriesIndexes, idx, out min, out max);

                if (endMinIndex == -1 && min < minPrice)
                    endMinIndex = idx;
                if (endMaxIndex == -1 && max > maxPrice)
                    endMaxIndex = idx;

                idx++;
            }
            int startIndex = Math.Min(startMinIndex, startMaxIndex);
            int endIndex = Math.Max(endMinIndex, endMaxIndex);
            if (startIndex == -1)
                startIndex = 0;
            if (endIndex == -1)
                endIndex = c.RecordCount - 1;

            c.FirstVisibleRecord = startIndex;
            c.LastVisibleRecord = endIndex;

            chartPanel.ResetYScale();
        }
    }
}
