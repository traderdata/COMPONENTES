
using System;
using System.Windows.Media;

namespace ModulusFE
{
    /// <summary>
    /// Constants used in StockChartX and internal classes
    /// </summary>
    internal static class Constants
    {
        /// <summary>
        /// the height of the calendar
        /// </summary>
        public const int CalendarPanelHeight = 25;

        /// <summary>
        /// the height of the panel where the minimized ChartPanels will be shown
        /// </summary>
        public const int PanelsBarHeight = 24;

        /// <summary>
        /// the width of the button shown for panel that is minimized in panels bar
        /// </summary>
        public const int PanelsBarButtonWidth = 64;

        /// <summary>
        /// the width of Y Axis
        /// </summary>
        public const int YAxisWidth = 45;

        /// <summary>
        /// The height of panel's title bar
        /// </summary>
        public const int PanelTitleBarHeight = 20;

        /// <summary>
        /// Maximum number of parameters an indicator may support
        /// </summary>
        public const short MaxIndicatorParamCount = 10;

        /// <summary>
        /// Maximum number of parameters a LineStudy may have
        /// </summary>
        public const short MaxParams = 101;

        /// <summary>
        /// the diameter of selction dot
        /// </summary>
        public const short SelectionDotSize = 8;

        /// <summary>
        /// Maximum number pf parameters a price style may have
        /// </summary>
        public const short MaxPriceStyleParams = 10;

        public static readonly Color FadeColor = Color.FromArgb(255, 50, 50, 50);

        public const IndicatorType MA_START = IndicatorType.SimpleMovingAverage;
        public const IndicatorType MA_END = IndicatorType.WeightedMovingAverage;

        public static Type TypeString = typeof(string);
    }

    internal static class ZIndexConstants
    {
        internal static int GridLines = 10;
        internal static int VolumeDepthBars = GridLines + 10;
        internal static int DarvasBoxes1 = VolumeDepthBars + 10;
        internal static int DarvasBoxes2 = DarvasBoxes1 + 10;
        internal static int DarvasBoxes3 = DarvasBoxes2 + 10;
        internal static int PriceStyles1 = DarvasBoxes3 + 10;
        internal static int PriceStyles2 = PriceStyles1 + 10;
        internal static int PriceStyles3 = PriceStyles2 + 10;
        internal static int PriceStyles4 = PriceStyles3 + 10;
        internal static int Indicators1 = PriceStyles4 + 10;
        internal static int Indicators2 = Indicators1 + 10;
        internal static int Indicators3 = Indicators2 + 10;
        internal static int LineStudies1 = Indicators3 + 10;
        internal static int LineStudies2 = LineStudies1 + 10;
        internal static int LineStudies3 = LineStudies2 + 10;
        internal static int SelectionPoint1 = LineStudies3 + 10;
        internal static int SelectionPoint2 = SelectionPoint1 + 10;
        internal static int MoveSeriesIndicator = SelectionPoint2 + 10;
        internal static int CrossHairs = MoveSeriesIndicator + 10;
        internal static int TextLabelTitle = CrossHairs + 10;
        internal static int InfoPanel = TextLabelTitle + 10;
        internal static int LineStudyContextMenu = InfoPanel + 10;
        internal static int ZoomArea = LineStudyContextMenu + 10;
    }
}

