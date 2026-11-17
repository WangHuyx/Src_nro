using System;
using System.Collections;
using static System.Net.Mime.MediaTypeNames;


public class mFont
{
	public static int LEFT = 0;

	public static int RIGHT = 1;

	public static int CENTER = 2;

	public static int RED = 0;

	public static int YELLOW = 1;

	public static int GREEN = 2;

	public static int FATAL = 3;

	public static int MISS = 4;

	public static int ORANGE = 5;

	public static int ADDMONEY = 6;

	public static int MISS_ME = 7;

	public static int FATAL_ME = 8;

	public static int HP = 9;

	public static int MP = 10;

	private int space;

	private Image imgFont;

	//private string strFont;

	//private int[][] fImages;

	public static int yAddFont;

	public static int[] colorJava = new int[31]
	{
		0, 16711680, 6520319, 16777215, 16755200, 5449989, 21285, 52224, 7386228, 16771788,
		0, 65535, 21285, 16776960, 5592405, 16742263, 33023, 8701737, 15723503, 7999781,
		16768815, 14961237, 4124899, 4671303, 16096312, 16711680, 16755200, 52224, 16777215, 6520319,
		16096312
	};

	public static mFont gI;

	public static mFont tahoma_7b_red;

	public static mFont tahoma_7b_blue;

	public static mFont tahoma_7b_white;

	public static mFont tahoma_7b_yellow;

	public static mFont tahoma_7b_yellowSmall;

	public static mFont tahoma_7b_dark;

	public static mFont tahoma_7b_green2;

	public static mFont tahoma_7b_green;

	public static mFont tahoma_7b_focus;

	public static mFont tahoma_7b_unfocus;

	public static mFont tahoma_7;

	public static mFont tahoma_7_blue1;

	public static mFont tahoma_7_blue1Small;

	public static mFont tahoma_7_green2;

	public static mFont tahoma_7_yellow;

	public static mFont tahoma_7_grey;

	public static mFont tahoma_7_red;

	public static mFont tahoma_7_blue;

	public static mFont tahoma_7_green;

	public static mFont tahoma_7_white;

	public static mFont tahoma_8b;

	public static mFont number_yellow;

	public static mFont number_red;

	public static mFont number_green;

	public static mFont number_gray;

	public static mFont number_orange;

	public static mFont bigNumber_red;

	public static mFont bigNumber_While;

	public static mFont bigNumber_yellow;

	public static mFont bigNumber_green;

	public static mFont bigNumber_orange;

	public static mFont bigNumber_blue;

	public static mFont bigNumber_black;

	public static mFont nameFontRed;

	public static mFont nameFontYellow;

	public static mFont nameFontGreen;

	public static mFont tahoma_7_greySmall;

	public static mFont tahoma_7b_yellowSmall2;

	public static mFont tahoma_7b_green2Small;

	public static mFont tahoma_7_whiteSmall;

	public static mFont tahoma_7b_greenSmall;

	public SKTypeface myFont;

    //private SKPaint _measurePaint;

    private int height;

	private int wO;

	public SKColor color1 = SKColors.White;

	public SKColor color2 = SKColors.Gray;

    private readonly SKPaint paint;
    public float yOffset;

    public sbyte id;

	public int fstyle;

	public string st1 = "áàảãạăắằẳẵặâấầẩẫậéèẻẽẹêếềểễệíìỉĩịóòỏõọôốồổỗộơớờởỡợúùủũụưứừửữựýỳỷỹỵđÁÀẢÃẠĂẮẰẲẴẶÂẤẦẨẪẬÉÈẺẼẸÊẾỀỂỄỆÍÌỈĨỊÓÒỎÕỌÔỐỒỔỖỘƠỚỜỞỠỢÚÙỦŨỤƯỨỪỬỮỰÝỲỶỸỴĐ";

	public string st2 = "\u00b8µ¶·¹\u00a8¾»¼½Æ©ÊÇÈÉËÐÌÎÏÑªÕÒÓÔÖÝ×ØÜÞãßáâä«èåæçé¬íêëìîóïñòô\u00adøõö÷ùýúûüþ®\u00b8µ¶·¹¡¾»¼½Æ¢ÊÇÈÉËÐÌÎÏÑ£ÕÒÓÔÖÝ×ØÜÞãßáâä¤èåæçé¥íêëìîóïñòô¦øõö÷ùýúûüþ§";

	public const string str = " 0123456789+-*='_?.,<>/[]{}!@#$%^&*():aáàảãạâấầẩẫậăắằẳẵặbcdđeéèẻẽẹêếềểễệfghiíìỉĩịjklmnoóòỏõọôốồổỗộơớờởỡợpqrstuúùủũụưứừửữựvxyýỳỷỹỵzwAÁÀẢÃẠĂẰẮẲẴẶÂẤẦẨẪẬBCDĐEÉÈẺẼẸÊẾỀỂỄỆFGHIÍÌỈĨỊJKLMNOÓÒỎÕỌÔỐỒỔỖỘƠỚỜỞỠỢPQRSTUÚÙỦŨỤƯỨỪỬỮỰVXYÝỲỶỸỴZW";

	private int yAdd = 0;

	private string pathImage;

	public static sbyte cl__white;

	public static sbyte cl__yellow;

	public static sbyte cl__yellowSmall;

	public static sbyte cl_dark = 0;

	public static sbyte cl_green = 1;

	public static sbyte cl__blue = 2;

	public static sbyte cl_red = 3;

    private int _cachedHeight;

    private readonly System.Collections.Generic.Dictionary<(string, int), string[]> _splitCache = new();

    public mFont(SKTypeface typeface, float size, SKColor color, sbyte b)
    {
        id = b;
        paint = new SKPaint
        {
            Typeface = typeface,
            TextSize = size,
            Color = color,
            IsAntialias = true,
            FilterQuality = SKFilterQuality.None,
            SubpixelText = true
        };

        var metrics = paint.FontMetrics;
        yOffset = System.Math.Abs(metrics.Ascent);
        float physicalHeight = metrics.Descent - metrics.Ascent;
        _cachedHeight = (int)System.Math.Ceiling(physicalHeight / mGraphics.zoomLevel);
    }

    public static void init()
	{
        float size =  (mGraphics.zoomLevel == 1) ? 9.5f : 11.5f;
        float sizeb = (mGraphics.zoomLevel == 1) ? 10f : 12.5f;
        SKTypeface normal = SKFontManager.Default.MatchFamily("Arial", SKFontStyle.Normal);
        SKTypeface bold = SKFontManager.Default.MatchFamily("Arial", SKFontStyle.Bold);
		gI = new mFont(SKTypeface.Default,size,SKColors.White,0);
        tahoma_7b_red = new mFont(bold, sizeb, SKColors.Red,1);
        tahoma_7b_blue = new mFont(bold, sizeb, SKColors.Blue,2);
        tahoma_7b_white = new mFont(bold, sizeb, SKColors.White,3);
        tahoma_7b_yellow = new mFont(bold, sizeb, SKColors.Yellow,4);
        tahoma_7b_yellowSmall = new mFont(bold, sizeb, SKColors.LightYellow,4);
        tahoma_7b_dark = new mFont(bold, sizeb, SKColors.DarkGray,5);
        tahoma_7b_green2 = new mFont(bold, sizeb, SKColors.GreenYellow,6);
        tahoma_7b_green = new mFont(bold, sizeb, SKColors.Green,7);
        tahoma_7b_focus = new mFont(bold, sizeb, SKColors.Gray,8);
        tahoma_7b_unfocus = new mFont(bold, sizeb, SKColors.LightGray,9);
        tahoma_7 = new mFont(normal, size, SKColors.Black,10);
        tahoma_7_blue1 = new mFont(normal, size, SKColors.DarkBlue,11);
		tahoma_7_blue1Small = tahoma_7_blue1;
        tahoma_7_green2 = new mFont(normal, size, SKColors.GreenYellow,12);
        tahoma_7_yellow = new mFont(normal, size, SKColors.Yellow,13);
        tahoma_7_grey = new mFont(normal, size, SKColors.Gray,14);
        tahoma_7_red = new mFont(normal, size, SKColors.Red,15);
        tahoma_7_blue = new mFont(normal, size, SKColors.Blue,16);
        tahoma_7_green = new mFont(normal, size, SKColors.Green,17);
        tahoma_7_white = new mFont(normal, size, SKColors.White,18);
        tahoma_8b = new mFont(bold, sizeb, SKColors.Black,19);
        number_yellow = new mFont(normal, sizeb, SKColors.Yellow,20);
        number_red = new mFont(normal, sizeb, SKColors.Red,21);
        number_green = new mFont(normal, sizeb, SKColors.Green,22);
        number_gray = new mFont(normal, sizeb, SKColors.Gray,23);
        number_orange = new mFont(normal, sizeb, SKColors.Orange,24);
        bigNumber_red = number_red;
        bigNumber_While = tahoma_7b_white;
        bigNumber_yellow = number_yellow;
        bigNumber_green = number_green;
        bigNumber_orange = number_orange;
        bigNumber_blue = tahoma_7_blue1;
        nameFontRed = tahoma_7_red;
        nameFontYellow = tahoma_7_yellow;
        nameFontGreen = tahoma_7_green;
        tahoma_7_greySmall = tahoma_7_grey;
        tahoma_7b_yellowSmall2 = tahoma_7_yellow;
        tahoma_7b_green2Small = tahoma_7b_green2;
        tahoma_7_whiteSmall = tahoma_7_white;
        tahoma_7b_greenSmall = tahoma_7b_green;
        tahoma_7_blue1Small = tahoma_7_blue1;
        if (mGraphics.zoomLevel == 1)
		{
            yAddFont = -3;
            return;
		}
		yAddFont = 1;
	}

	public void setHeight(int height)
	{
		this.height = height;
	}

	public SKColor setColor(int rgb)
	{
        // Lấy trực tiếp các kênh màu (0 - 255) và ép kiểu sang byte
        byte b = (byte)(rgb & 0xFF);
        byte g = (byte)((rgb >> 8) & 0xFF);
        byte r = (byte)((rgb >> 16) & 0xFF);

        // Khởi tạo SKColor với thứ tự (Red, Green, Blue)
        return new SKColor(r, g, b);
    }

	public SKColor bigColor(int id)
	{
		SKColor[] array = new SKColor[7]
		{
			SKColors.Red,
			SKColors.Yellow,
			SKColors.Green,
			SKColors.White,
			setColor(40404),
			SKColors.Red,
			SKColors.Black
		};
		return array[id - 25];
	}

	public void setColorByID(int ID)
	{
		color1 = setColor(colorJava[ID]);
		color2 = setColor(colorJava[ID]);
	}

	//fix
	public void setTypePaint(mGraphics g, string st, int x, int y, int align, sbyte idFont)
	{
        sbyte targetId = (idFont > 0) ? idFont : id;
        //x--;
        if (id > 24)
        {
			SKColor[] array = new SKColor[6]
			{
					setColor(6029312),
					setColor(7169025),
					setColor(7680),
					setColor(0),
					setColor(9264),
					setColor(6029312)
			};
			color1 = array[id - 25];
			color2 = array[id - 25];
            //_drawString(g, st, x, y, align);
            //_drawString(g, st, x + 1, y, align);
            //_drawString(g, st, x - 1, y, align);
            //_drawString(g, st, x, y - 1, align);
            //_drawString(g, st, x, y + 1, align);
            //_drawString(g, st, x + 1, y + 1, align);
            //_drawString(g, st, x + 1, y - 1, align);
            //_drawString(g, st, x - 1, y - 1, align);
            //_drawString(g, st, x - 1, y + 1, align);
            color1 = bigColor(id);
			color2 = bigColor(id);
		}
        else
        {
            setColorByID(targetId);
        }
        _drawString(g, st, x, y - yAdd, align);
    }

    //public SKColor setColorFont(sbyte id)
    //{
    //    return setColor(colorJava[id]);
    //}

    public void drawString(mGraphics g, string st, int x, int y, int align)
	{
        
        setTypePaint(g, st, x, y, align, 0);
		
	}

	public void drawStringBorder(mGraphics g, string st, int x, int y, int align)
	{
		if (mGraphics.zoomLevel == 1)
		{
			drawString(g, st, x, y, align);
		}
		else
		{
			setTypePaint(g, st, x, y, align, 0);
		}
	}

	public void drawStringBorder(mGraphics g, string st, int x, int y, int align, mFont font2)
	{
		if (mGraphics.zoomLevel == 1)
		{
			drawString(g, st, x, y, align, font2);
		}
		else
		{
			drawStringBd(g, st, x, y, align, font2);
		}
	}

	public void drawStringBd(mGraphics g, string st, int x, int y, int align, mFont font)
	{
		//setTypePaint(g, st, x - 1, y - 1, align, 0);
		//setTypePaint(g, st, x - 1, y + 1, align, 0);
		//setTypePaint(g, st, x + 1, y - 1, align, 0);
		//setTypePaint(g, st, x + 1, y + 1, align, 0);
		//setTypePaint(g, st, x, y - 1, align, 0);
		//setTypePaint(g, st, x, y + 1, align, 0);
		//setTypePaint(g, st, x + 1, y, align, 0);
		//setTypePaint(g, st, x - 1, y, align, 0);
		setTypePaint(g, st, x, y, align, 0);
	}

	public void drawString(mGraphics g, string st, int x, int y, int align, mFont font)
	{
        
        sbyte targetId = /*(font != null) ? font.id :*/ this.id;
        setTypePaint(g, st, x, y + 1, align, targetId);
    }

    public MyVector splitFontVector(string src, int lineWidth)
    {
        MyVector myVector = new MyVector();
        string[] arr = splitFontArray(src, lineWidth);
        for (int i = 0; i < arr.Length; i++)
        {
            myVector.addElement(arr[i]);
        }
        return myVector;
    }

    public string[] splitFontArray(string src, int lineWidth)
    {
        if (string.IsNullOrEmpty(src)) return Array.Empty<string>();

        var key = (src, lineWidth);
        if (_splitCache.TryGetValue(key, out string[] cachedLines))
        {
            return cachedLines;
        }

        // Giới hạn kích thước cache để tránh đầy RAM nếu có chuỗi chat ngẫu nhiên từ người chơi
        if (_splitCache.Count > 500) _splitCache.Clear();

        List<string> lines = new List<string>();
        ReadOnlySpan<char> span = src.AsSpan();

        while (!span.IsEmpty)
        {
            int newlineIdx = span.IndexOf('\n');
            ReadOnlySpan<char> para = (newlineIdx >= 0) ? span.Slice(0, newlineIdx) : span;
            span = (newlineIdx >= 0) ? span.Slice(newlineIdx + 1) : ReadOnlySpan<char>.Empty;

            if (para.IsEmpty)
            {
                lines.Add(string.Empty);
                continue;
            }

            while (!para.IsEmpty)
            {
                // Đo trên chuỗi tạm của dòng hiện tại
                string tempPara = para.ToString();
                int charCount = (int)this.paint.BreakText(tempPara, lineWidth, out _);
                if (charCount <= 0) break;

                if (charCount < para.Length)
                {
                    int lastSpace = para.Slice(0, charCount).LastIndexOf(' ');
                    if (lastSpace > 0) charCount = lastSpace;
                }

                lines.Add(para.Slice(0, charCount).Trim().ToString());
                para = para.Slice(charCount).TrimStart();
            }
        }

        string[] result = lines.ToArray();
        _splitCache[key] = result;
        return result;
    }

    public bool compare(string strSource, string str)
    {
        if (string.IsNullOrEmpty(str) || str.Length != 1) return false;
        return strSource.IndexOf(str[0]) >= 0;
    }

    public int getWidth(string s)
	{
		//if (mGraphics.zoomLevel == 1)
		//{
		//	int num = 0;
		//	for (int i = 0; i < s.Length; i++)
		//	{
		//		int num2 = strFont.IndexOf(s[i]);
		//		if (num2 == -1)
		//		{
		//			num2 = 0;
		//		}
		//		num += fImages[num2][2] + space;
		//	}
		//	return num;
		//}
		return getWidthExactOf(s);
	}

	//fix
	public int getWidthExactOf(string s)
	{
        if (string.IsNullOrEmpty(s)) return 0;
        float width = paint.MeasureText(s);
        return (int)System.Math.Ceiling(width / mGraphics.zoomLevel);
    }

    //fix
    public int getHeight() => _cachedHeight;

    //fix
    public void _drawString(mGraphics g, string st, int x0, int y0, int align)
	{
        if (string.IsNullOrEmpty(st)) return;

        paint.Color = this.color1;
        paint.TextAlign = align switch
        {
            1 => SKTextAlign.Right,  // Align RIGHT
            2 => SKTextAlign.Center, // Align CENTER
            3 => SKTextAlign.Center,
            _ => SKTextAlign.Left    // Mặc định LEFT (0)
        };
        // 4. VẼ TRỰC TIẾP: Sử dụng x0 làm điểm neo duy nhất
        g.drawString(st, x0, y0, paint);
    }

	public void reloadImage()
	{
        if (mGraphics.zoomLevel == 1)
        {
            // nạp từ Resources/res/x1/myfont/tahoma_...png
            imgFont = GameCanvas.loadImage(pathImage);
        }
    }

	public void freeImage()
	{
	}

	public static mFont GetFont(sbyte colorCode)
	{
		return colorCode switch
		{
			0 => tahoma_7b_dark, 
			1 => tahoma_7b_green, 
			2 => tahoma_7b_blue, 
			3 => tahoma_7_red, 
			4 => tahoma_7_green, 
			5 => tahoma_7_blue, 
			7 => tahoma_7b_red, 
			_ => tahoma_7, 
		};
	}
}
