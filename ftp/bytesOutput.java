import java.io.*;

public class bytesOutput{
	private ByteArrayOutputStream baos;
	private DataOutputStream dos;
	private int n;

	public bytesOutput(int x)
	{
		n=x;
		baos=new ByteArrayOutputStream(n);
		dos=new DataOutputStream(baos);	
	}

	public int size()
	{
		return n;
	}

	
	public void writeInt(int i) throws IOException
	{	
		dos.writeInt(i);
	}

	public void writeByte(byte i) throws IOException
	{	
		dos.writeByte(i);
	}

	public byte[] toByteArray()
	{
		return baos.toByteArray();
	}
}
