using System;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Web;

namespace Easychart.Finance.Objects
{
	public enum ImageType {FixedSize,Resizable,Rotate}

	/// <summary>
	/// Summary description for ImageObject.
	/// </summary>
	public class ImageObject:BaseObject
	{
		ColorMatrix cm = new ColorMatrix();
		ImageAttributes ia = new ImageAttributes();
		Bitmap B;

		string imageFile;
		//[TypeConverter(typeof(ImageFileConverter))]
		[Editor(typeof(ImageFileEditor), typeof(System.Drawing.Design.UITypeEditor))]
		[XmlAttribute]
		public string ImageFile
		{
			get
			{
				
				return imageFile;
			}
			set
			{
				imageFile = value;
//				string s = Assembly.GetExecutingAssembly().CodeBase;
//				if (s.StartsWith("file:///"))
//					s = s.Substring(8).Replace("/","\\");
//				s = s.Substring(0,s.Length-Path.GetFileName(s).Length)+"Images\\"+value;
				string s = FormulaHelper.GetImageFile(value);
				if (File.Exists(s))
					B = (Bitmap)Bitmap.FromFile(s);
			}
		}

		SnapType snap;
		[DefaultValue(SnapType.None),XmlAttribute]
		public SnapType Snap
		{
			get
			{
				return snap;
			}
			set
			{
				snap = value;
			}
		}

		public void FixedImage()
		{
			ImageType = ImageType.FixedSize;
		}

		public void ResizableImage()
		{
			ImageType = ImageType.Resizable;
		}

		public void RotateImage()
		{
			ImageType = ImageType.Rotate;
		}

		ImageType imageType = ImageType.Resizable;
		[RefreshProperties(RefreshProperties.All)]
		[DefaultValue(ImageType.Resizable),XmlAttribute]
		public ImageType ImageType
		{
			get
			{
				return imageType;
			}
			set
			{
				imageType = value;
				ObjectPoint[] ops = ControlPoints;
				ControlPoints = new ObjectPoint[ControlPointNum];
				for(int i=0; i<ControlPoints.Length; i++)
				{
					if (i<ops.Length)
						ControlPoints[i] = ops[i];
				}

				if (ops.Length<ControlPoints.Length)
				{
					if (ops.Length==1) 
					{
						ControlPoints[1] = ops[0];
						if (ControlPoints.Length==2) 
						{
							ControlPoints[1].X+=2;
							ControlPoints[1].Y-=1;
						} 
						else 
						{
							ControlPoints[2] = ops[0];
							ControlPoints[2].X+=2;
							ControlPoints[2].Y-=1;

							ControlPoints[1].X = ControlPoints[2].X;
							ControlPoints[1].Y = ControlPoints[0].Y;
						}
					} 
					else
					{
						ControlPoints[0] = ops[0];

						ControlPoints[1].X = ops[1].X;
						ControlPoints[1].Y = ops[0].Y;

						ControlPoints[2].X = ops[0].X;
						ControlPoints[2].Y = ops[1].Y;
					}
				}
			}
		}

		public override int ControlPointNum
		{
			get
			{
				if (imageType==ImageType.FixedSize)
					return 1;
				else if (imageType==ImageType.Resizable)
					return 2;
				else return 3;
			}
		}

		public override int InitNum
		{
			get
			{
				return ControlPointNum;
			}
		}


		byte alpha = 160;
		[DefaultValue(160),XmlAttribute]
		public byte Alpha
		{
			get
			{
				return alpha;
			}
			set
			{
				alpha = value;
			}
		}

		public ImageObject()
		{
			ImageFile = "up.gif";
		}

		private RectangleF GetDestRect(int w)
		{
			SetSnapPrice(snap);

			PointF[] pfs = ToPoints(ControlPoints);

			if (pfs.Length==1) 
				pfs[0].X -=B.Width/2;

			ArrayList al = new ArrayList();
			al.AddRange(pfs);

			if (imageType==ImageType.Rotate) 
				al.Add(new PointF(pfs[1].X-pfs[0].X+pfs[2].X,pfs[1].Y-pfs[0].Y+pfs[2].Y));
			else if (imageType==ImageType.FixedSize) 
			{
				if (B!=null) 
					al.Add(new PointF(pfs[0].X+B.Width,pfs[0].Y+B.Height));
			}
			return base.GetMaxRect((PointF[])al.ToArray(typeof(PointF)),w);
		}


		public override RectangleF GetMaxRect()
		{
			return GetDestRect(LinePen.Width+6);
		}


		public override bool InObject(int X, int Y)
		{
			return GetMaxRect().Contains(X,Y);
		}

		public override void Draw(System.Drawing.Graphics g)
		{
			base.Draw (g);
			if (B!=null)
			{
				SetSnapPrice(snap);
				PointF[] pfs = ToPoints(ControlPoints);
				cm.Matrix33 = (float)alpha/255;
				ia.SetColorMatrix(cm);
				Rectangle Src = new Rectangle(0,0,B.Width,B.Height);
				Rectangle Dest = Rectangle.Truncate(GetDestRect(0));

				if (imageType==ImageType.Rotate)	
					g.DrawImage(B,pfs,Src,GraphicsUnit.Pixel,ia);
				else g.DrawImage(B,Dest,0,0,B.Width,B.Height,GraphicsUnit.Pixel,ia);
			}
		}

		public override ObjectInit[] RegObject()
		{
			return
				new ObjectInit[]{
						   new ObjectInit("Fixed Image",typeof(ImageObject),"FixedImage","Image","ImgFix",900),
						   new ObjectInit("Resizable Image",typeof(ImageObject),"ResizableImage","Image","ImgResize"),
						   new ObjectInit("Rotate Image",typeof(ImageObject),"RotateImage","Image","ImgRotate"),
				};
		}
	}
}