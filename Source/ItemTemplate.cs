public class ItemTemplate
{
	public short id;

	public sbyte type;

	public sbyte gender;

	public string name;

	public string[] subName;

	public string description;

	public sbyte level;

	public short iconID;

	public short part;

	public bool isUpToUp;

	public int w;

	public int h;

	public int strRequire;

	public ItemTemplate(short templateID, sbyte type, sbyte gender, string name, string description, sbyte level, int strRequire, short iconID, short part, bool isUpToUp)
	{
//        File.AppendAllText("getByte.txt", string.Format("ItemTemplate {0}: templateID={1}, ={2}, ={3}, ={4}, ={5}, {6}, {7}, {8}, {9}\n", new object[]
//{
//            templateID,
//            type,
//            gender,
//            name,
//            description,
//            level,
//            strRequire,
//            iconID,
//            part,
//            isUpToUp
//		}));
        id = templateID;
		this.type = type;
		this.gender = gender;
		this.name = name;
		this.name = Res.changeString(this.name);
		this.description = description;
		this.description = Res.changeString(this.description);
		this.level = level;
		this.strRequire = strRequire;
		this.iconID = iconID;
		this.part = part;
		this.isUpToUp = isUpToUp;
	}
}
