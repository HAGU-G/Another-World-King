using System.Collections.Generic;
using UnityEngine;

public enum UNIT_STATE
{
    IDLE,
    MOVE,
    ATTACK,
    DEAD,
    CANT_ACT,
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
        OnDead += () => { SetUnitState(UNIT_STATE.DEAD); };
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
        foreach (var buff in Buff)
        {
            if (buff.Value.skillData.sturn)
            {
                SetUnitState(UNIT_STATE.CANT_ACT);
                return;
            }
        }

        if (GetOrder() > AttackOrder)
        {
            SetUnitState(UNIT_STATE.MOVE);
            return;
        }

        TargetFiltering();
        if (targets.Count <= 0)
        {
            SetUnitState(UNIT_STATE.MOVE);
            return;
        }

        if (Time.time >= lastAttackTime + AttackSpeed)
            SetUnitState(UNIT_STATE.ATTACK);
        else if (CombatType == COMBAT_TYPE.STOP_ON_HAVE_TARGET)
            SetUnitState(UNIT_STATE.IDLE);

    }

    public void SetAnimation(string trigger)
    {
        if (Animators == null || Animators.Length == 0)
            return;

        foreach (var animator in Animators)
        {
            animator.SetTrigger(trigger);
        }
    }

    public void SetUnitState(UNIT_STATE state)
    {
        if (unitState == state)
            return;
        else if (state == UNIT_STATE.MOVE && isBlocked)
            return;

        switch (state)
        {
            case UNIT_STATE.IDLE:
                SetAnimation(AnimatorTriggers.idle);
                rb.velocity = Vector3.zero;
                break;
            case UNIT_STATE.MOVE:
                SetAnimation(AnimatorTriggers.move);
                rb.velocity = transform.forward * MoveSpeed * -transform.localScale.x;
                break;
            case UNIT_STATE.ATTACK:
                SetAnimation(AnimatorTriggers.attack);
                lastAttackTime = Time.time;
                rb.velocity = Vector3.zero;
                AttackTargets();
                break;
            case UNIT_STATE.DEAD:
                SetAnimation(AnimatorTriggers.dead);
                foreach (var c in GetComponents<Collider2D>())
                    c.enabled = false;
                rb.velocity = Vector3.zero;
                Destroy(gameObject, 3f);
                break;
            case UNIT_STATE.CANT_ACT:
                SetAnimation(AnimatorTriggers.cantAct);
                rb.velocity = Vector3.zero;
                break;
            default:
                break;
        }

        unitState = state;
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
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        SetUnitState(UNIT_STATE.IDLE);
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

        if (Tower.enemyTower.units.Count == 0 && enemyInRange.Contains(Tower.enemyTower))
        {
            targets.Add(Tower.enemyTower);
            return;
        }

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
    }

    protected virtual void AttackTargets()
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
            foreach (var target in targets)
            {
                target.Damaged(AttackDamage);
                if (!target.IsDead && Skill != null && Skill.target == TARGET.ENEMY)
                    target.ApplyBuff(Skill);
            }
        }

        if (Skill != null && Skill.target == TARGET.ONESELF)
        {
            ApplyBuff(Skill);
            Healed(Heal);
        }

        if (CombatType == COMBAT_TYPE.STOP_ON_ATTACK)
            SetUnitState(UNIT_STATE.MOVE);
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
                SetUnitState(UNIT_STATE.IDLE);
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
}
