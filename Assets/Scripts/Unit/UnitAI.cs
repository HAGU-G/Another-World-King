using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(UnitStats))]
[RequireComponent(typeof(Rigidbody2D))]
public class UnitAI : MonoBehaviour
{
    public Animator animator;
    public Rigidbody2D rb;

    private UnitStats unitStats;
    public BoxCollider2D attackCollider;

    private UnitStats target = null;
    private bool isFighting;

    private float lastAttackTime;

    protected virtual void Awake()
    {
        unitStats = GetComponent<UnitStats>();
        unitStats.OnDead += () =>
        {
            foreach (var c in GetComponents<Collider>())
                c.enabled = false;

            //TESTCODE
            Destroy(gameObject);
        };
    }
    protected virtual void OnEnable()
    {
        unitStats.ResetStats();
        ResetAI();
    }

    protected virtual void Update()
    {
        if (isFighting)
        {
            rb.isKinematic = true;
            rb.velocity = Vector3.zero;
            if (target != null && Time.time >= lastAttackTime + 1f / unitStats.AttackSpeed)
                TryAttack();
        }
        else
        {
            rb.isKinematic = false;
            rb.velocity = transform.forward * unitStats.MoveSpeed * -transform.localScale.x;
        }
    }

    private void ResetAI()
    {
        foreach (var c in GetComponents<Collider>())
            c.enabled = true;
        attackCollider.size = new Vector2(0.3f + unitStats.AttackRange, 0.1f);
        attackCollider.offset = new Vector2(-attackCollider.size.x * 0.5f, unitStats.isPlayer ? 0.2f : 0.6f);

        lastAttackTime = Time.time - 1f / unitStats.AttackSpeed;
        if (unitStats.isPlayer)
            transform.localScale = new(-1f, 1f, 1f);
        else
            transform.localScale = Vector3.one;
    }

    protected virtual bool SetTarget(UnitStats target)
    {
        if (unitStats.isPlayer != target.isPlayer)
            this.target = target;
        return this.target != null;
    }

    protected virtual void TryAttack()
    {
        lastAttackTime = Time.time;
        target.Damaged(unitStats.AttackDamage);
        if (target == null || target.IsDead)
        {
            target = null;
            isFighting = false;
        }

    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        var us = collision.GetComponent<UnitStats>();
        if (us == null)
            return;

        if (SetTarget(us))
            isFighting = true;
    }

    protected virtual void OnTriggerExit2D(Collider2D collision)
    {
        var us = collision.GetComponent<UnitStats>();
        if (us == null)
            return;

        if (target == us)
        {
            isFighting = false;
            target = null;
        }
    }
}
