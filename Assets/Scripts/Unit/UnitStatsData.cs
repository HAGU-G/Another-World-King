using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitStats_", menuName = "CreateData/UnitStats")]
public class UnitStatsData : ScriptableObject
{
    //Init Stats
    public int id;

    public int initHP = 30;
    public bool useStartHP = false;
    public int initHPStart = 30;

    public int initAttackDamage = 10;
    public float initAttackSpeed = 10;
    public float initAttackRange = 10;

    public bool IsHealer = false;
    public int initHeal = 0;

    public float initMoveSpeed = 10;

    public int price = 200;
    public int initDropGold = 50;
    public int initDropExp = 100;
}

