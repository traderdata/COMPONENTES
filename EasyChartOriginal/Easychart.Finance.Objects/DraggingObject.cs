using System;
using System.Drawing;

namespace Easychart.Finance.Objects
{
	/// <summary>
	/// Summary description for DraggingObject.
	/// </summary>
	public class DraggingObject
	{
		public ObjectPoint[] ControlPoints;
		public PointF StartPoint;
		public int ControlPointIndex;
		public ObjectBase Object;

		public DraggingObject(PointF StartPoint,int ControlPointIndex,ObjectBase Object)
		{
			this.StartPoint = StartPoint;
			this.ControlPointIndex = ControlPointIndex;
			this.Object = Object;
			this.ControlPoints = (ObjectPoint[])Object.ControlPoints.Clone();
		}
	}
}
