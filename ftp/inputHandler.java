import java.io.*;
import java.util.*;
import java.awt.*;
import java.awt.event.*;

class inputHandler implements ActionListener{
	
	private GUI gui;
	private int command;
	
	public inputHandler(GUI a,int x){
		gui=a;
		command=x;
	}//constructor
		
	public void actionPerformed(ActionEvent event){
		switch(command){
			case 0:
				gui.getConnectClose();
				break;
			case 1:
				gui.getGetClose();
				break;
			case 2:
				gui.getCdClose();
				break;
		}
	}

}//class