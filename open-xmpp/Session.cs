using System;
using System.Collections;
using System.Diagnostics;
using System.Text;
using System.Xml;
using System.Threading;
using System.IO;
using System.Net;

namespace openXMPP
{

	/// <summary>
	/// States the stream can be in
	/// </summary>
	public enum SessionState
	{
		Offline,		// No network connection is established
		Connected,		// Stream established, no authentication or security
		StartingTls,	// Negotiating TLS security
		StartingSasl,	// Authentication in progress
		StartingSession,// Sending <stream> or binding
		LoggedIn,		// Log in completed
		Disconnecting	// End of stream was reached
	}


	/// <summary>
	/// Handler for generic session events
	/// </summary>
	public delegate void SessionEventHandler(object sender, object evtObj);

	/// <summary>
	/// Handler for stanza events
	/// </summary>
	public delegate void StanzaHandler(object sender, Stanza s);

	/// <summary>
	/// An XMPP session composed of a network connection and XMPP stream.
	/// Responsible for establishing network connections, requesting new streams,
	/// authenticating users, sending and receiving stanzas and data, etc.
	/// </summary>
	public class Session
	{
		/// <summary>
		/// The namespace of all stream elements.
		/// </summary>
		public const string STREAM_NAMESPACE = "http://etherx.jabber.org/streams";
		
		#region Events

		/// <summary>
		/// Triggered when the session establishes a network connection with the server.
		/// Called when insecure connection established and when TLS connection is established.
		/// </summary>
		public event SessionEventHandler OnConnected;

		/// <summary>
		/// Triggered when the session's network connection is disconnected.
		/// </summary>
		public event SessionEventHandler OnDisconnected;

		/// <summary>
		/// Triggered when the session successfully authenticates to the server.
		/// After this method has been called the client can procede to send and receive stanzas.
		/// </summary>
		//public event SessionEventHandler OnLoginSuccess;

		/// <summary>
		/// Triggered when an authentication failure element is received.
		/// </summary>
		//public event XmlProtocolElementHandler OnLoginFailure;

		/// <summary>
		/// Triggered when a stream I/O error occurs or a stream:error element is received.
		/// The connection will always be terminated after this event is raised.
		/// </summary>
		public event SessionEventHandler OnError;

		/// <summary>
		/// Triggered when the session changes state.
		/// E.g. ... StartingTLS -> Connected -> StartingSASL ...
		/// </summary>
		public event SessionEventHandler OnStateChange;

		/// <summary>
		/// Triggered when xml of any kind is sent to the server (except for opening and closing session tags)
		/// </summary>
		public event XmlProtocolElementHandler OnAnySend;

		/// <summary>
		/// Triggered when xml of any kind is received from the server (including opening and closing session tags)
		/// </summary>
		public event XmlProtocolElementHandler OnAnyReceive;

		/// <summary>
		/// Triggered when a stanza is sent to the server.
		/// Stanzas can only be sent without error if the session is in the LoggedIn state.
		/// </summary>
		public event StanzaHandler OnStanzaSend;

		/// <summary>
		/// Triggered when a stanza is received from the server and the session is in the LoggedIn state.
		/// </summary>
		public event StanzaHandler OnStanzaReceive;

		#endregion

		#region Fields

		// Size of read buffer
		private const int READ_MAX = 1024;

		// Hostname of server we connect to
//		private string hostname;
		// Port to connect on
//		private int port;
		// XMPP domain (e.g. gmail.com)
		private string domain;
		// XMPP username
		private string username;
		// XMPP password
		private string password;
		// Resource to bind on login (optional)
		private string resource;
		// Version of XMPP protocol.  Must be at least 1.0
		private string xmppVersion;
		// Unique identifier of this session
		private string sessionId;

		// Byte-level interface to server
		private SecurableTcpClient tcpClient;
		private int waitingReads, waitingWrites;

		// Mutex for blocking the open method until we are logged in or there is an error
		private ManualResetEvent blockingOpenEvent;

		// Buffer to hold bytes read from network stream
		private byte[] readBuff;
		// State-based parser to extract XML from the stream and convert it to an XmlProtocolElement
		private XMLStreamParser streamParser;
		// The current state of this session.
		private SessionState state;

		#endregion

		#region Constructors / Destructor

		/// <summary>
		/// Creates a new stream connection for XMPP communication
		/// </summary>
		public Session(SecurableTcpClient tcpClient)
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
			setState(SessionState.Offline);
		}

	
		/// <summary>
		/// Class destructor.
		/// Closes the session.
		/// </summary>
		~Session()
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
		/// Gets the state the session is in
		/// </summary>
		public SessionState State
		{
			get { return this.state; }
		}

	
		/// <summary>
		/// Gets the user's authentication domain.
		/// </summary>
		public string Domain
		{
			get { return this.domain; }
		}

	
		/// <summary>
		/// Gets the user's name.
		/// </summary>
		public string Username 
		{
			get { return this.username; }
		}


		/// <summary>
		/// Gets the user's password.
		/// </summary>
		public string Password 
		{
			get { return this.password; }
		}


		/// <summary>
		/// Gets bound jabber resource (e.g. a location)
		/// </summary>
		public string Resource
		{
			get { return this.resource; }
		}

		
		/// <summary>
		/// Gets the complete jabber ID
		/// </summary>
		public string JabberID
		{
			get 
			{ 
				string id = this.username + "@" + this.domain;
				if(resource != null)
					id += (resource.Length > 0 ? "/" + resource : "");
				return id;
			}
		}

		#endregion

		#region Public Methods
		
		/// <summary>
		/// Opens an XMPP sream connection
		/// </summary>
		/// <param name="hostname">DNS name of host to connect to</param>
		/// <param name="port">Port to connect on</param>
		/// <param name="domain">User's authentication domain</param>
		/// <param name="resource">The XMPP resource to bind on authentication.  If not specified, the server may generate a random resource.</param>
		/// <param name="username">User's name</param>
		/// <param name="password">User password</param>
		/// <param name="timeout">How long to wait for a session to be established.  Set to System.Threading.Timeout.Infinite to wait forever.  Set to zero to continue without waiting.</param>
		/// <param name="register">Attempt to register the user if a login failure occurs</param>
		public bool Open(string domain, string resource, string username, string password, int timeout)
		{
			this.domain = domain;
			this.resource = resource;
			this.username = username;
			this.password = password;

			// Create a blocking mutex for this method
			blockingOpenEvent = new ManualResetEvent(false);

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
			
			// Block until we enter the LogedIn state or we time out
			bool notTimedOut = blockingOpenEvent.WaitOne(timeout, false);
			blockingOpenEvent = null;

			return this.state == SessionState.LoggedIn && notTimedOut;
		}

		/// <summary>
		/// Closes this stream but leaves the network connection open
		/// </summary>
		public void Close()
		{
			closeStream();
			closeTcpClient();
		}
		
		/// <summary>
		/// Sends a stanza asynchronously.
		/// </summary>
		/// <param name="s">Stanza to send</param>
		public void SendStanza(Stanza s)
		{
			// Don't send unless we are logged in
			if(state != SessionState.LoggedIn)
			{
				OpenXMPPException err = new OpenXMPPException("Tried to send stanza \""+s+"\" in the "+state+" state.");
				if(OnError != null) OnError(this, err);
			}

			send(s);

			// Raise event
			if(OnStanzaSend != null) OnStanzaSend(this, s);
		}


		#endregion

		#region Private Methods

		/// <summary>
		/// Closes the XML stream without closing the network connection.
		/// </summary>
		private void closeStream() 
		{
			if(state == SessionState.Offline) return;

			// Enter the Disconnecting state and send the closing tag
			if(state != SessionState.Disconnecting)
			{
				setState(SessionState.Disconnecting);
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
			setState(SessionState.Offline);
			tcpClient.Close();
		}


		/// <summary>
		/// Starts an asynchronous read in a new thread
		/// </summary>
		private void startReading()
		{
			try
			{
				if(tcpClient.Stream != null && !tcpClient.IsSecurityChanging && tcpClient.Stream.CanRead)
				{
					++waitingReads;
					tcpClient.Stream.BeginRead(readBuff, 0, readBuff.Length, new AsyncCallback(stream_OnRead), tcpClient);
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
				if(tcpClient.Stream != null && !tcpClient.IsSecurityChanging && tcpClient.Stream.CanWrite)
				{
					++waitingWrites;
					byte[] bytes = Encoding.UTF8.GetBytes(message);
					tcpClient.Stream.BeginWrite(bytes, 0, bytes.Length, new AsyncCallback(stream_OnWrite), tcpClient);
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
		/// Called when a new stanza is parsed from the stream.
		/// </summary>
		/// <param name="stanza">The stanza associated with the event.</param>
		private void onNewStanza(Stanza stanza)
		{
			// Handle resource binding here, but everything else is the application's responsibility
			if(state == SessionState.StartingSession && stanza is IQStanza)
			{
				IQStanza iq = (IQStanza)stanza;
				if(iq.Type == "result")
				{
					if(iq.Query != null && iq.Query.Name == "bind")
					{
						// Start a session with the given resource name
						startSession(iq.Query.FirstChild.InnerText);
					}
					else if(iq.Query == null || (iq.Query != null && iq.Query.Name == "session"))
					{
						// Bind has been confirmed so we are now logged in
						setState(SessionState.LoggedIn);
						return;	
					}
				}
			}
			else if(state == SessionState.LoggedIn)
			{
				if(OnStanzaReceive != null) OnStanzaReceive(this, stanza);
			}
			else
			{
				if(OnError != null) OnError(this, "Stanza "+stanza+" was received in the "+state+" state");
			}
		}

		
		/// <summary>
		/// Called when a new stream element is parsed from the stream.
		/// </summary>
		/// <param name="e">Element parsed from stream</param>
		private void onNewStreamElement(StreamElement e)
		{
			if(e is ResponseElement)
			{
				ResponseElement responseE = (ResponseElement)e;
				switch(responseE.Type)
				{
					case ResponseType.Success:
						if(state == SessionState.StartingSasl)
						{
							// Restart stream in the online state
							streamParser.Reset();
							openStream(domain, "1.0");
							setState(SessionState.StartingSession);
						}
						break;
					case ResponseType.Failure:
						if(state == SessionState.StartingSasl)
						{
							setState(SessionState.Connected);
							// Unblock so client can deal with the login failure.
							blockingOpenEvent.Set();
						}
						break;
					case ResponseType.StreamError:
						if(OnError != null) OnError(this, responseE.ToString());
						break;
					case ResponseType.SaslChallenge:
						SaslResponseElement saslChallengeE = (SaslResponseElement)responseE;

						if(saslChallengeE.Data["rspauth"] != null)
						{
							SaslResponseElement response = StreamElementFactory.GetSaslResponseElement();
							send(response);
						}
						else
						{
							SaslResponseElement response = 
								StreamElementFactory.GetSaslResponseElement(username, password,
								saslChallengeE.Data["realm"] as string,
								saslChallengeE.Data["nonce"] as string,
								saslChallengeE.Data["qop"] as string);
							send(response);
						}
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
					case StreamFeaturesType.SaslMechanisms:
						if(state == SessionState.Connected)
						{
							setState(SessionState.StartingSasl);
							AuthenticationElement response = StreamElementFactory.GetAuthenticationElement(featuresE.BestSaslAuthMech, this.domain, this.username, this.password);
							send(response);
						}
						break;
					case StreamFeaturesType.StartTls:
						if(state == SessionState.Connected)
						{
							setState(SessionState.StartingTls);
							ResponseElement startTls = StreamElementFactory.GetStartTlsElement();
							send(startTls);
						}
						break;
					case StreamFeaturesType.Bind:
						if(state == SessionState.StartingSession)
						{
							IQStanza bindStanza = StanzaFactory.GetRequestResourceBindStanza(resource);
							send(bindStanza);
						}
						break;
					case StreamFeaturesType.Session:
						startSession(this.JabberID);
						break;
				}
			}
		}

	
		/// <summary>
		/// Changes the state of the session.
		/// </summary>
		/// <param name="s">The new state to enter</param>
		private void setState(SessionState s)
		{
			SessionState oldState = this.state;
			this.state = s;
			switch(s)
			{
				case SessionState.Disconnecting:
				case SessionState.Offline:
					if(blockingOpenEvent != null) blockingOpenEvent.Set();
					break;
				case SessionState.LoggedIn:
					blockingOpenEvent.Set();
					break;
				case SessionState.Connected:
					break;
				case SessionState.StartingSasl:
					break;
				case SessionState.StartingSession:
					break;
				case SessionState.StartingTls:
					break;
			}
			if(OnStateChange != null) OnStateChange(this, s);
		}

		
		/// <summary>
		/// Sends an session-opening stream element.
		/// </summary>
		/// <param name="jid"></param>
		private void startSession(string jid)
		{
			string[] jidFields = jid.Split('@', '/');
			username = jidFields[0];
			domain = jidFields[1];
			resource = jidFields[2];

			IQStanza sessionStanza = StanzaFactory.GetStartSessionStanza();
			send(sessionStanza);
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
			writer.WriteAttributeString("xmlns:stream", STREAM_NAMESPACE);
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
			// Make sure the stream is safe to read
			if(tcpClient.IsSecurityChanging || !tcpClient.Stream.CanRead) return;

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
					startReading();
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

			if(state != SessionState.StartingSession)
			{
				setState(SessionState.Connected);
				if(OnConnected != null) OnConnected(this, "Network connection initiated");
			}
			if(OnAnyReceive != null) OnAnyReceive(this, e);
		}

		
		/// <summary>
		/// Event handler for when a stream has ended.
		/// </summary>
		/// <param name="sender">The object that raised this event.</param>
		/// <param name="e">The stream element associated with the event.  Should always be a stream:stream closing tag.</param>
		private void streamParser_OnStreamEnd(object sender, XmlProtocolElement e)
		{
			// Right now we just close the client, but it would be better to leave the connection
			// open until we know if we are going to reinitiate the connection.
			// For now, just call Close() ; Open() and lament your hard lot in life.
			closeTcpClient();
			if(OnAnyReceive != null) OnAnyReceive(this, e);
			if(OnDisconnected != null) OnDisconnected(this, "Network connection terminated");
		}

		
		/// <summary>
		/// Event handler for when a new piece of XML has been parsed from the stream.
		/// Determines if the XML is a stanza or stream element and handles it accordingly.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="element"></param>
		private void streamParser_OnNewXmlProtocolElement(object sender, XmlProtocolElement element)
		{
			// Raise event
			if(OnAnyReceive != null) OnAnyReceive(this, element);

			// Handle the XML
			if(element.IsStanza)
				onNewStanza(StanzaFactory.GetStanza(element.InternalXml));
			else
				onNewStreamElement(StreamElementFactory.GetStreamElement(element.InternalXml));
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
