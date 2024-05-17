
using CsvHelper.Configuration.Attributes;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class UnitData_Csv
{
    [Index(0)] public string List { get; set; }
    [Index(1)] public int ID { get; set; }
    [Index(2)] public DIVISION Division { get; set; }
    [Index(3)] public int Hp { get; set; }
    [Index(4)] public int Attack { get; set; }
    [Index(5)] public float A_Speed { get; set; }
    [Index(6)] public float A_Range { get; set; }
    [Index(7)] public int Chr_Position { get; set; }
    [Index(8)] public string Effect { get; set; }
    [Index(9)] public string Image { get; set; }
    [Index(10)] public string Skill { get; set; }
    [Index(11)] public string Char_ID { get; set; }
    [Ignore] public int Heal { get; set; }
    [Ignore] public int EnemyCount { get; set; }
    [Ignore] public string TypeCounter { get; set; }


    public void ToScriptable(bool isPlayer)
    {
        UnitData unitData;
        if (isPlayer)
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
        unitData.initHeal = Heal;
        unitData.prefab = Char_ID;
        unitData.typeCounter = TypeCounter; ;
        if (EnemyCount > 1)
        {
            unitData.initAttackEnemyCount = EnemyCount;
            unitData.initAttackEnemyOrder.Clear();
            for (int i = 0; i < EnemyCount; i++)
            {
                unitData.initAttackEnemyOrder.Add(i + 1);
            }
        }
    }
    protected virtual void SaveData(UnitData unitData)
    {
        List = unitData.ignore;
        ID = unitData.id;
        Division = unitData.division;
        Hp = unitData.initHP;
        Attack = unitData.initAttackDamage;
        A_Speed = unitData.initAttackSpeed;
        A_Range = unitData.initAttackRange;
        Skill = unitData.skill;
        Effect = unitData.effect;
        Image = unitData.image;
        Chr_Position = unitData.initAttackOrder;
        Heal = unitData.initHeal;
        EnemyCount = unitData.initAttackEnemyCount;
        Char_ID = unitData.prefab;
    }

    public UnitData_Csv() { }
    public UnitData_Csv(UnitData initStats)
    {
        SaveData(initStats);
    }
}

public class Player_Csv : UnitData_Csv
{
    [Index(12)] public int Spawn_Price { get; set; }
    [Index(13)] public float Spawn_Time { get; set; }
    [Index(14)] public string Char_Info { get; set; }
    [Index(15)] public int Shop_Value { get; set; }

    public Player_Csv() : base() { }
    public Player_Csv(UnitData initStats) : base(initStats) { }

    protected override void LoadData(UnitData unitData)
    {
        base.LoadData(unitData);
        unitData.cost = Spawn_Price;
        unitData.spawnTime = Spawn_Time;
        unitData.desc = Char_Info;
        unitData.price = Shop_Value;
    }

    protected override void SaveData(UnitData unitData)
    {
        base.SaveData(unitData);
        Spawn_Price = unitData.cost;
        Spawn_Time = unitData.spawnTime;
        Char_Info = unitData.desc;
        Shop_Value = unitData.price;
    }
}

public class Enemy_Csv : UnitData_Csv
{
    [Index(12)] public int Dead_Gold { get; set; }
    [Index(13)] public int Dead_Exp { get; set; }
    public Enemy_Csv() : base() { }
    public Enemy_Csv(UnitData unitData) : base(unitData) { }

    protected override void LoadData(UnitData unitData)
    {
        base.LoadData(unitData);
        unitData.initDropGold = Dead_Gold;
        unitData.initDropExp = Dead_Exp;
    }

    protected override void SaveData(UnitData unitData)
    {
        base.SaveData(unitData);
        Dead_Gold = unitData.initDropGold;
        Dead_Exp = unitData.initDropExp;
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

public class Skill_Csv
{
    public string List { get; set; }
    public string ID { get; set; }
    public TARGET Target { get; set; }
    public int Gold_Supply { get; set; }
    public int Exp_Supply { get; set; }
    public int Flag_Supply { get; set; }
    public int Hp_Healing { get; set; }
    public int Wide_Area_Range { get; set; }
    public float A_Speed_Decrease { get; set; }
    public float A_Speed_Increase { get; set; }
    public int A_Speed_Increase_Nesting { get; set; }
    public float Stun_Duration { get; set; }

    public void ToScriptable()
    {
        SkillData skillData;
        skillData = Resources.Load<SkillData>(string.Format(Paths.resourcesSkill, ID));
        bool create = false;
        if (skillData == null)
        {
            skillData = ScriptableObject.CreateInstance<SkillData>();
            create = true;
        }

        skillData.ignore = List;
        skillData.id = ID;
        skillData.target = Target;
        skillData.onApplyGold = Gold_Supply;
        skillData.onApplyExp = Exp_Supply;
        skillData.clearFlag = Flag_Supply;
        skillData.attackSpeed = 0 + A_Speed_Decrease;
        skillData.attackSpeed -= A_Speed_Increase;
        skillData.nesting = A_Speed_Increase_Nesting;
        skillData.duration = Stun_Duration;
        skillData.sturn = Stun_Duration > 0;
        if (skillData.sturn && skillData.nesting <= 0)
        {
            skillData.nesting = 1;
            skillData.doResetDurationOnApply = true;
        }
        if (Stun_Duration == 0 && A_Speed_Increase_Nesting > 0)
            skillData.infinityDuration = true;
        else
            skillData.infinityDuration = false;

        if (create)
        {
            AssetDatabase.CreateAsset(skillData,
            string.Concat(
                Paths.folderResources,
                string.Format(Paths.resourcesSkill, skillData.id),
                Paths._asset));
        }
    }
}

public class Counter_Csv
{
    public string List { get; set; }
    public string ID { get; set; }
    public DIVISION Target { get; set; }
    public float Attack_Increase { get; set; }
    public float A_Speed_Decrease { get; set; }
    public int Heal_Increase { get; set; }
    public float Stun_Increased { get; set; }

    public void ToScriptable()
    {
        SkillData skillData;
        skillData = Resources.Load<SkillData>(string.Format(Paths.resourcesSkill, ID));
        bool create = false;
        if (skillData == null)
        {
            skillData = ScriptableObject.CreateInstance<SkillData>();
            create = true;
        }

        switch (ID)
        {
            case "104":
                skillData.target = TARGET.TEAM;
                break;
            case "105":
                skillData.target = TARGET.ENEMY;
                break;
            default:
                skillData.target = TARGET.ONESELF;
                break;
        }

        skillData.ignore = List;
        skillData.id = ID;
        if (Attack_Increase > 0f)
            skillData.attackDamage_P = Attack_Increase - 1f;
        skillData.attackSpeed = 0 + A_Speed_Decrease;
        skillData.duration = Stun_Increased * 2f;
        skillData.sturn = skillData.duration > 0;
        skillData.heal = Heal_Increase;

        if (skillData.sturn && skillData.nesting <= 0)
        {
            skillData.nesting = 1;
            skillData.doResetDurationOnApply = true;
        }
        if (skillData.duration == 0 && skillData.nesting > 0)
            skillData.infinityDuration = true;
        else
            skillData.infinityDuration = false;

        if (create)
        {
            AssetDatabase.CreateAsset(skillData,
            string.Concat(
                Paths.folderResources,
                string.Format(Paths.resourcesSkill, skillData.id),
                Paths._asset));
        }
    }
}