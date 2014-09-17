using System;
using System.Collections;
using System.Xml.Serialization;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Reflection;
using System.Globalization;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using System.IO;

namespace Easychart.Finance
{
    /// <summary>
    /// Stock chart skin and predefined skins
    /// </summary>
    public class FormulaSkin
    {
        static private Hashtable htSkins = new Hashtable();
        static private XmlSerializer xs;
        const string Ext = ".sml";

        public event EventHandler CollectionValueChanged;

        public void RefreshCollectionValue()
        {
            if (CollectionValueChanged != null)
                CollectionValueChanged(this, new EventArgs());
        }

        public string GetSkinFile(string FileName)
        {
            return FormulaHelper.SkinRoot + FileName + Ext;
        }

        public string GetSkinFile()
        {
            return GetSkinFile(skinName);
        }

        private string skinName = "";
        [DefaultValue(""), XmlAttribute, Category("Name"), Description("Skin Name")]
        public string SkinName
        {
            get
            {
                return skinName;
            }
            set
            {
                FormulaSkin fs = (FormulaSkin)htSkins[skinName];
                if (fs != null)
                {
                    htSkins.Remove(skinName);
                    htSkins[value] = fs;
                    string s = GetSkinFile();
                    if (File.Exists(s))
                        File.Move(s, GetSkinFile(value));
                }
                skinName = value;
            }
        }

        static public void AddSkin()
        {
        }

        private string displayName = "";
        [DefaultValue(""), XmlAttribute, Category("Name")]
        public string DisplayName
        {
            get
            {
                return displayName;
            }
            set
            {
                displayName = value;
            }
        }

        private FormulaBack back;
        /// <summary>
        /// The Back wall of the chart
        /// </summary>
        public FormulaBack Back
        {
            get
            {
                return back;
            }
            set
            {
                back = value;
            }
        }


        /// <summary>
        /// X - Axis of the chart
        /// </summary>
        [XmlIgnore]
        public FormulaAxisX AxisX;
        private AxisXCollection axisXs;
        /// <summary>
        /// Other X-Axis of the chart
        /// </summary>
        [Category("Axis"), XmlElement("AxisX")]
        [Editor(typeof(SkinCollectionEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public AxisXCollection AxisXs
        {
            get
            {
                return axisXs;
            }
            //			set
            //			{
            //				axisXs = value;
            //			}
        }

        private FormulaAxisY axisY;
        /// <summary>
        /// Y - Axis of the chart
        /// </summary>
        [Category("Axis")]
        public FormulaAxisY AxisY
        {
            get
            {
                return axisY;
            }
            set
            {
                axisY = value;
            }
        }

        private StickRenderType stickRenderType = StickRenderType.Default;
        /// <summary>
        /// true - Draw stock volume as line
        /// false - Draw stock volume as bar
        /// </summary>
        [DefaultValue(StickRenderType.Default), XmlAttribute]
        public StickRenderType StickRenderType
        {
            get
            {
                return stickRenderType;
            }
            set
            {
                stickRenderType = value;
            }
        }

        private StockRenderType stockRenderType = StockRenderType.Candle;
        /// <summary>
        /// Set stock render type.
        /// Bar,Candle or line
        /// </summary>
        [DefaultValue(StockRenderType.Candle), XmlAttribute]
        public StockRenderType StockRenderType
        {
            get
            {
                return stockRenderType;
            }
            set
            {
                stockRenderType = value;
            }
        }

        private ScaleType scaleType = ScaleType.Normal;
        /// <summary>
        /// Set y - axis scale type , Log or Normal.
        /// </summary>
        [DefaultValue(ScaleType.Normal), XmlAttribute]
        public ScaleType ScaleType
        {
            get
            {
                return scaleType;
            }
            set
            {
                scaleType = value;
            }
        }

        private Color[] colors = { Color.Blue, Color.Red, Color.Green, Color.Black, Color.Orange, Color.DarkGray, Color.DarkTurquoise };
        /// <summary>
        /// Set color map of the chart formulas
        /// </summary>
        [Description("Formula line color map."), XmlIgnore]
        public Color[] Colors
        {
            get
            {
                return colors;
            }
            set
            {
                colors = value;
            }
        }

        [Browsable(false)]
        [XmlAttribute]
        public string LineColors
        {
            get
            {
                TypeConverter tc = TypeDescriptor.GetConverter(typeof(Color));
                string[] ss = new string[colors.Length];
                for (int i = 0; i < colors.Length; i++)
                    ss[i] = tc.ConvertToString(null, FormulaHelper.enUS, colors[i]);
                return string.Join(",", ss);
            }
            set
            {
                TypeConverter tc = TypeDescriptor.GetConverter(typeof(Color));
                string[] ss = value.Split(',');
                colors = new Color[ss.Length];
                for (int i = 0; i < ss.Length; i++)
                    colors[i] = (Color)tc.ConvertFromString(null, FormulaHelper.enUS, ss[i]);
            }
        }

        /// <summary>
        /// Used by Xml serializer
        /// </summary>
        /// <returns></returns>
        public bool ShouldSerializeLinePen()
        {
            return PenMapper.NotDefault(LinePen);
        }

        private PenMapper linePen = new PenMapper(Color.Black);// new Pen(Color.Black);
        /// <summary>
        /// Set the formula line pen style
        /// </summary>
        public PenMapper LinePen
        {
            get
            {
                return linePen;
            }
            set
            {
                linePen = value;
            }
        }

        private PenMapper[] barPens = {
										  new PenMapper(Color.Black),
										  new PenMapper(Color.White),
										  new PenMapper(Color.Blue)};
        /// <summary>
        /// Set the pens for up , down or equal
        /// </summary>
        [Category("Stock Bars")]
        public PenMapper[] BarPens
        {
            get
            {
                return barPens;
            }
            set
            {
                barPens = value;
            }
        }

        private BrushMapper[] barBrushes = {
											   BrushMapper.Empty,
											   BrushMapper.Empty,
											   new BrushMapper(Color.Blue)};
        /// <summary>
        /// Set the brushes for up, down or equal
        /// </summary>
        [Category("Stock Bars")]
        public BrushMapper[] BarBrushes
        {
            get
            {
                return barBrushes;
            }
            set
            {
                barBrushes = value;
            }
        }

        /// <summary>
        /// Used by xml serializer
        /// </summary>
        /// <returns></returns>
        public bool ShouldSerializeNameBrush()
        {
            return BrushMapper.NotDefault(NameBrush);
        }

        private BrushMapper nameBrush = new BrushMapper(Color.Black);
        /// <summary>
        /// The brush for formula name on the chart
        /// </summary>
        public BrushMapper NameBrush
        {
            get
            {
                return nameBrush;
            }
            set
            {
                nameBrush = value;
            }
        }

        private Font nameFont = new Font("Verdana", 7);
        /// <summary>
        /// The font for formula name on the chart
        /// </summary>
        [XmlIgnore, DefaultValue(typeof(Font), "Verdana, 7pt")]
        public Font NameFont
        {
            get
            {
                return nameFont;
            }
            set
            {
                nameFont = value;
            }
        }

        private Font textFont = new Font("Verdana", 7);
        /// <summary>
        /// The font for text on the chart
        /// </summary>
        [XmlIgnore, DefaultValue(typeof(Font), "Verdana, 7pt")]
        public Font TextFont
        {
            get
            {
                return textFont;
            }
            set
            {
                textFont = value;
            }
        }

        /// <summary>
        /// Used by Xml serializer
        /// </summary>
        /// <returns></returns>
        public bool ShouldSerializeCursorPen()
        {
            return PenMapper.NotDefault(CursorPen);
        }

        private PenMapper cursorPen = new PenMapper(Color.Black);
        /// <summary>
        /// The pen for screen cursor
        /// </summary>
        public PenMapper CursorPen
        {
            get
            {
                return cursorPen;
            }
            set
            {
                cursorPen = value;
            }
        }

        private bool showXAxisInLastArea;
        /// <summary>
        /// Show X-Axis in last area only
        /// </summary>
        [DefaultValue(false), XmlAttribute]
        public bool ShowXAxisInLastArea
        {
            get
            {
                return showXAxisInLastArea;
            }
            set
            {
                showXAxisInLastArea = value;
            }
        }

        private bool showValueLabel = true;
        [DefaultValue(true), XmlAttribute]
        public bool ShowValueLabel
        {
            get
            {
                return showValueLabel;
            }
            set
            {
                showValueLabel = value;
            }
        }

        private XFormatCollection allXFormats;
        [Editor(typeof(SkinCollectionEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [XmlElement("Format")]
        public XFormatCollection AllXFormats
        {
            get
            {
                return allXFormats;
            }
            set
            {
                allXFormats = value;
            }
        }

        public FormulaSkin()
        {
            AxisX = new FormulaAxisX();
            axisXs = new AxisXCollection();
            axisXs.Add(AxisX);

            AxisY = new FormulaAxisY();
            Back = new FormulaBack();
        }

        public FormulaSkin(string skinName)
            : this()
        {
            this.skinName = skinName;
        }

        public void Bind(FormulaChart fc)
        {
            foreach (FormulaArea fa in fc.Areas)
            {
                if (fa.AxisXs.Count != AxisXs.Count)
                {
                    fa.AxisXs.Clear();
                    for (int i = 0; i < AxisXs.Count; i++)
                        fa.AxisXs.Add(new FormulaAxisX()); //AxisXs[i]
                    if (AxisXs.Count > 0)
                        fa.AxisX = fa.AxisXs[0];
                }

                for (int i = 0; i < AxisXs.Count; i++)
                    fa.AxisXs[i].CopyFrom(AxisXs[i]);

                foreach (FormulaAxisY fay in fa.AxisYs)
                    fay.CopyFrom(AxisY);
                fa.Back = (FormulaBack)Back.Clone();
                fa.Colors = (Color[])Colors.Clone();
                fa.LinePen = LinePen.GetPen();// LinePen.Clone();

                fa.BarPens = new Pen[] { BarPens[0].GetPen(), BarPens[1].GetPen(), BarPens[2].GetPen() };// (Pen[])BarPens.Clone();
                fa.BarBrushes = new Brush[] { BarBrushes[0].GetBrush(), BarBrushes[1].GetBrush(), BarBrushes[2].GetBrush() };
                fa.NameBrush = NameBrush.GetBrush();
                fa.NameFont = (Font)NameFont.Clone();
                fa.TextFont = (Font)TextFont.Clone();
                fa.StockRenderType = StockRenderType;
                if (fa.IsMain())
                    fa.AxisY.Scale = scaleType;
            }
            fc.StickRenderType = StickRenderType;
            fc.CursorPen = CursorPen.GetPen();
            fc.ShowXAxisInLastArea = ShowXAxisInLastArea;
            fc.ShowValueLabel = ShowValueLabel;
            fc.AllXFormats = AllXFormats;
        }

        public static void InitSerializer()
        {
            if (xs == null)
                xs = new XmlSerializer(typeof(FormulaSkin));
        }

        public static FormulaSkin GetSkinByName(string SkinName)
        {
            FormulaSkin fs = (FormulaSkin)htSkins[SkinName];
            if (fs == null)
            {
                Type t = typeof(FormulaSkin);
                try
                {
                    fs = (FormulaSkin)t.InvokeMember(SkinName, BindingFlags.Static | BindingFlags.Public | BindingFlags.GetProperty, null, null, null);
                }
                catch
                {
                    fs = null;
                }
                if (fs == null)
                {
                    string s = FormulaHelper.SkinRoot + SkinName;
                    if (File.Exists(s))
                    {
                        InitSerializer();
                        try
                        {
                            fs = (FormulaSkin)xs.Deserialize(File.OpenRead(s));
                        }
                        catch
                        {
                            fs = null;
                        }
                    }
                }
                if (fs != null)
                    htSkins[SkinName] = fs;
            }
            return fs;
        }

        public Stream GetStream()
        {
            InitSerializer();
            MemoryStream ms = new MemoryStream();
            xs.Serialize(ms, this);
            return ms;
        }

        public void SaveToStream(Stream s)
        {
            InitSerializer();
            xs.Serialize(s, this);
        }

        public void Save()
        {
            using (FileStream fs = File.Create(GetSkinFile()))
                SaveToStream(fs);
        }

        public static FormulaSkin LoadFromStream(Stream s)
        {
            InitSerializer();
            return (FormulaSkin)xs.Deserialize(s);
        }

        public FormulaSkin Clone()
        {
            Stream s = GetStream();
            s.Position = 0;
            return LoadFromStream(s);
        }

        public static string[] GetCustomSkins()
        {
            string[] ss = Directory.GetFiles(FormulaHelper.SkinRoot);
            for (int i = 0; i < ss.Length; i++)
                ss[i] = Path.GetFileName(ss[i]);
            return ss;
        }

        private static string[] buildInSkins;
        public static string[] GetBuildInSkins()
        {
            if (buildInSkins == null)
            {
                Type t = typeof(FormulaSkin);
                PropertyInfo[] pis = t.GetProperties(BindingFlags.Static | BindingFlags.Public);
                buildInSkins = new string[pis.Length];
                for (int i = 0; i < pis.Length; i++)
                    buildInSkins[i] = pis[i].Name;
            }
            return buildInSkins;
        }

        public static bool CheckSkinName(string SkinName)
        {
            string[] ss = GetBuildInSkins();
            foreach (string s in ss)
                if (string.Compare(s, SkinName, true) == 0)
                    return false;
            ss = GetCustomSkins();
            foreach (string s in ss)
                if (string.Compare(s, SkinName, true) == 0)
                    return false;
            return true;
        }

        [XmlIgnore]
        public static FormulaSkin BlackBlue //Yahoo style
        {
            get
            {
                FormulaSkin fs = new FormulaSkin("BlackBlue");
                fs.AxisX.Back.BackGround = new BrushMapper(Color.FromArgb(224, 224, 224));// new SolidBrush(Color.FromArgb(224,224,224));
                fs.AxisX.AxisLabelAlign = AxisLabelAlign.TickCenter;
                fs.AxisX.DataCycle = new DataCycle(DataCycleBase.DAY, 20);
                fs.AllXFormats = XFormatCollection.Default;
                //fs.AxisX.DateFormatProvider = DateTimeFormatInfo.InvariantInfo;
                fs.BarPens = new PenMapper[] { new PenMapper(Color.Black), new PenMapper(Color.Black), new PenMapper(Color.Blue) };
                //fs.BarBrushes = new Brush[]{null,null,Brushes.Blue};

                fs.AxisY.AxisPos = AxisPos.Left;
                fs.CursorPen = new PenMapper(Color.Black);
                return fs;
            }
        }

        [XmlIgnore]
        public static FormulaSkin PinkBlue //www.Investor.com
        {
            get
            {
                FormulaSkin fs = new FormulaSkin("PinkBlue");

                fs.AxisX.Back.BackGround = new BrushMapper(Color.White);// Brushes.White;
                fs.AxisX.DataCycle = new DataCycle(DataCycleBase.WEEK, 2);
                //fs.AxisX.DateFormatProvider = DateTimeFormatInfo.InvariantInfo;
                fs.AxisX.Format = "%d";
                fs.AxisX.AxisLabelAlign = AxisLabelAlign.TickRight;

                fs.AxisX.MajorTick.FullTick = true;
                fs.AxisX.MajorTick.Inside = false;
                fs.AxisX.MajorTick.LinePen.DashPattern = new float[] { 1, 3 };
                fs.AxisX.MajorTick.LinePen.Color = Color.Black;
                fs.AxisX.MajorTick.TickPen.Color = Color.Black;

                fs.AxisX.MinorTick.Visible = false;
                fs.AxisX.MinorTick.DataCycle = DataCycle.Week;
                fs.AxisX.MinorTick.LinePen = (PenMapper)fs.AxisX.MajorTick.LinePen.Clone();
                fs.AxisX.MinorTick.ShowText = false;
                fs.AxisX.MinorTick.ShowLine = true;

                FormulaAxisX fax = new FormulaAxisX();
                fax.CopyFrom(fs.AxisX);
                fax.DataCycle = DataCycle.Quarter;
                fax.Format = "MMMM";
                fax.MajorTick.LinePen = new PenMapper(Color.Black);
                fax.MajorTick.ShowText = false;
                fax.MajorTick.TickPen.Color = Color.Black;

                fax.MinorTick.FullTick = true;
                fax.MinorTick.Inside = false;
                fax.MinorTick.ShowText = true;
                fax.MinorTick.ShowTick = true;
                fax.MinorTick.ShowLine = false;
                fax.MinorTick.DataCycle = DataCycle.Month;
                fax.MinorTick.Format = "{yy}MMMM";
                //fax.MinorTick.DateFormatProvider = DateTimeFormatInfo.InvariantInfo;
                fax.MinorTick.TickPen.Color = Color.Black;

                fs.AxisXs.Add(fax);
                fs.AxisY.MajorTick.LinePen.DashPattern = new float[] { 1, 3 };
                fs.AxisY.MajorTick.LinePen.Color = Color.Black;

                fs.AllXFormats = XFormatCollection.TwoAxisX;

                fs.BarPens = new PenMapper[] { new PenMapper(Color.Blue, 2), new PenMapper(Color.Black, 2), new PenMapper(Color.DeepPink, 2) };
                fs.BarBrushes = new BrushMapper[] { BrushMapper.Empty, BrushMapper.Empty, BrushMapper.Empty };

                fs.CursorPen = new PenMapper(Color.Black);
                fs.ShowXAxisInLastArea = true;
                fs.StockRenderType = StockRenderType.HLCBars;
                return fs;
            }
        }

        [XmlIgnore]
        public static FormulaSkin TraderdataEOD //traderdata
        {
            get
            {
                FormulaSkin fs = new FormulaSkin("TraderdataEOD");


                //Axis Y
                fs.AxisY.Back.BackGround = new BrushMapper(Color.DimGray);
                fs.AxisY.LabelBrush = new BrushMapper(Color.White);
                fs.AxisY.MajorTick.ShowLine = true;
                fs.AxisY.MajorTick.LinePen = new PenMapper(Color.FromArgb(30, 23, 23), 1);
                fs.AxisY.MinorTick.ShowLine = false;


                //Aix X
                //FormulaAxisX axisBase = new FormulaAxisX();
                fs.AxisX.Back.BackGround = new BrushMapper(Color.DimGray);// Brushes.White;
                fs.AxisX.LabelBrush = new BrushMapper(Color.White);
                fs.AxisX.DataCycle = new DataCycle(DataCycleBase.MONTH, 1);
                fs.AxisX.Format = "dd-MM-yyyy";
                fs.AxisX.AxisLabelAlign = AxisLabelAlign.TickRight;
                fs.AllXFormats = XFormatCollection.Default;

                fs.AxisX.MajorTick.ShowLine = true;
                fs.AxisX.MajorTick.DataCycle = DataCycle.Month;
                fs.AxisX.MajorTick.LinePen = new PenMapper(Color.FromArgb(30, 23, 23), 1);
                fs.AxisX.MinorTick.ShowLine = false;
                //fs.AxisX.MajorTick.LinePen.DashPattern = new float[] { 1, 2 };
                fs.AxisX.MajorTick.FullTick = false;
                fs.AxisX.MajorTick.Inside = false;


                //setando a cor dos candles
                fs.BarPens = new PenMapper[]{	new PenMapper(Color.GreenYellow, 1), 
												new PenMapper(Color.GreenYellow, 1),
												new PenMapper(Color.Red, 1)};

                fs.BarBrushes = new BrushMapper[] { BrushMapper.Empty, BrushMapper.Empty, new BrushMapper(Color.Red) };
                fs.StockRenderType = StockRenderType.Candle;
                fs.CursorPen = new PenMapper(Color.Yellow);
                fs.Back.BackGround = new BrushMapper(Color.Black);
                fs.Back.FrameColor = Color.White;
                fs.ShowXAxisInLastArea = true;


                return fs;
            }
        }

        [XmlIgnore]
        public static FormulaSkin TraderdataIntraday //traderdata
        {
            get
            {
                FormulaSkin fs = new FormulaSkin("TraderdataIntraday");


                //Axis Y
                fs.AxisY.Back.BackGround = new BrushMapper(Color.DimGray);
                fs.AxisY.LabelBrush = new BrushMapper(Color.White);
                fs.AxisY.MajorTick.ShowLine = true;
                fs.AxisY.MajorTick.LinePen = new PenMapper(Color.FromArgb(30, 23, 23), 1);
                fs.AxisY.MinorTick.ShowLine = false;


                //Aix X
                //FormulaAxisX axisBase = new FormulaAxisX();
                fs.AxisX.Back.BackGround = new BrushMapper(Color.DimGray);// Brushes.White;
                fs.AxisX.LabelBrush = new BrushMapper(Color.White);
                fs.AxisX.DataCycle = new DataCycle(DataCycleBase.HOUR, 1);
                fs.AxisX.Format = "dd-MM HH:MM";
                fs.AxisX.AxisLabelAlign = AxisLabelAlign.TickRight;

                fs.AxisX.MajorTick.ShowLine = true;
                fs.AxisX.MajorTick.DataCycle = DataCycle.Hour;
                fs.AxisX.MajorTick.LinePen = new PenMapper(Color.FromArgb(30, 23, 23), 1);
                fs.AxisX.MinorTick.ShowLine = false;
                //fs.AxisX.MajorTick.LinePen.DashPattern = new float[] { 1, 2 };
                fs.AxisX.MajorTick.FullTick = false;
                fs.AxisX.MajorTick.Inside = false;


                //setando a cor dos candles
                fs.BarPens = new PenMapper[]{	new PenMapper(Color.GreenYellow, 1), 
												new PenMapper(Color.GreenYellow, 1),
												new PenMapper(Color.Red, 1)};

                fs.BarBrushes = new BrushMapper[] { BrushMapper.Empty, BrushMapper.Empty, new BrushMapper(Color.Red) };
                fs.StockRenderType = StockRenderType.Candle;
                fs.CursorPen = new PenMapper(Color.Yellow);
                fs.Back.BackGround = new BrushMapper(Color.Black);
                fs.Back.FrameColor = Color.White;
                fs.ShowXAxisInLastArea = true;


                return fs;
            }
        }

        [XmlIgnore]
        public static FormulaSkin GreenRed
        {
            get
            {
                FormulaSkin fs = new FormulaSkin("GreenRed");

                fs.AxisX.Back.BackGround = new BrushMapper(Color.FromArgb(224, 224, 224));// new SolidBrush(Color.FromArgb(224,224,224));
                fs.AxisX.AxisLabelAlign = AxisLabelAlign.TickCenter;
                fs.AxisX.DataCycle = new DataCycle(DataCycleBase.DAY, 20);
                //fs.AxisX.DateFormatProvider = DateTimeFormatInfo.InvariantInfo;
                fs.AllXFormats = XFormatCollection.Default;

                fs.BarPens = new PenMapper[]{
												new PenMapper(Color.FromArgb(0,96,0)), 
												new PenMapper(Color.Black),
												new PenMapper(Color.FromArgb(96,0,0)),};
                fs.BarBrushes = new BrushMapper[]{
													 new BrushMapper(Color.FromArgb(0x88,0xff,0x88)),
													 new BrushMapper(Color.White),
													 new BrushMapper(Color.Red),
											   };

                fs.CursorPen = new PenMapper(Color.Black);
                return fs;
            }
        }

        [XmlIgnore]
        public static FormulaSkin Traderdata
        {
            get
            {
                FormulaSkin fs = new FormulaSkin("Traderdata");

                fs.Colors = new Color[] { Color.Blue, Color.Fuchsia, Color.DarkGray, Color.Maroon, Color.DarkGreen, Color.DarkBlue, Color.Olive };

                fs.AxisY.Back.BackGround = new BrushMapper(Color.DimGray);
                fs.AxisY.LabelBrush = new BrushMapper(Color.White);
                fs.AxisY.MajorTick.ShowLine = true;
                fs.AxisY.MajorTick.LinePen = new PenMapper(Color.FromArgb(30, 23, 23), 1);
                fs.AxisY.MinorTick.ShowLine = false;


                fs.AxisX.AxisLabelAlign = AxisLabelAlign.TickCenter;
                fs.AxisX.DataCycle = new DataCycle(DataCycleBase.DAY, 20);

                fs.AxisX.Back.BackGround = new BrushMapper(Color.DimGray);//FromArgb(224, 224, 224));// new SolidBrush(Color.FromArgb(224,224,224));
                fs.AxisX.LabelBrush = new BrushMapper(Color.White);

                fs.AxisX.MajorTick.ShowLine = true;
                fs.AxisX.MajorTick.LinePen = new PenMapper(Color.FromArgb(30, 23, 23), 1);
                fs.AxisX.MinorTick.ShowLine = false;
                fs.AllXFormats = XFormatCollection.Default;

                //setando a cor dos candles
                fs.BarPens = new PenMapper[]{	new PenMapper(Color.GreenYellow, 1), 
												new PenMapper(Color.GreenYellow, 1),
												new PenMapper(Color.Red, 1)};

                fs.BarBrushes = new BrushMapper[] { BrushMapper.Empty, BrushMapper.Empty, new BrushMapper(Color.Red) };
                fs.StockRenderType = StockRenderType.Candle;
                fs.CursorPen = new PenMapper(Color.Yellow);
                fs.Back.BackGround = new BrushMapper(Color.Black);
                fs.Back.FrameColor = Color.White;
                fs.ShowXAxisInLastArea = true;
                return fs;
            }
        }

        [XmlIgnore]
        public static FormulaSkin BlackWhite
        {
            get
            {
                FormulaSkin fs = new FormulaSkin("BlackWhite");
                fs.Back.BackGround = new BrushMapper(Color.Black);// Brushes.Black;
                fs.Back.FrameColor = Color.White;
                fs.Colors = new Color[] { Color.White, Color.Yellow, Color.LightBlue, Color.LightCoral, Color.LightGray, Color.LightPink, Color.Olive, Color.LightPink, Color.Aqua };

                fs.BarPens = new PenMapper[] { new PenMapper(Color.DeepPink), new PenMapper(Color.White), new PenMapper(Color.Aqua) };
                fs.BarBrushes = new BrushMapper[] { BrushMapper.Empty, BrushMapper.Empty, new BrushMapper(Color.Aqua) };
                fs.NameBrush = new BrushMapper(Color.Yellow);// Brushes.Yellow;

                //fs.AxisX.Format = "yyMMM";
                //fs.AxisX.DateFormatProvider = DateTimeFormatInfo.InvariantInfo;
                fs.AxisX.Back.BackGround = new BrushMapper(Color.Black);// Brushes.Black;
                fs.AxisX.Back.FrameColor = Color.White;
                fs.AxisX.LabelBrush = new BrushMapper(Color.White);
                fs.AxisX.LabelFont = new Font("Verdana", 7);
                fs.AllXFormats = XFormatCollection.Default;

                fs.AxisY.LabelBrush = new BrushMapper(Color.Black);
                fs.AxisY.Back = (FormulaBack)fs.AxisX.Back.Clone();
                fs.AxisY.MultiplyBack.BackGround = new BrushMapper(Color.Black); //Brushes.Black;
                fs.AxisY.MultiplyBack.FrameColor = Color.Yellow;
                fs.AxisY.LabelFont = new Font("Verdana", 7);

                fs.CursorPen = new PenMapper(Color.White);
                return fs;
            }
        }

        [XmlIgnore]
        public static FormulaSkin GreenWhite
        {
            get
            {
                FormulaSkin fs = new FormulaSkin("GreenWhite");
                fs.Colors = new Color[] { Color.Blue, Color.Fuchsia, Color.DarkGray, Color.Maroon, Color.DarkGreen, Color.DarkBlue, Color.Olive };
                fs.BarPens = new PenMapper[] { new PenMapper(Color.Red), new PenMapper(Color.Green), new PenMapper(Color.Green) };
                fs.BarBrushes = new BrushMapper[]{
													 BrushMapper.Empty,//new BrushMapper(Color.White),
													 BrushMapper.Empty,
													 new BrushMapper(Color.Green)};
                fs.NameBrush = new BrushMapper(Color.Black); //Brushes.Black;

                //fs.AxisX.Format = "yyMMM";
                //fs.AxisX.DateFormatProvider = DateTimeFormatInfo.InvariantInfo;
                fs.AllXFormats = XFormatCollection.Default;

                return fs;
            }
        }

        [XmlIgnore]
        public static FormulaSkin CyanGreen
        {
            get
            {
                FormulaSkin fs = new FormulaSkin("CyanGreen");
                fs.Colors = new Color[] { Color.Blue, Color.Fuchsia, Color.DarkGray, Color.Maroon, Color.DarkGreen, Color.Cyan, Color.Olive };
                fs.BarPens = new PenMapper[] { new PenMapper(Color.Fuchsia), new PenMapper(Color.White), new PenMapper(Color.Green) };
                fs.BarBrushes = new BrushMapper[]{
													 BrushMapper.Empty,
													 BrushMapper.Empty,
													 new BrushMapper(Color.Green),};
                fs.NameBrush = new BrushMapper(Color.Black);

                fs.Back.BackGround = new BrushMapper(Color.FromArgb(0xc0, 0xe0, 0xc0));// new SolidBrush(Color.FromArgb(0xc0,0xe0,0xc0));

                //fs.AxisX.Format = "yyMMM";
                //fs.AxisX.DateFormatProvider = DateTimeFormatInfo.InvariantInfo;
                fs.AxisX.Back.BackGround = new BrushMapper(Color.FromArgb(0xd0, 0xe0, 0xd0));//new SolidBrush(Color.FromArgb(0xd0,0xe0,0xd0));
                fs.AllXFormats = XFormatCollection.Default;

                fs.AxisY.Back.BackGround = new BrushMapper(Color.FromArgb(0xd0, 0xe0, 0xd0));//new SolidBrush(Color.FromArgb(0xd0,0xe0,0xd0));
                return fs;
            }
        }

        [XmlIgnore]
        public static FormulaSkin OceanBlue
        {
            get
            {
                FormulaSkin fs = new FormulaSkin("OceanBlue");
                fs.Back.BackGround = new BrushMapper(Color.Navy);// Brushes.Navy;
                fs.Back.FrameColor = Color.Yellow;
                fs.Colors = new Color[] { Color.White, Color.Yellow, Color.Fuchsia, Color.Green, Color.LightGray, Color.Blue, Color.Olive, Color.Purple, Color.Aqua };

                fs.BarPens = new PenMapper[] { new PenMapper(Color.Red), new PenMapper(Color.White), new PenMapper(Color.Aqua) };
                fs.BarBrushes = new BrushMapper[]{
													 BrushMapper.Empty,
													 BrushMapper.Empty,
													 new BrushMapper(Color.Aqua)
										   };
                fs.NameBrush = new BrushMapper(Color.Yellow); //Brushes.Yellow;

                //fs.AxisX.Format = "yyMMM";
                //fs.AxisX.DateFormatProvider = DateTimeFormatInfo.InvariantInfo;
                fs.AxisX.Back.BackGround = new BrushMapper(Color.Blue);// Brushes.Blue;
                fs.AxisX.Back.FrameColor = Color.Yellow;
                fs.AxisX.LabelBrush = new BrushMapper(Color.White);
                fs.AllXFormats = XFormatCollection.Default;

                fs.AxisY.LabelBrush = new BrushMapper(Color.White);
                fs.AxisY.Back = (FormulaBack)fs.AxisX.Back.Clone();
                fs.AxisY.MultiplyBack.BackGround = new BrushMapper(Color.Black);// Brushes.Black;
                fs.AxisY.MultiplyBack.FrameColor = Color.Yellow;

                fs.CursorPen = new PenMapper(Color.Yellow);
                return fs;
            }
        }

        [XmlIgnore]
        public static FormulaSkin RedBlack
        {
            get
            {
                FormulaSkin fs = new FormulaSkin("RedBlack");
                fs.Back.BackGround = new BrushMapper(Color.Ivory);// Brushes.Ivory;
                fs.Back.FrameWidth = 1;

                fs.Colors = new Color[] { Color.Black, Color.Blue, Color.Red, Color.Fuchsia, Color.DarkGray, Color.Maroon, Color.DarkGreen, Color.Plum, Color.Olive };
                fs.BarPens = new PenMapper[] { new PenMapper(Color.Black), new PenMapper(Color.Black), new PenMapper(Color.Firebrick) };
                fs.BarBrushes = new BrushMapper[]{
													 BrushMapper.Empty,
													 BrushMapper.Empty,
													 new BrushMapper(Color.Firebrick)};
                fs.NameBrush = new BrushMapper(Color.Black);//Brushes.Black;

                //fs.AxisX.Format = "yyMMM";
                //fs.AxisX.DateFormatProvider = DateTimeFormatInfo.InvariantInfo;
                fs.AxisX.Back.BackGround = new BrushMapper(Color.WhiteSmoke);// Brushes.WhiteSmoke;
                fs.AxisX.Back.LeftPen.Width = 1;
                fs.AxisX.MajorTick.LinePen = new PenMapper(Color.DarkKhaki);
                fs.AllXFormats = XFormatCollection.Default;

                fs.AxisY.Back.BackGround = new BrushMapper(Color.WhiteSmoke);// Brushes.WhiteSmoke;
                fs.AxisY.MultiplyBack.BackGround = new BrushMapper(Color.BlanchedAlmond);// Brushes.BlanchedAlmond;
                fs.AxisY.MajorTick.LinePen = new PenMapper(Color.DarkKhaki);

                fs.AxisY.Back.RightPen.Width = 1;
                fs.AxisY.Back.TopPen.Width = 1;

                fs.CursorPen = new PenMapper(Color.Black);
                return fs;
            }
        }

        [XmlIgnore]
        public static FormulaSkin RedWhite
        {
            get
            {
                FormulaSkin fs = new FormulaSkin("RedWhite");
                fs.Back.BackGround = new BrushMapper(Color.WhiteSmoke);// Brushes.WhiteSmoke;
                fs.Back.FrameWidth = 2;

                fs.Colors = new Color[] { Color.Black, Color.Blue, Color.Red, Color.Fuchsia, Color.DarkGray, Color.Maroon, Color.DarkGreen, Color.Plum, Color.Olive };
                fs.BarPens = new PenMapper[] { new PenMapper(Color.Black), new PenMapper(Color.Black), new PenMapper(Color.Maroon) };
                fs.BarBrushes = new BrushMapper[]{
													 BrushMapper.Empty,
													 BrushMapper.Empty,
													 new BrushMapper(Color.Maroon)};
                fs.NameBrush = new BrushMapper(Color.Black);//Brushes.Black;

                //fs.AxisX.Format = "yyMMM";
                //fs.AxisX.DateFormatProvider = DateTimeFormatInfo.InvariantInfo;
                fs.AxisX.Back.BackGround = new BrushMapper(Color.Azure);// Brushes.Azure;
                fs.AxisX.Back.LeftPen.Width = 2;
                fs.AllXFormats = XFormatCollection.Default;

                fs.AxisY.Back.BackGround = new BrushMapper(Color.AliceBlue);// Brushes.AliceBlue;
                fs.AxisY.MultiplyBack.BackGround = new BrushMapper(Color.BlanchedAlmond);// Brushes.BlanchedAlmond;

                fs.AxisY.Back.RightPen.Width = 2;
                fs.AxisY.Back.TopPen.Width = 2;

                fs.CursorPen = new PenMapper(Color.Black);
                return fs;
            }
        }

        [XmlIgnore]
        public static FormulaSkin Temp
        {
            get
            {
                FormulaSkin fs = new FormulaSkin("Temp");
                fs.Back.BackGround = new BrushMapper(Color.FromArgb(0x1f, 0x73, 0xb4));// Brushes.WhiteSmoke;
                fs.back.FrameColor = Color.White;

                fs.Colors = new Color[] { Color.White, Color.Yellow, Color.Fuchsia, Color.Green, Color.LightGray, Color.Blue, Color.Olive, Color.Purple, Color.Aqua };

                fs.AxisX.Back.BackGround = new BrushMapper(Color.FromArgb(0x1f, 0x73, 0xb4));// new SolidBrush(Color.FromArgb(224,224,224));
                fs.AxisX.Back.FrameColor = Color.White;
                fs.AxisX.LabelBrush = new BrushMapper(Color.White);
                fs.AxisX.AxisLabelAlign = AxisLabelAlign.TickCenter;

                fs.AxisX.DataCycle = new DataCycle(DataCycleBase.DAY, 20);
                //fs.AxisX.DateFormatProvider = DateTimeFormatInfo.InvariantInfo;

                fs.AxisY.Back.BackGround = new BrushMapper(Color.FromArgb(0x1f, 0x73, 0xb4));
                fs.AxisY.LabelBrush = new BrushMapper(Color.White);
                fs.AxisY.Back.FrameColor = Color.White;
                fs.AxisY.MultiplyBack.BackGround = new BrushMapper(Color.Black); //Brushes.Black;
                fs.NameBrush = new BrushMapper(Color.Yellow);
                fs.AllXFormats = XFormatCollection.Default;

                fs.BarPens = new PenMapper[] { new PenMapper(Color.White), new PenMapper(Color.White), new PenMapper(Color.White), };
                fs.BarBrushes = new BrushMapper[]{
													 new BrushMapper(Color.FromArgb(0x88,0xff,0x88)),
													 new BrushMapper(Color.White),
													 new BrushMapper(Color.Red),
				};

                fs.CursorPen = new PenMapper(Color.Black);
                return fs;

            }
        }

        [XmlIgnore]
        public static FormulaSkin White
        {
            get
            {
                FormulaSkin fs = new FormulaSkin("White");
                fs.Back.BackGround = new BrushMapper(Color.FromArgb(243, 246, 251));// Brushes.WhiteSmoke;
                fs.Back.FrameWidth = 1;

                fs.Colors = new Color[] { Color.Black, Color.Blue, Color.Red, Color.Fuchsia, Color.DarkGray, Color.Maroon, Color.DarkGreen, Color.Plum, Color.Olive };
                fs.BarPens = new PenMapper[] { new PenMapper(Color.Black), new PenMapper(Color.Black), new PenMapper(Color.Maroon) };
                fs.BarBrushes = new BrushMapper[]{
													 BrushMapper.Empty,
													 BrushMapper.Empty,
													 new BrushMapper(Color.Maroon)};
                fs.NameBrush = new BrushMapper(Color.Black);//Brushes.Black;

                //fs.AxisX.Format = "yyMMM";
                //fs.AxisX.DateFormatProvider = DateTimeFormatInfo.InvariantInfo;
                fs.AxisX.Back.BackGround = new BrushMapper(Color.FromArgb(230, 234, 243));
                fs.AxisX.Back.LeftPen.Width = 1;
                fs.AxisX.LabelFont = new Font("Verdana", 7);
                fs.AxisX.Height = 14;
                fs.AxisX.AxisLabelAlign = AxisLabelAlign.TickCenter;

                fs.AllXFormats = XFormatCollection.Default;

                fs.AxisY.Back.BackGround = new BrushMapper(Color.FromArgb(230, 234, 243));
                fs.AxisY.MultiplyBack.BackGround = new BrushMapper(Color.BlanchedAlmond);
                fs.AxisY.LabelFont = new Font("Verdana", 7);
                fs.AxisY.MajorTick.TickWidth = 2;
                fs.AxisY.MinorTick.TickWidth = 1;

                fs.AxisY.Back.RightPen.Width = 1;
                fs.AxisY.Back.TopPen.Width = 1;

                fs.CursorPen = new PenMapper(Color.Black);
                return fs;
            }
        }
    }

    public class SkinCollectionEditor : CollectionEditor
    {
        public SkinCollectionEditor(Type type)
            : base(type)
        {
        }

        protected override System.ComponentModel.Design.CollectionEditor.CollectionForm CreateCollectionForm()
        {
            CollectionForm cf = base.CreateCollectionForm();
            foreach (Control c in cf.Controls)
            {
                if (c is PropertyGrid)
                {
                    PropertyGrid pg = (PropertyGrid)c;
                    pg.PropertyValueChanged += new PropertyValueChangedEventHandler(SkinCollectionEditor_PropertyValueChanged);
                    pg.SelectedObjectsChanged += new EventHandler(pg_SelectedObjectsChanged);
                }
            }
            return cf;
        }

        private void NotifyFormulaSkin()
        {
            if (Context != null && Context.Instance is FormulaSkin)
            {
                FormulaSkin fs = (FormulaSkin)Context.Instance;
                fs.RefreshCollectionValue();
            }
        }

        private void SkinCollectionEditor_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            NotifyFormulaSkin();
        }

        private void pg_SelectedObjectsChanged(object sender, EventArgs e)
        {
            NotifyFormulaSkin();
        }
    }
}