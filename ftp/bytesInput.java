import java.io.*;

public class bytesInput {
	private ByteArrayInputStream bais;
	private DataInputStream dis;

	public bytesInput(byte[] data)
	{
		bais=new ByteArrayInputStream(data);
		dis=new DataInputStream(bais);
	}
	
	public int readInt() throws IOException
	{
		return dis.readInt();
	}

	public int readUnsignedShort() throws IOException
	{
		return dis.readUnsignedShort();
	}

	public byte readByte() throws IOException
	{
		return dis.readByte();
	}
}
