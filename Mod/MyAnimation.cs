using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NRO
{
    public class MyAnimation
    {
        public static int getR(int color)
        {
            return color >> 16 & 255;
        }

        public static int getG(int color)
        {
            return color >> 8 & 255;
        }

        public static int getB(int color)
        {
            return color & 255;
        }
        public static void fillGradientRect(mGraphics g, int x, int y, int w, int h, int colorCenter, int colorEdge, int shortLen)
        {
            int num = h - 1;
            for (int i = 0; i < num; i++)
            {
                float num2 = (float)i / (float)(num - 1);
                int num5 = (int)((float)MyAnimation.getR(colorCenter) * (1f - num2) + (float)MyAnimation.getR(colorEdge) * num2);
                int num3 = (int)((float)MyAnimation.getG(colorCenter) * (1f - num2) + (float)MyAnimation.getG(colorEdge) * num2);
                int num4 = (int)((float)MyAnimation.getB(colorCenter) * (1f - num2) + (float)MyAnimation.getB(colorEdge) * num2);
                int color = num5 << 16 | num3 << 8 | num4;
                g.setColor(color);
                if (i <= shortLen - 2)
                {
                    g.fillRect(x + i + 1, y + i * h / num, w + (i - 2), h / num + 2);
                }
                else if (num - i <= shortLen - 2)
                {
                    g.fillRect(x + 2 - (num - i), y + i * h / num, w - (num - i), h / num + 2);
                }
                else
                {
                    g.fillRect(x, y + i * h / num, w + 1, h / num + 2);
                }
            }
        }

        public static void drawSparkles(mGraphics g, int x, int y, int w, int h)
        {
            int sparkleX = Res.random(x + 3, x + w / 2);
            int sparkleY = Res.random(y + h / 2, y + h - 3);
            int sparkleX2 = Res.random(x + w / 2, x + w - 3);
            int sparkleY2 = Res.random(y + h / 2, y + h - 3);
            if (GameCanvas.gameTick % 2 == 0)
            {
                g.setColor(16777215);
                g.fillRect(sparkleX, sparkleY, 1, 1);
                g.fillRect(sparkleX2, sparkleY2, 1, 1);
            }
        }
        public static void FillTriangle(mGraphics g, int x0, int y0, int x1, int y1, int x2, int y2)
        {
            // Sắp xếp đỉnh theo thứ tự từ trên xuống dưới
            if (y1 < y0) { Swap(ref x0, ref x1); Swap(ref y0, ref y1); }
            if (y2 < y0) { Swap(ref x0, ref x2); Swap(ref y0, ref y2); }
            if (y2 < y1) { Swap(ref x1, ref x2); Swap(ref y1, ref y2); }

            int totalHeight = y2 - y0;
            for (int i = 0; i < totalHeight; i++)
            {
                bool secondHalf = i > y1 - y0 || y1 == y0;
                int segmentHeight = secondHalf ? y2 - y1 : y1 - y0;

                float alpha = (float)i / totalHeight;
                float beta = (float)(i - (secondHalf ? (y1 - y0) : 0)) / segmentHeight;

                int ax = x0 + (int)((x2 - x0) * alpha);
                int bx = secondHalf
                    ? x1 + (int)((x2 - x1) * beta)
                    : x0 + (int)((x1 - x0) * beta);

                if (ax > bx) { Swap(ref ax, ref bx); }

                g.fillRect(ax, y0 + i, bx - ax + 1, 1);
            }
        }

        private static void Swap(ref int a, ref int b)
        {
            int temp = a;
            a = b;
            b = temp;
        }
    }
}
