using System;
using System.Xml;
using System.IO;
using System.Diagnostics;
using System.Collections;
using System.Text;


namespace openXMPP
{
	/// <summary>
	/// Handler for xml element events
	/// </summary>
	public delegate void XmlProtocolElementHandler(object sender, XmlProtocolElement element);

	/// <summary>
	/// Parses incoming bytes into XML for use in a openXMPP.Stream
	/// </summary>
	class XMLStreamParser
	{

		#region Events

		/// <summary>
		/// Raised when the beginning of a stream is detected
		/// </summary>
		public event XmlProtocolElementHandler OnStreamBegin;

		/// <summary>
		/// Raised when the end of a stream is detected
		/// </summary>
		public event XmlProtocolElementHandler OnStreamEnd;

		/// <summary>
		/// Raised when a new XML stream element has been parsed from the stream
		/// </summary>
		public event XmlProtocolElementHandler OnNewXmlProtocolElement;

		#endregion

		/// <summary>
		/// The possible states for this parser
		/// </summary>
		private enum ParserState 
		{
			Text,				// not inside tag
			Tag,				// tag started with "<"
			ElementOpen,		// element opened with "<"+white
			ValueSQuoted,		// inside single quotes ''
			ValueDQuoted,		// inside double quotes ""
			ElementEmpty,		// after "/", before ">"
			ElementClose,		// "</"
			ProcessingStart,	// "<?"
			ProcessingEnding,	// "?"
		}

		private ParserState state;
		private bool parsingActive = false;	// Flag for thread safety
		private int chunkStart = 0;
		private int elementLevel = 0;
		private string newData = "";
		private string oldData = "";
		private string docBegin = "";
		private string docEnd = "</stream:stream>";
		private Queue inQueue;
		
		private XmlDocument doc;

		/// <summary>
		/// Constructor
		/// </summary>
		public XMLStreamParser()
		{
			doc = new XmlDocument();
			inQueue = Queue.Synchronized(new Queue());
			Reset();
		}

		/// <summary>
		/// The stream as an XML document
		/// </summary>
		public XmlDocument Doc
		{
			get { return doc; }
		}

		/// <summary>
		/// Returns the parser to the default state
		/// </summary>
		public void Reset()
		{
			elementLevel = 0;
			state = ParserState.Text;
		}

		/// <summary>
		/// Feed bytes read from a stream through the parser
		/// </summary>
		/// <param name="b">Bytes to parse</param>
		public void Feed(byte[] b)
		{
			// Convert bytes to string
			string s = Encoding.UTF8.GetString(b, 0, b.Length).Replace("\0","");

			// Add string to buffer
			inQueue.Enqueue(s);
	
			// Continue if safe
			if(!parsingActive)
			{
				parsingActive = true;
				while(inQueue.Count > 0)
				{
					chunkStart = 0;
					newData = inQueue.Dequeue() as string;
					parse();
					oldData += newData.Substring(chunkStart, newData.Length-chunkStart);
				}
				parsingActive = false;
			}
		}
	
		private void receivedXmlProtocolElement(int dataLength)
		{
			string chunk = newData.Substring(chunkStart, dataLength-chunkStart+1);

			if (oldData.Length > 0) 
			{
				chunk = oldData + chunk;
				oldData = "";
			}

			// Place protocol element in document context so "stream" namespace is always well-defined (bloody stupid .NET xml parser...)
			chunk = docBegin+chunk.Trim()+docEnd;
			chunkStart = dataLength+1;

			doc.LoadXml(chunk);

			XmlProtocolElement newElement = new XmlProtocolElement(doc.DocumentElement.FirstChild);

			if(OnNewXmlProtocolElement != null)
				OnNewXmlProtocolElement(this, newElement);
		}
	
		private void receivedStreamBeginning(int dataLength)
		{
			string chunk = newData.Substring(chunkStart, dataLength-chunkStart+1);
			if (oldData.Length > 0) 
			{
				chunk  = oldData + chunk;
				oldData = "";
			}
			docBegin = chunk;
			doc.LoadXml(docBegin+docEnd);	// Hack to work with .NET's phobia of open tags in XmlDocuments
			StreamElement startElement = new StreamElement(doc.DocumentElement);
			chunkStart = dataLength+1;
			if(OnStreamBegin != null) OnStreamBegin(this, startElement);
		}
	
		private void receivedStreamEnd(int dataLength)
		{
			string chunk = newData.Substring(chunkStart, dataLength-chunkStart+1);
			if(oldData.Length > 0)
			{
				chunk = oldData + chunk;
				oldData = "";
			}
			docEnd = chunk;
			doc.LoadXml(docBegin+docEnd);	// Hack to work with .NET's phobia of open tags in XmlDocuments
			StreamElement endElement = new StreamElement(doc.DocumentElement);
			chunkStart = dataLength + 1;
			if(OnStreamEnd != null) OnStreamEnd(this, endElement);
		}
	
		private void parse()
		{
			chunkStart = 0;
			char c;
			int lim = newData.Length;		
			for (int i=0; i<lim; i++) 
			{
				c = newData[i];
				switch (state) 
				{
					case ParserState.Text:
						if (c == '<') state = ParserState.Tag;
						break;
					case ParserState.Tag:
						switch(c) 
						{
							case '?':
								state = ParserState.ProcessingStart;
								break;
							case '!': 
								//TODO
								break;
							case '/':
								state = ParserState.ElementClose;
								break;
							default:
								state = ParserState.ElementOpen;
								elementLevel++;
								break;
						}
						break;
					case ParserState.ElementOpen:   // element opened with "<"+white
						switch(c) 
						{
							case '>':
								state = ParserState.Text;
								if (elementLevel == 1) receivedStreamBeginning(i);
								break;
							case '/':
								state = ParserState.ElementEmpty;
								break;
							case '\'':
								state = ParserState.ValueSQuoted;
								break;
							case '\"':
								state = ParserState.ValueDQuoted;
								break;
							default:
								break;
						}
						break;
					case ParserState.ValueSQuoted: 
						if (c == '\'') state = ParserState.ElementOpen;
						break;
					case ParserState.ValueDQuoted:  
						if (c == '\"') state = ParserState.ElementOpen;
						break;
					case ParserState.ElementEmpty:      
						if (c == '>') 
						{
							state = ParserState.Text;
							elementLevel--;
							if (elementLevel == 0) receivedStreamEnd(i); //empty <stream>?
							else if (elementLevel == 1) receivedXmlProtocolElement(i);
						} 
						else 
						{
							state = ParserState.ElementOpen;
						}
						break;
					case ParserState.ElementClose:      // "</"
						if (c == '>') 
						{
							state = ParserState.Text;
							elementLevel--;
							if (elementLevel == 0) receivedStreamEnd(i);
							else if (elementLevel == 1) receivedXmlProtocolElement(i);
						}
						break;
					case ParserState.ProcessingStart:   // "<?"
						if (c == '?')	state = ParserState.ProcessingEnding;
						break;
					case ParserState.ProcessingEnding:  // "?"
						if (c == '>') 
						{
							state = ParserState.Text;
						} 
						else 
						{
							state = ParserState.ProcessingStart;
						}
						break;
					default:
						break;
				}
			}	
		}
	
	}

}