/*
 * XML Schema Tree
 *
 * Copyright (c) 2002 Rahul
 * All rights reserved.
 * http://www.irahul.com
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
 * DISCLAIMED.  IN NO EVENT SHALL THE APACHE SOFTWARE FOUNDATION OR
 * ITS CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
 * SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
 * LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF
 * USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
 * ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
 * OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT
 * OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF
 * SUCH DAMAGE.
 * ====================================================================
 *
 */

package com.irahul.XST.Node;

import com.irahul.XST.XSGrammar;
import java.util.Hashtable;
import java.util.Enumeration;

/** Main XSTNode
* Sub classes derived from it
*
* @author Rahul
* @version Since v0.1
*
*/

public class XSTNode
{
	protected XSTNode nodeLeft,nodeRight,nodeUp,nodeDown;
	protected int nodeType;
	protected Hashtable attr;
	/** DUMMY - DO NOT USE */
	public XSTNode()
	{
	}
	/**
	  * Constructor for class XSTNode
	  */
	public XSTNode(int type)
	{
		this.nodeType=type;
		this.nodeUp=null;
		this.nodeDown=null;
		this.nodeLeft=null;
		this.nodeRight=null;
		this.attr=new Hashtable();
	}
	/**
	  * Returns the XST node at a level above the one on which this method is called
	  */
	public XSTNode getUp()
	{
		return this.nodeUp;
	}

	/**
	  * Returns the XST node at a level below the one on which this method is called
	  */
	public XSTNode getDown()
	{
		return this.nodeDown;
	}

	/**
	  * Returns the XST node to the left of the one on which this method is called
	  */
	public XSTNode getLeft()
	{
		return this.nodeLeft;
	}

	/**
	  * Returns the XST node to the right of the one on which this method is called
	  */
	public XSTNode getRight()
	{
		return this.nodeRight;
	}

	/**
	  * Returns the type of this node.
	  */
	public int getTypeValue()
	{
		return this.nodeType;
	}
	
	/**
	  * Return Reverse mapped type for this node
	  */
	public String getType()
	{
		XSGrammar xsg=new XSGrammar();
		return xsg.rMapStruct(getTypeValue());
	}

	/**
	  * Inserts a node above this XSTNode
	  * @param up Type:XSTNode The node to be inserted above in the XST
	  */
	public void insertUp(XSTNode up)
	{
		up.nodeUp=this.nodeUp;
		if(this.nodeUp != null)
			this.nodeUp.nodeDown= up;
		this.nodeUp=up;
		up.nodeDown=this;
	}

	/**
	  * Inserts a node below this XSTNode
	  * @param down Type:XSTNode The node to be inserted below in the XST
	  */
	public void insertDown(XSTNode down)
	{
		down.nodeDown=this.nodeDown;
		if(this.nodeDown != null)
			this.nodeDown.nodeUp= down;
		this.nodeDown=down;
		down.nodeUp=this;
	}

	/**
	  * Inserts a node to the right of this XSTNode
	  * @param right Type:XSTNode The node to be inserted to the right in the XST
	  */
	public void insertRight(XSTNode right)
	{
		right.nodeRight=this.nodeRight;
		if(this.nodeRight != null)
			this.nodeRight.nodeLeft= right;
		this.nodeRight= right;
		right.nodeLeft = this;
	}


	/**
	  * Inserts a node to the left of this XSTNode
	  * @param left Type:XSTNode The node to be inserted to the left in the XST
	  */
	public void insertLeft(XSTNode left)
	{
		left.nodeLeft=this.nodeLeft;
		if(this.nodeLeft != null)
			this.nodeLeft.nodeRight=left;
		this.nodeLeft= left;
		left.nodeRight= this;
	}

	/**
	  * Deletes <code>this</code> XSTNode from the XST
	  */
	public void deleteNode()
	{
		if(this.nodeLeft !=null)
			this.nodeLeft.nodeRight = this.nodeRight;
		if(this.nodeRight != null)
			this.nodeRight.nodeLeft = this.nodeLeft;
		if(this.nodeUp != null)
			this.nodeUp.nodeDown=this.nodeDown;
		if(this.nodeDown != null)
			this.nodeDown.nodeUp=this.nodeUp;
	}
	
	/** Set attr,value pair for Node */
	public void setAttrValue(int attrib,String value)
	{
		attr.put(new Integer(attrib),value);
	}
	
	/** Return value for required attr */
	public String getValue(int attrib)
	{
		return (String)attr.get(new Integer(attrib));
	}
	
	/** Print this Node - useful for debug */
	public String toString()
	{
		XSGrammar xsg=new XSGrammar();
		String desc=new String();
		desc+="Type="+this.getType()+" [Left] Type=";
		if(nodeLeft == null) desc+="NULL"; else desc+=nodeLeft.getType();
		desc+=" [Right] Type=";
		if(nodeRight==null) desc+="NULL"; else desc+=nodeRight.getType();
		desc+=" [Up] Type=";
		if(nodeUp==null) desc+="NULL"; else desc+=nodeUp.getType();
		desc+=" [Down] Type=";
		if(nodeDown==null) desc+="NULL"; else desc+=nodeDown.getType();
		desc+=" [Attr,Value pairs] ";
		Enumeration e = attr.keys();
		while (e.hasMoreElements())
		{
			Object o=e.nextElement();
			desc+=" ("+xsg.rMapAttr(Integer.parseInt(o.toString()))+","+(String)attr.get(o)+")";
		}
		return desc;
	}
	
	/** this is always over-ridden by sub-classes but created cos have to */
	public boolean isValidAttr(int x)
	{
		return false;
	}
	/** this is always over-ridden by sub-classes but created cos have to */
	public boolean isValidContent(int x)
	{
		return false;
	}
}
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                        

























































































































































