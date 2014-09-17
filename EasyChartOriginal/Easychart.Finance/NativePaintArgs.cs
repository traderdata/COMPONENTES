using System;
using System.Drawing;

namespace Easychart.Finance
{
	public class NativePaintArgs
	{
		public Graphics Graphics;
		public Rectangle Rect;
		public Bitmap NativeBitmap;
		public Bitmap NewBitmap;

		public NativePaintArgs(Graphics graphics,Rectangle Rect,Bitmap NativeBitmap)
		{
			this.Graphics = graphics;
			this.Rect = Rect;
			this.NativeBitmap = NativeBitmap;
		}
	}

	public delegate void NativePaintHandler(object sender,NativePaintArgs e);
}
