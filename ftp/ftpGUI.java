/*
#
############################################
##                                        ##
##                   FTP                  ##
##           by Rahul Agarwal &           ##
##             Shantanu Kumar             ##
##                                        ##
##             version:  0.1.0            ##
##         last modified: 08/10/2000      ##
##           copyright (c) 2000           ##
##                                        ##
##    latest version is available from    ##
##        http://www.irahul.com/ftp       ##
##                                        ##
############################################
#
# NOTICE:
#
# This program is being distributed as freeware.  It may be used and
# modified by anyone, so long as this notice and the header
# above remain intact. By using this
# program you agree to indemnify Rahul Agarwal & Shantanu Kumar from any liability.
#
# Selling the code for this program is
# expressly forbidden.  Obtain permission before redistributing this
# program over the Internet or in any other medium.  In all cases
# notice and header must remain intact.

*/

//ftpGUI
import java.io.*;
import java.util.*;
import java.awt.*;
import java.awt.event.*;

public class ftpGUI{  // main class

private client user;//object of client which manages application

private static ftpGUI ftp=new ftpGUI();
private static GUI frame=new GUI(ftp);

public ftpGUI(){
}
	
public static void main(String args[]){
     
     frame.setVisible(true);
}
//#
//#Methods to respond to events
//#

public void connect() throws Exception{
  	//get host n port
  	String host,port;
  	String ans=frame.getConnect();
  	if(ans.equals("Please try again")){
  		frame.display("Invalid Host or Port");
  	}
  	else{
  		StringTokenizer st=new StringTokenizer(ans);
  		host=st.nextToken();
  		port=st.nextToken();
  		frame.display("Connecting to "+host+" on port "+port);
  		user=new client(host,port);
  		String msg=user.connect();
  		frame.display(msg);
  		frame.display("Mode set to Binary");
  		frame.enableButtons();
  		frame.setPath("Path set to: /");
  		frame.setMode("Current Mode: Binary");
  	}	
  }
  
  public void disconnect() throws Exception{
  	frame.setPath("");
  	frame.setMode("");
  	frame.display(user.disconnect());
  	frame.disableButtons();
  }
  
  public void ascii() throws Exception{
  	user.setMode(0);
  	frame.display("Mode set to ASCII");
  	frame.setMode("Current Mode: ASCII");
  }
  
  public void binary() throws Exception{
  	user.setMode(1);
  	frame.display("Mode set to Binary");
  	frame.setMode("Current Mode: Binary");
  }
  
  public void ls() throws Exception{
  	Vector list;
  	list=user.getListing();
  	for(int i=0;i<list.size();i++){
  		frame.display(list.get(i).toString());
  	}
  	frame.display("Listing Complete");
  }
  
  public void cd() throws Exception{
  	String ans=frame.getCd();
  	String temp =user.setPath(ans);
  	if(temp.equals("Invalid directory")){
  		frame.display("Invalid directory");
  	}
  	else{
  		frame.setPath(temp);
  		frame.display(temp);
  	}
  }
  
  public void get() throws Exception{
  	String ans=frame.getGet();
  	String temp=user.getFile(ans);
  	frame.display(temp);
  	
  }
  
  public void help(){
  	frame.display("\nConnect: Connect by specifying Host and Port\nDisconnect: Close connection if connected\nASCII: Set transfer mode to ASCII\nBinary: Set transfer mode to Binary\nls: Get directory listing of current directory\ncd: Change directory\nget: Transfer specified file\nQuit: Quit this program\nHelp: This screen\nCredits: Some information\n");
  }
  
  public void quit() throws Exception{
  	
  	frame.display("Goodbye!");
  	System.exit(0);
  	//
  }
  
  public void credits(){
  	frame.display("\nFTP Server/Client and GUI pogramming by:");
  	frame.display(" Rahul (rahul@irahul.com) & Shantanu");
  	frame.display(" Visit http://www.irahul.com/ftp for latest updates\n");
  }
  
  //
  //doalog for data entry
  //
  
  
}