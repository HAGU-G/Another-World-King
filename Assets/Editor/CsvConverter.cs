using CsvHelper.Configuration.Attributes;
using System.Diagnostics.PerformanceData;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class UnitData_Csv
{
    [Index(0)] public string List { get; set; }
    [Index(1)] public int ID { get; set; }
    [Index(2)] public int Division { get; set; }
    [Index(3)] public int Hp { get; set; }
    [Index(4)] public int Attack { get; set; }
    [Index(5)] public float A_Speed { get; set; }
    [Index(6)] public float A_Range { get; set; }
    [Index(7)] public int Chr_Position { get; set; }
    [Index(8)] public string My_Effect { get; set; }
    [Index(9)] public string Target_Effect { get; set; }
    [Index(10)] public string A_Effect_Image { get; set; }
    [Index(11)] public string Skill { get; set; }
    [Index(12)] public string String_ID { get; set; }
    [Ignore] public int Heal { get; set; }
    [Ignore] public int EnemyCount { get; set; }
    [Ignore] public string TypeCounter { get; set; }
    [Ignore] public float AttackRange { get; set; }


    public void ToScriptable(bool isPlayer)
    {
        UnitData unitData = ScriptableObject.CreateInstance<UnitData>();
        LoadData(unitData);
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
        unitData.division = (UnitData.DIVISION)Division;
        unitData.initHP = Hp;
        unitData.initAttackDamage = Attack;
        unitData.initAttackSpeed = A_Speed;
        unitData.initAttackStartRange = A_Range;
        unitData.skill = Skill;
        unitData.effectAttack = My_Effect;
        unitData.effectAttackHit = Target_Effect;
        unitData.projectile = A_Effect_Image;
        unitData.initAttackOrder = Chr_Position;
        unitData.initHeal = Heal;
        unitData.prefab = String_ID;
        unitData.typeCounter = TypeCounter;

        if (AttackRange != 0 && unitData.division != UnitData.DIVISION.HEALER)
            unitData.initAttackRange = AttackRange;
        else
            unitData.initAttackRange = A_Range;

        
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
        Division = (int)unitData.division;
        Hp = unitData.initHP;
        Attack = unitData.initAttackDamage;
        A_Speed = unitData.initAttackSpeed;
        A_Range = unitData.initAttackStartRange;
        Skill = unitData.skill;
        My_Effect = unitData.effectAttack;
        Target_Effect = unitData.effectAttackHit;
        A_Effect_Image = unitData.projectile;
        Chr_Position = unitData.initAttackOrder;
        Heal = unitData.initHeal;
        EnemyCount = unitData.initAttackEnemyCount;
        String_ID = unitData.prefab;
    }

    public UnitData_Csv() { }
    public UnitData_Csv(UnitData initStats)
    {
        SaveData(initStats);
    }
}

public class Player_Csv : UnitData_Csv
{
    [Index(13)] public int Spawn_Price { get; set; }
    [Index(14)] public float Spawn_Time { get; set; }
    [Index(15)] public string Char_Info { get; set; }
    [Index(16)] public int Shop_Value { get; set; }
    [Index(17)] public int Upgrade_ID_AT { get; set; }
    [Index(18)] public int Upgrade_ID_DF { get; set; }

    public Player_Csv() : base() { }
    public Player_Csv(UnitData initStats) : base(initStats) { }

    protected override void LoadData(UnitData unitData)
    {
        base.LoadData(unitData);
        unitData.cost = Spawn_Price;
        unitData.spawnTime = Spawn_Time;
        unitData.desc = Char_Info;
        unitData.price = Shop_Value;
        unitData.upgradeDamageID = Upgrade_ID_AT;
        unitData.upgradeHPID = Upgrade_ID_DF;
    }

    protected override void SaveData(UnitData unitData)
    {
        base.SaveData(unitData);
        Spawn_Price = unitData.cost;
        Spawn_Time = unitData.spawnTime;
        Char_Info = unitData.desc;
        Shop_Value = unitData.price;
        Upgrade_ID_AT = unitData.upgradeDamageID;
        Upgrade_ID_DF = unitData.upgradeHPID;
    }
}

public class Enemy_Csv : UnitData_Csv
{
    [Index(13)] public int Dead_Gold { get; set; }
    [Index(14)] public int Dead_Exp { get; set; }
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
    public int Target { get; set; }
    public int Gold_Supply { get; set; }
    public int Exp_Supply { get; set; }
    public int Flag_Supply { get; set; }
    public int Hp_Healing { get; set; }
    public int Wide_Area_Range { get; set; }
    public float A_Speed_Decrease { get; set; }
    public float A_Speed_Increase { get; set; }
    public int A_Speed_Nesting { get; set; }
    public float Stun_Duration { get; set; }

    public void ToScriptable()
    {
        SkillData skillData = ScriptableObject.CreateInstance<SkillData>();

        skillData.ignore = List;
        skillData.id = ID;
        skillData.isCounterData = false;
        skillData.target = (SkillData.TARGET)Target;
        skillData.onApplyGold = Gold_Supply;
        skillData.onApplyExp = Exp_Supply;
        skillData.clearFlag = Flag_Supply;
        skillData.attackSpeed = 0 + A_Speed_Decrease;
        skillData.attackSpeed -= A_Speed_Increase;
        skillData.nesting = A_Speed_Nesting;
        skillData.duration = Stun_Duration;
        skillData.sturn = Stun_Duration > 0;
        if (skillData.sturn && skillData.nesting <= 0)
        {
            skillData.nesting = 1;
            skillData.doResetDurationOnApply = true;
        }
        if (Stun_Duration == 0 && A_Speed_Nesting > 0)
            skillData.infinityDuration = true;
        else
            skillData.infinityDuration = false;

        AssetDatabase.CreateAsset(skillData,
        string.Concat(
            Paths.folderResources,
            string.Format(Paths.resourcesSkill, skillData.id),
            Paths._asset));
    }

    public Skill_Csv() { }
    public Skill_Csv(SkillData skillData)
    {
        List = skillData.ignore;
        ID = skillData.id;
        Target = (int)skillData.target;
        Gold_Supply = skillData.onApplyGold;
        Exp_Supply = skillData.onApplyExp;
        Flag_Supply = skillData.clearFlag;
        if (skillData.attackSpeed > 0)
            A_Speed_Decrease = skillData.attackSpeed;
        if (skillData.attackSpeed < 0)
        {
            A_Speed_Increase = -skillData.attackSpeed;
            A_Speed_Nesting = skillData.nesting;
        }
        if (skillData.sturn)
            Stun_Duration = skillData.duration;


        foreach (var unitData in Resources.LoadAll<UnitData>(Paths.resourcesPlayer))
        {
            if (unitData.skill == ID)
            {
                Hp_Healing = unitData.initHeal;
                Wide_Area_Range = unitData.initAttackEnemyCount;
                break;
            }
        }
    }
}

public class Counter_Csv
{
    public string List { get; set; }
    public string ID { get; set; }
    public int Char_ID { get; set; }
    public int Division { get; set; }
    public int Target { get; set; }
    public float Attack_Increase { get; set; }
    public float Heal_Increase { get; set; }
    public float Stun_Increase { get; set; }
    public float A_Speed_Decrease { get; set; }

    public void ToScriptable()
    {
        SkillData skillData = ScriptableObject.CreateInstance<SkillData>();

        if (A_Speed_Decrease > 0f)
            skillData.target = SkillData.TARGET.ENEMY;
        else
            skillData.target = SkillData.TARGET.ONESELF;


        skillData.ignore = List;
        skillData.id = ID;
        skillData.applyCharID = Char_ID;
        skillData.applyDivision = (UnitData.DIVISION)Division;
        skillData.isCounterData = true;
        skillData.targetDivision = (UnitData.DIVISION)Target;
        if (Attack_Increase > 0f)
            skillData.attackDamage_P = Attack_Increase - 1f;
        skillData.attackSpeed = 0 + A_Speed_Decrease;
        skillData.skillDuration = Stun_Increase > 0f ? Stun_Increase : 0f;
        skillData.heal_P = Heal_Increase;
        skillData.nesting = 0;
        skillData.infinityDuration = true;

        AssetDatabase.CreateAsset(skillData,
        string.Concat(
            Paths.folderResources,
            string.Format(Paths.resourcesCounter, skillData.id),
            Paths._asset));
    }

    public Counter_Csv() { }
    public Counter_Csv(SkillData skillData)
    {
        List = skillData.ignore;
        ID = skillData.id;
        Target = (int)skillData.targetDivision;
        Attack_Increase = skillData.attackDamage_P + 1f;
        A_Speed_Decrease = skillData.attackSpeed;
        Heal_Increase = skillData.heal_P;
        Stun_Increase = skillData.skillDuration;
    }
}