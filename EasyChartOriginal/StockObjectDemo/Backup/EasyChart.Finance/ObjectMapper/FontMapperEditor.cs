using System;
using System.Drawing;
using System.Drawing.Text;
using System.Drawing.Drawing2D;
using System.Drawing.Design;

namespace Easychart.Finance
{
	/// <summary>
	/// Summary description for ObjectFontEditor.
	/// </summary>
	public class FontMapperEditor: UITypeEditor
	{
		public FontMapperEditor()
		{
		}

		public override void PaintValue(PaintValueEventArgs e)
		{
			FontMapper fm = (FontMapper)e.Value;
			TextRenderingHint trh = e.Graphics.TextRenderingHint;
			e.Graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
			fm.DrawString("A",e.Graphics,e.Bounds);
			e.Graphics.TextRenderingHint  = trh;
			base.PaintValue (e);
		}

		public override bool GetPaintValueSupported(System.ComponentModel.ITypeDescriptorContext context)
		{
			return true;
		}

	}
}
