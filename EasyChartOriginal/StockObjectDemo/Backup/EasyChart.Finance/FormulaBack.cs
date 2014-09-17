using System;
using System.Drawing;
using System.ComponentModel;

namespace Easychart.Finance
{
	/// <summary>
	/// Stock Formula background
	/// </summary>
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class FormulaBack:ICloneable // DeepClone
	{
		private BrushMapper backGround;
		private PenMapper leftPen;
		private PenMapper topPen;
		private PenMapper rightPen;
		private PenMapper bottomPen;

		/// <summary>
		/// Background brush
		/// </summary>
		public BrushMapper BackGround
		{
			get 
			{
				return backGround;
			}
			set 
			{
				backGround = value;
			}
		}

		public bool ShouldSerializeLeftPen()
		{
			return PenMapper.NotDefault(LeftPen);
		}

		/// <summary>
		/// Frame left border pen
		/// </summary>
		public PenMapper LeftPen
		{
			get 
			{
				return leftPen;
			}
			set 
			{
				leftPen = value;
			}
		}

		public bool ShouldSerializeTopPen()
		{
			return PenMapper.NotDefault(TopPen);
		}

		/// <summary>
		/// Frame top border pen
		/// </summary>
		public PenMapper TopPen
		{
			get 
			{
				return topPen;
			}
			set 
			{
				topPen = value;
			}
		}

		public bool ShouldSerializeRightPen()
		{
			return PenMapper.NotDefault(RightPen);
		}

		/// <summary>
		/// Frame right border pen
		/// </summary>
		public PenMapper RightPen
		{
			get 
			{
				return rightPen;
			}
			set 
			{
				rightPen = value;
			}
		}

		public bool ShouldSerializeBottomPen()
		{
			return PenMapper.NotDefault(BottomPen);
		}

		/// <summary>
		/// Frame bottom border pen
		/// </summary>
		public PenMapper BottomPen
		{
			get 
			{
				return bottomPen;
			}
			set 
			{
				bottomPen = value;
			}
		}

		/// <summary>
		/// Set frame width
		/// </summary>
		public int FrameWidth
		{
			set 
			{
				LeftPen.Width = value;
				TopPen.Width = value;
				RightPen.Width = value;
				BottomPen.Width = value;
			}
		}

		/// <summary>
		/// Set frame color
		/// </summary>
		public Color FrameColor
		{
			set 
			{
				LeftPen.Color = value;
				TopPen.Color = value;
				RightPen.Color = value;
				BottomPen.Color = value;
			}
		}

		/// <summary>
		/// Constructor of FormulaBack
		/// </summary>
		public FormulaBack()
		{
			BackGround = new BrushMapper(Color.White);// new SolidBrush(Color.White);
			LeftPen = new PenMapper(Color.Black);// Pen(Color.Black,1);
			TopPen = new PenMapper(Color.Black);// Pen(Color.Black,1);
			RightPen = new PenMapper(Color.Black);// Pen(Color.Black,1);
			BottomPen = new PenMapper(Color.Black);// Pen(Color.Black,1);
		}

		/// <summary>
		/// A Clone of FormulaBack
		/// </summary>
		/// <returns></returns>
		public object Clone() 
		{
			FormulaBack fb = new FormulaBack();
			fb.BackGround = BackGround.Clone();
			fb.LeftPen = LeftPen.Clone();
			fb.RightPen = RightPen.Clone();
			fb.BottomPen = BottomPen.Clone();
			fb.TopPen = TopPen.Clone();
			return fb;
		}

		/// <summary>
		/// Render the FormulaBack
		/// </summary>
		/// <param name="g">Graphics to render</param>
		/// <param name="R">Rectangle</param>
		public void Render(Graphics g,Rectangle R) 
		{
			g.FillRectangle(BackGround.GetBrush(),R);
			R.Width--;
			R.Height--;

			int w = (int)(LeftPen.Width-1);
			if (w>=0)
				g.DrawLine(LeftPen.GetPen(), R.Left+w,R.Top,R.Left+w,R.Bottom);

			w = (int)(TopPen.Width-1);
			if (w>=0)
				g.DrawLine(TopPen.GetPen(), R.Left,R.Top+w,R.Right,R.Top+w);

			w = (int)(RightPen.Width-1);
			if (w>=0)
				g.DrawLine(RightPen.GetPen(), R.Right,R.Top,R.Right,R.Bottom);

			w = (int)(BottomPen.Width-1);
			if (w>=0)
				g.DrawLine(BottomPen.GetPen(), R.Left,R.Bottom,R.Right,R.Bottom);
		}

		public override string ToString()
		{
			return backGround.ToString ();
		}
	}
}