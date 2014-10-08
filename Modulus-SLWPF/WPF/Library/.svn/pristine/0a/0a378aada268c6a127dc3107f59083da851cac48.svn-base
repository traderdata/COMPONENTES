using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ModulusFE.OMS.Interface;

namespace ModulusFE.DS.Random
{
    ///<summary>
    /// Datasource of random logical data
    ///</summary>
    [CLSCompliant(false)]
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


        #region Overrides of DataFeedBase

        /// <summary>
        /// Name
        /// </summary>
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
                _lastPrices[i].LastPrice = lastPrice = Random(lastPrice * 0.99, lastPrice * 1.01);

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

        /// <summary>
        /// Stop RT feed
        /// </summary>
        protected override void Stop()
        {
            InvokeDataFeedStatus(DataFeedStatus.Offline);
        }

        /// <summary>
        /// Subscribe to symbol
        /// </summary>
        /// <param name="symbol"></param>
        protected override void SubscribeToSymbol(string symbol)
        {
            if (SymbolExists(symbol)) return;

            InitSymbolLastValues(symbol);
        }

        /// <summary>
        /// Unscuscribe
        /// </summary>
        /// <param name="symbol"></param>
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
                      case Periodicity.Secondly:
                          datetimeStep = 1;
                          break;
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
                          double maxHigh = symbolValue.LastPrice * 1.1;
                          double minLow = symbolValue.LastPrice;
                          barData.HighPrice = Random(minLow, maxHigh);
                          minLow = symbolValue.LastPrice * 0.9;
                          maxHigh = symbolValue.LastPrice;
                          barData.LowPrice = Random(minLow, maxHigh);
                          barData.ClosePrice = Random(barData.LowPrice.Value, barData.HighPrice.Value);
                          symbolValue.LastPrice = barData.ClosePrice.Value;

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

        ///<summary>
        ///</summary>
        ///<param name="symbol"></param>
        ///<param name="nextPrice"></param>
        ///<param name="nextVolume"></param>
        ///<returns></returns>
        public bool GetNextTick(string symbol, out double nextPrice, out double nextVolume)
        {
            nextPrice = nextVolume = double.NaN;

            SymbolValue symbolValue = ValueBySymbol(symbol);
            if (symbolValue == null)
                return false;

            nextVolume = symbolValue.LastVolume = NextVolume(symbolValue.LastVolume);
            nextPrice = symbolValue.LastPrice = Random(symbolValue.LastPrice * 0.99, symbolValue.LastPrice * 1.01);

            return true;
        }

        ///<summary>
        ///</summary>
        ///<param name="symbol"></param>
        ///<returns></returns>
        public BarData GetNextBar(string symbol)
        {
            SymbolValue symbolValue = ValueBySymbol(symbol);
            if (symbolValue == null)
                return null;

            BarData barData = new BarData { TradeDate = GetNextTradeTime(symbolValue.LastDate), OpenPrice = symbolValue.LastPrice };
            double maxHigh = symbolValue.LastPrice * 1.1;
            double minLow = symbolValue.LastPrice;
            barData.HighPrice = Random(minLow, maxHigh);
            minLow = symbolValue.LastPrice * 0.9;
            maxHigh = symbolValue.LastPrice;
            barData.LowPrice = Random(minLow, maxHigh);
            barData.ClosePrice = Random(barData.LowPrice.Value, barData.HighPrice.Value);
            symbolValue.LastPrice = barData.ClosePrice.Value;

            barData.Volume = symbolValue.LastVolume = NextVolume(symbolValue.LastVolume);

            symbolValue.LastDate = symbolValue.LastDate.AddSeconds(symbolValue.TimeStep);

            return barData;
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

            return lastVolume + sign * Random(0, lastVolume * 0.03); //1%
        }


        private bool SymbolExists(string symbol)
        {
            return _lastPrices.FindIndex(value => value.Symbol.Equals(symbol, StringComparison.InvariantCultureIgnoreCase)) != -1;
        }

        private SymbolValue ValueBySymbol(string symbol)
        {
            return _lastPrices.FirstOrDefault(value => value.Symbol == symbol);
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
