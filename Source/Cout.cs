

public class Cout
{
	public static int count;

	public static void println(string s)
	{
		if (mSystem.isTest)
		{
			//Console.WriteLine(((count % 2 != 0) ? "***--- " : ">>>--- ") + s);
			count++;
		}
	}

	public static void Log(string str)
	{
		if (mSystem.isTest)
		{
			//Console.WriteLine(str);
		}
	}

	public static void LogError(string str)
	{
		if (mSystem.isTest)
		{
			Console.WriteLine(str);
		}
	}

	public static void LogError2(string str)
	{
		if (!mSystem.isTest)
		{
		}
	}

	public static void LogError3(string str)
	{
		if (!mSystem.isTest)
		{
		}
	}

	public static void LogWarning(string str)
	{
		if (mSystem.isTest)
		{
			//Console.WriteLine(str);
		}
	}
}
