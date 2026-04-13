using System.Text.Json;
using System.Text.Json.Serialization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace QLTK;

public class AccountSetting
{
    //temp
    private string _listItemOut = "";
    private string _listZoneChange = "";
    private string _listChardorge = "";
    private string _listMob = "";
    private string _listSKill = "";
    private string _listItem = "";
    private string _listItemthrow = "";
    private string _listItemuse = "";

    [JsonIgnore]
    public int[] ListItemIds { get; private set; } = Array.Empty<int>();
    public int[] ListZoneChange { get; private set; } = Array.Empty<int>();
    public string[] ListCharDorge { get; private set; } = Array.Empty<string>();
    public int[] ListMob { get; private set; } = Array.Empty<int>();
    public int[] ListSkill { get; private set; } = Array.Empty<int>();
    public int[] ListItem { get; private set; } = Array.Empty<int>();
    public int[] ListItemthow { get; private set; } = Array.Empty<int>();
    public int[] ListItemuse { get; private set; } = Array.Empty<int>();

    public enum TypeHPMob
    {
        All,
        Less,
        More,
    }
    public enum TypeMob
    {
        All,
        Index,
        Template_id,
    }
    public enum TypePickItem
    {
        NotPick,
        PickAll,
        PickIndex,
    }

    // Các thiết lập hẹn giờ
    public string TimeAutoOff { get; set; } = "";
    public string TimeAutoOn { get; set; } = "";
    public bool IsOutMaxSM { get; set; } = false;
    public long MaxSM { get; set; } = 0;
    public bool IsOutMaxTime { get; set; } = false;
    public int MaxTime { get; set; } = 0;
    public bool IsOutMaxItem { get; set; } = false;
    public int MaxItem { get; set; } = 0;
    public string ListItemOut
    {
        get => _listItemOut;
        set
        {
            _listItemOut = value;
            ListItemIds = GetListItemIds(value);
        }
    }
    // Các thiết lập train
    public bool IsGobackTime { get; set; } = false;
    public int GobackTime { get; set; } = 0;
    public bool IsGobackCondition { get; set; } = false;
    public int Map { get; set; } = 0;
    public string Zone
    {
        get => _listZoneChange;
        set
        {
            _listZoneChange = value;
            ListZoneChange = GetListItemIds(value);
        }
    }

    public bool IsChangeZoneMob { get; set; } = false;
    public bool IsChangeZoneChar { get; set; } = false;
    public string CharList
    {
        get => _listChardorge;
        set
        {
            _listChardorge = value;
            ListCharDorge = GetListItemStr(value);
        }
    }
    public bool IsUseGLT { get; set; } = false;
    public bool IsUseTDLT { get; set; } = false;
    public bool IsUseBT { get; set; } = false;
    // Các thiết lập chức năng khác
    public bool IsTrainBasicMob { get; set; } = false;
    public bool IsTrainSuperMob { get; set; } = false;
    public TypeHPMob IsTrainMobHP { get; set; } = TypeHPMob.All;
    public long TrainMobHP { get; set; } = 0;
    public TypeMob TrainMobType { get; set; } = TypeMob.All;
    public string ListMobTrain
    {
        get => _listMob;
        set
        {
            _listMob = value;
            ListMob = GetListItemIds(value);
        }
    }
    public string ListSkillTrain
    {
        get => _listSKill;
        set
        {
            _listSKill = value;
            ListSkill = GetListItemIds(value);
        }
    }
    public TypePickItem PickItemType { get; set; } = TypePickItem.NotPick;
    public string ListItemPick
    {
        get => _listItem;
        set
        {
            _listItem = value;
            ListItem = GetListItemIds(value);
        }
    }
    public bool IsClearBag { get; set; } = false;
    public string ListItemClear
    {
        get => _listItemthrow;
        set
        {
            _listItemthrow = value;
            ListItemthow = GetListItemIds(value);
        }
    }
    public bool IsByTDLT { get; set; } = false;
    public int TDLTTime { get; set; } = 0;
    public bool IsUseflag { get; set; } = false;
    public int Flag { get; set; } = -1;
    public bool IsUseItem { get; set; } = false;
    public string ListItemUse
    {
        get => _listItemuse;
        set
        {
            _listItemuse = value;
            ListItemuse = GetListItemIds(value);
        }
    }
    public bool IsAutoPoint { get; set; } = false;
    public int HPPoint { get; set; } = 0;
    public int MPPoint { get; set; } = 0;
    public int DamePoint { get; set; } = 0;
    public bool GetPeans { get; set; } = false;
    public bool PeansDonate { get; set; } = false;

    private int[] GetListItemIds(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return Array.Empty<int>();

        string[] parts = input.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        List<int> result = new List<int>();

        foreach (var part in parts)
        {
            if (int.TryParse(part, out int id)) result.Add(id);
        }
        return result.ToArray();
    }

    private string[] GetListItemStr(string input)
    {
        // 1. Kiểm tra an toàn đầu vào
        if (string.IsNullOrWhiteSpace(input))
            return Array.Empty<string>();
        // 2. Tách, xóa khoảng trắng và loại bỏ phần tử rỗng chỉ trong 1 dòng
        return input.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    }
}
[JsonSerializable(typeof(AccountSetting))]
internal partial class ClientJsonContext : JsonSerializerContext
{
}