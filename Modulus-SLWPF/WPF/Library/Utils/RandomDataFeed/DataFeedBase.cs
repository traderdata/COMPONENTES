using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ModulusFE.OMS.Interface
{
    ///<summary>
    ///</summary>
    [CLSCompliant(false)]
    public abstract class DataFeedBase
    {
        /// <summary>
        /// Clients that are subscribed for real-time data for a given symbol
        /// </summary>
        protected volatile Dictionary<string, List<IDataFeedClient>> _rtClients
          = new Dictionary<string, List<IDataFeedClient>>();

        /// <summary>
        /// ctor
        /// </summary>
        protected DataFeedBase()
        {
            DataFeedStatus = DataFeedStatus.Offline;
        }

        /// <summary>
        /// Event fired everytime when status of DataFeed changes
        /// </summary>
        public event Action<object, DataFeedStatus> OnDataFeedStatus;

        #region Public common methods/properties

        /// <summary>
        /// Gets the current status of DataFeed
        /// </summary>
        public DataFeedStatus DataFeedStatus { get; protected set; }

        ///<summary>
        ///</summary>
        ///<param name="client"></param>
        ///<param name="symbol"></param>
        public void Subscribe(IDataFeedClient client, string symbol)
        {
            lock (_rtClients)
            {
                List<IDataFeedClient> symbolClients;
                if (!_rtClients.TryGetValue(symbol, out symbolClients))
                {
                    //symbol wasn't subscribed yet, create an entry
                    _rtClients.Add(symbol, symbolClients = new List<IDataFeedClient>());
                    //make the real subscription
                    SubscribeToSymbol(symbol);
                }

                //add client to the client list
                if (symbolClients.FindIndex(feedClient => feedClient.ClientId == client.ClientId) == -1)
                    symbolClients.Add(client);
            }
        }

        ///<summary>
        ///</summary>
        ///<param name="client"></param>
        ///<param name="symbol"></param>
        ///<exception cref="DataFeedException"></exception>
        public void UnSubscribe(IDataFeedClient client, string symbol)
        {
            lock (_rtClients)
            {
                List<IDataFeedClient> symbolClients;
                if (!_rtClients.TryGetValue(symbol, out symbolClients))
                    throw new DataFeedException("Attempt to unsubscribe from non-existent symbol: " + symbol);

                int clientIndex = symbolClients.FindIndex(feedClient => feedClient.ClientId == client.ClientId);
                if (clientIndex == -1)
                    throw new DataFeedException("Attept to unsubscribe non-existent client: " + client.ClientId);

                symbolClients.RemoveAt(clientIndex);

                Debug.Assert(symbolClients.Count(feedClient => feedClient.ClientId == client.ClientId) == 0);

                //if symbol has no more clients, remove symbol subscription
                if (symbolClients.Count == 0)
                {
                    //make the real un-subscribe at data provider
                    UnSubscribeFromSymbol(symbol);
                    //remove from map
                    _rtClients.Remove(symbol);
                }
            }
        }
        #endregion

        #region Protected common methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newDataFeedStatus"></param>
        protected void InvokeDataFeedStatus(DataFeedStatus newDataFeedStatus)
        {
            DataFeedStatus = newDataFeedStatus;
            if (OnDataFeedStatus != null)
                OnDataFeedStatus(this, DataFeedStatus);
        }

        /// <summary>
        /// methods invoked by specific data provider when a new tick comes
        /// </summary>
        /// <param name="data"></param>
        protected void OnTickData(TickData data)
        {
            lock (_rtClients)
            {
                foreach (var client in _rtClients[data.Symbol])
                {
                    client.OnTickData(data);
                }
            }
        }

        #endregion

        #region Abstract methods to be ovveriden by specific data providers

        ///<summary>
        ///</summary>
        public abstract string Name { get; }

        /// <summary>
        /// Starts RT feed
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="args"></param>
        protected abstract void Start(string username, string password, params object[] args);
        /// <summary>
        /// Stops RT feed
        /// </summary>
        protected abstract void Stop();
        /// <summary>
        /// Subscribe
        /// </summary>
        /// <param name="symbol"></param>
        protected abstract void SubscribeToSymbol(string symbol);
        /// <summary>
        /// Un Subscribe
        /// </summary>
        /// <param name="symbol"></param>
        protected abstract void UnSubscribeFromSymbol(string symbol);
        /// <summary>
        /// Gets historical data
        /// </summary>
        /// <param name="historyRequest">Request parameters</param>
        /// <param name="onHistoryDataReady">Callback function that will be called async when server will
        /// have historical data ready </param>
        public abstract void GetHistory(HistoryRequest historyRequest, Action<List<BarData>> onHistoryDataReady);

        #endregion
    }

    ///<summary>
    ///</summary>
    public interface IDataFeedClient
    {
        ///<summary>
        ///</summary>
        Guid ClientId { get; }

        ///<summary>
        ///</summary>
        ///<param name="tickData"></param>
        void OnTickData(TickData tickData);
    }
}
