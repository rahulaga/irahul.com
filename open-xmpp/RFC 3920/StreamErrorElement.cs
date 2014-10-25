using System;
using System.Xml;

namespace openXMPP
{

	/// <summary>
	/// Types of stream errors
	/// </summary>
	public enum StreamErrorType
	{
		BadFormat,
		BadNamespacePrefix,
		Conflict,
		ConnectionTimeout,
		HostGone,
		HostUnknown,
		ImproperAddressing,
		InternalServerError,
		InvalidFrom,
		InvalidId,
		InvalidNamespace,
		InvalidXml,
		NotAuthorized,
		PolicyViolation,
		RemoteConnectionFailed,
		ResourceConstraint,
		RestrictedXml,
		SeeOtherHost,
		SystemShutdown,
		UndefinedCondition,
		UnsupportedEncoding,
		UnsupportedStanzaType,
		UnsupportedVersion,
		XmlNotWellFormed
	};


	/// <summary>
	/// Summary description for StreamErrorElement.
	/// </summary>
	public class StreamErrorElement : ResponseElement
	{
		// The tyoe of stream error
		protected StreamErrorType errorType;

		public StreamErrorElement(XmlNode xml) : base(xml)
		{ }

		public StreamErrorElement(XmlNode xml, StreamErrorType errorType) : base(xml)
		{
			this.errorType = errorType;
		}

		/// <summary>
		/// Gets the type of this stream error.
		/// </summary>
		public StreamErrorType ErrorType
		{
			get { return errorType; }
		}

	}
}
