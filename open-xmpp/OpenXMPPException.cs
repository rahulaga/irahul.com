using System;

namespace openXMPP
{
	/// <summary>
	/// Generic exception class for openXMPP.
	/// </summary>
	public class OpenXMPPException : Exception
	{
		public OpenXMPPException()
		{ }

		public OpenXMPPException(string message) : base(message)
		{
			
		}
	}
}
