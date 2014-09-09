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
import java.util.Hashtable;
/**
* XML Schema Grammar Def, how much of XML is understood by XST,
* To extend it you need to create the approriate sub-classes of XSTNode
* and modify XST.createTree(DOM) accordingly.
*
* @author Rahul
* @version Since v0.2
*
*/
public class XSGrammar
{
	/** XML Schema understood - Keywords */
	public final static String[] structName={
			"schema",
			"annotation",
			"appinfo",
			"attribute",
			"attributeGroup",
			"complexType",
			"documentation",
			"element",
			"enumeration",
			"restriction",
			"sequence",
			"simpleType"
			};
	public final static int[] structType={
			XSTConstants.TYPE_SCHEMA,
			XSTConstants.TYPE_ANNOTATION,
			XSTConstants.TYPE_APPINFO,
			XSTConstants.TYPE_ATTRIBUTE,
			XSTConstants.TYPE_ATTRIBUTE_GROUP,
			XSTConstants.TYPE_COMPLEX,
			XSTConstants.TYPE_DOCUMENTATION,
			XSTConstants.TYPE_ELEMENT,
			XSTConstants.TYPE_ENUMERATION,
			XSTConstants.TYPE_RESTRICTION,
			XSTConstants.TYPE_SEQUENCE,
			XSTConstants.TYPE_SIMPLE
			};
	public final static String[] structClass={
			"XSTNodeSchema",
			"XSTNodeAnnotation",
			"XSTNodeAppinfo",
			"XSTNodeAttribute",
			"XSTNodeAttributeGroup",
			"XSTNodeComplex",
			"XSTNodeDocumentation",
			"XSTNodeElement",
			"XSTNodeEnumeration",
			"XSTNodeRestriction",
			"XSTNodeSequence",
			"XSTNodeSimple"
			};
			
	public final static String[] attrName={
			"name",
			"type",
			"minOccurs",
			"maxOccurs",
			"ref",
			"default",
			"xmlns",
			"value",
			"base",
			"use"
			};
			
	public final static int[] attrValue={
			XSTConstants.ATTR_NAME,
			XSTConstants.ATTR_TYPE,
			XSTConstants.ATTR_MINOCCURS,
			XSTConstants.ATTR_MAXOCCURS,
			XSTConstants.ATTR_REF,
			XSTConstants.ATTR_DEFAULT,
			XSTConstants.ATTR_XMLNS,
			XSTConstants.ATTR_VALUE,
			XSTConstants.ATTR_BASE,
			XSTConstants.ATTR_USE
			};
	
	
	private Hashtable htStruct;//used to reverse map
	private Hashtable htAttr;
	
	/** Only needed for debug, otherwise dont need to create this
	* object
	*/
	public XSGrammar()
	{
		htStruct=new Hashtable();
		htStruct.put(new Integer(XSTConstants.TYPE_SCHEMA),new String("schema"));
		htStruct.put(new Integer(XSTConstants.TYPE_ANNOTATION),new String("annotation"));
		htStruct.put(new Integer(XSTConstants.TYPE_APPINFO),new String("Appinfo"));
		htStruct.put(new Integer(XSTConstants.TYPE_ATTRIBUTE),new String("attribute"));
		htStruct.put(new Integer(XSTConstants.TYPE_ATTRIBUTE_GROUP),new String("attributeGroup"));
		htStruct.put(new Integer(XSTConstants.TYPE_COMPLEX),new String("complexType"));
		htStruct.put(new Integer(XSTConstants.TYPE_DOCUMENTATION),new String("documentation"));
		htStruct.put(new Integer(XSTConstants.TYPE_ELEMENT),new String("element"));
		htStruct.put(new Integer(XSTConstants.TYPE_ENUMERATION),new String("enumuration"));
		htStruct.put(new Integer(XSTConstants.TYPE_RESTRICTION),new String("restriction"));
		htStruct.put(new Integer(XSTConstants.TYPE_SEQUENCE),new String("sequence"));
		htStruct.put(new Integer(XSTConstants.TYPE_SIMPLE),new String("simpleType"));
		htAttr=new Hashtable();
		htAttr.put(new Integer(XSTConstants.ATTR_NAME),new String("name"));
		htAttr.put(new Integer(XSTConstants.ATTR_TYPE),new String("type"));
		htAttr.put(new Integer(XSTConstants.ATTR_MINOCCURS),new String("minOccurs"));
		htAttr.put(new Integer(XSTConstants.ATTR_MAXOCCURS),new String("maxOccurs"));
		htAttr.put(new Integer(XSTConstants.ATTR_REF),new String("ref"));
		htAttr.put(new Integer(XSTConstants.ATTR_DEFAULT),new String("default"));
		htAttr.put(new Integer(XSTConstants.ATTR_XMLNS),new String("xmlns"));
		htAttr.put(new Integer(XSTConstants.ATTR_VALUE),new String("value"));
		htAttr.put(new Integer(XSTConstants.ATTR_BASE),new String("base"));
		htAttr.put(new Integer(XSTConstants.ATTR_USE),new String("use"));
		
	}
	/** Reverse map Struct */
	public String rMapStruct(int type)
	{
		return (String)htStruct.get(new Integer(type));
	}
	/** Reverse map Attr */
	public String rMapAttr(int type)
	{
		return (String)htAttr.get(new Integer(type));
	}
}