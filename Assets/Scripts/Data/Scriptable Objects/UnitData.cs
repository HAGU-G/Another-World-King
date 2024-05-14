using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using System.Collections.Generic;
using UnityEngine;
public enum COMBAT_TYPE
{
    STOP_ON_HAVE_TARGET,
    STOP_ON_ATTACK
}
public enum DIVISION
{
    NONE,
    MELEE,
    TANKER,
    MARKSMAN,
    HEALER,
    MAGIC,
    SPECIAL,
}


[CreateAssetMenu(fileName = "캐릭터 ID", menuName = "데이터 생성/캐릭터 정보")]
public class UnitData : ScriptableObject
{
    public string ignore; //Don't use in game

    //Init Stats
    public string id;
    public DIVISION division;
    public string prefab = Strings.nonePrefab;

    public bool isTower = false;

    public int initHP;
    public bool useStartHP = false;
    public int initHPStart;

    public COMBAT_TYPE combatType;
    public int initAttackDamage;
    public float initAttackSpeed;
    public float initAttackRange;
    public int initAttackEnemyCount;
    public int initAttackOrder;
    public List<int> initAttackEnemyOrder = new() { 1 };

    public int initHeal = 0;
    public float initMoveSpeed = 80;

    public int cost;
    public float spawnTime;
    public int initDropGold;
    public int initDropExp;

    public string effect;
    public string image;
    public string skill;
    public string desc;
    public int price;
}

