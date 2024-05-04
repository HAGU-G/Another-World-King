using System;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]
public class UnitAI : RuntimeStats
{
    public Animator animator;
    public Rigidbody2D rb;
    public BoxCollider2D attackCollider;

    public List<RuntimeStats> unitsOrder;
    private List<RuntimeStats> targetInRange = new();
    private List<RuntimeStats> targetCanAttack = new();

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

        if (GetOrder() > AttackOrder)
        {
            Move();
            return;
        }

        TargetFiltering();
        if (targetCanAttack.Count > 0)
        {
            if (CombatType == COMBAT_TYPE.STOP_ON_HAS_TARGET)
                Stop();

            if (Time.time >= lastAttackTime + 1f / AttackSpeed)
                AttackTargets();
        }
        else
        {
            Move();
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

        unitsOrder = null;
        targetInRange.Clear();

    }

    public int GetOrder() => unitsOrder.IndexOf(this) + 1;

    protected virtual void TargetFiltering()
    {
        targetCanAttack.Clear();

        foreach (var target in targetInRange)
        {
            if (!target.IsTower)
            {
                var unit = target as UnitAI;
                if (AttackEnemyOrder.Contains(unit.GetOrder()))
                    targetCanAttack.Add(target);
            }
            else
            {
                targetCanAttack.Add(target);
            }
        }
    }


    public void Stop()
    {
        rb.isKinematic = true;
        rb.velocity = Vector3.zero;
    }

    public void Move()
    {
        rb.isKinematic = false;
        rb.velocity = transform.forward * MoveSpeed * -transform.localScale.x;
    }

    protected virtual void AttackTargets()
    {
        lastAttackTime = Time.time;
        if (CombatType == COMBAT_TYPE.STOP_ON_ATTACK)
            Stop();

        if (targetCanAttack.Count > 1)
        {
            for (int i = 0, j = 0; i < AttackEnemyOrder.Count && j < AttackEnemyCount; i++)
            {
                var target = targetCanAttack.Find(x =>
                {
                    var unit = x as UnitAI;
                    if (unit != null)
                        return unit.GetOrder() == AttackEnemyOrder[i];
                    else
                        return false;
                });
                if (target != null)
                {
                    target.Damaged(AttackDamage);
                    j++;
                }
            }
        }
        else
        {
            targetCanAttack[0].Damaged(AttackDamage);
        }

        if (CombatType == COMBAT_TYPE.STOP_ON_ATTACK)
            Move();
    }



    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        var target = collision.GetComponent<RuntimeStats>();
        if (target == null)
            return;

        if (isPlayer != target.isPlayer && !targetInRange.Contains(target))
            targetInRange.Add(target);
    }

    protected virtual void OnTriggerExit2D(Collider2D collision)
    {
        var target = collision.GetComponent<RuntimeStats>();
        if (target == null)
            return;

        if (!target.IsDead)
            targetInRange.Remove(target);
    }
}
