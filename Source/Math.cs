using System;

public class Math
{
	public const double PI = System.Math.PI;

	public static int abs(int i)
	{
		return (i <= 0) ? (-i) : i;
	}

	public static int min(int x, int y)
	{
		return (x >= y) ? y : x;
	}

	public static int max(int x, int y)
	{
		return (x <= y) ? y : x;
	}

	public static int pow(int data, int x)
	{
		int num = 1;
		for (int i = 0; i < x; i++)
		{
			num *= data;
		}
		return num;
	}

	public static int Clamp(int color, int min, int max)
	{
        return color = (color < 0) ? 0 : (color > 255) ? 255 : color;
    }
}
