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

package com.irahul.XST;

import com.irahul.XST.Node.*;

import org.w3c.dom.Document;
import org.w3c.dom.Attr;
import org.w3c.dom.Node;
import org.w3c.dom.Element;
import org.w3c.dom.NodeList;
import org.w3c.dom.NamedNodeMap;
import org.w3c.dom.Text;


/**
* XML Schema Tree
*
* @author Rahul
* @version Since v0.1
*
*/
public class XST
{
	private XSTNode head;//root pointer
	private XSTNode current;//current pointer
	private int countElement=0;
	private int countAttribute=0;
	private String NSPrefix=new String();
	private boolean debug=true; //set false to disable debug print
	private XSGrammar xsg=new XSGrammar();
	/**
	  * Constructor for class XST
	  * It contains a pointer called <code>current</code> pointing to the present XSTNode in question
	  * To traverse the tree, call the appropriate methods which in turn move current.
	  */
	public XST()
	{
		//constructor
		this.head=new XSTNodeSchema();//root node must be of type SCHEMA
		this.current=this.head;
	}
	/**
	* Sets current to head
	*/
	public void reset()
	{
		this.current=this.head;
	}
	
	/**
	  * Adds an XSTNode to the left of the node where <code> current </code> is pointing
	  * @param node Type:XSTNode.Specifies the node being added to the XST e.g. an element or attribute
	  * @return Returns <code>true</code> if the operation is successful, else <code>false</code>.
	  */
	public boolean addLeft(XSTNode node)
	{
		if (current.equals(head))
		{
			return false;
		}
		else
		{
			current.insertLeft(node);
			//current = node;
			return true;
		}
	}
	
	/**
	  * Adds an XSTNode to the right of the node where <code> current </code> is pointing
	  * @param node Type:XSTNode.Specifies the node being added to the XST e.g. an element or attribute
	  * @return Returns <code>true</code> if the operation is successful, else <code>false</code>.
	  */
	public boolean addRight(XSTNode node)
	{
		if (current.equals(head))
		{
			return false;
		}
		else
		{
			current.insertRight(node);
			//current = node;
			return true;
		}
	}
	
	/**
	  * Adds an XSTNode UP (above) of the node where <code> current </code> is pointing
	  * @param node Type:XSTNode.Specifies the node being added to the XST e.g. an element or attribute
	  * @return Returns <code>true</code> if the operation is successful, else <code>false</code>.
	  */
	public boolean addUp(XSTNode node)
	{
		if (current.equals(head))
		{
			return false;
		}
		else
		{
			current.insertUp(node);
			//current = node;
			return true;
		}
	}
	
	/**
	  * Adds an XSTNode DOWN (Below) of the node where <code> current </code> is pointing
	  * @param node Type:XSTNode.Specifies the node being added to the XST e.g. an element or attribute
	  * @return Returns <code>true</code> if the operation is successful, else <code>false</code>.
	  */
	public boolean addDown(XSTNode node)
	{
		current.insertDown(node);
		//current = node;
		return true;
	}
	//###########################################
	//####
	//#### Above 4 methods are most generic, other add methods created
	//#### are abstracted on these
	//####
	//##########################################
	
	/**
	  * Deletes an XSTNode where <code> current </code> is pointing; This
	  * is a complex task, since need to take care of pointers correctly, current is set to root
	  * @return Returns <code>true</code> if the operation is successful, else <code>false</code>.
	  */
	public boolean deleteNode()
	{
		if (current.equals(head) || current==null)
		{
			return false;
		}
		else
		{
			current.deleteNode();
			reset();
			return true;
		}
	}
	
	/**
	  * Shifts <code>current</code> to the left.
	  * @return Returns <code>true</code> if the left traversal is successful, <code>false</code> otherwise.
	  */
	public boolean goLeft()
	{
		if (hasLeft())
		{
			current=current.getLeft();
			return true;
		}
		else
		{
			return false;
		}
	}
	
	/**
	  * Shifts <code>current</code> to the Right.
	  * @return Returns <code>true</code> if the left traversal is successful, <code>false</code> otherwise.
	  */
	public boolean goRight()
	{
		if (hasRight())
		{
			current=current.getRight();
			return true;
		}
		else
		{
			return false;
		}
	}
	
	/**
	  * Shifts <code>current</code> Up.
	  * @return Returns <code>true</code> if the left traversal is successful, <code>false</code> otherwise.
	  */
	public boolean goUp()
	{
		if (hasUp())
		{
			current=current.getUp();
			return true;
		}
		else
		{
			return false;
		}
	}
	
	/**
	  * Shifts <code>current</code> Down.
	  * @return Returns <code>true</code> if the left traversal is successful, <code>false</code> otherwise.
	  */
	public boolean goDown()
	{
		if (hasDown())
		{
			current=current.getDown();
			return true;
		}
		else
		{
			return false;
		}
	}
	
	//####### As for add other traversals are abstracted on these
	
	
	/**
	  * Checks for node Left of <code>current</code>
	  * @return Returns <code>true</code> if the left traversal is successful, <code>false</code> otherwise.
	  */
	public boolean hasLeft()
	{
		if(current.getLeft()==null) return false; else return true;	
	}
	
	/**
	  * Checks for node Right of <code>current</code>
	  * @return Returns <code>true</code> if the left traversal is successful, <code>false</code> otherwise.
	  */
	public boolean hasRight()
	{
		if(current.getRight()==null) return false; else return true;
	}
	
	/**
	  * Checks for node Up of <code>current</code>
	  * @return Returns <code>true</code> if the left traversal is successful, <code>false</code> otherwise.
	  */
	public boolean hasUp()
	{
		if(current.getUp()==null) return false; else return true;
	}
	
	/**
	  * Checks for node Down of <code>current</code>
	  * @return Returns <code>true</code> if the left traversal is successful, <code>false</code> otherwise.
	  */
	public boolean hasDown()
	{
		if(current.getDown()==null) return false; else return true;
	}
	
	/**
	  * @return Returns <code>current</code> node.
	  */
	public XSTNode getCurrent()
	{
		return this.current;
	}
	/** @return Root of this tree */
	public XSTNode getRoot()
	{
		return this.head;
	}
	
	/**
	* Main Tree Create Method
	* A tree consists of list of XSTNode which are in turn a XST
	*/
	public void createTree(Node node, XSTNode rootedAt) throws Exception
	{
		boolean nsSuccess=false,flagNode=false,flagAttr=false;
		//create tree of org.w3c.dom.Node object
		//which is a parsed XSD file
		//recieves the full parsed document -> org.w3c.dom.Document
		//
		// is there anything to do?
        if (node == null) {
           throw new XSTException("Failed in XST.createTree, empty document object");
        }
		else
		{
			//traverse and create XST object from this DOM
			int nodetype = node.getNodeType();
	        switch (nodetype) {
	            case Node.DOCUMENT_NODE: {
					//if createTree method not called with root then this
					//is called, otherwise case will never happen
					//this is top-level, even above <xsd:schema>
					//System.out.println("Document Node:"+node.getNodeName());
	                Document document = (Document)node;
	                createTree(document.getDocumentElement(),getRoot());//now traversing root
					// root is <xsd:schema>
	                break;
	            }
				//actual start to tree creation
	            case Node.ELEMENT_NODE: {
					//special check for <schema>, countElement==0
					//mean MUST be <schema> else not a valid Schema
					if (countElement==0)
					{
	                	XSTNodeSchema xsSchema=(XSTNodeSchema)current;
						NamedNodeMap attrs = node.getAttributes();
						//get namespace first - currently can handle only one namespace
	                	if (attrs == null) throw new XSTException("No Namespace");
						else
						{
							for(int i=0;i<attrs.getLength();i++)
							{
								Attr curr=(Attr)attrs.item(i);
								if(curr.getName().substring(0,6).equals("xmlns:")){
									xsSchema.setAttrValue(XSTConstants.ATTR_XMLNS,curr.getValue());
									this.NSPrefix=curr.getName().substring(6);
									//if(debug) System.out.println("prefix="+this.NSPrefix+" URN="+xsSchema.getValue(XSTConstants.ATTR_XMLNS));
									nsSuccess=true;
								}
							}
							if(!nsSuccess) throw new XSTException("No Namespace");
							if(!node.getNodeName().equals(NSPrefix+":schema")) throw new XSTException("Root is not <schema>");
							countElement++;
							/*
							Node child=node.getFirstChild();
							while (child!=null)
							{
								createTree(child);
								child=child.getNextSibling();
							}
							*/
							createTree(node,getRoot());
						}
	                }
					else
					{
						//generic XSD tag handling
						//Note:: XSGrammar and each sub-class of XSTNode
						//Imp grammar info to validate got from these sources
						//if(debug) System.out.println("processing node "+node.getNodeName());
						
						NodeList children=node.getChildNodes();
						countElement+=children.getLength();
						
						for(int itr=0;itr<children.getLength();itr++)
						{
							Node currChild=children.item(itr);
							//create XSTNode for currChild
							flagNode=false;
							for(int k=0;k<XSGrammar.structName.length;k++)
							{
								if (currChild.getNodeName().equals(NSPrefix+":"+XSGrammar.structName[k]))
								{
									//now create node
									flagNode=true;
									XSTNode newNode=new XSTNode();
									XSTNode o = (XSTNode)Class.forName("com.irahul.XST.Node."+XSGrammar.structClass[k]).newInstance();
									//
									//cast into sub-class -> dunno how to do this using auto so this if-then :(
									if (XSGrammar.structType[k]==XSTConstants.TYPE_ANNOTATION) {newNode=(XSTNodeAnnotation) o;}
									if (XSGrammar.structType[k]==XSTConstants.TYPE_APPINFO) {newNode=(XSTNodeAppinfo) o;}
									if (XSGrammar.structType[k]==XSTConstants.TYPE_ATTRIBUTE) {newNode=(XSTNodeAttribute) o;}
									if (XSGrammar.structType[k]==XSTConstants.TYPE_ATTRIBUTE_GROUP) {newNode=(XSTNodeAttributeGroup) o;}
									if (XSGrammar.structType[k]==XSTConstants.TYPE_COMPLEX) {newNode=(XSTNodeComplex) o;}
									if (XSGrammar.structType[k]==XSTConstants.TYPE_DOCUMENTATION) {newNode=(XSTNodeDocumentation) o;}
									if (XSGrammar.structType[k]==XSTConstants.TYPE_ELEMENT) {newNode=(XSTNodeElement) o;}
									if (XSGrammar.structType[k]==XSTConstants.TYPE_ENUMERATION) {newNode=(XSTNodeEnumeration) o;}
									if (XSGrammar.structType[k]==XSTConstants.TYPE_RESTRICTION) {newNode=(XSTNodeRestriction) o;}
									if (XSGrammar.structType[k]==XSTConstants.TYPE_SCHEMA) {newNode=(XSTNodeSchema) o;}
									if (XSGrammar.structType[k]==XSTConstants.TYPE_SEQUENCE) {newNode=(XSTNodeSequence) o;}
									if (XSGrammar.structType[k]==XSTConstants.TYPE_SIMPLE) {newNode=(XSTNodeSimple) o;}
									//
									//if (debug) System.out.println("Node Class="+newNode.getClass().getName());
									//set and verify attr of newNode
									NamedNodeMap attrs = currChild.getAttributes();
									//if(attrs.getLength()==0) flagAttr=true;
									Attr curr;
									for(int p=0;p<attrs.getLength();p++)
									{
										curr=(Attr)attrs.item(p);
										flagAttr=false;
										for(int q=0;q<XSGrammar.attrName.length;q++)
										{
											if(curr.getName().equals(XSGrammar.attrName[q]) && newNode.isValidAttr(XSGrammar.attrValue[q])){
												//found attr, set in hashtable of node
												flagAttr=true;
												newNode.setAttrValue(XSGrammar.attrValue[q],curr.getValue());
											}
										}
										if(!flagAttr) throw new XSTException("Invalid or Not Surrpoted attribute \""+curr.getName()+"\" in structure \""+currChild.getNodeName()+"\"");
									}
									//newNode now represents currChild
									//
									//this is child of rootedAt
									//
									//chk if newNode is valid child
									if(!rootedAt.isValidContent(newNode.getTypeValue())) throw new XSTException("Invalid or Not Supported child in structure \""+node.getNodeName()+"\"");
									if (rootedAt.getDown()!=null)
									{
										XSTNode temp=rootedAt.getDown();
										while (temp.getRight()!=null)
										{
											temp=temp.getRight();
										}
										temp.insertRight(newNode);
									}
									else
									{
										rootedAt.insertDown(newNode);
									}
									//recurse
									createTree(currChild,newNode);
								}
							}
						}
						
					}//end else
	            }
	            case Node.TEXT_NODE: {
					//Text text = (Text)node;
					//System.out.println("Text Node:="+node.getNodeName()+" data="+text.getData());
	                break;
	            }
				
	        }				
		}
	}//createTree(Document)
	
	/** Print this XSTree as a String - useful for debug */
	public String toString()
	{
		String tree=new String();
		reset();
		//tree+="NODE:"+current.toString()+"\n";
		//if(current.getDown()!=null) tree+=current.getDown().toString();
		printTree(current);
		return tree;
	}
	private void printTree(XSTNode node)
	{
		System.out.println("NODE="+node.toString()+"\n");
		if(node.getDown()!=null) printTree(node.getDown());
		//if(node.getUp()!=null) printTree(node.getUp());
		//if(node.getLeft()!=null) printTree(node.getLeft());
		if(node.getRight()!=null) printTree(node.getRight());
	}
	
	
	/**
	* Prints out as is the Document -> Node (Node is superinterface) given;
	* Only for debug, not related to XST in any way
	* <br><br>
	* INCOMPLETE
	*
	*/
	public void toString(Node node)
	{
		if (node == null) {
           System.out.println("Empty node given");
        }

        int type = node.getNodeType();
        switch (type) {
            case Node.DOCUMENT_NODE: {
				//this is top-level, even above <xsd:schema>
				//System.out.println("Document Node:"+node.getNodeName());
                Document document = (Document)node;
                toString(document.getDocumentElement());//now traversing root
				// root is <xsd:schema>
                break;
            }
			//actual start
            case Node.ELEMENT_NODE: {
                System.out.println("Element Node:"+node.getNodeName());
                NamedNodeMap attrs = node.getAttributes();
                if (attrs != null) {
                    //System.out.println("Element Node:"+node.getNodeName()+" Num Attr in this node="+attrs.getLength());
					for(int i=0;i<attrs.getLength();i++)
					{
						Attr curr=(Attr)attrs.item(i);
						System.out.println(curr.getName()+"="+curr.getValue());
					}
                }
            }
			
            case Node.ENTITY_REFERENCE_NODE: {
                Node child = node.getFirstChild();
                while (child != null) {
                    toString(child);
                    child = child.getNextSibling();
                }
                break;
            }
			
            case Node.CDATA_SECTION_NODE: {
                //fCharacters += ((Text)node).getLength();
                break;
            }

            case Node.TEXT_NODE: {
				//Text text = (Text)node;
				//System.out.println("Text Node:="+node.getNodeName()+" data="+text.getData());
                break;
            }
        }
		
	}//end toString(Node)
}