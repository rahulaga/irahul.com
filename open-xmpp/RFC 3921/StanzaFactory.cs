using System;
using System.Xml;
using System.IO;
using System.Diagnostics;
using System.Text;
using System.Collections;

namespace openXMPP
{
	/// <summary>
	/// Creates stanzas.
	/// </summary>
	public class StanzaFactory
	{

		/// <summary>
		/// Namespace of all bind stanzas
		/// </summary>
		public const string BIND_NAMESPACE = "urn:ietf:params:xml:ns:xmpp-bind";

		/// <summary>
		/// Namespace for session stanzas
		/// </summary>
		public const string SESSION_NAMESPACE = "urn:ietf:params:xml:ns:xmpp-session";

		/// <summary>
		/// id counter for iq stanzas
		/// </summary>
		private static int iqIdCounter = 1;

		private static string iqIdBase;

		/// <summary>
		/// This class cannot be instantiated
		/// </summary>
		private StanzaFactory()
		{ }


		public static Stanza GetStanza(XmlNode xml) 
		{
			Stanza s = null;
			switch(xml.Name)
			{
				case "iq":
					s = new IQStanza(xml);
					break;
				case "message":
					s = new MessageStanza(xml);
					break;
				case "presence":
					s = new PresenceStanza(xml);
					break;
				default:
					s = new Stanza(xml);
					break;
			}
			return s;
		}

		
		public static IQStanza GetRequestResourceBindStanza(string resource)
		{
			StringWriter xmlString = new StringWriter();
			XmlTextWriter xmlWriter = new XmlTextWriter(xmlString);

			xmlWriter.WriteStartElement("iq");
			xmlWriter.WriteAttributeString("type", "set");
			xmlWriter.WriteAttributeString("id", getNextIqId());
			xmlWriter.WriteStartElement("bind", BIND_NAMESPACE);
			xmlWriter.WriteElementString("resource", resource);
			xmlWriter.WriteEndElement();
			xmlWriter.WriteEndElement();

			XmlDocument xml = new XmlDocument();
			xml.LoadXml(xmlString.ToString());

			//
			// FIXME:
			// Should require a response, but that causes problems somehow...
			//
			return new IQStanza(xml.DocumentElement);
		}

		
		public static IQStanza GetStartSessionStanza()
		{
			StringWriter xmlString = new StringWriter();
			XmlTextWriter xmlWriter = new XmlTextWriter(xmlString);

			xmlWriter.WriteStartElement("iq");
			xmlWriter.WriteAttributeString("type", "set");
			xmlWriter.WriteAttributeString("id", getNextIqId());
			xmlWriter.WriteStartElement("session", SESSION_NAMESPACE);
			xmlWriter.WriteEndElement();
			xmlWriter.WriteEndElement();

			XmlDocument xml = new XmlDocument();
			xml.LoadXml(xmlString.ToString());
			
			return new IQStanza(xml.DocumentElement);
		}

		
		public static IQStanza GetRosterStanza(string jabberID)
		{
			StringWriter xmlString = new StringWriter();
			XmlTextWriter xmlWriter = new XmlTextWriter(xmlString);

			xmlWriter.WriteStartElement("iq");
			xmlWriter.WriteAttributeString("type", "get");
			xmlWriter.WriteAttributeString("from", jabberID);
			xmlWriter.WriteAttributeString("id", getNextIqId());
				xmlWriter.WriteStartElement("query");
				xmlWriter.WriteAttributeString("xmlns", "jabber:iq:roster");
			xmlWriter.WriteEndElement();
			xmlWriter.WriteEndElement();

			XmlDocument xml = new XmlDocument();
			xml.LoadXml(xmlString.ToString());
			
			return new IQStanza(xml.DocumentElement);
		}

		
		public static IQStanza GetAddFriendStanza(string jabberID, string friendJID, string friendName, string group)
		{
			StringWriter xmlString = new StringWriter();
			XmlTextWriter xmlWriter = new XmlTextWriter(xmlString);

			xmlWriter.WriteStartElement("iq");
				xmlWriter.WriteAttributeString("type", "set");
				xmlWriter.WriteAttributeString("from", jabberID);
				xmlWriter.WriteAttributeString("id", getNextIqId());
				xmlWriter.WriteStartElement("query");
					xmlWriter.WriteAttributeString("xmlns", "jabber:iq:roster");
					xmlWriter.WriteStartElement("item");
						xmlWriter.WriteAttributeString("jid", friendJID);
						xmlWriter.WriteAttributeString("name", friendName);
						if(group.Length>0)
						{
							xmlWriter.WriteStartElement("group");
							xmlWriter.WriteString(group);
							xmlWriter.WriteEndElement();
						}
					xmlWriter.WriteEndElement();
				xmlWriter.WriteEndElement();
			xmlWriter.WriteEndElement();

			XmlDocument xml = new XmlDocument();
			xml.LoadXml(xmlString.ToString());
			
			return new IQStanza(xml.DocumentElement);
		}

		
		public static IQStanza GetDeleteFriendStanza(string jabberID, string friendJID)
		{
			StringWriter xmlString = new StringWriter();
			XmlTextWriter xmlWriter = new XmlTextWriter(xmlString);

			xmlWriter.WriteStartElement("iq");
			xmlWriter.WriteAttributeString("type", "set");
			xmlWriter.WriteAttributeString("from", jabberID);
			xmlWriter.WriteAttributeString("id", getNextIqId());
				xmlWriter.WriteStartElement("query");
					xmlWriter.WriteAttributeString("xmlns", "jabber:iq:roster");
					xmlWriter.WriteStartElement("item");
						xmlWriter.WriteAttributeString("jid", friendJID);
						xmlWriter.WriteAttributeString("subscription", "remove");
					xmlWriter.WriteEndElement();
				xmlWriter.WriteEndElement();
			xmlWriter.WriteEndElement();

			XmlDocument xml = new XmlDocument();
			xml.LoadXml(xmlString.ToString());
			
			return new IQStanza(xml.DocumentElement);
		}

		
		public static IQStanza GetRequestRegistrationFieldsStanza()
		{
			StringWriter xmlString = new StringWriter();
			XmlTextWriter xmlWriter = new XmlTextWriter(xmlString);

			xmlWriter.WriteStartElement("iq");
			xmlWriter.WriteAttributeString("type", "get");
			xmlWriter.WriteAttributeString("id", getNextIqId());
			xmlWriter.WriteStartElement("query", "jabber:iq:register");
			xmlWriter.WriteEndElement();
			xmlWriter.WriteEndElement();

			XmlDocument xml = new XmlDocument();
			xml.LoadXml(xmlString.ToString());

			return new IQStanza(xml.DocumentElement);
		}

		
		public static IQStanza GetRegisterUserStanza(Hashtable requiredInfo)
		{
			StringWriter xmlString = new StringWriter();
			XmlTextWriter xmlWriter = new XmlTextWriter(xmlString);

			xmlWriter.WriteStartElement("iq");
			xmlWriter.WriteAttributeString("type", "set");
			xmlWriter.WriteAttributeString("id", getNextIqId());
			xmlWriter.WriteStartElement("query", "jabber:iq:register");
			foreach(string key in requiredInfo.Keys)
			{
				xmlWriter.WriteStartElement(key);
				xmlWriter.WriteString(requiredInfo[key].ToString());
				xmlWriter.WriteEndElement();
			}
			xmlWriter.WriteEndElement();
			xmlWriter.WriteEndElement();

			XmlDocument xml = new XmlDocument();
			xml.LoadXml(xmlString.ToString());

			return new IQStanza(xml.DocumentElement);
		}


		public static MessageStanza GetChatMessageStanza(string fromJID, string to, string lang, string body, string subject, string thread)
		{
			MessageBody[] bodies = null;
			MessageSubject[] subjects = null;
			if(body != null)
			{
				bodies = new MessageBody[1];
				bodies[0] = new MessageBody();
				bodies[0].Body = body;
				bodies[0].Language = lang;
			}
			if(subject != null)
			{
				subjects = new MessageSubject[1];
				subjects[0] = new MessageSubject();
				subjects[0].Subject = subject;
				subjects[0].Language = lang;
			}

			return GetChatMessageStanza(fromJID, to, lang, bodies, subjects, thread);
		}

		
		public static MessageStanza GetChatMessageStanza(string fromJID, string to, string lang, MessageBody body)
		{
			MessageBody[] bodies = null;
			if(body != null)
			{
				bodies = new MessageBody[1];
				bodies[0] = body;
			}
			return GetChatMessageStanza(fromJID, to, lang, bodies, null, null);
		}

	
		public static MessageStanza GetChatMessageStanza(string fromJID, string to, string lang, MessageBody body, MessageSubject subject)
		{
			MessageSubject[] subjects = null;
			MessageBody[] bodies = null;
			if(subject != null)
			{
				subjects = new MessageSubject[1];
				subjects[0] = subject;
			}
			if(body != null)
			{
				bodies = new MessageBody[1];
				bodies[0] = body;
			}
			return GetChatMessageStanza(fromJID, to, lang, bodies, subjects, null);
		}

		
		public static MessageStanza GetChatMessageStanza(string fromJID, string to, string lang, MessageBody body, MessageSubject subject, string thread)
		{
			MessageSubject[] subjects = null;
			MessageBody[] bodies = null;
			if(subject != null)
			{
				subjects = new MessageSubject[1];
				subjects[0] = subject;
			}
			if(body != null)
			{
				bodies = new MessageBody[1];
				bodies[0] = body;
			}
			return GetChatMessageStanza(fromJID, to, lang, bodies, subjects, thread);
		}
	

		/// <summary>
		/// Chat message stanza
		/// </summary>
		/// <param name="fromJID"></param>
		/// <param name="to"></param>
		/// <param name="lang"></param>
		/// <param name="body">multiple bodies with different languages possible. body[i,0] is language
		/// and body[i,1] is the message</param>
		/// <returns></returns>
		public static MessageStanza GetChatMessageStanza(string fromJID, string to, string lang, MessageBody[] bodies)
		{
			return GetChatMessageStanza(fromJID, to, lang, bodies, null, null);
		}
		

		/// <summary>
		/// Message stanza with subject
		/// </summary>
		/// <param name="fromJID"></param>
		/// <param name="to"></param>
		/// <param name="lang"></param>
		/// <param name="body">multiple bodies with different languages possible. body[i,0] is language
		/// and body[i,1] is the message</param>
		/// <param name="subject">multiple subjects with different languages possible. subject[i,0] is language
		/// and subject[i,1] is the text</param>
		/// <returns></returns>
		public static MessageStanza GetChatMessageStanza(string fromJID, string to, string lang, MessageBody[] bodies, MessageSubject[] subjects)
		{
			return GetChatMessageStanza(fromJID, to, lang, bodies, subjects, null);
		}
		

		/// <summary>
		/// Threaded message
		/// </summary>
		/// <param name="fromJID"></param>
		/// <param name="to"></param>
		/// <param name="lang"></param>
		/// <param name="body"></param>
		/// <param name="thread">Thread</param>
		/// <returns></returns>
		public static MessageStanza GetChatMessageStanza(string fromJID, string to, string lang, MessageBody[] bodies, string thread)
		{
			return GetChatMessageStanza(fromJID, to, lang, bodies, null, thread);
		}

	
		/// <summary>
		/// Threaded message with subject
		/// </summary>
		/// <param name="fromJID"></param>
		/// <param name="to"></param>
		/// <param name="lang"></param>
		/// <param name="body"></param>
		/// <param name="subject"></param>
		/// <param name="thread">Thread</param>
		/// <returns></returns>
		public static MessageStanza GetChatMessageStanza(string fromJID, string to, string lang, MessageBody[] bodies, MessageSubject[] subjects, string thread)
		{
			if(fromJID == null) throw new ArgumentNullException("fromJID");
			if(to == null) throw new ArgumentNullException("to");
			if(lang == null) throw new ArgumentNullException("lang");

			StringWriter xmlString = new StringWriter();
			XmlTextWriter xmlWriter = new XmlTextWriter(xmlString);

			xmlWriter.WriteStartElement("message");
			xmlWriter.WriteAttributeString("type", "chat");
			xmlWriter.WriteAttributeString("from", fromJID);
			xmlWriter.WriteAttributeString("to", to);
			xmlWriter.WriteAttributeString("xml:lang", lang);
			if(bodies != null)
			{
				foreach(MessageBody body in bodies)
				{
					xmlWriter.WriteStartElement("body");
					xmlWriter.WriteAttributeString("xml:lang", body.Language);
					xmlWriter.WriteString(body.Body);
					xmlWriter.WriteEndElement();
				}
			}
			if(subjects != null)
			{
				foreach(MessageSubject subject in subjects)
				{
					xmlWriter.WriteStartElement("subject");
					xmlWriter.WriteAttributeString("xml:lang", subject.Language);
					xmlWriter.WriteString(subject.Subject);
					xmlWriter.WriteEndElement();
				}
			}
			if(thread != null && thread.Length > 1)
			{
				xmlWriter.WriteStartElement("thread");
				xmlWriter.WriteString(thread);
				xmlWriter.WriteEndElement();
			}
			xmlWriter.WriteEndElement();
			XmlDocument xml = new XmlDocument();
			xml.LoadXml(xmlString.ToString());
			
			return new MessageStanza(xml.DocumentElement, MessageStanzaType.Chat);
		}


		/// <summary>
		/// Presence broadcast stanza
		/// </summary>
		/// <param name="show">Show message (eg. Away, Busy, Idle etc)</param>
		/// <param name="language"></param>
		/// <returns></returns>
		public static PresenceStanza GetPresenceBroadcastStanza(string show, string language)
		{
			StringWriter xmlString = new StringWriter();
			XmlTextWriter xmlWriter = new XmlTextWriter(xmlString);

			xmlWriter.WriteStartElement("presence");
				xmlWriter.WriteAttributeString("xml:lang", language);			
				xmlWriter.WriteStartElement("show");
					xmlWriter.WriteString(show);
				xmlWriter.WriteEndElement();
			xmlWriter.WriteEndElement();

			XmlDocument xml = new XmlDocument();
			xml.LoadXml(xmlString.ToString());
			
			return new PresenceStanza(xml.DocumentElement);

		}


		/// <summary>
		/// Presence broadcast stanza, with a message ('status')
		/// </summary>
		/// <param name="show">Show message (eg. Away, Busy, Idle etc)</param>
		/// <param name="language"></param>
		/// <param name="status">Away messages, can be multiple with different languages</param>
		/// <returns></returns>
		public static PresenceStanza GetPresenceBroadcastStanza(string show, string language, string[,] status)
		{
			StringWriter xmlString = new StringWriter();
			XmlTextWriter xmlWriter = new XmlTextWriter(xmlString);

			xmlWriter.WriteStartElement("presence");
				xmlWriter.WriteAttributeString("xml:lang", language);			
				xmlWriter.WriteStartElement("show");
					xmlWriter.WriteString(show);
				xmlWriter.WriteEndElement();
				for(int i=0;i<status.GetLength(0);i++)
				{
					xmlWriter.WriteStartElement("status");
						xmlWriter.WriteAttributeString("xml:lang",status[i,0]);
						xmlWriter.WriteString(status[i,1]);
					xmlWriter.WriteEndElement();
				}
			xmlWriter.WriteEndElement();

			XmlDocument xml = new XmlDocument();
			xml.LoadXml(xmlString.ToString());
			
			return new PresenceStanza(xml.DocumentElement);
		}


		public static PresenceStanza GetReplySubscriptionRequestStanza(string requestorJID, bool approve)
		{
			StringWriter xmlString = new StringWriter();
			XmlTextWriter xmlWriter = new XmlTextWriter(xmlString);

			xmlWriter.WriteStartElement("presence");
			xmlWriter.WriteAttributeString("to", requestorJID);
			xmlWriter.WriteAttributeString("type", approve ? "subscribed" : "unsubscribed");
			xmlWriter.WriteEndElement();

			XmlDocument xml = new XmlDocument();
			xml.LoadXml(xmlString.ToString());

			return new PresenceStanza(xml.DocumentElement);
		}
		

		public static PresenceStanza GetRequestSubscriptionStanza(string to)
		{
			StringWriter xmlString = new StringWriter();
			XmlTextWriter xmlWriter = new XmlTextWriter(xmlString);

			xmlWriter.WriteStartElement("presence");
			xmlWriter.WriteAttributeString("to", to);
			xmlWriter.WriteAttributeString("type", "subscribe");
			xmlWriter.WriteEndElement();

			XmlDocument xml = new XmlDocument();
			xml.LoadXml(xmlString.ToString());

			return new PresenceStanza(xml.DocumentElement);
		}

		private static string getNextIqId()
		{
			if(iqIdBase == null)
			{
				Random r = new Random();
				char[] chars = new char[8];
				for(int i=0; i<chars.Length; i++) 
					chars[i] = (char)(r.Next(26)+97);
				iqIdBase = "openXMPP_"+(new String(chars));
			}
			return iqIdBase+(iqIdCounter++);
		}
	}
}
