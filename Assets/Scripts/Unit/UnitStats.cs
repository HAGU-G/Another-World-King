using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitStats_", menuName = "CreateData/UnitStats")]
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
    public int initDropGold = 50;
    public int initDropExp = 100;

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
            return Mathf.Clamp((int)Mathf.Ceil(buffedStat * (1f + persentage)), 1, int.MaxValue);
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
    public float AttackSpeed
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
    public float AttackRange
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
    public float MoveSpeed
    {
        get
        {
            float buffedStat = initMoveSpeed;
            float persentage = 0f;
            foreach (var buff in buffs)
            {
                buffedStat += buff.Value.moveSpeed;
                persentage += buff.Value.moveSpeed_P;
            }
            return buffedStat * (1f + persentage);
        }
    }
    public float DropGold
    {
        get
        {
            float buffedStat = initDropGold;
            float persentage = 0f;
            foreach (var buff in buffs)
            {
                buffedStat += buff.Value.dropGold;
                persentage += buff.Value.dropGold_P;
            }
            return buffedStat * (1f + persentage);
        }
    }
    public float DropExp
    {
        get
        {
            float buffedStat = initDropExp;
            float persentage = 0f;
            foreach (var buff in buffs)
            {
                buffedStat += buff.Value.dropExp;
                persentage += buff.Value.dropExp_P;
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
        List<int> removes = new();

        foreach (var buff in buffs)
        {
            if (buff.Value.UpdateDuration(deltaTime) == 0)
                removes.Add(buff.Key);
        }
        if (removes.Count == 0)
            return;
        foreach(var key in removes)
        {
            buffs.Remove(key);
        }
    }

}

