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

//
//Client modified to meet GUI
//

import java.io.*;
import java.net.*;
import java.util.*;

public class client {
  private int MAXSIZE=512;
	private DatagramSocket connection;
	private DatagramPacket inPacket;
	private DatagramPacket outPacket;
	private String serverAddress, tempServerAddress;
  private int serverPortID, tempServerPortID;
  private byte[] inBuffer,outBuffer;
  private InetAddress serverInetAddr;
  private int mode=1; //default :: mode=0 (ascii) mode=1 (binary)
  private final int TIMEOUT=2000;
  private final int MAX_TRIES=5;
  private final int RWS=8,SWS=8;
  private boolean disconnectFlag=false;
  
  BufferedReader stdin=new BufferedReader(new InputStreamReader(System.in));

	public String connect() throws Exception{
    return connectServer();//get new port from server
	}
	
  public client(String h,String p){
  	serverPortID=Integer.parseInt(p);
    serverAddress=new String(h);
	    //create buffers for in and out packets
      inBuffer=new byte[MAXSIZE];
      outBuffer=new byte[MAXSIZE];
			//create in and out packets
      inPacket=new DatagramPacket(inBuffer,inBuffer.length);
			outPacket=new DatagramPacket(outBuffer,outBuffer.length);

  }
  
  private String connectServer() throws Exception{
  	
  	inBuffer=new byte[MAXSIZE];
    inPacket=new DatagramPacket(inBuffer,inBuffer.length);

  	try{
    	serverInetAddr=InetAddress.getByName(serverAddress);
    	System.out.println("Connecting...");//make connection
      connection=new DatagramSocket();
			connection.connect(serverInetAddr,serverPortID);//connect to server
			checkForAck("init");//send init to server
			connection.disconnect();//disconnect from socket
				
			connection.receive(inPacket);//get new port
			serverPortID=inPacket.getPort();
						
			sendAck();

			connection.connect(serverInetAddr,serverPortID);//listen on new port
      System.out.println("Connected to:"+ serverInetAddr.toString() + " on Port:" +serverPortID);
      disconnectFlag=false;
      System.out.println("Mode set to Binary");
      return "Connected to:"+ serverInetAddr.toString() + " on Port:" +serverPortID;
    }
    catch(Exception e){
    	System.out.println("Unknown host");
			return "Unkown Host.... Connect again....";
    }
  }
  
  public String disconnect() throws Exception{
  	
  	connection.disconnect();
  	checkForAck("quit");
  	disconnectFlag=true;
  	
  	return "Closed Connection";
  }

	/*
	################################
	#														   #
	#     Private Methods          #
	#                              #
	################################
	*/
	//
	///////////////////////////////Path/////////////////////////////////////////////////
	private void displayPath() throws Exception{
		checkForAck("pwd");
		
		connection.receive(inPacket);
		sendAck();
		displayMessage(inPacket);
	}

	public String setPath(String path) throws Exception{
		checkForAck("cd "+path);
		
		connection.receive(inPacket);
		sendAck();
		
		displayMessage(inPacket);
		return getString(inPacket);
	}
	////////////////////////////end Path//////////////////////////////////////////////
	//
	//////////////////////////////Listing/////////////////////////////////////////////
	public Vector getListing() throws Exception{
		//display list from server
		Vector listing=new Vector();
		boolean flag=true;
		String rc;
		checkForAck("ls");
		//recieve and display
		while(flag){
			inBuffer=new byte[MAXSIZE];
			inPacket=new DatagramPacket(inBuffer,inBuffer.length);
			connection.receive(inPacket);
			sendAck();
			rc=getString(inPacket);
			if(rc.equals("ListingComplete")){
				flag=false;
			}
			else{
				listing.add(rc);
				//System.out.println(rc);
			}
		}
		System.out.println("Listing complete");
		return listing;
	}
	/////////////////////////////////////end listing//////////////////////////////////
	//
	///////////////////////////File Transfers/////////////////////////////////////////
	public String getFile(String fileName) throws Exception{
		//get file based on mode
		checkForAck("get "+fileName+ " " + mode);
		
		connection.receive(inPacket);
		sendAck();
		
		if(getString(inPacket).equals("File Not Found")){
			displayMessage(inPacket);
			return "File Not Found";
		}
		else{
			displayMessage(inPacket);
			if(mode==0){
				getFileAscii(fileName);
			}
			else{
				incomingFile(fileName);
			}
		return "Transfer Complete";
		}
	}
	
	private void getFileAscii(String fileName) throws Exception{
		//get file
		FileOutputStream fout =  new FileOutputStream(fileName);
		PrintStream myOutput = new PrintStream(fout);
		File f1 = new File(fileName);
		f1.createNewFile();
		boolean flag = true;
		String rc;
			while(flag){
			connection.receive(inPacket);
			sendAck();
			rc=getString(inPacket);
			inBuffer =new byte[MAXSIZE];
			inPacket = new DatagramPacket(inBuffer,inBuffer.length);
			if(rc.equals("Transfer Complete")){
				flag=false;
			}
			else{
				myOutput.println(rc);
			}
		}
		fout.close();
		System.out.println("Transfer Complete");
	}
	//#
	//#Sliding window at end
	//#
	//////////////////////////////////////////////////////////////////////////////////
	////////////////////////////Generic methods///////////////////////////////////////
	private String getString(DatagramPacket p){
		String s=new String(p.getData());
		return s.trim();
	}
	
	private void displayMessage(DatagramPacket p){
		System.out.println(getString(p));
	}
	
	private void sendMessage(String msg) throws Exception{
		//flushSocket();
		outBuffer=new byte[MAXSIZE];
		outPacket=new DatagramPacket(outBuffer,outBuffer.length);
		outBuffer=msg.getBytes();
		outPacket.setData(outBuffer);
		outPacket.setLength(outBuffer.length);
		outPacket.setPort(serverPortID);
		outPacket.setAddress(serverInetAddr);
		connection.send(outPacket);
	}
	//
	//send ack
	//
	private void sendAck() throws Exception{
		sendMessage("ACK");	
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
			connection.setSoTimeout(TIMEOUT);
			tries++;
			try{
			connection.receive(inPacket);
			}
			catch(Exception e){
				connection.setSoTimeout(0);
				inBuffer = new byte[MAXSIZE];
				inPacket = new DatagramPacket(inBuffer,inBuffer.length);
				//falied to get ack
				if(tries==MAX_TRIES){
					flag=false;
					System.out.println("Error in Link. Please reconnect and try again");
					System.out.println("Goodbye!");
					connection.close();
					System.exit(1);
				}	
			}
			//got some packet
			connection.setSoTimeout(0);
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
	////////////////////////////////end generic methods/////////////////////////////
	//
	///////////////////////Set Mode/////////////////////////////////////////////////
	public void setMode(int a) throws Exception{
		mode=a;
		if(a==0){
			System.out.println("Mode set to ASCII");
		}
		else{
			System.out.println("Mode set to Binary");
		}
	}
	//
	///////////////////////////////////end set mode/////////////////////////////////
	//
	/////////////////////////////////Help///////////////////////////////////////////
	private void displayHelp(){
		//display help
		System.out.println("ls  cd   pwd   get  bin  ascii  quit  bye   help  ?");
	}
	/////////////////////////////end help///////////////////////////////////////////
	//
	//
	///Sliding window transfer
	//get file
	//
	//
	//
	public void incomingFile(String fileName) throws Exception{		
		
		File f1 = new File(fileName);
		f1.createNewFile();
		RandomAccessFile out = new RandomAccessFile(f1,"rw");
		int tries=0;
		Vector incomingData = new Vector(RWS);
		int setNum=0,num_sets=1;
		byte in_buf[]=new byte[MAXSIZE + 5]; 
		inPacket=new DatagramPacket(in_buf,in_buf.length);
		byte out_buf[]=new byte[MAXSIZE + 5]; 
		inPacket=new DatagramPacket(out_buf,out_buf.length);
		int seqNo;
		
		connection.receive(inPacket);
		sendAck();
		int totalPackets=Integer.parseInt(getString(inPacket));
		in_buf=new byte[MAXSIZE + 5]; 
		inPacket=new DatagramPacket(in_buf,in_buf.length);
		connection.receive(inPacket);
		sendAck();
		int num_packs=Integer.parseInt(getString(inPacket));
		in_buf=new byte[MAXSIZE + 5]; 
		inPacket=new DatagramPacket(in_buf,in_buf.length);
		connection.receive(inPacket);
		sendAck();
		int dummyPackets=Integer.parseInt(getString(inPacket));
		in_buf=new byte[MAXSIZE + 5]; 
		inPacket=new DatagramPacket(in_buf,in_buf.length);
		num_sets=0;
		while(num_sets<(totalPackets+dummyPackets)/RWS){
			incomingData = new Vector(RWS);//refresh 'sorting' array for each set
			//total number of windows to ack
			if(setNum==0){
				try{
					tries++;
					connection.setSoTimeout(TIMEOUT);
					//only recieve seqNo 0 to RWS-1
					in_buf=new byte[MAXSIZE + 5]; 
					inPacket=new DatagramPacket(in_buf,in_buf.length);
					connection.receive(inPacket);
					connection.setSoTimeout(0);
					seqNo=getSeq(inPacket);
					
					if (seqNo> RWS-1){
						
						flushSocket();
						//reject and wait for server to timeout and resend
					}
					else{
						//recieve RWS number of packets and send ack
						
						if(checkCheckSum(inPacket.getData()))
						incomingData.add(seqNo,inPacket.getData());
						try{
							for(int i=0;i<RWS-1;i++){
								connection.setSoTimeout(TIMEOUT);
								in_buf=new byte[MAXSIZE + 5]; 
								inPacket=new DatagramPacket(in_buf,in_buf.length);
								//get remaining packets
								connection.receive(inPacket);
								connection.setSoTimeout(0);
								if(checkCheckSum(inPacket.getData()))
								incomingData.add(getSeq(inPacket),inPacket.getData());
							}
							//got all packets for this set
							sendAck();
							setNum=1;//ready for next set
							num_sets++;				
							tries=0;
							//write to file
							if(num_sets==(totalPackets+dummyPackets)/RWS){
								//last set so ignore dummy
								for(int i=0;i<RWS-dummyPackets;i++){
									out.write((byte[])incomingData.get(i),5,((byte[])(incomingData.get(i))).length - 5);
								}
							}
							else{
								for(int i=0;i<RWS;i++){
									out.write((byte[])incomingData.get(i),5,((byte[])(incomingData.get(i))).length - 5);
								}
							}
						}
						catch (Exception e){
							flushSocket();
							if(tries==MAX_TRIES){
								System.out.println("Error in Link. Please reconnect and try again");
								System.out.println("Goodbye!");
								connection.close();
								System.exit(1);
							}		
							//did not get packets so do not send ack and wait for resend
						}
					}
				}
				catch(Exception e){
					flushSocket();
						if(tries==MAX_TRIES){
							System.out.println("Error in Link. Please reconnect and try again");
							System.out.println("Goodbye!");
							connection.close();
							System.exit(1);
						}		
						//did not get packets so do not send ack and wait for resend
				}
			}
			else{
				try{
				//only recieve seqNo RWS to 2*RWS-1
				tries++;
				in_buf=new byte[MAXSIZE + 5];
				connection.setSoTimeout(TIMEOUT); 
				inPacket=new DatagramPacket(in_buf,in_buf.length);
				connection.receive(inPacket);
				connection.setSoTimeout(0);
				seqNo=getSeq(inPacket);
				if (seqNo < RWS){
					flushSocket();
					//reject and wait for server to timeout and resend
				}
				else{
					//recieve RWS number of packets and send ack
					if(checkCheckSum(inPacket.getData()))
					incomingData.add(seqNo-RWS,inPacket.getData());
					try{
						for(int i=0;i<RWS-1;i++){
							connection.setSoTimeout(TIMEOUT);
							in_buf=new byte[MAXSIZE + 5]; 
							inPacket=new DatagramPacket(in_buf,in_buf.length);
							//get remaining packets
							connection.receive(inPacket);
							connection.setSoTimeout(0);
							if(checkCheckSum(inPacket.getData()))
							incomingData.add(getSeq(inPacket)-RWS,inPacket.getData());
						}
						//got all packets for this set
						sendAck();
						setNum=0;//ready for next set
						num_sets++;
						tries=0;
						//write to file
						if(num_sets==(totalPackets+dummyPackets)/RWS){
							//last set so ignore dummy
							for(int i=0;i<RWS-dummyPackets;i++){
								out.write((byte[])incomingData.get(i),5,((byte[])(incomingData.get(i))).length - 5);
							}
						}
						else{
							for(int i=0;i<RWS;i++){
								out.write((byte[])incomingData.get(i),5,((byte[])(incomingData.get(i))).length - 5);
							}
						}
					}
					catch (Exception e){
						flushSocket();
						if(tries==MAX_TRIES){
							System.out.println("Error in Link. Please reconnect and try again");
							System.out.println("Goodbye!");
							connection.close();
							System.exit(1);
						}
						//did not get packets so do not send ack and wait for resend
					}
				}
				}
				catch(Exception e){
					flushSocket();
						if(tries==MAX_TRIES){
							System.out.println("Error in Link. Please reconnect and try again");
							System.out.println("Goodbye!");
							connection.close();
							System.exit(1);
						}		
						//did not get packets so do not send ack and wait for resend
				}
			}
		}//while loop end
		System.out.println("Transfer Complete");
		out.close();	
		
	}//method incoming File
		
	private int getSeq(DatagramPacket p){
		return p.getData()[0];
	}
	
	//the following method checks for bit errors if any.
	private boolean checkCheckSum(byte[] array) throws Exception
	{
		int sum=0,check_sum;
		for(int i=5;i<array.length-5;i++)
		{
			sum=sum+(new Byte(array[i])).intValue();
		}
		byte y[]=new byte[4];
		for(int j=0;j<4;j++)
		{
			y[j]=array[j+1];
		}
		bytesInput bi;
		bi=new bytesInput(y);
		check_sum=bi.readInt();
		//System.out.println("checksum= "+ check_sum+" " +"sum= " + sum);
		if(check_sum==sum)
			return true;
		else return false;	
	}
	
	//
	//flush
	private void flushSocket() throws Exception{
		try{
			connection.setSoTimeout(TIMEOUT);
			while(true){
				connection.receive(inPacket);
			}
		}
		catch(Exception e){
			//cleared all
		}
		connection.setSoTimeout(0);
	}
	//
	//end sliding window
	//
}//class client

	