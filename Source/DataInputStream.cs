using System;
using System.Threading;

public class DataInputStream
{
	public myReader r;

	private const int INTERVAL = 5;

	private const int MAXTIME = 500;

	public static DataInputStream istemp;

	private static int status;

	private static string filenametemp;

    public DataInputStream(string filename)
    {
        // 1. Xây dựng đường dẫn vật lý đến thư mục Resources
        string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", filename);
		// Kiểm tra sự tồn tại của file trước khi đọc
		if (File.Exists(path))
		{
			byte[] rawData = File.ReadAllBytes(path);
			// 2. Tạo mảng sbyte[] có cùng độ dài
			sbyte[] sData = new sbyte[rawData.Length];
			// 3. Sao chép bộ nhớ thô từ rawData sang sData
			Buffer.BlockCopy(rawData, 0, sData, 0, rawData.Length);
			// 4. Khởi tạo reader với mảng đã chuyển đổi
			this.r = new myReader(sData);

			if (r == null) { throw new Exception("null"); }
		}
    }

    public DataInputStream(sbyte[] data)
	{
		r = new myReader(data);
	}

	public static void update()
	{
		if (status == 2)
		{
			status = 1;
			istemp = __getResourceAsStream(filenametemp);
			status = 0;
		}
	}

	public static DataInputStream getResourceAsStream(string filename)
	{
		return __getResourceAsStream(filename);
	}

	private static DataInputStream _getResourceAsStream(string filename)
	{
		if (status != 0)
		{
			for (int i = 0; i < 500; i++)
			{
				Thread.Sleep(5);
				if (status == 0)
				{
					break;
				}
			}
			if (status != 0)
			{
				//Console.WriteLine("CANNOT GET INPUTSTREAM " + filename + " WHEN GETTING " + filenametemp);
				return null;
			}
		}
		istemp = null;
		filenametemp = filename;
		status = 2;
		int j;
		for (j = 0; j < 500; j++)
		{
			Thread.Sleep(5);
			if (status == 0)
			{
				break;
			}
		}
		if (j == 500)
		{
			//Console.WriteLine("TOO LONG FOR CREATE INPUTSTREAM " + filename);
			status = 0;
			return null;
		}
		return istemp;
	}

	private static DataInputStream __getResourceAsStream(string filename)
	{
		try
		{
			return new DataInputStream(filename);
		}
		catch (Exception)
		{
			return null;
		}
	}

	public short readShort()
	{
		return r.readShort();
	}

	public int readInt()
	{
		return r.readInt();
	}

	public int read()
	{
		return r.readUnsignedByte();
	}

	public void read(ref sbyte[] data)
	{
		r.read(ref data);
	}

	public void close()
	{
		r.Close();
	}

	public void Close()
	{
		r.Close();
	}

	public string readUTF()
	{
		return r.readUTF();
	}

	public sbyte readByte()
	{
		return r.readByte();
	}

	public long readLong()
	{
		return r.readLong();
	}

	public bool readBoolean()
	{
		return r.readBoolean();
	}

	public int readUnsignedByte()
	{
		return (byte)r.readByte();
	}

	public int readUnsignedShort()
	{
		return r.readUnsignedShort();
	}

	public void readFully(ref sbyte[] data)
	{
		r.read(ref data);
	}

	public int available()
	{
		return r.available();
	}

	internal void read(ref sbyte[] byteData, int p, int size)
	{
		throw new NotImplementedException();
	}
}
