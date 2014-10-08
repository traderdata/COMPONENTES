
namespace ModulusFE.OMS.Interface
{
    public partial class HistoryRequest
    {
        public string Symbol { get; set; }

        public Periodicity Periodicity { get; set; }

        public int BarSize { get; set; }

        public int BarCount { get; set; }
    }
}