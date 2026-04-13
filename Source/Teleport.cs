public class Teleport
{
	public static MyVector vTeleport = new MyVector();

	public int x;

	public int y;

	public int headId;

	public int type;

	public bool isMe;

	public int y2;

	public int id;

	public int dir;

	public int planet;

	public static Image[] maybay = new Image[5];

	public static Image hole;

	public bool isUp;

	public bool isDown;

	private bool createShip;

	public bool paintFire;

	private bool painHead;

	private int tPrepare;

	private int vy = 1;

	private int tFire;

	private int tDelayHole;

	private bool tHole;

	private bool isShock;

	public Teleport(int x, int y, int headId, int dir, int type, bool isMe, int planet)
	{
		this.x = x;
		this.y = 5;
		y2 = y;
		this.headId = headId;
		this.type = type;
		this.isMe = isMe;
		this.dir = dir;
		this.planet = planet;
		tPrepare = 0;
		int num = 0;
		while (num < 100)
		{
			num++;
			y2 += 12;
			if (TileMap.tileTypeAt(x, y2, 2))
			{
				if (y2 % 24 != 0)
				{
					y2 -= y2 % 24;
				}
				break;
			}
		}
		isDown = true;
		isUp = false;
		if (this.planet > 2)
		{
			y2 += 4;
			if (maybay[3] == null)
			{
				maybay[3] = GameCanvas.loadImage("/mainImage/myTexture2dmaybay4a.png");
			}
			if (maybay[4] == null)
			{
				maybay[4] = GameCanvas.loadImage("/mainImage/myTexture2dmaybay4b.png");
			}
			if (hole == null)
			{
				hole = GameCanvas.loadImage("/mainImage/hole.png");
			}
		}
		else if (maybay[planet] == null)
		{
			maybay[planet] = GameCanvas.loadImage("/mainImage/myTexture2dmaybay" + (planet + 1) + ".png");
		}
		if (x > GameScr.cmx && x < GameScr.cmx + GameCanvas.w && y2 > 100 && !SoundMn.gI().isPlayAirShip() )
		{
			createShip = true;
			//SoundMn.gI().airShip();
		}
	}

	public static void addTeleport(Teleport p)
	{
		vTeleport.addElement(p);
	}

	public void paintHole(mGraphics g)
	{
		if (planet > 2 && tHole)
		{
			g.drawImage(hole, x, y2 + 20, StaticObj.BOTTOM_HCENTER);
		}
	}

	public void paint(mGraphics g)
	{
		if (Char.isLoadingMap || x < GameScr.cmx || x > GameScr.cmx + GameCanvas.w)
		{
			return;
		}
		Part part = GameScr.parts[headId];
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		if (planet == 0)
		{
			num = 15;
			num2 = 40;
			num3 = 5;
		}
		if (planet == 1)
		{
			num = 7;
			num2 = 55;
			num3 = 20;
		}
		if (planet == 2)
		{
			num = 18;
			num2 = 52;
			num3 = 10;
		}
		if (painHead && planet < 3)
		{
			SmallImage.drawSmallImage(g, part.pi[Char.CharInfo[0][0][0]].id, x + ((dir != 1) ? (-num) : num), y - num2, (dir != 1) ? 2 : 0, StaticObj.TOP_CENTER);
		}
		if (planet < 3)
		{
			g.drawRegion(maybay[planet], 0, 0, mGraphics.getImageWidth(maybay[planet]), mGraphics.getImageHeight(maybay[planet]), (dir == 1) ? 2 : 0, x, y, StaticObj.BOTTOM_HCENTER);
		}
		else if (isDown)
		{
			if (tPrepare > 10)
			{
				g.drawRegion(maybay[4], 0, 0, mGraphics.getImageWidth(maybay[4]), mGraphics.getImageHeight(maybay[4]), (dir == 1) ? 2 : 0, (dir != 1) ? (x + 11) : (x - 11), y + 2, StaticObj.BOTTOM_HCENTER);
			}
			else
			{
				g.drawRegion(maybay[3], 0, 0, mGraphics.getImageWidth(maybay[3]), mGraphics.getImageHeight(maybay[3]), (dir == 1) ? 2 : 0, x, y, StaticObj.BOTTOM_HCENTER);
			}
		}
		else if (tPrepare < 20)
		{
			g.drawRegion(maybay[4], 0, 0, mGraphics.getImageWidth(maybay[4]), mGraphics.getImageHeight(maybay[4]), (dir == 1) ? 2 : 0, (dir != 1) ? (x + 11) : (x - 11), y + 2, StaticObj.BOTTOM_HCENTER);
		}
		else
		{
			g.drawRegion(maybay[3], 0, 0, mGraphics.getImageWidth(maybay[3]), mGraphics.getImageHeight(maybay[3]), (dir == 1) ? 2 : 0, x, y, StaticObj.BOTTOM_HCENTER);
		}
	}

	public void update()
	{
        if (this.planet > 2 && this.paintFire && this.y != -80)
        {
            if (this.isDown && this.tPrepare == 0)
            {
                if (GameCanvas.gameTick % 3 == 0)
                {
                    ServerEffect.addServerEffect(1, this.x, this.y, 1, 0);
                }
            }
            else if (this.isUp && GameCanvas.gameTick % 3 == 0)
            {
                ServerEffect.addServerEffect(1, this.x, this.y + 16, 1, 1);
            }
        }
        this.tFire++;
        if (this.tFire > 3)
        {
            this.tFire = 0;
        }
        if (this.isDown)
        {
            if (!this.isShock)
            {
                ServerEffect.addServerEffect(92, this.x + 4, this.y + 14, 1, 0);
                GameScr.shock_scr = 10;
                this.isShock = true;
            }
            this.tPrepare += 300;
            if (this.tPrepare > 30)
            {
                this.isDown = false;
                this.isUp = true;
                this.paintFire = false;
            }
            if (this.type == 1)
            {
                if (this.isMe)
                {
                    global::Char.myCharz().isTeleport = false;
                }
                else if (GameScr.findCharInMap(this.id) != null)
                {
                    GameScr.findCharInMap(this.id).isTeleport = false;
                }
                this.painHead = false;
            }
        }
        else if (this.isUp)
        {
            if (this.isMe && this.type == 0)
            {
                Controller.isStopReadMessage = false;
                global::Char.ischangingMap = true;
            }
            if (!this.isMe && GameScr.findCharInMap(this.id) != null && this.type == 0)
            {
                GameScr.vCharInMap.removeElement(GameScr.findCharInMap(this.id));
            }
            if (this.planet < 3)
            {
                Teleport.vTeleport.removeElement(this);
            }
            else
            {
                this.y = -80;
                this.tDelayHole++;
                if (this.tDelayHole > 80)
                {
                    this.tDelayHole = 0;
                    Teleport.vTeleport.removeElement(this);
                }
            }
        }
        if (this.paintFire && this.planet < 3 && Res.abs(this.y - this.y2) <= 50 && GameCanvas.gameTick % 5 == 0)
        {
            EffecMn.addEff(new Effect(19, this.x, this.y2 + 20, 2, 1, -1));
        }
    }
}
