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
/** SMS Text object
* @author Rahul (http://www.irahul.com)
* @version 0.1
*/

public class SMSText implements SMS
{
	private int index;//index of msg
	private String stat;//status of msg
	private String daoa;//dest or origin addr
	private String dtm;//msg dtm
	private String data;//msg txt
	
	public SMSText(int index,String stat,String daoa,String dtm,String data)
	{
		this.index=index;
		this.stat=stat;
		this.daoa=daoa;
		this.dtm=dtm;
		this.data=data;
	}
	public int getIndex(){return this.index;}
	public String getStat(){return this.stat;}
	public String getDAOA(){return this.daoa;}
	public String getDTM(){return this.dtm;}
	public String getData(){return this.data;}
	/** to String */
	public String toString()
	{
		return new String("Index="+this.index+" stat="+this.stat+" daoa="+this.daoa+" dtm="+this.dtm+" txt="+this.data);
	}
	
	
}