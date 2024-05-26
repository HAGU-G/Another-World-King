using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(fileName = "캐릭터 ID", menuName = "데이터 생성/캐릭터 정보")]
public class UnitData : ScriptableObject
{
    public enum DIVISION
    {
        NONE,
        MELEE,
        TANKER,
        ARCHER,
        HEALER,
        MAGIC,
        SPECIAL,
        CANNON,
        BOMBER
    }

    public string ignore; //Don't use in game

    //Init Stats
    public int id;
    public DIVISION division;
    public string prefab = Defines.nonePrefab;

    public int initHP;
    public bool useStartHP = false;
    public int initHPStart;

    public string typeCounter;
    public int initAttackDamage;
    public float initAttackSpeed;
    public float initAttackRange;
    public int initAttackEnemyCount;
    public int initAttackOrder;
    public List<int> initAttackEnemyOrder = new() { 1 };
    public string skill;

    public int initHeal = 0;
    public float initMoveSpeed = 80;

    public int price;
    public int cost;
    public float spawnTime;
    public int initDropGold;
    public int initDropExp;

    public string effectAttack;
    public string effectAttackHit;
    public string projectile;
    public string desc;

    public bool isTower = false;
    public int stars_3_CastleHp;
    public int stars_3_reward;
    public int stars_2_CastleHp;
    public int stars_2_reward;
    public int stars_1_reward;
    public int repeat_Reward;

    public int upgradeDamageID;
    public int upgradeHPID;
}

