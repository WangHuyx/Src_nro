using Assets.src.e;
using System;
using System.Collections;
public class mGraphics
{
	public const int HCENTER = 1;

	public const int VCENTER = 2;

	public const int LEFT = 4;

	public const int RIGHT = 8;

	public const int TOP = 16;

	public const int BOTTOM = 32;

	private int r;

	private int g;

	private int b;

	private float a;

	public int clipX;

	public int clipY;

	public int clipW;

	public int clipH;

	private bool isClip;

	private bool isTranslate = true;

	private int translateX;

	private int translateY;

	private float translateXf;

	private float translateYf;

	public static int zoomLevel = 1;

	public const int BASELINE = 64;

	public const int SOLID = 0;

	public const int DOTTED = 1;

	public const int TRANS_MIRROR = 2;

	public const int TRANS_MIRROR_ROT180 = 1;

	public const int TRANS_MIRROR_ROT270 = 4;

	public const int TRANS_MIRROR_ROT90 = 7;

	public const int TRANS_NONE = 0;

	public const int TRANS_ROT180 = 3;

	public const int TRANS_ROT270 = 6;

	public const int TRANS_ROT90 = 5;

	//public static Hashtable cachedTextures = new Hashtable();

	public static int addYWhenOpenKeyBoard;

	private int clipTX;

	private int clipTY;

	private int currentBGColor;

	//private SKPoint pos = new SKPoint(0f, 0f);

	//private Rect rect;

	//private Matrix4x4 matrixBackup;

	private SKPoint pivot;

	public SKPoint size = new SKPoint(128f, 128f);

	public SKPoint relativePosition = new SKPoint(0f, 0f);

	public SKColor clTrans;

    public static SKColor transParentColor = new SKColor(255, 255, 255, 0);


    // --- SKIASHARP CORE ---
    public SKCanvas _canvas { get; set; }
    private SKPaint _paint; // Dùng chung 1 cọ vẽ để tiết kiệm RAM

    // Trạng thái (Thay vì biến r,g,b cũ)
    private SKColor _currentColor = SKColors.Black;

    //private Material lineMaterial;

    //private void cache(string key, Texture value)
    //{
    //	if (cachedTextures.Count > 400)
    //	{
    //		cachedTextures.Clear();
    //	}
    //	if (value.width * value.height < GameCanvas.w * GameCanvas.h)
    //	{
    //		cachedTextures.Add(key, value);
    //	}
    //}
    public mGraphics()
    {
        _paint = new SKPaint
        {
            IsAntialias = false, // Game Pixel (NRO) nên tắt khử răng cưa để ảnh sắc nét
            FilterQuality = SKFilterQuality.None
        };
    }

    public void SetCanvas(SKCanvas canvas)
    {
        _canvas = canvas;
    }

    //fix
    public void translate(int tx, int ty)
	{
		tx *= zoomLevel;
		ty *= zoomLevel;
        this.translateX += tx;
        this.translateY += ty;
        this.isTranslate = true;
        bool flag = this.translateX == 0 && this.translateY == 0;
        if (flag)
        {
            this.isTranslate = false;
        }
    }

	public void translate(float x, float y)
	{
		translateXf += x;
		translateYf += y;
		isTranslate = true;
		if (translateXf == 0f && translateYf == 0f)
		{
			isTranslate = false;
		}
	}

	public int getTranslateX()
	{
		return translateX / zoomLevel;
	}

	public int getTranslateY()
	{
		return translateY / zoomLevel + addYWhenOpenKeyBoard;
	}

    //fix
	public void setClip(int x, int y, int w, int h)
	{
		x *= zoomLevel;
		y *= zoomLevel;
		w *= zoomLevel;
		h *= zoomLevel;
        this.clipTX = this.translateX;
        this.clipTY = this.translateY;
        this.clipX = x;
        this.clipY = y;
        this.clipW = w;
        this.clipH = h;
        this.isClip = true;
        //if (_canvas != null)
        //{
        //    _canvas.ClipRect(new SKRect(x, y, x + w, y + h), SKClipOperation.Intersect);
        //}
    }

	public int getClipX()
	{
		return GameScr.cmx;
	}

	public int getClipY()
	{
		return GameScr.cmy;
	}

	public int getClipWidth()
	{
		return GameScr.gW;
	}

	public int getClipHeight()
	{
		return GameScr.gH;
	}

	public void fillRect(int x, int y, int w, int h, int color, float alpha)
	{
        float alpha2 = 0.5f;
        setColor(color, alpha2);
        fillRect(x, y, w, h);
	}

	public void drawLine(int x1, int y1, int x2, int y2)
	{
        if (_canvas == null) return;

        // 1. Nhân tỉ lệ ZoomLevel cho tọa độ điểm đầu và điểm cuối
        int sx1 = x1 * zoomLevel;
        int sy1 = y1 * zoomLevel;
        int sx2 = x2 * zoomLevel;
        int sy2 = y2 * zoomLevel;

        // 2. Xử lý Dịch chuyển hệ tọa độ (isTranslate)
        if (this.isTranslate)
        {
            sx1 += this.translateX;
            sy1 += this.translateY;
            sx2 += this.translateX;
            sy2 += this.translateY;
        }

        // 3. Xử lý Vùng cắt (isClip)
        bool hasClip = this.isClip;
        if (hasClip)
        {
            _canvas.Save(); // Tương đương GUI.BeginGroup trong Unity

            // Tính toán vùng Clip đã nhân Zoom
            int cX = this.clipX * zoomLevel;
            int cY = this.clipY * zoomLevel;
            int cW = this.clipW * zoomLevel;
            int cH = this.clipH * zoomLevel;

            if (this.isTranslate)
            {
                cX += this.clipTX;
                cY += this.clipTY;
            }

            // Áp dụng mặt nạ cắt (Clip mask)
            SKRect clipRect = new SKRect(cX, cY, cX + cW, cY + cH);
            _canvas.ClipRect(clipRect);
        }

        // 4. Thiết lập nét vẽ
        _paint.Style = SKPaintStyle.Stroke;
        // Độ dày nét vẽ cũng phải tỉ lệ thuận với Zoom để không bị quá mảnh
        _paint.StrokeWidth = 1 /** zoomLevel*/;

        // 5. Thực hiện vẽ đường thẳng
        _canvas.DrawLine(sx1, sy1, sx2, sy2, _paint);

        // 6. Khôi phục trạng thái Canvas nếu có Clip
        if (hasClip)
        {
            _canvas.Restore(); // Tương đương GUI.EndGroup
        }
        //x1 *= zoomLevel;
        //y1 *= zoomLevel;
        //x2 *= zoomLevel;
        //y2 *= zoomLevel;
        //if (y1 == y2)
        //{
        //	if (x1 > x2)
        //	{
        //		int num = x2;
        //		x2 = x1;
        //		x1 = num;
        //	}
        //	fillRect(x1, y1, x2 - x1, 1);
        //	return;
        //}
        //if (x1 == x2)
        //{
        //	if (y1 > y2)
        //	{
        //		int num2 = y2;
        //		y2 = y1;
        //		y1 = num2;
        //	}
        //	fillRect(x1, y1, 1, y2 - y1);
        //	return;
        //}
        //if (isTranslate)
        //{
        //	x1 += translateX;
        //	y1 += translateY;
        //	x2 += translateX;
        //	y2 += translateY;
        //}
        //string key = "dl" + r + g + b;
        //Texture2D texture2D = (Texture2D)cachedTextures[key];
        //if (texture2D == null)
        //{
        //	texture2D = new Texture2D(1, 1);
        //	Color color = new Color(r, g, b);
        //	texture2D.SetPixel(0, 0, color);
        //	texture2D.Apply();
        //	cache(key, texture2D);
        //}
        //SKPoint vector = new SKPoint(x1, y1);
        //SKPoint SKPoint = new SKPoint(x2, y2);
        //SKPoint vector3 = SKPoint - vector;
        //float num3 = 57.29578f * Mathf.Atan(vector3.y / vector3.x);
        //if (vector3.x < 0f)
        //{
        //	num3 += 180f;
        //}
        //int num4 = (int)Mathf.Ceil(0f);
        //GUIUtility.RotateAroundPivot(num3, vector);
        //int num5 = 0;
        //int num6 = 0;
        //int num7 = 0;
        //int num8 = 0;
        //if (isClip)
        //{
        //	num5 = clipX;
        //	num6 = clipY;
        //	num7 = clipW;
        //	num8 = clipH;
        //	if (isTranslate)
        //	{
        //		num5 += clipTX;
        //		num6 += clipTY;
        //	}
        //}
        //if (isClip)
        //{
        //	GUI.BeginGroup(new Rect(num5, num6, num7, num8));
        //}
        //Graphics.DrawTexture(new Rect(vector.x - (float)num5, vector.y - (float)num4 - (float)num6, vector3.magnitude, 1f), texture2D);
        //if (isClip)
        //{
        //	GUI.EndGroup();
        //}
        //GUIUtility.RotateAroundPivot(0f - num3, vector);
    }

	public SKColor setColorMiniMap(int rgb)
	{
        // Lấy trực tiếp các kênh màu (0 - 255) và ép kiểu sang byte
        byte b = (byte)(rgb & 0xFF);
        byte g = (byte)((rgb >> 8) & 0xFF);
        byte r = (byte)((rgb >> 16) & 0xFF);

        // Khởi tạo SKColor với thứ tự (Red, Green, Blue)
        return new SKColor(r, g, b);
    }

	//public float[] getRGB(Color cl)
	//{
	//	float num = 256f * cl.r;
	//	float num2 = 256f * cl.g;
	//	float num3 = 256f * cl.b;
	//	return new float[3] { num, num2, num3 };
	//}

	public void drawRect(int x, int y, int w, int h)
	{
		int num = 1;
		fillRect(x, y, w, num);
		fillRect(x, y, num, h);
		fillRect(x + w, y, num, h + 1);
		fillRect(x, y + h, w + 1, num);
	}

    //fix
	public void fillRect(int x, int y, int w, int h)
	{
        if (_canvas == null) return;

        // 1. Áp dụng ZoomLevel ngay từ đầu cho tọa độ vẽ
        int scaledX = x * zoomLevel;
        int scaledY = y * zoomLevel;
        int scaledW = w * zoomLevel;
        int scaledH = h * zoomLevel;
        if (scaledW < 0 || scaledH < 0) return;

        // 2. Xử lý Dịch chuyển (isTranslate) - Giống hệt logic Unity
        if (this.isTranslate)
        {
            // Lưu ý: translateX/Y thường đã được nhân zoom ở hàm translate(tx, ty)
            scaledX += this.translateX;
            scaledY += this.translateY;
        }

        // 3. Xử lý Vùng cắt (isClip)
        bool hasClip = this.isClip;
        if (hasClip)
        {
            _canvas.Save(); // Tương đương GUI.BeginGroup

            // Tính toán vùng Clip (num, num2, num3, num4 trong code cũ)
            int cX = this.clipX /** zoomLevel*/;
            int cY = this.clipY /** zoomLevel*/;
            int cW = this.clipW /** zoomLevel*/;
            int cH = this.clipH /** zoomLevel*/;

            if (this.isTranslate)
            {
                cX += this.clipTX; // clipTX thường là giá trị đã nhân zoom
                cY += this.clipTY;
            }

            // Áp dụng vùng cắt lên Skia Canvas
            SKRect clipRect = new SKRect(cX, cY, cX + cW, cY + cH);
            _canvas.ClipRect(clipRect);
        }

        // 4. Thực hiện vẽ Rectangle
        _paint.Style = SKPaintStyle.Fill;
        // _paint.Color đã được set từ hàm g.setColor(color) trước đó
        _canvas.DrawRect(new SKRect(scaledX, scaledY, scaledX + scaledW, scaledY + scaledH), _paint);

        // 5. Khôi phục trạng thái Canvas
        if (hasClip)
        {
            _canvas.Restore();
        }
    }

	public void setColor(int rgb)
	{
        // 1. Trích xuất các kênh màu (0-255)
        // Trong mã nguồn cũ: num là Blue, num2 là Green, num3 là Red
        this.b = rgb & 0xFF;
        this.g = (rgb >> 8) & 0xFF;
        this.r = (rgb >> 16) & 0xFF;
        this.a = 255; // Mặc định không trong suốt

        // 2. Cập nhật trạng thái màu hiện tại cho SkiaSharp
        this._currentColor = new SKColor((byte)r, (byte)g, (byte)b, (byte)a);

        // 3. Gán màu vào cọ vẽ (Paint) để các hàm fillRect, drawLine... sử dụng ngay
        if (this._paint != null)
        {
            this._paint.Color = this._currentColor;
        }
        //int num = rgb & 0xFF;
        //int num2 = (rgb >> 8) & 0xFF;
        //int num3 = (rgb >> 16) & 0xFF;
        //b = (float)num / 256f;
        //g = (float)num2 / 256f;
        //r = (float)num3 / 256f;
        //a = 255f;
    }

	//public void setColor(Color color)
	//{
	//	b = color.b;
	//	g = color.g;
	//	r = color.r;
	//}

	//public void setBgColor(int rgb)
	//{
	//	if (rgb != currentBGColor)
	//	{
	//		currentBGColor = rgb;
	//		int num = rgb & 0xFF;
	//		int num2 = (rgb >> 8) & 0xFF;
	//		int num3 = (rgb >> 16) & 0xFF;
	//		b = (float)num / 256f;
	//		g = (float)num2 / 256f;
	//		r = (float)num3 / 256f;
	//		Main.main.GetComponent<Camera>().backgroundColor = new Color(r, g, b);
	//	}
	//}

	public void drawString(string s, int x, int y, SKPaint customPaint)
	{
        if (_canvas == null || string.IsNullOrEmpty(s)) return;

        // 1. Nhân zoomLevel ngay từ đầu (Giống hệt Unity)
        float drawX = x * zoomLevel;
        float drawY = y * zoomLevel;

        // 2. Xử lý isTranslate
        if (this.isTranslate)
        {
            drawX += this.translateX;
            drawY += this.translateY;
        }

        // Khởi tạo các biến vùng cắt (Tương đương num, num2, num3, num4)
        float cX = 0f, cY = 0f, cW = 0f, cH = 0f;
        bool hasClip = this.isClip;

        // 3. Xử lý logic isClip
        if (hasClip)
        {
            cX = this.clipX;
            cY = this.clipY;
            cW = this.clipW;
            cH = this.clipH;

            if (this.isTranslate)
            {
                cX += this.clipTX;
                cY += this.clipTY;
            }

            // Tương đương GUI.BeginGroup
            _canvas.Save();
            _canvas.ClipRect(new SKRect(cX, cY, cX + cW, cY + cH));
        }

        // 4. Xử lý Baseline của Skia
        // Không dùng customPaint.TextSize vì nó không chính xác tuyệt đối.
        // Dùng FontMetrics.Ascent để lấy khoảng cách từ Top xuống Baseline.
        float baselineOffsetY = -customPaint.FontMetrics.Ascent;

        // 5. Thực hiện vẽ (Tương đương GUI.Label)
        // LƯU Ý BACK-END: GUI.BeginGroup trong Unity làm thay đổi gốc tọa độ (0,0) nên phải trừ đi (num, num2).
        // Tuy nhiên, _canvas.ClipRect của Skia CHỈ CẮT ảnh chứ KHÔNG dời gốc tọa độ.
        // Do đó, ta giữ nguyên drawX, drawY thay vì (drawX - cX, drawY - cY).
        // Tôi cũng KHÔNG set lại customPaint.TextAlign ở đây vì hàm vỏ bên ngoài đã xử lý.
        _canvas.DrawText(s, drawX, drawY + baselineOffsetY, customPaint);

        // 6. Tương đương GUI.EndGroup
        if (hasClip)
        {
            _canvas.Restore();
        }
    }

    public void setColor(int rgb, float alpha)
	{
        // 1. Trích xuất các kênh màu (0-255)
        // Trong mã nguồn cũ: num là Blue, num2 là Green, num3 là Red
        this.b = rgb & 0xFF;
        this.g = (rgb >> 8) & 0xFF;
        this.r = (rgb >> 16) & 0xFF;
        this.a = alpha; // Mặc định không trong suốt

        // 2. Cập nhật trạng thái màu hiện tại cho SkiaSharp
        int alphaByte = (int)(alpha * 255f);
        this._currentColor = new SKColor((byte)r, (byte)g, (byte)b, (byte)((alphaByte < 256) ? alphaByte : 256));

        // 3. Gán màu vào cọ vẽ (Paint) để các hàm fillRect, drawLine... sử dụng ngay
        //if (this._paint != null)
        //{
            this._paint.Color = this._currentColor;
        //}
    }

    private List<string> WrapText(string text, SKPaint paint, float maxWidth)
    {
        List<string> lines = new List<string>();
        string[] words = text.Split(' '); // Tách văn bản thành các từ
        string currentLine = "";

        foreach (var word in words)
        {
            string testLine = string.IsNullOrEmpty(currentLine) ? word : currentLine + " " + word;

            // Đo độ rộng của dòng dự kiến
            if (paint.MeasureText(testLine) <= maxWidth)
            {
                currentLine = testLine;
            }
            else
            {
                if (!string.IsNullOrEmpty(currentLine)) lines.Add(currentLine);
                currentLine = word;
            }
        }

        if (!string.IsNullOrEmpty(currentLine)) lines.Add(currentLine);
        return lines;
    }

    private void UpdatePos(int anchor)
	{
		SKPoint vector = new SKPoint(0f, 0f);
		switch (anchor)
		{
		case 3:
			vector = new SKPoint(size.Y / 2f, size.X / 2f);
			break;
		case 20:
			vector = new SKPoint(0f, 0f);
			break;
		case 17:
			vector = new SKPoint(GameCanvas.w / 2, 0f);
			break;
		case 24:
			vector = new SKPoint(GameCanvas.w, 0f);
			break;
		case 6:
			vector = new SKPoint(0f, GameCanvas.h / 2);
			break;
		case 10:
			vector = new SKPoint(GameCanvas.w, GameCanvas.h / 2);
			break;
		case 36:
			vector = new SKPoint(0f, GameCanvas.h);
			break;
		case 33:
			vector = new SKPoint(GameCanvas.w / 2, GameCanvas.h);
			break;
		case 40:
			vector = new SKPoint(GameCanvas.w, GameCanvas.h);
			break;
		}
		//pos = vector + relativePosition;
		//rect = new Rect(pos.x - size.x * 0.5f, pos.y - size.y * 0.5f, size.x, size.y);
		//pivot = new SKPoint(rect.xMin + rect.width * 0.5f, rect.yMin + rect.height * 0.5f);
	}

	public void drawRegion(Image arg0, int x0, int y0, int w0, int h0, int arg5, int x, int y, int arg8)
	{
		if (arg0 != null)
		{
            x *= zoomLevel;
            y *= zoomLevel;
            x0 *= zoomLevel;
            y0 *= zoomLevel;
            w0 *= zoomLevel;
            h0 *= zoomLevel;
            if (!Program.PaintCanvas) return;
            _drawRegion(arg0, x0, y0, w0, h0, arg5, x, y, arg8);
		}
	}

	public void drawRegion(Image arg0, int x0, int y0, int w0, int h0, int arg5, float x, float y, int arg8)
	{
		if (arg0 != null)
		{
            x *= (float)zoomLevel;
            y *= (float)zoomLevel;
            x0 *= zoomLevel;
            y0 *= zoomLevel;
            w0 *= zoomLevel;
            h0 *= zoomLevel;
            __drawRegion(arg0, x0, y0, w0, h0, arg5, x, y, arg8);
		}
	}

	public void drawRegion(Image arg0, int x0, int y0, int w0, int h0, int arg5, int x, int y, int arg8, bool isClip)
	{
		drawRegion(arg0, x0, y0, w0, h0, arg5, x, y, arg8);
	}

    public void __drawRegion(Image image, int x0, int y0, int w, int h, int transform, float x, float y, int anchor)
    {
        if (image == null || _canvas == null || image.texture == null) return;

        // 1. Xử lý Dịch chuyển hệ tọa độ (World/Map Translate)
        if (this.isTranslate)
        {
            x += (float)this.translateX;
            y += (float)this.translateY;
        }

        // 2. Tính toán Anchor (Mỏ neo) trên giá trị thô
        float num5 = 0f;
        float num6 = 0f;
        if ((anchor & mGraphics.HCENTER) == mGraphics.HCENTER) num5 -= (float)w / 2f;
        if ((anchor & mGraphics.VCENTER) == mGraphics.VCENTER) num6 -= (float)h / 2f;
        if ((anchor & mGraphics.RIGHT) == mGraphics.RIGHT) num5 -= (float)w;
        if ((anchor & mGraphics.BOTTOM) == mGraphics.BOTTOM) num6 -= (float)h;

        x += num5;
        y += num6;

        // Các biến điều khiển kích thước vẽ và UV (Sau khi bị Clip cắt xén)
        float drawW = (float)w;
        float drawH = (float)h;
        float offsetSrcX = 0f; // num3 trong mã cũ
        float offsetSrcY = 0f; // num4 trong mã cũ
        float mirrorOffset = 0f; // num8 trong mã cũ

        // 3. Logic Clipping thủ công (isClip, clipTX, clipTY...)
        if (this.isClip)
        {
            int num10 = this.clipX;
            int num12 = this.clipY;
            int num11 = this.clipW;
            int num13 = this.clipH;

            if (this.isTranslate)
            {
                num10 += this.clipTX;
                num12 += this.clipTY;
            }

            // Tạo Rect để tính giao điểm tương đương intersectRect
            SKRect rDraw = new SKRect(x, y, x + (float)w, y + (float)h);
            SKRect rClip = new SKRect((float)num10, (float)num12, (float)(num10 + num11), (float)(num12 + num13));

            // Tính vùng giao nhau (Intersection)
            SKRect rectIntersect = rDraw;
            rectIntersect.Intersect(rClip); // Nếu không giao nhau thì không vẽ

            drawW = rectIntersect.Width;
            drawH = rectIntersect.Height;
            offsetSrcX = rectIntersect.Left - rDraw.Left;
            offsetSrcY = rectIntersect.Top - rDraw.Top;

            // Xử lý Mirror Offset (num8) cho các trường hợp lật ảnh
            if (transform == 2 || transform == 4)
            {
                if ((float)num10 > x)
                    mirrorOffset = 0f - offsetSrcX;
                else if ((float)(num10 + num11) < x + (float)w)
                    mirrorOffset = 0f - ((float)(num10 + num11) - x - (float)w);
            }
        }

        // 4. Thực hiện vẽ bằng SkiaSharp
        _canvas.Save();

        // Áp dụng Zoom cho tọa độ hiển thị cuối cùng
        float finalX = (x + offsetSrcX) * zoomLevel;
        float finalY = (y + offsetSrcY) * zoomLevel;
        float finalW = drawW * zoomLevel;
        float finalH = drawH * zoomLevel;

        // Tính toán Transform (Xoay/Lật)
        float scaleX = 1f;
        float scaleY = 1f;
        float rotation = 0f;

        switch (transform)
        {
            case 1: scaleY = -1f; break; // Lật dọc
            case 2: scaleX = -1f; break; // Lật ngang
            case 3: scaleX = -1f; scaleY = -1f; break; // Xoay 180
            case 4: rotation = 270f; scaleX = -1f; break;
            case 5: rotation = 90f; break;
            case 6: rotation = 270f; break;
            case 7: rotation = 90f; scaleX = -1f; break;
        }

        // Di chuyển tâm vẽ để Transform không bị lệch
        _canvas.Translate(finalX + finalW / 2f, finalY + finalH / 2f);
        if (rotation != 0) _canvas.RotateDegrees(rotation);
        _canvas.Scale(scaleX, scaleY);
        _canvas.Translate(-finalW / 2f, -finalH / 2f);

        // 5. Source Rect và Destination Rect
        // Source: Cắt chính xác pixel trên texture, bao gồm cả phần bù do Clip
        SKRect srcRect = new SKRect(
            (float)x0 + offsetSrcX + mirrorOffset,
            (float)y0 + offsetSrcY,
            (float)x0 + offsetSrcX + mirrorOffset + drawW,
            (float)y0 + offsetSrcY + drawH
        );

        // Destination: Luôn vẽ từ (0,0) vì đã dùng Translate ở bước trên
        SKRect destRect = new SKRect(0, 0, finalW, finalH);

        _canvas.DrawBitmap(image.texture, srcRect, destRect, _paint);

        _canvas.Restore();
    }

    // Hàm bổ trợ để tái sử dụng logic xoay/lật
    private void ApplyTransform(int transform)
    {
        switch (transform)
        {
            case 1: _canvas.Scale(1, -1); break;         // TRANS_MIRROR_ROT180 (Lật dọc)
            case 2: _canvas.Scale(-1, 1); break;         // TRANS_MIRROR (Lật ngang)
            case 3: _canvas.RotateDegrees(180); break;   // TRANS_ROT180
            case 4:                                      // TRANS_MIRROR_ROT270
                _canvas.RotateDegrees(270);
                _canvas.Scale(-1, 1);
                break;
            case 5: _canvas.RotateDegrees(90); break;    // TRANS_ROT90
            case 6: _canvas.RotateDegrees(270); break;   // TRANS_ROT270
            case 7:                                      // TRANS_MIRROR_ROT90
                _canvas.RotateDegrees(90);
                _canvas.Scale(-1, 1);
                break;
        }
    }


    public void _drawRegion(Image image, float x0, float y0, int w, int h, int transform, int x, int y, int anchor)
    {
        if (image == null || _canvas == null || image.texture == null) return;

        // --- 1. Xử lý Dịch chuyển (Translate) ---
        float drawX = x;
        float drawY = y;
        if (this.isTranslate)
        {
            drawX += this.translateX;
            drawY += this.translateY;
        }

        // --- 2. Tính toán Anchor (Mỏ neo) ---
        float num5 = 0f;
        float num6 = 0f;
        if ((anchor & mGraphics.HCENTER) == mGraphics.HCENTER) num5 -= w / 2f;
        if ((anchor & mGraphics.VCENTER) == mGraphics.VCENTER) num6 -= h / 2f;
        if ((anchor & mGraphics.RIGHT) == mGraphics.RIGHT) num5 -= w;
        if ((anchor & mGraphics.BOTTOM) == mGraphics.BOTTOM) num6 -= h;

        drawX += (int)num5;
        drawY += (int)num6;

        // Các biến phụ trợ để cắt ảnh (Manual Clipping)
        float drawW = w;
        float drawH = h;
        float offsetSrcX = 0f; // num3 trong code cũ
        float offsetSrcY = 0f; // num4 trong code cũ
        float mirrorOffset = 0f; // num8 trong code cũ

        // --- 3. Logic Clipping thủ công (isClip, clipTX, clipTY...) ---
        if (this.isClip)
        {
            int cX = this.clipX;
            int cY = this.clipY;
            int cW = this.clipW;
            int cH = this.clipH;

            if (this.isTranslate)
            {
                cX += this.clipTX;
                cY += this.clipTY;
            }

            // Tạo Rect để tính giao điểm (intersectRect)
            SKRect rDraw = new SKRect(drawX, drawY, drawX + w, drawY + h);
            SKRect rClip = new SKRect(cX, cY, cX + cW, cY + cH);

            SKRect rectIntersect = rDraw;
            rectIntersect.Intersect(rClip);// Nếu không giao nhau thì không vẽ(tạm bỏ)

            // Tính toán lại kích thước và tọa độ sau khi cắt
            drawW = rectIntersect.Width;
            drawH = rectIntersect.Height;
            offsetSrcX = rectIntersect.Left - rDraw.Left;
            offsetSrcY = rectIntersect.Top - rDraw.Top;
            // Xử lý biến num8 cho lật ngang (Mirror)
            if (transform == 2 || transform == 4)
            {
                if (cX > drawX) mirrorOffset = 0f - offsetSrcX;
                else if (cX + cW < drawX + w) mirrorOffset = (float)(-(float)(cX + cW - drawX - w));
            }
        }

        // --- 4. Thực hiện vẽ bằng SkiaSharp ---
        _canvas.Save();

        // Áp dụng Zoom cho tọa độ hiển thị
        float finalX = (drawX + offsetSrcX);
        float finalY = (drawY + offsetSrcY);
        float finalW = drawW;
        float finalH = drawH;

        // Tính toán Transform (Xoay/Lật) bằng Ma trận
        // Lưu ý: Skia vẽ Scale âm sẽ lật ảnh, tương đương logic num7, num9
        float scaleX = 1f;
        float scaleY = 1f;
        float rotation = 0f;

        switch (transform)
        {
            case 1: scaleY = -1f; break; // Mirror_Rot180
            case 2: scaleX = -1f; break; // Mirror
            case 3: scaleX = -1f; scaleY = -1f; break; // Rot 180
            case 4: rotation = 270f; scaleX = -1f; break;
            case 5: rotation = 90f; break;
            case 6: rotation = 270f; break;
            case 7: rotation = 90f; scaleX = -1f; break;
        }

        // Di chuyển tâm vẽ để Transform
        _canvas.Translate(finalX + finalW / 2f, finalY + finalH / 2f);
        if (rotation != 0) _canvas.RotateDegrees(rotation);
        _canvas.Scale(scaleX, scaleY);
        _canvas.Translate(-finalW / 2f, -finalH / 2f);

        // --- 5. Source Rect và Destination Rect ---
        // Source: Cắt chính xác phần pixel trên ảnh gốc, có tính đến manual clipping
        SKRect srcRect = new SKRect(
            x0 + offsetSrcX + mirrorOffset,
            y0 + offsetSrcY,
            x0 + offsetSrcX + mirrorOffset + drawW,
            y0 + offsetSrcY + drawH
        );

        SKRect destRect = new SKRect(0, 0, finalW, finalH);
        this._paint.Color = SKColors.White.WithAlpha(255);
        this._paint.ColorFilter = null;
        _canvas.DrawBitmap(image.texture, srcRect, destRect, _paint);

        _canvas.Restore();
    }

    public void drawImage(Image image, int x, int y, int anchor)
	{
		if (image != null)
		{
			drawRegion(image, 0, 0, getImageWidth(image), getImageHeight(image), 0, x, y, anchor);
		}
	}

	public void drawImageFog(Image image, int x, int y, int anchor)
	{
		if (image != null)
		{
			drawRegion(image, 0, 0, image.texture.Width, image.texture.Height, 0, x, y, anchor);
		}
	}

	public void drawImage(Image image, int x, int y)
	{
		if (image != null)
		{
			drawRegion(image, 0, 0, getImageWidth(image), getImageHeight(image), 0, x, y, TOP | LEFT);
		}
	}

	public void drawImage(Image image, float x, float y, int anchor)
	{
		if (image != null)
		{
			drawRegion(image, 0, 0, getImageWidth(image), getImageHeight(image), 0, x, y, anchor);
		}
	}

	public void drawRoundRect(int x, int y, int w, int h, int arcWidth, int arcHeight)
	{
		drawRect(x, y, w, h);
	}

	public void fillRoundRect(int x, int y, int width, int height, int arcWidth, int arcHeight)
	{
		fillRect(x, y, width, height);
	}

	public void reset()
	{
		isClip = false;
		isTranslate = false;
		translateX = 0;
		translateY = 0;
	}

	//public Rect intersectRect(Rect r1, Rect r2)
	//{
	//	float num = r1.x;
	//	float num2 = r1.y;
	//	float x = r2.x;
	//	float y = r2.y;
	//	float num3 = num;
	//	num3 += r1.width;
	//	float num4 = num2;
	//	num4 += r1.height;
	//	float num5 = x;
	//	num5 += r2.width;
	//	float num6 = y;
	//	num6 += r2.height;
	//	if (num < x)
	//	{
	//		num = x;
	//	}
	//	if (num2 < y)
	//	{
	//		num2 = y;
	//	}
	//	if (num3 > num5)
	//	{
	//		num3 = num5;
	//	}
	//	if (num4 > num6)
	//	{
	//		num4 = num6;
	//	}
	//	num3 -= num;
	//	num4 -= num2;
	//	if (num3 < -30000f)
	//	{
	//		num3 = -30000f;
	//	}
	//	if (num4 < -30000f)
	//	{
	//		num4 = -30000f;
	//	}
	//	return new Rect(num, num2, (int)num3, (int)num4);
	//}

	//public void drawImageScale(Image image, int x, int y, int w, int h, int tranform)
	//{
	//	GUI.color = Color.red;
	//	x *= zoomLevel;
	//	y *= zoomLevel;
	//	w *= zoomLevel;
	//	h *= zoomLevel;
	//	if (image != null)
	//	{
	//		Graphics.DrawTexture(new Rect(x + translateX, y + translateY, (tranform != 0) ? (-w) : w, h), image.texture);
	//	}
	//}

	//public void drawImageSimple(Image image, int x, int y)
	//{
	//	x *= zoomLevel;
	//	y *= zoomLevel;
	//	if (image != null)
	//	{
	//		Graphics.DrawTexture(new Rect(x, y, image.w, image.h), image.texture);
	//	}
	//}

	public static int getImageWidth(Image image)
	{
        int sizeW = 0;
        try
        {
            sizeW = image.getWidth();

        }
        catch (Exception ez)
        {
            Console.WriteLine("Error getting image width: " + ez.Message);
        }
        return sizeW;
    }

	public static int getImageHeight(Image image)
	{
        int sizeH = 0;
        try
        {
            sizeH = image.getHeight();

        }
        catch (Exception ex)
        {
            Console.WriteLine("Error getting image height: " + ex.Message);
        }
        return sizeH;
    }

	public static bool isNotTranColor(SKColor color)
	{
        return color.Alpha > 0;
        //if ( color.Alpha == 0||color == SKColor.Empty || color == transParentColor)
        //{
        //	return false;
        //}
        //return true;
    }

    public static Image blend(Image img0, float level, int rgb)
	{
        if (img0 == null || img0.texture == null) return null;

        // 1. Trích xuất RGB từ số nguyên (0-255)
        byte targetR = (byte)((rgb >> 16) & 0xFF);
        byte targetG = (byte)((rgb >> 8) & 0xFF);
        byte targetB = (byte)(rgb & 0xFF);

        // 2. Tạo bản sao của bitmap cũ để không làm hỏng ảnh gốc
        SKBitmap newBitmap = img0.texture.Copy();

        // 3. Truy cập trực tiếp vào mảng Pixels (Rất nhanh trong .NET 8)
        SKColor[] pixels = newBitmap.Pixels;

        for (int i = 0; i < pixels.Length; i++)
        {
            SKColor c = pixels[i];

            // 4. Chỉ xử lý nếu pixel không trong suốt (Alpha > 0)
            if (c.Alpha > 0)
            {
                // Áp dụng công thức Lerp cho từng kênh màu
                byte r = (byte)(c.Red + (targetR - c.Red) * level);
                byte g = (byte)(c.Green + (targetG - c.Green) * level);
                byte b = (byte)(c.Blue + (targetB - c.Blue) * level);

                // Cập nhật lại màu mới nhưng giữ nguyên độ trong suốt (Alpha) cũ
                pixels[i] = new SKColor(r, g, b, c.Alpha);
            }
        }

        // 5. Gán lại mảng pixels đã xử lý và trả về đối tượng Image
        newBitmap.Pixels = pixels;

        // Giả sử class Image của bạn có constructor nhận SKBitmap
        return new Image(newBitmap);
    }

	public static SKColor setColorObj(int rgb)
	{
        byte b = (byte)(rgb & 0xFF);
        byte g = (byte)((rgb >> 8) & 0xFF);
        byte r = (byte)((rgb >> 16) & 0xFF);

  //      int num = rgb & 0xFF;
		//int num2 = (rgb >> 8) & 0xFF;
		//int num3 = (rgb >> 16) & 0xFF;
		//float num4 = (float)num / 256f;
		//float num5 = (float)num2 / 256f;
		//float num6 = (float)num3 / 256f;
		return new SKColor(r, g, b);
	}

	public void fillTrans(Image imgTrans, int x, int y, int w, int h)
	{
		setColor(0, 0);
		fillRect(x * zoomLevel, y * zoomLevel, w * zoomLevel, h * zoomLevel);
	}

    public static int blendColor(float level, int color, int colorBlend)
    {
        // 1. Trích xuất màu từ mã nguyên bằng hàm setColorObj bạn đã có
        SKColor cOrig = setColorObj(color);
        SKColor cBlend = setColorObj(colorBlend);

        // 2. Thực hiện nội suy tuyến tính (Lerp) cho từng kênh màu
        // Công thức: start + (end - start) * level
        int r = (int)(cOrig.Red + (cBlend.Red - cOrig.Red) * level);
        int g = (int)(cOrig.Green + (cBlend.Green - cOrig.Green) * level);
        int b = (int)(cOrig.Blue + (cBlend.Blue - cOrig.Blue) * level);

        // 3. Sử dụng Math.Clamp của .NET 8 để giữ giá trị luôn nằm trong 0-255
        r = Math.Clamp(r, 0, 255);
        g = Math.Clamp(g, 0, 255);
        b = Math.Clamp(b, 0, 255);

        // 4. Đóng gói lại thành mã màu 0xRRGGBB bằng toán tử bitwise OR (|)
        return (r << 16) | (g << 8) | b;
    }

    public static int getIntByColor(SKColor cl)
	{
        int r = cl.Red;
        int g = cl.Green;
        int b = cl.Blue;
        return (r << 16) | (g << 8) | b;
    }

	public static int getRealImageWidth(Image img)
	{
		return img.w;
	}

	public static int getRealImageHeight(Image img)
	{
		return img.h;
	}

	public void fillArg(int i, int j, int k, int l, int m, int n)
	{
		fillRect(i * zoomLevel, j * zoomLevel, k * zoomLevel, l * zoomLevel);
	}

	//public void CreateLineMaterial()
	//{
	//	if (!lineMaterial)
	//	{
	//		lineMaterial = new Material("Shader \"Lines/Colored Blended\" {SubShader { Pass {  Blend SrcAlpha OneMinusSrcAlpha  ZWrite Off Cull Off Fog { Mode Off }  BindChannels { Bind \"vertex\", vertex Bind \"color\", color }} } }");
	//		lineMaterial.hideFlags = HideFlags.HideAndDontSave;
	//		lineMaterial.shader.hideFlags = HideFlags.HideAndDontSave;
	//	}
	//}

	//public void drawlineGL(MyVector totalLine)
	//{
	//	lineMaterial.SetPass(0);
	//	GL.PushMatrix();
	//	GL.Begin(1);
	//	for (int i = 0; i < totalLine.size(); i++)
	//	{
	//		mLine mLine2 = (mLine)totalLine.elementAt(i);
	//		GL.Color(new Color(mLine2.r, mLine2.g, mLine2.b, mLine2.a));
	//		int num = mLine2.x1 * zoomLevel;
	//		int num2 = mLine2.y1 * zoomLevel;
	//		int num3 = mLine2.x2 * zoomLevel;
	//		int num4 = mLine2.y2 * zoomLevel;
	//		if (isTranslate)
	//		{
	//			num += translateX;
	//			num2 += translateY;
	//			num3 += translateX;
	//			num4 += translateY;
	//		}
	//		for (int j = 0; j < zoomLevel; j++)
	//		{
	//			GL.Vertex(new SKPoint(num + j, num2 + j));
	//			GL.Vertex(new SKPoint(num3 + j, num4 + j));
	//			if (j > 0)
	//			{
	//				GL.Vertex(new SKPoint(num + j, num2));
	//				GL.Vertex(new SKPoint(num3 + j, num4));
	//				GL.Vertex(new SKPoint(num, num2 + j));
	//				GL.Vertex(new SKPoint(num3, num4 + j));
	//			}
	//		}
	//	}
	//	GL.End();
	//	GL.PopMatrix();
	//	totalLine.removeAllElements();
	//}

	//public void drawLine(mGraphics g, int x, int y, int xTo, int yTo, int nLine, int color)
	//{
	//	MyVector myVector = new MyVector();
	//	for (int i = 0; i < nLine; i++)
	//	{
	//		myVector.addElement(new mLine(x, y, xTo + i, yTo + i, color));
	//	}
	//	g.drawlineGL(myVector);
	//}

	internal void drawRegion(Small img, int p1, int p2, int p3, int p4, int transform, int x, int y, int anchor)
	{
		throw new NotImplementedException();
	}
}
