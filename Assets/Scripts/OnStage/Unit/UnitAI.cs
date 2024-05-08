using System.Collections.Generic;
using UnityEngine;


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
        if (targets.Count > 0)
        {
            if (CombatType == COMBAT_TYPE.STOP_ON_HAVE_TARGET)
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
        if(Tower != null)
            isPlayer = Tower.isPlayer;

        foreach (var c in GetComponents<Collider>())
            c.enabled = true;
        attackCollider.size = new Vector2(0.3f + AttackRange, 0.1f);
        attackCollider.offset = new Vector2(-attackCollider.size.x * 0.5f, isPlayer ? 0.2f : 0.6f);

        lastAttackTime = Time.time - 1f / AttackSpeed;
        if (isPlayer)
            transform.localScale = new(-1f, 1f, 1f);
        else
            transform.localScale = Vector3.one;

        enemyInRange.Clear();
        targets.Clear();

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

        foreach (var target in targets)
        {
            target.Damaged(AttackDamage);
        }

        if (CombatType == COMBAT_TYPE.STOP_ON_ATTACK)
            Move();
    }



    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        var target = collision.GetComponent<RuntimeStats>();
        if (target == null)
            return;

        if (isPlayer != target.isPlayer && !enemyInRange.Contains(target))
            enemyInRange.Add(target);
    }

    protected virtual void OnTriggerExit2D(Collider2D collision)
    {
        var target = collision.GetComponent<RuntimeStats>();
        if (target == null)
            return;

        enemyInRange.Remove(target);
    }
}
