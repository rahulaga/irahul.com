using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace AndanteXMPP
{
	/// <summary>
	/// Summary description for AddContactForm.
	/// </summary>
	public class AddContactForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox jabberIdBox;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox nameBox;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox groupBox;
		private System.Windows.Forms.Button okBtn;
		private System.Windows.Forms.Button cancelBtn;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public AddContactForm()
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

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.label1 = new System.Windows.Forms.Label();
			this.jabberIdBox = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.nameBox = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.groupBox = new System.Windows.Forms.TextBox();
			this.okBtn = new System.Windows.Forms.Button();
			this.cancelBtn = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(8, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(56, 16);
			this.label1.TabIndex = 0;
			this.label1.Text = "Jabber ID:";
			// 
			// jabberIdBox
			// 
			this.jabberIdBox.Location = new System.Drawing.Point(72, 16);
			this.jabberIdBox.Name = "jabberIdBox";
			this.jabberIdBox.Size = new System.Drawing.Size(208, 20);
			this.jabberIdBox.TabIndex = 1;
			this.jabberIdBox.Text = "";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(8, 48);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(38, 16);
			this.label2.TabIndex = 2;
			this.label2.Text = "Name:";
			// 
			// nameBox
			// 
			this.nameBox.Location = new System.Drawing.Point(72, 48);
			this.nameBox.Name = "nameBox";
			this.nameBox.Size = new System.Drawing.Size(208, 20);
			this.nameBox.TabIndex = 3;
			this.nameBox.Text = "";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(8, 80);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(39, 16);
			this.label3.TabIndex = 4;
			this.label3.Text = "Group:";
			// 
			// groupBox
			// 
			this.groupBox.Location = new System.Drawing.Point(72, 80);
			this.groupBox.Name = "groupBox";
			this.groupBox.Size = new System.Drawing.Size(208, 20);
			this.groupBox.TabIndex = 5;
			this.groupBox.Text = "";
			// 
			// okBtn
			// 
			this.okBtn.Location = new System.Drawing.Point(32, 120);
			this.okBtn.Name = "okBtn";
			this.okBtn.TabIndex = 6;
			this.okBtn.Text = "OK";
			this.okBtn.Click += new System.EventHandler(this.okBtn_Click);
			// 
			// cancelBtn
			// 
			this.cancelBtn.Location = new System.Drawing.Point(184, 120);
			this.cancelBtn.Name = "cancelBtn";
			this.cancelBtn.TabIndex = 7;
			this.cancelBtn.Text = "Cancel";
			this.cancelBtn.Click += new System.EventHandler(this.cancelBtn_Click);
			// 
			// AddContactForm
			// 
			this.AcceptButton = this.okBtn;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(292, 157);
			this.Controls.Add(this.cancelBtn);
			this.Controls.Add(this.okBtn);
			this.Controls.Add(this.groupBox);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.nameBox);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.jabberIdBox);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "AddContactForm";
			this.Text = "AddContactForm";
			this.ResumeLayout(false);

		}
		#endregion

		public string ContactJabberID
		{
			get { return jabberIdBox.Text; }
		}

		public string ContactName
		{
			get { return nameBox.Text; }
		}

		public string ContactGroup
		{
			get { return groupBox.Text; }
		}

		private void okBtn_Click(object sender, System.EventArgs e)
		{
			this.DialogResult = DialogResult.OK;
		}

		private void cancelBtn_Click(object sender, System.EventArgs e)
		{
			this.DialogResult = DialogResult.Cancel;
		}
	}
}
