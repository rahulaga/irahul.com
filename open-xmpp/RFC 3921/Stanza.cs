using System;
using System.Diagnostics;
using System.Xml;
using System.IO;

namespace openXMPP
{
	/// <summary>
	/// Base class for all stanza elements.
	/// RFC 3920 defines a "stanza" as an XML session element with the name "message", "presence", or "iq"
	/// </summary>
	public class Stanza : XmlProtocolElement
	{
		/// <summary>
		/// Constructor.
		/// Creates a new stanza with the given XML representation
		/// </summary>
		/// <param name="xml">The XML representation of this stanza</param>
		public Stanza(XmlNode xml) : base(xml)
		{ }

		public string To
		{
			get 
			{
				XmlAttribute a = InternalXml.Attributes["to"];
				return (a == null ? null : a.Value); 
			}
		}

		public string From
		{
			get 
			{
				XmlAttribute a = InternalXml.Attributes["from"];
				return (a == null ? null : a.Value); 
			}
		}

		public string ID
		{
			get 
			{
				XmlAttribute a = InternalXml.Attributes["id"];
				return (a == null ? null : a.Value); 
			}
		}

		public string Type
		{
			get 
			{
				XmlAttribute a = InternalXml.Attributes["type"];
				return (a == null ? null : a.Value); 
			}
		}

		public string Language
		{
			get 
			{
				XmlAttribute a = InternalXml.Attributes["lang"];
				return (a == null ? "en" : a.Value); 
			}
		}

		public string Namespace
		{
			get { 
				return this.internalXml.NamespaceURI;
			}
		}
	}
}
