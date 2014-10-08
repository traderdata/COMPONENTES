using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ModulusFE.PaintObjects;

#if SILVERLIGHT
using ModulusFE.SL;
#endif
#if WPF
using System.ComponentModel;
using Label = ModulusFE.PaintObjects.Label;
#endif

namespace ModulusFE
{
    internal static class Utils
    {
        /// <summary>
        /// Returns whether the control is in design mode (running under Blend or Visual Studio).
        /// </summary>
        /// <returns>True if in design mode.</returns>
        public static bool GetIsInDesignMode(DependencyObject element)
        {
#if SILVERLIGHT
            if (!IsInDesignMode.HasValue)
            {
                IsInDesignMode = (null == Application.Current) || Application.Current.GetType() == typeof(Application);
            }
#endif
#if WPF
            IsInDesignMode = DesignerProperties.GetIsInDesignMode(element);
#endif
            return IsInDesignMode.Value;
        }

        /// <summary>
        /// Stores the computed InDesignMode value.
        /// </summary>
        private static bool? IsInDesignMode;



        public static bool Between<T>(T value, T min, T max) where T : IComparable
        {
            return (value.CompareTo(min) >= 0) && (value.CompareTo(max) <= 0);
        }

        public static bool StrICmp(string s1, string s2)
        {
            return s1.Equals(s2, StringComparison.CurrentCultureIgnoreCase);
        }

        public static Color StringToColor(string sColor)
        {
            if (sColor.Length == 9 && sColor.StartsWith("#")) //#FFFF0000 - format
            {
                string a = sColor.Substring(1, 2);
                string r = sColor.Substring(3, 2);
                string g = sColor.Substring(5, 2);
                string b = sColor.Substring(7, 2);

                return Color.FromArgb(byte.Parse(a, NumberStyles.AllowHexSpecifier),
                                      byte.Parse(r, NumberStyles.AllowHexSpecifier),
                                      byte.Parse(g, NumberStyles.AllowHexSpecifier),
                                      byte.Parse(b, NumberStyles.AllowHexSpecifier));
            }
            if (sColor.StartsWith("sc#")) //sc#0.7284751, 0.08637705, 0, 0.518262267 - format
            {
                string[] values = sColor.Substring(3).Split(',');
                if (values.Length != 4) throw new NotSupportedException("Color format not supported. Value " + sColor);

                return Color.FromArgb((byte)(double.Parse(values[0]) * 255),
                                      (byte)(double.Parse(values[1]) * 255),
                                      (byte)(double.Parse(values[2]) * 255),
                                      (byte)(double.Parse(values[3]) * 255));
            }

            throw new NotSupportedException("Color format not supported. Value " + sColor);
        }

        public static void EnsureValues(params double?[] values)
        {
            for (int i = 0; i < values.Length; i++)
            {
                values[i] = values[i].HasValue ? values[i] : 0.0;
            }
        }

        public static void Swap<T>(ref T v1, ref T v2)
        {
            T t = v1;
            v1 = v2;
            v2 = t;
        }

        public static double SafeMod(double d1, double d2)
        {
            if (d2 == 0)
            {
                return 0;
            }

            double d3 = d1 / d2;
            return ((d2 * (d3 - Math.Floor(d3))));
        }

        private const double MathLN10 = 2.302585092994046;
        public static void GridScale1(double min, double max, int stepCount, bool ceil, out double realMin, out double realMax, out double step)
        {
            if (max - min < double.MinValue)
            {
                max = max + (0.2 * (max == 0 ? 1 : Math.Abs(max)));
                min = min - (0.2 * (min == 0 ? 1 : Math.Abs(min)));
            }

            //      if (min > 0 && (min / (max - min)) < 0.25)
            //      {
            //        min = 0;
            //      }
            //
            //      if (max < 0 && (max / (max - min)) < 0.25)
            //      {
            //        max = 0;
            //      }
            //
            double range = max - min;
            double stepValue = range / stepCount;
            double v5 = Math.Floor(Math.Log10(stepValue) / MathLN10);
            double v6 = Math.Pow(10, v5);
            int v7 = (int)(ceil ? Math.Ceiling(stepValue / v6) : ((int)(stepValue / v6) + 0.5));
            if (v7 > 2)
            {
                v7 = 4;
            }
            else
            {
                if (v7 > 1)
                {
                    v7 = 2;
                }
            }

            step = v7 * v6;
            realMin = min - SafeMod(min, step);
            double d2 = SafeMod(max, step);
            realMax = max;
            if (d2 != 0)
            {
                realMax = max + (step - d2);
            }

            step = (realMax - realMin) / stepCount;
        }


        public static void GridScaleReal(double min, double max, int stepCount, out double realMin, out double realMax, out int realStepCount, out double step)
        {
            realStepCount = stepCount;
            GridScale(min, max, stepCount, out realMin, out step);
            realMax = realMin + (step * stepCount);

            if (realMin == min)
            {
                return;
            }

            double v;
            //low margin
            while ((v = (realMin + step)) <= min)
            {
                realStepCount--;
                realMin = v;
            }

            while ((v = (realMax - step)) >= max)
            {
                realStepCount--;
                realMax = v;
            }
        }

        public static int GridScale(double xMin, double xMax, int n, out double sMin, out double step)
        {
            int iNegScl;       //  Negative scale flag
            double lfTmp;         //  Temporary value
            int it;            //  Iteration counter 


            //  Neat steps
            int[] steps = { 1, 2, 5, 10, 12, 15, 16, 20, 25, 30, 40, 50, 60, 75, 80, 100, 120, 150 };
            //      int[] steps = { 10, 15, 20, 25, 50, 75, 100 };
            int iNs = steps.Length;

            sMin = xMin;
            step = 0.0;
            //  Some checks
            if (xMin > xMax)
            {
                lfTmp = xMin;
                xMin = xMax;
                xMax = lfTmp;
            }
            if (xMin == xMax)
                xMax = xMin == 0.0 ? 1.0 : xMin + Math.Abs(xMin) / 10.0;

            //  Reduce to positive scale case if possible 
            if (xMax <= 0)
            {
                iNegScl = 1;
                lfTmp = xMin;
                xMin = -xMax;
                xMax = -lfTmp;
            }
            else
                iNegScl = 0;

            if (n < 2)
                n = 2;
            int iNm1 = n - 1;

            for (it = 0; it < 3; it++)
            {
                //  Compute initial and scaled steps 
                double lfIniStep = (xMax - xMin) / iNm1;     //  Initial step
                double lfSclStep = lfIniStep;     //  Scaled step 

                for (; lfSclStep < 10.0; lfSclStep *= 10.0) { }
                for (; lfSclStep > 100.0; lfSclStep /= 10.0) { }

                //  Find a suitable neat step
                int i;             //  Neat step counter
                for (i = 0; i < iNs && lfSclStep > steps[i]; i++) { }
                double lfSclFct = lfIniStep / lfSclStep;      //  Scaling back factor 

                //  Compute step and scale minimum value 
                do
                {
                    step = lfSclFct * steps[i];
                    sMin = Math.Floor(xMin / step) * step;
                    double lfSMax = sMin + iNm1 * step; //  Scale maximum value 
                    if (xMax <= lfSMax) //  Function maximum is in the range: the
                    {
                        // work is done.
                        if (iNegScl != 0)
                            sMin = -lfSMax;
                        step *= (double)iNm1 / (n - 1);
                        return 1;
                    }
                    i++;
                } while (i < iNs);

                //  Double number of intervals
                iNm1 *= 2;
            }

            //  Could not solve the problem
            return 0;
        }

        internal static void DrawLine(double x1, double y1, double x2, double y2, Brush strokeBrush,
          LinePattern strokePattern, double strokeThickness, double opacity,
          PaintObjectsManager<Line> lines)
        {
            Line linePo = lines.GetPaintObject();
            System.Windows.Shapes.Line line = linePo._line;

            line.X1 = x1;
            line.X2 = x2;
            line.Y1 = y1;
            line.Y2 = y2;
            if (!BrushesEqual(line.Stroke, strokeBrush))
                line.Stroke = strokeBrush;

            line.Opacity = opacity;
            line.StrokeThickness = strokeThickness;
            Types.SetShapePattern(line, strokePattern);
        }

        internal static void DrawLine(double x1, double y1, double x2, double y2, Brush strokeBrush,
          LinePattern strokePattern, double strokeThickness, double opacity,
          System.Windows.Shapes.Line line)
        {
            line.X1 = x1;
            line.X2 = x2;
            line.Y1 = y1;
            line.Y2 = y2;
            if (!BrushesEqual(line.Stroke, strokeBrush))
                line.Stroke = strokeBrush;

            line.Opacity = opacity;
            line.StrokeThickness = strokeThickness;
            Types.SetShapePattern(line, strokePattern);
        }

        public static bool BrushesEqual(Brush a, Brush b)
        {
            if (a == null && b == null) return true;
            if (a == null || b == null) return false;
            if ((a is SolidColorBrush) && (b is SolidColorBrush))
            {
                SolidColorBrush sa = (SolidColorBrush)a;
                SolidColorBrush sb = (SolidColorBrush)b;
                if (sa.Color != sb.Color) return false;
                return sa.Opacity == sb.Opacity;
            }
            if ((a is LinearGradientBrush) && (b is LinearGradientBrush))
            {
                LinearGradientBrush la = (LinearGradientBrush)a;
                LinearGradientBrush lb = (LinearGradientBrush)b;
                if (la.Opacity != lb.Opacity) return false;
                if (la.StartPoint != lb.StartPoint) return false;
                if (la.EndPoint != lb.EndPoint) return false;
                if (la.GradientStops.Count != lb.GradientStops.Count) return false;
                for (int i = 0; i < la.GradientStops.Count; i++)
                {
                    GradientStop ga = la.GradientStops[i];
                    GradientStop gb = lb.GradientStops[i];
                    if (ga.Color != gb.Color) return false;
                    if (ga.Offset != gb.Offset) return false;
                }
                return true;
            }
            return a.Equals(b);
        }

        public static Rectangle DrawRectangle(double x1, double y1, double x2, double y2, Brush fillBrush,
          PaintObjectsManager<Rectangle> rects)
        {
            Rectangle rectangle = rects.GetPaintObject();
            System.Windows.Shapes.Rectangle r = rectangle._rectangle;

            Canvas.SetLeft(r, x1);
            Canvas.SetTop(r, y1);
            r.Width = Math.Abs(x2 - x1);
            r.Height = Math.Abs(y2 - y1);
            if (r.Fill == null || !r.Fill.Equals(fillBrush))
                r.Fill = fillBrush;
            if (r.Stroke == null || !r.Stroke.Equals(Brushes.Transparent))
                r.Stroke = Brushes.Transparent;
            return rectangle;
        }

        public static Rectangle DrawRectangle(Types.RectEx rectEx, Brush fillBrush,
          PaintObjectsManager<Rectangle> rects)
        {
            return DrawRectangle(rectEx.Left, rectEx.Top, rectEx.Right, rectEx.Bottom, fillBrush, rects);
        }

        public static Rectangle3D Draw3DRectangle(Types.RectEx bounds, Brush topLeft, Brush bottomRight,
          PaintObjectsManager<Rectangle3D> rects)
        {
            Rectangle3D rectangle3D = rects.GetPaintObject();
            rectangle3D.SetPos(bounds, topLeft, bottomRight);

            return rectangle3D;
        }

        public static Rectangle3D Draw3DRectangle(double X1, double Y1, double X2, double Y2, Brush topLeft, Brush bottomRight,
          PaintObjectsManager<Rectangle3D> rects)
        {
            return Draw3DRectangle(new Types.RectEx(X1, Y1, X2, Y2), topLeft, bottomRight, rects);
        }

        public static Label DrawText(double x, double y, string text, Brush foreground, double fontSize, FontFamily fontFamily,
          PaintObjectsManager<Label> labels)
        {
            Label label = labels.GetPaintObject();
            label._textBlock.Text = text;
            label._textBlock.Foreground = foreground;
            label._textBlock.FontSize = fontSize;
            label._textBlock.FontFamily = fontFamily;
            label.Left = x;
            label.Top = y;

            return label;
        }

        public static LinearGradientBrush CreateFadeVertBrush(Color color1, Color color2)
        {
            LinearGradientBrush gr =
              new LinearGradientBrush { EndPoint = new Point(0.5, 1), StartPoint = new Point(0.5, 0) };
            GradientStop grStop1 = new GradientStop { Color = color1, Offset = 0 };
            gr.GradientStops.Add(grStop1);
            GradientStop grStop2 = new GradientStop { Color = color2, Offset = 1 };
            gr.GradientStops.Add(grStop2);
            return gr;
        }

        public static LinearGradientBrush CreateFadeHorzBrush(Color color1, Color color2)
        {
            LinearGradientBrush gr = new LinearGradientBrush { EndPoint = new Point(1, 0.5), StartPoint = new Point(0, 0.5) };
            GradientStop grStop1 = new GradientStop { Color = color1, Offset = 0 };
            gr.GradientStops.Add(grStop1);
            GradientStop grStop2 = new GradientStop { Color = color2, Offset = 1 };
            gr.GradientStops.Add(grStop2);
            return gr;
        }

        public static string GetCurrentAssemblyName()
        {
            Assembly a = Assembly.GetExecutingAssembly();

            return a.FullName.Split(',')[0];
        }

        public static void TryFreeze(this Brush self)
        {
#if !SILVERLIGHT
            if (self.CanFreeze)
            {
                self.Freeze();
            }
#endif
        }


        /// <summary> 
        /// Encrypt the data 
        /// </summary> 
        /// <param name="input">String to encrypt</param>
        /// <param name="password">Password used for encryption </param>
        /// <returns>Encrypted string</returns> 
        public static string Encrypt(string input, string password)
        {

            byte[] utfData = Encoding.UTF8.GetBytes(input);
            byte[] saltBytes = Encoding.UTF8.GetBytes(password);
            string encryptedString;
            using (AesManaged aes = new AesManaged())
            {
                Rfc2898DeriveBytes rfc = new Rfc2898DeriveBytes(password, saltBytes);

                aes.BlockSize = aes.LegalBlockSizes[0].MaxSize;
                aes.KeySize = aes.LegalKeySizes[0].MaxSize;
                aes.Key = rfc.GetBytes(aes.KeySize / 8);
                aes.IV = rfc.GetBytes(aes.BlockSize / 8);

                using (ICryptoTransform encryptTransform = aes.CreateEncryptor())
                {
                    using (MemoryStream encryptedStream = new MemoryStream())
                    {
                        using (CryptoStream encryptor =
                            new CryptoStream(encryptedStream, encryptTransform, CryptoStreamMode.Write))
                        {
                            encryptor.Write(utfData, 0, utfData.Length);
                            encryptor.Flush();
                            encryptor.Close();

                            byte[] encryptBytes = encryptedStream.ToArray();
                            encryptedString = Convert.ToBase64String(encryptBytes);
                        }
                    }
                }
            }
            return encryptedString;
        }

        /// <summary> 
        /// Decrypt a string 
        /// </summary> 
        /// <param name="input">Input string in base 64 format</param>
        /// <param name="password">Password for decryption</param>
        /// <returns>Decrypted string</returns> 
        public static string Decrypt(string input, string password)
        {

            byte[] encryptedBytes = Convert.FromBase64String(input);
            byte[] saltBytes = Encoding.UTF8.GetBytes(password);
            string decryptedString;
            using (var aes = new AesManaged())
            {
                Rfc2898DeriveBytes rfc = new Rfc2898DeriveBytes(password, saltBytes);
                aes.BlockSize = aes.LegalBlockSizes[0].MaxSize;
                aes.KeySize = aes.LegalKeySizes[0].MaxSize;
                aes.Key = rfc.GetBytes(aes.KeySize / 8);
                aes.IV = rfc.GetBytes(aes.BlockSize / 8);

                using (ICryptoTransform decryptTransform = aes.CreateDecryptor())
                {
                    using (MemoryStream decryptedStream = new MemoryStream())
                    {
                        CryptoStream decryptor =
                            new CryptoStream(decryptedStream, decryptTransform, CryptoStreamMode.Write);
                        decryptor.Write(encryptedBytes, 0, encryptedBytes.Length);
                        decryptor.Flush();
                        decryptor.Close();

                        byte[] decryptBytes = decryptedStream.ToArray();
                        decryptedString = Encoding.UTF8.GetString(decryptBytes, 0, decryptBytes.Length);
                    }
                }
            }

            return decryptedString;
        }
    }
}
