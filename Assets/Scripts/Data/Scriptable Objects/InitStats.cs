using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(fileName = "UnitStats_", menuName = "CreateData/UnitStats")]
public class InitStats : ScriptableObject
{
    //Init Stats
    public int id;
    public string prefab;
    public bool isTower;

    public int initHP = 30;
    public bool useStartHP = false;
    public int initHPStart = 30;

    public COMBAT_TYPE combatType;
    public int initAttackDamage = 10;
    public float initAttackSpeed = 10;
    public float initAttackRange = 10;
    public int initAttackEnemyCount = 1;
    public int initAttackOrder = 1;
    public List<int> initAttackEnemyOrder = new() { 1 };

    public bool isHealer = false;
    public int initHeal = 0;

    public float initMoveSpeed = 10;

    public int cost = 200;
    public int initDropGold = 50;
    public int initDropExp = 100;
}

