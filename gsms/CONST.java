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
 
/** Constants for GSMS 
* @author Rahul (http://www.irahul.com)
* @version 0.1
*/
import javax.comm.SerialPort;

public class CONST
{
	/**timeout in milliseconds while attempting to connect to port*/
	public static final int CONNECT_TIMEOUT=2000;
	/** connect baud rate */
	public static final int CONNECT_BAUD_9600=9600;
	/**Values to connect modem*/
	public static final int DATABITS=SerialPort.DATABITS_8;
	public static final int STOPBITS=SerialPort.STOPBITS_1;
	public static final int PARITY=SerialPort.PARITY_NONE;
	public static final int FLOWCONTROL=SerialPort.FLOWCONTROL_NONE;
	/** Time to wait for modem response before querying reply*/
	public static final long WAIT_TIME=1000;
	
}