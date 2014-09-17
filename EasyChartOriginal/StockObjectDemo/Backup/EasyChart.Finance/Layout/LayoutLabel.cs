using System;
using System.Collections;
using System.Drawing;

namespace Easychart.Finance
{
	/// <summary>
	/// Summary description for LayoutLabel.
	/// </summary>
	public class LayoutLabel
	{
		public LayoutLabel()
		{
		}

		public Rectangle Back;
		public Color BackColor;
		public Rectangle Frame;
		public Color FrameColor;
		public Color TextColor;
		public string Text;
		public string Icon;
		public TextAlign Align;
		public bool UseColor;
		public Point Pos;
		public Font TextFont;
	}

	public class LayoutLabelCollection : CollectionBase
	{
		public LayoutLabelCollection()
		{
		}

		public virtual void Add(LayoutLabel ll)
		{
			this.List.Add(ll);
		}

		public void Remove(LayoutLabel ll) 
		{
			List.Remove(ll);
		}

	}
}
