using System;
using System.Collections.Generic;
using System.Threading;
using ModulusFE.OMS.Interface;
using ModulusFE.SL;

namespace ModulusFE.DS.Random
{
    public class DsRandom : DataFeedBase
    {
        private class SymbolValue
        {
            public string Symbol;
            public double LastPrice;
            public double LastVolume;
            public DateTime LastDate;
            public long TimeStep;
        }

        private readonly List<SymbolValue> _lastPrices = new List<SymbolValue>();
        private readonly System.Random _rnd = new System.Random();
        private Timer _timerRT;
        private double _mess = 0.01; // 0.01 low (and default) the higher the number the messier


        #region Overrides of DataFeedBase

        public override string Name
        {
            get { return "Random Data Generator"; }
        }

        /// <summary>
        /// Starts the data generation timer
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="args">
        /// args[0] - (if given) Timer period (ms). Valid values 50...5000
        /// </param>
        protected override void Start(string username, string password, params object[] args)
        {
            short interval = 1000;
            if (args != null)
            {
                if (args.Length > 0)
                    interval = Convert.ToInt16(args[0]);
                if (interval < 50 || interval > 5000)
                    throw new ArgumentException("Invalid timer period value: " + interval, "args");
            }

            _timerRT = new Timer(TimerCallback, null, 0, interval);

            InvokeDataFeedStatus(DataFeedStatus.Online);

            _lastPrices.Clear();
        }

        private void TimerCallback(object state)
        {
            for (int i = 0; i < _lastPrices.Count; i++)
            {
                double lastPrice = _lastPrices[i].LastPrice;
                _lastPrices[i].LastPrice = lastPrice = Random(lastPrice * (1.0 - _mess), lastPrice * (1.0 + _mess));

                OnTickData(new TickData
                             {
                                 Symbol = _lastPrices[i].Symbol,
                                 TradeDate = DateTime.Now,
                                 Price = lastPrice,
                                 Volume = _lastPrices[i].LastVolume = NextVolume(_lastPrices[i].LastVolume),
                                 Bid = _rnd.NextDouble() * lastPrice,
                                 BidSize = (long)(_rnd.NextDouble() * _lastPrices[i].LastVolume),
                                 Ask = _rnd.NextDouble() * lastPrice,
                                 AskSize = (long)(_rnd.NextDouble() * _lastPrices[i].LastVolume)
                             });
            }
        }

        protected override void Stop()
        {
            InvokeDataFeedStatus(DataFeedStatus.Offline);
        }

        protected override void SubscribeToSymbol(string symbol)
        {
            if (SymbolExists(symbol)) return;

            InitSymbolLastValues(symbol);
        }

        protected override void UnSubscribeFromSymbol(string symbol)
        {

        }

        /// <summary>
        /// Gets historical data
        /// </summary>
        /// <param name="historyRequest">Request parameters</param>
        /// <param name="onHistoryDataReady">Callback function that will be called async when server will
        /// have historical data ready </param>
        public override void GetHistory(HistoryRequest historyRequest, Action<List<BarData>> onHistoryDataReady)
        {
            if (!SymbolExists(historyRequest.Symbol))
            {
                InitSymbolLastValues(historyRequest.Symbol);
            }

            ThreadPool.QueueUserWorkItem(
              state =>
              {

                  long datetimeStep = 0; //seconds
                  List<BarData> res = new List<BarData>();

                  if (historyRequest.BarSize > 500 || historyRequest.BarSize < 1)
                  {
                      onHistoryDataReady(res);
                      return;
                  }

                  switch (historyRequest.Periodicity)
                  {
                      case Periodicity.Minutely:
                          datetimeStep = 60;
                          break;
                      case Periodicity.Hourly:
                          datetimeStep = 60 * 60;
                          break;
                      case Periodicity.Daily:
                          datetimeStep = 60 * 60 * 24;
                          break;
                      case Periodicity.Weekly:
                          datetimeStep = 60 * 60 * 24 * 7;
                          break;
                  }

                  SymbolValue symbolValue = ValueBySymbol(historyRequest.Symbol) ??
                                            InitSymbolLastValues(historyRequest.Symbol);
                  lock (symbolValue)
                  {
                      DateTime startDate = DateTime.Now;
                      for (int i = 0; i < historyRequest.BarCount; i++)
                      {
                          startDate = GetNextTradeTime(startDate.AddSeconds(datetimeStep));

                          BarData barData = new BarData { TradeDate = startDate, OpenPrice = symbolValue.LastPrice };

                          double maxHigh = symbolValue.LastPrice * (1.0 + _mess);
                          double minLow = symbolValue.LastPrice;
                          barData.HighPrice = Random(minLow, maxHigh);

                          minLow = symbolValue.LastPrice * (1.0 - _mess);
                          maxHigh = symbolValue.LastPrice;
                          barData.LowPrice = Random(minLow, maxHigh);

                          barData.ClosePrice = Random(barData.LowPrice, barData.HighPrice);
                          symbolValue.LastPrice = barData.ClosePrice;

                          barData.Volume = symbolValue.LastVolume = NextVolume(symbolValue.LastVolume);

                          res.Add(barData);
                      }
                      symbolValue.LastDate = startDate;
                      symbolValue.TimeStep = datetimeStep;
                  }

                  onHistoryDataReady(res);
              });
        }

        #endregion

        public bool GetNextTick(string symbol, out double nextPrice, out double nextVolume, out DateTime timeStamp)
        {
            nextPrice = nextVolume = double.NaN;
            timeStamp = DateTime.MinValue;

            SymbolValue symbolValue = ValueBySymbol(symbol);
            if (symbolValue == null)
                return false;

            nextVolume = symbolValue.LastVolume = NextVolume(symbolValue.LastVolume);
            nextPrice = symbolValue.LastPrice = Random(symbolValue.LastPrice * (1.0 - _mess), symbolValue.LastPrice * (1.0 + _mess));
            timeStamp = symbolValue.LastDate = symbolValue.LastDate.AddSeconds(symbolValue.TimeStep);

            return true;
        }

        public BarData GetNextBar(string symbol)
        {
            SymbolValue symbolValue = ValueBySymbol(symbol);
            if (symbolValue == null)
                return null;

            BarData barData = new BarData
            {
                TradeDate = GetNextTradeTime(symbolValue.LastDate.AddSeconds(symbolValue.TimeStep)),
                OpenPrice = symbolValue.LastPrice
            };

            double maxHigh = symbolValue.LastPrice * (1.0 + _mess);
            double minLow = symbolValue.LastPrice;
            barData.HighPrice = Random(minLow, maxHigh);

            minLow = symbolValue.LastPrice * (1.0 - _mess);
            maxHigh = symbolValue.LastPrice;

            barData.LowPrice = Random(minLow, maxHigh);
            barData.ClosePrice = Random(barData.LowPrice, barData.HighPrice);
            barData.Volume = NextVolume(symbolValue.LastVolume);

            symbolValue.LastDate = barData.TradeDate;
            symbolValue.LastPrice = barData.ClosePrice;
            symbolValue.LastVolume = barData.Volume;

            return barData;
        }

        public double Mess
        {
            get
            {
                return _mess;
            }
            set
            {
                if (_mess == value) return;
                if (value > 0.2) return;
                if (value < 0.001) return;
                _mess = value;
            }
        }

        private double Random(double min, double max)
        {
            return min + (_rnd.NextDouble() * (max - min));
        }

        private double NextVolume(double lastVolume)
        {
            int sign = 1;
            if (_rnd.NextDouble() > 0.5)
                sign = -1;

            return lastVolume + sign * Random(0, lastVolume * _mess);
        }


        private bool SymbolExists(string symbol)
        {
            return _lastPrices.FindIndex(value => value.Symbol.Equals(symbol, StringComparison.InvariantCultureIgnoreCase)) != -1;
        }

        private SymbolValue ValueBySymbol(string symbol)
        {
            foreach (var value in _lastPrices)
            {
                if (value.Symbol == symbol)
                    return value;
            }
            return null;
        }

        private SymbolValue InitSymbolLastValues(string symbol)
        {
            SymbolValue res;
            _lastPrices.Add(res = new SymbolValue
                              {
                                  Symbol = symbol,
                                  LastPrice = _rnd.Next(10, 500),
                                  LastVolume = Random(1E6, 1E9)
                              });
            return res;
        }

        private readonly DateTime MarketStart = new DateTime(2000, 1, 1, 8, 0, 0);
        private readonly DateTime MarketEnd = new DateTime(2000, 1, 1, 16, 0, 0);

        private static int CompareTime(DateTime dt1, DateTime dt2)
        {
            if (dt1.Hour != dt2.Hour)
                return dt1.Hour - dt2.Hour;
            if (dt1.Minute != dt2.Minute)
                return dt1.Minute - dt2.Minute;
            return dt1.Second - dt2.Second;
        }

        private DateTime GetNextTradeTime(DateTime current)
        {
            if (CompareTime(current, MarketStart) >= 0 && CompareTime(current, MarketEnd) <= 0)
                return current;

            if (CompareTime(current, MarketEnd) > 0) //make it next day 8AM
            {
                current = current.AddDays(1);
                return new DateTime(current.Year, current.Month, current.Day, 8, 0, 0);
            }

            //is before market start, just make it 8AM
            return new DateTime(current.Year, current.Month, current.Day, 8, 0, 0);
        }
    }
}
