using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace AndanteXMPP
{
	/// <summary>
	/// Summary description for LogForm.
	/// </summary>
	public class LogForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.TextBox logBox;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public LogForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
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

		public void Log(string message)
		{
			try 
			{
				logBox.Text += message + "\r\n\r\n";
				Console.WriteLine(message);
			} 
			catch
			{
			}
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.logBox = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// logBox
			// 
			this.logBox.BackColor = System.Drawing.Color.White;
			this.logBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.logBox.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.logBox.Location = new System.Drawing.Point(0, 0);
			this.logBox.Multiline = true;
			this.logBox.Name = "logBox";
			this.logBox.ReadOnly = true;
			this.logBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.logBox.Size = new System.Drawing.Size(688, 504);
			this.logBox.TabIndex = 0;
			this.logBox.Text = "";
			this.logBox.WordWrap = false;
			// 
			// LogForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(688, 504);
			this.Controls.Add(this.logBox);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.Name = "LogForm";
			this.Text = "Log";
			this.Closing += new System.ComponentModel.CancelEventHandler(this.LogForm_Closing);
			this.ResumeLayout(false);

		}
		#endregion

		private void LogForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			this.Visible = false;
			e.Cancel = true;
		}

	}
}
