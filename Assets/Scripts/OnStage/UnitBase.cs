using System.Collections.Generic;
using UnityEngine;

public class UnitBase : MonoBehaviour
{
    public enum UPGRADE
    {
        NONE,
        DAMAGE,
        HP
    }

    public UnitData unitData { get; set; }
    private StageManager stageManager;

    //State
    public bool IsTower => unitData.isTower;
    public bool isPlayer;
    private bool isInvincibility;
    private bool isDead;
    public bool IsDead
    {
        get => isDead;
        private set
        {
            isDead = value;
            if (isDead && OnDead != null)
                OnDead();
        }
    }


    //Buffed Stats (Use at Runtime)
    public int Level { get; private set; } = 1;
    public int MaxHP
    {
        get
        {
            float buffedStat = unitData.initHP;
            float persentage = 0f;
            foreach (var buff in Buff)
            {
                buffedStat += buff.Value.skillData.hp * buff.Value.Count;
                persentage += buff.Value.skillData.hp_P * buff.Value.Count;
            }
            buffedStat += hpUpgradeValue;
            return Mathf.Clamp((int)Mathf.Ceil(buffedStat * (1f + persentage)), 1, int.MaxValue);
        }
    }
    private int hp;
    public int HP { get => hp; set => hp = Mathf.Clamp(value, isInvincibility ? 1 : 0, MaxHP); }
    public int AttackDamage
    {
        get
        {
            float buffedStat = unitData.initAttackDamage;
            float persentage = 0f;
            foreach (var buff in Buff)
            {
                buffedStat += buff.Value.skillData.attackDamage * buff.Value.Count;
                persentage += buff.Value.skillData.attackDamage_P * buff.Value.Count;
            }
            buffedStat += damageUpgradeValue;
            return (int)Mathf.Ceil(buffedStat * (1f + persentage));
        }
    }
    public float AttackSpeed
    {
        get
        {
            float buffedStat = unitData.initAttackSpeed;
            float persentage = 0f;
            foreach (var buff in Buff)
            {
                buffedStat += buff.Value.skillData.attackSpeed * buff.Value.Count;
                persentage += buff.Value.skillData.attackSpeed_P * buff.Value.Count;
            }
            var result = buffedStat * (1f + persentage);
            return result > 0f ? result : 0.001f;
        }
    }
    public float AttackRange
    {
        get
        {
            float buffedStat = unitData.initAttackRange;
            float persentage = 0f;
            foreach (var buff in Buff)
            {
                buffedStat += buff.Value.skillData.attackRange * buff.Value.Count;
                persentage += buff.Value.skillData.attackRange_P * buff.Value.Count;
            }
            return buffedStat * (1f + persentage);
        }
    }
    public List<int> AttackEnemyOrder => unitData.initAttackEnemyOrder;
    public int AttackOrder => unitData.initAttackOrder;
    public int AttackEnemyCount => unitData.initAttackEnemyCount;

    //etc
    public float MoveSpeed
    {
        get
        {
            float buffedStat = unitData.initMoveSpeed;
            float persentage = 0f;
            foreach (var buff in Buff)
            {
                buffedStat += buff.Value.skillData.moveSpeed * buff.Value.Count;
                persentage += buff.Value.skillData.moveSpeed_P * buff.Value.Count;
            }
            return buffedStat * (1f + persentage);
        }
    }
    public float DropGold
    {
        get
        {
            float buffedStat = unitData.initDropGold;
            float persentage = 0f;
            foreach (var buff in Buff)
            {
                buffedStat += buff.Value.skillData.dropGold * buff.Value.Count;
                persentage += buff.Value.skillData.dropGold_P * buff.Value.Count;
            }
            return buffedStat * (1f + persentage);
        }
    }
    public float DropExp
    {
        get
        {
            float buffedStat = unitData.initDropExp;
            float persentage = 0f;
            foreach (var buff in Buff)
            {
                buffedStat += buff.Value.skillData.dropExp * buff.Value.Count;
                persentage += buff.Value.skillData.dropExp_P * buff.Value.Count;
            }
            return buffedStat * (1f + persentage);
        }
    }
    public bool IsHealer => unitData.division == UnitData.DIVISION.HEALER;
    public int Heal
    {
        get
        {
            float buffedStat = unitData.initHeal;
            float persentage = 0f;
            foreach (var buff in Buff)
            {
                buffedStat += buff.Value.skillData.heal * buff.Value.Count;
                persentage += buff.Value.skillData.heal_P * buff.Value.Count;
            }
            return (int)Mathf.Ceil(buffedStat * (1f + persentage));
        }
    }



    //Buff
    public SkillData Skill { get; private set; } = null;
    public SkillData CounterSkill { get; private set; } = null;
    public Dictionary<string, SkillBase> Buff { get; private set; } = new();
    public void SetSkill(SkillData skill)
    {
        Skill = skill;
    }
    public void SetCounterSkill(SkillData skill)
    {
        CounterSkill = skill;
    }
    public void ApplyBuff(SkillData buff, int durationIncrease = 0)
    {
        if (!Buff.ContainsKey(buff.id))
        {
            var skillBase = new SkillBase(buff);
            skillBase.IncreaseDuration(durationIncrease);
            Buff.Add(buff.id, skillBase);
        }
        if (Buff[buff.id].Apply())
            OnApplyBuff(buff);
    }
    public void ClearBuff(SkillData buff)
    {
        if (Buff.ContainsKey(buff.id))
        {
            Buff.Remove(buff.id);
        }
    }
    public void OnApplyBuff(SkillData buff)
    {
        if (stageManager != null)
        {
            stageManager.GetGold(buff.onApplyGold);
            stageManager.GetExp(buff.onApplyExp);
        }
    }
    private void UpdateBuffDuration(float deltaTime)
    {
        List<string> removes = new();

        foreach (var buff in Buff)
        {
            if (buff.Value.UpdateDuration(deltaTime) == 0)
                removes.Add(buff.Key);
        }

        if (removes.Count == 0)
            return;

        foreach (var key in removes)
        {
            Buff.Remove(key);
        }
        if (HP > MaxHP)
            HP = MaxHP;
    }

    //Upgrade
    public int damageUpgradeValue;
    public int hpUpgradeValue;

    //Behaviour
    protected virtual void Start()
    {
        stageManager = GameObject.FindWithTag(Tags.player).GetComponent<StageManager>();
    }
    protected virtual void Update()
    {
        if (IsDead)
            return;
        UpdateBuffDuration(Time.deltaTime);
    }

    public virtual void ResetUnit()
    {
        Buff.Clear();
        hp = unitData.useStartHP ? unitData.initHPStart : MaxHP;
        IsDead = false;
    }


    //Combat & Evemt
    public event System.Action OnDead = null;
    public event System.Action OnDamaged = null;
    public void Damaged(int damage)
    {
        HP -= damage;
        if (OnDamaged != null)
            OnDamaged();

        if (!IsDead && HP <= 0)
            IsDead = true;
    }
    public void Healed(int heal)
    {
        if (!IsDead)
            HP += heal;
    }
    public void SetInvincibility(bool value)
    {
        isInvincibility = value;
    }
}
