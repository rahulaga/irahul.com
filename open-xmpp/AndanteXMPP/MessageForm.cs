using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using openXMPP;

namespace AndanteXMPP
{
	/// <summary>
	/// Summary description for MessageForm.
	/// </summary>
	public class MessageForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.TextBox inputBox;
		private System.Windows.Forms.TextBox textBox;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		private Session session;
		private string receiverJid;
		private string language;
		private string threadId;

//		public MessageForm(Session session, string title, string receiverJid)
//		{
//			//
//			// Required for Windows Form Designer support
//			//
//			InitializeComponent();
//
//			this.session = session;
//			this.Text = title;
//			this.receiverJid = receiverJid;
//		}

		public MessageForm(Session session, string title, string receiverJid, string language, string threadId) //: this(session, title, receiverJid)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			this.session = session;
			this.Text = title;
			this.receiverJid = receiverJid;
			this.language = language; 
			this.threadId = threadId;
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
			this.inputBox = new System.Windows.Forms.TextBox();
			this.textBox = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// inputBox
			// 
			this.inputBox.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.inputBox.Location = new System.Drawing.Point(0, 377);
			this.inputBox.Name = "inputBox";
			this.inputBox.Size = new System.Drawing.Size(544, 20);
			this.inputBox.TabIndex = 0;
			this.inputBox.Text = "";
			this.inputBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.inputBox_KeyDown);
			// 
			// textBox
			// 
			this.textBox.BackColor = System.Drawing.Color.White;
			this.textBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.textBox.Location = new System.Drawing.Point(0, 0);
			this.textBox.Multiline = true;
			this.textBox.Name = "textBox";
			this.textBox.ReadOnly = true;
			this.textBox.Size = new System.Drawing.Size(544, 377);
			this.textBox.TabIndex = 1;
			this.textBox.Text = "";
			// 
			// MessageForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(544, 397);
			this.Controls.Add(this.textBox);
			this.Controls.Add(this.inputBox);
			this.Name = "MessageForm";
			this.Text = "MessageForm";
			this.Closing += new System.ComponentModel.CancelEventHandler(this.MessageForm_Closing);
			this.ResumeLayout(false);

		}
		#endregion

		public string ReceiverJid
		{
			get { return this.receiverJid; }
			set { this.receiverJid = value; }
		}
		public string Language
		{
			get { return this.language; }
			set { this.language = value; }
		}

		public string ThreadId
		{
			get { return this.threadId; }
			set { this.threadId = value; }
		}

		
		private void inputBox_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if(e.KeyCode == Keys.Enter)
			{
				MessageStanza reply = StanzaFactory.GetChatMessageStanza(session.JabberID, receiverJid, language, inputBox.Text, null, threadId);
				session.SendStanza(reply);
				appendMessage(reply);
				inputBox.Text = "";
			}
		}

		public void NewMessage(MessageStanza s)
		{	
			appendMessage(s);
		}

		private void appendMessage(MessageStanza s)
		{
			textBox.Text += "["+DateTime.Now.ToShortTimeString()+"] <"+receiverJid+"> : "+ s.FirstBody.Body + "\r\n";
		}

		private void MessageForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			this.Visible = false;
			e.Cancel = true;
		}
	}
}
