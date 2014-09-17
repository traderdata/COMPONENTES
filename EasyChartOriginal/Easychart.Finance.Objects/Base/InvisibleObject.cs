using System;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Drawing;
using Easychart.Finance;
using Easychart.Finance.DataProvider;

namespace Easychart.Finance.Objects
{
	/// <summary>
	/// Summary description for InvisibleObject.
	/// </summary>
	public class InvisibleObject : BaseObject
	{
		public InvisibleObject()
		{
		}

		public override int InitNum
		{
			get
			{
				return 0;
			}
		}

		public override int ControlPointNum
		{
			get
			{
				return 0;
			}
		}


		[Browsable(false),XmlIgnore]
		public override double SnapPercent
		{
			get
			{
				return base.SnapPercent;
			}
			set
			{
				base.SnapPercent = value;
			}
		}

		[Browsable(false)]
		public override PenMapper LinePen
		{
			get
			{
				return base.LinePen;
			}
			set
			{
				base.LinePen = value;
			}
		}

		[Browsable(false),XmlIgnore]
		public override ObjectPoint[] ControlPoints
		{
			get
			{
				return base.ControlPoints;
			}
			set
			{
				base.ControlPoints = value;
			}
		}

		[Browsable(false)]
		public override ObjectSmoothingMode SmoothingMode
		{
			get
			{
				return base.SmoothingMode;
			}
			set
			{
				base.SmoothingMode = value;
			}
		}
	}
}
