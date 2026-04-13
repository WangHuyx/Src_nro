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

    //public mFont(string strFont, string pathImage, string pathData, int space)
    //{
    //       try
    //       {
    //           this.strFont = strFont;
    //           this.space = space;

    //		// 1. Xử lý đường dẫn ảnh vật lý (Sử dụng hàm đã sửa ở Image)
    //		// pathImage cũ: "/myfont/tahoma_7b_red.png"
    //		this.pathImage = pathImage;
    //		reloadImage();

    //		// 2. Xử lý nạp file dữ liệu font (.bin)
    //		// pathData cũ: "res/myfont/tahoma_7b"
    //		string binPath = "res/"+pathData.TrimStart('/') + ".bin";

    //           // DataInputStream hiện tại đã được sửa để đọc từ thư mục vật lý (Data/res/...)
    //           DataInputStream dataInputStream = new DataInputStream(binPath);

    //           try
    //           {
    //               // Đọc dữ liệu nhị phân từ file .bin
    //               fImages = new int[dataInputStream.readShort()][];
    //               for (int i = 0; i < fImages.Length; i++)
    //               {
    //                   fImages[i] = new int[4];
    //                   fImages[i][0] = dataInputStream.readShort(); // x
    //                   fImages[i][1] = dataInputStream.readShort(); // y
    //                   fImages[i][2] = dataInputStream.readShort(); // w
    //                   fImages[i][3] = dataInputStream.readShort(); // h
    //                   setHeight(fImages[i][3]);
    //               }
    //               dataInputStream.close();
    //           }
    //           catch (Exception ex)
    //           {
    //               //Console.WriteLine("[ERROR] Load Font Data Fail: " + binPath + " -> " + ex.Message);
    //           }
    //       }
    //       catch (Exception ex3)
    //       {
    //           //Console.WriteLine(ex3.StackTrace.ToString());
    //       }
    //   }
    public mFont(SKTypeface typeface, float size, SKColor color, sbyte b)
    {
		id = b;
        paint = new SKPaint
        {
            Typeface = typeface,
            TextSize = size,
            Color = color,
            IsAntialias = false, // Giữ pixel-art style
            FilterQuality = SKFilterQuality.None,
            SubpixelText = true
        };

        // Tính toán yOffset để chữ không bị lệch so với bản cũ
        // Công thức: yOffset = |Ascent| (khoảng cách từ baseline lên đỉnh chữ) [cite: 2026-04-09]
        var metrics = paint.FontMetrics;
        yOffset = System.Math.Abs(metrics.Ascent);
    }
    //public mFont(sbyte id)
    //{
    //    string text = "chelthm";
    //    if ((id > 0 && id < 10) || id == 19)
    //    {
    //        yAdd = 1;
    //        text = "barmeneb";
    //    }
    //    else if (id >= 10 && id <= 18)
    //    {
    //        text = "chelthm";
    //        yAdd = 2;
    //    }
    //    else if (id > 24)
    //    {
    //        text = "staccato";
    //    }
    //    this.id = id;

    //    // 1. Xây dựng đường dẫn vật lý (Physical Path)
    //    // Lưu ý: Files trên ổ cứng bắt buộc phải có phần mở rộng (ví dụ: .ttf hoặc .otf)
    //    string folderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "FontSys", "x" + mGraphics.zoomLevel);
    //    string fontPath = Path.Combine(folderPath, text + ".ttf");

    //    try
    //    {
    //        if (File.Exists(fontPath))
    //        {
    //            // 2. Load font trực tiếp từ file vật lý
    //            myFont = SKTypeface.FromFile(fontPath);
    //        }
    //        else
    //        {
    //            // Fallback nếu không tìm thấy file, sử dụng font mặc định của hệ thống
    //            myFont = SKTypeface.Default;
    //            //Console.WriteLine($"[WARNING] Font file not found: {fontPath}. Using default font.");
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        myFont = SKTypeface.Default;
    //        //Console.WriteLine($"[ERROR] Failed to load font: {ex.Message}");
    //    }

    //    // 3. Xử lý logic màu sắc cũ
    //    if (id < 25)
    //    {
    //        color1 = setColorFont(id);
    //        color2 = setColorFont(id);
    //    }
    //    else
    //    {
    //        color1 = bigColor(id);
    //        color2 = bigColor(id);
    //    }

    //    // 4. Đo độ rộng ký tự (Cần SKPaint để đo)
    //    wO = getWidthExactOf("o");
    //}

    public static void init()
	{
        float size =  (mGraphics.zoomLevel == 1) ? 9f : 15f;
        float sizeb = (mGraphics.zoomLevel == 1) ? 9.5f : 16f;
        SKTypeface normal = SKFontManager.Default.MatchFamily("Tahoma", SKFontStyle.Normal);
        SKTypeface bold = SKFontManager.Default.MatchFamily("Tahoma", SKFontStyle.Bold);
		gI = new mFont(SKTypeface.Default,size,SKColors.Black,0);
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
			_drawString(g, st, x + 1, y, align);
			_drawString(g, st, x - 1, y, align);
			_drawString(g, st, x, y - 1, align);
			_drawString(g, st, x, y + 1, align);
			_drawString(g, st, x + 1, y + 1, align);
			_drawString(g, st, x + 1, y - 1, align);
			_drawString(g, st, x - 1, y - 1, align);
			_drawString(g, st, x - 1, y + 1, align);
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
        //Res.outz("Debug String:"+st);
        //if (mGraphics.zoomLevel == 1)
        //{
        //	int length = st.Length;
        //	int num = align switch
        //	{
        //		0 => x, 
        //		1 => x - getWidth(st), 
        //		_ => x - (getWidth(st) >> 1), 
        //	};
        //	for (int i = 0; i < length; i++)
        //	{
        //		int num2 = strFont.IndexOf(st[i] + string.Empty);
        //		if (num2 == -1)
        //		{
        //			num2 = 0;
        //		}
        //		if (num2 > -1)
        //		{
        //			int x2 = fImages[num2][0];
        //			int num3 = fImages[num2][1];
        //			int w = fImages[num2][2];
        //			int num4 = fImages[num2][3];
        //			if (num3 + num4 > imgFont.texture.Height)
        //			{
        //				num3 -= imgFont.texture.Height;
        //				x2 = imgFont.texture.Width / 2;
        //			}
        //			g.drawRegion(imgFont, x2, num3, w, num4, 0, num, y, 20);
        //		}
        //		num += fImages[num2][2] + space;
        //	}
        //}
        //else
        //{
        setTypePaint(g, st, x, y, align, 0);
		//}
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
		setTypePaint(g, st, x - 1, y - 1, align, 0);
		setTypePaint(g, st, x - 1, y + 1, align, 0);
		setTypePaint(g, st, x + 1, y - 1, align, 0);
		setTypePaint(g, st, x + 1, y + 1, align, 0);
		setTypePaint(g, st, x, y - 1, align, 0);
		setTypePaint(g, st, x, y + 1, align, 0);
		setTypePaint(g, st, x + 1, y, align, 0);
		setTypePaint(g, st, x - 1, y, align, 0);
		setTypePaint(g, st, x, y, align, 0);
	}

	public void drawString(mGraphics g, string st, int x, int y, int align, mFont font)
	{
        //Console.WriteLine($"{st} {x} {y}");
        //if (string.IsNullOrEmpty(st)) return;
        //      if (mGraphics.zoomLevel == 1)
        //{
        //	int length = st.Length;
        //	int num = align switch
        //	{
        //		0 => x,
        //		1 => x - getWidth(st),
        //		_ => x - (getWidth(st) >> 1),
        //	};
        //	for (int i = 0; i < length; i++)
        //	{
        //		int num2 = strFont.IndexOf(st[i]);
        //		if (num2 == -1)
        //		{
        //			num2 = 0;
        //		}
        //		if (num2 > -1)
        //		{
        //			int x2 = fImages[num2][0];
        //			int num3 = fImages[num2][1];
        //			int w = fImages[num2][2];
        //			int num4 = fImages[num2][3];
        //			if (num3 + num4 > imgFont.texture.Height)
        //			{
        //				num3 -= imgFont.texture.Height;
        //				x2 = imgFont.texture.Width / 2;
        //			}
        //			//if (!GameCanvas.lowGraphic && font != null)
        //			//{
        //			//	g.drawRegion(font.imgFont, x2, num3, w, num4, 0, num + 1, y, 20);
        //			//	g.drawRegion(font.imgFont, x2, num3, w, num4, 0, num, y + 1, 20);
        //			//}
        //			//g.drawRegion(imgFont, x2, num3, w, num4, 0, num, y, 20);
        //		}
        //		num += fImages[num2][2] + space;
        //	}
        //}
        //else
        //{
        sbyte targetId = (font != null) ? font.id : this.id;
        setTypePaint(g, st, x, y + 1, align, targetId);
        //setTypePaint(g, st, x, y, align, 0);
        //}
    }

    public MyVector splitFontVector(string src, int lineWidth)
	{
        MyVector myVector = new MyVector();
        if (string.IsNullOrEmpty(src)) return myVector;

        // 1. Xử lý các dấu xuống dòng chủ động (\n) trước [cite: 2026-01-28]
        string[] paragraphs = src.Split('\n');

        foreach (string para in paragraphs)
        {
            string remainingText = para;

            while (remainingText.Length > 0)
            {
                // 2. BreakText trả về số lượng ký tự tối đa vừa khít lineWidth [cite: 2026-04-09]
                int count = (int)this.paint.BreakText(remainingText, lineWidth, out _);

                if (count <= 0) break; // Tránh vòng lặp vô tận nếu lineWidth quá nhỏ

                if (count < remainingText.Length)
                {
                    // 3. Logic Word-Wrap: Tìm khoảng trắng gần nhất để không bị cắt ngang chữ [cite: 2026-04-10]
                    int lastSpace = remainingText.LastIndexOf(' ', count);
                    if (lastSpace > 0)
                    {
                        count = lastSpace;
                    }
                }

                // 4. Cắt dòng và thêm vào kết quả [cite: 2026-04-06]
                string line = remainingText.Substring(0, count).Trim();
                myVector.addElement(line);

                // Cập nhật phần văn bản còn lại [cite: 2026-04-10]
                remainingText = remainingText.Substring(count).TrimStart();
            }
        }

        return myVector;
        //MyVector myVector = new MyVector();
        //string text = string.Empty;
        //for (int i = 0; i < src.Length; i++)
        //{
        //	if (src[i] == '\n' || src[i] == '\b')
        //	{
        //		myVector.addElement(text);
        //		text = string.Empty;
        //		continue;
        //	}
        //	text += src[i];
        //	if (getWidth(text) > lineWidth)
        //	{
        //		int num = 0;
        //		num = text.Length - 1;
        //		while (num >= 0 && text[num] != ' ')
        //		{
        //			num--;
        //		}
        //		if (num < 0)
        //		{
        //			num = text.Length - 1;
        //		}
        //		myVector.addElement(text.Substring(0, num));
        //		i = i - (text.Length - num) + 1;
        //		text = string.Empty;
        //	}
        //	if (i == src.Length - 1 && !text.Trim().Equals(string.Empty))
        //	{
        //		myVector.addElement(text);
        //	}
        //}
        //return myVector;
    }

	//public string splitFirst(string str)
	//{
	//	string text = string.Empty;
	//	bool flag = false;
	//	for (int i = 0; i < str.Length; i++)
	//	{
	//		if (!flag)
	//		{
	//			string text2 = str.Substring(i);
	//			text = ((!compare(text2, " ")) ? (text + text2) : (text + str[i] + "-"));
	//			flag = true;
	//		}
	//		else if (str[i] == ' ')
	//		{
	//			flag = false;
	//		}
	//	}
	//	return text;
	//}

	//public string[] splitStrInLine(string src, int lineWidth)
	//{
	//	ArrayList arrayList = splitStrInLineA(src, lineWidth);
	//	string[] array = new string[arrayList.Count];
	//	for (int i = 0; i < arrayList.Count; i++)
	//	{
	//		array[i] = (string)arrayList[i];
	//	}
	//	return array;
	//}

	//public ArrayList splitStrInLineA(string src, int lineWidth)
	//{
	//	ArrayList arrayList = new ArrayList();
	//	int i = 0;
	//	int num = 0;
	//	int length = src.Length;
	//	if (length < 5)
	//	{
	//		arrayList.Add(src);
	//		return arrayList;
	//	}
	//	string text = string.Empty;
	//	try
	//	{
	//		while (true)
	//		{
	//			if (getWidthNotExactOf(text) < lineWidth)
	//			{
	//				text += src[num];
	//				num++;
	//				if (src[num] != '\n')
	//				{
	//					if (num < length - 1)
	//					{
	//						continue;
	//					}
	//					num = length - 1;
	//				}
	//			}
	//			if (num != length - 1 && src[num + 1] != ' ')
	//			{
	//				int num2 = num;
	//				while (src[num + 1] != '\n' && (src[num + 1] != ' ' || src[num] == ' ') && num != i)
	//				{
	//					num--;
	//				}
	//				if (num == i)
	//				{
	//					num = num2;
	//				}
	//			}
	//			string text2 = src.Substring(i, num + 1 - i);
	//			if (text2[0] == '\n')
	//			{
	//				text2 = text2.Substring(1, text2.Length - 1);
	//			}
	//			if (text2[text2.Length - 1] == '\n')
	//			{
	//				text2 = text2.Substring(0, text2.Length - 1);
	//			}
	//			arrayList.Add(text2);
	//			if (num == length - 1)
	//			{
	//				break;
	//			}
	//			for (i = num + 1; i != length - 1 && src[i] == ' '; i++)
	//			{
	//			}
	//			if (i == length - 1)
	//			{
	//				break;
	//			}
	//			num = i;
	//			text = string.Empty;
	//		}
	//	}
	//	catch (Exception ex)
	//	{
	//		Cout.LogWarning("EXCEPTION WHEN REAL SPLIT " + src + "\nend=" + num + "\n" + ex.Message + "\n" + ex.StackTrace);
	//		arrayList.Add(src);
	//	}
	//	return arrayList;
	//}

	public string[] splitFontArray(string src, int lineWidth)
	{
        if (string.IsNullOrEmpty(src)) return new string[0];

        List<string> lines = new List<string>();

        // 1. Tách các dòng có sẵn dấu xuống dòng \n trước [cite: 2026-01-28]
        string[] paragraphs = src.Split('\n');

        foreach (string para in paragraphs)
        {
            string text = para;
            while (text.Length > 0)
            {
                // 2. BreakText: Trả về số lượng ký tự có thể "nhét" vừa vào lineWidth [cite: 2026-04-09]
                int charCount = (int)this.paint.BreakText(text, lineWidth, out _);

                if (charCount <= 0) break;

                if (charCount < text.Length)
                {
                    // 3. Logic Word-Wrap: Tìm vị trí khoảng trắng cuối cùng để không bị cắt ngang từ [cite: 2026-04-10]
                    int lastSpace = text.LastIndexOf(' ', charCount);
                    if (lastSpace > 0)
                    {
                        charCount = lastSpace;
                    }
                }

                // 4. Cắt dòng và thêm vào danh sách
                lines.Add(text.Substring(0, charCount).Trim());

                // Cập nhật phần văn bản còn lại để tiếp tục vòng lặp
                text = text.Substring(charCount).TrimStart();
            }
        }

        return lines.ToArray();
        //MyVector myVector = splitFontVector(src, lineWidth);
        //string[] array = new string[myVector.size()];
        //for (int i = 0; i < myVector.size(); i++)
        //{
        //	array[i] = (string)myVector.elementAt(i);
        //}
        //return array;
    }

	public bool compare(string strSource, string str)
	{
		for (int i = 0; i < strSource.Length; i++)
		{
			if ((string.Empty + strSource[i]).Equals(str))
			{
				return true;
			}
		}
		return false;
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
        //try
        //{

        //// 2. Sử dụng MeasureText của SkiaSharp
        //// Kết quả trả về là float, ta ép kiểu về int
        //float width = _measurePaint.MeasureText(s);

        //// 3. Trả về giá trị đã chia cho zoomLevel theo logic cũ của bạn
        //return (int)(width / mGraphics.zoomLevel);

        ////GUIStyle gUIStyle = new GUIStyle();
        ////gUIStyle.font = myFont;
        ////return (int)gUIStyle.CalcSize(new GUIContent(s)).x / mGraphics.zoomLevel;
        //}
        //catch (Exception ex)
        //{
        //	Cout.LogError("GET WIDTH OF " + s + " FAIL.\n" + ex.Message + "\n" + ex.StackTrace);
        //	return getWidthNotExactOf(s);
        //}
    }

    //public int getWidthNotExactOf(string s)
    //{
    //	return s.Length * wO / mGraphics.zoomLevel;
    //}

    //fix
    public int getHeight()
	{
        var metrics = paint.FontMetrics;
        // Chiều cao vật lý = Descent (dương) - Ascent (âm)
        float physicalHeight = metrics.Descent - metrics.Ascent;
        // Trả về tọa độ logic (chia cho zoomLevel) để khớp với hệ thống tọa độ cũ của game
        return (int)System.Math.Ceiling(physicalHeight / mGraphics.zoomLevel);
        //if (mGraphics.zoomLevel == 1)
        //{
        //	return height;
        //}
        //if (height > 0)
        //{
        //	return height / mGraphics.zoomLevel;
        //}
        //GUIStyle gUIStyle = new GUIStyle();
        //gUIStyle.font = myFont;
        //try
        //{
        //	height = (int)gUIStyle.CalcSize(new GUIContent("Adg")).y + 2;
        //}
        //catch (Exception ex)
        //{
        //	Cout.LogError("FAIL GET HEIGHT " + ex.StackTrace);
        //	height = 20;
        //}
        //return height / mGraphics.zoomLevel;
    }

	//fix
	public void _drawString(mGraphics g, string st, int x0, int y0, int align)
	{
        if (string.IsNullOrEmpty(st)) return;

        // 1. Cộng khoảng cách dòng phụ (yAddFont)
        //y0 += yAddFont;

        // 2. Sử dụng đối tượng Paint để thiết lập phong cách vẽ

        paint.IsAntialias = false; // Giữ nét pixel cho game
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

	//public static string[] splitStringSv(string _text, string _searchStr)
	//{
	//	int num = 0;
	//	int startIndex = 0;
	//	int length = _searchStr.Length;
	//	int num2 = _text.IndexOf(_searchStr, startIndex);
	//	while (num2 != -1)
	//	{
	//		startIndex = num2 + length;
	//		num2 = _text.IndexOf(_searchStr, startIndex);
	//		num++;
	//	}
	//	string[] array = new string[num + 1];
	//	int num3 = _text.IndexOf(_searchStr);
	//	int num4 = 0;
	//	int num5 = 0;
	//	while (num3 != -1)
	//	{
	//		array[num5] = _text.Substring(num4, num3 - num4);
	//		num4 = num3 + length;
	//		num3 = _text.IndexOf(_searchStr, num4);
	//		num5++;
	//	}
	//	array[num5] = _text.Substring(num4, _text.Length - num4);
	//	return array;
	//}

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
