using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using ModulusFE.LineStudies;

namespace ModulusFE
{
    public partial class StockChartX
    {
        /// <summary>
        /// Occurs when indicator's dialog with parameters is shown
        /// </summary>
        public event EventHandler DialogShown;
        internal void FireOnDialogShown()
        {
            if (DialogShown != null)
            {
                DialogShown(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Provides data for the <see cref="DeleteSeries"/> event.
        /// </summary>
        public class DeleteSeriesEventArgs : EventArgs
        {
            /// <summary>
            /// Name of the series being removed
            /// </summary>
            public string RemovedSeries { get; private set; }
            /// <summary>
            /// Initializes a new instance of the <see cref="DeleteSeriesEventArgs"/> class.
            /// </summary>
            /// <param name="seriesName">Name of the series being removed</param>
            public DeleteSeriesEventArgs(string seriesName)
            {
                RemovedSeries = seriesName;
            }
        }

        /// <summary>
        /// Occurs when a series or indicator is removed from chart. No user interaction is possibly here
        /// </summary>
        public event EventHandler<DeleteSeriesEventArgs> DeleteSeries;
        internal void FireSeriesRemoved(string seriesName)
        {
            if (DeleteSeries != null)
                DeleteSeries(this, new DeleteSeriesEventArgs(seriesName));
        }

        /// <summary>
        /// Penetration type of a linestudy
        /// </summary>
        public enum TrendLinePenetrationEnum
        {
            /// <summary>
            /// penetration was above LineStudy
            /// </summary>
            Above,
            /// <summary>
            /// penetration was below LineStudy
            /// </summary>
            Below
        }
        /// <summary>
        /// Provides data for the <see cref="TrendLinePenetration"/> event.
        /// </summary>
        public class TrendLinePenetrationArgs : EventArgs
        {
            /// <summary>
            /// reference to the LineStudy that was penetrated
            /// </summary>
            public TrendLine TrendLine { get; private set; }
            /// <summary>
            /// penetration type
            /// </summary>
            public TrendLinePenetrationEnum TrendLinePenetrationType { get; private set; }
            /// <summary>
            /// reference to the Series that penetrated the LineStudy
            /// </summary>
            public Series Series { get; private set; }
            /// <summary>
            /// Initializes a new instance of the <see cref="TrendLinePenetrationArgs"/> class.
            /// </summary>
            /// <param name="trendLine">Reference to trendline being penetrated</param>
            /// <param name="trendLinePenetrationEnum">Penetration type</param>
            /// <param name="series">Reference to the Series that penetrated the trendline</param>
            public TrendLinePenetrationArgs(TrendLine trendLine,
              TrendLinePenetrationEnum trendLinePenetrationEnum,
              Series series)
            {
                TrendLine = trendLine;
                TrendLinePenetrationType = trendLinePenetrationEnum;
                Series = series;
            }
        }
        /// <summary>
        /// Occurs when a value from a series crosses a watchable trendline
        /// </summary>
        public event EventHandler<TrendLinePenetrationArgs> TrendLinePenetration;
        internal void FireTrendLinePenetration(TrendLine trendLine,
          TrendLinePenetrationEnum trendLinePenetrationEnum,
          Series series)
        {
            if (TrendLinePenetration != null)
                TrendLinePenetration(this,
                                     new TrendLinePenetrationArgs(trendLine, trendLinePenetrationEnum, series));
        }

        /// <summary>
        /// Occurs when entire chart was recreated
        /// </summary>
        public event EventHandler ChartReseted;
        internal void FireChartReseted()
        {
            if (ChartReseted != null)
                ChartReseted(this, EventArgs.Empty);
        }

        ///<summary>
        /// Provides data for the <see cref="SeriesRightClick"/> event.
        ///</summary>
        public class SeriesRightClickEventArgs : EventArgs
        {
            /// <summary>
            /// reference to series being clicked
            /// </summary>
            public Series Series { get; private set; }
            /// <summary>
            /// mouse position relative to ChartPanel where the Series is located
            /// </summary>
            public Point Position { get; private set; }
            /// <summary>
            /// Initializes a new instance of the <see cref="SeriesRightClickEventArgs"/> class.
            /// </summary>
            /// <param name="series">Reference to series</param>
            /// <param name="position">Mouse position</param>
            public SeriesRightClickEventArgs(Series series, Point position)
            {
                Series = series;
                Position = position;
            }
        }

        /// <summary>
        /// Occurs when a right click occurs on a series
        /// </summary>
        public event EventHandler<SeriesRightClickEventArgs> SeriesRightClick;
        internal void FireSeriesRightClick(Series series, Point p)
        {
            if (SeriesRightClick != null)
                SeriesRightClick(this, new SeriesRightClickEventArgs(series, p));
        }

        /// <summary>
        /// Provides data for the <see cref="LineStudyRightClick"/> event.
        /// </summary>
        public class LineStudiesRightClickEventArgs : EventArgs
        {
            /// <summary>
            /// reference to a line study being clicked
            /// </summary>
            public LineStudy LineStudy { get; private set; }
            /// <summary>
            /// position of the mouse relative the ChartPanel where the LineStudy is located
            /// </summary>                            
            public Point Position { get; private set; }
            /// <summary>
            /// Initializes a new instance of the <see cref="LineStudiesRightClickEventArgs"/> class.
            /// </summary>
            /// <param name="lineStudy">Reference to line study</param>
            /// <param name="position">Mouse position</param>
            public LineStudiesRightClickEventArgs(LineStudy lineStudy, Point position)
            {
                LineStudy = lineStudy;
                Position = position;
            }
        }
        /// <summary>
        /// Occurs when a LineStudy was right-clicked
        /// </summary>
        public event EventHandler<LineStudiesRightClickEventArgs> LineStudyRightClick;
        internal void FireLineStudyRightClick(LineStudy lineStudy, Point position)
        {
            if (LineStudyRightClick != null)
                LineStudyRightClick(this, new LineStudiesRightClickEventArgs(lineStudy, position));
        }

        /// <summary>
        /// Provides data for the <see cref="LineStudyDoubleClick"/> event.
        /// </summary>
        public class LineStudyMouseEventArgs : EventArgs
        {
            /// <summary>
            /// reference to the linestudies
            /// </summary>
            public LineStudy LineStudy { get; private set; }
            /// <summary>
            /// Initializes a new instance of the <see cref="LineStudyMouseEventArgs"/> class.
            /// </summary>
            /// <param name="lineStudy">Reference to line study</param>
            public LineStudyMouseEventArgs(LineStudy lineStudy)
            {
                LineStudy = lineStudy;
            }
        }
        /// <summary>
        /// Occurs when a LineStudy is double clicked
        /// </summary>
        public event EventHandler<LineStudyMouseEventArgs> LineStudyDoubleClick;
        internal void FireLineStudyDoubleClick(LineStudy lineStudy)
        {
            if (LineStudyDoubleClick != null)
                LineStudyDoubleClick(this, new LineStudyMouseEventArgs(lineStudy));
        }

        /// <summary>
        /// Occurs when a mouse click a line study
        /// </summary>
        public event EventHandler<LineStudyMouseEventArgs> LineStudyLeftClick;
        internal void FireLineStudyLeftClick(LineStudy lineStudy)
        {
            if (LineStudyLeftClick != null)
                LineStudyLeftClick(this, new LineStudyMouseEventArgs(lineStudy));
        }

        /// <summary>
        /// Provides data for the <see cref="ChartPanelMouseMoveArgs"/> event.
        /// </summary>
        public class ChartPanelMouseMoveArgs : EventArgs
        {
            /// <summary>
            /// Gets the panel index where mouse is moving. 0-based
            /// </summary>
            public int PanelIndex { get; private set; }
            /// <summary>
            /// Gets the mouse Y coordinate relative to the panel
            /// </summary>
            public double MouseY { get; private set; }
            /// <summary>
            /// Gets the mouse X coordinate relative to the panel
            /// </summary>
            public double MouseX { get; private set; }
            /// <summary>
            /// Gets the Y value from current mouse position
            /// </summary>
            public double Y { get; private set; }
            /// <summary>
            /// Gets the record number from current mouse position. It has the visible index of the bar,
            /// to get the actual index add <see cref="StockChartX.FirstVisibleRecord"/> to it.
            /// </summary>
            public int Record { get; private set; }

            /// <summary>
            /// ctor
            /// </summary>
            /// <param name="panelIndex"></param>
            /// <param name="mouseY"></param>
            /// <param name="mouseX"></param>
            /// <param name="y"></param>
            /// <param name="record"></param>
            public ChartPanelMouseMoveArgs(int panelIndex, double mouseY, double mouseX, double y, int record)
            {
                PanelIndex = panelIndex;
                MouseY = mouseY;
                MouseX = mouseX;
                Y = y;
                Record = record;
            }
        }
        /// <summary>
        /// Occurs when mouse is moving above a <see cref="ChartPanel"/>
        /// </summary>
        public event EventHandler<ChartPanelMouseMoveArgs> ChartPanelMouseMove;

        internal bool IsChartPanelMouseMoveHandled
        {
            get { return ChartPanelMouseMove != null; }
        }

        internal void InvokeChartPanelMouseMove(int panelIndex, double mouseY, double mouseX, double y, int record)
        {
            EventHandler<ChartPanelMouseMoveArgs> handler = ChartPanelMouseMove;
            if (handler != null)
            {
                handler(this, new ChartPanelMouseMoveArgs(panelIndex, mouseY, mouseX, y, record));
            }
        }

        /// <summary>
        /// Provides data for the <see cref="LineStudyBeforeDelete"/> event.
        /// </summary>
        public class LineStudyBeforeDeleteEventArgs : EventArgs
        {
            /// <summary>
            /// reference to LineStudy that is going to be deleted
            /// </summary>
            public LineStudy LineStudy;
            /// <summary>
            /// set to [true] to cancel LineStudies deleting
            /// </summary>
            public bool CancelDelete;
            /// <summary>
            /// Initializes a new instance of the <see cref="LineStudyBeforeDeleteEventArgs"/> class.
            /// </summary>
            /// <param name="lineStudy">Reference to a line study</param>
            public LineStudyBeforeDeleteEventArgs(LineStudy lineStudy)
            {
                LineStudy = lineStudy;
                CancelDelete = false;
            }
        }
        /// <summary>
        /// Occurs before a LineStudy is deleted. Here user may cancel LineStudy deletition
        /// </summary>
        public event EventHandler<LineStudyBeforeDeleteEventArgs> LineStudyBeforeDelete;
        internal bool FireLineStudyBeforeDelete(LineStudy lineStudy)
        {
            LineStudyBeforeDeleteEventArgs eventArgs = new LineStudyBeforeDeleteEventArgs(lineStudy);
            if (LineStudyBeforeDelete != null)
                LineStudyBeforeDelete(this, eventArgs);
            return eventArgs.CancelDelete;
        }

        /// <summary>
        /// Provides data for the <see cref="IndicatorDoubleClick"/> event.
        /// </summary>
        public class IndicatorDoubleClickEventArgs : EventArgs
        {
            /// <summary>
            /// a reference to the indicator being clicked
            /// </summary>
            public Indicators.Indicator Indicator { get; private set; }
            /// <summary>
            /// if set to true the double click won't show the indicator properties dialog
            /// </summary>
            public bool CancelPropertiesDialog { get; private set; }
            /// <summary>
            /// Initializes a new instance of the <see cref="IndicatorDoubleClickEventArgs"/> class.
            /// </summary>
            /// <param name="indicator">Reference to the indicator being double-clicked</param>
            public IndicatorDoubleClickEventArgs(Indicators.Indicator indicator)
            {
                Indicator = indicator;
                CancelPropertiesDialog = true;
            }
        }
        /// <summary>
        /// Occusr when a series-indicator is double clicked. Here user can cancel indicator's properties dialog.
        /// </summary>
        public event EventHandler<IndicatorDoubleClickEventArgs> IndicatorDoubleClick;
        internal bool FireIndicatorDoubleClick(Indicators.Indicator indicator)
        {
            IndicatorDoubleClickEventArgs args = new IndicatorDoubleClickEventArgs(indicator);
            if (IndicatorDoubleClick != null)
            {
                IndicatorDoubleClick(this, args);
            }

            return args.CancelPropertiesDialog;
        }

        /// <summary>
        /// Provides data for the <see cref="SeriesDoubleClick"/> event.
        /// </summary>
        public class SeriesDoubleClickEventArgs : EventArgs
        {
            /// <summary>
            /// reference to the series being double-clicked
            /// </summary>
            public Series Series { get; private set; }
            /// <summary>
            /// Initializes a new instance of the <see cref="SeriesDoubleClickEventArgs"/> class.
            /// </summary>
            /// <param name="series">Reference to series</param>
            public SeriesDoubleClickEventArgs(Series series)
            {
                Series = series;
            }
        }
        /// <summary>
        /// Occurs when a Series is double clicked
        /// </summary>
        public event EventHandler<SeriesDoubleClickEventArgs> SeriesDoubleClick;
        internal void FireSeriesDoubleClick(Series series)
        {
            if (SeriesDoubleClick != null)
                SeriesDoubleClick(this, new SeriesDoubleClickEventArgs(series));
        }

        /// <summary>
        /// Provides data for the <see cref="SeriesBeforeDelete"/> event.
        /// </summary>
        public class SeriesBeforeDeleteEventArgs : EventArgs
        {
            /// <summary>
            /// reference to the series that is going to be deleted
            /// </summary>
            public Series Series;
            /// <summary>
            /// set to [true] to cancel series deleting
            /// </summary>
            public bool CancelDelete;

            /// <summary>
            /// Initializes a new instance of the <see cref="SeriesBeforeDeleteEventArgs"/> class.
            /// </summary>
            /// <param name="series">Reference to series</param>
            public SeriesBeforeDeleteEventArgs(Series series)
            {
                Series = series;
                CancelDelete = false;
            }
        }
        /// <summary>
        /// Occurs before a Series is deleted. Here user can cancel Series deletition
        /// </summary>
        public event EventHandler<SeriesBeforeDeleteEventArgs> SeriesBeforeDelete;
        internal bool FireIndicatorBeforeDelete(Series series)
        {
            if (SeriesBeforeDelete != null)
            {
                SeriesBeforeDeleteEventArgs args = new SeriesBeforeDeleteEventArgs(series);
                SeriesBeforeDelete(this, args);
                return args.CancelDelete;
            }
            return false;
        }

        /// <summary>
        /// Provides data for the <see cref="SeriesMoved"/> event.
        /// </summary>
        public class SeriesMovedEventArgs : EventArgs
        {
            /// <summary>
            /// a reference to a series that was moved
            /// </summary>
            public Series Series { get; private set; }
            /// <summary>
            /// panel index where series was located. -1 if panel will be deleted after series move
            /// </summary>
            public int ChartPanelFrom { get; private set; }
            /// <summary>
            /// panel index where series is being moved. 
            /// </summary>
            public int ChartPanelTo { get; private set; }
            /// <summary>
            /// Initializes a new instance of the <see cref="SeriesMovedEventArgs"/> class.
            /// </summary>
            /// <param name="series">Referemce to series</param>
            /// <param name="chartPanelFrom">Index of panel from where series is moved</param>
            /// <param name="chartPanelTo">Index of panel where series is moved.</param>
            public SeriesMovedEventArgs(Series series, int chartPanelFrom, int chartPanelTo)
            {
                Series = series;
                ChartPanelFrom = chartPanelFrom;
                ChartPanelTo = chartPanelTo;
            }
        }
        /// <summary>
        /// Occurs whenever the user has moved a series or indicator from one panel to another panel. 
        /// </summary>
        public event EventHandler<SeriesMovedEventArgs> SeriesMoved;
        internal void FireSeriesMoved(Series series, int chartPanelFrom, int chartPanelTo)
        {
            if (SeriesMoved != null)
                SeriesMoved(this, new SeriesMovedEventArgs(series, chartPanelFrom, chartPanelTo));
        }

        /// <summary>
        /// Provides data for the <see cref="ChartPanelBeforeClose"/> event.
        /// </summary>
        public class ChartPanelBeforeCloseEventArgs : EventArgs
        {
            /// <summary>
            /// reference to the panel that is going to be closed
            /// </summary>
            public ChartPanel ChartPanel { get; private set; }
            /// <summary>
            /// set to [true] to cancel panel closing
            /// </summary>
            public bool CancelClose;

            /// <summary>
            /// Initializes a new instance of the <see cref="ChartPanelBeforeCloseEventArgs"/> class.
            /// </summary>
            /// <param name="chartPanel">Reference to chart panel</param>
            public ChartPanelBeforeCloseEventArgs(ChartPanel chartPanel)
            {
                ChartPanel = chartPanel;
                CancelClose = false;
            }
        }
        /// <summary>
        /// Occurs before a panel is closed. Here user can cancel panel closing.
        /// </summary>
        public event EventHandler<ChartPanelBeforeCloseEventArgs> ChartPanelBeforeClose;
        internal bool FireChartPanelBeforeClose(ChartPanel chartPanel)
        {
            ChartPanelBeforeCloseEventArgs args = new ChartPanelBeforeCloseEventArgs(chartPanel);
            if (ChartPanelBeforeClose != null)
            {
                ChartPanelBeforeClose(this, args);
                return args.CancelClose;
            }
            return args.CancelClose;
        }

        /// <summary>
        ///  Provides data for the <see cref="CustomIndicatorNeedsData"/> event.
        /// </summary>
        public class CustomIndicatorNeedsDataEventArgs : EventArgs
        {
            /// <summary>
            /// A reference to the custom indicator that needs data
            /// </summary>
            public Indicators.CustomIndicator Indicator { get; private set; }
            /// <summary>
            /// A reference to an array of values that must be filled by user.
            /// The length of this array can't be bigger than RecordCount, all those values will be ignored 
            /// When the event is fired this array will have the existing values, that can be overwritten
            /// </summary>
            public double?[] Values { get; set; }

            /// <summary>
            /// Initializes a new instance of the <see cref="CustomIndicatorNeedsDataEventArgs"/> class.
            /// </summary>
            /// <param name="indicator">Reference to the custom indicaror that needs data.</param>
            /// <param name="values">Values passed to user</param>
            public CustomIndicatorNeedsDataEventArgs(Indicators.CustomIndicator indicator, double?[] values)
            {
                Indicator = indicator;
                Values = values;
            }
        }

        /// <summary>
        /// Occurs whenever StockChartX updates with a new tick, new bar, or anything changes and the indicator needs to be re-calculated, you will be informed of this via the CustomIndicatorNeedData event. 
        /// </summary>
        public event EventHandler<CustomIndicatorNeedsDataEventArgs> CustomIndicatorNeedsData;

        internal bool CustomIndicatorNeedsDataIsHooked()
        {
            return CustomIndicatorNeedsData != null;
        }

        internal void FireCustomIndicatorNeedsData(CustomIndicatorNeedsDataEventArgs args)
        {
            if (CustomIndicatorNeedsData != null)
                CustomIndicatorNeedsData(this, args);
        }

        /// <summary>
        /// Provides data for the <see cref="ChartPanelPaint"/> event.
        /// </summary>
        public class ChartPanelPaintEventArgs : EventArgs
        {
            /// <summary>
            /// Reference to chart panel that was painted
            /// </summary>
            public ChartPanel ChartPanel { get; private set; }
            /// <summary>
            /// Initializes a new instance of the <see cref="ChartPanelPaintEventArgs"/> class.
            /// </summary>
            /// <param name="chartPanel">Reference to chart panel</param>
            public ChartPanelPaintEventArgs(ChartPanel chartPanel)
            {
                ChartPanel = chartPanel;
            }
        }

        /// <summary>
        /// Occurs each time a panel is repainted.
        /// </summary>
        public event EventHandler<ChartPanelPaintEventArgs> ChartPanelPaint;
        internal void FireChartPanelPaint(ChartPanel chartPanel)
        {
            if (ChartPanelPaint != null)
                ChartPanelPaint(this, new ChartPanelPaintEventArgs(chartPanel));
        }

        /// <summary>
        /// Occurs when chart is scrolled with the mouse wheel or programmatically.
        /// </summary>
        public event EventHandler ChartScroll;
        internal void FireChartScroll()
        {
            if (ChartScroll != null)
                ChartScroll(this, EventArgs.Empty);
        }

        /// <summary>
        /// Provides data for the <see cref="UserDrawingComplete"/> event.
        /// </summary>
        public class UserDrawingCompleteEventArgs : EventArgs
        {
            /// <summary>
            /// StudyType of the line study being drawn
            /// </summary>
            public LineStudy.StudyTypeEnum StudyType { get; private set; }
            /// <summary>
            /// The unique key of line study
            /// </summary>
            public string Key { get; private set; }

            /// <summary>
            /// Initializes a new instance of the <see cref="UserDrawingCompleteEventArgs"/> class.
            /// </summary>
            /// <param name="studyTypeEnum">Study type that was painted</param>
            /// <param name="key">Unique associated with line study</param>
            public UserDrawingCompleteEventArgs(LineStudy.StudyTypeEnum studyTypeEnum, string key)
            {
                StudyType = studyTypeEnum;
                Key = key;
            }
        }
        /// <summary>
        /// Occurs after the user has completed drawing a line study or trend line.
        /// </summary>
        public event EventHandler<UserDrawingCompleteEventArgs> UserDrawingComplete;
        internal void FireUserDrawingComplete(LineStudy.StudyTypeEnum studyTypeEnum, string key)
        {
            if (UserDrawingComplete != null)
                UserDrawingComplete(this, new UserDrawingCompleteEventArgs(studyTypeEnum, key));
        }

        /// <summary>
        /// Provides data for the <see cref="ShowInfoPanel"/> event.
        /// </summary>
        public class ShowInfoPanelEventArgs : EventArgs
        {
            /// <summary>
            /// Has entries shown on info panel
            /// </summary>
            public List<Tuple<string, string>> Entries { get; private set; }
            /// <summary>
            /// Initializes a new instance of the <see cref="ShowInfoPanelEventArgs"/> class.
            /// </summary>
            /// <param name="entries">Reference to a series that are present in info panel.</param>
            public ShowInfoPanelEventArgs(List<Tuple<string, string>> entries)
            {
                Entries = entries;
            }
        }
        /// <summary>
        /// Occurs when the info panel is displayed.
        /// </summary>
        public event EventHandler<ShowInfoPanelEventArgs> ShowInfoPanel;
        internal void FireShowInfoPanel()
        {
            if (ShowInfoPanel != null)
                ShowInfoPanel(this, new ShowInfoPanelEventArgs(_panelsContainer._infoPanel.Entries));
        }

        /// <summary>
        /// Occurs anytime the chart is zoomed
        /// </summary>
        public event EventHandler ChartZoom;
        internal void FireZoom()
        {
            if (ChartZoom != null)
                ChartZoom(this, EventArgs.Empty);

            OnPropertyChanged(Property_Zoomed);
        }

        /// <summary>
        /// internal usage
        /// </summary>
        public event EventHandler ChartLoaded = delegate { };

        /// <summary>
        /// Provides data for <see cref="IndicatorAddComplete"/> event.
        /// </summary>
        public class IndicatorAddCompletedEventArgs : EventArgs
        {
            /// <summary>
            /// Panel index where indicator was/is required to be
            /// </summary>
            public int PanelIndex { get; private set; }
            /// <summary>
            /// Indicator name
            /// </summary>
            public string IndicatorName { get; private set; }
            /// <summary>
            /// Gets the value indicating if action was canceled by user or not. 
            /// </summary>
            public bool CanceledByUser { get; private set; }
            ///<summary>
            /// Initializes a new instance of the <see cref="IndicatorAddCompletedEventArgs"/> class.
            ///</summary>
            ///<param name="panelIndex">Panel index</param>
            ///<param name="indicatorName">Indicator name</param>
            ///<param name="canceledByUser">Was operation canceled by user or not</param>
            public IndicatorAddCompletedEventArgs(int panelIndex, string indicatorName, bool canceledByUser)
            {
                PanelIndex = panelIndex;
                IndicatorName = indicatorName;
                CanceledByUser = canceledByUser;
            }
        }

        /// <summary>
        /// Occurs after an attempt to add an indicator.
        /// </summary>
        public event EventHandler<IndicatorAddCompletedEventArgs> IndicatorAddComplete = delegate { };

        internal void FireIndicatorAddCompleted(int panelIndex, string indicatorName, bool userCanceled)
        {
            IndicatorAddComplete(this, new IndicatorAddCompletedEventArgs(panelIndex, indicatorName, userCanceled));
        }

        internal delegate void OnCandleCustomBrushHandler(string seriesName, int barIndex, Brush newBrush);

        internal event OnCandleCustomBrushHandler OnCandleCustomBrush;

        /// <summary>
        /// Provides data for <see cref="StockChartX.LineStudyContextMenu"/> event.
        /// </summary>
        public class LineStudyContextMenuEventArgs : EventArgs
        {
            /// <summary>
            /// Gets the reference who needs a context menu
            /// </summary>
            public LineStudy LineStudy { get; private set; }

            /// <summary>
            /// Gets or sets whether built-in context menu will be shown
            /// </summary>
            public bool Cancel { get; set; }
            /// <summary>
            /// Initializes a new instance of the <see cref="LineStudyContextMenuEventArgs"/> class.
            /// </summary>
            /// <param name="lineStudy"></param>
            public LineStudyContextMenuEventArgs(LineStudy lineStudy)
            {
                LineStudy = lineStudy;
            }
        }

        /// <summary>
        /// Occurs when user clicks a <see cref="LineStudy"/> context-menu line
        /// </summary>
        public event EventHandler<LineStudyContextMenuEventArgs> LineStudyContextMenu;

        internal bool InvokeLineStudyContextMenu(LineStudy lineStudy)
        {
            EventHandler<LineStudyContextMenuEventArgs> menu = LineStudyContextMenu;
            var args = new LineStudyContextMenuEventArgs(lineStudy) { Cancel = false };
            if (menu != null)
            {
                menu(this, args);
            }

            return args.Cancel;
        }

        ///<summary>
        /// Provider data for Drag &amp; Drop operations on LineStudies
        ///</summary>
        public class DragDropLineStudyEventArgs : EventArgs
        {
            ///<summary>
            /// Reference to a <see cref="LineStudy"/> that is being drag &amp; droped
            ///</summary>
            public LineStudy LineStudy { get; private set; }
            ///<summary>
            /// Determines the type of current DD operation
            ///</summary>
            public enum DragDropActionType
            {
                ///<summary>
                /// Operations just started
                ///</summary>
                Started,
                ///<summary>
                /// LineStudy is being moved
                ///</summary>
                Moving,
                ///<summary>
                /// DD Operation has ended
                ///</summary>
                Ended
            }
            /// <summary>
            /// Gets the current DD action type
            /// </summary>
            public DragDropActionType ActionType { get; private set; }
            /// <summary>
            /// Gets whether LineStudy supports DD cancelation
            /// </summary>
            public bool CanCancelAction { get; private set; }
            /// <summary>
            /// Gets or sets DD action
            /// </summary>
            public bool CancelAction { get; set; }

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="lineStudy"></param>
            /// <param name="actionType"></param>
            /// <param name="canCancelAction"></param>
            public DragDropLineStudyEventArgs(LineStudy lineStudy, DragDropActionType actionType, bool canCancelAction)
            {
                LineStudy = lineStudy;
                ActionType = actionType;
                CanCancelAction = canCancelAction;
            }
        }

        /// <summary>
        /// Occurs when user initiated DD operation
        /// </summary>
        public event EventHandler<DragDropLineStudyEventArgs> DragDropStarted;

        internal void InvokeDragDropStarted(DragDropLineStudyEventArgs e)
        {
            EventHandler<DragDropLineStudyEventArgs> handler = DragDropStarted;
            if (handler != null) handler(this, e);
        }

        /// <summary>
        /// Occurs when user is moving a LineStudy
        /// </summary>
        public event EventHandler<DragDropLineStudyEventArgs> DragDropMoving;

        internal void InvokeDragDropMoving(DragDropLineStudyEventArgs e)
        {
            EventHandler<DragDropLineStudyEventArgs> handler = DragDropMoving;
            if (handler != null) handler(this, e);
        }

        /// <summary>
        /// Occurs when DD has been ended
        /// </summary>
        public event EventHandler<DragDropLineStudyEventArgs> DragDropEnded;

        internal void InvokeDragDropEnded(DragDropLineStudyEventArgs e)
        {
            EventHandler<DragDropLineStudyEventArgs> handler = DragDropEnded;
            if (handler != null) handler(this, e);
        }

        /// <summary>
        /// Provides data for <see cref="StockChartX.ChartPanelMouseLeftClick"/> event.
        /// </summary>
        public class ChartPanelMouseLeftClickEventArgs : EventArgs
        {
            /// <summary>
            /// Gets the reference to the panel where mouse was clicked
            /// </summary>
            public ChartPanel Panel { get; private set; }
            /// <summary>
            /// Gets the mouse X coordinate
            /// </summary>
            public double X { get; private set; }
            /// <summary>
            /// Gets the mouse Y coordinate. 
            /// </summary>
            public double Y { get; private set; }
            /// <summary>
            /// Gets the Y-axis price value that corresponds to mouse position. 
            /// </summary>
            public double Price { get; private set; }
            /// <summary>
            /// Gets the X-axis Timestamp value that corresponds to mouse position
            /// </summary>
            public DateTime? Timestamp { get; private set; }

            ///<summary>
            /// Ctor  
            ///</summary>
            ///<param name="panel"></param>
            ///<param name="x"></param>
            ///<param name="y"></param>
            ///<param name="price"></param>
            ///<param name="timestamp"></param>
            public ChartPanelMouseLeftClickEventArgs(ChartPanel panel, double x, double y, double price, DateTime? timestamp)
            {
                Panel = panel;
                X = x;
                Y = y;
                Price = price;
                Timestamp = timestamp;
            }
        }

        /// <summary>
        /// Occurs when user makes a left-click on the chart panel.
        /// </summary>
        public event EventHandler<ChartPanelMouseLeftClickEventArgs> ChartPanelMouseLeftClick;

        internal void InvokeChartPanelMouseLeftClick(ChartPanelMouseLeftClickEventArgs e)
        {
            EventHandler<ChartPanelMouseLeftClickEventArgs> handler = ChartPanelMouseLeftClick;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// A class that provides data for <see cref="LineStudyCreated"/> event.
        /// </summary>
        public class LineStudyCreatedEventArgs : EventArgs
        {
            /// <summary>
            /// A reference to LineStudy
            /// </summary>
            public LineStudy LineStudy { get; private set; }

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="lineStudy"></param>
            public LineStudyCreatedEventArgs(LineStudy lineStudy)
            {
                LineStudy = lineStudy;
            }
        }

        /// <summary>
        /// Occurs when a LineStudy was fully created
        /// </summary>
        public event EventHandler<LineStudyCreatedEventArgs> LineStudyCreated;

        internal void InvokeLineStudyCreated(LineStudyCreatedEventArgs e)
        {
            EventHandler<LineStudyCreatedEventArgs> handler = LineStudyCreated;
            if (handler != null) handler(this, e);
        }

        /// <summary>
        /// A class that provides data for <see cref="ChartPanelStatusChanged"/> event
        /// </summary>
        public class ChartPanelStatusChangedEventArgs : EventArgs
        {
            /// <summary>
            /// Old status
            /// </summary>
            public ChartStatus OldStatus { get; private set; }

            /// <summary>
            /// New status
            /// </summary>
            public ChartStatus NewStatus { get; private set; }

            /// <summary>
            /// A reference to ChartPanel
            /// </summary>
            public ChartPanel ChartPanel { get; private set; }

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="chartPanel"></param>
            /// <param name="oldStatus"></param>
            /// <param name="newStatus"></param>
            public ChartPanelStatusChangedEventArgs(ChartPanel chartPanel, ChartStatus oldStatus, ChartStatus newStatus)
            {
                ChartPanel = chartPanel;
                OldStatus = oldStatus;
                NewStatus = newStatus;
            }
        }

        /// <summary>
        /// Occurs when the status of a panel was changed.
        /// </summary>
        public event EventHandler<ChartPanelStatusChangedEventArgs> ChartPanelStatusChanged;

        internal void InvokeChartPanelStatusChanged(ChartPanelStatusChangedEventArgs e)
        {
            EventHandler<ChartPanelStatusChangedEventArgs> handler = ChartPanelStatusChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Occurs when charts is actually update after <see cref="Update"/> method was called.
        /// The way chart is done, the call of <see cref="Update"/>  method does not make chart update
        /// immediatly. Instead a timer is set that after a short period of time will update the chart.
        /// </summary>
        public event EventHandler ChartUpdated;

        internal void InvokeChartUpdated(EventArgs e)
        {
            EventHandler handler = ChartUpdated;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// A class that provides data for <see cref="StockChartX.BeforeZoom"/> event
        /// </summary>
        public class BeforeZoomEventArgs : EventArgs
        {
            /// <summary>
            /// Gets the zoom area start index
            /// </summary>
            public int StartIndex { get; private set; }

            /// <summary>
            /// Gets the zoom area end index
            /// </summary>
            public int EndIndex { get; private set; }

            /// <summary>
            /// Gets or sets whether the zoom operation needs to be canceled
            /// </summary>
            public bool Cancel { get; set; }

            /// <summary>
            /// ctor
            /// </summary>
            /// <param name="startIndex"></param>
            /// <param name="endIndex"></param>
            public BeforeZoomEventArgs(int startIndex, int endIndex)
            {
                StartIndex = startIndex;
                EndIndex = endIndex;
                Cancel = false;
            }
        }

        /// <summary>
        /// Occurs before chart is going to be zoomed
        /// </summary>
        public event EventHandler<BeforeZoomEventArgs> BeforeZoom;

        internal void InvokeBeforeZoom(BeforeZoomEventArgs e)
        {
            EventHandler<BeforeZoomEventArgs> handler = BeforeZoom;
            if (handler != null) handler(this, e);
        }
    }
}

