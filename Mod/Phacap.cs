using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;


namespace NRO;

public class Phacap
{
	public static string KeyCapcha;

	public static bool khoa = false;

	public static string textsuccess;

	public static async Task solvecap(string text)
	{
		if (khoa) return;
		khoa = true;
		try
		{
			if ((MobCapcha.isAttack && MobCapcha.explode) || GameScr.gI().mobCapcha == null)
			{
				return;
			}
			int num = (int)mSystem.currentTimeMillis();
			await Task.Delay(2000);
			int num2 = (int)mSystem.currentTimeMillis() - num;
			if (num2 < 5000)
			{
				await Task.Delay(5000 - num2);
			}
			if (text.Length >= 4 && text.Length <= 7)
			{
				Mob mobCapcha = GameScr.gI().mobCapcha;
				int retry = 0;
                while (GameScr.gI().mobCapcha != null && GameScr.gI().mobCapcha == mobCapcha&&retry < 3)
				{
					GameScr gameScr = GameScr.gI();
					gameScr.keyInput = "";
					for (int i = 0; i < text.Length; i++)
					{
                        gameScr.keyInput += text[i];
						Service.gI().mobCapcha(text[i]);
						await Task.Delay(Res.random(500, 1000));
					}
					retry++;
                    await Task.Delay(2000);
				}
				if(retry >= 3)
				{
					Program.Cleanup();
                }
            }
		}
		catch (Exception)
		{
		}
		finally
		{
            await Task.Delay(5000);
			khoa = false;
        }
    }
}
