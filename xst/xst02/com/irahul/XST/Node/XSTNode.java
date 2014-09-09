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

import com.irahul.XST.*;
import java.util.LinkedList;
import java.util.Hashtable;
import java.util.Enumeration;
import java.util.Stack;

/** Main XSTNode
* Sub classes derived from it
*
* @author Rahul
* @version Since v0.2
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
	
	/** TODO Recursive or Not */
	public boolean isRecursive()
	{
		return false;
	}
	
	/** Leaf or not - only applied to attribute and element */
	public boolean isLeaf()
	{
		XSTNode node=getDown();
		if(node==null || node.getTypeValue()==XSTConstants.TYPE_SIMPLE || node.getTypeValue()==XSTConstants.TYPE_COMPLEX) return true;
		else return false;
	}
	
	/** TODO Returns the ref node, only applicable for a recursive node*/
	public XSTNode getRef()
	{
		if (isRecursive())
		{
			return new XSTNode();
		}
		else
		{
			return null;
		}
	}
	
	/** returns root of this node (basically root of tree) */
	public XSTNode getRoot() throws Exception
	{
		XSTNode p;
		while ((p=getParent())!=null)
		{
			if(p.getTypeValue()==XSTConstants.TYPE_SCHEMA) return p;
		}
		throw new XSTException("NO ROOT - that is impossible!!");
		//return new XSTNode();
	}
	/** Parent */
	public XSTNode getParent()
	{
		XSTNode p=new XSTNode();
		XSTNode now=this;
		
		if (now.getLeft()!=null)
		{
			now=now.getLeft();
		}
		else
		{
			if(now.getUp()==null) return null;
			else p=now.getUp();
		}
		return p;
	}
	/** return list of children of this node, LL contains XSTNode */
	public LinkedList getChildren()
	{
		LinkedList ll=new LinkedList();
		if (getDown()!=null)
		{
			XSTNode down=getDown();
			ll.add(down);
			while (down.getRight()!=null)
			{
				ll.add(down.getRight());
				down=down.getRight();
			}
		}
		return ll;
	}
	
	/** return list of leaves for tree rooted at this node, LL contains XSTNode */
	public LinkedList getLeaves()
	{
		LinkedList nodeList=new LinkedList();
		Stack s =new Stack();
		LinkedList l=getChildren();
		for(int i=0;i<l.size();i++) s.push(l.get(i));
		while (!s.isEmpty())
		{
			XSTNode n=(XSTNode)s.pop();
			if (n.getDown()==null)
			{
				nodeList.add(n);
			}
			LinkedList li=n.getChildren();
			for(int i=0;i<li.size();i++) s.push(li.get(i));
		}
		return nodeList;
	}
	
	/** return list containing XSTNode which define path from x to this node
	 * NOTE - Path is NOT directly as in XST, must use parent relation
	 */
	public LinkedList getPath(XSTNode x)
	{
		LinkedList nodeList=new LinkedList();
		boolean flag=true;
		XSTNode now=this;
		XSTNode p;
		nodeList.add(now);
		while ((p=now.getParent())!=null && flag)
		{
			nodeList.add(p);
			if (p.equals(x)) flag=false;
			else
			{
				LinkedList children=p.getChildren();
				for(int i=0;i<children.size();i++)
				{
					if(children.get(i).equals(x)) flag=false;
				}
				now=p;
			}	
		}
		nodeList.add(x);
		return reverse(nodeList);
	}
	private LinkedList reverse(LinkedList l)
	{
		LinkedList nw=new LinkedList();
		for(int i=l.size()-1;i>=0;i--)
		{
			nw.add(l.get(i));
		}
		return nw;
	}
	/** NOT IMPLEMENTED */
	public String getNamespace()
	{
		return null;
	}
	
	/** returns the minOccurs for this node - default value is 0 if not specified*/
	public int getMinOccurs()
	{
		String min=getValue(XSTConstants.ATTR_MINOCCURS);
		if(min==null) return 0;
		else return Integer.parseInt(min);
	}
	
	/** defalut value is 1 is not specified, unbounded=Integer.MAX_VALUE */
	public int getMaxOccurs()
	{
		String max=getValue(XSTConstants.ATTR_MAXOCCURS);
		if(max==null) return 1;
		if(max.equals("unbounded")) return Integer.MAX_VALUE;
		else return Integer.parseInt(max);
	}
	
	/** Only applies to Elements and Attributes */
	public boolean isTypeSimple(String NSP)
	{
		if (getTypeValue()==XSTConstants.TYPE_ELEMENT || getTypeValue()==XSTConstants.TYPE_ATTRIBUTE)
		{
			try
			{
				getDataType(NSP);
				return true;
			}
			catch(Exception e)
			{
				return false;
			}
		}
		return false;
	}
	
	/**  Only applies to Elements */
	public boolean isTypeComplex()
	{
		if (getTypeValue()==XSTConstants.TYPE_ELEMENT)
		{
			//anonomous complex types
			if (nodeDown!=null)
			{
				if(nodeDown.getTypeValue()==XSTConstants.TYPE_COMPLEX) return true;
			}
			//named complex type
			//String attrType=getValue(XSTConstants.ATTR_TYPE);
			// named have been reduced to anon so can ignore for now
		}
		return false;
	}
	
	/** The data type value from XSTConstants - only used for Simple Types
	 * For user -defined return base 
	 * @param NSP Name Space Prefix
	 */
	public int getDataType(String NSP) throws XSTException
	{
		String typeString=new String();
		if (nodeDown!=null)
		{
			if (nodeDown.getTypeValue()==XSTConstants.TYPE_SIMPLE)
			{
				typeString=nodeDown.getValue(XSTConstants.ATTR_BASE);
			}
			else typeString=getValue(XSTConstants.ATTR_TYPE);
		}
		else typeString=getValue(XSTConstants.ATTR_TYPE);
		
		if (NSP==null || NSP.length()==0)
		{
			//do nothing
		}
		else 
		{
			NSP+=":";
		}
		//System.out.println("NSP="+NSP+" typesr="+typeString);
		if (typeString.equals(NSP+"string")) return XSTConstants.DATA_STRING;
		if (typeString.equals(NSP+"boolean")) return XSTConstants.DATA_BOOLEAN;
		if (typeString.equals(NSP+"base64Binary")) return XSTConstants.DATA_BASE64BINARY;
		if (typeString.equals(NSP+"hexBinary")) return XSTConstants.DATA_HEXBINARY;
		if (typeString.equals(NSP+"float")) return XSTConstants.DATA_FLOAT;
		if (typeString.equals(NSP+"decimal")) return XSTConstants.DATA_DECIMAL;
		if (typeString.equals(NSP+"double")) return XSTConstants.DATA_DOUBLE;
		if (typeString.equals(NSP+"anyURI")) return XSTConstants.DATA_ANYURI;
		if (typeString.equals(NSP+"QName")) return XSTConstants.DATA_QNAME;
		if (typeString.equals(NSP+"NOTATION")) return XSTConstants.DATA_NOTATION;
		if (typeString.equals(NSP+"duration")) return XSTConstants.DATA_DURATION;
		if (typeString.equals(NSP+"dateTime")) return XSTConstants.DATA_DATETIME;
		if (typeString.equals(NSP+"time")) return XSTConstants.DATA_TIME;
		if (typeString.equals(NSP+"date")) return XSTConstants.DATA_DATE;
		if (typeString.equals(NSP+"gYearMonth")) return XSTConstants.DATA_GYEARMONTH;
		if (typeString.equals(NSP+"gYear")) return XSTConstants.DATA_GYEAR;
		if (typeString.equals(NSP+"gMonthDay")) return XSTConstants.DATA_GMONTHDAY;
		if (typeString.equals(NSP+"gDay")) return XSTConstants.DATA_GDAY;
		if (typeString.equals(NSP+"gMonth")) return XSTConstants.DATA_GMONTH;
		throw new XSTException ("Invalid PRIMITIVE Type request");
	}
	
	/** Name of this node, does not apply to all types of nodes */
	public String getName()
	{
		return getValue(XSTConstants.ATTR_NAME);
	}
	
}
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                        

























































































































































