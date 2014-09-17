using System;
using System.Drawing;

namespace Easychart.Finance.Objects
{
	/// <summary>
	/// Summary description for DraggingObject.
	/// </summary>
	public class ObjectDragging
	{
		public ObjectPoint[] ControlPoints;
		public PointF StartPoint;
		public int ControlPointIndex;
		public BaseObject Object;

		public ObjectDragging(PointF StartPoint,int ControlPointIndex,BaseObject Object)
		{
			this.StartPoint = StartPoint;
			this.ControlPointIndex = ControlPointIndex;
			this.Object = Object;
			this.ControlPoints = (ObjectPoint[])Object.ControlPoints.Clone();
		}
	}
}
