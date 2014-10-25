using System;
using System.Collections;
using System.Threading;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;

namespace openXMPP
{

	public delegate Hashtable AccountRegistrationHandler(object sender, Hashtable registrationFields);

	/// <summary>
	/// Summary description for AccountManager.
	/// </summary>
	public class AccountManager
	{

		private enum AccountManagerState
		{
			Offline,		// No network connection is established
			Connected,		// Stream established, no authentication or security
			StartingTls,	// Negotiating TLS security
			Registering,	// Registering an account
			Deleting,		// Deleting an account
			Success,		// Attempted operation was successful
			Failed,			// Attempted operation failed
			Disconnecting	// End of stream was reached
		}

		
		#region Events

		/// <summary>
		/// sdjf
		/// </summary>
		public event AccountRegistrationHandler OnRegistrationFieldsReceived;

		public event EventHandler OnSuccess;

		public event EventHandler OnFailure;

		/// <summary>
		/// Triggered when a stream I/O error occurs or a stream:error element is received.
		/// The connection will always be terminated after this event is raised.
		/// </summary>
		public event SessionEventHandler OnError;

		/// <summary>
		/// Triggered when xml of any kind is sent to the server (except for opening and closing session tags)
		/// </summary>
		public event XmlProtocolElementHandler OnAnySend;

		/// <summary>
		/// Triggered when xml of any kind is received from the server (including opening and closing session tags)
		/// </summary>
		public event XmlProtocolElementHandler OnAnyReceive;

		#endregion

		#region Fields

		// Size of read buffer
		private const int READ_MAX = 1024;

		// XMPP domain (e.g. gmail.com)
		private string domain;
		// Version of XMPP protocol.  Must be at least 1.0
		private string xmppVersion;
		// Unique identifier of this session
		private string sessionId;

		// Byte-level interface to server
		private SecurableTcpClient tcpClient;
		private int waitingReads, waitingWrites;

		// Buffer to hold bytes read from network stream
		private byte[] readBuff;
		// State-based parser to extract XML from the stream and convert it to an XmlProtocolElement
		private XMLStreamParser streamParser;
		// The current state of this session.
		private AccountManagerState state;

		#endregion

		#region Constructors / Destructor

		/// <summary>
		/// Creates a new stream connection for XMPP communication
		/// </summary>
		public AccountManager(SecurableTcpClient tcpClient)
		{
			this.tcpClient = tcpClient;

			// Buffer for incoming data
			readBuff = new byte[READ_MAX];

			// Need to parse incoming stuff into stanzas
			streamParser = new XMLStreamParser();

			// Wire up some evens for when new things come down the pipe
			streamParser.OnNewXmlProtocolElement += new XmlProtocolElementHandler(streamParser_OnNewXmlProtocolElement);
			streamParser.OnStreamBegin += new XmlProtocolElementHandler(streamParser_OnStreamBegin);
			streamParser.OnStreamEnd += new XmlProtocolElementHandler(streamParser_OnStreamEnd);

			// Start in the Offline state
			setState(AccountManagerState.Offline);
		}


		/// <summary>
		/// Class destructor.
		/// Closes the session.
		/// </summary>
		~AccountManager()
		{
			Close();
		}

		#endregion

		#region Properties
	
		/// <summary>
		/// Gets the version XMPP the server uses
		/// </summary>
		public string XmppVersion
		{
			get { return this.xmppVersion; }
		}


		/// <summary>
		/// Gets this session's unique identifier
		/// </summary>
		public string SessionId
		{
			get { return this.sessionId; }
		}

		/// <summary>
		/// Gets the user's authentication domain.
		/// </summary>
		public string Domain
		{
			get { return this.domain; }
		}
		#endregion

		#region Public Methods
	
		public void RequestAccountRegistration(string domain, int timeout)
		{
			if(OnRegistrationFieldsReceived == null)
			{
				throw new OpenXMPPException("You must create an event handler for the OnAccountRegistration event before calling this method.");
			}

			this.domain = domain;

			// Connect to the network and start reading
			try 
			{
				tcpClient.OnCertificateVerified += new EventHandler(tcpClient_OnCertificateVerified);
				tcpClient.Open();

				openStream(domain, "1.0");
				startReading();
			} 
			catch (Exception err) 
			{
				if(OnError != null) OnError(this, err);
				Close();
			}
		}

		
		public void RequestAccountDeletion(string domain, int timeout)
		{
			// Todo
		}

		
		/// <summary>
		/// Closes this stream but leaves the network connection open
		/// </summary>
		public void Close()
		{
			closeStream();
			closeTcpClient();
		}
	
		#endregion

		#region Private Methods

		/// <summary>
		/// Closes the XML stream without closing the network connection.
		/// </summary>
		private void closeStream() 
		{
			if(state == AccountManagerState.Offline) return;

			// Enter the Disconnecting state and send the closing tag
			if(state != AccountManagerState.Disconnecting)
			{
				setState(AccountManagerState.Disconnecting);
				startSending("</stream:stream>");
			}

			// Wait until reads and writes have completed or until we time out
			int timer = 100;
			while((waitingReads > 0 || waitingWrites > 0) && timer > 0)
			{
				Thread.Sleep(10);
				--timer;
			}
		}


		/// <summary>
		/// Closes the network connection.
		/// </summary>
		private void closeTcpClient()
		{
			// Enter the offline state and close the connection
			setState(AccountManagerState.Offline);
			if(tcpClient != null)
			{
				tcpClient.Close();
				tcpClient = null;
			} 
		}


		/// <summary>
		/// Starts an asynchronous read in a new thread
		/// </summary>
		private void startReading()
		{
			try
			{
				if(!tcpClient.IsSecurityChanging && tcpClient.Stream.CanRead)
				{
					++waitingReads;
					tcpClient.Stream.BeginRead(readBuff, 0, readBuff.Length, new AsyncCallback(stream_OnRead), tcpClient.Stream);
				}
			}
			catch(IOException err)
			{
				if(OnError != null) OnError(this, err);
				closeTcpClient();
			}
			catch(Exception err)
			{
				if(OnError != null) OnError(this, err);
				Close();
			}
		}
		
	
		/// <summary>
		/// Sends XML to the server.  Raises OnAnySend event.
		/// </summary>
		/// <param name="e">The XML to send</param>
		private void send(XmlProtocolElement e)
		{
			startSending(e.ToString());
			if(OnAnySend != null) OnAnySend(this, e);
		}
	
	
		/// <summary>
		/// Sends a string to the server asynchronously.
		/// </summary>
		/// <param name="message">The string to send</param>
		private void startSending(string message)
		{
			try
			{
				if(!tcpClient.IsSecurityChanging && tcpClient.Stream.CanWrite)
				{
					++waitingWrites;
					byte[] bytes = Encoding.UTF8.GetBytes(message);
					tcpClient.Stream.BeginWrite(bytes, 0, bytes.Length, new AsyncCallback(stream_OnWrite), tcpClient.Stream);
				}
			}
			catch(IOException err)
			{
				if(OnError != null) OnError(this, err);
				closeTcpClient();
			}
			catch (Exception err)
			{
				if(OnError != null) OnError(this, err);
				Close();
			}
		}

	
		/// <summary>
		/// Changes the state of the session.
		/// </summary>
		/// <param name="s">The new state to enter</param>
		private void setState(AccountManagerState s)
		{
			this.state = s;
			switch(s)
			{
				case AccountManagerState.Success:
					if(OnSuccess != null) OnSuccess(this, EventArgs.Empty);
					break;
				case AccountManagerState.Failed:
					if(OnFailure != null) OnFailure(this, EventArgs.Empty);
					break;
			}
		}
	
		
		/// <summary>
		/// Sends a new stream:stream element.
		/// </summary>
		/// <param name="domain">XMPP domain</param>
		/// <param name="xmppVersion">XMPP protocol version</param>
		private void openStream(string domain, string xmppVersion)
		{
			StringWriter xmlString = new StringWriter();
			XmlTextWriter writer = new XmlTextWriter(xmlString);
		
			writer.WriteStartElement("stream:stream", "jabber:client");
			writer.WriteAttributeString("to", domain);
			writer.WriteAttributeString("xmlns:stream", Session.STREAM_NAMESPACE);
			writer.WriteAttributeString("version", xmppVersion);
			writer.WriteEndElement();

			string str = "<?xml version='1.0'?>"+xmlString.ToString();
			str = str.Remove(str.Length - 2, 1);	// Delete '/' character so we aren't closing the stream right after opening it.
			startSending(str);
		}

		#endregion

		#region Callback Methods

		/// <summary>
		/// Called when an asynchronous read is completing.
		/// Takes bytes from the stream and feeds them to the parser.
		/// </summary>
		/// <param name="ar"></param>
		private void stream_OnRead(IAsyncResult ar) 
		{
			if(tcpClient == null) return;
			try
			{
				int byteCount = tcpClient.Stream.EndRead(ar);
				--waitingReads;
				if (byteCount > 0)
				{
					byte[] bytes = new byte[byteCount];
					bytes = new byte[byteCount];
					Array.Copy(readBuff, bytes, byteCount);
					streamParser.Feed(bytes);
					if(tcpClient != null && !tcpClient.IsSecurityChanging && tcpClient.Stream.CanRead) startReading();
				}
			}    
			catch(IOException err)
			{
				if(OnError != null) OnError(this, err);
				closeTcpClient();
			}
			catch (Exception err) 
			{
				if(OnError != null) OnError(this, err);
				Close();
			}
		}

	
		/// <summary>
		/// Called when an asynchronous write is completing.
		/// </summary>
		/// <param name="ar"></param>
		private void stream_OnWrite(IAsyncResult ar)
		{
			if(tcpClient == null) return;
			try
			{
				tcpClient.Stream.EndWrite(ar);
				--waitingWrites;
			}
			catch(IOException err)
			{
				if(OnError != null) OnError(this, err);
				closeTcpClient();
			}
			catch(Exception err)
			{
				if(OnError != null) OnError(this, err);
				Close();
			}
		}
	
	
		/// <summary>
		/// Event handler for when a stream has just been opened.
		/// At this point, the network is connected and a stream:stream element
		/// has been received.
		/// </summary>
		/// <param name="sender">Object that triggered the event</param>
		/// <param name="e">The stream element associated with the event</param>
		private void streamParser_OnStreamBegin(object sender, XmlProtocolElement e)
		{
			if (streamParser.Doc.DocumentElement != null)
				sessionId = streamParser.Doc.DocumentElement.GetAttribute("id");

			xmppVersion = streamParser.Doc.DocumentElement.HasAttribute("version") ? streamParser.Doc.DocumentElement.GetAttribute("version") : "0.0";

			//Check version is >= 1.0. If not, terminate connection as stated in RFC 3920.
			int xmppMajor = int.Parse(xmppVersion.Split('.')[0]);
			if(xmppMajor < 1)
			{
				StreamErrorElement err = StreamElementFactory.GetStreamErrorElement(StreamErrorType.UnsupportedVersion);
				send(err);
				if(OnError != null) OnError(this, tcpClient.Hostname+":"+tcpClient.Port+" does not support XMPP version 1.0 or higher.");
				Close();
			}

			setState(AccountManagerState.Connected);

//			if(state != SessionState.StartingSession)
//			{
//				setState(SessionState.Connected);
//				if(OnConnected != null) OnConnected(this, "Network connection initiated");
//			}
			if(OnAnyReceive != null) OnAnyReceive(this, e);
		}

	
		/// <summary>
		/// Event handler for when a stream has ended.
		/// </summary>
		/// <param name="sender">The object that raised this event.</param>
		/// <param name="e">The stream element associated with the event.  Should always be a stream:stream closing tag.</param>
		private void streamParser_OnStreamEnd(object sender, XmlProtocolElement e)
		{
			closeTcpClient();
			if(OnAnyReceive != null) OnAnyReceive(this, e);
		}

	
		/// <summary>
		/// Event handler for when a new piece of XML has been parsed from the stream.
		/// Determines if the XML is a stanza or stream element and handles it accordingly.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="element"></param>
		private void streamParser_OnNewXmlProtocolElement(object sender, XmlProtocolElement e)
		{
			// Raise event
			if(OnAnyReceive != null) OnAnyReceive(this, e);

			if(e.IsStanza) e = StanzaFactory.GetStanza(e.InternalXml);
			else e = StreamElementFactory.GetStreamElement(e.InternalXml);

			if(e is IQStanza)
			{
				IQStanza iq = (IQStanza)e;
				if(state == AccountManagerState.Registering)
				{
					if(iq.Type == "result")
					{
						// Received required registration fields
						if(iq.Query != null && iq.Query.NamespaceURI == "jabber:iq:register")
						{
							Hashtable regFields = new Hashtable();
							foreach(XmlNode n in iq.Query.ChildNodes)
							{
								regFields.Add(n.Name, n.InnerText);
							}

							// Get field values from user
							regFields = OnRegistrationFieldsReceived(this, regFields);

							// Regiser the account
							send(StanzaFactory.GetRegisterUserStanza(regFields));
						} 
						else if(iq.Query == null)
						{
							if(OnSuccess != null) OnSuccess(this, EventArgs.Empty);
						}
						else
						{
							if(OnError != null) OnError(this, "The server isn't registering accounts in a standard way.");
						}
					}
					else if(iq.Type == "error")
					{
						if(OnFailure != null) OnFailure(this, EventArgs.Empty);
					}
				}
				else if(state == AccountManagerState.Deleting && e is IQStanza)
				{
					if(iq.Type == "result")
					{
						// TODO
					}
					else if(iq.Type == "error")
					{
						// TODO
					}
				}
			}
			else if(e is ResponseElement)
			{
				ResponseElement responseE = (ResponseElement)e;
				switch(responseE.Type)
				{
					case ResponseType.Success:
						if(state == AccountManagerState.Registering)
						{
							// Restart stream in the online state
							streamParser.Reset();
							openStream(domain, "1.0");
							setState(AccountManagerState.Registering);
						}
						break;
					case ResponseType.StreamError:
						if(OnError != null) OnError(this, responseE.ToString());
						break;
					case ResponseType.ProceedTls:
						streamParser.Reset();
						try
						{
							tcpClient.Stream.Flush();
							tcpClient.IsSecure = true;
						}
						catch(IOException err)
						{
							if(OnError != null) OnError(this, err);
							closeTcpClient();
						}
						catch (Exception err)
						{
							if(OnError != null) OnError(this, err);
							Close();
						}
						break;
				}
			}
			else if (e is StreamFeaturesElement)
			{
				StreamFeaturesElement featuresE = (StreamFeaturesElement)e;
				
				switch(featuresE.Type)
				{
					case StreamFeaturesType.StartTls:
						if(state == AccountManagerState.Connected)
						{
							setState(AccountManagerState.StartingTls);
							ResponseElement startTls = StreamElementFactory.GetStartTlsElement();
							send(startTls);
						}
						break;
					case StreamFeaturesType.SaslMechanisms:
						if(state == AccountManagerState.Connected)
						{
							setState(AccountManagerState.Registering);
							IQStanza reqFieldsIq = StanzaFactory.GetRequestRegistrationFieldsStanza();
							send(reqFieldsIq);
						}
						break;
				}
			}
		}

	
		/// <summary>
		/// Called when the SecurableTcpClient has verified the server-side certificate.
		/// Reinitiates the stream and restarts the reading loop.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void tcpClient_OnCertificateVerified(object sender, EventArgs e)
		{
			openStream(domain, "1.0");
			startReading();
		}

		#endregion
	}
}
