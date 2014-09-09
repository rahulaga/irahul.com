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

import java.io.*;
import java.net.*;
import java.util.*;

class server{
	/*Server which accepts clients and assigns them to different threads */
	
	private int sserverPort;
	private DatagramSocket sserverSocket;
	private DatagramPacket sinPacket;
	private DatagramPacket soutPacket;
	private byte[] sinBuffer,soutBuffer;
	private int clientPort;
	private InetAddress clientInetAddr;
	
	public static void main(String[] args) throws Exception{
		if(args.length !=1){
			System.out.println("Wrong port number");
			System.exit(1);
		}
		server s=new server(args[0]);
		s.waitForClients();
	}
	
	public server(String port) throws Exception{
		sserverPort=Integer.parseInt(port);//create port
		sserverSocket=new DatagramSocket(sserverPort);
		System.out.println("Created socket on port:"+ sserverPort);
		System.out.println("Waiting for clients...");
	}
	
	public void waitForClients(){
		while(true){
			try{
				//create buffers/packets for recieving/sending
				sinBuffer =new byte[100];
				soutBuffer=new byte[100];
				sinPacket = new DatagramPacket(sinBuffer,sinBuffer.length);
				soutPacket = new DatagramPacket(soutBuffer,soutBuffer.length);
				//get init msg from client
				sserverSocket.receive(sinPacket);//get init from client
							
				clientPort=sinPacket.getPort();
				clientInetAddr=sinPacket.getAddress();
				
				sendAck();
												
				new threadedServer(clientInetAddr,clientPort).start();
			}
			catch (Exception e){
				System.out.println("Server error....terminated");
				System.exit(1);
			}
		}
	}
	
	private void sendMessage(String msg) throws Exception{
		//send message to client
		soutBuffer=new byte[100];
		soutPacket=new DatagramPacket(soutBuffer,soutBuffer.length);
		soutBuffer=msg.getBytes();
		
		soutPacket.setData(soutBuffer);
		soutPacket.setLength(soutBuffer.length);
		soutPacket.setPort(clientPort);
		soutPacket.setAddress(clientInetAddr);
		
		sserverSocket.send(soutPacket);
		
	}
	
	private void sendAck() throws Exception{
		sendMessage("ACK");
	}
	
}//class server
				
/* A new thread created for every client based on this server */
	

class threadedServer extends Thread{
  private int MAXSIZE=512;
	private int serverPort;
	private DatagramSocket serverSocket;
	private DatagramPacket inPacket,inPacket1;
	private DatagramPacket outPacket,outPacket1;
	private byte[] inBuffer,inBuffer1,outBuffer,outBuffer1,outBuffer2,store;
	private String currentPath=new String("/");//current path maintained by server
	private RandomAccessFile sendFile;
	private final int TIMEOUT=2000;
	private final int MAX_TRIES=5;
	private int clientPort;
	private InetAddress clientInetAddr;
	private final int SWS=8,RWS=8;
	private RandomAccessFile rfl;
	private LinkedList sendList;
	private final byte NACK=(byte)199;

	public void run(){
		
		waitForPackets();
	}
	
	public threadedServer( InetAddress clAddr, int oldPort) throws Exception{

		clientInetAddr=clAddr;
		serverSocket=new DatagramSocket();
		clientPort=oldPort;
		serverSocket.connect(clientInetAddr,clientPort);
		checkForAck("Port Changed");

		System.out.println("Client from:"+ clientInetAddr.toString() + " now served on port:" +serverPort);
	}
	
	public void waitForPackets(){
		while(true){
			try{
				//create buffers/packets for recieving/sending
				inBuffer =new byte[MAXSIZE];
				outBuffer=new byte[MAXSIZE];
				inPacket = new DatagramPacket(inBuffer,inBuffer.length);
				outPacket = new DatagramPacket(outBuffer,outBuffer.length);
				//get command
				
				serverSocket.receive(inPacket);
				clientPort=inPacket.getPort();
				clientInetAddr=inPacket.getAddress();
				sendAck();
			
				//respond to command
				String command=new String(inPacket.getData());
				System.out.println("Incoming command: "+command.trim() +" from: " +clientInetAddr + " from Port: "+clientPort);
				//respond to known commands
				// ls = list
				// get x.x m= send file x.x in mode m
				// pwd = send path
				// cd x= change dir to x
				//quit kill thread
				StringTokenizer st=new StringTokenizer(command.trim());
				String firstToken=st.nextToken();
				
				if(firstToken.equals("ls")){
					getListing();
				}
				
				if(firstToken.equals("get")){
					getFile(st.nextToken(),st.nextToken());
				}

				if(firstToken.equals("cd")){
					changePath(st.nextToken());
				}
				if(firstToken.equals("pwd")){
					displayPath();
				}
				if(firstToken.equals("quit")){
					//kill thread
					this.interrupt();
				}
				//end of commands
				//unknown commands cannot come as rejected by client.
			}
			catch(Exception e){
				System.out.print(e.toString()+"\n");
				e.printStackTrace();
			}
		}
	}//class waitForPackets
	
	/*
	################################
	#														   #
	#     Private Methods          #
	#                              #
	################################
	*/
	//
	//////////////////////////////////Generic Methods ///////////////////////////////////	
	//
	private void sendMessage(String msg) throws Exception{
		//send message to client
		outBuffer=new byte[MAXSIZE];
		outPacket=new DatagramPacket(outBuffer,outBuffer.length);
		outBuffer=msg.getBytes();
		
		outPacket.setData(outBuffer);
		outPacket.setLength(outBuffer.length);
		outPacket.setPort(clientPort);
		outPacket.setAddress(clientInetAddr);
		
		serverSocket.send(outPacket);
		
	}
	
	private void sendAck() throws Exception{
		sendMessage("ACK");

		//inBuffer = new byte[MAXSIZE];
		//inPacket = new DatagramPacket(inBuffer,inBuffer.length);

	}
	
	//#
	//method to check for ack
	//#
	private void checkForAck(String s) throws Exception{
		boolean flag=true;
		int tries=0;
		inBuffer = new byte[MAXSIZE];
		inPacket = new DatagramPacket(inBuffer,inBuffer.length);
		while(flag){
			sendMessage(s);
			serverSocket.setSoTimeout(TIMEOUT);
			tries++;
			try{
			serverSocket.receive(inPacket);
			serverPort=inPacket.getPort();
			}
			catch(Exception e){
				serverSocket.setSoTimeout(0);
				inBuffer = new byte[MAXSIZE];
				inPacket = new DatagramPacket(inBuffer,inBuffer.length);
				//falied to get ack
				if(tries==MAX_TRIES){
					flag=false;
					System.out.println("Error in Link. Cannot send to client");
					System.out.println("Killed thread for this client");
					this.interrupt();
				}	
			}
			//got some packet
			serverSocket.setSoTimeout(0);
			if (!(new String(inPacket.getData())).toString().trim().equals("ACK")){
				inBuffer = new byte[MAXSIZE];
				inPacket = new DatagramPacket(inBuffer,inBuffer.length);
				//incorrect ack
			}
			else{
				inBuffer = new byte[MAXSIZE];
				inPacket = new DatagramPacket(inBuffer,inBuffer.length);
				flag = false;	
				//correct ack
			}
		}
	}

	///////////////////////////////end generic methods//////////////////////////////////////
	//
	/////////////////////////////////Listing ///////////////////////////////////////////////
	private void getListing() throws Exception{
		//send directory list to client
		File fl=new File(currentPath);
		String[] files=fl.list();
		
		for(int i=0;i<files.length;i++){
			checkForAck(files[i]);
		}
		checkForAck("ListingComplete");
	}
	////////////////////////////////end Listing////////////////////////////////////////////
	//
	///////////////////File Transfers//////////////////////////////////////////////////////
	private void getFile(String fileName, String var) throws Exception{
		int fsize;
		int mode=Integer.parseInt(var);
		//send file to client based on mode
		File fl=new File(currentPath+fileName);
		if(fl.isFile()){

			checkForAck("Transfering File: "+fileName);
			
			if(mode==1) {
				sendFile(fileName,fl);
			}
			else {
				sendASCII(fileName,fl);	
			}
		}
		else{
			checkForAck("File Not Found");
		}	
	}
	
	private void sendASCII(String fileName,File fl) throws Exception{
		String record;
		int i = 0;
		FileReader filereader = new FileReader(fl);
		BufferedReader	dis = new BufferedReader(filereader);  
			
		try{
			while(true){
				record=dis.readLine();
				checkForAck(record);
			}
		}
		catch(Exception e){
			checkForAck("Transfer Complete");	
		}
		finally{
			filereader.close();
		}			
	}
	//
	//#Sliding window at end
	//
	//////////////////////////////////////end file transfers///////////////////////////////
	//
	///////////////////////// Path/////////////////////////////////////////////////////////
	private void displayPath() throws Exception{
		checkForAck(currentPath);
	}

	private void changePath(String newPath) throws Exception{
		String temp,oldPath=currentPath;
		if(!newPath.endsWith("/")){
			newPath+="/";
		}//always end path with / 
		try{
			if(newPath.equals("/") || (newPath.substring(0,1)).equals("/")){
				isValidPath(newPath);
			}
			else{
				if(newPath.equals("../")){
					if(currentPath.equals("/")){
						//do nothing
						isValidPath("/");
					}
					else{
						//lower one level
						for(int k=currentPath.length()-2;k>=0;k--){
							if(currentPath.charAt(k)=='/'){
								isValidPath(currentPath.substring(0,k+1));
								break;
							}
						}
					}
				}
				else{
					isValidPath(currentPath+newPath);
				}
			}
		}
		catch(Exception e){
			throw new Exception();
		}
	}
	private void isValidPath(String newPath) throws Exception{
		File temp=new File(newPath);
		try{
			if(temp.isDirectory()){
				currentPath=newPath;
				checkForAck("Path set to: " + currentPath);
			}
			else{
				//do nothing as wrong path
				checkForAck("Invalid directory");
			}
		}
		catch(Exception e){
			throw new Exception();
		}
	}
	//////////////////end path /////////////////////////////////////////
	//
	//	
	////Sliding window transfers
	//
	//
	public void sendFile(String fileName, File fl) throws Exception{
		int fsize,num_packs,last_blk_size,no_read,i,start=0;
		boolean flag=false;
		rfl = new RandomAccessFile(fl,"r");
		long length = rfl.length();
		fsize=(new Long(rfl.length())).intValue();
		num_packs = fsize/MAXSIZE;//number of complete packets
		byte out_buf[]=new byte[MAXSIZE + 5]; 
		outPacket=new DatagramPacket(out_buf,out_buf.length);
		byte in_buf[]=new byte[MAXSIZE + 5]; 
		inPacket=new DatagramPacket(in_buf,in_buf.length);
		last_blk_size = fsize%MAXSIZE ;//number of bytes in last packet
		int totalPackets;//number of valid packets to send
		int dummyPackets;//number of dummy packets to send
		byte[] temp =new byte[MAXSIZE + 5];
		byte[] temp1 = new byte[4];
		
		if(last_blk_size==0){
			totalPackets=num_packs;
		}
		else{
			totalPackets=num_packs+1;
		}
		
		dummyPackets=SWS-(totalPackets%SWS);//number of dummy packets to send
		if(totalPackets%SWS==0) dummyPackets=0;
		
		DatagramPacket[] dp=new DatagramPacket[totalPackets+dummyPackets];
		
		//#Step 1
		//create packets and put in list
		//#read full packets
		for(i=0;i<num_packs;i++){
			out_buf[0]=(byte)(i%(SWS*2));
			
			no_read=rfl.read(out_buf,5,out_buf.length - 5);
			temp1=checkSum(out_buf,MAXSIZE);
			for(int c1=0;c1<4;c1++)
			{
				out_buf[c1+1]=temp1[c1];
			}
			temp1=new byte[4];
			//outPacket=new DatagramPacket(temp,temp.length);
			temp=new byte[MAXSIZE+5];
			outPacket=new DatagramPacket(temp,temp.length);
			dp[i]=createPacket(out_buf);
			out_buf=new byte[MAXSIZE+5];
		}
		
		//#make last valid packet
		if(last_blk_size!=0){
			out_buf[0]=(byte)(i%(SWS*2));
								
			temp=new byte[MAXSIZE +5];
			outPacket=new DatagramPacket(temp,temp.length);
			no_read=rfl.read(out_buf,5,last_blk_size);
			temp1=checkSum(out_buf,MAXSIZE);
			for(int c2=0;c2<4;c2++)
			{
				out_buf[c2+1]=temp1[c2];
			}
			temp1=new byte[4];
			dp[i]=createPacket(out_buf);
			out_buf=new byte[MAXSIZE+5];
		}
		rfl.close();
		
		//#make dummy packets
		for(int j=totalPackets;j<totalPackets+dummyPackets;j++){
			temp=new byte[MAXSIZE+5];
			outPacket=new DatagramPacket(temp,temp.length);
			out_buf[0]=(byte)(j%(SWS*2));
			out_buf[1]=0;
			out_buf[2]=0;
			out_buf[3]=0;
			out_buf[4]=0;
			
			dp[j]=createPacket(out_buf);
			out_buf=new byte[MAXSIZE+1];
		}
		
		//step 2
		//send file info
		//
		checkForAck(new Integer(totalPackets).toString());
		checkForAck(new Integer(num_packs).toString());
		checkForAck(new Integer(dummyPackets).toString());
		//#step 3
		//##send window and get ack
		//#
		//
		long startTime=System.currentTimeMillis();//start timer for transfer
		//
		//
		while(start<totalPackets+dummyPackets){		
			flag=false;
			sendPackets(dp,start);
			serverSocket.setSoTimeout(TIMEOUT);
			try{
				serverSocket.receive(inPacket);
				flag=true;
			}
			catch(Exception e){
				//didnt get ack so send again
				System.out.println("din get ack");
			}
			if(flag){
				start+=SWS;
			}
			serverSocket.setSoTimeout(0);
		}
		//#Stats
		long stopTime=System.currentTimeMillis();
		System.out.println("\nTransfer Statistics for: "+fileName);
		System.out.println("Size:" + fsize+ " bytes, Number of Packets: " + totalPackets + " Size of packet: " + MAXSIZE + " bytes");
		long actualTime=stopTime-startTime;
		System.out.println("Transfer time: " + actualTime + " ms");
		System.out.println("Throughput: " + (double)fsize*8/((double)actualTime/1000.0) + " bits/sec");
		System.out.println("Average Packet Delay: " + ((double)actualTime)/num_packs + " ms");
		System.out.println();
		//#
	}//method end
	
	private DatagramPacket createPacket(byte[] out_buf){
		outPacket.setData(out_buf);
		outPacket.setLength(out_buf.length);
		outPacket.setPort(clientPort);
		outPacket.setAddress(clientInetAddr);
		return outPacket;
	}
	
	//the following method adds the checksum to the packet(4 bytes)
	private byte[] checkSum(byte[] out_buf,int length) throws Exception
	{
		int sum=0;
		byte y[] = new byte[4];
		bytesOutput bo;
		bo=new bytesOutput(4);
		for(int i=5;i<length;i++)
		{
			sum=sum+(new Byte(out_buf[i])).intValue();
		}
		
		bo.writeInt(sum);
		y=bo.toByteArray();
		return y;		
	
	}
	
	private void sendPackets(DatagramPacket[] dp,int start) throws Exception{
		int check_sum;
		for(int k=0;k<SWS;k++){
			//System.out.println("checksum: ");
			byte y[]=new byte[4];
		for(int j=0;j<4;j++)
		{
			y[j]=dp[start+k].getData()[j+1];
		}
		bytesInput bi;
		bi=new bytesInput(y);
		check_sum=bi.readInt();
		
			serverSocket.send(dp[start+k]);
		}
	}
}//class