using System;
using System.Xml;

namespace openXMPP
{
	/// <summary>
	/// Base class of all XMPP stream elements.
	/// These are things like "auth", "challenge", "response", etc.
	/// </summary>
	public class StreamElement : XmlProtocolElement
	{
		public StreamElement(XmlNode xml) : base(xml)
		{ }
	}
}
