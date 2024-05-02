using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitStats : MonoBehaviour
{
    public UnitStatsData data;

    //Buffed Stats (Use at Runtime)
    public int level { get; private set; } = 1;
    public int MaxHP
    {
        get
        {
            float buffedStat = data.initHP;
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
            float buffedStat = data.initAttackDamage;
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
            float buffedStat = data.initAttackSpeed;
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
            float buffedStat = data.initAttackRange;
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
            float buffedStat = data.initMoveSpeed;
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
            float buffedStat = data.initDropGold;
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
            float buffedStat = data.initDropExp;
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
    public Dictionary<int, BuffData> buffs = new();
    public void ApplyBuff(BuffData buff)
    {
        if (!buffs.ContainsKey(buff.id))
        {
            buffs.Add(buff.id, buff);
        }
        buffs[buff.id].Apply();
    }
    private void UpdateBuffDuration(float deltaTime)
    {
        List<int> removes = new();

        foreach (var buff in buffs)
        {
            if (buff.Value.UpdateDuration(deltaTime) == 0)
                removes.Add(buff.Key);
        }
        if (removes.Count == 0)
            return;
        foreach (var key in removes)
        {
            buffs.Remove(key);
        }
    }

    //Behaviour
    private void Awake()
    {
        ResetStats();
    }

    public void ResetStats()
    {
        hp = data.useStartHP ? data.initHPStart : MaxHP;
    }

    private void Update()
    {
        UpdateBuffDuration(Time.deltaTime);
    }
}
