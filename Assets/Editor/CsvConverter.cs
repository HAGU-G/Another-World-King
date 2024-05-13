
using CsvHelper.Configuration.Attributes;
using UnityEditor;
using UnityEngine;

public class InitStats_Csv
{
    [Index(0)] public string List { get; set; }
    [Index(1)] public string ID { get; set; }
    [Index(2)] public DIVISION Division { get; set; }
    [Index(3)] public int Hp { get; set; }
    [Index(4)] public int Attack { get; set; }
    [Index(5)] public float A_Speed { get; set; }
    [Index(6)] public float A_Range { get; set; }
    [Index(7)] public int Chr_Position { get; set; }
    [Index(8)] public string Effect { get; set; }
    [Index(9)] public string Image { get; set; }
    [Index(10)] public string Skill { get; set; }


    public void ToScriptable(bool isPlayer)
    {
        UnitData unitData;
        if(isPlayer)
            unitData = Resources.Load<UnitData>(string.Format(Paths.resourcesPlayer, ID));
        else
            unitData = Resources.Load<UnitData>(string.Format(Paths.resourcesEnemy, ID));
        bool create = false;
        if (unitData == null)
        {
            unitData = ScriptableObject.CreateInstance<UnitData>();
            create = true;
        }

        LoadData(unitData);

        if (create)
            CreateData(unitData);
    }
    protected virtual void CreateData(UnitData unitData)
    {
        AssetDatabase.CreateAsset(unitData,
            string.Concat(
                Paths.folderResources,
                string.Format(Paths.resourcesPlayer, unitData.id),
                Paths._asset));
    }

    protected virtual void LoadData(UnitData unitData)
    {
        unitData.ignore = List;
        unitData.id = ID;
        unitData.division = Division;
        unitData.initHP = Hp;
        unitData.initAttackDamage = Attack;
        unitData.initAttackSpeed = A_Speed;
        unitData.initAttackRange = A_Range;
        unitData.skill = Skill;
        unitData.effect = Effect;
        unitData.image = Image;
        unitData.initAttackOrder = Chr_Position;
    }
    protected virtual void SaveData(UnitData initStats)
    {
        List = initStats.ignore;
        ID = initStats.id;
        Division = initStats.division;
        Hp = initStats.initHP;
        Attack = initStats.initAttackDamage;
        A_Speed = initStats.initAttackSpeed;
        A_Range = initStats.initAttackRange;
        Skill = initStats.skill;
        Effect = initStats.effect;
        Image = initStats.image;
        Chr_Position = initStats.initAttackOrder;
    }

    public InitStats_Csv() { }
    public InitStats_Csv(UnitData initStats)
    {
        SaveData(initStats);
    }
}

public class Player_Csv : InitStats_Csv
{
    [Index(11)] public int Spawn_Price { get; set; }
    [Index(12)] public float Spawn_Time { get; set; }
    [Index(13)] public string Char_ID { get; set; }
    [Index(14)] public string Char_Info { get; set; }
    public Player_Csv() : base() { }
    public Player_Csv(UnitData initStats) : base(initStats) { }

    protected override void LoadData(UnitData initStats)
    {
        base.LoadData(initStats);
        initStats.cost = Spawn_Price;
        initStats.spawnTime = Spawn_Time;
        initStats.prefab = Char_ID;
        initStats.desc = Char_Info;
    }

    protected override void SaveData(UnitData initStats)
    {
        base.SaveData(initStats);
        Spawn_Price = initStats.cost;
        Spawn_Time = initStats.spawnTime;
        Char_ID = initStats.prefab;
        Char_Info = initStats.desc;
    }
}

public class Enemy_Csv : InitStats_Csv
{

    [Index(11)] public int Dead_Gold { get; set; }
    [Index(12)] public int Dead_Exp { get; set; }
    public Enemy_Csv() : base() { }
    public Enemy_Csv(UnitData initStats) : base(initStats) { }

    protected override void LoadData(UnitData initStats)
    {
        base.LoadData(initStats);
        initStats.initDropGold = Dead_Gold;
        initStats.initDropExp = Dead_Exp;
    }

    protected override void SaveData(UnitData initStats)
    {
        base.SaveData(initStats);
        Dead_Gold = initStats.initDropGold;
        Dead_Exp = initStats.initDropExp;
    }

    protected override void CreateData(UnitData unitData)
    {
        AssetDatabase.CreateAsset(unitData,
            string.Concat(
                Paths.folderResources,
                string.Format(Paths.resourcesEnemy, unitData.id),
                Paths._asset));
    }
}