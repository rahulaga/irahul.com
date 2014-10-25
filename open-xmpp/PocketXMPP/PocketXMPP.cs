using System;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;
using System.Data;

namespace PocketXMPP
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class Form1 : System.Windows.Forms.Form
	{
		private System.Windows.Forms.MainMenu mainMenu;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem menuItem2;
		private System.Windows.Forms.MenuItem menuItem3;
		private System.Windows.Forms.MenuItem menuItem4;
		private System.Windows.Forms.MenuItem menuItem5;
		private System.Windows.Forms.MenuItem menuItem6;
		private System.Windows.Forms.MenuItem menuItem7;
		private System.Windows.Forms.MenuItem menuItem8;
		private System.Windows.Forms.MenuItem menuItem9;
		private System.Windows.Forms.MenuItem menuItem10;

		public Form1()
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
			base.Dispose( disposing );
		}
		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(Form1));
			this.mainMenu = new System.Windows.Forms.MainMenu();
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.menuItem2 = new System.Windows.Forms.MenuItem();
			this.menuItem3 = new System.Windows.Forms.MenuItem();
			this.menuItem4 = new System.Windows.Forms.MenuItem();
			this.menuItem5 = new System.Windows.Forms.MenuItem();
			this.menuItem6 = new System.Windows.Forms.MenuItem();
			this.menuItem7 = new System.Windows.Forms.MenuItem();
			this.menuItem8 = new System.Windows.Forms.MenuItem();
			this.menuItem9 = new System.Windows.Forms.MenuItem();
			this.menuItem10 = new System.Windows.Forms.MenuItem();
			// 
			// mainMenu
			// 
			this.mainMenu.MenuItems.Add(this.menuItem1);
			this.mainMenu.MenuItems.Add(this.menuItem8);
			this.mainMenu.MenuItems.Add(this.menuItem2);
			this.mainMenu.MenuItems.Add(this.menuItem3);
			// 
			// menuItem1
			// 
			this.menuItem1.MenuItems.Add(this.menuItem5);
			this.menuItem1.MenuItems.Add(this.menuItem6);
			this.menuItem1.MenuItems.Add(this.menuItem7);
			this.menuItem1.Text = "Tools";
			// 
			// menuItem2
			// 
			this.menuItem2.Text = "Chats";
			// 
			// menuItem3
			// 
			this.menuItem3.MenuItems.Add(this.menuItem4);
			this.menuItem3.Text = "Help";
			// 
			// menuItem4
			// 
			this.menuItem4.Text = "About";
			// 
			// menuItem5
			// 
			this.menuItem5.Text = "Sign In/Sign Out";
			// 
			// menuItem6
			// 
			this.menuItem6.Text = "Options...";
			// 
			// menuItem7
			// 
			this.menuItem7.Text = "Exit";
			// 
			// menuItem8
			// 
			this.menuItem8.MenuItems.Add(this.menuItem9);
			this.menuItem8.MenuItems.Add(this.menuItem10);
			this.menuItem8.Text = "Roster";
			// 
			// menuItem9
			// 
			this.menuItem9.Text = "Add...";
			// 
			// menuItem10
			// 
			this.menuItem10.Text = "Remove...";
			// 
			// Form1
			// 
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Menu = this.mainMenu;
			this.Text = "Pocket XMPP";

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>

		static void Main() 
		{
			Application.Run(new Form1());
		}
	}
}
