using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Timeline;

public enum UNIT_STATE
{
    IDLE,
    MOVE,
    ATTACK,
    DEAD
}

[RequireComponent(typeof(Rigidbody2D))]
public class UnitAI : RuntimeStats
{
    public Animator animator;
    public Rigidbody2D rb;
    public BoxCollider2D attackCollider;

    public TowerAI Tower { get; set; }
    private List<RuntimeStats> enemyInRange = new();
    private List<RuntimeStats> targets = new();

    private float lastAttackTime;

    private UNIT_STATE unitState = UNIT_STATE.IDLE; //애니메이션으로 교체 예정
    private bool isBlocked;

    protected virtual void Awake()
    {
        OnDead += () => { SetUnitState(UNIT_STATE.DEAD); };
    }

    protected override void Update()
    {
        base.Update();

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

    public void SetUnitState(UNIT_STATE state)
    {
        if (unitState == state)
            return;
        else if (state == UNIT_STATE.MOVE && isBlocked)
            return;

        switch (state)
        {
            case UNIT_STATE.IDLE:
                rb.velocity = Vector3.zero;
                break;
            case UNIT_STATE.MOVE:
                rb.velocity = transform.forward * MoveSpeed * -transform.localScale.x;
                break;
            case UNIT_STATE.ATTACK:
                lastAttackTime = Time.time;
                rb.velocity = Vector3.zero;
                AttackTargets();
                break;
            case UNIT_STATE.DEAD:
                foreach (var c in GetComponents<Collider>())
                    c.enabled = false;
                Destroy(gameObject);
                break;
        }

        unitState = state;
    }

    public override void ResetUnit()
    {
        base.ResetUnit();

        foreach (var c in GetComponents<Collider>())
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
        if(IsHealer)
        {
            for(int i = 0; i < GetOrder();i++)
            {
                Tower.units[i].Healed(Heal);
            }
        }
        else
        {
            foreach (var target in targets)
            {
                target.Damaged(AttackDamage);
            }
        }
        

        if (CombatType == COMBAT_TYPE.STOP_ON_ATTACK)
            SetUnitState(UNIT_STATE.MOVE);
    }



    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        var target = collision.GetComponent<RuntimeStats>();
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
        var target = collision.GetComponent<RuntimeStats>();
        if (target == null)
            return;

        enemyInRange.Remove(target);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (isBlocked)
            return;

        var character = collision.gameObject.GetComponent<UnitAI>();
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

        var character = collision.gameObject.GetComponent<UnitAI>();
        if (character == null)
            return;

        if (Mathf.Sign(character.transform.position.x - transform.position.x) == (isPlayer ? 1f : -1f))
            isBlocked = false;
    }
}
