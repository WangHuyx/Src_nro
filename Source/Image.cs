using SkiaSharp;
using System;
using System.Drawing;
using System.Threading;

public class Image : IDisposable
{
	private const int INTERVAL = 5;

	private const int MAXTIME = 500;

	//public Texture2D texture = new Texture2D(1, 1);

	public static Image imgTemp;

	public static string filenametemp;

	public static byte[] datatemp;

	public static Image imgSrcTemp;

	public static int xtemp;

	public static int ytemp;

	public static int wtemp;

	public static int htemp;

	public static int transformtemp;

	public int w;

	public int h;

	public static int status;

	public SKColor colorBlend = SKColors.Black;

    internal SKBitmap texture;

    public Image() { }

	public Image(SKBitmap bitmap)
	{
		this.texture = bitmap;
	}
    public void Dispose()
    {
        if (texture != null)
        {
            texture.Dispose(); // Giải phóng dữ liệu ảnh thực sự khỏi RAM
            texture = null;
        }
    }

    public static Image createEmptyImage()
	{
		return __createEmptyImage();
	}

	public static Image createImage(string filename)
	{
		return __createImage(filename);
	}

	public static Image createImage(byte[] imageData)
	{
		return __createImage(imageData);
	}

	public static Image createImage(Image src, int x, int y, int w, int h, int transform)
	{
		return __createImage(src, x, y, w, h, transform);
	}

	public static Image createImage(int w, int h)
	{
		return __createImage(w, h);
	}

	//public static Image createImage(Image img)
	//{
	//	Image image = createImage(img.w, img.h);
	//	image.texture = img.texture;
	//	image.texture.Apply();
	//	return image;
	//}

	public static Image createImage(sbyte[] imageData, int offset, int lenght)
	{
		if (offset + lenght > imageData.Length)
		{
			return null;
		}
		byte[] array = new byte[lenght];
		for (int i = 0; i < lenght; i++)
		{
			array[i] = convertSbyteToByte(imageData[i + offset]);
		}
		return createImage(array);
	}

	public static byte convertSbyteToByte(sbyte var)
	{
		if (var > 0)
		{
			return (byte)var;
		}
		return (byte)(var + 256);
	}

	public static byte[] convertArrSbyteToArrByte(sbyte[] var)
	{
		byte[] array = new byte[var.Length];
		for (int i = 0; i < var.Length; i++)
		{
			if (var[i] > 0)
			{
				array[i] = (byte)var[i];
			}
			else
			{
				array[i] = (byte)(var[i] + 256);
			}
		}
		return array;
	}

    public static Image createRGBImage(int[] rbg, int w, int h, bool bl)
	{
		Image image = createImage(w, h);
		SKColor[] array = new SKColor[rbg.Length];
		for (int i = 0; i < array.Length; i++)
		{
			ref SKColor reference = ref array[i];
			reference = setColorFromRBG(rbg[i]);
		}
        int index = 0;
        for (int i = 0; i < 0 + h; i++)
        {
            for (int j = 0; j < 0 + w; j++)
            {
                image.texture.SetPixel(j, i, array[index++]);
            }
        }
        return image;
	}

	public static SKColor setColorFromRBG(int rgb)
	{
        // Lấy trực tiếp các kênh màu (0 - 255) và ép kiểu sang byte
        byte b = (byte)(rgb & 0xFF);
        byte g = (byte)((rgb >> 8) & 0xFF);
        byte r = (byte)((rgb >> 16) & 0xFF);

        // Khởi tạo SKColor với thứ tự (Red, Green, Blue)
        return new SKColor(r, g, b);
    }

	public static void update()
	{
		if (status == 2)
		{
			status = 1;
			imgTemp = __createEmptyImage();
			status = 0;
		}
		else if (status == 3)
		{
			status = 1;
			imgTemp = __createImage(filenametemp);
			status = 0;
		}
		else if (status == 4)
		{
			status = 1;
			imgTemp = __createImage(datatemp);
			status = 0;
		}
		else if (status == 5)
		{
			status = 1;
			imgTemp = __createImage(imgSrcTemp, xtemp, ytemp, wtemp, htemp, transformtemp);
			status = 0;
		}
		else if (status == 6)
		{
			status = 1;
			imgTemp = __createImage(wtemp, htemp);
			status = 0;
		}
	}

	private static Image __createImage(string filename)
	{
        // 1. Xác định đường dẫn thực tế (Giả sử bạn để ảnh trong thư mục Resources của dự án)
        // Lưu ý: Thêm ".png" nếu filename cũ không bao gồm phần mở rộng ,"res",$"x{mGraphics.zoomLevel}"
        string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", filename);

        if (!File.Exists(fullPath))
        {
            throw new Exception($"[NULL POINTER EXCEPTION] File not found at: {fullPath}");
        }

        // 2. Sử dụng FileStream để nạp ảnh (Tối ưu nhất cho I/O)
        using (var stream = File.OpenRead(fullPath))
        {
            SKBitmap bitmap = SKBitmap.Decode(stream);

            if (bitmap == null)
            {
                throw new Exception("FAILED TO DECODE IMAGE AT: " + filename);
            }

            // 3. Khởi tạo đối tượng Image mới với SKBitmap
            // Sử dụng cấu tử (Constructor) nhận SKBitmap mà chúng ta đã thảo luận
            Image image = new Image(bitmap);
            image.w = bitmap.Width;
            image.h = bitmap.Height;

            // Lưu ý: setTextureQuality(image) trong Skia thường không cần thiết 
            // vì chất lượng vẽ được quyết định bởi SKPaint khi Draw.

            return image;
        }
        //Image image = new Image();
        //Texture2D texture2D = Resources.Load(filename) as Texture2D;
        //if (texture2D == null)
        //{
        //	throw new Exception("NULL POINTER EXCEPTION AT Image __createImage " + filename);
        //}
        //image.texture = texture2D;
        //image.w = image.texture.Width;
        //image.h = image.texture.Height;
        //setTextureQuality(image);
        //return image;
    }

	private static Image __createImage(byte[] imageData)
	{
        if (imageData == null || imageData.Length == 0)
        {
            return null;
        }

        try
        {
            // 1. Decode mảng byte trực tiếp thành SKBitmap
            // SkiaSharp tự động nhận diện định dạng (PNG, JPG, v.v.)
            SKBitmap bitmap = SKBitmap.Decode(imageData);

            if (bitmap == null)
            {
                Cout.LogError("DECODE IMAGE FROM ARRAY FAIL");
                return null;
            }

            // 2. Khởi tạo Image thông qua Constructor đã nạp chồng (Overloaded)
            Image image = new Image(bitmap);

            // 3. Cập nhật kích thước logic cho Game
            image.w = bitmap.Width;
            image.h = bitmap.Height;

            // Lưu ý: setTextureQuality(image) thường không cần thiết trong Skia
            // vì chất lượng được quyết định bởi SKPaint khi vẽ.

            return image;
        }
        catch (Exception ex)
        {
            Cout.LogError("CREATE IMAGE FROM ARRAY FAIL \n" + ex.Message + "\n" + Environment.StackTrace);
            return null;
        }
        //if (imageData == null || imageData.Length == 0)
        //{
        //	Cout.LogError("Create Image from byte array fail");
        //	return null;
        //}
        //Image image = new Image();
        //try
        //{
        //	image.texture.LoadImage(imageData);
        //	image.w = image.texture.Width;
        //	image.h = image.texture.Height;
        //	setTextureQuality(image);
        //}
        //catch (Exception)
        //{
        //	Cout.LogError("CREAT IMAGE FROM ARRAY FAIL \n" + Environment.StackTrace);
        //}
        //return image;
    }

	private static Image __createImage(Image src, int x, int y, int w, int h, int transform)
	{
		Image image = new Image();
        image.texture = new SKBitmap(w, h);
		y = src.texture.Height - y - h;
		for (int i = 0; i < w; i++)
		{
			for (int j = 0; j < h; j++)
			{
				int num = i;
				if (transform == 2)
				{
					num = w - i;
				}
				int num2 = j;
				image.texture.SetPixel(i, j, src.texture.GetPixel(x + num, y + num2));
			}
		}
		//image.texture.Apply();
		image.w = image.texture.Width;
		image.h = image.texture.Height;
		//setTextureQuality(image);
		return image;
	}

	private static Image __createEmptyImage()
	{
		return new Image();
	}

	public static Image __createImage(int w, int h)
	{
		Image image = new Image();
		image.texture = new SKBitmap(w, h,SKColorType.Rgba8888, SKAlphaType.Premul);
        //setTextureQuality(image);
		image.w = w;
		image.h = h;
		//image.texture.Apply();
		return image;
	}

	public static int getImageWidth(Image image)
	{
		return image.getWidth();
	}

	public static int getImageHeight(Image image)
	{
		return image.getHeight();
	}

	public int getWidth()
	{
		return w / mGraphics.zoomLevel;
	}

	public int getHeight()
	{
		return h / mGraphics.zoomLevel;
	}

	private static void setTextureQuality(Image img)
	{
		setTextureQuality(img.texture);
	}

	public static void setTextureQuality(SKBitmap paint)
	{
        // 1. Tương đương FilterMode.Point (Cho game pixel, không bị nhòe khi phóng to)
        //paint.FilterQuality = SKFilterQuality.None;

        // 2. Tương đương anisoLevel = 0 và mipMapBias = 0
        // Skia quản lý việc này qua cơ chế Sampling, FilterQuality.None đã tắt các hiệu ứng này
        //paint.IsAntialias = false;

        // 3. Tương đương TextureWrapMode.Clamp
        // Trong Skia, WrapMode được định nghĩa khi tạo Shader từ Bitmap.
        // Thông thường, lệnh DrawBitmap mặc định đã là Clamp.
        //texture.anisoLevel = 0;
        //texture.filterMode = FilterMode.Point;
        //texture.mipMapBias = 0f;
        //texture.wrapMode = TextureWrapMode.Clamp;
    }

 //   public SKColor[] getColor()
	//{
	//	return texture.GetPixel();
	//}

	public int getRealImageWidth()
	{
		return w;
	}

	public int getRealImageHeight()
	{
		return h;
	}

	public void getRGB(ref int[] data, int x1, int x2, int x, int y, int w, int h)
	{
        if (texture == null) return;

        int index = 0;
        // SkiaSharp sử dụng Top-Left (0,0 ở góc trên bên trái)
        // Nên chúng ta KHÔNG cần (this.h - 1 - y) như Unity nữa.

        for (int i = y; i < y + h; i++)
        {
            for (int j = x; j < x + w; j++)
            {
                // 1. Lấy màu tại tọa độ (j, i)
                SKColor pixelColor = texture.GetPixel(j, i);

                // 2. Chuyển đổi sang int bằng hàm chúng ta đã viết ở bước trước
                if (index < data.Length)
                {
                    data[index++] = mGraphics.getIntByColor(pixelColor);
                }
            }
        }
    }
}
