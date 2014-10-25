using System;
using System.IO;
using Org.Mentalis.Security.Ssl;
using Org.Mentalis.Security.Certificates;

namespace openXMPP
{
	/// <summary>
	/// Summary description for MentalisTcpClient.
	/// </summary>
	public class MentalisTcpClient : openXMPP.SecurableTcpClient
	{
		/// <summary>
		/// Creats a new TcpClient based on Org.Mentalis.Security.Ssl.SecureTcpClient
		/// initialized with TLS not used (plain stream)
		/// </summary>
		/// <param name="hostname">The host to connect to</param>
		/// <param name="port">The port to connect on</param>
		public MentalisTcpClient(string hostname, int port) : this(hostname, port, false)
		{ }

	
		/// <summary>
		/// Creats a new TcpClient based on Org.Mentalis.Security.Ssl.SecureTcpClient
		/// </summary>
		/// <param name="hostname">he host to connect to</param>
		/// <param name="port">The port to connect on</param>
		/// <param name="secured">Indicate if TLS should be enabled by default</param>
		public MentalisTcpClient(string hostname, int port, bool secured) : base(hostname, port, secured)
		{ }
	
		
		/// <summary>
		/// Opens the stream
		/// </summary>
		public override void Open()
		{
			SecurityOptions opts = null;
			if(secured)
			{
				opts = new SecurityOptions(SecureProtocol.Tls1);
				opts.Protocol = SecureProtocol.Tls1;
				opts.Certificate = null;
				opts.AllowedAlgorithms = SslAlgorithms.SECURE_CIPHERS;
				opts.VerificationType = CredentialVerification.Manual;
				opts.Verifier = new CertVerifyEventHandler(stream_OnCertVerify);
				opts.Flags = SecurityFlags.Default;
			}
			else 
			{
				opts = new SecurityOptions(SecureProtocol.None);
			}
			SecureTcpClient cli = new SecureTcpClient(hostname, port, opts);
			stream = cli.GetStream();
		}

		
		/// <summary>
		/// Closes the stream
		/// </summary>
		public override void Close()
		{
			stream.Close();
		}


		/// <summary>
		/// Enables or disables TLS security on the stream.
		/// </summary>
		/// <param name="enabled">True to enable TLS</param>
		public override bool IsSecure
		{
			get
			{
				return this.isSecure;
			}
			set
			{
				isSecurityChanging = true;
				lock(stream)
				{
					SecurityOptions opts = null;
					if(value)
					{
						opts = new SecurityOptions(SecureProtocol.Tls1);
						opts.Protocol = SecureProtocol.Tls1;
						opts.Certificate = null;
						opts.AllowedAlgorithms = SslAlgorithms.SECURE_CIPHERS;
						opts.VerificationType = CredentialVerification.Manual;
						opts.Verifier = new CertVerifyEventHandler(stream_OnCertVerify);
						opts.Flags = SecurityFlags.Default;
					}
					else
					{
						opts = new SecurityOptions(SecureProtocol.None);
					}
					stream.Flush();
					((SecureNetworkStream)stream).ChangeSecurityProtocol(opts);
					this.isSecure = value;
				}
			}
		}


		private void stream_OnCertVerify(SecureSocket sock, Certificate cert, CertificateChain chain, VerifyEventArgs e)
		{
			isSecurityChanging = false;
			raiseCertificateVerifiedEvent(EventArgs.Empty);
		}
		
	}
}
