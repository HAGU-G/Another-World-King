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
    public Transform dropEffectPosition;
    public DivisionIcon hudDivision;
    public StatInfo hudStat;
    public HealthBar hudHealthBar;
    public Transform bossStatPos;
    private CharacterSound characterSound;
    private GameObject prefab;

    public TowerAI Tower { get; set; }
    private List<UnitBase> enemyInRange = new();
    private List<UnitBase> targets = new();
    private List<UnitBase> blocks = new();

    public bool IsSuicide { get; private set; }
    private float lastAttackTime;
    public float alphaReduceTime = 3f;
    private float knockbackTime;
    private float currentAlphaTime;
    private bool isCounterBuffed;
    public bool IsSelfDestruct { get; private set; }

    public UNIT_STATE UnitState { get; private set; }

    private void OnDisable()
    {
        Tower = null;
        enemyInRange.Clear();
        targets.Clear();
        blocks.Clear();
        IsSuicide = false;
        lastAttackTime = 0f;
        knockbackTime = 0f;
        currentAlphaTime = 0f;
        isCounterBuffed = false;
        IsSelfDestruct = false;
        UnitState = UNIT_STATE.IDLE;
        CurrnetUnitData = null;
        CurrentStageManager = null;
        SetInvincibility(false);
        IsDead = false;
        SetSkill(null);
        SetCounterSkill(null);
        Buff.Clear();
        damageUpgradeValue = 0;
        hpUpgradeValue = 0;
        EvnetClear();
        if (prefab != null)
        {
            Destroy(prefab);
        }
    }
    public void Init()
    {
        OnDead += () => { Dead(); };
#if UNITY_EDITOR
        //TESTCODE
        //var d = Instantiate(Resources.Load(Paths.resourcesDebugStat), transform);
        //OnDead += () => { Destroy(d); };
#endif
        if (hudDivision != null)
            hudDivision.gameObject.SetActive(true);
        if (hudStat != null)
            hudStat.gameObject.SetActive(true);
        if (hudHealthBar != null)
            hudHealthBar.gameObject.SetActive(true);
    }

    protected override void Update()
    {
        base.Update();

        if (IsDead)
        {
            currentAlphaTime += Time.deltaTime;
            if (spriteRenderers != null && spriteRenderers.Length > 0)
            {
                foreach (var spriteRenderer in spriteRenderers)
                {
                    spriteRenderer.color = Color.Lerp(
                        new(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1f),
                        new(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0f),
                        currentAlphaTime / alphaReduceTime);
                }
            }
            if (currentAlphaTime >= alphaReduceTime)
            {
                CurrentStageManager.CharacterAIPool.Release(this);
            }
            return;
        }
        if (UnitState == UNIT_STATE.KNOCKBACK)
        {
            if (Time.time < knockbackTime + 1f / 10f)
                return;
            knockbackTime = Time.time;

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
        else if (Mathf.Abs(targets[0].transform.position.x - transform.position.x) > AttackStartRange)
        {
            Move();
            return;
        }

        //ÀÚÆø
        if (IsSelfDestruct)
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
        { Idle(); blocks.RemoveAll(x => x == null || x.IsDead); }
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

                animator.speed = speed * CurrnetUnitData.initAttackSpeed / AttackSpeed;
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
                        animator.speed = MoveSpeed / 80f;
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
            blocks.RemoveAll(x => x == null || x.IsDead);
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
        prefab = Instantiate(characterInfos.dress, transform);
        CurrnetUnitData = characterInfos.unitData;
        SetSkill(characterInfos.skillData);
        SetCounterSkill(characterInfos.counterSkillData);

        damageUpgradeValue = characterInfos.damageOnceUpgradeValue * characterInfos.damageUpgradedCount;
        hpUpgradeValue = characterInfos.hpOnceUpgradeValue * characterInfos.hpUpgradedCount;
        hudDivision.Init();
        hudHealthBar.Init();
    }

    public override void ResetUnit()
    {
        base.ResetUnit();

        foreach (var c in GetComponents<Collider2D>())
        {
            c.enabled = true;
        }

        InitAttackCollider();

        lastAttackTime = Time.time - AttackSpeed;

        enemyInRange.Clear();
        targets.Clear();

        blocks.Clear();
        Animators = GetComponentsInChildren<Animator>();
        if (Animators != null && Animators.Length > 0)
        {
            var eventListener = Animators[0].AddComponent<CharacterAnimationEventListner>();
            eventListener.Init();
            eventListener.onAttackHit += AttackHit;
            eventListener.onAttackEnd += AttackEnd;
            eventListener.onPlayAttackEffect += PlayAttackEffect;
            eventListener.onKillSelf += () =>
            {
                IsSuicide = true;
                Damaged(MaxHP);
            };
        }
        characterSound = GetComponentInChildren<CharacterSound>();
        spriteRenderers = prefab.GetComponentsInChildren<SpriteRenderer>();

        Idle();
    }

    private void InitAttackCollider()
    {
        attackCollider.size = new Vector2(0.3f + AttackRange * 0.6f, 0.1f);
        //attackCollider.offset = new Vector2(-attackCollider.size.x * 0.5f, isPlayer ? 0.2f : 0.6f);
        attackCollider.offset = new Vector2(-attackCollider.size.x * 0.5f, 0.2f);

        float charScale = 1f;
        if (!isPlayer && CurrnetUnitData.id >= 400)
            charScale = 2f;

        if (isPlayer)
            transform.localScale = Vectors.filpX * charScale;
        else
            transform.localScale = Vector3.one * charScale;


        if (!isPlayer && CurrnetUnitData.id >= 400)
        {
            hudStat.transform.position = bossStatPos.position;
        }
        else
        {
            hudStat.transform.position = dropEffectPosition.position;
        }
    }

    public void SetTower(TowerAI tower)
    {
        Tower = tower;
        isPlayer = Tower.isPlayer;
        InitAttackCollider();
        hudDivision.Init();
        hudHealthBar.Init();
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
                {
                    targets.Add(Tower.enemyTower.units[attackEnemyOrder - 1]);
                }
            }
            if (count >= AttackEnemyCount)
                break;
        }

        if (enemyInRange.Contains(Tower.enemyTower))
            targets.Add(Tower.enemyTower);

        IsSelfDestruct = (targets.Count == 1 && targets[0].IsTower && (isPlayer || (!isPlayer && CurrnetUnitData.id < 400)));
    }

    protected virtual void Attack()
    {
        if (UnitState == UNIT_STATE.ATTACK)
            return;
        if (CurrnetUnitData.division == UnitData.DIVISION.BOMBER && blocks.Count == 0)
            return;

        UnitState = UNIT_STATE.ATTACK;
        lastAttackTime = Time.time;
        SetAnimation(AnimatorTriggers.attack);

        if (targets != null
            && CounterSkill != null
            && CounterSkill.target == SkillData.TARGET.ONESELF
            && targets.Find(x => x.CurrnetUnitData.division == CounterSkill.targetDivision) != null)
        {
            ApplyBuff(CounterSkill);
            isCounterBuffed = true;
        }
    }

    private void LaunchProjectile(out bool unitAttacked, UnitBase target)
    {
        unitAttacked = true;

        int atk;
        int counterAtk;
        if (CounterSkill != null)
        {
            ClearBuff(CounterSkill);
            atk = AttackDamage;
            ApplyBuff(CounterSkill);
            counterAtk = AttackDamage;
            if (isCounterBuffed && target.CurrnetUnitData.division != CounterSkill.targetDivision)
                ClearBuff(CounterSkill);
        }
        else
        {
            atk = AttackDamage;
            counterAtk = AttackDamage;
        }

        PlayAttackEffect();
        var projectile = Instantiate(Resources.Load<Projectile>(string.Format(Paths.resourcesProjectiles, CurrnetUnitData.projectile)));


        var targetPos = target.transform.position + Vector3.left * 0.3f * (isPlayer ? 1f : -1f);
        if (CurrnetUnitData.division == UnitData.DIVISION.CANNON)
        {
            if (Tower.enemyTower.IsBossPhase && Tower.enemyTower.units.Count > 0)
            {
                targetPos = Tower.enemyTower.units[^1].transform.position;
            }
            else
            {
                targetPos = Tower.enemyTower.transform.position;
                projectile.SetTowerTargeting();
            }
        }
        projectile.transform.position = transform.position + Vector3.up * 0.6f;
        projectile.Project(
            this,
            targetPos,
            atk,
            CounterSkill != null ? CounterSkill.targetDivision : UnitData.DIVISION.NONE,
            CounterSkill != null ? counterAtk : atk);
    }

    private void AttackHit()
    {
        bool towerAttacked = false;

        TargetFiltering();
        if (IsHealer)
        {
            if (GetOrder() == 1 && targets != null && targets.Count >= 1)
            {
                if (targets[0].IsTower)
                {
                    targets[0].Damaged(StageManager.Instance.castleDamage);
                    towerAttacked = true;
                }
                else
                {
                    targets[0].Damaged(AttackDamage);
                }
            }
            else
            {
                for (int i = 0; i < GetOrder() - 1; i++)
                {
                    Tower.units[i].Healed(Heal);
                    PlayHitEffect(Tower.units[i].transform.position);
                    characterSound.PlayAttackHitSound();
                    if (!Tower.units[i].IsDead && Skill != null && Skill.target == SkillData.TARGET.TEAM)
                        Tower.units[i].ApplyBuff(Skill);
                }
            }
        }
        else
        {
            characterSound.PlayAttackHitSound();
            int count = 0;
            bool unitAttacked = false;
            foreach (var target in targets)
            {
                if (unitAttacked && target.IsTower)
                    continue;

                if (target.IsTower)
                {
                    target.Damaged(StageManager.Instance.castleDamage);
                    PlayHitEffect(target.transform.position);
                    towerAttacked = true;
                }
                else
                {
                    if (isCounterBuffed && target.CurrnetUnitData.division != CounterSkill.targetDivision)
                        ClearBuff(CounterSkill);

                    if (CurrnetUnitData.division == UnitData.DIVISION.ARCHER
                        || CurrnetUnitData.division == UnitData.DIVISION.CANNON)
                    {
                        LaunchProjectile(out unitAttacked, target);
                    }
                    else
                    {
                        if (CurrnetUnitData.division == UnitData.DIVISION.BOMBER)
                        {
                            if (!target.isPlayer && target.CurrnetUnitData.id < 400)
                                target.Damaged(AttackDamage);
                            else
                                target.Damaged(target.MaxHP);
                        }
                        else
                        {
                            target.Damaged(AttackDamage);
                        }

                        PlayHitEffect(target.transform.position);
                        unitAttacked = true;

                        if (!target.IsDead && Skill != null && Skill.target == SkillData.TARGET.ENEMY)
                        {
                            target.ApplyBuff(Skill);
                        }
                        if (!target.IsDead
                            && CounterSkill != null
                            && CounterSkill.target == SkillData.TARGET.ENEMY
                            && target.CurrnetUnitData.division == CounterSkill.targetDivision)
                        {
                            target.ApplyBuff(CounterSkill);
                        }
                    }

                    if (isCounterBuffed && target.CurrnetUnitData.division != CounterSkill.targetDivision)
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

        if (towerAttacked && (isPlayer || (!isPlayer && CurrnetUnitData.id < 400)))
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
        knockbackTime = Time.time;
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

    public void PlayAttackEffect()
    {
        if (CurrnetUnitData.effectAttack == Defines.zero)
            return;

        if (EffectManager.Instance.EffectPool.ContainsKey(CurrnetUnitData.effectAttack))
        {
            var effect = EffectManager.Instance.EffectPool[CurrnetUnitData.effectAttack].Get();
            effect.gameObject.transform.position = transform.position;
            effect.transform.localScale = isPlayer ? Vectors.filpX : Vector3.one;
        }
    }

    public void PlayHitEffect(Vector3 enemyPos)
    {
        if (CurrnetUnitData.effectAttackHit == Defines.zero)
            return;

        if (EffectManager.Instance.EffectPool.ContainsKey(CurrnetUnitData.effectAttackHit))
        {
            var effect = EffectManager.Instance.EffectPool[CurrnetUnitData.effectAttackHit].Get();
            effect.gameObject.transform.position = enemyPos;
            effect.transform.localScale = isPlayer ? Vectors.filpX : Vector3.one;
        }
    }
}
