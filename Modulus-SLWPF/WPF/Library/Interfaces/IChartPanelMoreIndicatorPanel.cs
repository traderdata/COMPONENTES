using System.Collections.Generic;
using System.Windows;

namespace ModulusFE.Interfaces
{
    ///<summary>
    /// Interface used to show a panel when user click on a more indicator in any chart panel
    ///</summary>
    public interface IChartPanelMoreIndicatorPanel
    {
        ///<summary>
        /// A reference to the element that is going to be shown
        ///</summary>
        UIElement ElementToShow { get; }

        ///<summary>
        /// Metho called to initialize the panel. 
        ///</summary>
        ///<param name="chartPanel">A reference to chart panel that has the more indicator.</param>
        ///<param name="lineStudies"></param>
        ///<param name="position"></param>
        void Init(ChartPanel chartPanel, IEnumerable<LineStudies.LineStudy> lineStudies, ChartPanelMoreIndicatorPosition position);
    }

    ///<summary>
    /// Represents the position of more indicatr relative to hosting panel
    ///</summary>
    public enum ChartPanelMoreIndicatorPosition
    {
        ///<summary>
        /// Top Left
        ///</summary>
        TopLeft,
        ///<summary>
        /// Bottom Left
        ///</summary>
        BottomLeft,
        ///<summary>
        /// Top Right
        ///</summary>
        TopRight,
        ///<summary>
        /// Bottom Right
        ///</summary>
        BottomRight
    }
}
