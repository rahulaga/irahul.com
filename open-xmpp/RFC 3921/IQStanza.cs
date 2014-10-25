using System;
using System.Xml;

namespace openXMPP
{

	/// <summary>
	/// Summary description for IQStanza.
	/// </summary>
	public class IQStanza : Stanza
	{

//		private bool requiresResponse;

		public IQStanza(XmlNode xml) : base(xml)
		{ }

//		public IQStanza(XmlNode xml, bool requiresResponse) : base(xml)
//		{
//			this.requiresResponse = requiresResponse;
//		}

		public XmlNode Query
		{
			get { return this.internalXml.FirstChild; }
		}

		/// <summary>
		/// If true, the session will block until a response with matching ID is received
		/// </summary>
//		public bool RequiresResponse
//		{
//			get { return requiresResponse; }
//			set { this.requiresResponse = value; }
//		}
	}
}
