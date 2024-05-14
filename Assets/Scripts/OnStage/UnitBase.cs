using System.Collections.Generic;
using UnityEngine;

public class UnitBase : MonoBehaviour
{
    public UnitData unitData { get; set; }
    private StageManager stageManager;

    //State
    public bool IsTower => unitData.isTower;
    public bool isPlayer;
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
            foreach (var buff in skill)
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
            float buffedStat = unitData.initAttackDamage;
            float persentage = 0f;
            foreach (var buff in skill)
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
            float buffedStat = unitData.initAttackSpeed;
            float persentage = 0f;
            foreach (var buff in skill)
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
            float buffedStat = unitData.initAttackRange;
            float persentage = 0f;
            foreach (var buff in skill)
            {
                buffedStat += buff.Value.attackRange;
                persentage += buff.Value.attackRange_P;
            }
            return buffedStat * (1f + persentage);
        }
    }
    public List<int> AttackEnemyOrder => unitData.initAttackEnemyOrder;
    public int AttackOrder => unitData.initAttackOrder;
    public int AttackEnemyCount => unitData.initAttackEnemyCount;
    public COMBAT_TYPE CombatType => unitData.combatType;

    //etc
    public float MoveSpeed
    {
        get
        {
            float buffedStat = unitData.initMoveSpeed;
            float persentage = 0f;
            foreach (var buff in skill)
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
            float buffedStat = unitData.initDropGold;
            float persentage = 0f;
            foreach (var buff in skill)
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
            float buffedStat = unitData.initDropExp;
            float persentage = 0f;
            foreach (var buff in skill)
            {
                buffedStat += buff.Value.dropExp;
                persentage += buff.Value.dropExp_P;
            }
            return buffedStat * (1f + persentage);
        }
    }
    public bool IsHealer => unitData.division == DIVISION.HEALER;
    public int Heal
    {
        get
        {
            return unitData.initHeal;
        }
    }



    //Buff
    public Dictionary<string, SkillData> skill { get; private set; } = new();
    public void ApplyBuff(SkillData buff)
    {
        if (!skill.ContainsKey(buff.id))
        {
            skill.Add(buff.id, buff);
        }

        if (skill[buff.id].Apply())
            OnApplySkill(buff);
    }
    public void OnApplySkill(SkillData buff)
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

        foreach (var buff in skill)
        {
            if (buff.Value.UpdateDuration(deltaTime) == 0)
                removes.Add(buff.Key);
        }

        if (removes.Count == 0)
            return;
        foreach (var key in removes)
        {
            skill.Remove(key);
        }
        if (HP > MaxHP)
            HP = MaxHP;
    }


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
        hp = unitData.useStartHP ? unitData.initHPStart : MaxHP;
        skill.Clear();
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
}
