using System.Windows.Media;

namespace ModulusFE.SL
{
    ///<summary>
    /// Set of Brushes that are missing from standard .NET runtime
    ///</summary>
    public static class Brushes
    {
        ///<summary>
        ///</summary>
        public static Brush Transparent = new SolidColorBrush(Colors.Transparent);
        ///<summary>
        ///</summary>
        public static Brush Navy = new SolidColorBrush(ColorsEx.Navy);
        ///<summary>
        ///</summary>
        public static Brush SkyBlue = new SolidColorBrush(ColorsEx.SkyBlue);
        ///<summary>
        ///</summary>
        public static Brush Silver = new SolidColorBrush(ColorsEx.Silver);
        ///<summary>
        ///</summary>
        public static Brush White = new SolidColorBrush(Colors.White);
        ///<summary>
        ///</summary>
        public static Brush Black = new SolidColorBrush(Colors.Black);
        ///<summary>
        ///</summary>
        public static Brush Yellow = new SolidColorBrush(Colors.Yellow);
        ///<summary>
        ///</summary>
        public static Brush Red = new SolidColorBrush(Colors.Red);
        ///<summary>
        ///</summary>
        public static Brush Blue = new SolidColorBrush(Colors.Blue);
        ///<summary>
        ///</summary>
        public static Brush DarkRed = new SolidColorBrush(ColorsEx.DarkRed);
        ///<summary>
        ///</summary>
        public static Brush LightBlue = new SolidColorBrush(ColorsEx.LightBlue);
        ///<summary>
        ///</summary>
        public static Brush Green = new SolidColorBrush(Colors.Green);
    }

    ///<summary>
    /// Set or Colors that are missing from .NET standard library
    ///</summary>
    public static class ColorsEx
    {
        ///<summary>
        ///</summary>
        public static Color Gray = Color.FromArgb(0xFF, 0x2F, 0x4F, 0x4F);
        ///<summary>
        ///</summary>
        public static Color Navy = Color.FromArgb(0xFF, 0x00, 0x00, 0x80);
        ///<summary>
        ///</summary>
        public static Color SkyBlue = Color.FromArgb(0xFF, 0x87, 0xCE, 0xEB);
        ///<summary>
        ///</summary>
        public static Color Silver = Color.FromArgb(0xFF, 0xC0, 0xC0, 0xC0);
        ///<summary>
        ///</summary>
        public static Color Lime = Color.FromArgb(0xFF, 0x00, 0xFF, 0x00);
        ///<summary>
        ///</summary>
        public static Color LightBlue = Color.FromArgb(0xFF, 0xAD, 0xD8, 0xE6);
        ///<summary>
        ///</summary>
        public static Color LightSteelBlue = Color.FromArgb(0xFF, 0xB0, 0xC4, 0xDE);
        ///<summary>
        ///</summary>
        public static Color DarkRed = Color.FromArgb(0xFF, 0x8B, 0x00, 0x00);
        ///<summary>
        ///</summary>
        public static Color MidnightBlue = Color.FromArgb(0xFF, 0x19, 0x19, 0x70);
    }
}
