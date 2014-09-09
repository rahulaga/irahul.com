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
import com.irahul.XST.XSTConstants;
/**
* Attribute Node
*
* @author Rahul
* @version Since v0.1
*/
public class XSTNodeAttribute extends XSTNode
{	
	public XSTNodeAttribute()
	{
		super(XSTConstants.TYPE_ATTRIBUTE);
	}
	
	/** Check if type is permitted as child of this node */
	public boolean isValidContent(int type)
	{
		if(type==XSTConstants.TYPE_ANNOTATION) return true;
		if(type==XSTConstants.TYPE_SIMPLE) return true;
		return false;
	}
	
	public boolean isValidAttr(int attrib)
	{
		if(attrib==XSTConstants.ATTR_NAME) return true;
		if(attrib==XSTConstants.ATTR_TYPE) return true;
		if(attrib==XSTConstants.ATTR_REF) return true;
		if(attrib==XSTConstants.ATTR_DEFAULT) return true;
		return false;
		//currently supported
	}
	
}