using System;
using System.Xml;
using System.IO;
using System.Text;
using System.Collections;
using System.Security.Cryptography;

namespace openXMPP
{
	/// <summary>
	/// Creates stream elements.
	/// </summary>
	public class StreamElementFactory
	{

		/// <summary>
		/// The namespace of all TLS authentication elements.
		/// </summary>
		public const string TLS_NAMESPACE = "urn:ietf:params:xml:ns:xmpp-tls";

		/// <summary>
		/// The namespace of all SASL authentication elements.
		/// </summary>
        public const string SASL_NAMESPACE = "urn:ietf:params:xml:ns:xmpp-sasl";

		/// <summary>
		/// The namespace of stream:error elements.
		/// </summary>
		public const string STREAM_ELEMENT_NAMESPACE = "urn:ietf:params:xml:ns:xmpp-streams";

		/// <summary>
		/// The namespace of all jabber client elements.
		/// </summary>
		public const string JABBER_CLIENT_NAMESPACE = "jabber:client";

		/// <summary>
		/// Needed for MD5-DIGEST encryption salt.
		/// </summary>
		private static Random rand = new Random();

		/// <summary>
		/// This class cannot be instantiated
		/// </summary>
		private StreamElementFactory()
		{ }
		
		
		/// <summary>
		/// Returns the most appropriate subclass of StreamElement for the given XML
		/// </summary>
		/// <param name="xml">XML representation of some stream element</param>
		/// <returns>A subclass of StreamElement or new instance of StreamElement of no subclass was an exact match.</returns>
		public static StreamElement GetStreamElement(XmlNode xml)
		{
			StreamElement e = null;

			switch(xml.Name)
			{
				case "stream:features":
					e = new StreamFeaturesElement(xml);
					break;
				case "auth":
					e = new AuthenticationElement(xml);
					break;
				case "success":
					e = new ResponseElement(xml, ResponseType.Success);
					break;
				case "failure":
					e = new ResponseElement(xml, ResponseType.Failure);
					break;
				case "stream:error":
					e = new ResponseElement(xml, ResponseType.StreamError);
					break;
				case "starttls":
					e = new ResponseElement(xml, ResponseType.StartTls);
					break;
				case "proceed":
					e = new ResponseElement(xml, ResponseType.ProceedTls);
					break;
				case "challenge": 
				case "response":
					e = new SaslResponseElement(xml);
					break;
				default:
					e = new StreamElement(xml);
					break;
			}

			return e;
		}

		
		/// <summary>
		/// Creates a new stream element for user authentication.
		/// </summary>
		/// <param name="mech">The SASL authentication mechanism to use.</param>
		/// <param name="domain">The user's authentication domain (for PLAIN authentication)</param>
		/// <param name="username">The user's name (for PLAIN authentication)</param>
		/// <param name="password">The user's password (for PLAIN authentication)</param>
		/// <returns>A new authorization stanza for the given SASL authentication mechanism.</returns>
		public static AuthenticationElement GetAuthenticationElement(SaslAuthenticationMechanism mech, string domain, string username, string password)
		{
			StringWriter xmlString = new StringWriter();
			XmlTextWriter writer = new XmlTextWriter(xmlString);

			writer.WriteStartElement("auth");
			writer.WriteAttributeString("xmlns", SASL_NAMESPACE);

			switch(mech)
			{
				case SaslAuthenticationMechanism.DIGEST_MD5:
					writer.WriteAttributeString("mechanism", "DIGEST-MD5");
					break;
				case SaslAuthenticationMechanism.PLAIN:
					writer.WriteAttributeString("mechanism", "PLAIN");
					string authStr = string.Format("{0}@{1}\x00{2}\x00{3}", username, domain, username, password);
					byte[] bytes = Encoding.UTF8.GetBytes(authStr);
					writer.WriteBase64(bytes, 0, bytes.Length);
					break;
				default:
					throw new OpenXMPPException("Cannot create authorization stanza for mechanism "+mech+".");
			}
			writer.WriteEndElement();

			XmlDocument xml = new XmlDocument();
			xml.LoadXml(xmlString.ToString());

			return new AuthenticationElement(xml.DocumentElement, mech, domain, username, password);
		}

		
		/// <summary>
		/// Gets a new starttls element.  Used to initiate TLS security.
		/// </summary>
		/// <returns>A new starttls element.</returns>
		public static ResponseElement GetStartTlsElement()
		{
			StringWriter xmlString = new StringWriter();
			XmlTextWriter writer = new XmlTextWriter(xmlString);

			writer.WriteStartElement("starttls");
			writer.WriteAttributeString("xmlns", TLS_NAMESPACE);
			writer.WriteEndElement();

			XmlDocument xml = new XmlDocument();
			xml.LoadXml(xmlString.ToString());

			return new ResponseElement(xml.DocumentElement);
		}


		/// <summary>
		/// Gets a stream:error element for the specified error and diagnostic text
		/// </summary>
		/// <param name="type">The type of error as listed in RFC 3920.</param>
		/// <returns>A new StreamErrorElement for the specified error</returns>
		public static StreamErrorElement GetStreamErrorElement(StreamErrorType type)
		{
			StringWriter xmlString = new StringWriter();
			XmlTextWriter writer = new XmlTextWriter(xmlString);

			writer.WriteStartElement("stream:error");
			writer.WriteAttributeString("xmlns:stream", Session.STREAM_NAMESPACE);
			
			string errorStr = null;
			switch(type)
			{
				case StreamErrorType.UnsupportedVersion:
					errorStr = "unsupported-version";
					break;
					//TODO: ... Lots of cases ...
			}

			writer.WriteStartElement(errorStr, STREAM_ELEMENT_NAMESPACE);
			writer.WriteEndElement();

			writer.WriteEndElement();

			XmlDocument xml = new XmlDocument();
			xml.LoadXml(xmlString.ToString());

			return new StreamErrorElement(xml.DocumentElement);
		}

	
		/// <summary>
		/// Gets a stream:error element for the specified error and diagnostic text
		/// </summary>
		/// <param name="type">The type of error as listed in RFC 3920.</param>
		/// <param name="text">The diagnostic text associated with the error.</param>
		/// <returns>A new StreamErrorElement for the specified error</returns>
		public static StreamErrorElement GetStreamErrorElement(StreamErrorType type, string text)
		{
			StringWriter xmlString = new StringWriter();
			XmlTextWriter writer = new XmlTextWriter(xmlString);

			writer.WriteStartElement("stream:error");
			writer.WriteAttributeString("xmlns:stream", Session.STREAM_NAMESPACE);
			
			string errorStr = null;
			switch(type)
			{
				case StreamErrorType.UnsupportedVersion:
					errorStr = "unsupported-version";
					break;
				//TODO: ... Lots of cases ...
			}

			writer.WriteStartElement(errorStr, STREAM_ELEMENT_NAMESPACE);
			writer.WriteEndElement();

			writer.WriteStartElement("text", STREAM_ELEMENT_NAMESPACE);
			writer.WriteString(text);
			writer.WriteEndElement();

			writer.WriteEndElement();

			XmlDocument xml = new XmlDocument();
			xml.LoadXml(xmlString.ToString());

			return new StreamErrorElement(xml.DocumentElement);
		}
		
		
		/// <summary>
		/// Creates a challenge element for SASL DIGEST-MD5 authentication
		/// </summary>
		/// <param name="data">The SASL challenge key/value pairs</param>
		/// <returns>A new SASL challenge element</returns>
		public static SaslResponseElement GetSaslChallengeElement(Hashtable data)
		{
			StringWriter xmlString = new StringWriter();
			XmlTextWriter writer = new XmlTextWriter(xmlString);

			writer.WriteStartElement("challenge");
			writer.WriteAttributeString("xmlns",  SASL_NAMESPACE);

			string responseStr = buildSaslDataString(data);
			byte[] bytes = Encoding.UTF8.GetBytes(responseStr);
			writer.WriteBase64(bytes, 0, bytes.Length);
			writer.WriteEndElement();

			XmlDocument xml = new XmlDocument();
			xml.LoadXml(xmlString.ToString());

			return new SaslResponseElement(xml.DocumentElement, data);
		}

		
		/// <summary>
		/// Creates a response element for SASL DIGEST-MD5 authentication
		/// </summary>
		/// <param name="data">The SASL response key/value pairs</param>
		/// <returns>A new SASL response element</returns>
		public static SaslResponseElement GetSaslResponseElement(string username, string password, string realm, string nonce, string qop)
		{
			StringWriter xmlString = new StringWriter();
			XmlTextWriter writer = new XmlTextWriter(xmlString);

			writer.WriteStartElement("response");
			writer.WriteAttributeString("xmlns",  SASL_NAMESPACE);

			Hashtable data = new Hashtable();
			data["username"] = username;
			data["realm"] = realm;
			data["nonce"] = nonce;
			data["qop"] = qop;
			data["nc"] = "00000001";
			data["digest-uri"] = "xmpp/"+realm;
			data["charset"] = "utf-8";

			calcSaslMD5Response(data, password);

			string responseStr = buildSaslDataString(data);
			byte[] bytes = Encoding.UTF8.GetBytes(responseStr);
			writer.WriteBase64(bytes, 0, bytes.Length);
			writer.WriteEndElement();

			XmlDocument xml = new XmlDocument();
			xml.LoadXml(xmlString.ToString());

			return new SaslResponseElement(xml.DocumentElement, data);
		}

		
		/// <summary>
		/// Gets an empty SASL response stream element.
		/// Used to confirm that handshake was good.
		/// </summary>
		/// <returns>An empty response element.</returns>
		public static SaslResponseElement GetSaslResponseElement()
		{
			StringWriter xmlString = new StringWriter();
			XmlTextWriter writer = new XmlTextWriter(xmlString);

			writer.WriteStartElement("response");
			writer.WriteAttributeString("xmlns", SASL_NAMESPACE);
			writer.WriteEndElement();

			XmlDocument xml = new XmlDocument();
			xml.LoadXml(xmlString.ToString());

			return new SaslResponseElement(xml.DocumentElement);
		}

		
		/// <summary>
		/// Returns a new success element.
		/// Used to indicate successful authentication, etc.
		/// </summary>
		/// <returns>A new success element.</returns>
		public static ResponseElement GetSaslSuccessElement()
		{
			StringWriter xmlString = new StringWriter();
			XmlTextWriter writer = new XmlTextWriter(xmlString);

			writer.WriteStartElement("success");
			writer.WriteAttributeString("xmlns", SASL_NAMESPACE);
			writer.WriteEndElement();

			XmlDocument xml = new XmlDocument();
			xml.LoadXml(xmlString.ToString());

			return new ResponseElement(xml.DocumentElement, ResponseType.Success);
		}

		
		/// <summary>
		/// Returns a new failure element.
		/// Used to indicate failed authentication, etc.
		/// </summary>
		/// <returns>A new failure element.</returns>
		public static ResponseElement GetSaslFailureElement()
		{
			StringWriter xmlString = new StringWriter();
			XmlTextWriter writer = new XmlTextWriter(xmlString);

			writer.WriteStartElement("failure");
			writer.WriteAttributeString("xmlns", SASL_NAMESPACE);
			writer.WriteEndElement();

			XmlDocument xml = new XmlDocument();
			xml.LoadXml(xmlString.ToString());

			return new ResponseElement(xml.DocumentElement, ResponseType.Failure);
		}

			
		/// <summary>
		/// Converts a hashtable of SASL authentication key/value pairs into a string.
		/// </summary>
		/// <param name="data">Key/value pairs to convert</param>
		/// <returns>A new string containing the key/value pairs suitable for sending to an authentication server.</returns>
		private static string buildSaslDataString(Hashtable data)
		{
			StringBuilder sb = new StringBuilder();
			if (data.Count > 0)
			{
				foreach (string key in data.Keys)
				{
					sb.Append(key);
					sb.Append("=");
					switch(key)
					{
						case "nc":
						case "qop":
						case "response":
						case "charset":
						case "algorithm":
							sb.Append(data[key]); 
							break;
						default:
							sb.Append("\""+data[key]+"\""); 
							break;
					}
					sb.Append(",");
				}
			}

			sb.Length = sb.Length-1; //remove last comma
			return sb.ToString();
		}

		
		/// <summary>
		/// Given a partially complete hastable of required SASL data, produces the needed
		/// cnonce and response data and stores it back in the hashtable.
		/// </summary>
		/// <param name="data">Partially filled data table.  Must have username, realm, nonce, qop, nc, digest-uri, and charset specified.  Supplies cnonce and response."</param>
		/// <param name="password">The user's password.  Needed for cnonce.</param>
		private static void calcSaslMD5Response(Hashtable data, string password)
		{
			MD5CryptoServiceProvider MD5 = new MD5CryptoServiceProvider();
			byte[] H1, H2, H3;
			string s, A1, A2, A3, sH1, sH2;

			s = string.Format(null, "{0}:{1}:{2}", rand.Next(1024), data["username"], password);
			data["cnonce"] = toHexString(Encoding.ASCII.GetBytes(s)).ToLower();
			
			s = string.Format(null, "{0}:{1}:{2}", data["username"], data["realm"], password);
			H1 = MD5.ComputeHash(Encoding.ASCII.GetBytes(s));

			A1 = ":"+data["nonce"]+":"+data["cnonce"];

			MemoryStream ms = new MemoryStream();
			ms.Write(H1, 0, 16);

			byte[] bytes = Encoding.ASCII.GetBytes(A1);
			ms.Write(bytes, 0, bytes.Length);
			ms.Seek(0, System.IO.SeekOrigin.Begin);
			H1 = MD5.ComputeHash(ms);
            
			A2 = "AUTHENTICATE:" + data["digest-uri"];
			if(string.Compare(data["qop"] as string, "auth") != 0)
				A2 += ":00000000000000000000000000000000";

			H2 = MD5.ComputeHash(Encoding.ASCII.GetBytes(A2));
            
			sH1 = toHexString(H1).ToLower();
			sH2 = toHexString(H2).ToLower();
           
			A3 = string.Format(null, "{0}:{1}:{2}:{3}:{4}:{5}", sH1, data["nonce"],data["nc"], data["cnonce"], data["qop"], sH2);
			H3 = MD5.ComputeHash(Encoding.ASCII.GetBytes(A3));
			data["response"] = toHexString(H3).ToLower();
		}

	
		/// <summary>
		/// Produces a hexidecimal string for some given bytes.
		/// </summary>
		/// <param name="buf">Bytes to represent in hex string.</param>
		/// <returns>Hex string representation of the bytes</returns>
		private static string toHexString(byte[] buf)
		{
			StringBuilder sb = new StringBuilder();
			foreach (byte b in buf)
			{
				sb.Append(b.ToString("x2"));
			}
			return sb.ToString();            
		}
	}
}
