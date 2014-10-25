using System;
using System.Xml;
using System.IO;
using System.Diagnostics;

namespace openXMPP
{
	/// <summary>
	/// Base class of all XMPP xml elements.
	/// </summary>
	public class XmlProtocolElement
	{
		/// <summary>
		/// The XML describing this stanza
		/// </summary>
		//protected XmlNode internalXml;
		protected XmlNode internalXml;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="xml"></param>
		public XmlProtocolElement(XmlNode xml) 
		{
			this.internalXml = xml;
		}

		public string Name
		{
			get { return internalXml.Name; }
		}

		public bool IsStanza
		{
			get
			{
				return internalXml.Name == "message" || internalXml.Name == "presence" || internalXml.Name == "iq";
			}
		}

		/// <summary>
		/// Internal representation of this stanza
		/// </summary>
		public XmlNode InternalXml
		{
			get { return internalXml; }
		}

		/// <summary>
		/// ToString convinence method.
		/// </summary>
		/// <returns>The XML representation of this stanza</returns>
		public override String ToString() 
		{
			return internalXml.OuterXml;
		}
	}
}
