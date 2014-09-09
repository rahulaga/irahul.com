import java.io.*;
import java.util.*;
import java.awt.*;
import java.awt.event.*;

class GUI extends Frame{  // GUI window.
  
  public final static int CONNECT=0;
  public final static int DISCONNECT=1;
  public final static int ASCII=2;
  public final static int BINARY=3;
  public final static int LS=4;
  public final static int CD=5;
  public final static int GET=6;
  public final static int HELP=7;
  public final static int QUIT=8;
  public final static int CREDITS=9;
  
  private buttonHandler connect;
  private buttonHandler disconnect;
  private buttonHandler ascii;
  private buttonHandler binary;
	private buttonHandler ls;
	private buttonHandler cd;
	private buttonHandler get;
	private buttonHandler help;
	private buttonHandler quit;
	private buttonHandler credits;
  
  private Dialog dialogConnect;
	private Dialog dialogGet;
	private Dialog dialogCd;

  private Label labelHost;
	private Label labelPort;
	private TextField textHost;
	private TextField textPort;
	private Button buttonOK;

  
  
  private final int frameWIDTH=500;
  private final int frameHEIGHT=600;
  private Label labelTitle=new Label("FTP Client ",Label.LEFT);
  private Label labelTitle1=new Label("Status",Label.CENTER);
  private Label labelMode=new Label("",Label.LEFT);
  private Label labelPath=new Label("",Label.LEFT);
  private Font font1=new Font("Serif",Font.PLAIN,12);
  private Font font2=new Font("Serif",Font.PLAIN,13);

  private TextArea textMain=new TextArea("FTP Server/Client interface.\n Implemented in Java using User Datagram Protocol.\n Programming by Rahul (rahul@irahul.com) & Shantanu\n\n");
  
  private Button[] buttonArr=new Button[10];
	private ftpGUI ftp;
	
  public GUI(ftpGUI a){
  	super("FTP");
  	ftp=a;
  	setSize(frameWIDTH,frameHEIGHT);
  	//create interface
  	createInterface();
  }//gui constructor
  
  private void createInterface(){
  	
  	buttonArr[2] =new Button("ASCII");
  	buttonArr[3] =new Button("Binary");
  	buttonArr[8]=new Button("Quit");
  	buttonArr[4]=new Button("ls");
  	buttonArr[5]=new Button("cd");
  	buttonArr[6]=new Button("get");
  	buttonArr[0]=new Button("Connect");
  	buttonArr[1]=new Button("Disconnect");
  	buttonArr[7]=new Button("Help");
  	buttonArr[9]=new Button("Credits");
  	
  	
  	//creater handler objects
  	connect = new buttonHandler(ftp, this.CONNECT);
  	disconnect = new buttonHandler(ftp, this.DISCONNECT);
  	ascii = new buttonHandler(ftp, this.ASCII);
  	binary = new buttonHandler(ftp, this.BINARY);
  	ls = new buttonHandler(ftp, this.LS);
  	cd = new buttonHandler(ftp, this.CD);
  	get = new buttonHandler(ftp, this.GET);
  	help = new buttonHandler(ftp, this.HELP);
  	quit = new buttonHandler(ftp, this.QUIT);
  	credits = new buttonHandler(ftp, this.CREDITS);
  	
  	Panel panelTop=new Panel(new BorderLayout());
  	  	labelTitle.setFont(font1);
  	  	panelTop.add(labelTitle,"North");
  	  	panelTop.add(labelTitle1,"South");
  	Panel row=new Panel(new BorderLayout());
  			row.add(labelPath,"North");
  			row.add(labelMode,"Center");
  	Panel panelMiddle=new Panel(new BorderLayout());
  			textMain.setEditable(false);
  			panelMiddle.add(textMain,"Center");
  			panelMiddle.add(row,"South");
  	Panel panelBottom=new Panel(new FlowLayout());
  			for(int i=0;i<buttonArr.length;i++){
  				panelBottom.add(buttonArr[i]);
  				buttonArr[i].setFont(font2);
  				if(i>0 && i<7) buttonArr[i].setEnabled(false);
  			}
  	Panel panelMain=new Panel(new BorderLayout());
  			panelMain.add(panelTop,"North");
  			panelMain.add(panelMiddle,"Center");
  			panelMain.add(panelBottom,"South");
  			  			
  	add(panelMain);
  	
  	//ADD LISTENERS
  	buttonArr[0].addActionListener(connect);
  	buttonArr[1].addActionListener(disconnect);
		buttonArr[2].addActionListener(ascii);
		buttonArr[3].addActionListener(binary);
		buttonArr[4].addActionListener(ls);
		buttonArr[5].addActionListener(cd);
		buttonArr[6].addActionListener(get);
		buttonArr[7].addActionListener(help);
		buttonArr[8].addActionListener(quit);
		buttonArr[9].addActionListener(credits);
	  	
  }
 //
 //
 //GUI interaction method....called from ftpGUI
 //
 //
 
 	public void display(String str){
 		textMain.append(str+"\n");
 	}
 	
 	public String getConnect(){
 		dialogConnect=new Dialog(this,"Enter Host & Port",true);
 		
 		labelHost=new Label("Host");
 		labelPort=new Label("Port");
 		TextField textHost1=new TextField("",6);
 		textPort=new TextField("",6);
 		buttonOK=new Button("OK");
 		
 		buttonOK.addActionListener(new inputHandler(this,0));//0 => connect; 1 => get
 		
 		Panel p1=new Panel(new BorderLayout());
 		p1.add(labelHost,"North");
 		p1.add(textHost1,"South");
 		Panel p2=new Panel(new BorderLayout());
 		p2.add(labelPort,"North");
 		p2.add(textPort,"South");
 		
 		dialogConnect.add(p1,"North");
 		dialogConnect.add(p2,"Center");
 		dialogConnect.add(buttonOK,"South");
 		dialogConnect.setSize(100,150);
 		dialogConnect.setResizable(false);
 		
 		dialogConnect.show();
 		
 		String host=textHost1.getText();
 		String port=textPort.getText();
 		
 		if(host.equals("") || port.equals("")){
 			return ("Please try again");
 		}
 		else{
 			return host+ " " + port;
 		}
 	}
 	
 	public String getCd(){
 		dialogCd=new Dialog(this,"Enter new Path",true);
 		
 		Label labelPath=new Label("Path");
 		TextField textPath=new TextField("",6);
 		Button buttonOK1=new Button("OK");
 		
 		buttonOK1.addActionListener(new inputHandler(this,2));//0 => connect; 1 => get
 		
 		Panel p1=new Panel(new FlowLayout());
 		p1.add(labelPath);
 		p1.add(textPath);
 		p1.add(buttonOK1);
 		dialogCd.add(p1);
 		dialogCd.setSize(180,70);
 		dialogCd.setResizable(false);
 		
 		dialogCd.show();
 		
 		String path=textPath.getText();
 		
 		return path;
 	}
 	
 	public String getGet(){
 		dialogGet=new Dialog(this,"Enter File",true);
 		
 		Label labelFile=new Label("File");
 		TextField textFile=new TextField("",6);
 		Button buttonOK2=new Button("OK");
 		
 		buttonOK2.addActionListener(new inputHandler(this,1));//0 => connect; 1 => get
 		
 		Panel p1=new Panel(new FlowLayout());
 		p1.add(labelFile);
 		p1.add(textFile);
 		p1.add(buttonOK2);
 		dialogGet.add(p1);
 		dialogGet.setSize(180,70);
 		dialogGet.setResizable(false);
 		
 		dialogGet.show();
 		
 		String path=textFile.getText();
 		
 		return path;
 	}
 	
 	public void getConnectClose(){
 		dialogConnect.hide();
 	
 	}
 	
 	public void getGetClose(){
 		dialogGet.hide();
 	
 	}
 	
 	public void getCdClose(){
 		dialogCd.hide();
 	}
 	
 	public void enableButtons(){
 		for(int i=0;i<buttonArr.length;i++){
 			buttonArr[i].setEnabled(true);
 		}
 		buttonArr[0].setEnabled(false);
 		buttonArr[8].setEnabled(false);
 	}
 	
 	public void disableButtons(){
 		for(int i=0;i<buttonArr.length;i++){
  		if(i>0 && i<7) buttonArr[i].setEnabled(false);
  		else buttonArr[i].setEnabled(true);
  	}
 	}
 		
 	public void setPath(String path){
 		labelPath.setText(path);
 	}
 	
 	public void setMode(String mode){
 		labelMode.setText(mode);
 	}
} 