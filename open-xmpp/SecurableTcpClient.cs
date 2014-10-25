using System;

namespace openXMPP
{
	/// <summary>
	/// Summary description for TcpClient.
	/// </summary>
	public abstract class SecurableTcpClient
	{
		// DNS name of host we are connected to
		protected string hostname;
		// Port we are connected on
		protected int port;
		// True if this connection is secured
		protected bool secured;
		// True if the stream is using some kind of encryption (usually SSL/TLS)
		protected bool isSecure;
		// True if the stream is in the process of changing its security algorithm
		protected bool isSecurityChanging;
		// The byte-level network transport
		protected System.IO.Stream stream;

		/// <summary>
		/// Raised when the server-side certificate has been verified.
		/// The client can safely assume that the stream is usable if this event is raised.
		/// </summary>
		public event System.EventHandler OnCertificateVerified;

		/// <summary>
		/// Override to implement stream security.
		/// Set to true to enable, false to disable.
		/// </summary>
		public abstract bool IsSecure { get; set; }
		
		/// <summary>
		/// Opens the stream.
		/// </summary>
		public abstract void Open();

		/// <summary>
		/// Closes the stream
		/// </summary>
		public abstract void Close();

		protected SecurableTcpClient(string hostname, int port, bool secured)
		{
			this.hostname = hostname;
			this.port = port;
			this.secured = secured;
		}
		
		/// <summary>
		/// The byte-level network transport.
		/// </summary>
		public System.IO.Stream Stream
		{
			get { return this.stream; }
		}

	
		/// <summary>
		/// True if thie stream is in the process of changing its security level.
		/// </summary>
		public bool IsSecurityChanging
		{
			get { return isSecurityChanging; }
		}

		public string Hostname
		{
			get { return this.hostname; }
		}

		public int Port
		{
			get { return this.port; }
		}

		/// <summary>
		/// Raises a new OnCertificateVerified event
		/// </summary>
		/// <param name="e">Arguments to pass to the OnCertificateVerified event.</param>
		protected void raiseCertificateVerifiedEvent(EventArgs e)
		{
			if(OnCertificateVerified != null) OnCertificateVerified(this, e);
		}


	}
}
