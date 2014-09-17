using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace Easychart.Finance.Objects
{
	/// <summary>
	/// Summary description for ObjectTree.
	/// </summary>
	public class ObjectTree : System.Windows.Forms.UserControl
	{
		private System.Windows.Forms.TreeView tvObject;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		private ObjectManager om;

		public ObjectTree()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call

		}

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.tvObject = new System.Windows.Forms.TreeView();
			this.SuspendLayout();
			// 
			// tvObject
			// 
			this.tvObject.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.tvObject.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tvObject.HideSelection = false;
			this.tvObject.ImageIndex = -1;
			this.tvObject.Location = new System.Drawing.Point(0, 0);
			this.tvObject.Name = "tvObject";
			this.tvObject.SelectedImageIndex = -1;
			this.tvObject.Size = new System.Drawing.Size(168, 400);
			this.tvObject.TabIndex = 0;
			this.tvObject.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvObject_AfterSelect);
			// 
			// ObjectTree
			// 
			this.Controls.Add(this.tvObject);
			this.Name = "ObjectTree";
			this.Size = new System.Drawing.Size(168, 400);
			this.ResumeLayout(false);

		}
		#endregion

		public void RebuildTree(ObjectManager om)
		{
			this.om = om;
			tvObject.SuspendLayout();
			try
			{
				tvObject.BeginUpdate();
				try
				{
					tvObject.Nodes.Clear();
					foreach(BaseObject bo in om.Objects)
					{
						TreeNode tn = new TreeNode(bo.Name);
						tn.Tag = bo;
						tvObject.Nodes.Add(tn);
					}
				}
				finally
				{
					tvObject.EndUpdate();
				}
			}
			finally
			{
				tvObject.ResumeLayout();
			}
		}

		private void tvObject_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e)
		{
			if (om!=null)
			{
				object o = e.Node.Tag;
				if (o is BaseObject) 
				{
					om.SelectedObject = (BaseObject)o;
					om.Invalidate();
				}
			}
		}
	}
}
