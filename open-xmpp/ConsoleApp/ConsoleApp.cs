using System;
using System.Threading;
using openXMPP;

namespace ConsoleApp
{
	/// <summary>
	/// Console application
	/// </summary>
	public class ConsoleApp
	{
//		string host = "talk.google.com";
//		string port = "5222";
//		string username = "jcl.openxmpp";
//		string domain = "gmail.com";
//		string resource = "openXMPP";
//		string password = "OpEnXmPp";

		string host = "here.dk";
		string port = "5222";
		string username = "jcl.openxmpp";
		string domain = "here.dk";
		string resource = "openXMPP";
		string password = "OpEnXmPp";

//		string host = "odysseus";
//		string port = "5222";
//		string username = "windows";
//		string domain = "localhost";
//		string resource = "openXMPP";
//		string password = "windows";

		private Session session;

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		public static void Main(string[] args)
		{
			new ConsoleApp();
		}

		public ConsoleApp()
		{
			MentalisTcpClient tcpClient = new MentalisTcpClient(host, int.Parse(port));
			//SecurableTcpClient tcpClient = new MonoTcpClient(host, int.Parse(port));
			session = new Session(tcpClient);
			
			session.OnAnyReceive += new XmlProtocolElementHandler(session_OnAnyReceive);
			session.OnAnySend += new XmlProtocolElementHandler(session_OnAnySend);
			session.OnError += new SessionEventHandler(session_OnError);

			//get open args from console
//			Console.Write("Host:");
//			host = Console.ReadLine();
//			Console.Write("Port:");
//			port = Console.ReadLine();
//			Console.Write("Username:");
//			username = Console.ReadLine();
//			Console.Write("Domain:");
//			domain = Console.ReadLine();
//			Console.Write("Password:");
//			password = Console.ReadLine();
//			Console.Write("Resource:");
//			resource = Console.ReadLine();

			if(!session.Open(domain, resource, username, password, 30000))
			{
				Console.WriteLine("Login failure! Press any key...");
				Console.Read();
				return;
			}

			Console.Write("\nType command (help roster add delete chat chats presence xml exit):");
			string cmd = Console.ReadLine();
			cmd = cmd.Trim();
			while(!cmd.Equals("exit"))
			{				
				//process commands
				string[] tokens = cmd.Split(' ');
				//roster
				if(tokens[0].Equals("roster"))
				{
					session.SendStanza(StanzaFactory.GetRosterStanza(session.JabberID));
					Console.WriteLine("Roster retrieved");
				}
				//add friend@example.com name [group]
				if(tokens[0].Equals("add"))
				{
					if(tokens.Length==3) session.SendStanza(StanzaFactory.GetAddFriendStanza(session.JabberID,
						tokens[1],tokens[2],""));
					else session.SendStanza(StanzaFactory.GetAddFriendStanza(session.JabberID,
							 tokens[1],tokens[2],tokens[3]));
				}
				//delete friend@example.com
				if(tokens[0].Equals("delete"))
				{
					session.SendStanza(StanzaFactory.GetDeleteFriendStanza(session.JabberID,tokens[1]));
				}
				//chat friend@example.com body [lang]
				if(tokens[0].Equals("chat"))
				{
					MessageBody body = new MessageBody();
					body.Body = tokens[2];
					body.Language = "en";
					if(tokens.Length==4) body.Language = tokens[3];
					session.SendStanza(StanzaFactory.GetChatMessageStanza(session.JabberID, tokens[1], body.Language, body));
				}
				//chats friend@example.com subject body [subj lang] [body lang]
				if(tokens[0].Equals("chats"))
				{
					MessageBody body = new MessageBody();
					MessageSubject subject = new MessageSubject();
					body.Language = "en";
					subject.Language = "en";
					if(tokens.Length == 6) 
					{
						subject.Language = tokens[4];
						body.Language = tokens[5];
					}
					if(tokens.Length == 5)
					{
						subject.Language = tokens[4];
					}
					session.SendStanza(StanzaFactory.GetChatMessageStanza(session.JabberID, tokens[1], body.Language, body, subject));
				}
				//presence away [message message_language]
				if(tokens[0].Equals("presence"))
				{
					string language = "en";
					if(tokens.Length>=3)
					{
						language = tokens[3];
						string[,] status = new string[1,2];
						status[0,0]=language;
						status[0,1]=tokens[2];
						session.SendStanza(StanzaFactory.GetPresenceBroadcastStanza(tokens[1],language,status));
						}
					else
					{
						session.SendStanza(StanzaFactory.GetPresenceBroadcastStanza(tokens[1],language));
					}
				}
				//xml any_xml
				if(tokens[0].Equals("xml"))
				{
					//session.startSending(cmd.Substring(tokens[0].Length));
					Console.WriteLine("Sorry, not allowed!");
				}
				//help
				if(tokens[0].Equals("help"))
				{
					Console.WriteLine("Commands:");
					Console.WriteLine("roster: shows roster");
					Console.WriteLine("add: add a new friend. add <friend@example.com>, <Name> [, <group>]");
					Console.WriteLine("delete: delete a friend. delete <friend@example.com>");
					Console.WriteLine("chat: send a message. chat <friend@example.com>, <body> [, <body language>]");
					Console.WriteLine("chats: send a message with a subject. chats <friend@example.com>, <subject>, <body> [, <subject language>][, <body language]");
					Console.WriteLine("presence: broadcasts presence. presence <show> [, <status>, <status language>]");
					Console.WriteLine("exit: close console");
					Console.WriteLine("xml <raw xml>: send the raw xml");
				}
				// close
				if(tokens[0].Equals("restart"))
				{
					session.Close();
					
					tcpClient = new MentalisTcpClient(host, int.Parse(port));
					session = new Session(tcpClient);

					session.OnAnyReceive += new XmlProtocolElementHandler(session_OnAnyReceive);
					session.OnAnySend += new XmlProtocolElementHandler(session_OnAnySend);
					session.OnError += new SessionEventHandler(session_OnError);

					if(!session.Open(domain, resource, username, password, System.Threading.Timeout.Infinite))
					{
						Console.WriteLine("Login failure! Press any key...");
						Console.Read();
						return;
					}
				}
				//wait for next command
				Console.Write("\nType command:");
				cmd = Console.ReadLine();
			}
			//typed exit
			//Done!
		}

		public void session_OnAnyReceive(object sender, XmlProtocolElement e)
		{
			Console.WriteLine("RECV:"+e);
		}

		public void session_OnAnySend(object sender, XmlProtocolElement e)
		{
			Console.WriteLine("SENT:"+e);
		}

		private void session_OnError(object sender, object evtObj)
		{
			Console.WriteLine("ERROR: "+evtObj);
		}
	}
}
