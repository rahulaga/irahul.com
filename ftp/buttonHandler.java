import java.io.*;
import java.util.*;
import java.awt.*;
import java.awt.event.*;

class buttonHandler implements ActionListener{
	
	private ftpGUI gui;
	private int command;
	
	public buttonHandler(ftpGUI a,int x){
		gui=a;
		command=x;
	}//constructor
		
	public void actionPerformed(ActionEvent event){
		try{
		switch(command){
			case GUI.CONNECT:
				gui.connect();
				break;
			case GUI.DISCONNECT:
				gui.disconnect();
				break;
			case GUI.ASCII:
				gui.ascii();
				break;
			case GUI.BINARY:
				gui.binary();
				break;
			case GUI.LS:
				gui.ls();
				break;
			case GUI.CD:
				gui.cd();
				break;
			case GUI.GET:
				gui.get();
				break;
			case GUI.HELP:
				gui.help();
				break;
			case GUI.QUIT:
				gui.quit();
				break;
			case GUI.CREDITS:
				gui.credits();
				break;
		}
		}
		catch(Exception e){
		}
		
	}

}//class