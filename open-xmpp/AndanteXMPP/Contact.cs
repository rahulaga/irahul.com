using System;
using System.Collections;

namespace AndanteXMPP
{
	/// <summary>
	/// Summary description for Contact.
	/// </summary>
	public class Contact
	{
		private string name;
		private string baseJabberId;
		private ArrayList resources;

		public Contact()
		{
			resources = new ArrayList();
		}

		public string Name
		{
			get { return this.name; }
			set { this.name = value; }
		}

		public string BaseJabberId
		{
			get { return this.baseJabberId; }
			set { this.baseJabberId = value; }
		}

		public ArrayList Resources
		{
			get { return this.resources; }
		}

		public bool Online
		{
			get { return this.resources.Count > 0; }
		}

//		public string JabberId
//		{
//			get
//			{
//				return baseJabberId+"/"+(resources.Count > 0 ? resources[0] : "");
//			}
//		}

		public string[] AllJabberId
		{
			get 
			{
				string[] ids = new string[resources.Count];
				for(int i=0; i<ids.Length; i++)
					ids[i] = baseJabberId+"/"+resources[i];
				return ids;
			}
		}
	}
}
