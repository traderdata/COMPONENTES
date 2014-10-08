using System.Windows;

namespace ModulusFE
{
    internal class PointEx
    {
        public static Point Parse(string p)
        {
            string[] vals = p.Split(',');
            if (vals.Length != 2)
                return new Point(0, 0);

            return new Point(double.Parse(vals[0]), double.Parse(vals[1]));
        }
    }
}
