using System;
using System.Xml;
using System.Collections;

namespace openXMPP
{
	
	/// <summary>
	/// The type of this message stanza as in RFC 3921
	/// </summary>
	public enum MessageStanzaType
	{
		Chat,
		Error,
		Groupchat,
		Headline,
		Normal
	};

	public class MessageSubject
	{
		public string Subject;
		public string Language;
	}

	public class MessageBody
	{
		public string Body;
		public string Language;
	}

	/// <summary>
	/// Summary description for MessageStanza.
	/// </summary>
	public class MessageStanza : Stanza
	{
		private MessageStanzaType msgType;

		public MessageStanza(XmlNode xml) : base(xml)
		{
			if(xml.Attributes["type"] == null)
			{
				this.msgType = MessageStanzaType.Headline;
				return;
			}

			switch(xml.Attributes["type"].Value)
			{
				case "chat":
					this.msgType = MessageStanzaType.Chat;
					break;				
				case "error":
					this.msgType = MessageStanzaType.Error;
					break;
				case "groupchat":
					this.msgType = MessageStanzaType.Groupchat;
					break;
				case "headline":
					this.msgType = MessageStanzaType.Headline;
					break;
				case "normal":
					this.msgType = MessageStanzaType.Normal;
					break;
				default:
					throw new OpenXMPPException("An invalid Message type was specified.");
			}
		}
		
		public MessageStanza(XmlNode xml, MessageStanzaType msgType) : base(xml)
		{
			this.msgType = msgType;
		}

		public MessageStanzaType MessageType
		{
			get { return this.msgType; }
		}

		public MessageSubject FirstSubject
		{
			get
			{
				foreach(XmlNode n in internalXml.ChildNodes)
				{
					if(n.Name == "subject")
					{
						MessageSubject subject = new MessageSubject();
						subject.Subject = n.InnerText;
						subject.Language = n.Attributes["xml:lang"] == null ? this.Language : n.Attributes["xml:lang"].Value;
						return subject;
					}
				}
				return null;
			}
		}

		public MessageBody FirstBody
		{
			get
			{
				foreach(XmlNode n in internalXml.ChildNodes)
				{
					if(n.Name == "body")
					{
						MessageBody body = new MessageBody();
						body.Body = n.InnerText;
						body.Language = n.Attributes["xml:lang"] == null ? this.Language : n.Attributes["xml:lang"].Value;
						return body;
					}
				}
				return null;
			}
		}

		public MessageSubject[] Subjects
		{
			get 
			{
				ArrayList subjects = new ArrayList();
				foreach(XmlNode n in internalXml.ChildNodes)
				{
					if(n.Name == "subject")
					{
						MessageSubject subject = new MessageSubject();
						subject.Subject = n.InnerText;
						subject.Language = n.Attributes["xml:lang"] == null ? this.Language : n.Attributes["xml:lang"].Value;
						subjects.Add(subject);
					}
				}

				MessageSubject[] s = new MessageSubject[subjects.Count];
				for(int i=0; i<s.Length; i++)
					s[i] = (MessageSubject)subjects[i];

				return s;
			}
		}

		public MessageBody[] Bodies
		{
			get 
			{
				ArrayList bodies = new ArrayList();
				foreach(XmlNode n in internalXml.ChildNodes)
				{
					if(n.Name == "body")
					{
						MessageBody body = new MessageBody();
						body.Body = n.InnerText;
						body.Language = n.Attributes["xml:lang"] == null ? this.Language : n.Attributes["xml:lang"].Value;
						bodies.Add(body);
					}
				}

				MessageBody[] b = new MessageBody[bodies.Count];
				for(int i=0; i<b.Length; i++)
					b[i] = (MessageBody)bodies[i];

				return b;
			}
		}

		public string ThreadID
		{
			get 
			{
				foreach(XmlNode n in internalXml.ChildNodes)
					if(n.Name == "thread") return n.InnerText;
				return null;
			}
		}

	}
}
