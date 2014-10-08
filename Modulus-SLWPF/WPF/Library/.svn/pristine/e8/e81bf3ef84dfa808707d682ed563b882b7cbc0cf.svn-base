using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

#if WPF
using System.Windows.Threading;
using Label=ModulusFE.PaintObjects.Label;
#endif
using ModulusFE.PaintObjects;

#if SILVERLIGHT
using ModulusFE.SL;
using ModulusFE.SL.Utils;
#endif


namespace ModulusFE
{
    ///<summary>
    ///</summary>
    public partial class Calendar : Canvas, IInfoPanelAble
    {
        private const string TimerRepaint = "TimerRepaint";
        internal StockChartX _chartX;

        private readonly ChartTimers _timerRepaint = new ChartTimers();
        private readonly PaintObjectsManager<Label> _labels = new PaintObjectsManager<Label>();
        private Path _linesPath;

#if DEMO
		private Label _labelDemo;
		internal string _demoText;
#endif
        ///<summary>
        ///</summary>
        public Calendar()
        {
            _timerRepaint = new ChartTimers();
            _timerRepaint.RegisterTimer(TimerRepaint, () => Dispatcher.BeginInvoke(
#if WPF
					DispatcherPriority.Normal, 
#endif
new Action(Paint)), 50);
            SizeChanged += (sender, e) => _timerRepaint.StartTimerWork(TimerRepaint);
#if SILVERLIGHT
            Mouse.RegisterMouseMoveAbleElement(this);
            MouseMove += (sender, e) => Mouse.UpdateMousePosition(this, e.GetPosition(this));
#endif

            _labels.NewObjectCreated += label =>
            {
                label._textBlock.Foreground = _chartX.FontForeground;
                label._textBlock.FontSize = _chartX.FontSize;
                label._textBlock.FontFamily = _chartX.FontFamily;
            };
        }

        internal DataManager.DataManager DM
        {
            get { return _chartX._dataManager; }
        }

        internal void UpdateFontInformation()
        {
            _labels.Do(label =>
                                    {
                                        label._textBlock.Foreground = _chartX.FontForeground;
                                        label._textBlock.FontSize = _chartX.FontSize;
                                        label._textBlock.FontFamily = _chartX.FontFamily;
                                    });
        }

        private bool _painting;
        internal void Paint()
        {
            if (_chartX.CalendarVersion == CalendarVersionType.Version1)
                PaintVersion1();
            else if (_chartX.CalendarVersion == CalendarVersionType.Version2)
                PaintVersion2();
        }

        #region Paint VERSION 1

        private void PaintVersion1()
        {
            try
            {
                if (_painting) return;

                _painting = true;

                _timerRepaint.StopTimerWork(TimerRepaint);
#if WPF
#if DEMO
			if (!string.IsNullOrEmpty(_demoText) && _labelDemo == null)
			{
				_labelDemo = new Label();
				_labelDemo.AddTo(this);
				_labelDemo.Left = 10;
				_labelDemo.Top = 2;
//        _labelDemo._textBlock.Opacity = 0.7;
				_labelDemo._textBlock.Foreground = Brushes.Red;
				_labelDemo.Text = _demoText;
				_labelDemo._textBlock.FontSize = 16;
				_labelDemo.ZIndex = 100;
			}
#endif
#endif
                Rect rcBounds = new Rect(0, 0, ActualWidth, ActualHeight);

                if (_linesPath == null)
                {
                    _linesPath = new Path
                                                 {
                                                     Stroke = _chartX.GridStroke,
                                                     StrokeThickness = 1,
                                                 };
                    Children.Add(_linesPath);
                }

                //Background = _chartX.Background;
                //_lines.C = this;
                _labels.C = this;
                //_lines.Start();
                _labels.Start();

                int startIndex = _chartX._startIndex;
                GeometryGroup lines = new GeometryGroup();

                //Utils.DrawLine(rcBounds.Left, 0, rcBounds.Right, 0, _chartX.GridStroke, LinePattern.Solid, 1, _lines);
                lines.Children.Add(new LineGeometry
                                                         {
                                                             StartPoint = new Point(rcBounds.Left, 0),
                                                             EndPoint = new Point(rcBounds.Right, 0),
                                                         });

                int rcnt = _chartX.VisibleRecordCount;
                double periodPixels = _chartX.GetXPixel(rcnt) / rcnt;
                if (periodPixels < 1)
                    periodPixels = 1;

                _chartX._xGridMap.Clear();
                double tradeWeek = periodPixels * 5; // 5 trading days in a week (avg)
                double tradeMonth = periodPixels * 20; // 20 trading days in a month (avg)
                double tradeYear = periodPixels * 253; // 253 trading days in a year (avg)  

                double averageCharWidth = _chartX.GetTextWidth("0");

                // Level 1:
                // YYYY
                double level1 = averageCharWidth * 4;

                // Level 2:
                // YY F M A M J J A S O N D
                double level2 = averageCharWidth * 2;

                // Level 3:
                // YY Feb Mar Apr May Jun Jul Aug Sep Oct Nov Dec
                double level3 = averageCharWidth * 3;

                // Level 4:
                // YYYY February March April May June July August September October November December
                double level4 = averageCharWidth * 9;

                // Level 5:
                // From -5 periods on right end, begin:
                // Jan DD  Feb DD  Mar DD  Apr DD  May DD  Jun DD  Jul DD  Aug DD  Sep DD  Oct DD  Nov DD  Dec DD
                double level5 = averageCharWidth * 6;

                // Level 6
                // Jan DD HH:MM
                double level6 = averageCharWidth * 10;

                double incr;
                int xGrid = 0;
                double x, lx = 0;

                if (_chartX.RealTimeXLabels)
                {
                    string prevDay = "";

                    incr = level6;
                    string timeFormat = "HH:mm";
                    if (_chartX.ShowSeconds)
                    {
                        incr += averageCharWidth * 2;
                        timeFormat = "HH:mm:ss";
                    }

                    for (int period = 0; period < rcnt; period++)
                    {
                        x = _chartX.GetXPixel(period);
                        if (x == lx) continue;
                        DateTime dDate = DM.GetTimeStampByIndex(period + startIndex);
                        //if (dDate.Minute % 15 != 0) continue;
                        if (incr > level6)
                        {
                            incr = 0;
                            //Draw vertical line
                            //_renderDevice.PlotUnitSeparator((float)x, true, 0);
                            //Utils.DrawLine(x, 0, x, rcBounds.Height / 2, _chartX.GridStroke, LinePattern.Solid, 1, _lines);
                            lines.Children.Add(new LineGeometry
                                                                     {
                                                                         StartPoint = new Point(x, 0),
                                                                         EndPoint = new Point(x, rcBounds.Height / 2)
                                                                     });

                            string szTime = dDate.ToString(timeFormat);
                            string szDay = dDate.ToString("dd");
                            string szMonth = dDate.ToString("MMM");

                            string szDate;
                            if (prevDay != szDay)
                            {
                                prevDay = szDay;
                                szDate = szMonth + " " + szDay + " " + szTime;
                                level6 = averageCharWidth * 12;
                                lx += level6 / 2;
                            }
                            else
                            {
                                szDate = szTime;
                            }

                            //_renderDevice.PlotUnitText((float)x, szDate, 0);
                            var lb = _labels.GetPaintObject();
                            lb.Left = x;
                            lb.Top = 1;
                            lb.Text = szDate;
                            //Utils.DrawText(x, 1, szDate, _chartX.FontForeground, _chartX.FontSize, _chartX.FontFamily, _labels);
                            _chartX._xGridMap[xGrid++] = x;
                        }

                        incr += x - lx;
                        lx = x;
                    }

                    _painting = false;

                    _linesPath.Data = (System.Windows.Media.Geometry)lines.GetAsFrozen();
                    //_lines.Stop();
                    _labels.Stop();
                    return;
                }

                lx = 0;
                double oldX = -1;
                string sCache = "#";
                string sDate;
                DateTime timestamp;
                DateTime? prevDate = null;
                if (level5 <= tradeWeek)
                {
                    incr = level5;
                    for (int period = 0; period < rcnt; period++)
                    {
                        x = _chartX.GetXPixel(period);
                        timestamp = DM.GetTimeStampByIndex(period + startIndex);

                        if (prevDate.HasValue && prevDate.Value.Year != timestamp.Year)
                            sDate = timestamp.ToString("yyyy MMM");
                        else
                            sDate = timestamp.ToString("dd MMM");

                        prevDate = timestamp;

                        if (incr > level5 && sCache != sDate && oldX != x)
                        {
                            incr = 0; //Reset

                            lines.Children.Add(new LineGeometry
                                                                     {
                                                                         StartPoint = new Point(x, 0),
                                                                         EndPoint = new Point(x, rcBounds.Height / 2)
                                                                     });

                            var lb = _labels.GetPaintObject();
                            lb.Left = x + 2;
                            lb.Top = 1;
                            lb.Text = sDate;

                            sCache = sDate;
                            oldX = x;
                            _chartX._xGridMap[xGrid++] = x;
                        }
                        incr += (x - lx);
                        lx = x;

                    }
                }
                else if (level4 <= tradeMonth)
                {
                    incr = level4;
                    for (int period = 0; period < rcnt; period++)
                    {
                        x = _chartX.GetXPixel(period);

                        timestamp = DM.GetTimeStampByIndex(period + startIndex);
                        sDate = timestamp.ToString("MMMM");
                        if (timestamp.Month == 1)
                            sDate = timestamp.ToString("yyyy MMM");

                        if (incr > level4 && sDate != sCache)
                        {
                            incr = 0;
                            lines.Children.Add(new LineGeometry
                                                                     {
                                                                         StartPoint = new Point(x, 0),
                                                                         EndPoint = new Point(x, rcBounds.Height / 2)
                                                                     });

                            var lb = _labels.GetPaintObject();
                            lb.Left = x;
                            lb.Top = 1;
                            lb.Text = sDate;

                            xGrid++;
                        }
                        sCache = sDate;
                        incr += (x - lx);
                        lx = x;

                        _chartX._xGridMap[xGrid] = x;
                    }
                }
                else if (level3 + 2 <= tradeMonth)
                {
                    incr = level3;
                    sCache = "#";
                    for (int period = 0; period < rcnt; period++)
                    {
                        x = _chartX.GetXPixel(period);

                        timestamp = DM.GetTimeStampByIndex(period + startIndex);

                        sDate = timestamp.ToString("MMM");
                        if (timestamp.Month == 1)
                            sDate = timestamp.ToString("yy");

                        if (incr > level3 && sCache != sDate)
                        {
                            incr = 0;
                            lines.Children.Add(new LineGeometry
                                                                     {
                                                                         StartPoint = new Point(x, 0),
                                                                         EndPoint = new Point(x, rcBounds.Height / 2)
                                                                     });

                            var lb = _labels.GetPaintObject();
                            lb.Left = x;
                            lb.Top = 1;
                            lb.Text = sDate;

                            xGrid++;
                        }
                        sCache = sDate;
                        incr += (x - lx);
                        lx = x;

                        _chartX._xGridMap[xGrid] = x;
                    }
                }
                else if (level2 <= tradeMonth)
                {
                    incr = level2;
                    sCache = "#";
                    for (int period = 0; period < rcnt; period++)
                    {
                        x = _chartX.GetXPixel(period);

                        timestamp = DM.GetTimeStampByIndex(period + startIndex);
                        sDate = timestamp.ToString("MMM");
                        string sTemp;
                        if (timestamp.Month == 1)
                        {
                            sDate = timestamp.ToString("yy");
                            sTemp = sDate;
                        }
                        else
                        {
                            sTemp = sDate.Substring(0, 1);
                        }
                        if (incr > level2 && sCache != sDate)
                        {
                            incr = 0;

                            lines.Children.Add(new LineGeometry
                                                                     {
                                                                         StartPoint = new Point(x, 0),
                                                                         EndPoint = new Point(x, rcBounds.Height / 2)
                                                                     });

                            var lb = _labels.GetPaintObject();
                            lb.Left = x;
                            lb.Top = 1;
                            lb.Text = sTemp;

                            xGrid++;
                        }
                        sCache = sDate;
                        incr += (x - lx);
                        lx = x;

                        _chartX._xGridMap[xGrid] = x;
                    }
                }
                else if (level1 <= tradeYear)
                {
                    incr = level1;
                    sCache = "#";
                    for (int period = 0; period < rcnt; period++)
                    {
                        x = _chartX.GetXPixel(period);
                        if (x == -1) break;

                        timestamp = DM.GetTimeStampByIndex(period + startIndex);
                        sDate = timestamp.ToString("yyyy");

                        if (incr > level1 && sDate != sCache)
                        {
                            incr = 0;

                            lines.Children.Add(new LineGeometry
                                                                     {
                                                                         StartPoint = new Point(x, 0),
                                                                         EndPoint = new Point(x, rcBounds.Height / 2)
                                                                     });

                            var lb = _labels.GetPaintObject();
                            lb.Left = x;
                            lb.Top = 1;
                            lb.Text = sDate;

                            xGrid++;
                        }
                        sCache = sDate;
                        incr += (x - lx);
                        lx = x;

                        _chartX._xGridMap[xGrid] = x;
                    }
                }

                //_lines.Stop();
                _linesPath.Data = (System.Windows.Media.Geometry)lines.GetAsFrozen();
                _labels.Stop();
            }
            finally
            {
                _painting = false;

                //after calendar is painted must instruct each panel to repaint the X Grid if needed
                if (_chartX.XGrid)
                {
                    foreach (var panel in _chartX.PanelsCollection)
                    {
                        panel.PaintXGrid();
                    }
                }
            }
        }

        #endregion Paint VERSION 1

        #region Paint VERSION 2

        /// <summary>
        /// A date that was carfully chosen to be unusually long in most formats. This will be used as
        /// the default test date for how much space is going to be used for output.
        /// The DateTime chosen is Wednesday, December 30th, 2009 at 12:50:50 pm.
        /// Notes: September is slightly longer textually than December, but 12 will be longer than 8 in numeric 
        /// format. Also. an hour in the 20-23 range will be longer than 12, but this is only true in the 24h
        /// format, which is not used all that much.
        /// </summary>
        private readonly DateTime TEST_DATE_TIME = new DateTime(2009, 12, 30, 12, 50, 50, DateTimeKind.Local);

        /// <summary>
        /// The amount of space that must extist between lables so that each lable is distinct from the next.
        /// This value can be set to just about anything, though 3.0 works best.
        /// This value could be made public.
        /// </summary>
        internal double LABLE_GAP = 3.0;

        /// <summary>
        /// Just a boolean at the moment, but should probably be an enumeration. This flag chooses which 
        /// datestamp to use for the output. The beginning of the calendar block or the first valid 
        /// data point in the block. Both are stored in the CalendarOutputData object.
        /// This value could be made public.
        /// </summary>
        internal CalendarLabelBlockOutputType CalendarLabelBlockOutput = CalendarLabelBlockOutputType.Beginning;

        /// <summary>
        /// Boolean flag to tell the system if it should display the current time stamp in the middle of 
        /// the vertical on the right end of the chart to have a real time update of the latest tick
        /// in the system. Something akin to the "realTime" flag in the previous version.
        /// This value could be made public (though I'd make it an ENUM first).
        /// </summary>
        internal bool CurrentTimeStamp = true;

        /// <summary>
        /// Holds the list of scales that can be displayed by the calendar. There are a set of defaults
        /// that are created internallt, but the user could just as well supply theirs (if the object was 
        /// made public). Though there is a need that the list is ordered in relative size. There are
        /// methods available to assist in the ordering of the list.
        /// </summary>
        internal List<CalendarScaleData> CalendarScaleDataList = CalendarScaleData.DefaultCalendarScaleData();

#if DEBUG
        /// <summary>
        /// Helper for seeing where the labels really start and end on the screen (helps with layout and
        /// debugging, but really only useful when building application.
        /// </summary>
        private bool _verticalLablelLines = false;
#endif

        /// <summary>
        /// 
        /// </summary>
        private void PaintVersion2()
        {
            try
            {
                if (_painting) return;
                _painting = true;
                _timerRepaint.StopTimerWork(TimerRepaint);

                if (_chartX.VisibleRecordCount == 0) return;

                if (CalendarScaleDataList.Count == 0) return; // Or throw an error

                List<CalendarOutputData> periodsToBeLabeled = new List<CalendarOutputData>();
                List<CalendarOutputData> secondaryPeriodsToBeLabeled = new List<CalendarOutputData>();

                CalendarScaleData workingScaleData = GetWorkingScaleData();

                GetPeriodsToBeLabeled(workingScaleData, periodsToBeLabeled, secondaryPeriodsToBeLabeled);

                System.Diagnostics.Debug.WriteLine("Calendar.Paint()");

                // Ok, we are ready to draw things...
                // Start with the collection of lines that are going to be on the calendar.
                GeometryGroup lines = new GeometryGroup();
                if (_linesPath == null)
                {
                    _linesPath = new Path() { Stroke = _chartX.GridStroke, StrokeThickness = 1 };
                    Children.Add(_linesPath);
                }
                lines.Children.Add(new LineGeometry() { StartPoint = new Point(0, 0), EndPoint = new Point(ActualWidth, 0) });

                // Prep the labels drawing tool that will be used to get label drawing objects
                _labels.C = this;
                _labels.Start();

                // Clear out all ot the x grids (vertical lines) that will be added back on for each
                // calendar label elemenet
                _chartX._xGridMap.Clear();
                int xGrid = 0;


                // Here are a bunch of helper variables that are going to be used. For label output
                double periodX;
                double labelX;
                string outputString;
                double outputStringWidth;
                DateTime workingDateTime;

                // Variables used to bound the output and make sure nothing overlaps or exists the 
                // usable area.
                double lastUsedPixel = -0.01; // Set to a value just below 0 
                double lastAvalablePixel = _chartX.GetXPixel(_chartX.VisibleRecordCount - 1);

                // Display the Current TimeStamp if the user wants it
                if (CurrentTimeStamp)
                {
                    if (_chartX.LastVisibleRecord == DM.RecordCount && DM.GetLastTickEntry(_chartX.Symbol) != null)
                        workingDateTime = DM.GetLastTickEntry(_chartX.Symbol)._timeStamp;
                    else
                        workingDateTime = DM.GetTimeStampByIndex(_chartX._endIndex - 1);
                    System.Diagnostics.Debug.WriteLine("CurrentTimeStamp - " + workingDateTime.ToString());

                    // Format and calculate the data for the output string
                    outputString = workingDateTime.ToString(workingScaleData.CurrentTimeStampOutputFormat);
                    outputStringWidth = _chartX.GetTextWidth(outputString);

                    // Center the label on the final datapoint. It is actually viable to extend outside
                    // the "lastAvailablePixel" due to the gap provided by RightChartGap and the actual
                    // width of the candlestick itself. It is not possible to go outside the ActualWidth
                    // of the calendar though.
                    labelX = lastAvalablePixel - (outputStringWidth * 0.5);
                    if ((labelX + outputStringWidth) > ActualWidth)
                        labelX = ActualWidth - outputStringWidth;

                    if (labelX >= 0)
                    {
#if DEBUG
                        // Debug tool for seeing where the labels start and end
                        if (_verticalLablelLines)
                        {
                            lines.Children.Add(new LineGeometry() { StartPoint = new Point(labelX, 0), EndPoint = new Point(labelX, ActualHeight) });
                            lines.Children.Add(new LineGeometry() { StartPoint = new Point(labelX + outputStringWidth, 0), EndPoint = new Point(labelX + outputStringWidth, ActualHeight) });
                        }
#endif
                        Label label = _labels.GetPaintObject();
                        label.Left = labelX;
                        label.Top = ActualHeight / 2 - (_chartX.GetTextHeight(outputString) / 2);
                        label.Text = outputString;

                        lastAvalablePixel = labelX - LABLE_GAP;
                    }

                }

                // Write the labels into the calendar area from right to left.
                foreach (CalendarOutputData outputData in periodsToBeLabeled)
                {
                    // Get where on the axis the period exists
                    periodX = _chartX.GetXPixel(outputData.Period - _chartX._startIndex);

                    // Get which data point is going to be displayed
                    switch (CalendarLabelBlockOutput)
                    {
                        case CalendarLabelBlockOutputType.End:
                            workingDateTime = outputData.NextCalendarBlodkDateStamp;
                            break;
                        case CalendarLabelBlockOutputType.FirstValid:
                            workingDateTime = outputData.FirstValidDateStamp;
                            break;
                        default:
                            workingDateTime = outputData.CalendarBlockDateStamp;
                            break;
                    }

                    // Format and calculate the data for the output string
                    outputString = workingDateTime.ToString(workingScaleData.OutputFormat);
                    outputStringWidth = _chartX.GetTextWidth(outputString);

                    // The label and the "tick" might be in slightly different locations.
                    // The tick must be centered on the bar and the grid, but the label
                    // may be justified. Currently the only two options are centered and
                    // right of the tick.
                    labelX = periodX;
                    if (workingScaleData.HorozontalAlign == HorizontalAlignment.Center)
                        labelX -= outputStringWidth * 0.5;

                    // Only display the data if it is not overwriting something else and it is not
                    // going to exit the calendar area.
                    if (labelX > lastUsedPixel && (labelX + outputStringWidth) <= lastAvalablePixel)
                    {
#if DEBUG
                        // Debug tool for seeing where the labels start and end
                        if (_verticalLablelLines)
                        {
                            lines.Children.Add(new LineGeometry() { StartPoint = new Point(labelX, ActualHeight / 6), EndPoint = new Point(labelX, ActualHeight / 2) });
                            lines.Children.Add(new LineGeometry() { StartPoint = new Point(labelX + outputStringWidth, ActualHeight / 6), EndPoint = new Point(labelX + outputStringWidth, ActualHeight / 2) });
                        }
#endif

                        // Display the 'tick'
                        lines.Children.Add(new LineGeometry() { StartPoint = new Point(periodX, 0), EndPoint = new Point(periodX, ActualHeight / 4) });

                        // Display the label
                        Label label = _labels.GetPaintObject();
                        label.Left = labelX;
                        label.Top = ActualHeight / 6;
                        label.Text = outputString;

                        // Display the grid line.
                        _chartX._xGridMap[xGrid++] = periodX;

                        // Advance the last pixel used.
                        lastUsedPixel = labelX + outputStringWidth + LABLE_GAP;
                    }
                }


                // Loop through the secondaryPeriodsToBeLabeled backward and throw some data onto the screen.
                // It is run backwards because the way that the values have to overwrite each other.
                // Math for the lastUsedPixel is backwards too, though most of the remainder of the 
                // loop is just like the one above (therefore less comments).
                lastUsedPixel = lastAvalablePixel + 0.01;
                for (int index = secondaryPeriodsToBeLabeled.Count - 1; index >= 0; index--)
                {
                    CalendarOutputData outputData = secondaryPeriodsToBeLabeled[index];
                    periodX = _chartX.GetXPixel(outputData.Period - _chartX._startIndex);

                    // Due to an oddity of how GetXPixes works it is possible to have the left
                    // most period with a negative number (and therefore off the screen, and 
                    // therefore not displaying when it should). Therefore the value is set
                    // back to 0.0
                    if (periodX < 0)
                        periodX = 0.0;

                    switch (CalendarLabelBlockOutput)
                    {
                        case CalendarLabelBlockOutputType.End:
                            workingDateTime = outputData.NextCalendarBlodkDateStamp;
                            break;
                        case CalendarLabelBlockOutputType.FirstValid:
                            workingDateTime = outputData.FirstValidDateStamp;
                            break;
                        default:
                            workingDateTime = outputData.CalendarBlockDateStamp;
                            break;
                    }

                    outputString = workingDateTime.ToString(workingScaleData.SecondaryOutputFormat);
                    outputStringWidth = _chartX.GetTextWidth(outputString);

                    labelX = periodX;
                    if (workingScaleData.SecondaryHorozontalAlign == HorizontalAlignment.Center)
                        labelX -= outputStringWidth * 0.5;

                    // Reverse logic from above. But basically the same thing.
                    if (labelX >= 0 && (labelX + outputStringWidth) < lastUsedPixel && (labelX + outputStringWidth) <= lastAvalablePixel)
                    {
#if DEBUG
                        // Debug tool for seeing where the labels start and end
                        if (_verticalLablelLines)
                        {
                            lines.Children.Add(new LineGeometry() { StartPoint = new Point(labelX, ActualHeight / 2), EndPoint = new Point(labelX, ActualHeight) });
                            lines.Children.Add(new LineGeometry() { StartPoint = new Point(labelX + outputStringWidth, ActualHeight / 2), EndPoint = new Point(labelX + outputStringWidth, ActualHeight) });
                        }
#endif

                        Label label = _labels.GetPaintObject();
                        label.Left = labelX;
                        label.Top = ActualHeight / 2;
                        label.Text = outputString;

                        lastUsedPixel = labelX - LABLE_GAP;
                    }
                }

                // Output the lines and stop the labels object.
                _linesPath.Data = (System.Windows.Media.Geometry)lines.GetAsFrozen();
                _labels.Stop();
            }
            finally
            {
                _painting = false;

                //after calendar is painted must instruct each panel to repaint the X Grid if needed
                if (_chartX.XGrid)
                    foreach (var panel in _chartX.PanelsCollection)
                        panel.PaintXGrid();
            }
        }

        /// <summary>
        /// Quick math to figure out which scale is going to be used.
        /// This method uses the following assumptions.
        /// The CalendarScaleDataList object must already be ordered from the smallest to the largest scale.
        /// If the user is assigning the list, then a method will need to be written to order the list first. 
        /// There is a concept of stickyness in this method. There was a problem with the scale changing when
        /// the chart is scrolled (has to do with the mathematical imprecision). The current solution is to 
        /// make the chosen CalendarScaleDataIndex a bit sticky. 
        /// </summary>
        private CalendarScaleData GetWorkingScaleData()
        {
            DateTime firstVisibleTimeStamp, lastVisibleTimeStamp;
            DM.GetStartEndTimeStamp(out firstVisibleTimeStamp, out lastVisibleTimeStamp);

            // Use the helper method to calculate the number of seconds in the complete span. Takes into acount the non trading time (weekends,
            // after hours and the like)
            long visibleSecondsCount = CalendarScaleData.SecondsInTimeSpan(new TimeSpan(lastVisibleTimeStamp.Ticks - firstVisibleTimeStamp.Ticks));

            double WorkingWidth = _chartX.GetXPixel(_chartX.VisibleRecordCount - 1) - _chartX.GetXPixel(0);

            int viableScaleIndex = -1;
            for (int index = 0; index < CalendarScaleDataList.Count; index++)
            {
                CalendarScaleData scaleData = CalendarScaleDataList[index];
                scaleData.WorkingData = new CalendarScaleWorkingData();

                // Calculate all the maximal values for the scales
                scaleData.WorkingData.LabelMaxOutputWidth = _chartX.GetTextWidth(TEST_DATE_TIME.ToString(scaleData.OutputFormat));
                scaleData.WorkingData.SecondaryLabelMaxOutputWidth = _chartX.GetTextWidth(TEST_DATE_TIME.ToString(scaleData.SecondaryOutputFormat));

                // Calculate a theoretical number of periods for each scale using the number of seconds 
                // in the complete calendar and divide it by the number of seconds in each period of the scale.
                scaleData.WorkingData.TheoreticalPeriodCount = visibleSecondsCount / scaleData.SecondsPerPeriod;

                // Calculate the amount of space (pixels) required to display the current scale.
                scaleData.WorkingData.TheoreticalMaxOutputWidth = scaleData.WorkingData.TheoreticalPeriodCount * (long)Math.Ceiling(scaleData.WorkingData.LabelMaxOutputWidth * LABLE_GAP);

                // Grab the first viable scale
                if (viableScaleIndex == -1 && scaleData.WorkingData.TheoreticalMaxOutputWidth < WorkingWidth)
                    viableScaleIndex = index;
            }

            // If there is still not a viable scale index, we will choose the last one in the list
            // and then other math will have to take over later to clean things up.
            if (viableScaleIndex == -1)
                return CalendarScaleDataList[CalendarScaleDataList.Count - 1];
            else
                return CalendarScaleDataList[viableScaleIndex];
        }

        /// <summary>
        /// This method decides which periods are going to get to have labels.
        /// This methodology is a major departure from the current way of doing things. 
        /// The current ModulusFE code has static(-ish) label locations with values that would
        /// change. This way of doing things is that specific calendar periods are going to
        /// keep their labels as the chart moves across the screen (at least as much as 
        /// possible). This method is going to do the math to decide which periods are going
        /// to have labels. This is done by scaning through the timeStamp data in the DataManager
        /// and taking a reference of each period that is going to have a label associated with it.
        /// </summary>
        /// <param name="workingScaleData">The CalendarScaleData object that will be used for calculations</param>
        /// <param name="periodsToBeLabeled">The list of data periods to have lables on the first line of the calendar</param>
        /// <param name="secondaryPeriodsToBeLabeled">The list of data periods to have lables on the first line of the calendar</param>
        private void GetPeriodsToBeLabeled(CalendarScaleData workingScaleData, List<CalendarOutputData> periodsToBeLabeled, List<CalendarOutputData> secondaryPeriodsToBeLabeled)
        {
            if (workingScaleData == null || periodsToBeLabeled == null || secondaryPeriodsToBeLabeled == null)
                throw new InvalidOperationException();

            // Calculate the initial current and subsequent (next) periods for bookending the data.
            // Note: there is an additional AddScaleType() call so that the first partial period is not captured.
            // This simplifies things when coming to output. But this is not the case with the secondary scale
            // which must have the first partial period labeled.
            DateTime calendarScaleCurrent = DM.GetTimeStampByIndex(_chartX._startIndex);
            calendarScaleCurrent = CalendarScaleData.TopOfScalePeriod(calendarScaleCurrent, workingScaleData.ScaleType);
            calendarScaleCurrent = CalendarScaleData.AddScaleType(calendarScaleCurrent, workingScaleData.Scale, workingScaleData.ScaleType);
            DateTime calendarScaleNext = CalendarScaleData.AddScaleType(calendarScaleCurrent, workingScaleData.Scale, workingScaleData.ScaleType);

            DateTime secondaryCalendarScaleCurrent = DM.GetTimeStampByIndex(_chartX._startIndex); ;
            secondaryCalendarScaleCurrent = CalendarScaleData.TopOfScalePeriod(secondaryCalendarScaleCurrent, workingScaleData.SecondaryScaleType);
            DateTime secondaryCalendarScaleNext = CalendarScaleData.AddScaleType(secondaryCalendarScaleCurrent, workingScaleData.SecondaryScale, workingScaleData.SecondaryScaleType);

            DateTime workingDateTime;
            for (int index = _chartX._startIndex; index < _chartX._endIndex; index++)
            {
                workingDateTime = DM.GetTimeStampByIndex(index);

                // When we hit the first valid data point in a calendar period we grab the period that is
                // associated with it so that it'll get a lable. (And then advance the period values).
                // Both the calendar period value and the data period value is captured in an object so that
                // the output method can choose which one to display to the client.
                if (workingDateTime >= calendarScaleCurrent && workingDateTime < calendarScaleNext)
                {
                    periodsToBeLabeled.Add(new CalendarOutputData()
                    {
                        Period = index,
                        CalendarBlockDateStamp = calendarScaleCurrent,
                        FirstValidDateStamp = workingDateTime,
                        NextCalendarBlodkDateStamp = calendarScaleNext
                    });
                    calendarScaleCurrent = calendarScaleNext;
                    calendarScaleNext = CalendarScaleData.AddScaleType(calendarScaleCurrent, workingScaleData.Scale, workingScaleData.ScaleType);
                }

                // It is possible for the secondary scale to be blank (hence the first clause).
                if (workingScaleData.SecondaryScale > 0 && workingDateTime >= secondaryCalendarScaleCurrent && workingDateTime < secondaryCalendarScaleNext)
                {
                    secondaryPeriodsToBeLabeled.Add(new CalendarOutputData()
                    {
                        Period = index,
                        CalendarBlockDateStamp = secondaryCalendarScaleCurrent,
                        FirstValidDateStamp = workingDateTime,
                        NextCalendarBlodkDateStamp = secondaryCalendarScaleNext
                    });
                    secondaryCalendarScaleCurrent = secondaryCalendarScaleNext;
                    secondaryCalendarScaleNext = CalendarScaleData.AddScaleType(secondaryCalendarScaleCurrent, workingScaleData.SecondaryScale, workingScaleData.SecondaryScaleType);
                }

                // There could be a problem with 'gap-y' data that has to be controlled.
                // If the next period needs to be labeled, but there is a gap larger than
                // the current scale, it will get missed. Therefore, the current/next values
                // need to be advanced until the next period comes into range.
                if (index < _chartX._endIndex)
                {
                    workingDateTime = DM.GetTimeStampByIndex(index + 1);
                    while (workingDateTime >= calendarScaleNext)
                    {
                        calendarScaleCurrent = calendarScaleNext;
                        calendarScaleNext = CalendarScaleData.AddScaleType(calendarScaleCurrent, workingScaleData.Scale, workingScaleData.ScaleType);
                    }

                    // It is possible for the secondary scale to be blank (hence the first clause).
                    while (workingScaleData.SecondaryScale > 0 && workingDateTime >= secondaryCalendarScaleNext)
                    {
                        secondaryCalendarScaleCurrent = secondaryCalendarScaleNext;
                        secondaryCalendarScaleNext = CalendarScaleData.AddScaleType(secondaryCalendarScaleCurrent, workingScaleData.SecondaryScale, workingScaleData.SecondaryScaleType);
                    }
                }
            }
        }

        #endregion Paint VERSION 2

        ///<summary>
        ///</summary>
        public IEnumerable<InfoPanelItem> InfoPanelItems
        {
            get
            {
                return new InfoPanelItem[] { new CalendarInfoPanelItem { _noCaption = true } };
            }
        }
    }

    internal class CalendarInfoPanelItem : InfoPanelItem
    {
        public override string Caption
        {
            get { return "TimeStamp"; }
        }

        public override string Value
        {
            get
            {
                int index = (int)_infoPanel.GetReverseX();

                if (index < 0 || index >= _infoPanel._chartX._endIndex)
                {
                    return string.Empty;
                }

                index += _infoPanel._chartX._startIndex;

                DateTime dateTime = _infoPanel._chartX._dataManager.GetTimeStampByIndex(index);

                return dateTime == DateTime.MinValue ? string.Empty : dateTime.ToShortDateString() + " " + dateTime.ToShortTimeString();
            }
        }
    }

    #region Paint VERSION 2

    internal class CalendarScaleWorkingData
    {
        /// <summary>
        /// The theoretical maximum amount of space needed for output.
        /// </summary>
        internal double LabelMaxOutputWidth { get; set; }
        internal double SecondaryLabelMaxOutputWidth { get; set; }

        /// <summary>
        /// The theoretical number of display periods for the current scale
        /// </summary>
        internal long TheoreticalPeriodCount { get; set; }

        /// <summary>
        /// The theoretical amount of space needed to display the output.
        /// </summary>
        internal long TheoreticalMaxOutputWidth { get; set; }

    }

    internal class CalendarOutputData
    {
        internal int Period { get; set; }
        internal DateTime CalendarBlockDateStamp { get; set; }
        internal DateTime FirstValidDateStamp { get; set; }
        internal DateTime NextCalendarBlodkDateStamp { get; set; }
    }

    /// <summary>
    /// Information about each scale that the user wants the calendar to pass through.
    /// This class could be make public to the developer so that they can play with the 
    /// different scales visible to the user.
    /// </summary>
    public class CalendarScaleData
    {
        public enum ScaleTypes
        {
            Second,
            Minute,
            Day,
            Week,
            Month,
            Year
        };

        /// <summary>
        /// Class used by the calendar class to store data for each run of the paint routine.
        /// </summary>
        internal CalendarScaleWorkingData WorkingData { get; set; }

        /// <summary>
        /// The number of units in the scale 1min. 5min. etc.
        /// The valid combination for seconds and minutes have some rules.
        /// It's mostly about hitting the top of the hour or minute on each 
        /// loop. Therefore 1, 2, 3, 4, 5, 6, 10, 12, 15, 20, and 30 are 
        /// the only values below 60. Once you crack 60 (and it's only allowed
        /// for minutes as there is no hour scale) only hourly values are
        /// valid.
        /// Invalid values will not throw an error, they will simply not be set.
        /// </summary>
        private int scale = 1;
        public int Scale
        {
            get
            {
                return scale;
            }
            set
            {
                if (scale != value && ValidateScaleScaleType(value, ScaleType))
                    scale = value;
            }
        }

        /// <summary>
        /// The type of units in the scale
        /// The valid combination for seconds and minutes have some rules.
        /// It's mostly about hitting the top of the hour or minute on each 
        /// loop. Therefore 1, 2, 3, 4, 5, 6, 10, 12, 15, 20, and 30 are 
        /// the only values below 60. Once you crack 60 (and it's only allowed
        /// for minutes as there is no hour scale) only hourly values are
        /// valid.
        /// Invalid values will not throw an error, they will simply not be set.
        /// </summary>
        private ScaleTypes scaleType = ScaleTypes.Day;
        public ScaleTypes ScaleType
        {
            get
            {
                return scaleType;
            }
            set
            {
                if (scaleType != value && ValidateScaleScaleType(Scale, value))
                    scaleType = value;
            }
        }

        /// <summary>
        /// The style that is going to be used for output 
        /// </summary>
        public string OutputFormat { get; set; }

        /// <summary>
        /// The alignment of the text under the tick.
        /// </summary>
        public HorizontalAlignment HorozontalAlign { get; set; }

        /// <summary>
        /// Scale to be used for the second line of data.
        /// </summary>
        public int SecondaryScale { get; set; }

        /// <summary>
        /// Scale type to be used for the second line of data.
        /// </summary>
        public ScaleTypes SecondaryScaleType { get; set; }

        /// <summary>
        /// Style to be used for second line of data
        /// </summary>
        public string SecondaryOutputFormat { get; set; }

        /// <summary>
        /// The alignment of the text on the second line of data
        /// </summary>
        public HorizontalAlignment SecondaryHorozontalAlign { get; set; }

        /// <summary>
        /// The style that is going to be used for the current period data
        /// entry.
        /// </summary>
        public string CurrentTimeStampOutputFormat { get; set; }

        /// <summary>
        /// A number that gives the relative size of a scale based on plausible number of 
        /// seconds in the scale. Used for ordering of output scales. Also used as a good
        /// general (pseudo unique) reference for a list of scales.
        /// </summary>
        internal long RelativeScaleSize
        {
            get
            {
                return SecondsInScaleType(ScaleType) * Scale;
            }
        }

        internal long SecondsPerPeriod
        {
            get
            {
                return RelativeScaleSize;
            }
        }

        /// <summary>
        /// The valid combination for seconds and minutes have some rules.
        /// It's mostly about hitting the top of the hour or minute on each 
        /// loop. Therefore 1, 2, 3, 4, 5, 6, 10, 12, 15, 20, and 30 are 
        /// the only values below 60. Once you crack 60 (and it's only allowed
        /// for minutes as there is no hour scale) only hourly values are
        /// valid.
        /// </summary>
        /// <param name="scale"></param>
        /// <param name="scaleType"></param>
        /// <returns></returns>
        private bool ValidateScaleScaleType(int scale, ScaleTypes scaleType)
        {
            if (scaleType == ScaleTypes.Second || scaleType == ScaleTypes.Minute)
            {
                if (scale < 60)
                {
                    if ((60 % scale) != 0)
                    {
                        return false;
                    }
                }
                else // scale is 60 or greater
                {
                    if (scaleType == ScaleTypes.Second)
                    {
                        return false;
                    }
                    else if ((scale % 60) != 0)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// The number of seconds in any given scale type. 
        /// </summary>
        /// <param name="scaleType"></param>
        /// <returns></returns>
        public static long SecondsInScaleType(ScaleTypes scaleType)
        {
            switch (scaleType)
            {
                case ScaleTypes.Second:
                    return 1;
                case ScaleTypes.Minute:
                    return 60;
                case ScaleTypes.Day:
                    return SecondsInScaleType(ScaleTypes.Minute) * (long)(60.0 * 6.5); // 6.5h day in standard strading day. True day is 86400
                case ScaleTypes.Week:
                    return SecondsInScaleType(ScaleTypes.Day) * 5; // Most weeks have 5 trading days. True week is 604800
                case ScaleTypes.Month:
                    return SecondsInScaleType(ScaleTypes.Week) * 52 / 12; // Average month is 2628000
                case ScaleTypes.Year:
                    return SecondsInScaleType(ScaleTypes.Week) * 52; // True year is 31449600
            }
            return 0;
        }

        /// <summary>
        /// Due to the nature of markets and how long they are actually open, the number
        /// of seconds in a standard time span will return an super inflated number.
        /// Therefore this tool will return a more reasonable number of seconds that 
        /// will more closely approximate the number of seconds the market is likely to be 
        /// open during a given time span.
        /// </summary>
        /// <param name="timeSpan"></param>
        /// <returns></returns>
        public static long SecondsInTimeSpan(TimeSpan timeSpan)
        {
            long returnValue = 0;

            if (timeSpan != null)
            {
                int days = timeSpan.Days;
                if (days > 365)
                {
                    returnValue += SecondsInScaleType(ScaleTypes.Year) * (days / 365);
                    days = days % 365;
                }
                if (days > 7)
                {
                    returnValue += SecondsInScaleType(ScaleTypes.Week) * (days / 7);
                    days = days % 7;
                }
                if (days > 0)
                {
                    returnValue += SecondsInScaleType(ScaleTypes.Day) * days;
                }

                int hoursAsSeconds = timeSpan.Hours * 60 * 60;
                if (hoursAsSeconds > SecondsInScaleType(ScaleTypes.Day))
                {
                    returnValue += SecondsInScaleType(ScaleTypes.Day);
                }
                else
                {
                    returnValue += hoursAsSeconds;
                }

                returnValue += timeSpan.Minutes * SecondsInScaleType(ScaleTypes.Minute);
                returnValue += timeSpan.Seconds * SecondsInScaleType(ScaleTypes.Second);
            }
            return returnValue;
        }

        /// <summary>
        /// Given the supplied dateTime. Returns the "top" of the period for the scale supplied.
        /// Basically rounds-back to the top of the current period.
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="scaleType"></param>
        /// <returns></returns>
        public static DateTime TopOfScalePeriod(DateTime dt, ScaleTypes scaleType)
        {
            int year = dt.Year;
            int month = dt.Month;
            int day = dt.Day;
            int hour = dt.Hour;
            int minute = dt.Minute;
            int second = dt.Second;

            int weekdayOffset = 0;

            switch (scaleType)
            {
                case ScaleTypes.Second:
                    second = 0;
                    break;
                case ScaleTypes.Minute:
                    second = 0;
                    minute = 0;
                    break;
                case ScaleTypes.Day:
                    second = 0;
                    minute = 0;
                    hour = 0;
                    break;
                case ScaleTypes.Week:
                    second = 0;
                    minute = 0;
                    hour = 0;
                    switch (dt.DayOfWeek)
                    {
                        case DayOfWeek.Monday:
                            weekdayOffset = 0;
                            break;
                        case DayOfWeek.Tuesday:
                            weekdayOffset = -1;
                            break;
                        case DayOfWeek.Wednesday:
                            weekdayOffset = -2;
                            break;
                        case DayOfWeek.Thursday:
                            weekdayOffset = -3;
                            break;
                        case DayOfWeek.Friday:
                            weekdayOffset = -4;
                            break;
                        case DayOfWeek.Saturday:
                            weekdayOffset = -5;
                            break;
                        case DayOfWeek.Sunday:
                            weekdayOffset = -6;
                            break;
                    }
                    break;
                case ScaleTypes.Month:
                    second = 0;
                    minute = 0;
                    hour = 0;
                    day = 1;
                    break;
                case ScaleTypes.Year:
                    second = 0;
                    minute = 0;
                    hour = 0;
                    day = 1;
                    month = 1;
                    break;
            }
            return new DateTime(year, month, day, hour, minute, second).AddDays(weekdayOffset);
        }

        /// <summary>
        /// Method that will increment the given DateTime by the unit count of the given scaleType.
        /// Useful for advancing a reference variable to the next value.
        /// </summary>
        /// <param name="dt">DateTime to add the increment to</param>
        /// <param name="units">The number of increments to apply</param>
        /// <param name="scaleType">The size (scaleType) of the increment to apply</param>
        /// <returns></returns>
        public static DateTime AddScaleType(DateTime dt, int units, ScaleTypes scaleType)
        {
            DateTime returnDateTime = new DateTime(dt.Ticks);
            switch (scaleType)
            {
                case ScaleTypes.Second:
                    returnDateTime = dt.AddSeconds(units);
                    break;
                case ScaleTypes.Minute:
                    returnDateTime = dt.AddMinutes(units);
                    break;
                case ScaleTypes.Day:
                    returnDateTime = dt.AddDays(units);
                    break;
                case ScaleTypes.Week:
                    returnDateTime = dt.AddDays(7 * units);
                    break;
                case ScaleTypes.Month:
                    returnDateTime = dt.AddMonths(units);
                    break;
                case ScaleTypes.Year:
                    returnDateTime = dt.AddYears(units);
                    break;
            }
            return returnDateTime;

        }

        /// <summary>
        /// A simple default set of calendar scale data points from seconds up to years.
        /// </summary>
        /// <returns></returns>
        internal static List<CalendarScaleData> DefaultCalendarScaleData()
        {
            List<CalendarScaleData> defaultData = new List<CalendarScaleData>();
            CalendarScaleData dataPoint;

            dataPoint = new CalendarScaleData()
            {
                Scale = 1,
                ScaleType = ScaleTypes.Second,
                OutputFormat = "h:mm:ss t",
                HorozontalAlign = HorizontalAlignment.Center,
                SecondaryScale = 1,
                SecondaryScaleType = ScaleTypes.Day,
                SecondaryOutputFormat = "MMM. d, yyyy",
                SecondaryHorozontalAlign = HorizontalAlignment.Right,
                CurrentTimeStampOutputFormat = "M/d/yyyy h:mm:ss"
            };
            defaultData.Add(dataPoint);

            dataPoint = new CalendarScaleData()
            {
                Scale = 5,
                ScaleType = ScaleTypes.Second,
                OutputFormat = "h:mm:ss t",
                HorozontalAlign = HorizontalAlignment.Center,
                SecondaryScale = 1,
                SecondaryScaleType = ScaleTypes.Day,
                SecondaryOutputFormat = "MMM. d, yyyy",
                SecondaryHorozontalAlign = HorizontalAlignment.Right,
                CurrentTimeStampOutputFormat = "M/d/yyyy h:mm:ss"
            };
            defaultData.Add(dataPoint);

            dataPoint = new CalendarScaleData()
            {
                Scale = 15,
                ScaleType = ScaleTypes.Second,
                OutputFormat = "h:mm:ss t",
                HorozontalAlign = HorizontalAlignment.Center,
                SecondaryScale = 1,
                SecondaryScaleType = ScaleTypes.Day,
                SecondaryOutputFormat = "MMM. d, yyyy",
                SecondaryHorozontalAlign = HorizontalAlignment.Right,
                CurrentTimeStampOutputFormat = "M/d/yyyy h:mm:ss"
            };
            defaultData.Add(dataPoint);

            dataPoint = new CalendarScaleData()
            {
                Scale = 30,
                ScaleType = ScaleTypes.Second,
                OutputFormat = "h:mm:ss t",
                HorozontalAlign = HorizontalAlignment.Center,
                SecondaryScale = 1,
                SecondaryScaleType = ScaleTypes.Day,
                SecondaryOutputFormat = "MMM. d, yyyy",
                SecondaryHorozontalAlign = HorizontalAlignment.Right,
                CurrentTimeStampOutputFormat = "M/d/yyyy h:mm:ss"
            };
            defaultData.Add(dataPoint);

            dataPoint = new CalendarScaleData()
            {
                Scale = 1,
                ScaleType = ScaleTypes.Minute,
                OutputFormat = "h:mm t",
                HorozontalAlign = HorizontalAlignment.Center,
                SecondaryScale = 1,
                SecondaryScaleType = ScaleTypes.Day,
                SecondaryOutputFormat = "MMM. d, yyyy",
                SecondaryHorozontalAlign = HorizontalAlignment.Right,
                CurrentTimeStampOutputFormat = "M/d/yyyy h:mm:ss"
            };
            defaultData.Add(dataPoint);

            dataPoint = new CalendarScaleData()
            {
                Scale = 2,
                ScaleType = ScaleTypes.Minute,
                OutputFormat = "h:mm t",
                HorozontalAlign = HorizontalAlignment.Center,
                SecondaryScale = 1,
                SecondaryScaleType = ScaleTypes.Day,
                SecondaryOutputFormat = "MMM. d, yyyy",
                SecondaryHorozontalAlign = HorizontalAlignment.Right,
                CurrentTimeStampOutputFormat = "M/d/yyyy h:mm:ss"
            };
            defaultData.Add(dataPoint);

            dataPoint = new CalendarScaleData()
            {
                Scale = 5,
                ScaleType = ScaleTypes.Minute,
                OutputFormat = "h:mm t",
                HorozontalAlign = HorizontalAlignment.Center,
                SecondaryScale = 1,
                SecondaryScaleType = ScaleTypes.Day,
                SecondaryOutputFormat = "MMM. d, yyyy",
                SecondaryHorozontalAlign = HorizontalAlignment.Right,
                CurrentTimeStampOutputFormat = "M/d/yyyy h:mm:ss"
            };
            defaultData.Add(dataPoint);

            dataPoint = new CalendarScaleData()
            {
                Scale = 15,
                ScaleType = ScaleTypes.Minute,
                OutputFormat = "h:mm t",
                HorozontalAlign = HorizontalAlignment.Center,
                SecondaryScale = 1,
                SecondaryScaleType = ScaleTypes.Day,
                SecondaryOutputFormat = "MMM. d, yyyy",
                SecondaryHorozontalAlign = HorizontalAlignment.Right,
                CurrentTimeStampOutputFormat = "M/d/yyyy h:mm:ss"
            };
            defaultData.Add(dataPoint);

            dataPoint = new CalendarScaleData()
            {
                Scale = 30,
                ScaleType = ScaleTypes.Minute,
                OutputFormat = "h:mm t",
                HorozontalAlign = HorizontalAlignment.Center,
                SecondaryScale = 1,
                SecondaryScaleType = ScaleTypes.Day,
                SecondaryOutputFormat = "MMM. d, yyyy",
                SecondaryHorozontalAlign = HorizontalAlignment.Right,
                CurrentTimeStampOutputFormat = "M/d/yyyy h:mm:ss"
            };
            defaultData.Add(dataPoint);

            dataPoint = new CalendarScaleData()
            {
                Scale = 60,
                ScaleType = ScaleTypes.Minute,
                OutputFormat = "h t",
                HorozontalAlign = HorizontalAlignment.Center,
                SecondaryScale = 1,
                SecondaryScaleType = ScaleTypes.Day,
                SecondaryOutputFormat = "MMM. d, yyyy",
                SecondaryHorozontalAlign = HorizontalAlignment.Right,
                CurrentTimeStampOutputFormat = "M/d/yyyy h:mm:ss"
            };
            defaultData.Add(dataPoint);

            dataPoint = new CalendarScaleData()
            {
                Scale = 120,
                ScaleType = ScaleTypes.Minute,
                OutputFormat = "h t",
                HorozontalAlign = HorizontalAlignment.Center,
                SecondaryScale = 1,
                SecondaryScaleType = ScaleTypes.Day,
                SecondaryOutputFormat = "MMM. d, yyyy",
                SecondaryHorozontalAlign = HorizontalAlignment.Right,
                CurrentTimeStampOutputFormat = "M/d/yyyy h:mm:ss"
            };
            defaultData.Add(dataPoint);

            dataPoint = new CalendarScaleData()
            {
                Scale = 1,
                ScaleType = ScaleTypes.Day,
                OutputFormat = "%d",
                HorozontalAlign = HorizontalAlignment.Center,
                SecondaryScale = 1,
                SecondaryScaleType = ScaleTypes.Month,
                SecondaryOutputFormat = "MMM. yyyy",
                SecondaryHorozontalAlign = HorizontalAlignment.Right,
                CurrentTimeStampOutputFormat = "M/d/yyyy h:mm:ss"
            };
            defaultData.Add(dataPoint);

            dataPoint = new CalendarScaleData()
            {
                Scale = 1,
                ScaleType = ScaleTypes.Week,
                OutputFormat = "%d",
                HorozontalAlign = HorizontalAlignment.Center,
                SecondaryScale = 1,
                SecondaryScaleType = ScaleTypes.Month,
                SecondaryOutputFormat = "MMM. yyyy",
                SecondaryHorozontalAlign = HorizontalAlignment.Right,
                CurrentTimeStampOutputFormat = "M/d/yyyy h:mm:ss"
            };
            defaultData.Add(dataPoint);

            dataPoint = new CalendarScaleData()
            {
                Scale = 1,
                ScaleType = ScaleTypes.Month,
                OutputFormat = "MMM",
                HorozontalAlign = HorizontalAlignment.Center,
                SecondaryScale = 1,
                SecondaryScaleType = ScaleTypes.Year,
                SecondaryOutputFormat = "yyyy",
                SecondaryHorozontalAlign = HorizontalAlignment.Right,
                CurrentTimeStampOutputFormat = "M/d/yyyy h:mm:ss"
            };
            defaultData.Add(dataPoint);

            dataPoint = new CalendarScaleData()
            {
                Scale = 1,
                ScaleType = ScaleTypes.Year,
                OutputFormat = "yyyy",
                HorozontalAlign = HorizontalAlignment.Center,
                CurrentTimeStampOutputFormat = "M/d/yyyy h:mm:ss"
            };
            defaultData.Add(dataPoint);

            return defaultData;
        }
    }


    #endregion Paint VERSION 2
}

