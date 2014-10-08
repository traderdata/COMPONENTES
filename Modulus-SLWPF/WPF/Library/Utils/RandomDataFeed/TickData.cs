using System;

namespace ModulusFE.OMS.Interface
{
    ///<summary>
    ///</summary>
    public partial class TickData
    {
        private string _symbol;
        private DateTime _tradeDate;
        private double _price;
        private double _volume;
        private double _bid;
        private long _bidSize;
        private double _ask;
        private long _askSize;

        partial void SymbolChanged();
        partial void TradeDateChanged();
        partial void LastPriceChanged();
        partial void VolumeChanged();
        partial void BidChanged();
        partial void BidSizeChanged();
        partial void AskChanged();
        partial void AskSizeChanged();

        ///<summary>
        /// ctor
        ///</summary>
        public string Symbol
        {
            get { return _symbol; }
            set
            {
                if (_symbol == value) return;

                _symbol = value;
                SymbolChanged();
            }
        }

        ///<summary>
        ///</summary>
        public double Bid
        {
            get { return _bid; }
            set
            {
                if (_bid == value) return;

                _bid = value;
                BidChanged();
            }
        }

        ///<summary>
        ///</summary>
        public long BidSize
        {
            get { return _bidSize; }
            set
            {
                if (_bidSize == value) return;

                _bidSize = value;
                BidSizeChanged();
            }
        }

        ///<summary>
        ///</summary>
        public double Ask
        {
            get { return _ask; }
            set
            {
                if (_ask == value) return;

                _ask = value;
                AskChanged();
            }
        }

        ///<summary>
        ///</summary>
        public long AskSize
        {
            get { return _askSize; }
            set
            {
                if (_askSize == value) return;

                _askSize = value;
                AskSizeChanged();
            }
        }

        ///<summary>
        ///</summary>
        public DateTime TradeDate
        {
            get { return _tradeDate; }
            set
            {
                if (_tradeDate == value) return;
                _tradeDate = value;
                TradeDateChanged();
            }
        }

        ///<summary>
        ///</summary>
        public double Price
        {
            get { return _price; }
            set
            {
                if (_price == value) return;

                _price = value;
                LastPriceChanged();
            }
        }

        ///<summary>
        ///</summary>
        public double Volume
        {
            get { return _volume; }
            set
            {
                if (_volume == value) return;

                _volume = value;
                VolumeChanged();
            }
        }
    }
}
