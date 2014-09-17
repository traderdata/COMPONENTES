using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Xml;
using System.Windows.Forms;
using System.Reflection;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.IO;
using Easychart.Finance.Win;

namespace Easychart.Finance.Objects
{
	public delegate void ObjectEventHandler(object sender,ObjectBase Object);
	/// <summary>
	/// Summary description for ObjectManager.
	/// </summary>
	[XmlRoot(IsNullable = true,ElementName="Root")]
	[XmlInclude(typeof(ObjectLine)),XmlInclude(typeof(FibonacciLine)),XmlInclude(typeof(ObjectVLine))]
	[XmlInclude(typeof(ObjectCircle)),XmlInclude(typeof(ObjectEllipse)),XmlInclude(typeof(ObjectLabel))]
	[XmlInclude(typeof(FibonacciCircle)),XmlInclude(typeof(MultiArc))]
	[Serializable]
	public class ObjectManager
	{
		private ObjectBase selectedObject;
		private DraggingObject DragObject;
		private ObjectPoint StartPoint = ObjectPoint.Empty; 
		private ObjectPoint EndPoint = ObjectPoint.Empty;
		private Bitmap MemBmp;
		private Bitmap DragMemBmp;
		private ObjectCollection objects = new ObjectCollection();
		private bool SavedShowCrossCursor;
		private bool SavedShowStatistic;
		private int ObjectSteps;
		
		[XmlIgnore]
		public IObjectCanvas Canvas;
		[XmlIgnore]
		public Control Designer;
		[XmlIgnore]
		public Graphics FormGraphics;
		public event ObjectEventHandler AfterSelect;
		public event ObjectEventHandler AfterCreate;
		public event ObjectEventHandler ObjectChanged;
		[XmlIgnore]
		public ObjectInit ObjectType;

		public ObjectCollection Objects
		{
			get
			{
				return objects;
			}
			set
			{
				objects = value;
			}
		}

		public ObjectManager()
		{
		}

		public void SetCanvas(IObjectCanvas Canvas)
		{
			this.Canvas = Canvas;
			Designer = Canvas.DesignerControl;
			if (Designer!=null)
			{
				SetObjectManager();
				Designer.MouseDown +=new System.Windows.Forms.MouseEventHandler(DesignerControl_MouseDown);
				Designer.MouseMove +=new System.Windows.Forms.MouseEventHandler(DesignerControl_MouseMove);
				Designer.MouseUp +=new System.Windows.Forms.MouseEventHandler(DesignerControl_MouseUp);
				
				if (Designer is ChartWinControl)
					(Designer as ChartWinControl).ExtraPaint +=new NativePaintHandler(ObjectManager_ExtraPaint);
				else Designer.Paint +=new System.Windows.Forms.PaintEventHandler(DesignerControl_Paint);

				Designer.SizeChanged +=new EventHandler(DesignerControl_SizeChanged);
				Designer.KeyDown +=new KeyEventHandler(Designer_KeyDown);
			}
		}

		public ObjectManager(IObjectCanvas Canvas):this()
		{
			SetCanvas(Canvas);
		}

		public void DrawObject(Graphics g,ObjectBase ob,bool Selected)
		{
			Region OldClip = null;
			if (ob.Area!=null) 
			{
				OldClip = g.Clip;
				g.SetClip(ob.Area.Canvas.Rect,CombineMode.Intersect);
			}
			ob.Draw(g);
			if (Selected)
				ob.DrawControlPoint(g);
			if (OldClip!=null)
				g.SetClip(OldClip,CombineMode.Replace);
		}

		public void Draw(Graphics g,ObjectBase SelectedObject,ObjectBase MovingObject)
		{
			try
			{
				foreach(ObjectBase ob in objects)
					if (ob!=MovingObject)
						DrawObject(g,ob,ob==SelectedObject);
			} 
			catch (Exception e)
			{
				g.DrawString(e.Message,new Font("Verdana",13),Brushes.Black,10,10);
			}
		}

		public ObjectBase GetObjectAt(int X,int Y)
		{
			foreach(ObjectBase ob in objects) 
			{
				if (ob.InObject(X,Y)) 
					return ob;
			}
			return null;
		}

		public int GetPointIndex(int X,int Y,out ObjectBase CurrentObject)
		{
			foreach(ObjectBase ob in objects) 
			{
				int i =ob.GetControlPoint(X,Y);
				if (i>=0) 
				{
					CurrentObject = ob;
					return i;
				}
			}
			CurrentObject = null;
			return -1;
		}

		public int GetPointIndex(int X,int Y)
		{
			ObjectBase ob;
			return GetPointIndex(X,Y,out ob);
		}

		public void SetObjectManager()
		{
			foreach(ObjectBase ob in Objects) 
			{
				ob.SetObjectManager(this);
				if (Canvas.BackChart!=null)
					ob.Area = Canvas.BackChart[ob.AreaName];
			}
		}

		[XmlIgnore]
		public ObjectBase SelectedObject
		{
			get 
			{
				return selectedObject;
			}
			set 
			{
				selectedObject = value;
				if (value!=null && AfterSelect!=null)
					AfterSelect(this,value);
			}
		}

		#region Persistent
		public void WriteXml(TextWriter writer)
		{
			XmlSerializer xs = new XmlSerializer(typeof(ObjectManager));
			xs.Serialize(writer, this, 
				new XmlSerializerNamespaces(
				new XmlQualifiedName[]{new XmlQualifiedName("EasyChart","http://finance.easychart.net")}));
		}

		public void WriteXml(string FileName)
		{
			TextWriter writer = new StreamWriter(FileName);
			try
			{
				writer.NewLine = "\r\n";
				WriteXml(writer);
			} 
			finally 
			{
				writer.Close();
			}
		}

		public static ObjectManager ReadXml(TextReader reader,IObjectCanvas Canvas)
		{
			XmlSerializer xs = new XmlSerializer(typeof(ObjectManager));
			ObjectManager om = (ObjectManager)xs.Deserialize(reader);
			if (Canvas!=null)
			{
				om.SetCanvas(Canvas);
				om.Designer.Invalidate();
			}
			return om;
		}

		public static ObjectManager ReadXml(string FileName,IObjectCanvas Canvas) 
		{
			using (TextReader reader = new StreamReader(FileName))
			{
				return ReadXml(reader,Canvas);
			}
		}
		#endregion

		#region Event Handler
		private void CreateMemBmp()
		{
			if (MemBmp==null)
				MemBmp = new Bitmap(Designer.Width,Designer.Height,PixelFormat.Format32bppPArgb); 
		}

		private void DrawMemBmp(Bitmap BackImage,Region Clip)
		{
			CreateMemBmp();
			if (Designer!=null)
			{
				Graphics g = Graphics.FromImage(MemBmp);
				g.SetClip(Clip,CombineMode.Replace);
				if (BackImage!=null)
					g.DrawImage(BackImage,0,0);
				else g.Clear(Designer.BackColor);
				ObjectBase MovingObject = null;
				if (DragObject!=null)
					MovingObject = DragObject.Object;
				Draw(g,SelectedObject,MovingObject);
			}
		}

		private ObjectPoint GetValueFromPos(float X,float Y,ref FormulaArea fa)
		{
			return Canvas.BackChart.GetValueFromPos(X,Y,ref fa);
		}

		
		private void DesignerControl_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			FormulaArea fa = null;
			StartPoint = GetValueFromPos(e.X,e.Y,ref fa);
			ObjectBase ob;
			if (Designer is ChartWinControl)
			{
				SavedShowCrossCursor = (Designer as ChartWinControl).ShowCrossCursor;
				SavedShowStatistic = (Designer as ChartWinControl).ShowStatistic;
				(Designer as ChartWinControl).ShowCrossCursor = false;
				(Designer as ChartWinControl).ShowStatistic = false;
			}
			
			if (ObjectType==null)
			{
				int PointIndex = GetPointIndex(e.X,e.Y,out ob);
				if (ob==null)
					ob = GetObjectAt(e.X,e.Y);

				if (ob!=null)
				{
					SelectedObject = ob;
					DragObject = new DraggingObject(new PointF(e.X,e.Y),PointIndex,ob);
					Designer.Invalidate(ob.GetRegion());
					Canvas.Dragging = true;
					ob.InMove = true;
				}
			} 
			else 
			{
				if (e.Button==MouseButtons.Right)
				{
					objects.Remove(DragObject.Object);
					DragObjectFinished();
				}
				else 
				{
					if (ObjectSteps==0)
					{
						ob = ObjectType.Invoke();
						ob.AreaName = fa.Name;
						ob.Area = fa;
						ob.InSetup = true;
						ob.InMove = true;
						if (AfterCreate!=null)
							AfterCreate(this,ob);
						ob.SetObjectManager(this);
						for(int i=0; i<ob.ControlPointNum; i++)
							ob.ControlPoints[i] = StartPoint;
						objects.Add(ob);
						SelectedObject = ob;
						DragObject = new DraggingObject(new PointF(e.X,e.Y),ObjectSteps+1,ob);
						Canvas.Dragging = true;
					} 
					else
					{
						ob = DragObject.Object;
						DragObject.ControlPointIndex = ObjectSteps+1;
					}
					ObjectSteps++;
					if (ObjectSteps==ob.InitNum)
						DragObjectFinished();
				}
			}
			DragMemBmp = null;
		}

		private void InvalidateObject(ObjectBase ob)
		{
			Region R = ob.GetRegion();
			if (ob.Area!=null)
				R.Intersect(ob.Area.Canvas.Rect);
			Designer.Invalidate(R);
		}

		private void DesignerControl_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (DragObject!=null)
			{
				FormulaArea fa = DragObject.Object.Area;
				float DeltaX = e.X-DragObject.StartPoint.X;
				float DeltaY = e.Y-DragObject.StartPoint.Y;
				InvalidateObject(DragObject.Object);
				if (DragObject.ControlPointIndex<0) 
				{
					for(int i=0; i<DragObject.Object.ControlPoints.Length; i++) 
					{
						PointF pf = DragObject.Object.ToPointF(DragObject.ControlPoints[i]);
						DragObject.Object.ControlPoints[i] = GetValueFromPos(pf.X+DeltaX,pf.Y+DeltaY,ref fa);
					}
				} 
				else 
				{
					DragObject.Object.ControlPoints[DragObject.ControlPointIndex] = GetValueFromPos(e.X,e.Y,ref fa);
					if (DragObject.Object.InitPoints!=null && DragObject.Object.InSetup)
					{
						if (DragObject.ControlPoints.Length>1)
						{
							PointF pf1 = DragObject.Object.ToPointF(DragObject.Object.ControlPoints[1]);
							PointF pf0 = DragObject.Object.ToPointF(DragObject.Object.ControlPoints[0]);

							float FactorX = (pf1.X-pf0.X)/DragObject.Object.InitPoints[1].X;
							float FactorY = (pf1.Y-pf0.Y)/DragObject.Object.InitPoints[1].Y;
							for(int i=2; i<DragObject.Object.ControlPoints.Length; i++)
							{
								DragObject.Object.ControlPoints[i] = GetValueFromPos(
									pf0.X+FactorX*DragObject.Object.InitPoints[i].X,
									pf0.Y+FactorY*DragObject.Object.InitPoints[i].Y,ref fa);
							}
						}
					} 
				}
				InvalidateObject(DragObject.Object);
			} 
			else 
			{
				Cursor OldCursor = Designer.Cursor;

				bool b = GetPointIndex(e.X,e.Y)>=0;
				if (b)
					Designer.Cursor = Cursors.SizeAll;
				else 
				{
					ObjectBase ob = GetObjectAt(e.X,e.Y);
					if (ob!=null)
						Designer.Cursor = Cursors.Hand;
					else if (ObjectType==null)
						Designer.Cursor = OldCursor;
					else Designer.Cursor = Cursors.Cross;
				}
			}
		}

		private Bitmap ObjectPaint(Graphics ObjectG,Bitmap BackImage) 
		{
			if (DragObject!=null) 
			{
				bool IsEmpty = false;
				if (DragMemBmp==null) 
				{
					DragMemBmp = new Bitmap(MemBmp.Width,MemBmp.Height,PixelFormat.Format32bppPArgb);
					DrawMemBmp(BackImage,ObjectG.Clip);
					IsEmpty = true;
				}
				Graphics g = Graphics.FromImage(DragMemBmp);
				if (IsEmpty)
					g.DrawImage(MemBmp,0,0);

				g.SetClip(ObjectG.Clip,CombineMode.Replace);
				if (BackImage!=null)
					g.DrawImage(BackImage,0,0);

				g.DrawImage(MemBmp,0,0);
				DrawObject(g,DragObject.Object,true);
				return DragMemBmp;
			} 
			else
			{
				DrawMemBmp(BackImage,ObjectG.Clip);
				return MemBmp;
			}
		}

		private void DesignerControl_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			Bitmap B = ObjectPaint(e.Graphics,null);
			e.Graphics.DrawImage(B,0,0);
		}

		private void DragObjectFinished()
		{
			if (Designer is ChartWinControl) 
			{
				(Designer as ChartWinControl).ShowCrossCursor = SavedShowCrossCursor;
				(Designer as ChartWinControl).ShowStatistic = SavedShowStatistic;
			}

			if (ObjectChanged!=null && DragObject!=null) 
			{
				DragObject.Object.InSetup = false;
				DragObject.Object.InMove = false;
				ObjectChanged(this,DragObject.Object);
			}
			DragObject = null;
			ObjectType = null;
			Canvas.Dragging = false;
			ObjectSteps = 0;
			Designer.Invalidate();
		}

		private void DesignerControl_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (ObjectType==null)
				DragObjectFinished();
		}

		private void DesignerControl_SizeChanged(object sender, EventArgs e)
		{
			MemBmp = null;
			FormGraphics = Designer.CreateGraphics();
		}

		private void ObjectManager_ExtraPaint(object sender, NativePaintArgs e)
		{
			e.NewBitmap = ObjectPaint(e.Graphics,e.NativeBitmap);
		}
		#endregion

		#region Keyboard
		public void Delete()
		{
			int i = objects.IndexOf(SelectedObject);
			objects.Remove(SelectedObject);
			if (i>=objects.Count)
				i = objects.Count-1;
			if (i>=0)
				SelectedObject = objects[i];
			else SelectedObject = null;
			Designer.Invalidate();
			if (AfterSelect!=null)
				AfterSelect(this,SelectedObject);
		}

		public void Copy()
		{
			Clipboard.SetDataObject(SelectedObject);
		}

		public void Paste()
		{
			IDataObject ido = Clipboard.GetDataObject();
			string[] ss = ido.GetFormats();
			ObjectBase ob = (ObjectBase)ido.GetData("TestEpg.ObjectBase",true);
			if (ob!=null)
			{
			}
		}

		private void Designer_KeyDown(object sender, KeyEventArgs e)
		{
			switch (e.KeyCode) 
			{
				case Keys.Delete:
					Delete();
					break;
				case Keys.C:
					if (e.Control) Copy();
					break;
				case Keys.V:
					if (e.Control) Paste();
					break;
			}
		}
		#endregion
	}
}