using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using openXMPP;

namespace AndanteXMPP
{
	/// <summary>
	/// Summary description for SignInForm.
	/// </summary>
	public class SignInForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox usernameBox;
		private System.Windows.Forms.TextBox passwordBox;
		private System.Windows.Forms.TextBox domainBox;
		private System.Windows.Forms.TextBox resourceBox;
		private System.Windows.Forms.TextBox serverBox;
		private System.Windows.Forms.TextBox portBox;
		private System.Windows.Forms.Button okBtn;
		private System.Windows.Forms.Button cancelBtn;
		private System.Windows.Forms.ComboBox quickSettingsBox;
		private System.Windows.Forms.Button button1;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public SignInForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		public string Username
		{
			get { return usernameBox.Text; }
		}

		public string Password
		{
			get { return passwordBox.Text; }
		}

		public string Domain
		{
			get { return domainBox.Text; }
		}

		public string Resource
		{
			get { return resourceBox.Text; }
		}

		public string Server
		{
			get { return serverBox.Text; }
		}

		public int Port
		{
			get { return int.Parse(portBox.Text); }
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
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.usernameBox = new System.Windows.Forms.TextBox();
			this.passwordBox = new System.Windows.Forms.TextBox();
			this.domainBox = new System.Windows.Forms.TextBox();
			this.resourceBox = new System.Windows.Forms.TextBox();
			this.serverBox = new System.Windows.Forms.TextBox();
			this.portBox = new System.Windows.Forms.TextBox();
			this.okBtn = new System.Windows.Forms.Button();
			this.cancelBtn = new System.Windows.Forms.Button();
			this.quickSettingsBox = new System.Windows.Forms.ComboBox();
			this.button1 = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(16, 48);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(53, 16);
			this.label1.TabIndex = 0;
			this.label1.Text = "Userame:";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(16, 112);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(46, 16);
			this.label2.TabIndex = 1;
			this.label2.Text = "Domain:";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(16, 144);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(56, 16);
			this.label3.TabIndex = 2;
			this.label3.Text = "Resource:";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(16, 80);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(57, 16);
			this.label4.TabIndex = 3;
			this.label4.Text = "Password:";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(16, 176);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(86, 16);
			this.label5.TabIndex = 4;
			this.label5.Text = "Connect Server:";
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(16, 208);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(28, 16);
			this.label6.TabIndex = 5;
			this.label6.Text = "Port:";
			// 
			// usernameBox
			// 
			this.usernameBox.Location = new System.Drawing.Point(104, 48);
			this.usernameBox.Name = "usernameBox";
			this.usernameBox.Size = new System.Drawing.Size(176, 20);
			this.usernameBox.TabIndex = 6;
			this.usernameBox.Text = "jcl.openXMPP";
			// 
			// passwordBox
			// 
			this.passwordBox.Location = new System.Drawing.Point(104, 80);
			this.passwordBox.Name = "passwordBox";
			this.passwordBox.PasswordChar = '*';
			this.passwordBox.Size = new System.Drawing.Size(176, 20);
			this.passwordBox.TabIndex = 7;
			this.passwordBox.Text = "OpEnXmPp";
			// 
			// domainBox
			// 
			this.domainBox.Location = new System.Drawing.Point(104, 112);
			this.domainBox.Name = "domainBox";
			this.domainBox.Size = new System.Drawing.Size(176, 20);
			this.domainBox.TabIndex = 8;
			this.domainBox.Text = "gmail.com";
			// 
			// resourceBox
			// 
			this.resourceBox.Location = new System.Drawing.Point(104, 144);
			this.resourceBox.Name = "resourceBox";
			this.resourceBox.Size = new System.Drawing.Size(176, 20);
			this.resourceBox.TabIndex = 9;
			this.resourceBox.Text = "openXMPP";
			// 
			// serverBox
			// 
			this.serverBox.Location = new System.Drawing.Point(104, 176);
			this.serverBox.Name = "serverBox";
			this.serverBox.Size = new System.Drawing.Size(176, 20);
			this.serverBox.TabIndex = 10;
			this.serverBox.Text = "talk.google.com";
			// 
			// portBox
			// 
			this.portBox.Location = new System.Drawing.Point(104, 208);
			this.portBox.Name = "portBox";
			this.portBox.Size = new System.Drawing.Size(176, 20);
			this.portBox.TabIndex = 11;
			this.portBox.Text = "5222";
			// 
			// okBtn
			// 
			this.okBtn.Location = new System.Drawing.Point(120, 240);
			this.okBtn.Name = "okBtn";
			this.okBtn.TabIndex = 12;
			this.okBtn.Text = "OK";
			this.okBtn.Click += new System.EventHandler(this.okBtn_Click);
			// 
			// cancelBtn
			// 
			this.cancelBtn.Location = new System.Drawing.Point(200, 240);
			this.cancelBtn.Name = "cancelBtn";
			this.cancelBtn.TabIndex = 13;
			this.cancelBtn.Text = "Cancel";
			this.cancelBtn.Click += new System.EventHandler(this.cancelBtn_Click);
			// 
			// quickSettingsBox
			// 
			this.quickSettingsBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.quickSettingsBox.Items.AddRange(new object[] {
																  "talk.google.com",
																  "here.dk",
																  "odysseus"});
			this.quickSettingsBox.Location = new System.Drawing.Point(16, 16);
			this.quickSettingsBox.Name = "quickSettingsBox";
			this.quickSettingsBox.Size = new System.Drawing.Size(264, 21);
			this.quickSettingsBox.TabIndex = 14;
			this.quickSettingsBox.SelectedIndexChanged += new System.EventHandler(this.quickSettingsBox_SelectedIndexChanged);
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(16, 240);
			this.button1.Name = "button1";
			this.button1.TabIndex = 15;
			this.button1.Text = "Register";
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// SignInForm
			// 
			this.AcceptButton = this.okBtn;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(292, 273);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.quickSettingsBox);
			this.Controls.Add(this.cancelBtn);
			this.Controls.Add(this.okBtn);
			this.Controls.Add(this.portBox);
			this.Controls.Add(this.serverBox);
			this.Controls.Add(this.resourceBox);
			this.Controls.Add(this.domainBox);
			this.Controls.Add(this.passwordBox);
			this.Controls.Add(this.usernameBox);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "SignInForm";
			this.Text = "Sign In...";
			this.Load += new System.EventHandler(this.SignInForm_Load);
			this.ResumeLayout(false);

		}
		#endregion

		private void okBtn_Click(object sender, System.EventArgs e)
		{
			this.DialogResult = DialogResult.OK;
		}

		private void cancelBtn_Click(object sender, System.EventArgs e)
		{
			this.DialogResult = DialogResult.Cancel;
		}

		private void quickSettingsBox_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if(quickSettingsBox.SelectedIndex == 0)
			{
				domainBox.Text = "gmail.com";
				serverBox.Text = "talk.google.com";
			} 
			else if(quickSettingsBox.SelectedIndex == 1)
			{
				domainBox.Text = "here.dk";
				serverBox.Text = "here.dk";
			}
			else if(quickSettingsBox.SelectedIndex == 2)
			{
				domainBox.Text = "localhost";
				serverBox.Text = "odysseus";
			}
		}

		private void button1_Click(object sender, System.EventArgs e)
		{
			SecurableTcpClient tcpClient = new MonoTcpClient(serverBox.Text, int.Parse(portBox.Text));
			AccountManager manager = new AccountManager(tcpClient);

			manager.OnRegistrationFieldsReceived += new AccountRegistrationHandler(manager_OnRegistrationFieldsReceived);
			manager.OnSuccess += new EventHandler(manager_OnSuccess);
			manager.OnFailure += new EventHandler(manager_OnFailure);
			manager.OnAnyReceive += new XmlProtocolElementHandler(manager_OnAnyReceive);
			manager.OnAnySend += new XmlProtocolElementHandler(manager_OnAnySend);
			manager.OnError += new SessionEventHandler(manager_OnError);

			manager.RequestAccountRegistration(domainBox.Text, 30000);
		}

		private Hashtable manager_OnRegistrationFieldsReceived(object sender, Hashtable registrationFields)
		{
			Hashtable t = new Hashtable();

			foreach(string key in registrationFields.Keys)
			{
				switch(key)
				{
					case "username":
						t.Add(key, usernameBox.Text);
						break;
					case "password":
						t.Add(key, passwordBox.Text);
						break;
					case "instructions":
						MessageBox.Show(this, registrationFields[key].ToString(), "Server instructions", MessageBoxButtons.OK, MessageBoxIcon.Information);
						break;
					case "x":
						break; // Client-specific instructions.  Do nothing
					default:
						//MessageBox.Show(this, "The server requires the field "+key+" but I'm too lazy to get that from the user.  Sorry.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
						InputDialog idg = new InputDialog();
						idg.InfoInstructions = "Please provide the following: "+key;
						idg.Info = t[key] as string;
						DialogResult result = idg.ShowDialog();
						if(result == DialogResult.OK)
						{
							t.Add(key, idg.Info);
						}
						else 
						{
							MessageBox.Show(this, "'"+key+"' has been asked for so it's probably required.  Registration is likely to fail.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
						}

						break;
				}
			}

			return t;
		}

		private void manager_OnSuccess(object sender, EventArgs e)
		{
			MessageBox.Show("Account was successfully registered");
		}

		private void manager_OnFailure(object sender, EventArgs e)
		{
			MessageBox.Show("Account could not be registered.  The username may be in use or the password may not meet the server's requirements.");
		}

		private void SignInForm_Load(object sender, System.EventArgs e)
		{
			quickSettingsBox.SelectedIndex = 0;
		}

		private void manager_OnAnySend(object sender, XmlProtocolElement element)
		{
			Console.WriteLine("SEND: "+element);
		}

		private void manager_OnAnyReceive(object sender, XmlProtocolElement element)
		{
			Console.WriteLine("RECV:"+element);
		}

		private void manager_OnError(object sender, object evtObj)
		{
			MessageBox.Show(this, "An error has occured.  Error object: "+evtObj, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
	}
}
