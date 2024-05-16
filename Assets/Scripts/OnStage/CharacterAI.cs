using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
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
[RequireComponent(typeof(Rigidbody2D))]
public class CharacterAI : UnitBase
{
    public Animator[] Animators { get; private set; }
    private SpriteRenderer[] spriteRenderers;
    public Rigidbody2D rb;
    public BoxCollider2D attackCollider;

    public TowerAI Tower { get; set; }
    private List<UnitBase> enemyInRange = new();
    private List<UnitBase> targets = new();

    private float lastAttackTime;
    private float alphaReduceTime;

    private UNIT_STATE unitState;
    private bool isBlocked;

    protected virtual void Awake()
    {
        OnDead += () => { Dead(); };
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
        if (unitState == UNIT_STATE.KNOCKBACK)
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
                rb.velocity = Vector2.zero;
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
        if (targets.Count <= 0)
        {
            Move();
            return;
        }

        if (Time.time >= lastAttackTime + AttackSpeed)
        {
            Attack();
        }
        else if (CombatType == COMBAT_TYPE.STOP_ON_HAVE_TARGET)
        {
            Idle();
        }

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
                animator.SetTrigger(trigger);
            }
        }
        else
        {
            foreach (var animator in Animators)
            {
                animator.speed = 1f;
                animator.SetTrigger(trigger);
            }
        }


    }

    public void Idle()
    {
        if (unitState == UNIT_STATE.IDLE || unitState == UNIT_STATE.ATTACK)
            return;
        unitState = UNIT_STATE.IDLE;
        SetAnimation(AnimatorTriggers.idle);
        Stop();
    }

    public void Stop()
    {
        rb.velocity = Vector3.zero;
    }

    public void Move()
    {
        if (unitState == UNIT_STATE.MOVE || unitState == UNIT_STATE.ATTACK)
            return;
        if (isBlocked)
        {
            Idle();
            return;
        }

        unitState = UNIT_STATE.MOVE;
        SetAnimation(AnimatorTriggers.move);
        rb.velocity = transform.forward * MoveSpeed * -transform.localScale.x;
    }

    public void Dead()
    {
        if (unitState == UNIT_STATE.DEAD)
            return;
        unitState = UNIT_STATE.DEAD;
        SetAnimation(AnimatorTriggers.dead);
        foreach (var c in GetComponents<Collider2D>())
            c.enabled = false;
        Stop();
        Destroy(gameObject, 3f);
    }

    public void CantAct()
    {
        unitState = UNIT_STATE.CANT_ACT;
        SetAnimation(AnimatorTriggers.cantAct);
        Stop();
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

        isBlocked = false;
        Animators = GetComponentsInChildren<Animator>();
        if (Animators != null && Animators.Length > 0)
        {
            var eventListener = Animators[0].AddComponent<CharacterEvenListener>();
            eventListener.onAttackHit += AttackEnd;
            eventListener.onAttackEnd += Idle;
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
        //TODO 오류 수정
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
    }

    protected virtual void Attack()
    {
        if (unitState == UNIT_STATE.ATTACK)
            return;

        unitState = UNIT_STATE.ATTACK;
        SetAnimation(AnimatorTriggers.attack);
        lastAttackTime = Time.time;
        Stop();
    }

    public void AttackEnd()
    {
        bool towerAttacked = false;

        if (targets.Count == 1 && targets[0].IsTower)
        {
            targets[0].Damaged(AttackDamage);
            if (!targets[0].IsDead && Skill != null && Skill.target == TARGET.ENEMY)
                targets[0].ApplyBuff(Skill);
            if (targets[0].IsTower)
                towerAttacked = true;
        }
        else
        {
            if (IsHealer)
            {
                for (int i = 0; i < GetOrder() - 1; i++)
                {
                    Tower.units[i].Healed(Heal);
                    if (!Tower.units[i].IsDead && Skill != null && Skill.target == TARGET.TEAM)
                        Tower.units[i].ApplyBuff(Skill);
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

                    target.Damaged(AttackDamage);
                    if (!target.IsDead && Skill != null && Skill.target == TARGET.ENEMY)
                        target.ApplyBuff(Skill);

                    if (!target.IsTower)
                        unitAttacked = true;
                    else
                        towerAttacked = true;

                    count++;
                    if (count >= AttackEnemyCount)
                        break;
                }
            }
        }

        if (Skill != null && Skill.target == TARGET.ONESELF)
        {
            ApplyBuff(Skill);
            Healed(Heal);
        }

        if (towerAttacked)
            Damaged(MaxHP);

        if (unitState != UNIT_STATE.CANT_ACT)
            unitState = UNIT_STATE.ATTACK_END;
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
        if (isBlocked)
            return;

        var character = collision.gameObject.GetComponent<CharacterAI>();
        if (character == null)
            return;

        if (Mathf.Sign(character.transform.position.x - transform.position.x) == (isPlayer ? 1f : -1f))
        {
            isBlocked = true;
            if (unitState == UNIT_STATE.MOVE)
                Idle();
            else
                Stop();
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (isBlocked)
            return;

        var character = collision.gameObject.GetComponent<CharacterAI>();
        if (character == null)
            return;

        if (Mathf.Sign(character.transform.position.x - transform.position.x) == (isPlayer ? 1f : -1f))
        {
            isBlocked = true;
            if (unitState == UNIT_STATE.MOVE)
                Idle();
            else
                Stop();
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (!isBlocked)
            return;

        var character = collision.gameObject.GetComponent<CharacterAI>();
        if (character == null)
            return;

        if (Mathf.Sign(character.transform.position.x - transform.position.x) == (isPlayer ? 1f : -1f))
            isBlocked = false;
    }

    public void Knockback()
    {
        unitState = UNIT_STATE.KNOCKBACK;
        SetAnimation(AnimatorTriggers.cantAct);
        Stop();
        rb.velocity = Vector2.left * -transform.localScale.x * 10f;
        alphaReduceTime = Time.time;
    }
}
