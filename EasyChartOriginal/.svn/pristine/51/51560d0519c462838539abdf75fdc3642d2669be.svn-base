using System;
using System.Xml.Serialization;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;

namespace Easychart.Finance
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class FormulaTick //:DeepClone
	{
		public FormulaTick Clone() 
		{
			FormulaTick ft = new FormulaTick();
			ft.count = count;
			ft.dataCycle = dataCycle;
			ft.DateFormatProvider = DateFormatProvider;
			ft.format = format;
			ft.fullTick = fullTick;
			ft.inside = inside;
			ft.linePen = linePen.Clone();
			ft.minimumPixel = minimumPixel;
			ft.showLine = showLine;
			ft.showText = showText;
			ft.showTick = showTick;
			ft.tickPen = tickPen.Clone();
			ft.tickWidth = tickWidth;
			ft.visible = visible;
			return ft;
		}

		private int tickWidth = 5;
		[DefaultValue(5),XmlAttribute]
		public int TickWidth 
		{
			get
			{
				return tickWidth;
			}
			set
			{
				tickWidth = value;
			}
		}

		private bool showTick = true;
		[DefaultValue(true),XmlAttribute]
		public bool ShowTick 
		{
			get
			{
				return showTick;
			}
			set
			{
				showTick = value;
			}
		}

		private PenMapper tickPen;
		public PenMapper TickPen
		{
			get
			{
				return tickPen;
			}
			set
			{
				tickPen = value;
			}
		}

		private bool showLine = true;
		[DefaultValue(true),XmlAttribute]
		public bool ShowLine
		{
			get
			{
				return showLine;
			}
			set
			{
				showLine = value;
			}
		}

		private PenMapper linePen;
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

		private bool fullTick = false;
		[DefaultValue(false),XmlAttribute]
		public bool FullTick 
		{
			get
			{
				return fullTick;
			}
			set
			{
				fullTick = false;
			}
		}

		private bool inside = false;
		[DefaultValue(false),XmlAttribute]
		public bool Inside 
		{
			get
			{
				return inside;
			}
			set
			{
				inside = value;
			}
		}

		private int count = 4;
		[DefaultValue(4),XmlAttribute]
		public int Count 
		{
			get
			{
				return count;
			}
			set
			{
				count = value;
			}
		}

		private int minimumPixel = 0;
		[DefaultValue(0),XmlAttribute]
		public int MinimumPixel
		{
			get
			{
				return minimumPixel;
			}
			set
			{
				minimumPixel = value;
			}
		}

		private bool visible = true;
		[DefaultValue(true),XmlAttribute]
		public bool Visible 
		{
			get
			{
				return visible;
			}
			set
			{
				visible = value;
			}
		}
		/// <summary>
		/// For X-Axis
		/// </summary>
		private DataCycle dataCycle;
		public DataCycle DataCycle
		{
			get
			{
				return dataCycle;
			}
			set
			{
				dataCycle = value;
			}
		}
		
		private bool showText = true;
		[DefaultValue(true),XmlAttribute]
		public bool ShowText 
		{
			get
			{
				return showText;
			}
			set
			{
				showText = value;
			}
		}

		private string format;
		[DefaultValue(""),XmlAttribute]
		public string Format
		{
			get
			{
				return format;
			}
			set
			{
				format = value;
			}
		}

		[XmlIgnore]
		public IFormatProvider DateFormatProvider;

		public FormulaTick() 
		{
			TickPen = new PenMapper(Color.DarkGray);//Color.White);
			LinePen = new PenMapper(Color.DarkGray);//new Pen(Color.Beige,1);
			LinePen.DashStyle = DashStyle.Dot;
		}

		public void DrawXAxisTick(FormulaCanvas Canvas,double[] Date,FormulaData fdDate,PointF[] pfs,FormulaAxisX fax,ExchangeIntraday ei) 
		{
			if (DataCycle==null) return;

			try
			{
				int LastCycle = -1;
				int NowCycle = 0;
				string LastExtDate = "";
				string NowExtDate;
				int LastX = -10000;
				int LastIndex = -1;
				Graphics g = Canvas.CurrentGraph;

				double DateSpan = 0;
				if (Date.Length>0)
					DateSpan = Date[Date.Length-1]-Date[0];

				double[] SeqDate = Date;
				if (ei!=null && ei.NativeCycle) 
				{
					SeqDate = new double[Date.Length];
					for(int i=0; i<SeqDate.Length; i++) 
						SeqDate[i] = (int)Date[i]+ei.OneDayTime(Date[i]);
				}

				ArrayList alTick = new ArrayList();
				Font F = fax.LabelFont;
				Pen LP = LinePen.GetPen();
				Pen TP = TickPen.GetPen();
				for(int i=pfs.Length-1; i>=0; i--)
				{
					int DateIndex = Date.Length-1-Math.Max(0,Canvas.Start)-i;
					double d = Date[DateIndex];
					DateTime D = DateTime.FromOADate(d);
					NowCycle = DataCycle.GetSequence(SeqDate[DateIndex]);

					if (NowCycle!=LastCycle)
					{
						PointF P = pfs[i];
						LastCycle = NowCycle;
						if (ShowLine)
						{
							int B = Canvas.Rect.Bottom;
							if (!fax.Visible)
								B = Canvas.FrameRect.Bottom;
							g.DrawLine(LP,P.X,Canvas.FrameRect.Top,P.X,B);
						}
			
						if (fax.Visible && ShowTick)
						{
							int TickHeight = TickWidth;
							if (FullTick)
								TickHeight = fax.Rect.Height;
							if (Inside)
								TickHeight = -TickHeight;
							g.DrawLine(TP,P.X,fax.Rect.Top,P.X,TickHeight+fax.Rect.Top);
						}
			
						string s = D.ToString(Format,DateFormatProvider);
						int i1 = s.IndexOf('{');
						int i2 = s.IndexOf('}');
						if (i2>i1)
						{
							NowExtDate = s.Substring(i1+1,i2-i1-1);
							if (NowExtDate!=LastExtDate)
							{
								if (NowExtDate.StartsWith("$"))
									s = s.Remove(i2,s.Length-i2).Remove(i1,2);
								else s = s.Remove(i1,1).Remove(i2-1,1);
							}
							else s = s.Substring(0,i1)+s.Substring(i2+1);
							LastExtDate = NowExtDate;
						}
			
						float LabelWidth = g.MeasureString(s,F).Width;
						float LabelX = P.X;
										
						switch (fax.AxisLabelAlign)
						{
							case AxisLabelAlign.TickCenter:
								LabelX -=LabelWidth/2;
								break;
							case AxisLabelAlign.TickLeft:
								LabelX -=LabelWidth;
								break;
						}
						if (LabelX<fax.Rect.Left ) LabelX = fax.Rect.Left;
			
						if (fax.Visible && ShowText /*&& LastX+MinimumPixel<LabelX*/)
						{
							if (ei==null || ei.ShowFirstXLabel || i<pfs.Length-1 || DateSpan>1)
							{
								alTick.Add(new object[]{s,LabelX,LabelWidth,d,LastX+MinimumPixel<LabelX});
								//g.DrawString(s,F,fax.LabelBrush,LabelX,fax.Rect.Top);
								LastX = (int)(LabelX+LabelWidth);
							}
							LastIndex = i;
						}
					} //if (NowCycle!=LastCycle)
				} //for
			
				//Second pass
				for(int i=0; i<alTick.Count; i++)
				{
					object[] os = (object[])alTick[i];
					if (!(bool)os[4])
					{
						if ((int)(double)((object[])alTick[i-1])[3]==(int)(double)((object[])alTick[i])[3])
							alTick.RemoveAt(i);
						else alTick.RemoveAt(i-1);
						i--;
					}
				}

				//Third pass
				Brush lb  =fax.LabelBrush.GetBrush();
				for(int i=0; i<alTick.Count; i++)
				{
					object[] os = (object[])alTick[i];
					g.DrawString((string)os[0],F,lb,(float)os[1],fax.Rect.Top);
				}
			} 
			catch
			{
			}
		}

		public override string ToString()
		{
			return tickWidth+",Inside="+Inside;
		}
	}
}