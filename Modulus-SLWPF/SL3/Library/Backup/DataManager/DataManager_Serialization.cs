using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using ModulusFE.Data;

namespace ModulusFE.DataManager
{
    internal partial class DataManager
    {
        private readonly CultureInfo _usCI = new CultureInfo("en-us");

        internal void SaveToXml(XElement xRoot)
        {
            XElement xmlData = new XElement("SeriesData");
            xRoot.Add(xmlData);

            XElement xmlTimeStamps = new XElement("TimeStamps");
            xmlData.Add(xmlTimeStamps);
            StringBuilder sb = new StringBuilder(1000);
            foreach (KeyValuePair<DateTime, int> pair in _timestamps)
            {
                sb.Append(pair.Key.ToJDate().ToString(_usCI)).Append(" ");
            }
            xmlTimeStamps.Value = sb.ToString();

            XElement xmlTickData = new XElement("TickData");
            xmlData.Add(xmlTickData);
            sb.Length = 0;
            foreach (KeyValuePair<string, List<TickEntry>> pair in _tickValues)
            {
                XElement xmlTickSymbol = new XElement("Symbol");
                xmlTickData.Add(xmlTickSymbol);
                XAttribute attribute = new XAttribute("Name", pair.Key);
                xmlTickSymbol.Add(attribute);

                sb.Length = 0;
                foreach (TickEntry entry in pair.Value)
                {
                    sb.Append(entry._timeStamp.ToJDate().ToString(_usCI)).Append(" ");
                    sb.Append(entry._lastPrice.ToString(_usCI)).Append(" ");
                    sb.Append(entry._lastVolume.ToString(_usCI)).Append(" ");
                }
                xmlTickSymbol.Value = sb.ToString();
            }

            XElement xmlOhlcData = new XElement("OhlcData");
            xmlData.Add(xmlOhlcData);
            foreach (SeriesEntry entry in _seriesList)
            {
                if (entry.Series.OHLCType == SeriesTypeOHLC.Unknown) continue;

                XElement xmlSeries = new XElement("Series");
                xmlOhlcData.Add(xmlSeries);
                XAttribute attribute = new XAttribute("Name", entry.Name);
                xmlSeries.Add(attribute);
                attribute = new XAttribute("Index", entry.Series.SeriesIndex.ToString());
                xmlSeries.Add(attribute);
                attribute = new XAttribute("OhlcType", entry.Series.OHLCType.ToString());
                xmlSeries.Add(attribute);

                sb.Length = 0;
                foreach (DataEntry dataEntry in entry.Data)
                {
                    sb.Append(dataEntry.Value.HasValue ? dataEntry.Value.Value.ToString(_usCI) : null).Append(" ");
                }
                xmlSeries.Value = sb.ToString();
            }
        }

        internal void LoadFromXml(XElement xRoot)
        {
            XElement xmlData = xRoot.Element("SeriesData");
            if (xmlData == null) return;

            XElement xmlTimeStamps = xmlData.Element("TimeStamps");
            if (xmlTimeStamps == null) return;

            ClearData();

            string[] timeStamps = xmlTimeStamps.Value.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string timeStamp in timeStamps)
            {
                DateTime time = DateTimeEx.FromJDate(double.Parse(timeStamp, _usCI));
                _timestamps.Add(time, 1);
                _timestampIndexes[time] = 1;
            }
            _chart._endIndex = _timestamps.Count;

            XElement xmlTickData = xmlData.Element("TickData");
            if (xmlTickData != null)
            {
                foreach (XElement childNode in xmlTickData.Elements())
                {
                    if (childNode.Name != "Symbol") continue;
                    List<TickEntry> tickData;
                    string symbol = childNode.Attributes("Name").First().Value;
                    if (!_tickValues.TryGetValue(symbol, out tickData))
                        _tickValues[symbol] = (tickData = new List<TickEntry>(1000));
                    string[] values = childNode.Value.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < values.Length; i += 3)
                    {
                        tickData.Add(new TickEntry
                                       {
                                           _timeStamp = DateTimeEx.FromJDate(long.Parse(values[i], _usCI)),
                                           _lastPrice = double.Parse(values[i + 1], _usCI),
                                           _lastVolume = double.Parse(values[i + 2], _usCI)
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

            XElement xmlOhlcData = xmlData.Element("OhlcData");
            if (xmlOhlcData == null) return;
            foreach (XElement childNode in xmlOhlcData.Elements())
            {
                if (childNode.Name != "Series") return;

                int seriesIndex = int.Parse(childNode.Attributes("Index").First().Value);
                string symbol = childNode.Attributes("Name").First().Value;
                SeriesTypeOHLC ohlcType = (SeriesTypeOHLC)System.Enum.Parse(typeof(SeriesTypeOHLC),
                                                                            childNode.Attributes("OhlcType").First().Value, true);
                int existingSeriesIndex = SeriesIndex(symbol, ohlcType);
                if (seriesIndex != existingSeriesIndex) //in case chart has already some series
                    seriesIndex = existingSeriesIndex;
                if (seriesIndex >= _seriesList.Count || seriesIndex == -1)
                {
                    Debug.WriteLine("Series index out of range.");
                    continue;
                }

                string[] values = childNode.Value.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                int valueIndex = 0;
                foreach (string value in values)
                {
                    _seriesList[seriesIndex].Data[valueIndex] =
                      new DataEntry(double.Parse(value, _usCI))
                        {
                            _collectionOwner = _seriesList[seriesIndex].Data,
                            _dataManager = this,
                            Index = valueIndex
                        };
                    valueIndex++;
                }
            }
        }
    }
}

