using System;
using System.Diagnostics;
using System.Xml;

namespace openXMPP
{

	public enum StreamFeaturesType
	{
		SaslMechanisms,		// List of SASL authentication mechanisms
		StartTls,			// START_TLS command
		Bind,				// Binding
		Session				// Ready to open a new session
	};

	/// <summary>
	/// "stream:features" stream element.
	/// Used to specify authentication mechanisms and TLS support.
	/// </summary>
	public class StreamFeaturesElement : StreamElement
	{

		private StreamFeaturesType type;

		public StreamFeaturesElement(XmlNode xml) : base(xml)
		{ 
			if(xml.FirstChild == null)
				throw new OpenXMPPException("The server is not sending standard stream feature elements.");

			switch(xml.FirstChild.Name)
			{
				case "mechanisms":
					this.type = StreamFeaturesType.SaslMechanisms;
					break;
				case "starttls":
					this.type = StreamFeaturesType.StartTls;
					break;
				case "bind":
					this.type = StreamFeaturesType.Bind;
					break;
				case "session":
					this.type = StreamFeaturesType.Session;
					break;
			}
		}

		public StreamFeaturesElement(XmlNode xml, StreamFeaturesType type) : base(xml)
		{
			this.type = type;
		}

		/// <summary>
		/// Gets the type of stream features.
		/// </summary>
		public StreamFeaturesType Type
		{
			get { return this.type; }
		}

		/// <summary>
		/// Is true if this element contains a DIGEST-MD5 element.
		/// </summary>
		public bool SaslHasDigestMD5
		{
			get 
			{ 
				foreach(XmlNode child in internalXml.FirstChild.ChildNodes) 
					if(child.InnerText.ToUpper() == "DIGEST-MD5") return true;
				return false; 
			}
		}

		/// <summary>
		/// Is true if this element contains a PLAIN element.
		/// </summary>
		public bool SaslHasPlain
		{
			get 
			{ 
				foreach(XmlNode child in internalXml.FirstChild.ChildNodes)
					if(child.InnerText.ToUpper() == "PLAIN") return true;
				return false; 
			}
		}

		/// <summary>
		/// Returns the prefered SASL authentication mechanism.
		/// If DIGEST-MD5 is available, then this is used.
		/// Otherwise PLAIN is used.
		/// 
		/// If neither DIGEST-MD5 nor PLAIN is available, use an UNKNOWN (e.g. application defined) mechanism.
		/// </summary>
		public SaslAuthenticationMechanism BestSaslAuthMech
		{
			get 
			{
				if(SaslHasDigestMD5) return SaslAuthenticationMechanism.DIGEST_MD5;
				if(SaslHasPlain) return SaslAuthenticationMechanism.PLAIN;
				return SaslAuthenticationMechanism.UNKNOWN;
			}
		}
	}
}
