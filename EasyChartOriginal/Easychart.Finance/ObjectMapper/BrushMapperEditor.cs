using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Design;

namespace Easychart.Finance
{
	/// <summary>
	/// Object brush editor
	/// </summary>
	public class ObjectBrushEditor: UITypeEditor
	{
		public ObjectBrushEditor()
		{
		}

		public override void PaintValue(PaintValueEventArgs e)
		{
			BrushMapper bm = (BrushMapper)e.Value;

			e.Graphics.FillRectangle(bm.GetBrush(e.Bounds),e.Bounds);
			base.PaintValue (e);
		}

		public override bool GetPaintValueSupported(System.ComponentModel.ITypeDescriptorContext context)
		{
			return true;
		}
	}
}
