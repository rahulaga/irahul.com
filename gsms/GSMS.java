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
/** Using GSMS -> GSM+SMS = GSMS
* @author Rahul (http://www.irahul.com)
* @version 0.1
*/
import java.io.*;
import java.util.*;
import javax.comm.*;

public class GSMS
{
	public static void main(String[] args)
	{
		GSMModem myModem;
		try
		{
			//
			//
			//SEND
			//
			//connect
			myModem=new GSMModem("COM2",CONST.CONNECT_BAUD_9600);
			//init
			myModem.init();
			//see wat modem i have
			System.out.println(myModem.specs());
			//send sms
			myModem.setSMSMode(1);//send as text
			myModem.flushInput();//clear reply in input stream
			System.out.print("Sending...");
			//myModem.sendSMS("99999999","sending as a text");
			System.out.println(myModem.reply());
			//close connection
			myModem.disconnect();
			//
			//
			//RECV   
			Thread.sleep(5000);//just to relax sim card for a while =), no need if using independently
			//
			//connect
			myModem=new GSMModem("COM2",CONST.CONNECT_BAUD_9600);
			//init
			myModem.init();
			//show sms on sim
			LinkedList ll=myModem.showAllSMS();
			if(ll.size()==0) System.out.println("No msgs");
			for(int i=0;i<ll.size();i++)
				System.out.println(ll.get(i));
			
			//how to delete
			//myModem.deleteSMS(((SMSText)ll.get(0)).getIndex());
				
			//close connection
			myModem.disconnect();
		}catch(Exception e)	{System.out.println(e);	}
	}
}
	