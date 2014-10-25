using System;
using System.Xml;
using System.Text;

namespace openXMPP
{
	/// <summary>
	/// The various SASL authentication mechanisms we can use
	/// to authenticate a user to a server.
	/// </summary>
	public enum SaslAuthenticationMechanism
	{
		DIGEST_MD5,
		PLAIN,
		UNKNOWN
	};

	/// <summary>
	/// The "auth" element used to specify the mechanism
	/// for authenticating a client.
	/// </summary>
	public class AuthenticationElement : StreamElement
	{
		protected SaslAuthenticationMechanism mechanism;
		protected string domain;
		protected string username;
		protected string password;

		#region Constructors

		/// <summary>
		/// Constructor.
		/// Builds a new "auth" element with the given XML representation
		/// </summary>
		/// <param name="xml">The XML representation of this element</param>
		public AuthenticationElement(XmlNode xml) : base(xml)
		{
			if(xml.FirstChild.Attributes["mechanism"] == null)
				throw new OpenXMPPException("The server is not sending standard SASL authentication elements!  I cann't determine an authentication mechanism");

			// Parse authorization mechanism from XML
			switch(xml.FirstChild.Attributes["mechanism"].Value)
			{
				case "DIGEST-MD5":
					this.mechanism = SaslAuthenticationMechanism.DIGEST_MD5;
					break;
				case "PLAIN":
					this.mechanism = SaslAuthenticationMechanism.PLAIN;
					break;
				default:
					this.mechanism = SaslAuthenticationMechanism.UNKNOWN;
					break;
			}

			// Parse encoded username and password from XML
			string authStr = Encoding.ASCII.GetString(Convert.FromBase64String(xml.InnerText));
			string[] authParams = authStr.Split('@', '\x00');
			this.domain = authParams[1];
			this.username = authParams[2];
			this.password = authParams[3];
		}

		/// <summary>
		/// Constructor.
		/// Builds a new AuthorizationStanza object with the given xml.  Additional parameters are to avoid parsing overhead.
		/// </summary>
		/// <param name="xml">XML representation of this stanza.</param>
		/// <param name="mechanism">The SASL authorization mechanism specified in this stanza.</param>
		/// <param name="domain">The user's authorization domain.</param>
		/// <param name="username">The user's name.</param>
		/// <param name="password">The user's password.</param>
		public AuthenticationElement(XmlNode xml, SaslAuthenticationMechanism mechanism, string domain, string username, string password) : base(xml)
		{
			this.mechanism = mechanism;
			this.domain = domain;
			this.username = username;
			this.password = password;
		}

		#endregion

		#region Properties

		/// <summary>
		/// The SASL authentication mechanism used in this stanza.
		/// </summary>
		public SaslAuthenticationMechanism Mechanism
		{
			get { return this.mechanism; }
		}

		/// <summary>
		/// The user's authorization domain.
		/// </summary>
		public string Domain
		{
			get { return this.domain; }
		}

		/// <summary>
		/// The user's name.
		/// </summary>
		public string Username
		{
			get { return this.username; }
		}

		/// <summary>
		/// The user's password.
		/// </summary>
		public string Password
		{
			get { return this.password; }
		}

		#endregion

	}
}
