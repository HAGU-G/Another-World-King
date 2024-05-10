using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "ĳ���� ID", menuName = "������ ����/ĳ���� ����")]
public class InitStats : ScriptableObject
{
    public string ignore; //Don't use in game

    //Init Stats
    public string id;
    public int division;
    public string prefab;
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

    public bool isHealer = false;
    public int initHeal = 0;

    public float initMoveSpeed = 80;

    public int cost;
    public float spawnTime;
    public int initDropGold;
    public int initDropExp;

    public string effect;
    public string image;
    public string skill;
}

