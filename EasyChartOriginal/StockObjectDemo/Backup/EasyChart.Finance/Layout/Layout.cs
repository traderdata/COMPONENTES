using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using Easychart.Finance.DataProvider;

namespace Easychart.Finance
{
    public enum TextAlign { Left, Right }
    /// <summary>
    /// Control the layout of the chart, draw labels on to the chart
    /// </summary>
    public class Layout
    {
        Font NowFont = new Font("Verdana", 8);
        Color NowColor = Color.Black;
        Rectangle NowBack = Rectangle.Empty;
        Color NowBackColor = Color.Empty;
        bool NowUseColor = false;
        Font NowColorFont = new Font("Verdana", 8);
        TextAlign NowAlign;

        Rectangle NowFrame = Rectangle.Empty;
        Color NowFrameColor = Color.Empty;
        string NowText = "";
        string NowIcon = "";
        Point NowPos = Point.Empty;
        TypeConverter tcFont = null;
        TypeConverter tcColor = null;

        Color[] ColorTextColor = new Color[] { Color.White, Color.White, Color.White };

        public Layout()
        {
        }

        public string CompanyName;
        public string URL;
        public Rectangle ChartRect;
        public long StartTick;
        public LayoutLabelCollection Labels = new LayoutLabelCollection();

        public void Merge(Layout L)
        {
            foreach (LayoutLabel ll in L.Labels)
                Labels.Add(ll);
        }

        private void GetNameValue(string s, out string Name, out string Value)
        {
            int i = s.IndexOf('=');
            if (i > 0)
            {
                Name = s.Substring(0, i);
                Value = s.Substring(i + 1);
                if (Value.StartsWith("("))
                    Value = Value.Substring(1);
                if (Value.EndsWith(")"))
                    Value = Value.Substring(0, Value.Length - 1);
            }
            else
            {
                Name = s;
                Value = "";
            }
        }

        private string GetOneTag(string s, out string Name, out string Value)
        {
            int k = 0;
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == '(') k++;
                if (s[i] == ')') k--;
                if (s[i] == ';' && k == 0)
                {
                    GetNameValue(s.Substring(0, i), out Name, out Value);
                    return s.Substring(i + 1);
                }
            }
            GetNameValue(s, out Name, out Value);
            return "";
        }

        Rectangle ParseRect(Rectangle R, string s)
        {
            string[] ss = s.Split(',');
            try
            {
                if (ss.Length == 4)
                {
                    int i1 = int.Parse(ss[0]);
                    if (i1 >= 0)
                        R.X += i1;
                    else
                        R.X = R.Right + i1 + 1;

                    int i2 = int.Parse(ss[1]);
                    if (i2 >= 0)
                        R.Y += i2;
                    else
                        R.Y = R.Bottom + i2 + 1;

                    int i3 = int.Parse(ss[2]);
                    if (i3 > 0)
                        R.Width = i3 - R.Left;
                    else
                        R.Width = R.Width + i3 + 1 - R.Left;

                    int i4 = int.Parse(ss[3]);
                    if (i4 > 0)
                        R.Height = i4 - R.Top;
                    else
                        R.Height = R.Height + i4 + 1 - R.Top;

                    R.Width = Math.Abs(R.Width);
                    R.Height = Math.Abs(R.Height);
                    return R;
                }
                else return R;
            }
            catch
            {
                return R;
            }
        }

        Point ParsePoint(Rectangle R, string s)
        {
            string[] ss = s.Split(',');
            try
            {
                return new Point(R.X + int.Parse(ss[0]), R.Y + int.Parse(ss[1]));
            }
            catch
            {
                return Point.Empty;
            }
        }

        private void AddLabel()
        {
            LayoutLabel ll = new LayoutLabel();
            ll.Back = NowBack;
            ll.BackColor = NowBackColor;
            ll.Frame = NowFrame;
            ll.FrameColor = NowFrameColor;
            ll.Pos = NowPos;
            ll.Text = NowText;
            ll.TextColor = NowColor;
            ll.UseColor = NowUseColor;
            ll.Icon = NowIcon;
            ll.Align = NowAlign;
            if (NowUseColor)
                ll.TextFont = NowColorFont;
            else ll.TextFont = NowFont;
            Labels.Add(ll);
        }

        private void InternalParse(string s, Rectangle Rect) //,CommonDataProvider cdp
        {
            while (s != "")
            {
                string Name;
                string Value;
                s = GetOneTag(s, out Name, out Value);
                //Don't know why, if include this in the switch, will raise a security exception in asp.net 2.0
                if (Name == "Font" || Name == "ColorFont")
                {
                    if (tcFont == null)
                        tcFont = TypeDescriptor.GetConverter(typeof(Font));
                    Font F = (Font)tcFont.ConvertFromString(null, CultureInfo.InvariantCulture, Value);

                    if (Name == "Font")
                        NowFont = F;
                    else NowColorFont = F;
                }
                else
                    switch (Name)
                    {
                        case "ChartRect":
                            ChartRect = ParseRect(Rect, Value);
                            break;
                        //					case "Font":
                        //					case "ColorFont":
                        //						//if (tcFont==null)
                        //						//	tcFont = TypeDescriptor.GetConverter(typeof(Font));
                        //						//Font F = new Font("Verdana",8);// FormulaHelper.StringToFont(Value);
                        //							//(Font)tcFont.ConvertFromString(null,CultureInfo.InvariantCulture,Value);
                        //
                        ////						if (Name=="Font")
                        ////							NowFont = F;
                        ////						else NowColorFont = F;
                        //
                        //						break;
                        case "Align":
                            if (Value == "Right")
                                NowAlign = TextAlign.Right;
                            else NowAlign = TextAlign.Left;
                            break;
                        case "Color":
                            if (tcColor == null)
                                tcColor = TypeDescriptor.GetConverter(typeof(Color));
                            NowColor = (Color)tcColor.ConvertFromString(null, CultureInfo.InvariantCulture, Value);
                            //NowColor = ColorTranslator.FromHtml(Value);
                            break;
                        case "Back":
                            NowBack = ParseRect(Rect, Value);
                            NowText = "";
                            NowBackColor = NowColor;
                            AddLabel();
                            NowBack = Rectangle.Empty;
                            break;
                        case "Frame":
                            NowFrame = ParseRect(Rect, Value);
                            NowText = "";
                            NowFrameColor = NowColor;
                            AddLabel();
                            NowFrame = Rectangle.Empty;
                            break;
                        case "Pos":
                            NowPos = ParsePoint(Rect, Value);
                            break;
                        case "Text":
                            NowText = Value;
                            NowUseColor = false;
                            AddLabel();
                            NowPos = Point.Empty;
                            NowText = "";
                            break;
                        case "Icon":
                            NowIcon = Value;
                            AddLabel();
                            NowPos = Point.Empty;
                            NowIcon = "";
                            break;
                        case "ColorText":
                            int i3 = 0;
                            while (true)
                            {
                                int i1 = Value.IndexOf("{", i3);
                                if (i1 >= 0)
                                {
                                    int i2 = Value.IndexOf("}", i1);
                                    if (i2 > 0)
                                    {
                                        NowUseColor = false;
                                        NowText = Value.Substring(i3, i1 - i3);
                                        AddLabel();
                                        NowPos = Point.Empty;
                                        NowUseColor = true;
                                        NowText = Value.Substring(i1, i2 - i1 + 1);
                                        AddLabel();
                                        i3 = i2 + 1;
                                    }
                                    else
                                        break;
                                }
                                else
                                    break;
                            }
                            break;
                    }
            }
        }

        //"ChartRect=(0,20,0,-20);Font=(Verdana,7pt);Color=Red;Pos=(0,0);Text=(Close:{C})"	
        static public Layout ParseString(string s, Rectangle Rect) //,CommonDataProvider cdp
        {
            Layout L = new Layout();
            L.InternalParse(s, Rect); //,cdp
            return L;
        }

        double[] DATE = null;
        double[] CLOSE = null;
        int Bar = 0;

        public double GetLast()
        {
            if (Bar > 0 && Bar < CLOSE.Length + 1)
                return CLOSE[Bar - 1];
            return 0;
        }

        public int GetColorIndex(double d1, double d2)
        {
            if (d1 < d2)
                return 0;
            else if (d1 == d2)
                return 1;
            else return 2;
        }

        private string ReplaceText(FormulaChart fc, string s, out int ColorIndex)
        {
            ColorIndex = 0;
            while (true)
            {
                int i1 = s.IndexOf('{');
                int i2 = s.IndexOf('}');
                if (i2 > i1)
                {
                    string s1 = s.Substring(i1 + 1, i2 - i1 - 1);
                    int i = s1.IndexOf(':');
                    string s3 = "";
                    string s2 = s1;
                    if (i > 0)
                    {
                        s2 = s1.Substring(0, i);
                        s3 = s1.Substring(i + 1);
                    }
                    if (s2 == "Company") s2 = CompanyName;
                    else if (s2 == "URL") s2 = URL;
                    else if (s2 == "Time") s2 = ((DateTime.Now.Ticks - StartTick) / 10000).ToString() + "ms";
                    else
                    {
                        IDataProvider idp = fc.DataProvider;
                        FormulaArea Area = fc.MainArea;
                        //if (DATE==null)
                        //{
                        DATE = idp["DATE"];
                        CLOSE = idp["CLOSE"];
                        if (CLOSE == null || CLOSE.Length == 0)
                            return s;
                        if (Bar < 0)
                            for (int j = CLOSE.Length - 1; j >= 0; j--)
                                if (!double.IsNaN(CLOSE[j]))
                                {
                                    Bar = j;
                                    break;
                                }
                        if (Bar < 0) Bar = 0;
                        //}
                        if (string.Compare(s2, "D") == 0)
                        {
                            if (s3 == "")
                                s3 = "dd-MMM-yyyy dddd";
                            string LastTradeTime = idp.GetStringData("LastTradeTime");
                            DateTime dtLastTrade = fc.IndexToDate(Bar);
                            if (LastTradeTime != null && LastTradeTime != "")
                                dtLastTrade = FormulaHelper.ToDateDef(LastTradeTime, dtLastTrade);
                            s2 = dtLastTrade.ToString(s3); //,DateTimeFormatInfo.InvariantInfo
                        }
                        else
                        {
                            string s4 = idp.GetStringData(s2);
                            if (s4 != null)
                                s2 = s4;
                            else
                            {
                                try
                                {
                                    double Last = 0;
                                    //if (s3=="") s3 = "f2";
                                    s3 = Area.AxisY.Format;
                                    if (string.Compare(s2, "Change", true) == 0)
                                    {
                                        Last = GetLast();
                                        if (Bar < CLOSE.Length)
                                        {
                                            double n = CLOSE[Bar];
                                            ColorIndex = GetColorIndex(Last, n);
                                            string Percent = ((n - Last) / Last).ToString("p2"); //,NumberFormatInfo.InvariantInfo
                                            if (!Percent.StartsWith("-"))
                                                Percent = "+" + Percent;
                                            if (s3 == "")
                                                s3 = "f" + FormulaHelper.TestBestFormat(n, 2);
                                            s2 = (n - Last).ToString(s3) + "(" + Percent + ")"; //,NumberFormatInfo.InvariantInfo
                                        }
                                    }
                                    else
                                    {
                                        double d = 0;
                                        if (s2 == "LC")
                                            d = GetLast();
                                        else
                                        {
                                            FormulaData fd = idp[s2];
                                            double[] dd = null;
                                            if (!object.Equals(fd, null))
                                                dd = fd.Data;
                                            if (dd != null && Bar < dd.Length)
                                            {
                                                Last = GetLast();
                                                d = dd[Bar];
                                                if (s2 == "VOLUME")
                                                {
                                                    s3 = "f0";
                                                    ColorIndex = GetColorIndex(Last, CLOSE[Bar]);
                                                }
                                                else
                                                    ColorIndex = GetColorIndex(Last, d);
                                            }
                                            else d = double.NaN;
                                        }
                                        if (s3 == "")
                                            s3 = "f" + FormulaHelper.TestBestFormat(d, 2);
                                        s2 = FormulaHelper.FormatDouble(d, s3);
                                    }
                                }
                                catch
                                {
                                    s2 = "";
                                }
                            }
                        }
                    }
                    s = s.Substring(0, i1) + s2 + s.Substring(i2 + 1);
                }
                else break;
            }
            return s;
        }

        public void SetFont(Font F)
        {
            foreach (LayoutLabel ll in Labels)
                ll.TextFont = F;
        }

        public void Render(Graphics g, Rectangle R, FormulaChart fc)
        {
            Render(g, R, fc, Point.Empty);
        }

        public Point Render(Graphics g, Rectangle R, FormulaChart fc, Point NowPos)
        {
            return Render(g, R, fc, NowPos, -1);
        }

        private void AdjustNowPos(LayoutLabel ll, SizeF sf, ref Rectangle R, ref Point NowPos)
        {
            if (ll.Pos != Point.Empty)
            {
                if (ll.Pos.X < 0)
                {
                    if (ll.Align == TextAlign.Right)
                        NowPos.X = R.Right + ll.Pos.X;
                    else NowPos.X = R.Right + ll.Pos.X - (int)sf.Width;
                }
                else NowPos.X = R.X + ll.Pos.X;
                if (ll.Pos.Y < 0)
                    NowPos.Y = R.Bottom + ll.Pos.Y - (int)sf.Height;
                else NowPos.Y = R.Y + ll.Pos.Y;
            }
        }

        public Point Render(Graphics g, Rectangle R, FormulaChart fc, Point NowPos, int Bar)
        {
            this.Bar = Bar;
            foreach (LayoutLabel ll in Labels)
            {
                if (ll.Frame != Rectangle.Empty)
                    g.DrawRectangle(new Pen(ll.FrameColor), ll.Frame);
                if (ll.Back != Rectangle.Empty)
                    g.FillRectangle(new SolidBrush(ll.BackColor), ll.Back);
                int ColorIndex = 0;
                string s = ReplaceText(fc, ll.Text, out ColorIndex);

                SizeF sf = SizeF.Empty;
                if (ll.Text != null && ll.Text != "")
                {
                    sf = g.MeasureString(s, ll.TextFont);
                    AdjustNowPos(ll, sf, ref R, ref NowPos);

                    Color C;
                    if (ll.UseColor)
                        C = ColorTextColor[ColorIndex];
                    else C = ll.TextColor;

                    g.DrawString(s, ll.TextFont, new SolidBrush(C), NowPos);
                }
                if (ll.Icon != null && ll.Icon != "")
                {
                    string IconFile = FormulaHelper.GetImageFile(ll.Icon);
                    if (File.Exists(IconFile))
                    {
                        Image I = Bitmap.FromFile(IconFile);
                        sf = new SizeF(I.Width, I.Height);
                        AdjustNowPos(ll, sf, ref R, ref NowPos);
                        g.DrawImage(I, NowPos);
                    }
                }
                if (ll.Pos.X >= 0)
                    NowPos.X += (int)sf.Width;

            }
            return NowPos;
        }
    }
}