using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using openXMPP;
using System.Xml;

namespace AndanteXMPP
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class MainForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.MainMenu mainMenu;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem menuItem3;
		private System.Windows.Forms.MenuItem menuItem4;
		private System.Windows.Forms.MenuItem menuItem5;
		private System.Windows.Forms.MenuItem menuItem6;
		private System.Windows.Forms.MenuItem menuItem7;
		private System.Windows.Forms.MenuItem menuItem8;
		private System.Windows.Forms.MenuItem menuItem9;
		private System.Windows.Forms.TreeView rosterView;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		private openXMPP.Session session;
		private LogForm logForm;
		private StatusForm statForm;
		private System.Windows.Forms.MenuItem menuItem11;
		private System.Windows.Forms.MenuItem menuItem12;
		private System.Windows.Forms.MenuItem menuItem13;
		private System.Windows.Forms.MenuItem menuItem14;

		private Hashtable roster;
		private System.Windows.Forms.MenuItem menuItem15;
		private Hashtable chats;

		public MainForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			roster = new Hashtable();
			logForm = new LogForm();
			statForm = new StatusForm();
		}

	
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
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
			this.mainMenu = new System.Windows.Forms.MainMenu();
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.menuItem4 = new System.Windows.Forms.MenuItem();
			this.menuItem7 = new System.Windows.Forms.MenuItem();
			this.menuItem5 = new System.Windows.Forms.MenuItem();
			this.menuItem6 = new System.Windows.Forms.MenuItem();
			this.menuItem11 = new System.Windows.Forms.MenuItem();
			this.menuItem12 = new System.Windows.Forms.MenuItem();
			this.menuItem13 = new System.Windows.Forms.MenuItem();
			this.menuItem14 = new System.Windows.Forms.MenuItem();
			this.menuItem15 = new System.Windows.Forms.MenuItem();
			this.menuItem3 = new System.Windows.Forms.MenuItem();
			this.menuItem8 = new System.Windows.Forms.MenuItem();
			this.menuItem9 = new System.Windows.Forms.MenuItem();
			this.rosterView = new System.Windows.Forms.TreeView();
			this.SuspendLayout();
			// 
			// mainMenu
			// 
			this.mainMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					 this.menuItem1,
																					 this.menuItem11,
																					 this.menuItem3});
			// 
			// menuItem1
			// 
			this.menuItem1.Index = 0;
			this.menuItem1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.menuItem4,
																					  this.menuItem7,
																					  this.menuItem5,
																					  this.menuItem6});
			this.menuItem1.Text = "Tools";
			this.menuItem1.Click += new System.EventHandler(this.menuItem1_Click);
			// 
			// menuItem4
			// 
			this.menuItem4.Index = 0;
			this.menuItem4.Text = "Sign In...";
			this.menuItem4.Click += new System.EventHandler(this.menuItem4_Click);
			// 
			// menuItem7
			// 
			this.menuItem7.Index = 1;
			this.menuItem7.Text = "Add Contact...";
			this.menuItem7.Click += new System.EventHandler(this.menuItem7_Click);
			// 
			// menuItem5
			// 
			this.menuItem5.Index = 2;
			this.menuItem5.Text = "Options";
			this.menuItem5.Click += new System.EventHandler(this.menuItem5_Click);
			// 
			// menuItem6
			// 
			this.menuItem6.Index = 3;
			this.menuItem6.Text = "Exit";
			this.menuItem6.Click += new System.EventHandler(this.menuItem6_Click);
			// 
			// menuItem11
			// 
			this.menuItem11.Index = 1;
			this.menuItem11.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					   this.menuItem12,
																					   this.menuItem13,
																					   this.menuItem14,
																					   this.menuItem15});
			this.menuItem11.Text = "Presence";
			// 
			// menuItem12
			// 
			this.menuItem12.Index = 0;
			this.menuItem12.Text = "Available";
			this.menuItem12.Click += new System.EventHandler(this.menuItem12_Click);
			// 
			// menuItem13
			// 
			this.menuItem13.Index = 1;
			this.menuItem13.Text = "Away";
			this.menuItem13.Click += new System.EventHandler(this.menuItem13_Click);
			// 
			// menuItem14
			// 
			this.menuItem14.Index = 2;
			this.menuItem14.Text = "Do Not Disturb";
			this.menuItem14.Click += new System.EventHandler(this.menuItem14_Click);
			// 
			// menuItem15
			// 
			this.menuItem15.Index = 3;
			this.menuItem15.Text = "Offline";
			this.menuItem15.Click += new System.EventHandler(this.menuItem15_Click);
			// 
			// menuItem3
			// 
			this.menuItem3.Index = 2;
			this.menuItem3.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.menuItem8,
																					  this.menuItem9});
			this.menuItem3.Text = "Help";
			// 
			// menuItem8
			// 
			this.menuItem8.Index = 0;
			this.menuItem8.Text = "Show Log";
			this.menuItem8.Click += new System.EventHandler(this.menuItem8_Click);
			// 
			// menuItem9
			// 
			this.menuItem9.Index = 1;
			this.menuItem9.Text = "About...";
			this.menuItem9.Click += new System.EventHandler(this.menuItem9_Click);
			// 
			// rosterView
			// 
			this.rosterView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.rosterView.ImageIndex = -1;
			this.rosterView.Location = new System.Drawing.Point(0, 0);
			this.rosterView.Name = "rosterView";
			this.rosterView.SelectedImageIndex = -1;
			this.rosterView.Size = new System.Drawing.Size(536, 414);
			this.rosterView.TabIndex = 0;
			this.rosterView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.rosterView_KeyDown);
			this.rosterView.DoubleClick += new System.EventHandler(this.rosterView_DoubleClick);
			// 
			// MainForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(536, 414);
			this.Controls.Add(this.rosterView);
			this.Menu = this.mainMenu;
			this.Name = "MainForm";
			this.Text = "AndanteXMPP";
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new MainForm());
		}

	
		private void menuItem4_Click(object sender, System.EventArgs e)
		{
			if(session == null || session.State != SessionState.LoggedIn)
			{
				// Get login information
				SignInForm s = new SignInForm();
				s.ShowDialog();

				// Open the session
				if(s.DialogResult == DialogResult.OK)
				{
					// Show status bar
					statForm.Show();

					//Reinitialize chats
					chats = new Hashtable();

					// Close the old session
					if(session != null) session.Close();
					
					// Create TCP connection
					//SecurableTcpClient tcpClient = new MentalisTcpClient(s.Server, s.Port);
					SecurableTcpClient tcpClient = new MonoTcpClient(s.Server, s.Port);

					// Create a new session
					session = new Session(tcpClient);
					session.OnAnyReceive += new XmlProtocolElementHandler(session_OnAnyReceive);
					session.OnAnySend += new XmlProtocolElementHandler(session_OnAnySend);
					session.OnError += new SessionEventHandler(session_OnError);
					session.OnStateChange += new SessionEventHandler(session_OnStateChange);
					session.OnStanzaReceive += new StanzaHandler(session_OnStanzaReceive);

					// Open the session
					bool loggedIn = session.Open(s.Domain, s.Resource, s.Username, s.Password, 30000);
					statForm.Hide();

					// Retrieve the roster and send presence information
					if(loggedIn)
					{
						session.SendStanza(StanzaFactory.GetRosterStanza(session.JabberID));
						session.SendStanza(StanzaFactory.GetPresenceBroadcastStanza("online", "en"));
						menuItem4.Text = "Sign Out";
					}
					else
					{
						session.Close();
						MessageBox.Show("Login failure!");
					}
				}
			}
			else
			{
				session.Close();
				rosterView.Nodes.Clear();
				menuItem4.Text = "Sign In";
			}
		}

		private void menuItem6_Click(object sender, System.EventArgs e)
		{
			Application.Exit();
		}
		private void menuItem7_Click(object sender, System.EventArgs e)
		{
			AddContactForm frm = new AddContactForm();
			frm.ShowDialog();
			if(frm.DialogResult == DialogResult.OK)
			{
				session.SendStanza(StanzaFactory.GetAddFriendStanza(session.JabberID, frm.ContactJabberID, frm.ContactName, frm.ContactGroup));
				session.SendStanza(StanzaFactory.GetRequestSubscriptionStanza(frm.ContactJabberID));
			}
		}
		private void menuItem8_Click(object sender, System.EventArgs e)
		{
			logForm.Show();
		}

		private void menuItem9_Click(object sender, System.EventArgs e)
		{
			MessageBox.Show(this, "A simple graphical Jabber client for testing openXMPP.\r\n"+
				"Why the name?  Because I was listening to Mendelssohn's Andante from Symphony No. 5 when I wrote it ;)", "About");
		}

		private void menuItem12_Click(object sender, System.EventArgs e)
		{
			session.SendStanza(StanzaFactory.GetPresenceBroadcastStanza("online", "en"));
		}

		private void menuItem13_Click(object sender, System.EventArgs e)
		{
			session.SendStanza(StanzaFactory.GetPresenceBroadcastStanza("away", "en"));
		}

		private void menuItem14_Click(object sender, System.EventArgs e)
		{
			session.SendStanza(StanzaFactory.GetPresenceBroadcastStanza("dnd", "en"));
		}
		
		private void menuItem15_Click(object sender, System.EventArgs e)
		{
			session.SendStanza(StanzaFactory.GetPresenceBroadcastStanza("offline", "en"));
		}
		
		
		private void session_OnAnyReceive(object sender, XmlProtocolElement element)
		{
			logForm.Log("RECV: "+element.ToString());
		}

		private void session_OnAnySend(object sender, XmlProtocolElement element)
		{
			logForm.Log("SENT: "+element.ToString());
		}

		private void session_OnError(object sender, object message)
		{
			try 
			{
				logForm.Log("ERROR: "+message.ToString());
				//MessageBox.Show(this, "An error has occured.  Check the log file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			} 
			catch {}	// Sometimes the log form throws exceptions, but we can ignore them
		}

		private void session_OnStateChange(object sender, object newState)
		{
			statForm.Progress++;
			logForm.Log(newState.ToString());
		}

		
		private delegate void sesson_OnStanzaReceiveDelegate(object sender, Stanza s);
		private void session_OnStanzaReceive(object sender, Stanza s)
		{
			if(InvokeRequired)
			{
				BeginInvoke(new sesson_OnStanzaReceiveDelegate(session_OnStanzaReceive), new object[] {sender, s});
				return;
			}

			switch(s.Name)
			{
				case "message":
					onMessageStanzaReceived(s as MessageStanza);
					break;
				case "presence":
					onPresenceStanzaReceived(s as PresenceStanza);
					break;
				case "iq":
					onIqStanzaReceived(s as IQStanza);
					break;
			}
		}

		
		private void onPresenceStanzaReceived(PresenceStanza s)
		{
			string[] tokens = s.From.Split('/');
			Contact c = null;

			switch(s.Type)
			{
				case "subscribe":
					DialogResult res = MessageBox.Show(this, s.From+" is adding you to his roster.  Is this OK?", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
					session.SendStanza(StanzaFactory.GetReplySubscriptionRequestStanza(s.From, res == DialogResult.Yes));
					break;
				case "subscribed":
					// TODO
					break;
				case "unsubscribe":
					// TODO
					break;
				case "unsubscribed":
					// TODO
					break;
				case "unavailable":
					c = roster[tokens[0]] as Contact;
					if(c != null) c.Resources.Clear();
					refreshRosterView();
					break;
				default:
					// Someone has logged in so update their information
					c = roster[tokens[0]] as Contact;
					if(c != null)
					{
						if(!c.Resources.Contains(tokens[1])) c.Resources.Add(tokens[1]);
						refreshRosterView();
					}
					break;
			}
		}

		
		private void onMessageStanzaReceived(MessageStanza s)
		{
			// Get or create a chat window and show the message
			MessageForm msgf = null;
			if(chats[s.From] != null)
			{
				msgf = (MessageForm)chats[s.From];
			}
			else
			{
				msgf = new MessageForm(this.session, "Title", s.From, s.Language, s.ThreadID);
				chats[s.From] = msgf;
			}
			msgf.NewMessage(s);
			msgf.Show();
		}

		
		private void onIqStanzaReceived(IQStanza s)
		{
			switch(s.Type)
			{
				case "get":
					// TODO
					break;
				case "set":
					if(s.Query != null && s.Query.NamespaceURI == "jabber:iq:roster")
						session.SendStanza(StanzaFactory.GetRosterStanza(session.JabberID));
					break;
				case "error":
					// TODO
					break;
				case "result":
					// We have received our roster
					if(s.Query != null && s.Query.NamespaceURI == "jabber:iq:roster")
					{
						Hashtable newRoster = new Hashtable();
						rosterView.Nodes.Clear();

						// Create new roster containing only those people in the query
						foreach(XmlNode item in s.Query.ChildNodes)
						{
							if(item.Name == "item")
							{
								string jid = item.Attributes["jid"].Value;
								Contact c = new Contact();							
								c.BaseJabberId = jid;
								c.Name = item.Attributes["name"] == null ? jid : item.Attributes["name"].Value;
								newRoster.Add(jid, c);
							}
						}

						// Preserve resource information for people in our old roster
						foreach(Contact c in roster.Values)
						{
							if(newRoster[c.BaseJabberId] != null) newRoster[c.BaseJabberId] = c;
						}

						roster = newRoster;
							
						refreshRosterView();
					}
					break;
			}
		}


		private void rosterView_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			ContactTreeNode curNode = rosterView.SelectedNode as ContactTreeNode;
			if(curNode != null)
			{
				Contact c = curNode.Contact;
				if(e.KeyCode == Keys.Delete)
				{
					DialogResult res = MessageBox.Show(this, "Are you sure you want to delete "+c.Name+"?", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
					if(res == DialogResult.Yes) session.SendStanza(StanzaFactory.GetDeleteFriendStanza(session.JabberID, c.BaseJabberId));
				}
			}
		}

		
		private void rosterView_DoubleClick(object sender, System.EventArgs e)
		{
			ContactTreeNode curContact = rosterView.SelectedNode as ContactTreeNode;
			if(curContact != null)
			{
				Contact c = curContact.Contact;

				if(!c.Online)
				{
					MessageBox.Show(this, "That person is not online.");
					return;
				}

				MessageForm msgf = null;
				// See if we already have a chat with this person
				foreach(string s in c.AllJabberId)
				{
					if(chats[s] != null)
					{
						msgf = (MessageForm)chats[s];
						msgf.Show();
						return;
					}
				}

				// If not, start a new chat with their first resource
				string jid = c.BaseJabberId+"/"+c.Resources[0];
				msgf = new MessageForm(session, "Title", jid, "en", newThreadId());
				msgf.Language = "en";
				msgf.ThreadId = newThreadId();
				chats[jid] = msgf;
				msgf.Show();
			}
		}


		private void refreshRosterView()
		{
			rosterView.Nodes.Clear();
			foreach(Contact x in roster.Values)
			{
				ContactTreeNode ctn = new ContactTreeNode(x);
				string text = x.Name+" (";
				
				if(x.Resources.Count > 0)
				{
					for(int i=0; i<x.Resources.Count-1; i++)
						text += x.Resources[i] + ", ";
					text += x.Resources[x.Resources.Count-1]+")";
				}
				else
				{
					text += "Offline)";
				}

				ctn.Text = text;
				rosterView.Nodes.Add(ctn);
			}
		}
		
		// Generates a new ID for a thread
		private string newThreadId()
		{
			char[] hexchars = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f' };
			
			Random r = new Random();
			char[] chars = new char[40];
			for(int i=0; i<chars.Length; i++)
				chars[i] = hexchars[r.Next(15)];
			return new string(chars);
		}

		private void menuItem5_Click(object sender, System.EventArgs e)
		{
			refreshRosterView();
		}

		private void menuItem1_Click(object sender, System.EventArgs e)
		{
		
		}

		
		
		private class ContactTreeNode : TreeNode
		{
			private Contact contact;

			public ContactTreeNode(Contact contact)
			{
				this.Contact = contact;
			}

			public Contact Contact
			{
				get { return this.contact; }
				set { this.contact = value; }
			}
		}

	}
}
