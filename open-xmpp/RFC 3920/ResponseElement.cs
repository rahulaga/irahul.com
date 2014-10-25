using System;
using System.Xml;

namespace openXMPP
{

	/// <summary>
	/// Types of responses
	/// </summary>
	public enum ResponseType
	{
		Success,
		Failure,
		StreamError,
		SaslChallenge,
		SaslResponse,
		StartTls,
		ProceedTls,
		Unknown
	};

	/// <summary>
	/// Represents the various responses a client can send or receive.
	/// </summary>
	public class ResponseElement : StreamElement
	{
		// The type of response
		protected ResponseType type;

		/// <summary>
		/// Creates a new response object with the given XML representation.
		/// </summary>
		/// <param name="xml">XML representation of this response</param>
		public ResponseElement(XmlNode xml) : base(xml)
		{ 
			switch(xml.Name)
			{
				case "success":
					type = ResponseType.Success;
					break;
				case "failure":
					type = ResponseType.Failure;
					break;
				case "stream:error":
					type = ResponseType.StreamError;
					break;
				case "starttls":
					type = ResponseType.StartTls;
					break;
				case "proceed":
					type = ResponseType.ProceedTls;
					break;
				case "challenge":
					type = ResponseType.SaslChallenge;
					break;
				case "response":
					type = ResponseType.SaslResponse;
					break;
				default:
					throw new OpenXMPPException("An unknown response element was received.");
			}
		}

		/// <summary>
		/// Creates a new response object with the given XML representation and type.
		/// No error checking is done to verify that the type is appropriate for the XML.
		/// </summary>
		/// <param name="xml"></param>
		/// <param name="type"></param>
		public ResponseElement(XmlNode xml, ResponseType type) : base(xml)
		{
			this.type = type;
		}

		/// <summary>
		/// Gets the type of this response.
		/// </summary>
		public ResponseType Type
		{
			get { return type; }
		}
	}
}
