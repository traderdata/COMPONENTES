using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using ModulusFE.Controls;
using ModulusFE.PaintObjects;

#if SILVERLIGHT
using ModulusFE.SL;
#endif

namespace ModulusFE
{
    public partial class StockChartX
    {
        static StockChartX()
        {
            #region Dependency properties registration (WPF and SILVERLIGHT code)

#if WPF
            DefaultStyleKeyProperty.OverrideMetadata(typeof(StockChartX), new FrameworkPropertyMetadata(typeof(StockChartX)));

            ShowAnimationsProperty =
                DependencyProperty.Register("ShowAnimations",
                                            typeof(bool), typeof(StockChartX),
                                            new FrameworkPropertyMetadata(new PropertyChangedCallback(OnShowAnimationsChanged)));

            ScaleAlignmentProperty =
                DependencyProperty.Register("ScaleAlignment",
                                            typeof(ScaleAlignmentTypeEnum), typeof(StockChartX),
                                            new FrameworkPropertyMetadata(ScaleAlignmentTypeEnum.Right, OnScaleAlignmentChanged));

            YGridProperty =
                DependencyProperty.Register("ShowYGrid",
                                            typeof(bool), typeof(StockChartX),
                                            new FrameworkPropertyMetadata(true, OnShowYGridChanged));

            XGridProperty =
                DependencyProperty.Register("ShowXGrid",
                                            typeof(bool), typeof(StockChartX),
                                            new FrameworkPropertyMetadata(false, OnShowXGridChanged));

            GridStrokeProperty =
                DependencyProperty.Register("GridStroke",
                                            typeof(Brush), typeof(StockChartX),
                                            new FrameworkPropertyMetadata(Brushes.Silver, OnGridStrokeChanged));

            CrossHairsProperty =
                DependencyProperty.Register("CrossHairs",
                                            typeof(bool), typeof(StockChartX),
                                            new FrameworkPropertyMetadata(false, OnCrossHairsChanged));

            CrossHairsStrokeProperty =
                DependencyProperty.Register("CrossHairsStroke",
                                            typeof(Brush), typeof(StockChartX),
                                            new FrameworkPropertyMetadata(Brushes.Yellow, OnCrossHairsColorChanged));

            CrossHairsPositionProperty =
                DependencyProperty.Register("CrossHairsPosition",
                                            typeof(Point), typeof(StockChartX),
                                            new FrameworkPropertyMetadata(new Point(), OnCrossHairsPositionChanged));

            DisplayTitlesProperty =
                DependencyProperty.Register("DisplayTitles",
                                            typeof(bool), typeof(StockChartX),
                                            new FrameworkPropertyMetadata(true, OnDisplayTitlesChanged));

            PanelMinHeightProperty =
                DependencyProperty.Register("PanelMinHeight",
                                            typeof(double), typeof(StockChartX),
                                            new FrameworkPropertyMetadata((double)(Constants.PanelTitleBarHeight + 10), OnPanelMinHeightChanged));

            IndicatorDialogBackgroundProperty =
                DependencyProperty.Register("IndicatorDialogBackground",
                                            typeof(Brush), typeof(StockChartX),
                                            new FrameworkPropertyMetadata(
                                                new RadialGradientBrush
                                                {
                                                    Center = new Point(0.6, 0.7),
                                                    RadiusX = 1,
                                                    RadiusY = 1,
                                                    GradientStops =
                                                        new GradientStopCollection
                                                        {
                                                            new GradientStop(Colors.LightBlue, 0),
                                                            new GradientStop(Colors.LightSteelBlue, 1)
                                                        }
                                                }));

            IndicatorTwinTitleVisibilityProperty =
                DependencyProperty.Register("IndicatorTwinTitleVisibility",
                                            typeof(Visibility), typeof(StockChartX),
                                            new FrameworkPropertyMetadata(Visibility.Visible, OnIndicatorTwinTitleVisibilityChanged));

            InfoPanelLabelsBackgroundProperty =
                DependencyProperty.Register("InfoPanelLabelsBackground", 
                                            typeof(Brush), typeof(StockChartX),
                                            new FrameworkPropertyMetadata(Brushes.Yellow, OnInfoPanelLabelsBackgroundChanged));

            InfoPanelLabelsForegroundProperty =
                DependencyProperty.Register("InfoPanelLabelsForeground",
                                            typeof(Brush), typeof(StockChartX),
                                            new FrameworkPropertyMetadata(Brushes.Black, OnInfoPanelLabelsForegroundChanged));

            InfoPanelValuesBackgroundProperty =
                DependencyProperty.Register("InfoPanelValuesBackground",
                                            typeof(Brush), typeof(StockChartX),
                                            new FrameworkPropertyMetadata(Brushes.White, OnInfoPanelValuesBackgroundChanged));

            InfoPanelValuesForegroundProperty =
                DependencyProperty.Register("InfoPanelValuesForeground",
                                            typeof(Brush), typeof(StockChartX),
                                            new FrameworkPropertyMetadata(Brushes.Black, OnInfoPanelValuesForegroundChanged));

            InfoPanelFontFamilyProperty =
                DependencyProperty.Register("InfoPanelFontFamily",
                                            typeof(FontFamily), typeof(StockChartX),
                                            new FrameworkPropertyMetadata(new FontFamily("Arial"), OnInfoPanelFontFamilyChanged));

            InfoPanelFontSizeProperty =
                DependencyProperty.Register("InfoPanelFontSize",
                                            typeof(double), typeof(StockChartX),
                                            new FrameworkPropertyMetadata(9.0, OnInfoPanelFontSizeChanged));

            InfoPanelPositionProperty =
                DependencyProperty.Register("InfoPanelPosition",
                                            typeof(InfoPanelPositionEnum), typeof(StockChartX),
                                            new FrameworkPropertyMetadata(InfoPanelPositionEnum.FollowMouse, OnInfoPanelPositionChanged));

            IndicatorDialogLabelForegroundProperty =
                DependencyProperty.Register("IndicatorDialogLabelForeground",
                                            typeof(Brush), typeof(StockChartX),
                                            new FrameworkPropertyMetadata(new SolidColorBrush(Colors.Black), OnIndicatorDialogLabelForegroundChanged));

            IndicatorDialogLabelFontSizeProperty =
                DependencyProperty.Register("IndicatorDialogLabelFontSize",
                                            typeof(double), typeof(StockChartX),
                                            new FrameworkPropertyMetadata(11.0, OnIndicatorDialogLabelFontSizeChanged)); 
            
            VolumePostfixLetterProperty =
                DependencyProperty.Register("VolumePostfix",
                                            typeof(string), typeof(StockChartX),
                                            new FrameworkPropertyMetadata(""));

            HeatPanelLabelsForegroundProperty =
                DependencyProperty.Register("HeatPanelLabelsForeground",
                                            typeof(Brush), typeof(StockChartX),
                                            new FrameworkPropertyMetadata(Brushes.Yellow, OnHeatPanelLabelsForegroundChanged));

            HeatPanelLabelsBackgroundProperty =
                DependencyProperty.Register("HeatPanelLabelsBackground",
                                            typeof(Brush), typeof(StockChartX),
                                            new FrameworkPropertyMetadata(Brushes.Black, OnHeatPanelLabelsBackgroundChanged));

            HeatPanelLabelsFontSizeProperty =
                DependencyProperty.Register("HeatPanelLabelsFontSize",
                                            typeof(double), typeof(StockChartX),
                                            new FrameworkPropertyMetadata(12.0, OnHeatPanelLabelFontSizeChanged));

            LineStudyPropertyDialogBackgroundProperty =
                DependencyProperty.Register("LineStudyPropertyDialogBackground",
                                            typeof(Brush), typeof(StockChartX),
                                            new FrameworkPropertyMetadata(Brushes.White));

            ChartScrollerVisibleProperty =
                DependencyProperty.Register("IsChartScrollerVisible",
                                            typeof(bool), typeof(StockChartX),
                                            new FrameworkPropertyMetadata(true, OnChartScrollerVisibleChanged));

            ChartScrollerTrackBackgroundProperty =
                DependencyProperty.Register("ChartScrollerTrackBackground",
                                            typeof(Brush), typeof(StockChartX),
                                            new FrameworkPropertyMetadata(Brushes.Silver, OnChartScrollerTrackBackgroundChanged));

            ChartScrollerTrackButtonsBackgroundProperty =
                DependencyProperty.Register("ChartScrollerTrackButtonsBackground",
                                            typeof(Brush), typeof(StockChartX),
                                            new FrameworkPropertyMetadata(Brushes.Green, OnChartScrollerTrackButtonsBackgroundChanged));

            ChartScrollerThumbButtonBackgroundProperty =
                DependencyProperty.Register("ChartScrollerThumbButtonBackground",
                                            typeof(Brush), typeof(StockChartX),
                                            new FrameworkPropertyMetadata(
                                                new LinearGradientBrush
                                                {
                                                    StartPoint = new Point(0.486, 0),
                                                    EndPoint = new Point(0.486, 0.986),
                                                    GradientStops = new GradientStopCollection
                                                    {
                                                    new GradientStop(Colors.Gray, 0),
                                                    new GradientStop(Colors.MidnightBlue, 0.5),
                                                    new GradientStop(Colors.Gray, 1)
                                                    }
                                                }, OnChartScrollerThumbButtonBackgroundChanged
                                            ));

            ChartScrollerPropertiesProperty =
                DependencyProperty.Register("ChartScrollerProperties",
                                            typeof(ChartScrollerProperties), typeof(StockChartX),
                                            new FrameworkPropertyMetadata(null, ChartScrollerPropertiesChanged));

            AppendTickVolumeBehaviorProperty =
                DependencyProperty.Register("AppendTickVolumeBehavior",
                                            typeof(DataManager.AppendTickVolumeBehavior), typeof(StockChartX),
                                            new FrameworkPropertyMetadata(DataManager.AppendTickVolumeBehavior.Increment, OnAppendTickVolumeBehaviorChanged));

            MaxVisibleRecordsProperty =
                DependencyProperty.Register("MaxVisibleRecords",
                                            typeof(int), typeof(StockChartX),
                                            new FrameworkPropertyMetadata(300, OnMaxVisibleRecordsChanged));

            CalendarBackgroundProperty =
                DependencyProperty.Register("CalendarBackground",
                                            typeof(Brush), typeof(StockChartX),
                                            new FrameworkPropertyMetadata(new SolidColorBrush(Colors.Black)));

            ShowSecondsProperty =
                DependencyProperty.Register("ShowSeconds",
                                            typeof(bool), typeof(StockChartX),
                                            new FrameworkPropertyMetadata(false, OnShowSecondsChanged));

            CalendarVersionProperty =
                DependencyProperty.Register("CalendarVersion",
                                            typeof(CalendarVersionType), typeof(StockChartX),
                                            new FrameworkPropertyMetadata(CalendarVersionType.Version1, OnCalendarVersionChanged));

            CalendarV2LabelGapProperty =
                DependencyProperty.Register("CalendarV2LabelGap",
                                            typeof(double), typeof(StockChartX),
                                            new FrameworkPropertyMetadata(3.0, OnCalendarV2LabelGapChanged));

            CalendarV2CalendarLabelBlockOutputProperty =
                DependencyProperty.Register("CalendarV2CalendarLabelBlockOutput",
                                            typeof(CalendarLabelBlockOutputType), typeof(StockChartX),
                                            new FrameworkPropertyMetadata(CalendarLabelBlockOutputType.Beginning, OnCalendarV2CalendarLabelBlockOutputChanged));

            CalendarV2CurrentTimeStampProperty =
                DependencyProperty.Register("CalendarV2CurrentTimeStamp",
                                            typeof(bool), typeof(StockChartX),
                                            new FrameworkPropertyMetadata(true, OnCalendarV2CurrentTimeStampChanged));

            HorizontalGridLinePatternProperty =
                DependencyProperty.Register("HorizontalGridLinePattern",
                                            typeof(DoubleCollection), typeof(StockChartX),
                                            new FrameworkPropertyMetadata(null));

            VerticalGridLinePatternProperty =
                DependencyProperty.Register("VerticalGridLinePattern",
                                            typeof(DoubleCollection), typeof(StockChartX),
                                            new FrameworkPropertyMetadata(null));
#endif
#if SILVERLIGHT
            ShowAnimationsProperty =
                DependencyProperty.Register("ShowAnimations",
                                            typeof(bool), typeof(StockChartX),
                                            new PropertyMetadata(OnShowAnimationsChanged));

            ScaleAlignmentProperty =
                DependencyProperty.Register("ScaleAlignment",
                                            typeof(ScaleAlignmentTypeEnum), typeof(StockChartX),
                                            new PropertyMetadata(ScaleAlignmentTypeEnum.Right, OnScaleAlignmentChanged));

            YGridProperty =
                DependencyProperty.Register("ShowYGrid",
                                            typeof(bool), typeof(StockChartX),
                                            new PropertyMetadata(true, OnShowYGridChanged));

            XGridProperty =
                DependencyProperty.Register("ShowXGrid",
                                            typeof(bool), typeof(StockChartX),
                                            new PropertyMetadata(false, OnShowXGridChanged));

            GridStrokeProperty =
                DependencyProperty.Register("GridStroke",
                                            typeof(Brush), typeof(StockChartX),
                                            new PropertyMetadata(Brushes.Silver, OnGridStrokeChanged));

            CrossHairsProperty =
                DependencyProperty.Register("CrossHairs",
                                            typeof(bool), typeof(StockChartX),
                                            new PropertyMetadata(false, OnCrossHairsChanged));

            CrossHairsStrokeProperty =
                DependencyProperty.Register("CrossHairsStroke",
                                            typeof(Brush), typeof(StockChartX),
                                            new PropertyMetadata(Brushes.Yellow, OnCrossHairsColorChanged));

            CrossHairsPositionProperty =
                DependencyProperty.Register("CrossHairsPosition",
                                            typeof(Point), typeof(StockChartX),
                                            new PropertyMetadata(new Point(), OnCrossHairsPositionChanged));

            DisplayTitlesProperty =
                DependencyProperty.Register("DisplayTitles",
                                            typeof(bool), typeof(StockChartX),
                                            new PropertyMetadata(true, OnDisplayTitlesChanged));

            PanelMinHeightProperty =
                DependencyProperty.Register("PanelMinHeight",
                                            typeof(double), typeof(StockChartX),
                                            new PropertyMetadata((double)(Constants.PanelTitleBarHeight + 10), OnPanelMinHeightChanged));

            IndicatorDialogBackgroundProperty =
                DependencyProperty.Register("IndicatorDialogBackground",
                                            typeof(Brush), typeof(StockChartX),
                                            new PropertyMetadata(
                                                new RadialGradientBrush
                                                {
                                                    Center = new Point(0.6, 0.7),
                                                    RadiusX = 1,
                                                    RadiusY = 1,
                                                    GradientStops =
                                                        new GradientStopCollection
															{
																new GradientStop { Color = ColorsEx.LightBlue, Offset = 0 },
																new GradientStop { Color = ColorsEx.LightSteelBlue, Offset = 1 },
															}
                                                }));

            IndicatorTwinTitleVisibilityProperty =
                DependencyProperty.Register("IndicatorTwinTitleVisibility",
                                            typeof(Visibility), typeof(StockChartX),
                                            new PropertyMetadata(Visibility.Visible, OnIndicatorTwinTitleVisibilityChanged));

            InfoPanelLabelsBackgroundProperty =
                DependencyProperty.Register("InfoPanelLabelsBackground",
                                            typeof(Brush), typeof(StockChartX),
                                            new PropertyMetadata(Brushes.Yellow, OnInfoPanelLabelsBackgroundChanged));

            InfoPanelLabelsForegroundProperty =
                DependencyProperty.Register("InfoPanelLabelsForeground",
                                            typeof(Brush), typeof(StockChartX),
                                            new PropertyMetadata(Brushes.Black, OnInfoPanelLabelsForegroundChanged));

            InfoPanelValuesBackgroundProperty =
                DependencyProperty.Register("InfoPanelValuesBackground",
                                            typeof(Brush), typeof(StockChartX),
                                            new PropertyMetadata(Brushes.White, OnInfoPanelValuesBackgroundChanged));

            InfoPanelValuesForegroundProperty =
                DependencyProperty.Register("InfoPanelValuesForeground",
                                            typeof(Brush), typeof(StockChartX),
                                            new PropertyMetadata(Brushes.Black, OnInfoPanelValuesForegroundChanged));

            InfoPanelFontFamilyProperty =
                DependencyProperty.Register("InfoPanelFontFamily",
                                            typeof(FontFamily), typeof(StockChartX),
                                            new PropertyMetadata(new FontFamily("Arial"), OnInfoPanelFontFamilyChanged));

            InfoPanelFontSizeProperty =
                DependencyProperty.Register("InfoPanelFontSize",
                                            typeof(double), typeof(StockChartX),
                                            new PropertyMetadata(9.0, OnInfoPanelFontSizeChanged));

            InfoPanelPositionProperty =
                DependencyProperty.Register("InfoPanelPosition",
                                            typeof(InfoPanelPositionEnum), typeof(StockChartX),
                                            new PropertyMetadata(InfoPanelPositionEnum.FollowMouse, OnInfoPanelPositionChanged));

            IndicatorDialogLabelForegroundProperty =
                DependencyProperty.Register("IndicatorDialogLabelForeground",
                                            typeof(Brush), typeof(StockChartX),
                                            new PropertyMetadata(new SolidColorBrush(Colors.Black), OnIndicatorDialogLabelForegroundChanged));

            IndicatorDialogLabelFontSizeProperty =
                DependencyProperty.Register("IndicatorDialogLabelFontSize",
                                            typeof(double), typeof(StockChartX),
                                            new PropertyMetadata(11.0, OnIndicatorDialogLabelFontSizeChanged));

            VolumePostfixLetterProperty =
                DependencyProperty.Register("VolumePostfix",
                                            typeof(string), typeof(StockChartX),
                                            new PropertyMetadata(""));

            HeatPanelLabelsForegroundProperty =
                DependencyProperty.Register("HeatPanelLabelsForeground",
                                            typeof(Brush), typeof(StockChartX),
                                            new PropertyMetadata(Brushes.Yellow, OnHeatPanelLabelsForegroundChanged));

            HeatPanelLabelsBackgroundProperty =
                DependencyProperty.Register("HeatPanelLabelsBackground",
                                            typeof(Brush), typeof(StockChartX),
                                            new PropertyMetadata(Brushes.Black, OnHeatPanelLabelsBackgroundChanged));

            HeatPanelLabelsFontSizeProperty =
                DependencyProperty.Register("HeatPanelLabelsFontSize",
                                            typeof(double), typeof(StockChartX),
                                            new PropertyMetadata(12.0, OnHeatPanelLabelFontSizeChanged));

            LineStudyPropertyDialogBackgroundProperty =
                DependencyProperty.Register("LineStudyPropertyDialogBackground",
                                            typeof(Brush), typeof(StockChartX),
                                            new PropertyMetadata(Brushes.White));

            ChartScrollerVisibleProperty =
                DependencyProperty.Register("IsChartScrollerVisible",
                                            typeof(bool), typeof(StockChartX),
                                            new PropertyMetadata(true, OnChartScrollerVisibleChanged));

            ChartScrollerTrackBackgroundProperty =
                DependencyProperty.Register("ChartScrollerTrackBackground",
                                typeof(Brush), typeof(StockChartX),
                                new PropertyMetadata(Brushes.Silver, OnChartScrollerTrackBackgroundChanged));

            ChartScrollerTrackButtonsBackgroundProperty =
                DependencyProperty.Register("ChartScrollerTrackButtonsBackground",
                                            typeof(Brush), typeof(StockChartX),
                                            new PropertyMetadata(Brushes.Green, OnChartScrollerTrackButtonsBackgroundChanged));

            ChartScrollerThumbButtonBackgroundProperty =
                DependencyProperty.Register("ChartScrollerThumbButtonBackground",
                                            typeof(Brush), typeof(StockChartX),
                                            new PropertyMetadata(new LinearGradientBrush
                                            {
                                                StartPoint = new Point(0.486, 0),
                                                EndPoint = new Point(0.486, 0.986),
                                                GradientStops = new GradientStopCollection
												{
													new GradientStop
														{
															Color = ColorsEx.Gray,
															Offset = 0
														},
													new GradientStop
														{
															Color = ColorsEx.MidnightBlue,
															Offset = 0.5
														},
													new GradientStop
														{
															Color = ColorsEx.Gray,
															Offset = 1
														}
												}
                                            }, OnChartScrollerThumbButtonBackgroundChanged));

            ChartScrollerPropertiesProperty =
                DependencyProperty.Register("ChartScrollerProperties",
                                            typeof(ChartScrollerProperties), typeof(StockChartX),
                                            new PropertyMetadata(null, ChartScrollerPropertiesChanged));

            AppendTickVolumeBehaviorProperty =
                DependencyProperty.Register("AppendTickVolumeBehavior",
                                            typeof(DataManager.AppendTickVolumeBehavior), typeof(StockChartX),
                                            new PropertyMetadata(DataManager.AppendTickVolumeBehavior.Increment, OnAppendTickVolumeBehaviorChanged));

            MaxVisibleRecordsProperty =
                DependencyProperty.Register("MaxVisibleRecords",
                                            typeof(int), typeof(StockChartX),
                                            new PropertyMetadata(300, OnMaxVisibleRecordsChanged));

            CalendarBackgroundProperty =
                DependencyProperty.Register("CalendarBackground",
                                            typeof(Brush), typeof(StockChartX),
                                            new PropertyMetadata(new SolidColorBrush(Colors.Black)));

            ShowSecondsProperty =
                DependencyProperty.Register("ShowSeconds",
                                            typeof(bool), typeof(StockChartX),
                                            new PropertyMetadata(false, OnShowSecondsChanged));

            CalendarVersionProperty =
                DependencyProperty.Register("CalendarVersion",
                                            typeof(CalendarVersionType), typeof(StockChartX),
                                            new PropertyMetadata(CalendarVersionType.Version1, OnCalendarVersionChanged));

            CalendarV2LabelGapProperty =
                DependencyProperty.Register("CalendarV2LabelGap",
                                            typeof(double), typeof(StockChartX),
                                            new PropertyMetadata(3.0, OnCalendarV2LabelGapChanged));

            CalendarV2CalendarLabelBlockOutputProperty =
                DependencyProperty.Register("CalendarV2CalendarLabelBlockOutput",
                                            typeof(CalendarLabelBlockOutputType), typeof(StockChartX),
                                            new PropertyMetadata(CalendarLabelBlockOutputType.Beginning, OnCalendarV2CalendarLabelBlockOutputChanged));

            CalendarV2CurrentTimeStampProperty =
                DependencyProperty.Register("CalendarV2CurrentTimeStamp",
                                            typeof(bool), typeof(StockChartX),
                                            new PropertyMetadata(true, OnCalendarV2CurrentTimeStampChanged));

            HorizontalGridLinePatternProperty =
                DependencyProperty.Register("HorizontalGridLinePattern",
                                            typeof(DoubleCollection), typeof(StockChartX),
                                            new PropertyMetadata(null));

            VerticalGridLinePatternProperty =
                DependencyProperty.Register("VerticalGridLinePattern",
                                            typeof(DoubleCollection), typeof(StockChartX),
                                            new PropertyMetadata(null));

            URILicenseKeyProperty =
                DependencyProperty.Register("URILicenseKey",
                                            typeof(string), typeof(StockChartX),
                                            new PropertyMetadata("", OnURILicenseKeyPropertyChanged));

#endif
            #endregion
        }

        #region ShowAnimations
        /// <summary>
        /// Gets os sets the value indicating whether the animations will be shown or not. 
        /// </summary>
        public static DependencyProperty ShowAnimationsProperty;
        ///<summary>
        /// Gets os sets the value indicating whether the animations will be shown or not. 
        ///</summary>
        public bool ShowAnimations
        {
            get { return (bool)GetValue(ShowAnimationsProperty); }
            set { SetValue(ShowAnimationsProperty, value); }
        }
        private static void OnShowAnimationsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs eventArgs)
        {

        }
        #endregion

        #region Grid Properties

        #region ShowYGrid
        /// <summary>
        /// Gets or sets the visibility of Y grid
        /// </summary>
        public static readonly DependencyProperty YGridProperty;
        ///<summary>
        /// Gets or sets the visibility of Y grid
        ///</summary>
        public bool YGrid
        {
            get { return (bool)GetValue(YGridProperty); }
            set { SetValue(YGridProperty, value); }
        }
        private static void OnShowYGridChanged(DependencyObject sender, DependencyPropertyChangedEventArgs eventArgs)
        {
            StockChartX stockChartX = (StockChartX)sender;
            stockChartX.ShowHideYGridLines();
            stockChartX.OnPropertyChanged("YGrid");
        }
        #endregion

        #region ShowXGrid
        /// <summary>
        /// Gets or sets the visibility of X grid
        /// </summary>
        public static readonly DependencyProperty XGridProperty;
        ///<summary>
        /// Gets or sets the visibility of X grid
        ///</summary>
        public bool XGrid
        {
            get { return (bool)GetValue(XGridProperty); }
            set { SetValue(XGridProperty, value); }
        }
        private static void OnShowXGridChanged(DependencyObject sender, DependencyPropertyChangedEventArgs eventArgs)
        {
            StockChartX stockChartX = (StockChartX)sender;
            stockChartX.ShowHideXGridLines();
            stockChartX.OnPropertyChanged("XGrid");
        }
        #endregion

        #region Grid Stroke
        ///<summary>
        /// Gets or sets the <seealso cref="Brush"/> that specifies how grid lines are painted
        ///</summary>
        public static readonly DependencyProperty GridStrokeProperty;
        ///<summary>
        /// Gets or sets the <seealso cref="Brush"/> that specifies how grid lines are painted
        ///</summary>
        public Brush GridStroke
        {
            get { return (Brush)GetValue(GridStrokeProperty); }
            set { SetValue(GridStrokeProperty, value); }
        }
        private static void OnGridStrokeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs eventArgs)
        {
            ((StockChartX)sender).Update();
        }
        #endregion

        #region HorizontalGridLinePatternProperty
        ///<summary>
        ///</summary>
        public static readonly DependencyProperty HorizontalGridLinePatternProperty;
        ///<summary>
        /// Gets or sets a collection of Double values that indicate the pattern of dashes and gaps that is used to outline shapes
        /// for horizontal grid lines.
        ///</summary>
        public DoubleCollection HorizontalGridLinePattern
        {
            get { return (DoubleCollection)GetValue(HorizontalGridLinePatternProperty); }
            set { SetValue(HorizontalGridLinePatternProperty, value); }
        }
        #endregion

        #region VerticalGridLinePatternProperty
        ///<summary>
        ///</summary>
        public static readonly DependencyProperty VerticalGridLinePatternProperty;
        ///<summary>
        /// Gets or sets a collection of Double values that indicate the pattern of dashes and gaps that is used to outline shapes
        /// for vertical grid lines.
        ///</summary>
        public DoubleCollection VerticalGridLinePattern
        {
            get { return (DoubleCollection)GetValue(VerticalGridLinePatternProperty); }
            set { SetValue(VerticalGridLinePatternProperty, value); }
        }
        #endregion

        #endregion

        #region ScaleAlignment
        ///<summary>
        /// Gets or sets the value that deermines what Y axis is shown
        ///</summary>
        public static readonly DependencyProperty ScaleAlignmentProperty;
        ///<summary>
        /// Gets or sets the value that deermines what Y axis is shown
        ///</summary>
        public ScaleAlignmentTypeEnum ScaleAlignment
        {
            get { return (ScaleAlignmentTypeEnum)GetValue(ScaleAlignmentProperty); }
            set { SetValue(ScaleAlignmentProperty, value); }
        }
        private static void OnScaleAlignmentChanged(DependencyObject sender, DependencyPropertyChangedEventArgs eventArgs)
        {
            StockChartX chartX = (StockChartX)sender;
            chartX._scaleAlignement = (ScaleAlignmentTypeEnum)eventArgs.NewValue;
            chartX.SetYAxes();
        }
        #endregion

        #region Cross Hair Properties

        #region CrossHairs
        ///<summary>
        /// Gets or sets the visibility of crosshairs lines
        ///</summary>
        public static readonly DependencyProperty CrossHairsProperty;
        ///<summary>
        /// Gets or sets the visibility of crosshairs lines
        ///</summary>
        public bool CrossHairs
        {
            get { return (bool)GetValue(CrossHairsProperty); }
            set { SetValue(CrossHairsProperty, value); }
        }
        private static void OnCrossHairsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs eventArgs)
        {
            StockChartX chartX = (StockChartX)sender;

            if ((bool)eventArgs.NewValue)
                chartX._timers.StartTimerWork(TimerCrossHairs);
            else
            {
                chartX._timers.StopTimerWork(TimerCrossHairs);
                chartX._panelsContainer.HideCrossHairs();
            }
        }
        #endregion

        #region CrossHairs Color
        /// <summary>
        /// Gets or sets the color of crosshairs lines
        /// </summary>
        public static readonly DependencyProperty CrossHairsStrokeProperty;
        /// <summary>
        /// Gets or sets the color of crosshairs lines
        /// </summary>
        public Brush CrossHairsStroke
        {
            get { return (Brush)GetValue(CrossHairsStrokeProperty); }
            set { SetValue(CrossHairsStrokeProperty, value); }
        }
        private static void OnCrossHairsColorChanged(DependencyObject sender, DependencyPropertyChangedEventArgs eventArgs)
        {
            StockChartX chartX = (StockChartX)sender;

            if (chartX._panelsContainer != null)
                chartX._panelsContainer.UpdateCrossHairsColor();
        }
        #endregion

        #region CrossHairs Position
        /// <summary>
        /// Gets or sets the location of crosshairs lines
        /// </summary>
        public static readonly DependencyProperty CrossHairsPositionProperty;
        /// <summary>
        /// Gets or sets the location of crosshairs lines
        /// </summary>
        public Point CrossHairsPosition
        {
            get { return (Point)GetValue(CrossHairsPositionProperty); }
            set { SetValue(CrossHairsPositionProperty, value); }
        }
        private static void OnCrossHairsPositionChanged(DependencyObject sender, DependencyPropertyChangedEventArgs eventArgs)
        {
            StockChartX chartX = (StockChartX)sender;
            if (chartX._panelsContainer != null)
                chartX._panelsContainer.ShowCrossHairs();
        }
        #endregion

        #endregion

        #region DisplayTitles
        ///<summary>
        /// Gets or sets the value indicator whether to display panels titles or not.
        ///</summary>
        public static readonly DependencyProperty DisplayTitlesProperty;
        ///<summary>
        /// Gets or sets the value indicator whether to display panels titles or not.
        ///</summary>
        public bool DisplayTitles
        {
            get { return (bool)GetValue(DisplayTitlesProperty); }
            set { SetValue(DisplayTitlesProperty, value); }
        }
        private static void OnDisplayTitlesChanged(DependencyObject sender, DependencyPropertyChangedEventArgs eventArgs)
        {
            StockChartX chartX = ((StockChartX)sender);
            foreach (ChartPanel chartPanel in chartX._panelsContainer.Panels)
            {
                chartPanel.ShowHideTitleBar();
            }
        }
        #endregion

        #region PanelMinHeight
        ///<summary>
        /// Gets or sets the minimum height of the chart panels
        ///</summary>
        public static readonly DependencyProperty PanelMinHeightProperty;
        ///<summary>
        /// Gets or sets the minimum height of the chart panels. Used so that panels do not dissapear.
        /// Useful to set to 0 when you actually want a panel to be hidden.
        ///</summary>
        public double PanelMinHeight
        {
            get { return (double)GetValue(PanelMinHeightProperty); }
            set { SetValue(PanelMinHeightProperty, value); }
        }
        private static void OnPanelMinHeightChanged(DependencyObject sender, DependencyPropertyChangedEventArgs eventArgs)
        {
            StockChartX stockChartX = ((StockChartX)sender);
            stockChartX._panelsContainer.PanelAllowedMinimumHeight = stockChartX.PanelMinHeight;
        }
        #endregion

        #region Indicator Dialog Properties

        #region IndicatorDialogBackground
        ///<summary>
        /// Gets or sets the <seealso cref="Brush"/> used as background for indicators dialog.
        ///</summary>
        public static readonly DependencyProperty IndicatorDialogBackgroundProperty;
        ///<summary>
        /// Gets or sets the <seealso cref="Brush"/> used as background for indicators dialog.
        ///</summary>
        public Brush IndicatorDialogBackground
        {
            get { return (Brush)GetValue(IndicatorDialogBackgroundProperty); }
            set { SetValue(IndicatorDialogBackgroundProperty, value); }
        }
        #endregion

        #region IndicatorDialogLabelForegroundProperty (DependencyProperty)
        /// <summary>
        /// IndicatorDialogLabelForeground
        /// </summary>
        public static readonly DependencyProperty IndicatorDialogLabelForegroundProperty;
        /// <summary>
        /// IndicatorDialogLabelForeground summary
        /// </summary>
        public Brush IndicatorDialogLabelForeground
        {
            get { return (Brush)GetValue(IndicatorDialogLabelForegroundProperty); }
            set { SetValue(IndicatorDialogLabelForegroundProperty, value); }
        }
        private static void OnIndicatorDialogLabelForegroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((StockChartX)d).OnIndicatorDialogLabelForegroundChanged(e);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnIndicatorDialogLabelForegroundChanged(DependencyPropertyChangedEventArgs e)
        {
        }
        #endregion

        #region IndicatorDialogLabelFontSizeProperty (DependencyProperty)
        /// <summary>
        /// IndicatorDialogLabelFontSize
        /// </summary>
        public static readonly DependencyProperty IndicatorDialogLabelFontSizeProperty;
        /// <summary>
        /// IndicatorDialogLabelFontSize summary
        /// </summary>
        public double IndicatorDialogLabelFontSize
        {
            get { return (double)GetValue(IndicatorDialogLabelFontSizeProperty); }
            set { SetValue(IndicatorDialogLabelFontSizeProperty, value); }
        }
        private static void OnIndicatorDialogLabelFontSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((StockChartX)d).OnIndicatorDialogLabelFontSizeChanged(e);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnIndicatorDialogLabelFontSizeChanged(DependencyPropertyChangedEventArgs e)
        {
        }
        #endregion

        #endregion

        #region IndicatorTwinTitleVisibility
        /// <summary>
        /// Property variable for <see cref="IndicatorTwinTitleVisibility"/>
        /// </summary>
        public static readonly DependencyProperty IndicatorTwinTitleVisibilityProperty;
        /// <summary>
        /// Gets or sets the <see cref="Visibility "/> state on the titles of twin indicators. Twin indicators are indicators that have 
        /// more than one line.
        /// If a user does not want all of the lines to have titles at the top of the panel this value should be set to collapsed in which
        /// case only the main series will have a title in the panel's title bar.
        /// NOTE: This property is NOT dynamic, which is to say that changing this property while twin titles are already on the screen
        /// will not remove them nor add them if they are currently collapsed. It is simply a flag that will affect any newly added
        /// indicators. Though there is an event <see cref="IndicatorTwinTitleVisibilityChanged"/> that will be fired if there is anything 
        /// else that needs to be done.
        /// </summary>
        public Visibility IndicatorTwinTitleVisibility
        {
            get { return (Visibility)GetValue(IndicatorTwinTitleVisibilityProperty); }
            set { SetValue(IndicatorTwinTitleVisibilityProperty, value); }
        }
        private static void OnIndicatorTwinTitleVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((StockChartX)d).IndicatorTwinTitleVisibilityChanged.Invoke(d, new EventArgs());
        }
        /// <summary>
        /// Event that is fired when <see cref="IndicatorTwinTitleVisibility"/> is changed.
        /// </summary>
        public event EventHandler IndicatorTwinTitleVisibilityChanged = delegate { };
        #endregion

        #region VolumePostfixLetter
        /// <summary>
        /// Gets or sets the postfix letter used on to display volume in millions (e.g. you want to show 5,200,000 as "5.2 M" in the Y scale). 
        /// </summary>
        public static readonly DependencyProperty VolumePostfixLetterProperty;
        /// <summary>
        /// Gets or sets the postfix letter used on to display volume in millions (e.g. you want to show 5,200,000 as "5.2 M" in the Y scale). 
        /// </summary>
        public string VolumePostfixLetter
        {
            get { return (string)GetValue(VolumePostfixLetterProperty); }
            set { SetValue(VolumePostfixLetterProperty, value); }
        }
        #endregion

        #region InfoPanel Properties

        #region InfoPanelLabelsBackgroundProperty
        ///<summary>
        /// Gets or sets the background <seealso cref="Brush"/> of infopanel's labels.
        ///</summary>
        public static readonly DependencyProperty InfoPanelLabelsBackgroundProperty;
        ///<summary>
        /// Gets or sets the background <seealso cref="Brush"/> of infopanel's labels.
        ///</summary>
        public Brush InfoPanelLabelsBackground
        {
            get { return (Brush)GetValue(InfoPanelLabelsBackgroundProperty); }
            set { SetValue(InfoPanelLabelsBackgroundProperty, value); }
        }
        private static void OnInfoPanelLabelsBackgroundChanged(DependencyObject sender, DependencyPropertyChangedEventArgs eventArgs)
        {
            StockChartX chartX = (StockChartX)sender;
            chartX._panelsContainer.EnforceInfoPanelUpdate();
        }
        #endregion

        #region InfoPanelLabelsForegroundProperty
        ///<summary>
        /// Gets or sets the foreground <seealso cref="Brush"/> of infopanel's labels.
        ///</summary>
        public static readonly DependencyProperty InfoPanelLabelsForegroundProperty;
        ///<summary>
        /// Gets or sets the foreground <seealso cref="Brush"/> of infopanel's labels.
        ///</summary>
        public Brush InfoPanelLabelsForeground
        {
            get { return (Brush)GetValue(InfoPanelLabelsForegroundProperty); }
            set { SetValue(InfoPanelLabelsForegroundProperty, value); }
        }
        private static void OnInfoPanelLabelsForegroundChanged(DependencyObject sender, DependencyPropertyChangedEventArgs eventArgs)
        {
            StockChartX chartX = (StockChartX)sender;
            chartX._panelsContainer.EnforceInfoPanelUpdate();
        }
        #endregion

        #region InfoPanelValuesBackgroundProperty
        ///<summary>
        /// Gets or sets the background <seealso cref="Brush"/> of infopanel's values.
        ///</summary>
        public static readonly DependencyProperty InfoPanelValuesBackgroundProperty;
        ///<summary>
        /// Gets or sets the background <seealso cref="Brush"/> of infopanel's values.
        ///</summary>
        public Brush InfoPanelValuesBackground
        {
            get { return (Brush)GetValue(InfoPanelValuesBackgroundProperty); }
            set { SetValue(InfoPanelValuesBackgroundProperty, value); }
        }
        private static void OnInfoPanelValuesBackgroundChanged(DependencyObject sender, DependencyPropertyChangedEventArgs eventArgs)
        {
            StockChartX chartX = (StockChartX)sender;
            chartX._panelsContainer.EnforceInfoPanelUpdate();
        }
        #endregion

        #region InfoPanelValuesForegroundProperty
        ///<summary>
        /// Gets or sets the foreground <seealso cref="Brush"/> of infopanel's values.
        ///</summary>
        public static readonly DependencyProperty InfoPanelValuesForegroundProperty;
        ///<summary>
        /// Gets or sets the foreground <seealso cref="Brush"/> of infopanel's values.
        ///</summary>
        public Brush InfoPanelValuesForeground
        {
            get { return (Brush)GetValue(InfoPanelValuesForegroundProperty); }
            set { SetValue(InfoPanelValuesForegroundProperty, value); }
        }
        private static void OnInfoPanelValuesForegroundChanged(DependencyObject sender, DependencyPropertyChangedEventArgs eventArgs)
        {
            StockChartX chartX = (StockChartX)sender;
            chartX._panelsContainer.EnforceInfoPanelUpdate();
        }
        #endregion

        #region InfoPanelFontFamilyProperty
        ///<summary>
        /// Gets or sets the preferred top-level font family for the textblock used in info panel.
        ///</summary>
        public static readonly DependencyProperty InfoPanelFontFamilyProperty;
        ///<summary>
        /// Gets or sets the preferred top-level font family for the textblock used in info panel.
        ///</summary>
        public FontFamily InfoPanelFontFamily
        {
            get { return (FontFamily)GetValue(InfoPanelFontFamilyProperty); }
            set { SetValue(InfoPanelFontFamilyProperty, value); }
        }
        private static void OnInfoPanelFontFamilyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs eventArgs)
        {
            StockChartX chartX = (StockChartX)sender;
            chartX._panelsContainer.EnforceInfoPanelUpdate();
        }
        #endregion

        #region InfoPanelFontSizeProperty
        ///<summary>
        /// Gets or sets the preferred top-level font size for the textblock used in info panel.
        ///</summary>
        public static readonly DependencyProperty InfoPanelFontSizeProperty;
        ///<summary>
        /// Gets or sets the preferred top-level font size for the textblock used in info panel.
        ///</summary>
        public double InfoPanelFontSize
        {
            get { return (double)GetValue(InfoPanelFontSizeProperty); }
            set { SetValue(InfoPanelFontSizeProperty, value); }
        }
        private static void OnInfoPanelFontSizeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs eventArgs)
        {
            StockChartX chartX = (StockChartX)sender;
            chartX._panelsContainer.EnforceInfoPanelUpdate();
        }
        #endregion

        #region InfoPanelPositionProperty
        ///<summary>
        /// Gets or sets the positioning type of info panel.
        ///</summary>
        public static readonly DependencyProperty InfoPanelPositionProperty;
        ///<summary>
        /// Gets or sets the positioning type of info panel.
        ///</summary>
        public InfoPanelPositionEnum InfoPanelPosition
        {
            get { return (InfoPanelPositionEnum)GetValue(InfoPanelPositionProperty); }
            set { SetValue(InfoPanelPositionProperty, value); }
        }
        private static void OnInfoPanelPositionChanged(DependencyObject sender, DependencyPropertyChangedEventArgs eventArgs)
        {
            StockChartX chartX = (StockChartX)sender;
            InfoPanelPositionEnum positionEnum = (InfoPanelPositionEnum)eventArgs.NewValue;
            if (positionEnum == InfoPanelPositionEnum.Hidden || positionEnum == InfoPanelPositionEnum.FollowMouse)
            {
                chartX._timers.StopTimerWork(TimerInfoPanel);
                chartX._panelsContainer.HideInfoPanel();
                return;
            }

            //      if (positionEnum == InfoPanelPositionEnum.FollowMouseAlwaysOn)
            //      {
            //        chartX._timers.StartTimerWork(TimerInfoPanel);
            //        return;
            //      }

            if (positionEnum != InfoPanelPositionEnum.FixedPosition) return;
            chartX._timers.StartTimerWork(TimerInfoPanel);
            chartX._panelsContainer.MakeInfoPanelStatic();
        }
        #endregion

        #endregion

        #region Heat Panel Labels Properties

        #region HeatPanelLabelsForegroundProperty
        ///<summary>
        /// Gets or sets the <seealso cref="Brush"/> to apply to labels in heat panel.
        ///</summary>
        public static readonly DependencyProperty HeatPanelLabelsForegroundProperty;
        ///<summary>
        /// Gets or sets the <seealso cref="Brush"/> to apply to labels in heat panel.
        ///</summary>
        public Brush HeatPanelLabelsForeground
        {
            get { return (Brush)GetValue(HeatPanelLabelsForegroundProperty); }
            set { SetValue(HeatPanelLabelsForegroundProperty, value); }
        }
        private static void OnHeatPanelLabelsForegroundChanged(DependencyObject sender, DependencyPropertyChangedEventArgs eventArgs)
        {
            StockChartX chartX = (StockChartX)sender;
            if (chartX._panelsContainer != null)
                chartX._panelsContainer.ResetHeatMapPanels();
        }
        #endregion

        #region HeatPanelLabelsBackgroundProperty
        ///<summary>
        /// Gets or sets the <seealso cref="Brush"/> to apply to labels background
        ///</summary>
        public static readonly DependencyProperty HeatPanelLabelsBackgroundProperty;
        ///<summary>
        /// Gets or sets the <seealso cref="Brush"/> to apply to labels background
        ///</summary>
        public Brush HeatPanelLabelsBackground
        {
            get { return (Brush)GetValue(HeatPanelLabelsBackgroundProperty); }
            set { SetValue(HeatPanelLabelsBackgroundProperty, value); }
        }
        private static void OnHeatPanelLabelsBackgroundChanged(DependencyObject sender, DependencyPropertyChangedEventArgs eventArgs)
        {
            StockChartX chartX = (StockChartX)sender;
            if (chartX._panelsContainer != null)
                chartX._panelsContainer.ResetHeatMapPanels();
        }
        #endregion

        #region HeatPanelLabelsFontSizeProperty
        ///<summary>
        /// Gets or sets the font-size for labels used in heat-panel
        ///</summary>
        public static readonly DependencyProperty HeatPanelLabelsFontSizeProperty;
        ///<summary>
        /// Gets or sets the font-size for labels used in heat-panel
        ///</summary>
        public double HeatPanelLabelsFontSize
        {
            get { return (double)GetValue(HeatPanelLabelsFontSizeProperty); }
            set { SetValue(HeatPanelLabelsFontSizeProperty, value); }
        }
        private static void OnHeatPanelLabelFontSizeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs eventArgs)
        {
            StockChartX chartX = (StockChartX)sender;
            if (chartX._panelsContainer != null)
                chartX._panelsContainer.ResetHeatMapPanels();
        }
        #endregion

        #endregion

        #region Chart Scroller Properties

        #region ChartScrollerVisibleProperty (DependencyProperty)
        /// <summary>
        /// IsChartScrollerVisible
        /// </summary>
        public static readonly DependencyProperty ChartScrollerVisibleProperty;
        /// <summary>
        /// A description of the IsChartScrollerVisible.
        /// </summary>
        public bool IsChartScrollerVisible
        {
            get { return (bool)GetValue(ChartScrollerVisibleProperty); }
            set { SetValue(ChartScrollerVisibleProperty, value); }
        }
        private static void OnChartScrollerVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((StockChartX)d).OnChartScrollerVisibleChanged(e);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnChartScrollerVisibleChanged(DependencyPropertyChangedEventArgs e)
        {
            ShowHideChartScroller();
        }
        #endregion

        #region ChartScrollerTrackBackgroundProperty (DependencyProperty)
        /// <summary>
        /// ChartScrollerTrackBackground
        /// </summary>
        public static readonly DependencyProperty ChartScrollerTrackBackgroundProperty;
        /// <summary>
        /// A description of the ChartScrollerTrackBackground.
        /// </summary>
        public Brush ChartScrollerTrackBackground
        {
            get { return (Brush)GetValue(ChartScrollerTrackBackgroundProperty); }
            set { SetValue(ChartScrollerTrackBackgroundProperty, value); }
        }
        private static void OnChartScrollerTrackBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((StockChartX)d).OnChartScrollerTrackBackgroundChanged(e);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnChartScrollerTrackBackgroundChanged(DependencyPropertyChangedEventArgs e)
        {
            //      if (_chartScroller != null)
            //        _chartScroller.TrackBackground = (Brush)e.NewValue;
        }
        #endregion

        #region ChartScrollerTrackButtonsBackgroundProperty (DependencyProperty)
        /// <summary>
        /// ChartScrollerTrackButtonsBackground
        /// </summary>
        public static readonly DependencyProperty ChartScrollerTrackButtonsBackgroundProperty;
        /// <summary>
        /// A description of the ChartScrollerTrackButtonsBackground.
        /// </summary>
        public Brush ChartScrollerTrackButtonsBackground
        {
            get { return (Brush)GetValue(ChartScrollerTrackButtonsBackgroundProperty); }
            set { SetValue(ChartScrollerTrackButtonsBackgroundProperty, value); }
        }
        private static void OnChartScrollerTrackButtonsBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((StockChartX)d).OnChartScrollerTrackButtonsBackgroundChanged(e);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnChartScrollerTrackButtonsBackgroundChanged(DependencyPropertyChangedEventArgs e)
        {
            //      if (_chartScroller != null)
            //        _chartScroller.TrackButtonsBackground = (Brush)e.NewValue;
        }
        #endregion

        #region ChartScrollerThumbButtonBackgroundProperty (DependencyProperty)
        /// <summary>
        /// ChartScrollerThumbButtonBackground
        /// </summary>
        public static readonly DependencyProperty ChartScrollerThumbButtonBackgroundProperty;
        /// <summary>
        /// A description of the ChartScrollerThumbButtonBackground.
        /// </summary>
        public Brush ChartScrollerThumbButtonBackground
        {
            get { return (Brush)GetValue(ChartScrollerThumbButtonBackgroundProperty); }
            set { SetValue(ChartScrollerThumbButtonBackgroundProperty, value); }
        }
        private static void OnChartScrollerThumbButtonBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((StockChartX)d).OnChartScrollerThumbButtonBackgroundChanged(e);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnChartScrollerThumbButtonBackgroundChanged(DependencyPropertyChangedEventArgs e)
        {
            //      if (_chartScroller != null)
            //        _chartScroller.ThumbButtonBackground = (Brush)e.NewValue;
        }
        #endregion

        #region ChartScrollerPropertiesProperty
        /// <summary>
        /// Identifies the <see cref="ChartScrollerProperties"/> dependency property 
        /// </summary>
        public static readonly DependencyProperty ChartScrollerPropertiesProperty;
        /// <summary>
        /// Gets or sets chart scroller properties. This is a dependency property.
        /// </summary>
        public ChartScrollerProperties ChartScrollerProperties
        {
            get { return (ChartScrollerProperties)GetValue(ChartScrollerPropertiesProperty); }
            set { SetValue(ChartScrollerPropertiesProperty, value); }
        }
        private static void ChartScrollerPropertiesChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            ((StockChartX)dependencyObject).SetupChartScroller((ChartScrollerProperties)args.NewValue);
        }
        #endregion

        #endregion

        #region AppendTickVolumeBehaviorProperty (DependencyProperty)
        /// <summary>
        /// AppendTickVolumeBehavior
        /// </summary>
        public static readonly DependencyProperty AppendTickVolumeBehaviorProperty;
        /// <summary>
        /// A description of the AppendTickVolumeBehavior.
        /// </summary>
        public DataManager.AppendTickVolumeBehavior AppendTickVolumeBehavior
        {
            get { return (DataManager.AppendTickVolumeBehavior)GetValue(AppendTickVolumeBehaviorProperty); }
            set { SetValue(AppendTickVolumeBehaviorProperty, value); }
        }
        private static void OnAppendTickVolumeBehaviorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((StockChartX)d).OnAppendTickVolumeBehaviorChanged(e);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnAppendTickVolumeBehaviorChanged(DependencyPropertyChangedEventArgs e)
        {
        }
        #endregion

        #region LineStudies dialog background
        ///<summary>
        /// Gets or sets the background for the dialog properties if LineStudy objects
        ///</summary>
        public static DependencyProperty LineStudyPropertyDialogBackgroundProperty;
        ///<summary>
        /// /// Gets or sets the background for the dialog properties if LineStudy objects
        ///</summary>
        public Brush LineStudyPropertyDialogBackground
        {
            get { return (Brush)GetValue(LineStudyPropertyDialogBackgroundProperty); }
            set { SetValue(LineStudyPropertyDialogBackgroundProperty, value); }
        }
        #endregion

        #region MaxVisibleRecordsProperty (DependencyProperty)
        /// <summary>
        /// MaxVisibleRecords
        /// </summary>
        public static readonly DependencyProperty MaxVisibleRecordsProperty;
        /// <summary>
        /// Gets or sets the maximum visible records in the chart.
        /// </summary>
        public int MaxVisibleRecords
        {
            get { return (int)GetValue(MaxVisibleRecordsProperty); }
            set { SetValue(MaxVisibleRecordsProperty, value); }
        }
        private static void OnMaxVisibleRecordsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((StockChartX)d).OnMaxVisibleRecordsChanged(e);
        }
        /// <summary>
        /// Method called when <see cref="MaxVisibleRecords"/> property changes
        /// </summary>
        /// <param name="e">Arguments</param>
        protected virtual void OnMaxVisibleRecordsChanged(DependencyPropertyChangedEventArgs e)
        {
            Update();
        }
        #endregion

        #region Calendar Properties

        #region Calendar Background
        ///<summary>
        ///</summary>
        public static readonly DependencyProperty CalendarBackgroundProperty;
        ///<summary>
        /// Gets or sets the calendar background
        ///</summary>
        public Brush CalendarBackground
        {
            get { return (Brush)GetValue(CalendarBackgroundProperty); }
            set { SetValue(CalendarBackgroundProperty, value); }
        }
        #endregion

        #region ShowSecondsProperty (DependencyProperty)
        /// <summary>
        /// ShowSeconds
        /// </summary>
        public static readonly DependencyProperty ShowSecondsProperty;
        /// <summary>
        /// Gets or sets whether to show the seconds in calendar panel. Does not have effect when 
        /// <see cref="RealTimeXLabels"/>=false
        /// NOTE: Only valid for Calendar Version 1
        /// </summary>
        public bool ShowSeconds
        {
            get { return (bool)GetValue(ShowSecondsProperty); }
            set { SetValue(ShowSecondsProperty, value); }
        }
        private static void OnShowSecondsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((StockChartX)d).OnShowSecondsChanged(e);
        }
        /// <summary>
        /// Method called when <see cref="ShowSeconds"/> property changes
        /// </summary>
        /// <param name="e">Arguments</param>
        protected virtual void OnShowSecondsChanged(DependencyPropertyChangedEventArgs e)
        {
            if (_calendar == null)
                return;
            _calendar.Paint();
        }
        #endregion

        #region CalendarVersion (DependencyProperty)
        /// <summary>
        /// CalendarVersion
        /// </summary>
        public static readonly DependencyProperty CalendarVersionProperty;
        /// <summary>
        /// </summary>
        public CalendarVersionType CalendarVersion
        {
            get { return (CalendarVersionType)GetValue(CalendarVersionProperty); }
            set { SetValue(CalendarVersionProperty, value); }
        }
        private static void OnCalendarVersionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((StockChartX)d).OnCalendarVersionChanged(e);
        }
        /// <summary>
        /// Method called when <see cref="CalendarVersion"/> property changes
        /// </summary>
        /// <param name="e">Arguments</param>
        protected virtual void OnCalendarVersionChanged(DependencyPropertyChangedEventArgs e)
        {
            if (_calendar == null)
                return;
            _calendar.Paint();
        }
        #endregion

        #region Calendar V2 Properties

        #region CalendarV2LabelGap (DependencyProperty)
        /// <summary>
        /// CalendarV2LabelGap
        /// </summary>
        public static readonly DependencyProperty CalendarV2LabelGapProperty;
        /// <summary>
        /// </summary>
        public double CalendarV2LabelGap
        {
            get { return (double)GetValue(CalendarV2LabelGapProperty); }
            set { SetValue(CalendarV2LabelGapProperty, value); }
        }
        private static void OnCalendarV2LabelGapChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((StockChartX)d).OnCalendarV2LabelGapChanged(e);
        }
        /// <summary>
        /// Method called when <see cref="CalendarVersion"/> property changes
        /// </summary>
        /// <param name="e">Arguments</param>
        protected virtual void OnCalendarV2LabelGapChanged(DependencyPropertyChangedEventArgs e)
        {
            if (_calendar == null)
                return;
            _calendar.LABLE_GAP = CalendarV2LabelGap;
            _calendar.Paint();
        }
        #endregion

        #region CalendarV2CalendarLabelBlockOutput (DependencyProperty)
        /// <summary>
        /// CalendarV2CalendarLabelBlockOutput
        /// </summary>
        public static readonly DependencyProperty CalendarV2CalendarLabelBlockOutputProperty;
        /// <summary>
        /// </summary>
        public CalendarLabelBlockOutputType CalendarV2CalendarLabelBlockOutput
        {
            get { return (CalendarLabelBlockOutputType)GetValue(CalendarV2CalendarLabelBlockOutputProperty); }
            set { SetValue(CalendarV2CalendarLabelBlockOutputProperty, value); }
        }
        private static void OnCalendarV2CalendarLabelBlockOutputChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((StockChartX)d).OnCalendarV2CalendarLabelBlockOutputChanged(e);
        }
        /// <summary>
        /// Method called when <see cref="CalendarVersion"/> property changes
        /// </summary>
        /// <param name="e">Arguments</param>
        protected virtual void OnCalendarV2CalendarLabelBlockOutputChanged(DependencyPropertyChangedEventArgs e)
        {
            if (_calendar == null)
                return;
            _calendar.CalendarLabelBlockOutput = CalendarV2CalendarLabelBlockOutput;
            _calendar.Paint();
        }
        #endregion

        #region CalendarV2CurrentTimeStamp (DependencyProperty)
        /// <summary>
        /// CalendarV2CurrentTimeStamp
        /// </summary>
        public static readonly DependencyProperty CalendarV2CurrentTimeStampProperty;
        /// <summary>
        /// </summary>
        public bool CalendarV2CurrentTimeStamp
        {
            get { return (bool)GetValue(CalendarV2CurrentTimeStampProperty); }
            set { SetValue(CalendarV2CurrentTimeStampProperty, value); }
        }
        private static void OnCalendarV2CurrentTimeStampChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((StockChartX)d).OnCalendarV2CurrentTimeStampChanged(e);
        }
        /// <summary>
        /// Method called when <see cref="CalendarV2CurrentTimeStamp"/> property changes
        /// </summary>
        /// <param name="e">Arguments</param>
        protected virtual void OnCalendarV2CurrentTimeStampChanged(DependencyPropertyChangedEventArgs e)
        {
            if (_calendar == null)
                return;
            _calendar.CurrentTimeStamp = CalendarV2CurrentTimeStamp;
            _calendar.Paint();
        }
        #endregion

        #endregion

        #endregion

        #region WPF Only Code
#if WPF
        private static void CurrentOnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs args)
        {
            ExceptionWindow exceptionWindow = new ExceptionWindow(args.Exception);
            exceptionWindow.ShowDialog();
            args.Handled = true;
        }
#endif
        #endregion

        #region SILVERLIGHT Only Code
#if SILVERLIGHT

        #region URILicenseKeyProperty

        /// <summary>
        /// Identifies the <see cref="URILicenseKey"/> dependency property 
        /// </summary>
        public static readonly DependencyProperty URILicenseKeyProperty;

        /// <summary>
        /// <para>Gets or sets chart URI License key.</para>
        /// <para>Users that have the Source code can choose not to use this propert and instead edit the <see cref="CheckRegistration"/> method.
        /// Otherwise, users should get a URI/URL license from ModulusFE for the specific hostname that the 
        /// control is going to be running on.</para>
        /// <para>Go to http://www.modulusfe.com/support/getlicense.asp to get your license.</para>
        /// </summary>
        public string URILicenseKey
        {
            get { return (string)GetValue(URILicenseKeyProperty); }
            set { SetValue(URILicenseKeyProperty, value); }
        }

        private static void OnURILicenseKeyPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs eventArgs)
        {
            StockChartX stockChartX = (StockChartX)sender;
            stockChartX.DecryptURILicenseKey();
        }

        private string _uriLicenseKey = "";
        void DecryptURILicenseKey()
        {
            if (URILicenseKey.Length > 0)
                _uriLicenseKey = Utils.Decrypt(URILicenseKey, "ModulusFE");
            else
                _uriLicenseKey = "";
        }

        #endregion

#endif
        #endregion

    }
}

