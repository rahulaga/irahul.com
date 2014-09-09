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


/** making AT commands easier to use 
* @author Rahul (http://www.irahul.com)
* @version 0.1
*/

public class AT
{
	/** Init modem:AT"*/
	public static String _AT(){	return new String("AT\r");}
	/** Send SMS: AT+CMGS<da><CR><msg><Ctrl+Z><CR>"*/ 
	public static String CMGS(String da,String msg)
	{
		return new String ("AT+CMGS="+da+"\r"+msg+"\u001A\r");
	}
	/** Echo off: ATE0*/
	public static String echoOff(){return new String("ATE0\r");}
	/** Echo on: ATE1*/
	public static String echoOn(){return new String("ATE1\r");}
	
	/* Character set HEX GSM PCCP437 etc*/
	public static String setCharSet(String name){return new String("AT+CSCS=\""+name+"\"\r");}
	
	/** Mode text*/
	public static String setModeText(){return new String("AT+CMGF=1\r");}
	/** Mode PDU*/
	public static String setModePDU(){return new String("AT+CMGF=0\r");}
	
	/** Get all sms: AT+CMGL="ALL"*/
	public static String showAllSMS(){return new String("AT+CMGL=\"ALL\"\r");}
	
	/** Delete Msg*/
	public static String delSMS(int index){return new String("AT+CMGD="+index+"\r");}
}