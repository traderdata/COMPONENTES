using System;
using System.Drawing;
using System.Drawing.Design;
using System.IO;
using System.Windows.Forms.Design;

namespace Easychart.Finance.Objects
{
	/// <summary>
	/// Summary description for ImageFileEditor.
	/// </summary>
	public class ImageFileEditor : UITypeEditor
	{
		public ImageFileEditor()
		{
		}

		public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
		{
			return UITypeEditorEditStyle.DropDown;
		}

		public override void PaintValue(PaintValueEventArgs e)
		{
			string s = FormulaHelper.ImageRoot+e.Value;
			if (File.Exists(s))
			{
				Bitmap B = (Bitmap)Bitmap.FromFile(s);
				Rectangle R = e.Bounds;
				R.Inflate(-1,-1);
				e.Graphics.DrawImage(B,R,0,0,B.Width,B.Height,GraphicsUnit.Pixel);
			}
			base.PaintValue (e);
		}

		public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			IWindowsFormsEditorService edSvc = 
				(IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));

			if( edSvc != null )
			{
				ImgControl imgControl = new ImgControl((string)value,edSvc);
				edSvc.DropDownControl(imgControl);
				return imgControl.ImgName;
			}
			return base.EditValue (context, provider, value);
		}

		public override bool GetPaintValueSupported(System.ComponentModel.ITypeDescriptorContext context)
		{
			return true;
		}
	}
}
