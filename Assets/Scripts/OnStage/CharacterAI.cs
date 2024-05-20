using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;



[RequireComponent(typeof(Rigidbody2D))]
public class CharacterAI : UnitBase
{
    public enum UNIT_STATE
    {
        IDLE,
        MOVE,
        ATTACK,
        ATTACK_END,
        DEAD,
        CANT_ACT,
        KNOCKBACK
    }

    public Animator[] Animators { get; private set; }
    private SpriteRenderer[] spriteRenderers;
    public Rigidbody2D rb;
    public BoxCollider2D attackCollider;

    public TowerAI Tower { get; set; }
    private List<UnitBase> enemyInRange = new();
    private List<UnitBase> targets = new();
    private List<UnitBase> blocks = new();

    private float lastAttackTime;
    private float alphaReduceTime;
    private bool isCounterBuffed;
    private bool isSelfDestruct;

    public UNIT_STATE UnitState { get; private set; }

    protected virtual void Awake()
    {
        OnDead += () => { Dead(); };
#if UNITY_EDITOR
        var d = Instantiate(Resources.Load(Paths.resourcesDebugStat), transform);
        OnDead += () => { Destroy(d); };
#endif
    }

    protected override void Update()
    {
        base.Update();
        if (IsDead)
        {
            if (spriteRenderers != null && spriteRenderers.Length > 0 && Time.time >= alphaReduceTime + 1f / 3f)
            {
                alphaReduceTime = Time.time;
                foreach (var spriteRenderer in spriteRenderers)
                {
                    spriteRenderer.color = new(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, spriteRenderer.color.a - 1f / 9f);
                }

            }
            return;
        }
        if (UnitState == UNIT_STATE.KNOCKBACK)
        {
            if (Time.time < alphaReduceTime + 1f / 10f)
                return;
            alphaReduceTime = Time.time;

            if (Mathf.Sign(rb.velocity.x) == (isPlayer ? -1f : 1f))
            {
                rb.velocity += Vector2.left * transform.localScale.x;
            }
            else
            {
                Idle();
            }
            return;
        }
        foreach (var buff in Buff)
        {
            if (buff.Value.skillData.sturn)
            {
                CantAct();
                return;
            }
        }

        if (GetOrder() > AttackOrder)
        {
            Move();
            return;
        }

        TargetFiltering();
        if (targets.Count == 0)
        {
            Move();
            return;
        }

        //ÀÚÆø
        if (isSelfDestruct)
        {
            if (blocks.Find(x => x.IsTower) != null)
                Attack();
            else
                Move();
            return;
        }

        //°ø°Ý
        if (Time.time >= lastAttackTime + AttackSpeed)
            Attack();
        else if (blocks.Count > 0)
            Idle();
        else
            Move();

    }

    public void SetAnimation(string trigger)
    {
        if (Animators == null || Animators.Length == 0)
            return;

        if (trigger == AnimatorTriggers.attack)
        {
            foreach (var animator in Animators)
            {
                float speed = 1f;
                float clipLength = animator.runtimeAnimatorController.animationClips[4].length;
                if (clipLength > AttackSpeed)
                    speed = clipLength / AttackSpeed;

                animator.speed = speed * unitData.initAttackSpeed / AttackSpeed;
                animator.ResetTrigger(AnimatorTriggers.move);
                animator.ResetTrigger(AnimatorTriggers.idle);
                animator.ResetTrigger(AnimatorTriggers.cantAct);
                animator.SetTrigger(trigger);
            }
        }
        else
        {
            foreach (var animator in Animators)
            {
                animator.speed = 1f;
                switch (trigger)
                {
                    case string s when s == AnimatorTriggers.move:
                        animator.ResetTrigger(AnimatorTriggers.idle);
                        animator.ResetTrigger(AnimatorTriggers.cantAct);
                        break;
                    case string s when s == AnimatorTriggers.idle:
                        animator.ResetTrigger(AnimatorTriggers.move);
                        animator.ResetTrigger(AnimatorTriggers.cantAct);
                        break;
                    case string s when s == AnimatorTriggers.cantAct:
                        animator.ResetTrigger(AnimatorTriggers.move);
                        animator.ResetTrigger(AnimatorTriggers.idle);
                        break;
                }
                animator.SetTrigger(trigger);
            }
        }


    }

    public void Idle()
    {
        if (UnitState == UNIT_STATE.IDLE || UnitState == UNIT_STATE.ATTACK)
            return;
        UnitState = UNIT_STATE.IDLE;
        SetAnimation(AnimatorTriggers.idle);
        Stop();
    }

    public void Stop()
    {
        rb.velocity = Vector3.zero;
    }

    public void Move()
    {
        if (UnitState == UNIT_STATE.MOVE || UnitState == UNIT_STATE.ATTACK)
            return;
        if (blocks.Count > 0)
        {
            Idle();
            return;
        }

        UnitState = UNIT_STATE.MOVE;
        SetAnimation(AnimatorTriggers.move);
        rb.velocity = transform.forward * MoveSpeed * -transform.localScale.x;
    }

    public void Dead()
    {
        if (UnitState == UNIT_STATE.DEAD)
            return;
        UnitState = UNIT_STATE.DEAD;
        SetAnimation(AnimatorTriggers.dead);
        foreach (var c in GetComponents<Collider2D>())
            c.enabled = false;
        Stop();
        Destroy(gameObject, 3f);
    }

    public void CantAct()
    {
        UnitState = UNIT_STATE.CANT_ACT;
        SetAnimation(AnimatorTriggers.cantAct);
        Stop();
        if (CounterSkill != null)
        {
            ClearBuff(CounterSkill);
            isCounterBuffed = false;
        }
    }

    public void SetUnitData(CharacterInfos characterInfos)
    {
        Instantiate(characterInfos.dress, transform);
        unitData = characterInfos.unitData;
        SetSkill(characterInfos.skillData);
        SetCounterSkill(characterInfos.counterSkillData);

        upgrade = characterInfos.upgrade;
        upgradeDamage = characterInfos.upgradeDamage;
        upgradeHP = characterInfos.upgradeHP;
    }

    public override void ResetUnit()
    {
        base.ResetUnit();

        foreach (var c in GetComponents<Collider2D>())
            c.enabled = true;
        attackCollider.size = new Vector2(0.3f + (AttackRange <= 1f ? 0.3f : AttackRange * 0.6f), 0.1f);
        attackCollider.offset = new Vector2(-attackCollider.size.x * 0.5f, isPlayer ? 0.2f : 0.6f);

        lastAttackTime = Time.time - AttackSpeed;
        if (isPlayer)
            transform.localScale = Vectors.filpX;
        else
            transform.localScale = Vector3.one;

        enemyInRange.Clear();
        targets.Clear();

        blocks.Clear();
        Animators = GetComponentsInChildren<Animator>();
        if (Animators != null && Animators.Length > 0)
        {
            var eventListener = Animators[0].AddComponent<CharacterEvenListener>();
            eventListener.onAttackHit += AttackHit;
            eventListener.onAttackEnd += AttackEnd;
        }

        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        Idle();
    }

    public void SetTower(TowerAI tower)
    {
        Tower = tower;
        isPlayer = Tower.isPlayer;
        attackCollider.offset = new Vector2(-attackCollider.size.x * 0.5f, isPlayer ? 0.2f : 0.6f);
        if (isPlayer)
            transform.localScale = Vectors.filpX;
        else
            transform.localScale = Vector3.one;
    }

    public int GetOrder() => Tower.units.IndexOf(this) + 1;

    protected virtual void TargetFiltering()
    {
        targets.Clear();

        int count = 0;
        foreach (var attackEnemyOrder in AttackEnemyOrder)
        {
            if (Tower.enemyTower.units.Count >= attackEnemyOrder)
            {
                count++;
                if (!targets.Contains(Tower.enemyTower.units[attackEnemyOrder - 1])
                    && Tower.enemyTower.units[attackEnemyOrder - 1].GetOrder() == attackEnemyOrder
                    && enemyInRange.Contains(Tower.enemyTower.units[attackEnemyOrder - 1]))
                    targets.Add(Tower.enemyTower.units[attackEnemyOrder - 1]);
            }
            if (count >= AttackEnemyCount)
                break;
        }

        if (enemyInRange.Contains(Tower.enemyTower))
            targets.Add(Tower.enemyTower);

        isSelfDestruct = (targets.Count == 1 && targets[0].IsTower);
    }

    protected virtual void Attack()
    {
        if (UnitState == UNIT_STATE.ATTACK)
            return;

        UnitState = UNIT_STATE.ATTACK;
        lastAttackTime = Time.time;
        SetAnimation(AnimatorTriggers.attack);

        if (targets != null
            && CounterSkill != null
            && CounterSkill.target == SkillData.TARGET.ONESELF
            && targets.Find(x => x.unitData.division == CounterSkill.targetDivision) != null)
        {
            ApplyBuff(CounterSkill);
            isCounterBuffed = true;
        }
    }

    public void AttackHit()
    {
        bool towerAttacked = false;

        if (IsHealer)
        {
            if (GetOrder() == 1 && targets != null && targets.Count == 1 && targets[0].IsTower)
            {
                targets[0].Damaged(100);
                towerAttacked = true;
            }
            else
            {
                for (int i = 0; i < GetOrder() - 1; i++)
                {
                    Tower.units[i].Healed(Heal);
                    if (!Tower.units[i].IsDead && Skill != null && Skill.target == SkillData.TARGET.TEAM)
                        Tower.units[i].ApplyBuff(Skill);
                }
            }
        }
        else
        {
            int count = 0;
            bool unitAttacked = false;
            foreach (var target in targets)
            {
                if (unitAttacked && target.IsTower)
                    continue;

                if (target.IsTower)
                {
                    target.Damaged(100);
                    towerAttacked = true;
                }
                else
                {
                    if (isCounterBuffed && target.unitData.division != CounterSkill.targetDivision)
                        ClearBuff(CounterSkill);

                    target.Damaged(AttackDamage);
                    unitAttacked = true;

                    if (!target.IsDead && Skill != null && Skill.target == SkillData.TARGET.ENEMY)
                        target.ApplyBuff(Skill);
                    if (!target.IsDead
                        && CounterSkill != null
                        && CounterSkill.target == SkillData.TARGET.ENEMY
                        && target.unitData.division == CounterSkill.targetDivision)
                        target.ApplyBuff(CounterSkill);

                    if (isCounterBuffed && target.unitData.division != CounterSkill.targetDivision)
                        ApplyBuff(CounterSkill);
                }
                count++;
                if (count >= AttackEnemyCount)
                    break;
            }
        }

        if (Skill != null && Skill.target == SkillData.TARGET.ONESELF)
        {
            ApplyBuff(Skill);
            Healed(Heal);
        }

        if (towerAttacked)
            Damaged(MaxHP);
    }

    public void AttackEnd()
    {
        if (UnitState != UNIT_STATE.DEAD
            && UnitState != UNIT_STATE.CANT_ACT
            && UnitState != UNIT_STATE.KNOCKBACK)
        {
            UnitState = UNIT_STATE.ATTACK_END;
            Move();
        }
        if (CounterSkill != null)
        {
            ClearBuff(CounterSkill);
            isCounterBuffed = false;
        }
    }


    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        var target = collision.GetComponent<UnitBase>();
        if (target == null)
            return;

        if (isPlayer != target.isPlayer && !enemyInRange.Contains(target))
        {
            if (!target.IsTower && collision.isTrigger)
                return;
            enemyInRange.Add(target);
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        var target = collision.GetComponent<UnitBase>();
        if (target == null)
            return;

        if (isPlayer != target.isPlayer && !enemyInRange.Contains(target))
        {
            if (!target.IsTower && collision.isTrigger)
                return;
            enemyInRange.Add(target);
        }
    }

    protected virtual void OnTriggerExit2D(Collider2D collision)
    {
        var target = collision.GetComponent<UnitBase>();
        if (target == null)
            return;

        enemyInRange.Remove(target);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        var character = collision.gameObject.GetComponent<CharacterAI>();
        if (character == null)
            return;

        var sign = Mathf.Sign(character.transform.position.x - transform.position.x);
        if (sign == (isPlayer ? 1f : -1f) || character.GetOrder() <= GetOrder())
        {
            SetIsBlocked(true, character);
        }
        else if (UnitState == UNIT_STATE.KNOCKBACK
            && character.isPlayer == isPlayer
            && character.UnitState != UnitState)
        {
            Stop();
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        var character = collision.gameObject.GetComponent<CharacterAI>();
        if (character == null)
            return;

        var sign = Mathf.Sign(character.transform.position.x - transform.position.x);
        if (sign == (isPlayer ? 1f : -1f) && character.GetOrder() <= GetOrder())
        {
            SetIsBlocked(true, character);
        }
        else if (UnitState == UNIT_STATE.KNOCKBACK
            && character.isPlayer == isPlayer
            && character.UnitState != UnitState)
        {
            Stop();
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        var character = collision.gameObject.GetComponent<CharacterAI>();
        if (character == null)
            return;

        if (Mathf.Sign(character.transform.position.x - transform.position.x) == (isPlayer ? 1f : -1f) && character.GetOrder() <= GetOrder())
        {
            SetIsBlocked(false, character);
        }
    }

    public void Knockback()
    {
        UnitState = UNIT_STATE.KNOCKBACK;
        SetAnimation(AnimatorTriggers.cantAct);
        rb.velocity = Vector2.left * -transform.localScale.x * 10f;
        alphaReduceTime = Time.time;
        if (CounterSkill != null)
        {
            ClearBuff(CounterSkill);
            isCounterBuffed = false;
        }
    }

    public void SetIsBlocked(bool value, UnitBase unitBase)
    {
        if (value)
        {
            if (!blocks.Contains(unitBase))
                blocks.Add(unitBase);
            if (UnitState == UNIT_STATE.MOVE)
                Idle();
            else if (UnitState != UNIT_STATE.KNOCKBACK)
                Stop();
        }
        else
        {
            blocks.Remove(unitBase);
        }
    }
}
