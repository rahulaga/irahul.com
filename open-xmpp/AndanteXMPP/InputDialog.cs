using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace AndanteXMPP
{
	/// <summary>
	/// Summary description for InputDialog.
	/// </summary>
	public class InputDialog : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label infoLbl;
		private System.Windows.Forms.TextBox infoBox;
		private System.Windows.Forms.Button okBtn;
		private System.Windows.Forms.Button cancelBtn;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public InputDialog()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
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

		public string InfoInstructions
		{
			get { return infoLbl.Text; }
			set { this.infoLbl.Text = value; }
		}

		public string Info
		{
			get { return infoBox.Text; }
			set { this.infoBox.Text = (value == null ? "" : value); }
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.infoLbl = new System.Windows.Forms.Label();
			this.infoBox = new System.Windows.Forms.TextBox();
			this.okBtn = new System.Windows.Forms.Button();
			this.cancelBtn = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// infoLbl
			// 
			this.infoLbl.Location = new System.Drawing.Point(16, 8);
			this.infoLbl.Name = "infoLbl";
			this.infoLbl.Size = new System.Drawing.Size(360, 24);
			this.infoLbl.TabIndex = 0;
			// 
			// infoBox
			// 
			this.infoBox.Location = new System.Drawing.Point(8, 48);
			this.infoBox.Name = "infoBox";
			this.infoBox.Size = new System.Drawing.Size(368, 20);
			this.infoBox.TabIndex = 1;
			this.infoBox.Text = "";
			// 
			// okBtn
			// 
			this.okBtn.Location = new System.Drawing.Point(16, 80);
			this.okBtn.Name = "okBtn";
			this.okBtn.TabIndex = 2;
			this.okBtn.Text = "OK";
			this.okBtn.Click += new System.EventHandler(this.okBtn_Click);
			// 
			// cancelBtn
			// 
			this.cancelBtn.Location = new System.Drawing.Point(104, 80);
			this.cancelBtn.Name = "cancelBtn";
			this.cancelBtn.TabIndex = 3;
			this.cancelBtn.Text = "Cancel";
			this.cancelBtn.Click += new System.EventHandler(this.cancelBtn_Click);
			// 
			// InputDialog
			// 
			this.AcceptButton = this.okBtn;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(384, 117);
			this.Controls.Add(this.cancelBtn);
			this.Controls.Add(this.okBtn);
			this.Controls.Add(this.infoBox);
			this.Controls.Add(this.infoLbl);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "InputDialog";
			this.Text = "InputDialog";
			this.TopMost = true;
			this.ResumeLayout(false);

		}
		#endregion

		private void okBtn_Click(object sender, System.EventArgs e)
		{
			this.DialogResult = DialogResult.OK;
			Hide();
		}

		private void cancelBtn_Click(object sender, System.EventArgs e)
		{
			this.DialogResult = DialogResult.Cancel;
			Hide();
		}
	}
}
