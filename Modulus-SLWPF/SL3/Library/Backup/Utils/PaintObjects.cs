using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
#if SILVERLIGHT
using PathConverter;
#endif

namespace ModulusFE.PaintObjects
{
    internal static class ColorExtensions
    {
        public static int ToArgb(this Color color)
        {
            return (color.A << 32) & (color.R << 16) & (color.G << 8) & color.B;
        }

        public static Color Invert(this Color aColor)
        {
            const int mask_b = 255;
            const int mask_g = mask_b << 8;
            const int mask_r = mask_g << 8;

            int color = aColor.ToArgb();

            int r = ((color & mask_r) >> 16);
            int g = ((color & mask_g) >> 8);
            int b = (color & mask_b);

            return Color.FromArgb(0xFF, (byte)~r, (byte)~b, (byte)~g);
        }

        public static void ToHSL(this Color color, out double H, out double S, out double L)
        {
            double r = color.R / 255.0;
            double g = color.G / 255.0;
            double b = color.B / 255.0;
            double cmax = Math.Max(r, Math.Max(g, b));
            double cmin = Math.Min(r, Math.Min(g, b));

            L = (cmax + cmin) / 2.0;

            if (cmax == cmin)
            {
                S = 0;
                H = 0; // it's really undefined
            }
            else
            {
                if (L < 0.5)
                    S = (cmax - cmin) / (cmax + cmin);
                else
                    S = (cmax - cmin) / (2.0 - cmax - cmin);

                double delta = cmax - cmin;

                if (r == cmax)
                    H = (g - b) / delta;
                else if (g == cmax)
                    H = 2.0 + (b - r) / delta;
                else
                    H = 4.0 + (r - g) / delta;

                H /= 6.0;

                if (H < 0.0)
                    H += 1;
            }
            H *= 240.0;
            S *= 240.0;
            L *= 240.0;
        }

        public static double HuetoRGB(double m1, double m2, double h)
        {
            if (h < 0) h += 1.0;
            if (h > 1) h -= 1.0;
            if (6.0 * h < 1)
                return (m1 + (m2 - m1) * h * 6.0);
            if (2.0 * h < 1)
                return m2;
            if (3.0 * h < 2.0)
                return (m1 + (m2 - m1) * ((2.0 / 3.0) - h) * 6.0);
            return m1;
        }

        public static Color FromHSL(double H, double S, double L)
        {
            double r, g, b;

            H /= 240.0;
            S /= 240.0;
            L /= 240.0;

            if (S == 0)
            {
                r = g = b = L;
            }
            else
            {
                double m2;
                if (L <= 0.5)
                    m2 = L * (1.0 + S);
                else
                    m2 = L + S - L * S;

                double m1 = 2.0 * L - m2;
                r = HuetoRGB(m1, m2, H + 1.0 / 3.0);
                g = HuetoRGB(m1, m2, H);
                b = HuetoRGB(m1, m2, H - 1.0 / 3.0);
            }

            return Color.FromArgb(0xFF, (byte)(r * 255.0), (byte)(g * 255.0), (byte)(b * 255.0));
        }

        /// <summary>
        /// Sets the absolute brightness of a colour
        /// </summary>
        /// <param name="c">Original color</param>
        /// <param name="brightness">The luminance level to impose</param>
        /// <returns>an adjusted colour</returns>
        public static Color SetBrightness(this Color c, double brightness)
        {
            double h, s, l;
            c.ToHSL(out h, out s, out l);
            l = brightness;
            return FromHSL(h, s, l);
        }

        ///  <summary>
        /// Modifies an existing brightness level
        /// </summary>
        /// <remarks>
        /// To reduce brightness use a number smaller than 1. To increase brightness use a number larger tnan 1
        /// </remarks>
        /// <param name="c">The original colour</param>
        /// <param name="brightness">The luminance delta</param>
        /// <returns>An adjusted colour</returns>
        public static Color ModifyBrightness(this Color c, double brightness)
        {
            double h, s, l;
            c.ToHSL(out h, out s, out l);
            l *= brightness;
            return FromHSL(h, s, l);
        }

        ///  <summary>
        /// Sets the absolute saturation level
        /// </summary>
        /// <remarks>Accepted values 0-1</remarks>
        /// <param name="c">An original colour</param>
        /// <param name="Saturation">The saturation value to impose</param>
        /// <returns>An adjusted colour</returns>
        public static Color SetSaturation(this Color c, double Saturation)
        {
            double h, s, l;
            c.ToHSL(out h, out s, out l);
            s = Saturation;
            return FromHSL(h, s, l);
        }

        ///  <summary>
        /// Modifies an existing Saturation level
        /// </summary>
        /// <remarks>
        /// To reduce Saturation use a number smaller than 1. To increase Saturation use a number larger tnan 1
        /// </remarks>
        /// <param name="c">The original colour</param>
        /// <param name="Saturation">The saturation delta</param>
        /// <returns>An adjusted colour</returns>
        public static Color ModifySaturation(this Color c, double Saturation)
        {
            double h, s, l;
            c.ToHSL(out h, out s, out l);
            s *= Saturation;
            return FromHSL(h, s, l);
        }

        ///  <summary>
        /// Sets the absolute Hue level
        /// </summary>
        /// <remarks>Accepted values 0-1</remarks>
        /// <param name="c">An original colour</param>
        /// <param name="Hue">The Hue value to impose</param>
        /// <returns>An adjusted colour</returns>
        public static Color SetHue(this Color c, double Hue)
        {
            double h, s, l;
            c.ToHSL(out h, out s, out l);
            h = Hue;
            return FromHSL(h, s, l);
        }

        ///  <summary>
        /// Modifies an existing Hue level
        /// </summary>
        /// <remarks>
        /// To reduce Hue use a number smaller than 1. To increase Hue use a number larger tnan 1
        /// </remarks>
        /// <param name="c">The original colour</param>
        /// <param name="Hue">The Hue delta</param>
        /// <returns>An adjusted colour</returns>
        public static Color ModifyHue(Color c, double Hue)
        {
            double h, s, l;
            c.ToHSL(out h, out s, out l);
            h *= Hue;
            return FromHSL(h, s, l);
        }
    }

    internal static class PanelExtensions
    {
        /// <summary>
        /// Assigns the element a z-index which will ensure that 
        /// it is in front of every other element in the Canvas.
        /// The z-index of every element whose z-index is between 
        /// the element's old and new z-index will have its z-index 
        /// decremented by one.
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="targetElement">
        /// The element to be sent to the front of the z-order.
        /// </param>
        public static void BringToFront(this Panel canvas, UIElement targetElement)
        {
            canvas.UpdateZOrder(targetElement, true);
        }

        /// <summary>
        /// Assigns the element a z-index which will ensure that 
        /// it is behind every other element in the Canvas.
        /// The z-index of every element whose z-index is between 
        /// the element's old and new z-index will have its z-index 
        /// incremented by one.
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="targetElement">
        /// The element to be sent to the back of the z-order.
        /// </param>
        public static void SendToBack(this Panel canvas, UIElement targetElement)
        {
            canvas.UpdateZOrder(targetElement, false);
        }

        #region UpdateZOrder

        /// <summary>
        /// Helper method used by the BringToFront and SendToBack methods.
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="element">
        /// The element to bring to the front or send to the back.
        /// </param>
        /// <param name="bringToFront">
        /// Pass true if calling from BringToFront, else false.
        /// </param>
        private static void UpdateZOrder(this Panel canvas, UIElement element, bool bringToFront)
        {
            #region Safety Check

            if (element == null)
                throw new ArgumentNullException("element");

            if (!canvas.Children.Contains(element))
                throw new ArgumentException("Must be a child element of the Canvas.", "element");

            #endregion // Safety Check

            #region Calculate Z-Indici And Offset

            // Determine the Z-Index for the target UIElement.
            int elementNewZIndex = -1;
            if (bringToFront)
            {
                elementNewZIndex += canvas.Children.Cast<UIElement>().Count(elem => elem.Visibility != Visibility.Collapsed);
            }
            else
            {
                elementNewZIndex = 0;
            }

            // Determine if the other UIElements' Z-Index 
            // should be raised or lowered by one. 
            int offset = (elementNewZIndex == 0) ? +1 : -1;

            int elementCurrentZIndex = Canvas.GetZIndex(element);

            #endregion // Calculate Z-Indici And Offset

            #region Update Z-Indici

            // Update the Z-Index of every UIElement in the Canvas.
            foreach (UIElement childElement in canvas.Children)
            {
                if (childElement == element)
                    Canvas.SetZIndex(element, elementNewZIndex);
                else
                {
                    int zIndex = Canvas.GetZIndex(childElement);

                    // Only modify the z-index of an element if it is  
                    // in between the target element's old and new z-index.
                    if (bringToFront && elementCurrentZIndex < zIndex ||
                        !bringToFront && zIndex < elementCurrentZIndex)
                    {
                        Canvas.SetZIndex(childElement, zIndex + offset);
                    }
                }
            }

            #endregion // Update Z-Indici
        }

        #endregion // UpdateZOrder
    }

    internal static class RectangleExtensions
    {
        public static Point TopLeft(this System.Windows.Shapes.Rectangle self)
        {
            return new Point(Canvas.GetLeft(self), Canvas.GetTop(self));
        }

        public static Point TopRight(this System.Windows.Shapes.Rectangle self)
        {
            return new Point(Canvas.GetLeft(self) + self.ActualWidth, Canvas.GetTop(self));
        }

        public static Point BottomLeft(this System.Windows.Shapes.Rectangle self)
        {
            return new Point(Canvas.GetLeft(self), Canvas.GetTop(self) + self.ActualHeight);
        }

        public static Point BottomRight(this System.Windows.Shapes.Rectangle self)
        {
            return new Point(Canvas.GetLeft(self) + self.ActualWidth, Canvas.GetTop(self) + self.ActualHeight);
        }

        public static Point TopCenter(this System.Windows.Shapes.Rectangle self)
        {
            return new Point(Canvas.GetLeft(self) + self.ActualWidth / 2, Canvas.GetTop(self));
        }

        public static Point BottomCenter(this System.Windows.Shapes.Rectangle self)
        {
            return new Point(Canvas.GetLeft(self) + self.ActualWidth / 2, Canvas.GetTop(self) + self.ActualHeight);
        }

        public static Point MiddleLeft(this System.Windows.Shapes.Rectangle self)
        {
            return new Point(Canvas.GetLeft(self), Canvas.GetTop(self) + self.ActualHeight / 2);
        }

        public static Point MiddleRight(this System.Windows.Shapes.Rectangle self)
        {
            return new Point(Canvas.GetLeft(self) + self.ActualWidth, Canvas.GetTop(self) + self.ActualHeight / 2);
        }

        public static void FadeVert(this System.Windows.Shapes.Rectangle self, Color color1, Color color2)
        {
            LinearGradientBrush gr =
                new LinearGradientBrush { EndPoint = new Point(0.5, 1), StartPoint = new Point(0.5, 0) };
            GradientStop grStop1 = new GradientStop { Color = color1, Offset = 0 };
            gr.GradientStops.Add(grStop1);
            GradientStop grStop2 = new GradientStop { Color = color2, Offset = 1 };
            gr.GradientStops.Add(grStop2);
            self.Fill = gr;
        }

        public static void FadeHorz(this System.Windows.Shapes.Rectangle self, Color color1, Color color2)
        {
            LinearGradientBrush gr = new LinearGradientBrush { EndPoint = new Point(1, 0.5), StartPoint = new Point(0, 0.5) };
            GradientStop grStop1 = new GradientStop { Color = color1, Offset = 0 };
            gr.GradientStops.Add(grStop1);
            GradientStop grStop2 = new GradientStop { Color = color2, Offset = 1 };
            gr.GradientStops.Add(grStop2);
            self.Fill = gr;
        }
    }

    internal static class EllipseExtensions
    {
        public static Point TopLeft(this System.Windows.Shapes.Ellipse self)
        {
            return new Point(Canvas.GetLeft(self), Canvas.GetTop(self));
        }

        public static Point TopRight(this System.Windows.Shapes.Ellipse self)
        {
            return new Point(Canvas.GetLeft(self) + self.ActualWidth, Canvas.GetTop(self));
        }

        public static Point BottomLeft(this System.Windows.Shapes.Ellipse self)
        {
            return new Point(Canvas.GetLeft(self), Canvas.GetTop(self) + self.ActualHeight);
        }

        public static Point BottomRight(this System.Windows.Shapes.Ellipse self)
        {
            return new Point(Canvas.GetLeft(self) + self.ActualWidth, Canvas.GetTop(self) + self.ActualHeight);
        }

        public static Point TopCenter(this System.Windows.Shapes.Ellipse self)
        {
            return new Point(Canvas.GetLeft(self) + self.ActualWidth / 2, Canvas.GetTop(self));
        }

        public static Point BottomCenter(this System.Windows.Shapes.Ellipse self)
        {
            return new Point(Canvas.GetLeft(self) + self.ActualWidth / 2, Canvas.GetTop(self) + self.ActualHeight);
        }

        public static Point MiddleLeft(this System.Windows.Shapes.Ellipse self)
        {
            return new Point(Canvas.GetLeft(self), Canvas.GetTop(self) + self.ActualHeight / 2);
        }

        public static Point MiddleRight(this System.Windows.Shapes.Ellipse self)
        {
            return new Point(Canvas.GetLeft(self) + self.ActualWidth, Canvas.GetTop(self) + self.ActualHeight / 2);
        }
    }

    internal static class PointExtensions
    {
        public static double Distance(this Point self, Point another)
        {
            return Math.Sqrt((another.X - self.X) * (another.X - self.X) + (another.Y - self.Y) * (another.Y - self.Y));
        }

        public static bool InPolygon(this Point p, Point[] polygon)
        {
            int i, j;
            bool c = false;

            for (i = 0, j = polygon.Length - 1; i < polygon.Length; j = i++)
            {
                if ((((polygon[i].Y <= p.Y) && (p.Y < polygon[j].Y)) ||
                         ((polygon[j].Y <= p.Y) && (p.Y < polygon[i].Y))) &&
                        (p.X < (polygon[j].X - polygon[i].X) * (p.Y - polygon[i].Y) / (polygon[j].Y - polygon[i].Y) + polygon[i].X))
                    c = !c;
            }
            return c;
        }
    }

    internal static class CanvasExtensions
    {
        /// <summary>
        /// gap permited for hit testing objects on the chart
        /// </summary>
        private const int HitTestGap = 2;

        //    private static Rectangle _hitRect;
        public static object HitTest(this Canvas self, Point p, Type[] neededObjects)
        {
#if WPF
            for (int dx = -HitTestGap; dx <= HitTestGap; dx++)
                for (int dy = -HitTestGap; dy <= HitTestGap; dy++)
                {
                    Point point = new Point(p.X + dx, p.Y + dy);
                    HitTestResult hitTestResult = VisualTreeHelper.HitTest(self, point);
                    if (hitTestResult == null) continue;
                    FrameworkElement frameworkElement = hitTestResult.VisualHit as FrameworkElement;
                    if (frameworkElement == null || frameworkElement.Tag == null) continue;
                    Type frameworkElementType = frameworkElement.Tag.GetType();
                    if (neededObjects.Any(type => frameworkElementType.IsSubclassOf(type) || frameworkElementType.Equals(type)))
                    {
                        ContextLine contextLine = ((FrameworkElement)hitTestResult.VisualHit).Tag as ContextLine;
                        if (contextLine != null && !contextLine.Visible)
                        {
                            return contextLine.ContextAbleLineStudy.LineStudy;
                        }

                        return ((FrameworkElement)hitTestResult.VisualHit).Tag;
                    }
                }
            return null;
#endif
#if SILVERLIGHT
            Rect hitRect = new Rect(p.X - HitTestGap, p.Y - HitTestGap, 2 * HitTestGap, 2 * HitTestGap);
            Point point = self.TransformToVisual(Application.Current.RootVisual).Transform(new Point(hitRect.Left, hitRect.Top));
            return
                (from FrameworkElement element in
                     VisualTreeHelper.FindElementsInHostCoordinates(new Rect(point.X, point.Y, hitRect.Width, hitRect.Height), self)
                 where element.Tag != null
                 let frameworkElementType = element.Tag.GetType()
                 where neededObjects.Any(type => frameworkElementType.IsSubclassOf(type) || frameworkElementType == type)
                 select element.Tag).FirstOrDefault();
#endif
        }

#if SILVERLIGHT
        public static IEnumerable<FrameworkElement> FintElementsFromRect(Canvas root, Rect rect)
        {
            return from FrameworkElement child in root.Children
                   from corner in child.GetCanvasCorners().Where(rect.Contains)
                   select child;
        }

        public static IEnumerable<Point> GetCanvasCorners(this FrameworkElement element)
        {
            if (!(element.Parent is Canvas))
                throw new ArgumentException("This only works if FrameworkElement is on a Canas!");

            if (element is System.Windows.Shapes.Line)
            {
                System.Windows.Shapes.Line line = (System.Windows.Shapes.Line)element;
                yield return new Point(line.X1, line.Y1);
                yield return new Point(line.X2, line.Y2);
            }
            else
            {
                double left = Canvas.GetLeft(element);
                double top = Canvas.GetTop(element);
                double width = element.Width;
                double height = element.Height;

                yield return new Point(left, top);
                yield return new Point(left + width, top);
                yield return new Point(left, top + height);
                yield return new Point(left + width, top + height);
            }
        }
#endif
    }

    internal static class AsyncExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> items, Action<T> act)
        {
            foreach (T item in items)
            {
                act(item);
            }
        }

        public static void DoParallel<TInput>(this Action<TInput> f, IEnumerable<TInput> arg, Action callback)
        {
            var localArg = arg.ToList();

            int total = localArg.Count(), count = 0;
            localArg.ForEach(value =>
                                             f.BeginInvoke(value, result =>
                                                                                            {
                                                                                                if (++count == total)
                                                                                                    callback();
                                                                                            }, null)
                );
        }

        public static void DoAsync<TInput>(this Action<TInput> f, TInput arg, Action callback)
        {
            f.BeginInvoke(arg, x => callback(), null);
        }

        public static void ForEachParalel<T>(this IEnumerable<T> items, Action<T> act, Action finalMethod)
        {
            var localItems = items.ToList();
            int total = localItems.Count(), count = 0;

            localItems.ForEach(
                x =>
                act.DoAsync(x, () =>
                                                {
                                                    if (++count == total)
                                                        finalMethod();
                                                })
                );
        }
    }

    internal static class StringBuilderExtensions
    {
        public static StringBuilder BeginGeometryGroup(this StringBuilder self)
        {
            if (self.Length > 0)
                self.Append(" ");

            self.AppendLine("<GeometryGroup xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\">")
                .AppendLine("<GeometryGroup.Children>");

            return self;
        }

        public static StringBuilder EndGeometryGroup(this StringBuilder self)
        {
            if (self.Length > 0)
                self.Append(" ");

            self.AppendLine("</GeometryGroup.Children>").AppendLine("</GeometryGroup>");

            return self;
        }

        private static CultureInfo _cultureWpf;

        private static void CreateWpfCulture()
        {
            if (_cultureWpf != null)
                return;
            _cultureWpf = new CultureInfo("EN-US")
                                            {
                                                NumberFormat =
                                                    {
                                                        NumberGroupSeparator = string.Empty,
                                                        NumberDecimalSeparator = "."
                                                    }
                                            };
        }

        public static StringBuilder AddSegment(this StringBuilder self, Point p1, Point p2)
        {
            if (self.Length > 0)
                self.Append(" ");

            CreateWpfCulture();

            self.Append("<LineGeometry StartPoint=\"")
                .Append(p1.X.ToString(_cultureWpf)).Append(",").Append(p1.Y.ToString(_cultureWpf))
                .Append("\" ")
                .Append("EndPoint=\"")
                .Append(p2.X.ToString(_cultureWpf)).Append(",").Append(p2.Y.ToString(_cultureWpf))
                .Append("\"")
                .AppendLine("/>");

            return self;
        }

        public static StringBuilder AddSegment(this StringBuilder self, double x1, double y1, double x2, double y2)
        {
            if (self.Length > 0)
                self.Append(" ");

            CreateWpfCulture();

            self.Append("<LineGeometry StartPoint=\"")
                .Append(x1.ToString(_cultureWpf)).Append(",").Append(y1.ToString(_cultureWpf))
                .Append("\" ")
                .Append("EndPoint=\"")
                .Append(x2.ToString(_cultureWpf)).Append(",").Append(y2.ToString(_cultureWpf))
                .Append("\"")
                .AppendLine("/>");

            return self;
        }

        public static StringBuilder AddRectangle(this StringBuilder self, Rect rc)
        {
            if (self.Length > 0)
                self.Append(" ");

            CreateWpfCulture();

            self.Append("<RectangleGeometry Rect=\"")
                .Append(rc.Left.ToString(_cultureWpf)).Append(",").Append(rc.Top.ToString(_cultureWpf))
                .Append(" ").Append(rc.Width.ToString(_cultureWpf)).Append(",").Append(rc.Height.ToString(_cultureWpf))
                .AppendLine("\"/>");

            return self;
        }

        public static StringBuilder BeginDataPath(this StringBuilder self)
        {
            return self;
        }

        public static StringBuilder AddDataPathLine(this StringBuilder self, Point p1, Point p2)
        {
            if (self.Length > 0)
                self.Append(" ");

            CreateWpfCulture();

            self.Append("M ").Append(p1.X.ToString(_cultureWpf)).Append(",").Append(p1.Y.ToString(_cultureWpf));
            self.Append(" L ").Append(p2.X.ToString(_cultureWpf)).Append(",").Append(p2.Y.ToString(_cultureWpf));

            return self;
        }

        public static StringBuilder EndDataPath(this StringBuilder self)
        {
            if (self.Length > 0)
                self.Append(" ");
            self.Append("Z");
            return self;
        }

        public static System.Windows.Media.Geometry GetDataPathGeometry(this StringBuilder self)
        {
#if WPF
            if (self.Length == 0)
                return System.Windows.Media.Geometry.Empty;
            return System.Windows.Media.Geometry.Parse(self.ToString());
#endif
#if SILVERLIGHT
            if (self.Length == 0)
                return null;
            StringToPathGeometryConverter converter = new StringToPathGeometryConverter();
            return converter.Convert(self.ToString());
#endif
        }

        public static T ConvertTo<T>(this StringBuilder self) where T : class
        {
            if (self.Length == 0)
                return null;
#if WPF
            return (T)XamlReader.Parse(self.ToString());
#endif
#if SILVERLIGHT
            return (T)XamlReader.Load(self.ToString());
#endif
        }
    }
}

