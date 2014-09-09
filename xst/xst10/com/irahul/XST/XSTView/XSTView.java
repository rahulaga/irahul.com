package com.irahul.XST.XSTView;

import com.irahul.XST.*;
import com.irahul.XST.Node.*;

import javax.swing.*;
import java.awt.*;

import java.util.LinkedList;
import java.util.Stack;

import java.awt.image.*;
import Acme.*;
import Acme.JPM.Encoders.*;
import java.io.*;

/** Creating visual XST - Java Swing and created on J2SDK 1.4
* @author Rahul
* @version Since v1.0
*/

public class XSTView extends JPanel
{
	private int grid=XSTV.GRID;
	private int maxChar=XSTV.MAXCHAR;
	private int xpos,ypos;//use to set the component
	private int mid=grid/2;
	private int space=grid/10;
	private Stack posStack;
	private XST xst;
	private LinkedList gridList;
	private Color gcolor;
	private String filename;
	
	/** Creates a drawing of the XST */
	public XSTView(XST a,String name)
	{
		this.xst=a;
		this.filename=name;
		posStack=new Stack();
		gridList=new LinkedList();
		
		setBackground(XSTV.BG);//default colors
		gcolor=XSTV.FG;
        //setForeground(XSTV.FG);
	}
	
	/** Method that draws XST */ 
	public void paintComponent(Graphics g) {
        super.paintComponent(g);      //clears the background
		BufferedImage bi = new BufferedImage(XSTV.WIDTH,XSTV.HEIGHT,BufferedImage.TYPE_INT_RGB);
		g = bi.getGraphics();
		g.setColor(Color.white);
		g.fillRect(0,0,XSTV.WIDTH,XSTV.HEIGHT);
		g.setColor(Color.BLACK);
		setBackground(XSTV.BG);
		
		
		
		drawLOGIC(g);
		
		try 
		{
			System.out.println("Writing to GIF file "+filename);
			OutputStream os = new FileOutputStream(filename);
			GifEncoder ge = new GifEncoder(bi,os);
			ge.encode();
			os.close();
		}
		catch(Exception e){e.printStackTrace();}
		
    }
	
	/** Create the tree using Graphics g and XST xst */
	private void drawLOGIC(Graphics g) 
	{
		//controller
		LinkedList drawList=getDrawList();
		//drawList is depth first list of nodes. Based on it 
		gridList=new LinkedList();
		xpos=10;ypos=10;
		XSTNode node;
		int type=-1;
		for(int i=1;i<drawList.size();i++)
		{
			//ignore 0 which is TYPE_SCHEMA
			node=(XSTNode)drawList.get(i);
			type=node.getTypeValue();
			//System.out.println("["+i+"] xpos="+xpos+" ypos="+ypos+" typeValue="+type+" name="+node.getName());
			switch (type)
			{
				case XSTConstants.TYPE_ATTRIBUTE:
					drawATTRIB(g,node.getName(),node.getDataTypeName());
					setNextPos(node);
					break;
				case XSTConstants.TYPE_ATTRIBUTE_GROUP:
					drawGROUP(g,node.getName(),"ATTRGRP");
					setNextPos(node);
					break;
				case XSTConstants.TYPE_COMPLEX:
					drawCOMPLEX(g,node.getName());
					setNextPos(node);
					break;
				case XSTConstants.TYPE_ELEMENT:
					drawELEMENT(g,node.getName(),node.getMinOccurs(),node.getMaxOccurs(),node.getDataTypeName());
					setNextPos(node);
					break;
				case XSTConstants.TYPE_SEQUENCE:
					drawGROUP(g,node.getName(),"SEQ");
					setNextPos(node);
					break;
				case XSTConstants.TYPE_SIMPLE:
					drawSIMPLE(g,node.getName());
					setNextPos(node);
					break;
				case XSTConstants.TYPE_RESTRICTION:
					drawSPECIAL(g,"RESTR","["+node.getValue(XSTConstants.ATTR_BASE)+"]");	
					setNextPos(node);
					break;
				case XSTConstants.TYPE_ENUMERATION:
					drawSPECIAL(g,"ENUM","["+node.getValue(XSTConstants.ATTR_VALUE)+"]");	
					setNextPos(node);
					break;
				default:
					//drawSPECIAL(g,node.getName(),null);
					setNextPos(node);
					break;
			}//switch		
			
			//set color for next draw
			g.setColor(gcolor);
		}//for
	}
	
	private void setNextPos(XSTNode pos)
	{
		//already drawn at this pos
		gridList.add(new Point(xpos,ypos));
		//System.out.print("OLD POS "+xpos+","+ypos);
			
		int count=0;
		if(pos.getUp()!=null) count++;
		if(pos.getDown()!=null) count++;
		if(pos.getLeft()!=null) count++;
		if(pos.getRight()!=null) count++;
		
		if (count==1)
		{
			//next marker
			if (!posStack.isEmpty())
			{
				Point temp=(Point)posStack.pop();
				ypos=(int)temp.getX();
				xpos=(int)temp.getY();
			}
		}
		else
		{
			if (count==2)
			{
				//draw in order
				if(pos.getDown()!=null) ypos+=grid;
				if(pos.getRight()!=null) xpos+=grid;
			}
			else
			{
				if (count==3)
				{
					//set marker in posStack - right node pos
					posStack.push(new Point(ypos,xpos+grid));
					ypos+=grid;//set next pos
				}
				else
				{
					//cannot happen!
					System.out.println("Error in creating drawing XST");
					System.exit(0);
				}
			}
		}
		
		//chk if next point correct
		checkGrid();		
	}
	
	private LinkedList getDrawList()
	{
		//create list from this.xst
		LinkedList ll=new LinkedList();
		xst.reset();
		XSTNode node=xst.getCurrent();
		ll.addLast(node);
		Stack s=new Stack();
		if (node.getRight()!=null)
		{
			s.push(node.getRight());
		}
		if (node.getDown()!=null)
		{
			s.push(node.getDown());
		}
		while (!s.isEmpty())
		{
			node=(XSTNode)s.pop();
			ll.addLast(node);
			if (node.getRight()!=null)
			{
				s.push(node.getRight());
			}
			if (node.getDown()!=null)
			{
				s.push(node.getDown());
			}
		}
		
		return ll;
	}
	
	private void drawELEMENT(Graphics g,String name,int min, int max,String other)
	{
		g.drawRect(xpos+space,ypos+(2*space),grid-(2*space),grid-(4*space));
        g.drawString(name,xpos+space+2,ypos+(2*space)+2+maxChar);
		g.drawString("["+min+", "+max+"]",xpos+space+2,ypos+(2*space)+4+(2*maxChar));
		if(other!=null) g.drawString(other,xpos+space+2,ypos+(2*space)+6+(3*maxChar));		
	}
	
	private void drawCOMPLEX(Graphics g,String name)
	{
		g.drawLine(xpos+mid-3,ypos+2,xpos+mid-3,ypos+mid);
		g.drawLine(xpos+mid,ypos+2,xpos+mid,ypos+mid);
		g.drawLine(xpos+mid+3,ypos+2,xpos+mid+3,ypos+mid);
		
		g.drawLine(xpos+mid,ypos+mid,xpos+mid-10,ypos+mid-10);
		g.drawLine(xpos+mid,ypos+mid,xpos+mid+10,ypos+mid-10);
		
		g.drawOval(xpos+mid/2,ypos+mid,mid,mid);
		if(name!=null) g.drawString(name,xpos+space+2,ypos+(2*space)+mid+maxChar);
	}
	
	private void drawSIMPLE(Graphics g,String name)
	{
		g.drawLine(xpos+mid-3,ypos+2,xpos+mid-3,ypos+mid);
		g.drawLine(xpos+mid+3,ypos+2,xpos+mid+3,ypos+mid);
		
		g.drawLine(xpos+mid,ypos+mid,xpos+mid-10,ypos+mid-10);
		g.drawLine(xpos+mid,ypos+mid,xpos+mid+10,ypos+mid-10);
		
		g.drawOval(xpos+mid/2,ypos+mid,mid,mid);
		if(name!=null) g.drawString(name,xpos+space+2,ypos+(2*space)+mid+maxChar);
	}
	
	private void drawATTRIB(Graphics g,String name, String other)
	{
		g.drawLine(xpos+mid,ypos+2,xpos+mid,ypos+mid);
		
		g.drawLine(xpos+mid,ypos+mid,xpos+mid-10,ypos+mid-10);
		g.drawLine(xpos+mid,ypos+mid,xpos+mid+10,ypos+mid-10);
		
		g.drawOval(xpos+mid/2,ypos+mid,mid,mid);
		g.drawString(name,xpos+space+2,ypos+(2*space)+mid+maxChar);
		if(other!=null) g.drawString(name,xpos+space+2,ypos+(2*space)+mid+(2*maxChar)+4);
	
	}
	
	private void drawSPECIAL(Graphics g,String name,String other)
	{
		int len=grid-(2*space);
		for(int i=0;i<len;i+=(2*len/5))
		{
			g.drawLine(xpos+space+i,ypos+(2*space),xpos+space+i+len/5,ypos+(2*space));
			g.drawLine(xpos+space+i,ypos+grid-(2*space),xpos+space+i+len/5,ypos+grid-(2*space));
		}
		g.drawLine(xpos+space,ypos+(2*space),xpos+space,ypos+grid-(2*space));
		g.drawLine(xpos+grid-space,ypos+(2*space),xpos+grid-space,ypos+grid-(2*space));
        if(name!=null) g.drawString(name,xpos+space+2,ypos+(2*space)+2+maxChar);
		if(other!=null) g.drawString(other,xpos+space+2,ypos+(2*space)+4+(2*maxChar));		
	}
	
	private void drawGROUP(Graphics g,String name,String type)
	{
		g.drawLine(xpos+mid,ypos+2,xpos+mid,ypos+mid);
		
		g.drawLine(xpos+mid,ypos+mid,xpos+mid-10,ypos+mid-10);
		g.drawLine(xpos+mid,ypos+mid,xpos+mid+10,ypos+mid-10);
		
		g.drawLine(xpos+mid,ypos+mid,xpos+mid/2,ypos+mid+mid/2);
		g.drawLine(xpos+mid,ypos+mid,xpos+mid+mid/2,ypos+mid+mid/2);
		g.drawLine(xpos+mid/2,ypos+mid+mid/2,xpos+mid,ypos+grid);
		g.drawLine(xpos+mid+mid/2,ypos+mid+mid/2,xpos+mid,ypos+grid);
	
		g.drawString(type,xpos+2+(mid/2),ypos+mid+2+maxChar);
	}
	
	private void checkGrid()
	{
		int count=0;//lvl to go down
		
		while (gridList.contains(new Point(xpos,ypos)))
		{
			count++;
			//System.out.println("IN WHILE"+count);
			ypos+=grid;
		}
		//set new color
		switch (count)
		{
			case 0:
				//no change
				gcolor=XSTV.FG;
				break;
			case 1:
				gcolor=Color.BLUE;
				break;
			case 2:
				gcolor=Color.CYAN;
				break;
			case 3:
				gcolor=Color.GREEN;
				break;
			case 4:
				gcolor=Color.YELLOW;
				break;
			case 5:
				gcolor=Color.ORANGE;
				break;
			case 6:
				gcolor=Color.RED;
				break;
			case 7:
				gcolor=Color.MAGENTA;
				break;
			case 8:
				gcolor=Color.PINK;
				break;
			default:
				//System.out.println("TOO DEEP!");
				//System.exit(0);
				break;
		}
	}
}
	
	
	