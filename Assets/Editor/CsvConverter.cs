
using UnityEditor;
using UnityEngine;

public class InitStats_Csv
{
    public string List { get; set; }
    public string ID { get; set; }
    public int Division { get; set; }
    public int Hp { get; set; }
    public int Attack { get; set; }
    public float A_Speed { get; set; }
    public float A_Range { get; set; }
    public string Effect { get; set; }
    public int Spawn_Price { get; set; }
    public float Spawn_Time { get; set; }
    public string Image { get; set; }
    public string Skill { get; set; }

    public void ToScriptable()
    {
        var initStats = Resources.Load<InitStats>(string.Format(Paths.resourcesPlayer, ID));
        bool create = false;
        if (initStats == null)
        {
            initStats = ScriptableObject.CreateInstance<InitStats>();
            create = true;
        }

        initStats.ignore = List;
        initStats.id = ID;
        initStats.division = Division;
        initStats.initHP = Hp;
        initStats.initAttackDamage = Attack;
        initStats.initAttackSpeed = A_Speed;
        initStats.initAttackRange = A_Range;
        initStats.cost = Spawn_Price;
        initStats.skill = Skill;
        initStats.effect = Effect;
        initStats.image = Image;
        initStats.spawnTime = Spawn_Time;

        if (create)
        {
            AssetDatabase.CreateAsset(initStats,
                string.Concat(
                    Paths.resourcesRaw,
                    string.Format(Paths.resourcesPlayer, initStats.id),
                    Paths.asset));
        }
    }

    public InitStats_Csv() { }
    public InitStats_Csv(InitStats initStats)
    {
        List = initStats.ignore;
        ID = initStats.id;
        Division = initStats.division;
        Hp = initStats.initHP;
        Attack = initStats.initAttackDamage;
        A_Speed = initStats.initAttackSpeed;
        A_Range = initStats.initAttackRange;
        Spawn_Price = initStats.cost;
        Skill = initStats.skill;
        Effect = initStats.effect;
        Image = initStats.image;
        Spawn_Time = initStats.spawnTime;
    }
}
