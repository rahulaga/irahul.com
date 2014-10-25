using System;
using System.Xml;
using System.Text;
using System.Collections;

namespace openXMPP
{

	/// <summary>
	/// Responses specific to SASL authentication.
	/// </summary>
	public class SaslResponseElement : ResponseElement
	{
		/// <summary>
		/// The key/value pairs needed for SASL authentication.
		/// Typically keys are: username, password, realm, nonce, cnonce, qop, nc, digest-uri, charset, response
		/// </summary>
		protected Hashtable data = new Hashtable();

		/// <summary>
		/// Creates a new SASL response element with the given XML representation.
		/// </summary>
		/// <param name="xml">XML representation of the sasl response.</param>
		public SaslResponseElement(XmlNode xml) : base(xml)
		{
			if (xml.Attributes["xmlns"].Value != StreamElementFactory.SASL_NAMESPACE) 
				throw new OpenXMPPException("Incorrect challenge namespace.");

			byte[] bytes = Convert.FromBase64String(xml.InnerText);
			string challengeStr = Encoding.UTF8.GetString(bytes);
			
			string[] pairs = challengeStr.Split(',');
			foreach (string pair in pairs) 
			{
				string[] entry = pair.Split('=');
				if (entry.Length > 1)
				{
					string key = entry[0];
					string val = entry[1];
					switch (key) 
					{
						case "nc":
						case "response":
						case "charset":
						case "algorithm":
						case "rspauth":
							break; //do nothing
						default:
							val = val.Substring(1, val.Length-2); //remove double-quotes
							break;
					}
					data[key] = val;										
				}
			}
		}

		/// <summary>
		/// Creates a new SASL response element with the given XML representation and data.
		/// No error checking is done to verify that the data is appropriate for the XML.
		/// </summary>
		/// <param name="xml"></param>
		/// <param name="data"></param>
		public SaslResponseElement(XmlNode xml, Hashtable data) : base(xml)
		{
			this.data = data;
		}

		/// <summary>
		/// Gets the SASL authentication data.
		/// </summary>
		public Hashtable Data
		{
			get { return data; }
		}

		public override string ToString()
		{
			StringBuilder strBuff = new StringBuilder(base.ToString()+"\n");
			foreach(string key in Data.Keys)
			{
				strBuff.Append(key+": "+data[key]+"; ");
			}

			return strBuff.ToString();
		}
	}
}
