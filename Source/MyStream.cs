using System;

public class MyStream
{
	public static DataInputStream readFile(string path)
	{
		path = Begin.res + path;
		try
		{
			if (DataInputStream.getResourceAsStream(path).r == null) return null;
			return DataInputStream.getResourceAsStream(path);
		}
		catch (Exception)
		{
			return null;
		}
	}
}
