using System;
using System.Net.Sockets;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using Mono.Security.Protocol.Tls;

namespace openXMPP
{
	/// <summary>
	/// Summary description for MonoTcpClient.
	/// </summary>
	public class MonoTcpClient : openXMPP.SecurableTcpClient
	{
		private NetworkStream plainStream;
		private SslClientStream secureStream;

		public MonoTcpClient(string hostname, int port) : this(hostname, port, false)
		{ }

		
		public MonoTcpClient(string hostname, int port, bool secured) : base(hostname, port, secured)
		{ }
		

		public override void Open()
		{
			TcpClient plainClient = new TcpClient(hostname, port);
			plainStream = plainClient.GetStream();

			if(secured)
			{
				secureStream = new SslClientStream(stream, hostname, false, SecurityProtocolType.Tls, null);
				secureStream.CheckCertRevocationStatus = true;
				secureStream.ServerCertValidationDelegate += new CertificateValidationCallback(secureStream_OnServerCertValidation);
				stream = secureStream;
				raiseCertificateVerifiedEvent(EventArgs.Empty);
			}
			else
			{
				stream = plainStream;
			}
		}

		public override void Close()
		{
			this.stream.Close();
		}

		
		private bool secureStream_OnServerCertValidation(X509Certificate certificate, int[] certificateErrors)
		{
			isSecurityChanging = false;
			return true;
		}

		
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
					stream.Flush();
					if(value)
					{
						secureStream = new SslClientStream(stream, hostname, false, SecurityProtocolType.Tls, null);
						secureStream.CheckCertRevocationStatus = true;
						secureStream.ServerCertValidationDelegate += new CertificateValidationCallback(secureStream_OnServerCertValidation);
						stream = secureStream;
						isSecurityChanging = false;
						raiseCertificateVerifiedEvent(EventArgs.Empty);
					}
					else
					{
						stream = plainStream;
					}
					this.isSecure = value;
				}
			}
		}
	}
}
