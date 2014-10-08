
namespace ModulusFE.PriceStyles.Models
{
    internal abstract partial class PriceStyleModeBase
    {
        public int StartIndex { get; private set; }
        public int EndIndex { get; private set; }
        public Series[] Series { get; private set; }

        protected PriceStyleModeBase(int startIndex, int endIndex, Series[] series)
        {
            StartIndex = startIndex;
            EndIndex = endIndex;
            Series = series;
        }
    }
}
