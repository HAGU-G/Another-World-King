using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]
public class UnitAI : RuntimeStats
{
    public Animator animator;
    public Rigidbody2D rb;
    public BoxCollider2D attackCollider;

    public List<RuntimeStats> towerUnits = new();
    private RuntimeStats target = null;
    private bool isFighting;

    private float lastAttackTime;

    protected virtual void Awake()
    {
        OnDead += () =>
        {
            foreach (var c in GetComponents<Collider>())
                c.enabled = false;

            //TESTCODE
            Destroy(gameObject);
        };
    }
    protected virtual void OnEnable()
    {
        ResetAI();
    }

    protected override void Update()
    {
        base.Update();
        if (isFighting)
        {
            rb.isKinematic = true;
            rb.velocity = Vector3.zero;
            if (target != null && Time.time >= lastAttackTime + 1f / AttackSpeed)
                TryAttack();
        }
        else
        {
            rb.isKinematic = false;
            rb.velocity = transform.forward * MoveSpeed * -transform.localScale.x;
        }
    }

    public void ResetAI()
    {
        ResetStats();
        foreach (var c in GetComponents<Collider>())
            c.enabled = true;
        attackCollider.size = new Vector2(0.3f + AttackRange, 0.1f);
        attackCollider.offset = new Vector2(-attackCollider.size.x * 0.5f, isPlayer ? 0.2f : 0.6f);

        lastAttackTime = Time.time - 1f / AttackSpeed;
        if (isPlayer)
            transform.localScale = new(-1f, 1f, 1f);
        else
            transform.localScale = Vector3.one;
    }

    public int GetOrder()
    {
        Debug.Log($"{towerUnits.IndexOf(this) + 1}");
        return towerUnits.IndexOf(this) + 1;
    }

    protected virtual bool SetTarget(RuntimeStats target)
    {
        if (isPlayer != target.isPlayer && GetOrder() <= AttackOrder)
        {
            var unit = target as UnitAI;
            if (unit != null && AttackEnemyOrder.Contains(unit.GetOrder()))
            {
                this.target = target;
                return true;
            }

            var tower = target as TowerAI;
            if (tower != null && GetOrder() <= AttackOrder)
            {
                this.target = target;
                return true;
            }
        }

        return false;
    }

    protected virtual void TryAttack()
    {
        lastAttackTime = Time.time;
        target.Damaged(AttackDamage);
        if (target == null || target.IsDead)
        {
            target = null;
            isFighting = false;
        }

    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        var stats = collision.GetComponent<RuntimeStats>();
        if (stats == null)
            return;

        SetTarget(stats);
        if (target != null)
            isFighting = true;
    }

    protected virtual void OnTriggerExit2D(Collider2D collision)
    {
        var stats = collision.GetComponent<RuntimeStats>();
        if (stats == null)
            return;

        if (target == stats)
        {
            isFighting = false;
            target = null;
        }
    }
}
