using System;
using System.Collections.Generic;

namespace ModulusFE.CalendarCalcs
{
    internal static class Extensions
    {
        public enum DateStripType
        {
            Seconds,
            Minutes,
            Hours,
            Days,
            Months
        };
        public static DateTime StripTo(this DateTime date, DateStripType stripType)
        {
            switch (stripType)
            {
                case DateStripType.Seconds:
                    return date.AddSeconds(-date.Second);
                case DateStripType.Minutes:
                    return StripTo(date, DateStripType.Seconds).AddMinutes(-date.Minute);
                case DateStripType.Hours:
                    return StripTo(date, DateStripType.Minutes).AddHours(-date.Hour);
                case DateStripType.Days:
                    return StripTo(date, DateStripType.Hours).AddDays(-date.Day + 1);
                case DateStripType.Months:
                    return StripTo(date, DateStripType.Hours).AddMonths(-date.Month).AddDays(-date.Day + 1);
            }
            throw new ArgumentException("stripType");
        }
    }

    internal class StockCalendar
    {
        private DateTime m_dtStartTime;
        private DateTime m_dtEndTime;
        private bool m_bChanged;
        private readonly Dictionary<DateTime, double> m_datesCargo = new Dictionary<DateTime, double>(); //will have an additional value binded to a date, that will determine its scale

        public delegate DateTime DRoundDateTime(DateTime date);
        public delegate DateTime DIncreaseTime(DateTime date);
        public delegate DateTime DGetNextSubPanelSeparator(DateTime date);
        public delegate string DFormatDateTime(DateTime date);

        public class PanelFormat
        {
            public string Name;
            public int SymbolsPerUnit;
            public DFormatDateTime FormatDateTime;
            public DRoundDateTime RoundStartDateTime;
            public DRoundDateTime RoundEndDateTime;
            public string[] DateFormats;
            public DIncreaseTime IncreaseTime;
            public DGetNextSubPanelSeparator GetNextSubPanelSeparator;
        }

        public static string[] StaticDateFormats = 
    { 
      "dd MMMM yyyy, HH:mm",
      "dd MMM yyyy, HH:mm",
      "MM/dd/yyyy, HH:mm",
      "MM/dd/yy, HH:mm",
      "MM/dd, HH:mm",
      "dd, HH:mm",
      "HH:mm",
      "mm"
    };

        #region Panel Formats
        private readonly List<PanelFormat> panels = new List<PanelFormat>
    {
      new PanelFormat
      {
        Name = "1 Sec",
        SymbolsPerUnit = 2, 
        FormatDateTime = date => date.ToString("ss"),
        RoundStartDateTime = date => (date),
        RoundEndDateTime = date => (date),
        IncreaseTime = date => (date.AddSeconds(1)),
        DateFormats = StaticDateFormats,
        GetNextSubPanelSeparator = (time => time.Second == 0 ? time.AddSeconds(60) : time.AddSeconds(60 - time.Second))
      },
      new PanelFormat
      {
        Name = "5 Secs",
        SymbolsPerUnit = 2,
        FormatDateTime = date => date.ToString("ss"),
        RoundStartDateTime = date => (date),
        RoundEndDateTime = date => (date.AddSeconds(date.Second % 5 == 0 ? 0 : 5 - (date.Second % 5))),
        IncreaseTime = date => (date.AddSeconds(5)),
        DateFormats = StaticDateFormats,
        GetNextSubPanelSeparator = (time => time.Second == 0 ? time.AddSeconds(60) : time.AddSeconds(60 - time.Second))
      },
      new PanelFormat
      {
        Name = "10 Secs",
        SymbolsPerUnit = 2,
        FormatDateTime = date => date.ToString("ss"),
        RoundStartDateTime = date => (date),
        RoundEndDateTime = date => (date.AddSeconds(date.Second % 5 == 0 ? 0 : 5 - (date.Second % 5))),
        IncreaseTime = date => (date.AddSeconds(10)),
        DateFormats = StaticDateFormats,
        GetNextSubPanelSeparator = (time => time.Second == 0 ? time.AddSeconds(60) : time.AddSeconds(60 - time.Second))
      }, 
      new PanelFormat
      {
        Name = "1 Min",
        SymbolsPerUnit = 2,
        FormatDateTime = date => date.ToString("mm"),
        RoundStartDateTime = date => date,
        RoundEndDateTime = date => date.StripTo(Extensions.DateStripType.Seconds).AddMinutes(1),
        IncreaseTime = date => date.AddMinutes(1),
        DateFormats = StaticDateFormats,
        GetNextSubPanelSeparator = (time => time.Minute == 0 ? time.AddMinutes(60) : time.AddMinutes(60 - time.Minute))
      },
      new PanelFormat //5min
      {
        Name = "5 Mins",
        SymbolsPerUnit = 2,
        FormatDateTime = date => date.ToString("mm"),
        RoundStartDateTime = date => date,
        RoundEndDateTime = date => date.StripTo(Extensions.DateStripType.Seconds).AddMinutes(1),
        IncreaseTime = date => (date.AddMinutes(5)),
        DateFormats = StaticDateFormats,
        GetNextSubPanelSeparator = (time => time.Minute == 0 ? time.AddMinutes(60) : time.AddMinutes(60 - time.Minute))
      },
      new PanelFormat //10 min
      {
        Name = "10 Mins",
        SymbolsPerUnit = 2,
        FormatDateTime = date => date.ToString("mm"),
        RoundStartDateTime = date => date,
        RoundEndDateTime = date => date.StripTo(Extensions.DateStripType.Seconds).AddMinutes(1),
        IncreaseTime = date => date.AddMinutes(10),
        DateFormats = StaticDateFormats,
        GetNextSubPanelSeparator = (time => time.Minute == 0 ? time.AddMinutes(60) : time.AddMinutes(60 - time.Minute))
      }, 
      new PanelFormat //1 hour
      {
        Name = "1 Hour",
        SymbolsPerUnit = 2,
        FormatDateTime = date => date.ToString("hh"),
        RoundStartDateTime = date => date,
        RoundEndDateTime = date => date.StripTo(Extensions.DateStripType.Minutes).AddHours(1),
        DateFormats = new[] { "dd MMMM yyyy", "dd MMM yyyy", "MM/dd/yyyy", "MM/dd", "dd" },
        IncreaseTime = date => (date.AddHours(1)),
        GetNextSubPanelSeparator = (time => time.Hour == 0 ? time.AddHours(24) : time.AddHours(24 - time.Hour))
      },
      new PanelFormat //4 hour
      {
        Name = "4 Hours",
        SymbolsPerUnit = 2,
        FormatDateTime = date => date.ToString("hh"),
        RoundStartDateTime = date => date,
        RoundEndDateTime = date => date.StripTo(Extensions.DateStripType.Minutes).AddHours(1),
        DateFormats = new[] { "dd MMMM yyyy", "dd MMM yyyy", "MM/dd/yyyy", "MM/dd", "dd" },
        IncreaseTime = date => (date.AddHours(4)),
        GetNextSubPanelSeparator = (time => time.Hour == 0 ? time.AddHours(24) : time.AddHours(24 - time.Hour))
      },
      new PanelFormat //6 hour
      {
        Name = "6 Hours",
        SymbolsPerUnit = 2,
        FormatDateTime = date => date.ToString("hh"),
        RoundStartDateTime = date => date,
        RoundEndDateTime = date => date.StripTo(Extensions.DateStripType.Minutes).AddHours(1),
        DateFormats = new[] { "dd MMMM yyyy", "dd MMM yyyy", "MM/dd/yyyy", "MM/dd", "dd" },
        IncreaseTime = date => (date.AddHours(6)),
        GetNextSubPanelSeparator = (time => time.Hour == 0 ? time.AddHours(24) : time.AddHours(24 - time.Hour))
      },
      new PanelFormat //1 Day
      {
        Name = "1 Day",
        SymbolsPerUnit = 2, 
        FormatDateTime = date => date.ToString("dd"),
        RoundStartDateTime = date => date,
        RoundEndDateTime = date => date.StripTo(Extensions.DateStripType.Hours).AddHours(12),
        DateFormats = new[] { "MMMM yyyy", "MMM yyyy", "MM/yyyy", "MM" },
        IncreaseTime = date => (date.AddDays(1)),
        GetNextSubPanelSeparator = (time => time.Date.Day == DateTime.DaysInMonth(time.Year, time.Month) ? 
          time.AddMonths(1) : time.AddDays(DateTime.DaysInMonth(time.Year, time.Month) - time.Day + 1))
      },
      new PanelFormat //7 Day
      {
        Name = "7 Days",
        SymbolsPerUnit = 2, 
        FormatDateTime = date => date.ToString("dd"),
        RoundStartDateTime = date => date,
        RoundEndDateTime = date => date.StripTo(Extensions.DateStripType.Hours).AddDays(1),
        DateFormats = new[] { "MMMM yyyy", "MMM yyyy", "MM/yyyy", "MM" },
        IncreaseTime = date => (date.AddDays(7)),
        GetNextSubPanelSeparator = (time => time.Date.Day == DateTime.DaysInMonth(time.Year, time.Month) ? 
          time.AddMonths(1) : time.AddDays(DateTime.DaysInMonth(time.Year, time.Month) - time.Day + 1))
      },
      new PanelFormat //1 month - long
      {
        Name = "1 Month Long",
        SymbolsPerUnit = 9,
        FormatDateTime = date => date.ToString("MMMM").Substring(0, 9),
        RoundStartDateTime = date => date,
        RoundEndDateTime = date => date.StripTo(Extensions.DateStripType.Hours).AddDays(DateTime.DaysInMonth(date.Year, date.Month) / 2),
        DateFormats = new[] { "yyyy", "yy" },
        IncreaseTime = date => (date.AddMonths(1)),
        GetNextSubPanelSeparator = time => time.Month == 1 ? time.AddYears(1) : time.AddMonths(12 - time.Month)
      },
      new PanelFormat //1 month - short
      {
        Name = "1 Month Short",
        SymbolsPerUnit = 3,
        FormatDateTime = date => date.ToString("MMM"),
        RoundStartDateTime = date => date,
        RoundEndDateTime = date => date.StripTo(Extensions.DateStripType.Hours).AddDays(DateTime.DaysInMonth(date.Year, date.Month) / 2),
        DateFormats = new[] { "yyyy", "yy" },
        IncreaseTime = date => (date.AddMonths(1)),
        GetNextSubPanelSeparator = time => time.Month == 1 ? time.AddYears(1) : time.AddMonths(12 - time.Month + 1)
      },
      new PanelFormat //1 month shortest
      {
        Name = "1 Month Shortest",
        SymbolsPerUnit = 1,
        FormatDateTime = date => date.ToString("MMM").Substring(0, 1),
        RoundStartDateTime = date => date,
        RoundEndDateTime = date => date.StripTo(Extensions.DateStripType.Hours).AddDays(DateTime.DaysInMonth(date.Year, date.Month) / 2),
        DateFormats = new[] { "yyyy", "yy" },
        IncreaseTime = date => (date.AddMonths(1)),
        GetNextSubPanelSeparator = time => time.Month == 1 ? time.AddYears(1) : time.AddMonths(12 - time.Month + 1)
      },
      new PanelFormat //3 months short
      {
        Name = "3 Months Short",
        SymbolsPerUnit = 3,
        FormatDateTime = date => date.ToString("MMM").Substring(0, 3),
        RoundStartDateTime = date => date,
        RoundEndDateTime = date => date.StripTo(Extensions.DateStripType.Hours).AddDays(DateTime.DaysInMonth(date.Year, date.Month) / 2),
        DateFormats = new[] { "yyyy", "yy" },
        IncreaseTime = date => (date.AddMonths(3)),
        GetNextSubPanelSeparator = time => time.Month == 1 ? time.AddYears(1) : time.AddMonths(12 - time.Month + 1)
      },
      new PanelFormat //1 year
      {
        Name = "1 Year Long",
        SymbolsPerUnit = 4,
        FormatDateTime = date => date.ToString("yyyy"),
        RoundStartDateTime = date => date,
        RoundEndDateTime = date => date.StripTo(Extensions.DateStripType.Days).AddMonths(2),
        IncreaseTime = date => (date.AddYears(1))
      },
      new PanelFormat //1 year short
      {
        Name = "1 Year Short",
        SymbolsPerUnit = 2,
        FormatDateTime = date => date.ToString("yy"),
        RoundStartDateTime = date => date,
        RoundEndDateTime = date => date.StripTo(Extensions.DateStripType.Days).AddMonths(2),
        IncreaseTime = date => (date.AddYears(1))
      },
      new PanelFormat //10 years
      {
        Name = "10 Years",
        SymbolsPerUnit = 4,
        FormatDateTime = date => date.ToString("yyyy"),
        RoundStartDateTime = date => date,
        RoundEndDateTime = date => date.StripTo(Extensions.DateStripType.Days).AddMonths(2),
        IncreaseTime = date => (date.AddYears(10))
      }
    };
        #endregion Panel Formats

        public StockCalendar()
        {
            m_bChanged = false;
            m_dtStartTime = m_dtEndTime = DateTime.MinValue;
        }

        public DateTime StartTime
        {
            get { return m_dtStartTime; }
            set
            {
                m_dtStartTime = value;
                m_bChanged = true;
            }
        }

        public DateTime EndTime
        {
            get { return m_dtEndTime; }
            set
            {
                m_dtEndTime = value;
                m_bChanged = true;
            }
        }

        public void ResetDatesCargo()
        {
            m_datesCargo.Clear();
        }

        public void SetDateCargo(DateTime dateTime, double Cargo)
        {
            if (dateTime < StartTime || dateTime > EndTime)
                throw new ArgumentOutOfRangeException("dateTime");
            m_datesCargo[dateTime] = Cargo;
        }

        public DateTime ChangedStartTime { get; private set; }
        public DateTime ChangedEndTime { get; private set; }
        public PanelFormat MinPanel { get; private set; }

        private double _plotingUnitWidth;
        public double X(DateTime dateTime)
        {
            if (m_datesCargo.Count == 0) //constant width
            {
                return (dateTime - ChangedStartTime).TotalSeconds * _plotingUnitWidth;
            }
            throw new NotImplementedException();
        }

        public void PaintEx(IRenderDevice context)
        {
            if (!m_bChanged || m_dtStartTime == DateTime.MinValue || m_dtStartTime == DateTime.MaxValue)
                return;
            double iSymbolWidth = context.GetTextWidth("W") * 1.2;
            //find the minimum possible panel format
            MinPanel = null;
            double dAvailableWidth = context.AvailableWidth;
            DateTime curDateTime;
            foreach (PanelFormat panel in panels)
            {
                //round start and end-time accordingly to panel format
                ChangedStartTime = panel.RoundStartDateTime(m_dtStartTime);
                ChangedEndTime = panel.RoundEndDateTime(m_dtEndTime);

                curDateTime = ChangedStartTime;
                double dPanelLen = 0;
                while (dPanelLen <= dAvailableWidth && curDateTime <= ChangedEndTime)
                {
                    dPanelLen += panel.SymbolsPerUnit * iSymbolWidth;
                    curDateTime = panel.IncreaseTime(curDateTime);
                }

                if (dPanelLen > dAvailableWidth) continue;
                MinPanel = panel;
                break;
            }
            if (MinPanel == null)
                return;

            context.PlotPanelSeparator(0);
            double dTotalSeconds = (ChangedEndTime - ChangedStartTime).TotalSeconds;
            _plotingUnitWidth = dAvailableWidth / dTotalSeconds;
            double dx;
            string sText;

            //main panel
            curDateTime = ChangedStartTime;
            if (m_datesCargo.Count == 0) //even scale
            {
                for (; curDateTime <= ChangedEndTime; curDateTime = MinPanel.IncreaseTime(curDateTime))
                {
                    dx = (curDateTime - ChangedStartTime).TotalSeconds * _plotingUnitWidth;
                    context.PlotUnitSeparator((float)dx, true, 0);
                    sText = MinPanel.FormatDateTime(curDateTime);
                    if (curDateTime < ChangedEndTime && dx + context.GetTextWidth(sText) < dAvailableWidth)
                        context.PlotUnitText((float)dx, sText, 0);
                }
            }
            else
            {
                double dTotalCargo = 0;
                foreach (KeyValuePair<DateTime, double> pair in m_datesCargo)
                {
                    dTotalCargo += pair.Value;
                }
                double dPrevTotalWidth = 0;
                foreach (KeyValuePair<DateTime, double> pair in m_datesCargo)
                {
                    double dUnitCargo = pair.Value;
                    double dUnitWidth = (dUnitCargo * dAvailableWidth) / dTotalCargo;
                    if (MinPanel.SymbolsPerUnit * iSymbolWidth > dUnitWidth) continue;
                    context.PlotUnitSeparator((float)dPrevTotalWidth, true, 0);
                    context.PlotUnitText((float)dPrevTotalWidth, MinPanel.FormatDateTime(pair.Key), 0);
                    dPrevTotalWidth += dUnitWidth;
                }
            }

            //Sub Panel painting
            if (MinPanel.DateFormats == null || MinPanel.GetNextSubPanelSeparator == null) return;
            context.PlotPanelSeparator(1);
            curDateTime = ChangedStartTime;
            double firstWidth = 0;
            DateTime prevSeparator = DateTime.MinValue;

            for (; curDateTime <= ChangedEndTime; curDateTime = MinPanel.GetNextSubPanelSeparator(curDateTime))
            {
                if (curDateTime == ChangedStartTime) //at the first position draw the longest possible, this my overlay some of next ones, they must be ignored
                {
                    context.PlotUnitSeparator(0, true, 1);
                    if ((sText = FitDateTime(context, dAvailableWidth, curDateTime, MinPanel.DateFormats, out firstWidth)) != string.Empty)
                        context.PlotUnitText(0, sText, 1);
                }
                else
                {
                    //if (!MinPanel.IsSubPanelSeparator(curDateTime)) continue;
                    dx = (curDateTime - ChangedStartTime).TotalSeconds * _plotingUnitWidth;
                    if (dx <= firstWidth) continue; //ignore those separatoes that are overlayed by first one
                    context.PlotUnitSeparator((float)dx, true, 1);
                    if (prevSeparator == DateTime.MinValue)
                    {
                        prevSeparator = curDateTime;
                        continue;
                    }
                    PlotSubPanelText(context, dAvailableWidth, curDateTime, _plotingUnitWidth, out firstWidth, prevSeparator);
                    prevSeparator = curDateTime;//remember last position
                }
            }
            if (prevSeparator == DateTime.MinValue) return;
            PlotSubPanelText(context, dAvailableWidth, curDateTime, _plotingUnitWidth, out firstWidth, prevSeparator);
        }

        private void PlotSubPanelText(IRenderDevice context, double dAvailableWidth, DateTime curDateTime, double dPlotingUnitWidth, out double firstWidth, DateTime prevSeparator)
        {
            string sText;
            double dNextAvailableWidth = (curDateTime - prevSeparator).TotalSeconds * dPlotingUnitWidth;
            double dx = (prevSeparator - ChangedStartTime).TotalSeconds * dPlotingUnitWidth;
            dNextAvailableWidth = (dx + dNextAvailableWidth < dAvailableWidth)
                                    ? dNextAvailableWidth
                                    : (dAvailableWidth - dx);
            if ((sText = FitDateTime(context, dNextAvailableWidth, prevSeparator, MinPanel.DateFormats, out firstWidth)) != string.Empty)
                context.PlotUnitText((float)dx, sText, 1);
        }

        private static string FitDateTime(IRenderDevice context, double dAvailableWidth, DateTime datetime, IEnumerable<string> dateTimeFormats,
          out double dFoundWidth)
        {
            dFoundWidth = 0;
            foreach (string format in dateTimeFormats)
            {
                string sText = datetime.ToString(format);
                double textWidth = context.GetTextWidth(sText);
                if (textWidth >= dAvailableWidth) continue;
                dFoundWidth = textWidth;
                return sText;
            }
            return string.Empty;
        }
    }

    internal interface IRenderDevice
    {
        void Reset();
        float SymbolHeight { get; }
        float SymbolWidth { get; }
        float AvailableWidth { get; }
        float GetTextWidth(string Text);
        void PlotPanelSeparator(int SeparatorIndex);
        void PlotUnitSeparator(float X, bool Long, int PanelIndex);
        void PlotUnitText(float X, string Text, int PanelIndex);
    }
}
