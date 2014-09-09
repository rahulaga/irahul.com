/*
 * Copyright (c) 2003 Rahul (http://www.irahul.com) All rights 
 * reserved.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions
 * are met:
 *
 * 1. Redistributions of source code must retain the above copyright
 *    notice, this list of conditions and the following disclaimer. 
 *
 * 2. Redistributions in binary form must reproduce the above copyright
 *    notice, this list of conditions and the following disclaimer in
 *    the documentation and/or other materials provided with the
 *    distribution.
 *
 * THIS SOFTWARE IS PROVIDED ``AS IS'' AND ANY EXPRESSED OR IMPLIED
 * WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
 * OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
 * DISCLAIMED.  IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY DIRECT,
 * INDIRECT, INCIDENTAL,
 * SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
 * LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF
 * USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
 * ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
 * OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT
 * OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF
 * SUCH DAMAGE.
 */ 
/** Connecting to a GSM Modem to send SMS 
* @author Rahul (http://www.irahul.com)
* @version 0.1
*/
import java.io.*;
import java.util.*;
import javax.comm.*;

public class GSMModem implements SerialPortEventListener
{
	
	private CommPortIdentifier portId; //id in use
	private SerialPort serialPort;//serial port in use
	private OutputStream outputStream;
	private InputStream inputStream;
	private int SMSmode=1;//default mode=text
	/** Create a modem object to use */
	public GSMModem(String port,int baud) throws Exception
	{
		Enumeration portList;
		boolean flag=true;
		portList = CommPortIdentifier.getPortIdentifiers();
		while (portList.hasMoreElements() && flag) {
			portId = (CommPortIdentifier) portList.nextElement();
			if (portId.getPortType() == CommPortIdentifier.PORT_SERIAL) {
				if (portId.getName().equals(port)) {
					//found port specified
					try {
						//open port
						this.serialPort = (SerialPort)portId.open("GSMS", CONST.CONNECT_TIMEOUT);
						this.serialPort.setSerialPortParams(baud,CONST.DATABITS,CONST.STOPBITS,CONST.PARITY);
						//get streams
						this.outputStream = serialPort.getOutputStream();
						this.inputStream = serialPort.getInputStream();
						//set all notification bits
						this.serialPort.notifyOnBreakInterrupt(true);
						this.serialPort.notifyOnCarrierDetect(true);
						this.serialPort.notifyOnCTS(true);
						this.serialPort.notifyOnDataAvailable(true);
						this.serialPort.notifyOnDSR(true);
						this.serialPort.notifyOnFramingError(true);
						this.serialPort.notifyOnOutputEmpty(true);
						this.serialPort.notifyOnOverrunError(true);
						this.serialPort.notifyOnParityError(true);
						this.serialPort.notifyOnRingIndicator(true);
						//listen to events on port
						this.serialPort.addEventListener(this);
						//set other parameters
						this.serialPort.setFlowControlMode(CONST.FLOWCONTROL);
					}
					//catch (IOException e){ throw new GSMSException("Invalid operation on port");}
					catch (PortInUseException e) {throw new GSMSException("Port already in use");}
					catch (UnsupportedCommOperationException e) {throw new GSMSException("Unknown Operation on Port");}
					finally {flag=flag;}
				}
			}
			
		}//while
	}
	/** Close connection to port*/
	public synchronized void disconnect(){this.serialPort.close();}
	/** Init modem - recommended to use before any other command*/
	public synchronized void init() throws GSMSException
	{
		this.exec(AT._AT());
		this.exec(AT.echoOff());
		this.setSMSMode(1);
		this.flushInput();
	}
	/** Set Msg mode 0=Text, 1=PDU*/
	public synchronized void setSMSMode(int a) throws GSMSException
	{
		this.SMSmode=a;
		switch (a)
		{
		case 1:
			this.exec(AT.setModeText());
			break;
		case 0:
			this.exec(AT.setModePDU());
			break;
		}
	}
	/** Get Modem details*/
	public synchronized String specs()
	{
		String specs;
		try
		{	
			specs="The modem connected is:";
			this.exec("AT+CGMI\r");
			specs+=this.removeStr("OK",this.reply());
			this.exec("AT+CGMM\r");
			specs+="\nModel:"+this.removeStr("OK",this.reply());
			this.exec("AT+CGSN\r");
			specs+="\nSerial Number:"+this.removeStr("OK",this.reply());
			this.exec("AT+CSQ\r");
			specs+="\nSignal Quality:"+this.removeStr("OK",this.reply())+"%";
			specs+="\non Port="+serialPort.getName();
			specs+=" at Baud Rate="+serialPort.getBaudRate();
		}catch(Exception e)
		{
			specs="Unable to query modem";
		}
		return specs;
	}
	/** Flush input stream*/
	public synchronized void flushInput() throws GSMSException
	{
		//clear incoming
		this.reply();
	}
	/** Basic method to execute a AT Command, Use full string*/
	public synchronized void exec(String AT_COMMAND) throws GSMSException
	{
		try
		{
			//clear
			this.flushInput();
			//send command
			this.outputStream.write(AT_COMMAND.getBytes());
			this.outputStream.flush();
		}catch(IOException e){throw new GSMSException("Invalid operation on port");}
	}
	/** Get reply from modem */
	public synchronized String reply() throws GSMSException
	{
		String replyStr=new String();
		try
		{
			//wait for command to execute
			Thread.sleep(CONST.WAIT_TIME);
			//get return value
			int numWaiting=inputStream.available();
			if (numWaiting>0)
			{
				byte[] bstr=new byte[numWaiting];
				inputStream.read(bstr,0,numWaiting);
				replyStr=new String(bstr);
			}
			return replyStr;
		}catch(IOException e){throw new GSMSException("Error querying modem");}
		catch(InterruptedException e){throw new GSMSException("Error in Thread.sleep");}
	}
	/** Send a SMS */
	public synchronized void sendSMS(String mno,String msg) throws GSMSException
	{
		if (this.SMSmode==1)
		{
			//send as text
			this.exec(AT.setCharSet("HEX"));
			this.exec(AT.CMGS(mno,this.ascii2Hex(msg)));
		}
		else if (this.SMSmode==0)
		{
			//send as PDU
			//TODO
		}
	}
	/** Show all SMS on sim card
	* return List contains objects of type SMS
	*/
	public synchronized LinkedList showAllSMS() throws GSMSException
	{
		LinkedList smsList=new LinkedList();
		//MODE MUST be text to cmgl
		this.exec(AT.setModeText());
		//chr set hex to get funny characters as well
		this.exec(AT.setCharSet("HEX"));
		//get list of text
		this.flushInput();
		this.exec(AT.showAllSMS());
		String str2parse=this.reply();
		//make sense of reply - convert to SMS Object
		//reply is +CMGL: <imdex>,<stat><da/oa>,[][],<dtm><CR><LF><data>
		StringTokenizer st=new StringTokenizer(str2parse,"\r\n");
		for(int i=0;i<st.countTokens()-1;i++)
		{
			String HEAD=st.nextToken();
			String DATA=this.hex2ascii(st.nextToken());
			StringTokenizer st1=new StringTokenizer(HEAD,",");
			String INDEX=st1.nextToken();INDEX=INDEX.substring(7).trim();
			String STAT=st1.nextToken();STAT=STAT.substring(1,STAT.length()-1);
			String DAOA=st1.nextToken();DAOA=DAOA.substring(1,DAOA.length()-1);
			//now can have optional header
			//not for my modem so havent taken care of here :)
			//
			String DTM=st1.nextToken()+st1.nextToken();			
			if (this.SMSmode==1)
			{
				//create text
				SMSText sms=new SMSText(Integer.parseInt(INDEX),STAT,DAOA,DTM,DATA);
				smsList.add(sms);
			}
			else if (this.SMSmode==0)
			{
				//make PDU
				//TODO
			}
		}
		return smsList;
	}
	/** delete specific SMS on sim*/
	public synchronized void deleteSMS(int index) throws GSMSException
	{
		//del msg
		this.exec(AT.delSMS(index));
	}
	
	/** Remove <this> string <in> */
	private synchronized String removeStr(String thisStr,String inStr)
	{
		String replaced=new String();
		String temp;
		StringTokenizer st=new StringTokenizer(inStr,"\r\n");
		while (st.hasMoreTokens())
		{
			temp=st.nextToken();
			if (!temp.equals(thisStr)) replaced+=temp;
		}
		return replaced;
	}
	//########################################################
	// GSM 7 Bit representation
	//
	private final String alphabet = "@£$•!!!!!!!!!!@@ƒ_÷√ÀŸ–ÿ”»Œ@@@@@ !\"#§%&\'()*+,-./0123456789:;<=>?iABCDEFGHIJKLMNOPQRSTUVWXYZ@@@@ß?abcdefghijklmnopqrstuvwxyz@@@@@";
	
	/** ASCII to GSM 7 BIT - HEX charset*/
	public synchronized String ascii2Hex(String ascii)
	{
		String hex = new String();
		for (int i = 0; i < ascii.length(); i ++)
		{
			switch (ascii.charAt(i))
			{
				case '¡': case '·': case '‹':
					hex = hex + char2Hex('A');
					break;
				case '¬': case '‚':
					hex = hex + char2Hex('B');
					break;
				case '√': case '„':
					hex = hex + char2Hex('√');
					break;
				case 'ƒ': case '‰':
					hex = hex + char2Hex('ƒ');
					break;
				case '≈': case 'Â': case '›':
					hex = hex + char2Hex('E');
					break;
				case '∆': case 'Ê':
					hex = hex + char2Hex('Z');
					break;
				case '«': case 'Á': case 'ﬁ':
					hex = hex + char2Hex('H');
					break;
				case '»': case 'Ë':
					hex = hex + char2Hex('»');
					break;
				case '…': case 'È': case 'ﬂ':
					hex = hex + char2Hex('I');
					break;
				case ' ': case 'Í':
					hex = hex + char2Hex('K');
					break;
				case 'À': case 'Î':
					hex = hex + char2Hex('À');
					break;
				case 'Ã': case 'Ï':
					hex = hex + char2Hex('M');
					break;
				case 'Õ': case 'Ì':
					hex = hex + char2Hex('N');
					break;
				case 'Œ': case 'Ó':
					hex = hex + char2Hex('Œ');
					break;
				case 'œ': case 'Ô': case '¸':
					hex = hex + char2Hex('O');
					break;
				case '–': case '':
					hex = hex + char2Hex('–');
					break;
				case '—': case 'Ò':
					hex = hex + char2Hex('P');
					break;
				case '”': case 'Û': case 'Ú':
					hex = hex + char2Hex('”');
					break;
				case '‘': case 'Ù':
					hex = hex + char2Hex('T');
					break;
				case '’': case 'ı': case '˝':
					hex = hex + char2Hex('Y');
					break;
				case '÷': case 'ˆ':
					hex = hex + char2Hex('÷');
					break;
				case '◊': case '˜':
					hex = hex + char2Hex('X');
					break;
				case 'ÿ': case '¯':
					hex = hex + char2Hex('ÿ');
					break;
				case 'Ÿ': case '˘': case '˛':
					hex = hex + char2Hex('Ÿ');
					break;
				default:
					hex = hex + char2Hex(ascii.charAt(i));
					break;
			}
		}
		return hex;
	}
	/** Helper for above */
	private String char2Hex(char c)
	{
		for (int i = 0; i < alphabet.length(); i ++)
		{
			if (alphabet.charAt(i) == c) return (i <= 15 ? "0" + Integer.toHexString(i) : Integer.toHexString(i)); 
		}
		return "00";
	}
	/** GSM 7 Bit to ASCII */
	public synchronized String hex2ascii(String hex)
	{
		String ascii = new String();

		for (int i = 0; i < hex.length(); i += 2)
		{
			String hexChar = "" + hex.charAt(i) + hex.charAt(i + 1);
			int c = Integer.parseInt(hexChar, 16);
			ascii = ascii + alphabet.charAt((char) c);
		}
		return ascii;
	}
	
	//interface implementation
	/** event listener - respond according to event on port*/
	public synchronized void serialEvent(SerialPortEvent event)
	{
		//TODO
		switch(event.getEventType())
		{
			case SerialPortEvent.BI:
				break;
			case SerialPortEvent.CD:
				break;
			case SerialPortEvent.CTS:
				break;
			case SerialPortEvent.DATA_AVAILABLE:
				break;
			case SerialPortEvent.DSR:
				break;
			case SerialPortEvent.FE:
				break;
			case SerialPortEvent.OE:
				break;
			case SerialPortEvent.OUTPUT_BUFFER_EMPTY:
				break;
			case SerialPortEvent.PE:
				break;			
			case SerialPortEvent.RI:
				break;			
		}
	}//end interface
}