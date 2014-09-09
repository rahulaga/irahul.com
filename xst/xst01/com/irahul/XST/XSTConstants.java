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

/**
* XML Schema Tree Constants, Document describes current implementation
* restrictions. All cases for all objects are NOT supported. 
* @author Rahul
* @version Since v0.1
*
*/
public class XSTConstants
{
	//Node
	//these represent the structures curently understood
	
	/** Annotation - can occur in almost any tag */
	public static final int TYPE_ANNOTATION = 0;
	/** Only in Annotaion */
	public static final int TYPE_APPINFO = 1;
	/** Global -> Schema and Local -> Currently in attributeGroup and Complex Type */
	public static final int TYPE_ATTRIBUTE = 2;
	/** Global -> Currently in Schema and 
	* Local -> Currently in Complex Type */
	public static final int TYPE_ATTRIBUTE_GROUP = 3;
	/** Global -> Currently in Schema and Local -> Element */
	public static final int TYPE_COMPLEX = 4;//local and global
	/** In annotation only */
	public static final int TYPE_DOCUMENTATION = 5;
	/** Global -> Schema and Local -> Currently sequence */
	public static final int TYPE_ELEMENT = 6;//local and global
	/** Facet, Currently in simple type restriction */
	public static final int TYPE_ENUMERATION = 7;
	/** Simple Type only */
	public static final int TYPE_RESTRICTION = 8;
	/** MUST! */
	public static final int TYPE_SCHEMA = 9;
	/** Currently in Complex Type only */
	public static final int TYPE_SEQUENCE = 10;
	/** Global -> Currently Schema and Local -> Currently
	* attribute, element */
	public static final int TYPE_SIMPLE = 11;
	
	//Attr 
	//these are used to identify node attrs like minOccurs, name, type etc
	//
	public static final int ATTR_NAME = 1001;
	public static final int ATTR_TYPE = 1002;
	public static final int ATTR_MINOCCURS = 1003;
	public static final int ATTR_MAXOCCURS = 1004;
	public static final int ATTR_REF = 1005;
	public static final int ATTR_DEFAULT = 1006;
	
	public static final int ATTR_XMLNS = 1007;
	public static final int ATTR_VALUE = 1008;
	public static final int ATTR_BASE = 1009;
	
	
	// Data
	// these are the datatypes, a valid datatype would have
	// to be one of these
	
	//PRIMITIVES
	
	public static final int DATA_STRING = 2001;
	public static final int DATA_BOOLEAN = 2002;
	public static final int DATA_BASE64BINARY = 2003;
	public static final int DATA_HEXBINARY = 2004;
	public static final int DATA_FLOAT = 2005;
	public static final int DATA_DECIMAL = 2006;
	public static final int DATA_DOUBLE = 2007;
	public static final int DATA_ANYURI = 2008;
	public static final int DATA_QNAME = 2009;
	public static final int DATA_NOTATION = 2010;
	public static final int DATA_DURATION = 2011;
	public static final int DATA_DATETIME = 2012;
	public static final int DATA_TIME = 2013;
	public static final int DATA_DATE = 2014;
	public static final int DATA_GYEARMONTH = 2015;
	public static final int DATA_GYEAR = 2016;
	public static final int DATA_GMONTHDAY = 2017;
	public static final int DATA_GDAY = 2018;
	public static final int DATA_GMONTH = 2019;
	
	//DERIVED
	//note that values same as base primitive
	
}
