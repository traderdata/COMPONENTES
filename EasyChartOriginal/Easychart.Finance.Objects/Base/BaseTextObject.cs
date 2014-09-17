using System;
using System.Drawing;

namespace Easychart.Finance.Objects
{
	/// <summary>
	/// Summary description for BaseTextObject.
	/// </summary>
	public class BaseTextObject : LineGroupObject
	{
		private ObjectFont objectFont;

		public ObjectFont ObjectFont
		{
			get
			{
				return objectFont;
			}
			set
			{
				objectFont = value;
			}
		}
		
		public BaseTextObject()
		{
			objectFont = new ObjectFont();
		}

		public virtual RectangleF GetTextRect()
		{
			return RectangleF.Empty;
		}

		public override Region GetRegion()
		{
			Region  R = base.GetRegion ();
			R.Union(GetTextRect());
			return R;
		}
	}
}
