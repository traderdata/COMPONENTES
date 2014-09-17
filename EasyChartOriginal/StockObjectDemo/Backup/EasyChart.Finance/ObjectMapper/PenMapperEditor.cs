using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Design;

namespace Easychart.Finance
{
	/// <summary>
	/// Summary description for ObjectPenEditor.
	/// </summary>
	public class PenMapperEditor : UITypeEditor
	{
		public PenMapperEditor()
		{
		}

		public override void PaintValue(PaintValueEventArgs e)
		{
			PenMapper op = (PenMapper)e.Value;
			Rectangle R = e.Bounds;
			Region OldClip = e.Graphics.Clip;
			e.Graphics.SetClip(R);
			e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
			e.Graphics.DrawLine(op.GetPen(),e.Bounds.X,e.Bounds.Y,e.Bounds.Right-1,e.Bounds.Bottom-1);
			e.Graphics.SmoothingMode = SmoothingMode.Default;
			e.Graphics.Clip = OldClip;
			base.PaintValue (e);
		}

//		public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
//		{
//			return UITypeEditorEditStyle.DropDown;
//			//return base.GetEditStyle (context);
//		}

		public override bool GetPaintValueSupported(System.ComponentModel.ITypeDescriptorContext context)
		{
			return true;
		}
	}
}
