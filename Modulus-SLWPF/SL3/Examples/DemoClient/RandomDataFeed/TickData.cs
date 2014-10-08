using System;

namespace ModulusFE.OMS.Interface
{
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
