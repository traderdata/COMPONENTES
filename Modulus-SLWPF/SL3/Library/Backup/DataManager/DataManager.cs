using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ModulusFE.Data;
using ModulusFE.Exceptions;
using ModulusFE.Indicators;
#if SILVERLIGHT
using ModulusFE.SL.Utils;
#endif
#if WPF
using System.Text;
using System.Xml;
#endif

namespace ModulusFE.DataManager
{
    /// <summary>
    /// Gets the operation for volume tick appending
    /// </summary>
    public enum AppendTickVolumeBehavior
    {
        /// <summary>
        /// With every tick volume value is incremented by the new passed value
        /// </summary>
        Increment,
        /// <summary>
        /// At every tick the current volume value is replaced by the new one
        /// </summary>
        Change
    }

    internal partial class DataManager
    {
        private readonly StockChartX _chart;

        public class TickEntry
        {
            public DateTime _timeStamp;
            public double _lastPrice;
            public double _lastVolume;
        }
        /// <summary>
        /// Contains tick values for all symbols on chart
        /// </summary>
        private Dictionary<string, List<TickEntry>> _tickValues = new Dictionary<string, List<TickEntry>>(10);

        private int _tickPeriodicity = 5; //periodicity when compressing bars - seconds
        private TickCompressionEnum _tickCompressionType = TickCompressionEnum.Time;

        /// <summary>
        /// This list will have all the timestamps involved in the chart, series will have only a list of doubles to keep their values
        /// </summary>
        private SortedList<DateTime, int> _timestamps = new SortedList<DateTime, int>();
        private Dictionary<DateTime, int> _timestampIndexes = new Dictionary<DateTime, int>();

        /// <summary>
        /// Holds series indexes by their OHLC type and name
        /// </summary>
        private readonly Dictionary<SeriesTypeOHLC, Dictionary<string, int>> _seriesToIndex =
            new Dictionary<SeriesTypeOHLC, Dictionary<string, int>>(100);

        private readonly List<SeriesEntry> _seriesList = new List<SeriesEntry>();

        public DataManager(StockChartX chart)
        {
            _chart = chart;
        }

        public int RecordCount
        {
            get { return _timestamps.Count; }
        }

        public int TickPeriodicity
        {
            get { return _tickPeriodicity; }
            set
            {
                if (value < 5)
                    throw new ArgumentOutOfRangeException();

                if (_tickPeriodicity == value) return;
                _tickPeriodicity = value;
                // no need to Recompress at property change. Too much overhead in case the TickCompressionType changes as well.
                // User will method Chart.CompressTicks when he wants
                //if (_chart.ChartType == ChartTypeEnum.Tick) 
                //	ReCompressTicks();
            }
        }

        public TickCompressionEnum TickCompressionType
        {
            get { return _tickCompressionType; }
            set
            {
                if (_tickCompressionType == value) return;
                _tickCompressionType = value;
                // no need to Recompress at property change. Too much overhead in case the TickPeriodicity changes as well.
                // User will method Chart.CompressTicks when he wants
                //if (_chart.ChartType == ChartTypeEnum.Tick) 
                //	ReCompressTicks();
            }
        }

        public bool SeriesExists(string seriesName, SeriesTypeOHLC ohlcType)
        {
            Dictionary<string, int> nameIndex;
            return (_seriesToIndex.TryGetValue(ohlcType, out nameIndex) && nameIndex.ContainsKey(seriesName));
        }

        public int SeriesIndex(string seriesName, SeriesTypeOHLC ohlcType)
        {
            if (!SeriesExists(seriesName, ohlcType)) return -1;
            return _seriesToIndex[ohlcType][seriesName];
        }

        public void BindSeries(Series series)
        {
            lock (_seriesList)
            {
                series.SeriesIndex = _seriesToIndex[series.OHLCType][series.Name];
                _seriesList[series.SeriesIndex].Series = series;
            }
        }

        private void RegisterSeriesIndex(string seriesName, SeriesTypeOHLC ohlcType, int index)
        {
            Dictionary<string, int> name2Index;
            if (!_seriesToIndex.TryGetValue(ohlcType, out name2Index))
            {
                _seriesToIndex[ohlcType] = new Dictionary<string, int>();
            }

            _seriesToIndex[ohlcType][seriesName] = index;
        }

        internal void UnRegisterSeries(string seriesName, SeriesTypeOHLC ohlcType)
        {
            Dictionary<string, int> nameIndex;
            if (!_seriesToIndex.TryGetValue(ohlcType, out nameIndex))
            {
                return;
            }

            if (!nameIndex.ContainsKey(seriesName))
            {
                return;
            }

            int seriesIndex = nameIndex[seriesName];
            nameIndex.Remove(seriesName);
            _seriesList.RemoveAt(seriesIndex);

            //re-index other series
            for (int i = 0; i < _seriesList.Count; i++)
            {
                SeriesEntry seriesEntry = _seriesList[i];

                _seriesToIndex[seriesEntry.Series.OHLCType][seriesEntry.Series.Name] =
                    seriesEntry.Series.SeriesIndex = i;
            }

            //removed series might be an input parameter for on of the indicators from the chart
            //go over all indicator and check
            foreach (ChartPanel chartPanel in _chart._panelsContainer.Panels)
            {
                foreach (Indicator indicator in chartPanel.IndicatorsCollection)
                {
                    if (indicator.FullName == seriesName)
                    {
                        continue;
                    }

                    indicator._calculated = false;
                    indicator.Calculate();
                }
            }

            _chart.ReCalc = true;
        }

        public void AddOHLCSeries(string seriesName)
        {
            SeriesEntry seriesEntry;

            _seriesList.Add(seriesEntry = new SeriesEntry { Name = seriesName });
            InitSeries(seriesEntry);
            RegisterSeriesIndex(seriesName, SeriesTypeOHLC.Open, _seriesList.Count - 1);

            _seriesList.Add(seriesEntry = new SeriesEntry { Name = seriesName });
            InitSeries(seriesEntry);
            RegisterSeriesIndex(seriesName, SeriesTypeOHLC.High, _seriesList.Count - 1);

            _seriesList.Add(seriesEntry = new SeriesEntry { Name = seriesName });
            InitSeries(seriesEntry);
            RegisterSeriesIndex(seriesName, SeriesTypeOHLC.Low, _seriesList.Count - 1);

            _seriesList.Add(seriesEntry = new SeriesEntry { Name = seriesName });
            InitSeries(seriesEntry);
            RegisterSeriesIndex(seriesName, SeriesTypeOHLC.Close, _seriesList.Count - 1);
        }

        public void AddHLCSeries(string seriesName)
        {
            SeriesEntry seriesEntry;
            _seriesList.Add(seriesEntry = new SeriesEntry { Name = seriesName });
            InitSeries(seriesEntry);
            RegisterSeriesIndex(seriesName, SeriesTypeOHLC.High, _seriesList.Count - 1);

            _seriesList.Add(seriesEntry = new SeriesEntry { Name = seriesName });
            InitSeries(seriesEntry);
            RegisterSeriesIndex(seriesName, SeriesTypeOHLC.Low, _seriesList.Count - 1);

            _seriesList.Add(seriesEntry = new SeriesEntry { Name = seriesName });
            InitSeries(seriesEntry);
            RegisterSeriesIndex(seriesName, SeriesTypeOHLC.Close, _seriesList.Count - 1);
        }

        public int AddSeries(string seriesName, SeriesTypeOHLC ohlcType)
        {
            SeriesEntry seriesEntry;
            _seriesList.Add(seriesEntry = new SeriesEntry
                                                                            {
                                                                                Name = seriesName
                                                                            });
            InitSeries(seriesEntry);
            RegisterSeriesIndex(seriesName, ohlcType, _seriesList.Count - 1);

            return _seriesList.Count - 1;
        }

        public void AppendOHLCValues(string seriesName, DateTime timeStamp, double? open, double? high, double? low, double? close)
        {
            if (!SeriesExists(seriesName, SeriesTypeOHLC.Open))
            {
                throw new SeriesDoesNotExistsException(seriesName);
            }

            AppendValue(_seriesToIndex[SeriesTypeOHLC.Open][seriesName], timeStamp, open);
            AppendValue(_seriesToIndex[SeriesTypeOHLC.High][seriesName], timeStamp, high);
            AppendValue(_seriesToIndex[SeriesTypeOHLC.Low][seriesName], timeStamp, low);
            AppendValue(_seriesToIndex[SeriesTypeOHLC.Close][seriesName], timeStamp, close);
        }

        public void AppendHLCValues(string seriesName, DateTime timeStamp, double? high, double? low, double? close)
        {
            if (!SeriesExists(seriesName, SeriesTypeOHLC.High))
            {
                throw new SeriesDoesNotExistsException(seriesName);
            }

            AppendValue(_seriesToIndex[SeriesTypeOHLC.High][seriesName], timeStamp, high);
            AppendValue(_seriesToIndex[SeriesTypeOHLC.Low][seriesName], timeStamp, low);
            AppendValue(_seriesToIndex[SeriesTypeOHLC.Close][seriesName], timeStamp, close);
        }

        /// <summary>
        /// Appends a value to a series
        /// </summary>
        /// <param name="seriesName"></param>
        /// <param name="ohlcType"></param>
        /// <param name="timeStamp"></param>
        /// <param name="value"></param>
        public void AppendValue(string seriesName, SeriesTypeOHLC ohlcType, DateTime timeStamp, double? value)
        {
            if (!SeriesExists(seriesName, ohlcType))
            {
                throw new SeriesDoesNotExistsException(seriesName);
            }

            AppendValue(_seriesToIndex[ohlcType][seriesName], timeStamp, value);
        }

        public double? GetValue(int seriesIndex, int recordIndex)
        {
            return _seriesList[seriesIndex].Data[recordIndex].Value;
        }

        public DataEntryCollection GetDataCollection(int seriesIndex)
        {
            return _seriesList[seriesIndex].Data;
        }

        internal SeriesEntry this[int seriesIndex]
        {
            get
            {
                return _seriesList[seriesIndex];
            }
        }

        internal List<double?>[] GetGroupedOhlcValues(int[] ohlcSeriesIndexes)
        {
            SeriesEntry open = this[ohlcSeriesIndexes[0]];
            SeriesEntry high = this[ohlcSeriesIndexes[1]];
            SeriesEntry low = this[ohlcSeriesIndexes[2]];
            SeriesEntry close = this[ohlcSeriesIndexes[3]];

            List<double?>[] res = new[]
																{
																	new List<double?>(),
																	new List<double?>(),
																	new List<double?>(),
																	new List<double?>()
																};

            for (int i = _chart._startIndex; i < _chart._endIndex; i += _chart._groupingInterval)
            {
                res[0].Add(open[i].Value);
                res[1].Add(high.Max(i, _chart._groupingInterval));
                res[2].Add(low.Min(i, _chart._groupingInterval));
                res[3].Add(close.Last(i, _chart._groupingInterval));
            }

            return res;
        }

        internal List<double?> GetGroupedVolumeValues(int volumeSeriesIndex)
        {
            SeriesEntry volume = this[volumeSeriesIndex];
            List<double?> res = new List<double?>();

            for (int i = _chart._startIndex; i < _chart._endIndex; i += _chart._groupingInterval)
            {
                res.Add(volume.Sum(i, _chart._groupingInterval));
            }

            return res;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="timestamp"></param>
        /// <returns>index of the timestamp, -1 is such a timestamp doesn't exists</returns>
        public int GetTimeStampIndex(DateTime timestamp)
        {
            return _timestamps.ContainsKey(timestamp) ? _timestamps.IndexOfKey(timestamp) : -1;
        }

        /// <summary>
        /// Returns an aproximated record index for a given timeStamp.
        /// Usefull when an exact timestamp is unknown
        /// </summary>
        /// <param name="timestamp"></param>
        /// <param name="roundUp">
        /// if true - returns the next recordCount that has a value greater then given timestamp
        /// if false - returns the previos recordCount that has a value less then given timestamp
        /// </param>
        /// <returns>Timestamp index, -1 if no such aproximate value</returns>
        public int GetTimeStampIndex(DateTime timestamp, bool roundUp)
        {
#if WPF
			long ticks = timestamp.ToBinary();
#endif
#if SILVERLIGHT
            double ticks = timestamp.ToJDate();
#endif
            for (int i = 0; i < _timestamps.Keys.Count; i++)
            {
#if WPF
				long t = _timestamps.Keys[i].ToBinary();
#endif
#if SILVERLIGHT
                double t = _timestamps.Keys[i].ToJDate();
#endif
                if (t == ticks)
                    return i;
                if (t < ticks)
                    continue;
                if (roundUp)
                    return i;
                return i - 1;
            }
            return -1;
        }
        /// <summary>
        /// Returns TimeStamp by index. 
        /// If index is out of range it returns DateTime.MinValue
        /// Doesn't take care of chartX._startIndex
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public DateTime GetTimeStampByIndex(int index)
        {
            return (index < _timestamps.Count && index >= 0) ? _timestamps.Keys[index] : DateTime.MinValue;
        }

        internal DateTime TS(int index)
        {
            return _timestamps.Keys[index];
        }

        internal void GetStartEndTimeStamp(out DateTime timeStampStart, out DateTime timeStampEnd)
        {
            timeStampStart = timeStampEnd = DateTime.MinValue;
            if (_timestamps.Count == 0) return;
            timeStampStart = _timestamps.Keys[_chart._startIndex];
            timeStampEnd = _timestamps.Keys[_chart._endIndex > 0 ? _chart._endIndex - 1 : _chart._endIndex];
        }

        public void ClearAll()
        {
            _timestamps.Clear();
            _timestampIndexes.Clear();
            _seriesList.Clear();
            _tickValues.Clear();
            _seriesToIndex.Clear();
        }

        public void ClearData()
        {
            _timestamps.Clear();
            _timestampIndexes.Clear();
            _tickValues.Clear();
            //clear series data
            foreach (SeriesEntry entry in _seriesList)
            {
                entry.Data.Clear();
            }
        }

        public void AppendValue(int seriesIndex, DateTime timeStamp, double? value)
        {
            //      if (_chart.ChartType == Chart.StockChartX.ChartTypeEnum.Tick)
            //      {
            //        MessageBox.Show("At the moment chart accepts only tick values.", "Error", MessageBoxButton.OK,
            //                        MessageBoxImage.Error);
            //        return;
            //      }

            int newIndex;
            bool updateLineStudies = false;

            if (!_timestampIndexes.ContainsKey(timeStamp))
            {
                _timestampIndexes[timeStamp] = 1; //save the timestamp


                if (_chart._endIndex == _timestamps.Count) //chart is not shrinked
                {
                    _chart._endIndex = _timestamps.Count + 1;
                }
                else
                {
                    _chart._endIndex++;
                }

                if (_chart.KeepZoomLevel && _chart._endIndex != _timestamps.Count)
                {
                    _chart._startIndex++;
                }

                if (_chart._endIndex - _chart._startIndex > _chart.MaxVisibleRecords &&
                    _chart.MaxVisibleRecords != 0)
                {
                    _chart._startIndex = _chart._endIndex - _chart.MaxVisibleRecords;
                }

                _timestamps.Add(timeStamp, 1);

                newIndex = _timestamps.IndexOfKey(timeStamp);

                foreach (SeriesEntry t in _seriesList)
                {
                    t.Data.Insert(newIndex,
                                                new DataEntry(null)
                                                    {
                                                        _collectionOwner = t.Data,
                                                        _dataManager = this
                                                    });

                    t._dataChanged = true;
                    t._visibleDataChanged = true;

                    // update index value
                    for (int j = newIndex; j < t.Data.Count; j++)
                    {
                        t.Data[j].Index = j;
                    }
                }

                updateLineStudies = true;
                _chart.ReCalc = true;
                _chart.OnPropertyChanged(StockChartX.Property_NewRecord);
            }
            else
            {
                newIndex = _timestamps.IndexOfKey(timeStamp);
            }

            _seriesList[seriesIndex].Data[newIndex].Value = value;
            _seriesList[seriesIndex]._dataChanged = true;

            if (newIndex == RecordCount - 1 && value.HasValue)
            {
                _seriesList[seriesIndex].Series.InvokePropertyChanged(Series.PropertyLastValue);
            }

            //TODO Why this was added and who did it?
            if (updateLineStudies)
            {
                foreach (var chartPanel in _chart._panelsContainer.Panels)
                {
                    foreach (var lineStudy in chartPanel._lineStudies)
                    {
                        //lineStudy.UpdatePosition(newIndex);
                    }
                }
            }
        }

        public TickEntry GetLastTickEntry(string symbolName)
        {
            List<TickEntry> tickList;
            if (_tickValues.TryGetValue(symbolName, out tickList))
                return tickList.Last();
            return null;
        }

        public bool AppendTickValue(string symbolName, DateTime timeStamp, double value, double volumeValue)
        {
            if (RecordCount > 0 && _timestamps.Keys[RecordCount - 1] > timeStamp)
                return false; //the new timestamp must be greater then last timestamp 

            SeriesEntry open = SeriesExists(symbolName, SeriesTypeOHLC.Open)
                                                     ? this[_seriesToIndex[SeriesTypeOHLC.Open][symbolName]]
                                                     : null;
            SeriesEntry high = SeriesExists(symbolName, SeriesTypeOHLC.High)
                                                     ? this[_seriesToIndex[SeriesTypeOHLC.High][symbolName]]
                                                     : null;
            SeriesEntry low = SeriesExists(symbolName, SeriesTypeOHLC.Low)
                                                     ? this[_seriesToIndex[SeriesTypeOHLC.Low][symbolName]]
                                                     : null;
            SeriesEntry close = SeriesExists(symbolName, SeriesTypeOHLC.Close)
                                                     ? this[_seriesToIndex[SeriesTypeOHLC.Close][symbolName]]
                                                     : null;
            SeriesEntry volume = SeriesExists(symbolName, SeriesTypeOHLC.Volume)
                                                     ? this[_seriesToIndex[SeriesTypeOHLC.Volume][symbolName]]
                                                     : null;

            List<TickEntry> tickList;
            if (!_tickValues.TryGetValue(symbolName, out tickList))
            {
                _tickValues[symbolName] = (tickList = new List<TickEntry>());
            }

            // store in local memory all tick entries, they might be used later to compress with a different tick periodicity
            tickList.Add(new TickEntry { _timeStamp = timeStamp, _lastPrice = value, _lastVolume = volumeValue });

            if (RecordCount == 0) //no records yet, create the first entry
            {
                if (open != null)
                {
                    AppendValue(open.Series.SeriesIndex, timeStamp, value);
                    open.Series.InvokePropertyChanged(Series.PropertyLastTick);
                }

                if (high != null)
                {
                    AppendValue(high.Series.SeriesIndex, timeStamp, value);
                    high.Series.InvokePropertyChanged(Series.PropertyLastTick);
                }

                if (low != null)
                {
                    AppendValue(low.Series.SeriesIndex, timeStamp, value);
                    low.Series.InvokePropertyChanged(Series.PropertyLastTick);
                }

                if (close != null)
                {
                    AppendValue(close.Series.SeriesIndex, timeStamp, value);
                    close.Series.InvokePropertyChanged(Series.PropertyLastTick);
                }

                if (volume != null)
                {
                    AppendValue(volume.Series.SeriesIndex, timeStamp, volumeValue);
                    volume.Series.InvokePropertyChanged(Series.PropertyLastTick);
                }

                return false;
            }

            bool newCandle = false;
            DateTime newCandleTimeStamp = timeStamp;

            switch (_tickCompressionType)
            {
                case TickCompressionEnum.Time:
                    if (_tickPeriodicity <= 0)
                    {
                        newCandle = true;
                        break;
                    }
                    double wholeNumberOfPeriods = Math.Floor((timeStamp - _timestamps.Keys[RecordCount - 1]).TotalSeconds / _tickPeriodicity);
                    if (wholeNumberOfPeriods >= 1)
                    {
                        newCandle = true;

                        // The new candle time needs to be on the interval of the _tickPeriodicity.
                        newCandleTimeStamp = _timestamps.Keys[RecordCount - 1].AddSeconds(_tickPeriodicity * wholeNumberOfPeriods);
                    }
                    break;
                case TickCompressionEnum.Ticks:
                    newCandle = _tickValues[symbolName].Count % _tickPeriodicity == 0;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("");
            }


            if (newCandle) //just create a new candle and initialize all needed values
            {

                if (open != null)
                {
                    AppendValue(open.Series.SeriesIndex, newCandleTimeStamp, value);
                    open.Series.InvokePropertyChanged(Series.PropertyLastTick);
                }

                if (high != null)
                {
                    AppendValue(high.Series.SeriesIndex, newCandleTimeStamp, value);
                    high.Series.InvokePropertyChanged(Series.PropertyLastTick);
                }

                if (low != null)
                {
                    AppendValue(low.Series.SeriesIndex, newCandleTimeStamp, value);
                    low.Series.InvokePropertyChanged(Series.PropertyLastTick);
                }

                if (close != null)
                {
                    AppendValue(close.Series.SeriesIndex, newCandleTimeStamp, value);
                    close.Series.InvokePropertyChanged(Series.PropertyLastTick);
                }

                if (volume != null)
                {
                    AppendValue(volume.Series.SeriesIndex, newCandleTimeStamp, volumeValue);
                    volume.Series.InvokePropertyChanged(Series.PropertyLastTick);
                }

                //Debug.WriteLine("A new candle was created");
                return true;
            }

            if (high != null && value > high.Data[RecordCount - 1].Value) //find highest value
            {
                high.Data[RecordCount - 1].Value = value;
                high.Series.InvokePropertyChanged(Series.PropertyLastTick);
            }

            if (low != null && value < low.Data[RecordCount - 1].Value) //find the lowest value
            {
                low.Data[RecordCount - 1].Value = value;
                low.Series.InvokePropertyChanged(Series.PropertyLastTick);
            }

            if (close != null)
            {
                close.Data[RecordCount - 1].Value = value; //update close value
                close.Series.InvokePropertyChanged(Series.PropertyLastTick);
            }

            if (volume != null)
            {
                switch (_chart.AppendTickVolumeBehavior)
                {
                    case AppendTickVolumeBehavior.Increment:
                        volume.Data[RecordCount - 1].Value += volumeValue; //increment volume
                        break;
                    case AppendTickVolumeBehavior.Change:
                        volume.Data[RecordCount - 1].Value = volumeValue; //change volume
                        break;
                }

                volume.Series.InvokePropertyChanged(Series.PropertyLastTick);
            }

            return false;
        }

        internal void ReCompressTicks()
        {
            foreach (KeyValuePair<string, List<TickEntry>> pair in _tickValues)
            {
                string symbolName = pair.Key;

                SeriesEntry open = SeriesExists(symbolName, SeriesTypeOHLC.Open)
                                                     ? this[_seriesToIndex[SeriesTypeOHLC.Open][symbolName]]
                                                     : null;
                SeriesEntry high = SeriesExists(symbolName, SeriesTypeOHLC.High)
                                                         ? this[_seriesToIndex[SeriesTypeOHLC.High][symbolName]]
                                                         : null;
                SeriesEntry low = SeriesExists(symbolName, SeriesTypeOHLC.Low)
                                                         ? this[_seriesToIndex[SeriesTypeOHLC.Low][symbolName]]
                                                         : null;
                SeriesEntry close = SeriesExists(symbolName, SeriesTypeOHLC.Close)
                                                         ? this[_seriesToIndex[SeriesTypeOHLC.Close][symbolName]]
                                                         : null;
                SeriesEntry volume = SeriesExists(symbolName, SeriesTypeOHLC.Volume)
                                                         ? this[_seriesToIndex[SeriesTypeOHLC.Volume][symbolName]]
                                                         : null;

                if (open == null && high == null && low == null && close == null) continue;
                if (open != null) ClearValues(open.Series.SeriesIndex);
                if (high != null) ClearValues(high.Series.SeriesIndex);
                if (low != null) ClearValues(low.Series.SeriesIndex);
                if (close != null) ClearValues(close.Series.SeriesIndex);
                if (volume != null) ClearValues(volume.Series.SeriesIndex);

                int tickIndex = 0;
                DateTime newCandleTimeStamp = DateTime.MinValue;
                foreach (TickEntry tickEntry in pair.Value)
                {
                    bool newCandle = false;

                    switch (_tickCompressionType)
                    {
                        case TickCompressionEnum.Ticks:
                            if (tickIndex % _tickPeriodicity == 0)
                            {
                                newCandle = true;
                                newCandleTimeStamp = tickEntry._timeStamp;
                            }
                            break;
                        case TickCompressionEnum.Time:
                            if (tickIndex == 0 || _tickPeriodicity <= 0)
                            {
                                newCandle = true;
                                newCandleTimeStamp = tickEntry._timeStamp;
                                break;
                            }
                            double wholeNumberOfPeriods = Math.Floor((tickEntry._timeStamp - newCandleTimeStamp).TotalSeconds / _tickPeriodicity);
                            if (wholeNumberOfPeriods >= 1)
                            {
                                newCandle = true;
                                newCandleTimeStamp = newCandleTimeStamp.AddSeconds(_tickPeriodicity * wholeNumberOfPeriods);
                            }
                            break;
                        default:
                            throw new ArgumentOutOfRangeException("");
                    }
                    tickIndex++;
                    if (newCandle) //create a new candle
                    {
                        if (open != null) AppendValue(open.Series.SeriesIndex, newCandleTimeStamp, tickEntry._lastPrice);
                        if (high != null) AppendValue(high.Series.SeriesIndex, newCandleTimeStamp, tickEntry._lastPrice);
                        if (low != null) AppendValue(low.Series.SeriesIndex, newCandleTimeStamp, tickEntry._lastPrice);
                        if (close != null) AppendValue(close.Series.SeriesIndex, newCandleTimeStamp, tickEntry._lastPrice);
                        if (volume != null) AppendValue(volume.Series.SeriesIndex, newCandleTimeStamp, tickEntry._lastVolume);
                        continue;
                    }

                    if (high != null && tickEntry._lastPrice > high.Data[RecordCount - 1].Value) //find highest value
                        high.Data[RecordCount - 1].Value = tickEntry._lastPrice;
                    if (low != null && tickEntry._lastPrice < low.Data[RecordCount - 1].Value) //find the lowest value
                        low.Data[RecordCount - 1].Value = tickEntry._lastPrice;
                    if (close != null)
                        close.Data[RecordCount - 1].Value = tickEntry._lastPrice; //update close value
                    if (volume != null)
                        volume.Data[RecordCount - 1].Value += tickEntry._lastVolume; //increment volume
                }
            }
            _chart.ReCalc = true;
        }

        public void GetMinMax(int seriesIndex, out double min, out double max)
        {
            SeriesEntry seriesEntry = this[seriesIndex];
            if (!seriesEntry._dataChanged)
            {
                min = seriesEntry._min;
                max = seriesEntry._max;
                return;
            }

            min = double.MaxValue;
            max = double.MinValue;

            foreach (DataEntry dataEntry in this[seriesIndex].Data)
            {
                if (!dataEntry.Value.HasValue)
                {
                    continue;
                }

                if (dataEntry.Value.Value > max)
                {
                    max = dataEntry.Value.Value;
                }
                else if (dataEntry.Value.Value < min)
                {
                    min = dataEntry.Value.Value;
                }
            }

            seriesEntry._min = min;
            seriesEntry._max = max;
            seriesEntry._dataChanged = false;
        }

        public double Max(int seriesIndex)
        {
            double res = double.MinValue;
            foreach (DataEntry dataEntry in this[seriesIndex].Data)
            {
                if (dataEntry.Value.HasValue && dataEntry.Value.Value > res)
                {
                    res = dataEntry.Value.Value;
                }
            }

            return res;
        }

        public double Min(int seriesIndex)
        {
            double res = double.MaxValue;
            foreach (DataEntry dataEntry in this[seriesIndex].Data)
            {
                if (dataEntry.Value.HasValue && dataEntry.Value.Value < res)
                {
                    res = dataEntry.Value.Value;
                }
            }

            return res;
        }

        public double MaxFromInterval(int seriesIndex, ref int startIndex, ref int endIndex)
        {
            DataEntryCollection data = this[seriesIndex].Data;

            endIndex = Math.Min(endIndex, RecordCount);
            startIndex = Math.Min(Math.Max(startIndex, 0), endIndex);
            double res = double.MinValue;
            for (int i = startIndex; i < endIndex; i++)
            {
                double? value = data[i].Value;
                if (value.HasValue && value.Value > res)
                {
                    res = value.Value;
                }
            }

            return res;
        }

        public double MinFromInterval(int seriesIndex, ref int startIndex, ref int endIndex)
        {
            DataEntryCollection data = this[seriesIndex].Data;

            endIndex = Math.Min(endIndex, RecordCount);
            startIndex = Math.Min(Math.Max(startIndex, 0), endIndex);
            double res = double.MaxValue;
            for (int i = startIndex; i < endIndex; i++)
            {
                double? value = data[i].Value;
                if (value.HasValue && value.Value < res)
                {
                    res = value.Value;
                }
            }

            return res;
        }

        /// <summary>
        /// Returns min &amp; max for a given interval for needed series. 
        /// Does not check the validity of interval!!!
        /// </summary>
        /// <param name="seriesIdxs"></param>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public void MinMaxFromInterval(IEnumerable<int> seriesIdxs, int startIndex, int endIndex,
            out double min, out double max)
        {
            min = double.MaxValue;
            max = double.MinValue;

            foreach (DataEntryCollection data in seriesIdxs.Select(seriesIdx => this[seriesIdx].Data))
            {
                min = Math.Min(min, data.Skip(startIndex)
                    .Take(endIndex - startIndex)
                    .Where(_ => _.Value.HasValue)
                    .Select(_ => _.Value.Value)
                    .Min());
                max = Math.Max(max, data.Skip(startIndex)
                    .Take(endIndex - startIndex)
                    .Where(_ => _.Value.HasValue)
                    .Select(_ => _.Value.Value)
                    .Max());
            }
        }

        /// <summary>
        /// Gets the min &amp; max values from all supplied series for a given X index
        /// </summary>
        /// <param name="seriesIdx"></param>
        /// <param name="index"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public void MinMaxFromIndex(IEnumerable<int> seriesIdx, int index, out double min, out double max)
        {
            var seriesIdxLocal = seriesIdx.ToList();
            min = seriesIdxLocal.Select(_ => this[_].Data[index]).Where(_ => _.Value.HasValue).Min(_ => _.Value.Value);
            max = seriesIdxLocal.Select(_ => this[_].Data[index]).Where(_ => _.Value.HasValue).Max(_ => _.Value.Value);
        }

        public void VisibleMinMax(int seriesIndex, out double min, out double max)
        {
            SeriesEntry seriesEntry = this[seriesIndex];
            if (!seriesEntry._visibleDataChanged)
            {
                min = seriesEntry._visibleMin;
                max = seriesEntry._visibleMax;
                return;
            }

            int startIndex = _chart._startIndex;
            int endIndex = _chart._endIndex;

            DataEntryCollection data = this[seriesIndex].Data;

            endIndex = Math.Min(endIndex, RecordCount);
            startIndex = Math.Min(Math.Max(startIndex, 0), endIndex);
            min = double.MaxValue;
            max = double.MinValue;
            for (int i = startIndex; i < endIndex; i++)
            {
                double? value = data[i].Value;
                if (value.HasValue)
                {
                    min = Math.Min(value.Value, min);
                    max = Math.Max(value.Value, max);
                }
            }

            if (seriesEntry.Series._seriesTypeOHLC != SeriesTypeOHLC.Volume)
            {
                seriesEntry._visibleMin = min;
            }
            else
            {
                seriesEntry._visibleMin = min = 0;
            }

            seriesEntry._visibleMax = max;
            //seriesEntry._visibleDataChanged = false; //TODO
        }

        public void ClearValues(int seriesIndex)
        {
            SeriesEntry seriesEntry = this[seriesIndex];

            foreach (DataEntry t in seriesEntry.Data)
            {
                t.Value = null;
            }
        }

        public DataEntry LastVisibleDataEntry(int seriesIndex)
        {
            return LastVisibleDataEntry(seriesIndex, false);
        }

        public DataEntry LastVisibleDataEntry(int seriesIndex, bool ensureLatest)
        {
            // Add a check
            if (seriesIndex < 0 || seriesIndex >= _seriesList.Count)
            {
                return new DataEntry(null);
            }

            SeriesEntry seriesEntry = this[seriesIndex];
            // Add upper index check
            if (_chart._endIndex <= 0 || RecordCount == 0 || _chart._endIndex > seriesEntry.Data.Count)
            {
                if (!ensureLatest || !seriesEntry.Data.Any())
                {
                    return new DataEntry(null);
                }

                return seriesEntry.Data.Last(_ => _.Value.HasValue);
            }

            DataEntry dataEntry = seriesEntry.Data[_chart._endIndex - 1];
            if (!dataEntry.Value.HasValue && ensureLatest)
            {
                dataEntry = seriesEntry.Data.LastOrDefault(_ => _.Value.HasValue);
                return dataEntry ?? new DataEntry(null);
            }

            return dataEntry;
        }


        private void InitSeries(SeriesEntry seriesEntry)
        {
            //if a series was added after some other series we must append null values
            for (int i = seriesEntry.Data.Count; i < _timestamps.Count; i++)
            {
                seriesEntry.Data.Add(new DataEntry(null) { _collectionOwner = seriesEntry.Data, _dataManager = this, Index = i });
            }
            seriesEntry._visibleDataChanged = true;
        }

#if WPF
		internal void SaveToXml(XmlNode xRoot, XmlDocument xmlDocument)
		{
			XmlNode xmlData = xmlDocument.CreateElement("SeriesData");
			xRoot.AppendChild(xmlData);

			XmlNode xmlTimeStamps = xmlDocument.CreateElement("TimeStamps");
			xmlData.AppendChild(xmlTimeStamps);
			StringBuilder sb = new StringBuilder(1000);
			foreach (KeyValuePair<DateTime, int> pair in _timestamps)
			{
				sb.Append(pair.Key.ToBinary()).Append(" ");
			}
			xmlTimeStamps.InnerText = sb.ToString();

			XmlNode xmlTickData = xmlDocument.CreateElement("TickData");
			xmlData.AppendChild(xmlTickData);
			sb.Length = 0;
			foreach (KeyValuePair<string, List<TickEntry>> pair in _tickValues)
			{
				XmlNode xmlTickSymbol = xmlDocument.CreateElement("Symbol");
				xmlTickData.AppendChild(xmlTickSymbol);
				XmlAttribute attribute = xmlDocument.CreateAttribute("Name");
				attribute.Value = pair.Key;
				xmlTickSymbol.Attributes.Append(attribute);

				sb.Length = 0;
				foreach (TickEntry entry in pair.Value)
				{
					sb.Append(entry._timeStamp.ToBinary()).Append(" ");
					sb.Append(entry._lastPrice).Append(" ");
					sb.Append(entry._lastVolume).Append(" ");
				}
				xmlTickSymbol.InnerText = sb.ToString();
			}

			XmlNode xmlOhlcData = xmlDocument.CreateElement("OhlcData");
			xmlData.AppendChild(xmlOhlcData);
			foreach (SeriesEntry entry in _seriesList)
			{
				if (entry.Series.OHLCType == SeriesTypeOHLC.Unknown) continue;

				XmlNode xmlSeries = xmlDocument.CreateElement("Series");
				xmlOhlcData.AppendChild(xmlSeries);
				XmlAttribute attribute = xmlDocument.CreateAttribute("Name");
				attribute.Value = entry.Name;
				xmlSeries.Attributes.Append(attribute);
				attribute = xmlDocument.CreateAttribute("Index");
				attribute.Value = entry.Series.SeriesIndex.ToString();
				xmlSeries.Attributes.Append(attribute);
				attribute = xmlDocument.CreateAttribute("OhlcType");
				attribute.Value = entry.Series.OHLCType.ToString();
				xmlSeries.Attributes.Append(attribute);

				sb.Length = 0;
				foreach (DataEntry dataEntry in entry.Data)
				{
					sb.Append(dataEntry.Value).Append(" ");
				}
				xmlSeries.InnerText = sb.ToString();
			}
		}

		internal void LoadFromXml(XmlNode xRoot)
		{
			XmlNode xmlData = xRoot.SelectSingleNode("SeriesData");
			if (xmlData == null) return;

			XmlNode xmlTimeStamps = xmlData.SelectSingleNode("TimeStamps");
			if (xmlTimeStamps == null) return;

			ClearData();

			string[] timeStamps = xmlTimeStamps.InnerText.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
			foreach (string timeStamp in timeStamps)
			{
				DateTime time = DateTime.FromBinary(long.Parse(timeStamp));
				_timestamps.Add(time, 1);
				_timestampIndexes[time] = 1;
			}
			_chart._endIndex = _timestamps.Count;

			XmlNode xmlTickData = xmlData.SelectSingleNode("TickData");
			if (xmlTickData != null)
			{
				foreach (XmlNode childNode in xmlTickData.ChildNodes)
				{
					if (childNode.Name != "Symbol") continue;
					List<TickEntry> tickData;
					string symbol = childNode.Attributes["Name"].Value;
					if (!_tickValues.TryGetValue(symbol, out tickData))
						_tickValues[symbol] = (tickData = new List<TickEntry>(1000));
					string[] values = childNode.InnerText.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
					for (int i = 0; i < values.Length; i += 3)
					{
						tickData.Add(new TickEntry
													 {
														 _timeStamp = DateTime.FromBinary(long.Parse(values[i])),
														 _lastPrice = double.Parse(values[i + 1]),
														 _lastVolume = double.Parse(values[i + 2])
													 });
					}
				}  
			}
			foreach (var entry in _seriesList)
			{
				InitSeries(entry);
			}

			if (_chart.ChartType == ChartTypeEnum.Tick)
			{
				//initialize th series
				ReCompressTicks();
				return;
			}

			XmlNode xmlOhlcData = xmlData.SelectSingleNode("OhlcData");
			if (xmlOhlcData == null) return;
			foreach (XmlNode childNode in xmlOhlcData.ChildNodes)
			{
				if (childNode.Name != "Series") return;

				int seriesIndex = int.Parse(childNode.Attributes["Index"].Value);
				string symbol = childNode.Attributes["Name"].Value;
				SeriesTypeOHLC ohlcType = (SeriesTypeOHLC)Enum.Parse(typeof (SeriesTypeOHLC),
																														 childNode.Attributes["OhlcType"].Value);

				int existingSeriesIndex = SeriesIndex(symbol, ohlcType);
				if (seriesIndex != existingSeriesIndex) //in case chart has already some series
				{
					seriesIndex = existingSeriesIndex;
				}

				if (seriesIndex >= _seriesList.Count || seriesIndex == -1)
				{
					Debug.WriteLine("Series index out of range.");
					continue;
				}

				string[] values = childNode.InnerText.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
				int valueIndex = 0;
				foreach (string value in values)
				{
					_seriesList[seriesIndex].Data[valueIndex] =
						new DataEntry(double.Parse(value))
							{
								_collectionOwner = _seriesList[seriesIndex].Data,
								_dataManager = this,
								Index = valueIndex
							};
					valueIndex++;
				}        
			}
		}
#endif

        public void AppendOHLCVValues(string seriesName, DateTime timeStamp, double? open, double? high, double? low, double? close, double? volume, bool isPartial)
        {
            if (!SeriesExists(seriesName, SeriesTypeOHLC.Open) || !SeriesExists(seriesName, SeriesTypeOHLC.Volume))
                return;

            if (!isPartial || GetTimeStampIndex(timeStamp) < 0)
            {
                AppendValue(_seriesToIndex[SeriesTypeOHLC.Open][seriesName], timeStamp, open);
                AppendValue(_seriesToIndex[SeriesTypeOHLC.High][seriesName], timeStamp, high);
                AppendValue(_seriesToIndex[SeriesTypeOHLC.Low][seriesName], timeStamp, low);
                AppendValue(_seriesToIndex[SeriesTypeOHLC.Close][seriesName], timeStamp, close);
                AppendValue(_seriesToIndex[SeriesTypeOHLC.Volume][seriesName], timeStamp, volume);
                return;
            }

            //SeriesEntry seriesOpen = this[_seriesToIndex[SeriesTypeOHLC.Open][seriesName]];
            SeriesEntry seriesHigh = this[_seriesToIndex[SeriesTypeOHLC.High][seriesName]];
            SeriesEntry seriesLow = this[_seriesToIndex[SeriesTypeOHLC.Low][seriesName]];
            SeriesEntry seriesClose = this[_seriesToIndex[SeriesTypeOHLC.Close][seriesName]];
            SeriesEntry seriesVolume = this[_seriesToIndex[SeriesTypeOHLC.Volume][seriesName]];

            // update high value if new high
            if (high > seriesHigh.Data[RecordCount - 1].Value)
            {
                seriesHigh.Data[RecordCount - 1].Value = high;
                seriesHigh.Series.InvokePropertyChanged(Series.PropertyLastTick);
            }

            // update low value if new low
            if (low < seriesLow.Data[RecordCount - 1].Value)
            {
                seriesLow.Data[RecordCount - 1].Value = low;
                seriesLow.Series.InvokePropertyChanged(Series.PropertyLastTick);
            }

            // update close value
            seriesClose.Data[RecordCount - 1].Value = close;
            seriesClose.Series.InvokePropertyChanged(Series.PropertyLastTick);

            // increment volume
            seriesVolume.Data[RecordCount - 1].Value += volume;
            seriesVolume.Series.InvokePropertyChanged(Series.PropertyLastTick);
        }

        /// <summary>
        /// Will offset all the timestamps in chart by a given offset
        /// </summary>
        /// <param name="offset"></param>
        internal void OffsetTimestamps(TimeSpan offset)
        {
            foreach (TickEntry tickEntry in _tickValues.SelectMany(pair => pair.Value))
            {
                tickEntry._timeStamp = tickEntry._timeStamp.Add(offset);
            }

            _timestampIndexes = new Dictionary<DateTime, int>(_timestampIndexes.ToDictionary(pair => pair.Key.Add(offset), pair => pair.Value));
            _timestamps = new SortedList<DateTime, int>(_timestamps.ToDictionary(pair => pair.Key.Add(offset), pair => pair.Value));
        }

        internal int DeleteTimestamps(IEnumerable<DateTime> timestamps)
        {
            var timestampsLocal = timestamps.Where(_ => _timestamps.ContainsKey(_)).ToList();

            foreach (var timestamp in timestampsLocal)
            {
                int valueIndex = _timestamps.IndexOfKey(timestamp);
                foreach (SeriesEntry t in _seriesList)
                {
                    t._dataChanged = true;
                    t.Data.RemoveAt(valueIndex);
                }
            }

            foreach (DateTime timestamp in timestampsLocal)
            {
                _timestamps.Remove(timestamp);
                _timestampIndexes.Remove(timestamp);

                foreach (KeyValuePair<string, List<TickEntry>> pair in _tickValues)
                {
                    DateTime timestamp1 = timestamp;
                    int idx = Algorithms.FirstIndexOf(pair.Value, _ => _._timeStamp == timestamp1);
                    if (idx == -1)
                        continue;
                    pair.Value.RemoveAt(idx);
                }
            }

            return timestampsLocal.Count;
        }
    }
}

