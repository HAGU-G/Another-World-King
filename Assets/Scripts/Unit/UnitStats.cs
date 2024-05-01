using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitName", menuName = "CreateData/UnitData")]
public class UnitStats : ScriptableObject
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
    public int dropGold = 50;
    public int dropExp = 100;

    //Buffed Stats (Use at Runtime)
    public int level = 1;
    public int MaxHP
    {
        get
        {
            float buffedStat = initHP;
            float persentage = 0f;
            foreach (var buff in buffs)
            {
                buffedStat += buff.Value.hp;
                persentage += buff.Value.hp_P;
            }
            return (int)Mathf.Ceil(buffedStat * (1f + persentage));
        }
    }
    private int hp;
    public int HP { get => hp; set => hp = Mathf.Clamp(value, 0, MaxHP); }

    public int AttackDamage
    {
        get
        {
            float buffedStat = initAttackDamage;
            float persentage = 0f;
            foreach (var buff in buffs)
            {
                buffedStat += buff.Value.attackDamage;
                persentage += buff.Value.attackDamage_P;
            }
            return (int)Mathf.Ceil(buffedStat * (1f + persentage));
        }
    }
    private float AttackSpeed
    {
        get
        {
            float buffedStat = initAttackSpeed;
            float persentage = 0f;
            foreach (var buff in buffs)
            {
                buffedStat += buff.Value.attackSpeed;
                persentage += buff.Value.attackSpeed_P;
            }
            return buffedStat * (1f + persentage);
        }
    }
    private float AttackRange
    {
        get
        {
            float buffedStat = initAttackRange;
            float persentage = 0f;
            foreach (var buff in buffs)
            {
                buffedStat += buff.Value.attackRange;
                persentage += buff.Value.attackRange_P;
            }
            return buffedStat * (1f + persentage);
        }
    }

    //Buff
    public Dictionary<int, Buff> buffs;

    public void ApplyBuff(Buff buff)
    {
        if (!buffs.ContainsKey(buff.id))
        {
            buffs.Add(buff.id, buff);
        }
        buffs[id].Apply();
    }

    public void UpdateBuffDuration(float deltaTime)
    {
        foreach (var buff in buffs)
        {
            if (buff.Value.UpdateDuration(deltaTime) == 0)
                buffs.Remove(buff.Key);
        }
    }

}

