using System.Linq;
using ModulusFE.Data;

namespace ModulusFE.DataManager
{
    internal class SeriesEntry
    {
        internal double _min, _max; //linear
        internal double _visibleMin, _visibleMax;
        internal bool _visibleDataChanged;
        internal bool _dataChanged;

        public string Name;
        public Series Series;
        public readonly DataEntryCollection Data = new DataEntryCollection();

        public DataEntry this[int index]
        {
            get { return Data[index]; }
        }

        public double? Max(int startIndex, int count)
        {
            return Data.Skip(startIndex).Take(count).Select(_ => _.Value).Max();
        }

        public double? Min(int startIndex, int count)
        {
            return Data.Skip(startIndex).Take(count).Select(_ => _.Value).Min();
        }

        public double? Last(int startIndx, int count)
        {
            return Data.Skip(startIndx).Take(count).Select(_ => _.Value).Last();
        }

        public double? Sum(int startIndex, int count)
        {
            return Data.Skip(startIndex).Take(count).Select(_ => _.Value).Sum();
        }
    }
}
