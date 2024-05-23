using System.Collections.Generic;
using UnityEngine;

public class TowerAI : UnitBase
{
    private StageManager stage;
    public TowerAI enemyTower;
    public CharacterAI characterRoot;
    public List<CharacterAI> units { get; private set; } = new();
    public List<int> waitingUnits = new();

    private bool isBlocked;
    private float nextSpawnTime;
    private float waitTime;
    private bool isPatternEnd = true;
    private int phase;
    private bool isStopSpawn;

    public void SetStopSpawn(bool stopSpawn)
    {
        isStopSpawn = stopSpawn;
    }

    private void Awake()
    {
        OnDead += () =>
        {
            for (int i = 0; i < units.Count; i++)
            {
                if (units[i] != null && units[i].isPlayer == isPlayer)
                    units[i].Damaged(units[i].MaxHP);
            };
        };
        OnDamaged += () =>
        {
            if (phase < 2 && HP <= MaxHP * 0.5f)
            {
                phase = 2;
                foreach (var unit in enemyTower.units)
                {
                    unit.Knockback();
                }
            }
        };
    }
    protected override void Start()
    {
        base.Start();
        stage = GameObject.FindWithTag(Tags.player).GetComponent<StageManager>();
    }

    protected override void Update()
    {
        base.Update();
        if (IsDead || isPlayer)
            return;
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Delete))
            Damaged(100);
#endif
        if (isStopSpawn)
            return;

        if (isPatternEnd && Time.time >= nextSpawnTime)
        {
            int stageID = GameManager.Instance.SelectedStageID;
            if (HP <= MaxHP * 0.3f)
                stageID += 200;
            else if (HP <= MaxHP * 0.5f)
                stageID += 100;

            while (stageID >= 200 && !DataTableManager.MonsterAppares.ContainsKey(stageID))
            {
                stageID -= 100;
            }
            PatternSet patternSet = DataTableManager.MonsterAppares[stageID].GetPattern();

            var patterns = DataTableManager.Patterns[patternSet.pattern];
            if (patterns.Monster_1 != 0)
                waitingUnits.Add(patterns.Monster_1);
            if (patterns.Monster_2 != 0)
                waitingUnits.Add(patterns.Monster_2);
            if (patterns.Monster_3 != 0)
                waitingUnits.Add(patterns.Monster_3);
            isPatternEnd = false;
            waitTime = patternSet.waitingTime;
        }

        if (waitingUnits.Count > 0 && CanSpawnUnit())
        {
            CharacterInfos enemy = new();
            enemy.SetData(Resources.Load<UnitData>(string.Format(Paths.resourcesEnemy, waitingUnits[0])));
            SpawnUnit(enemy);
            waitingUnits.RemoveAt(0);
        }

        if (!isPatternEnd && waitingUnits.Count == 0)
        {
            isPatternEnd = true;
            nextSpawnTime = Time.time + waitTime;
        }
    }

    public override void ResetUnit()
    {
        base.ResetUnit();

        if (isPlayer)
            transform.localScale = Vectors.filpX;
        else
            transform.localScale = Vector3.one;

        isBlocked = false;
        phase = 1;
    }
    public bool CanSpawnUnit()
    {
        if (isBlocked ||
            (units != null && units.Count > 0
            && Mathf.Sign(units[^1].transform.position.x - transform.position.x) == (isPlayer ? -1f : 1f)))
            return false;
        else
            return true;
    }

    public void SpawnUnit(CharacterInfos characterInfos)
    {
        var unit = Instantiate(characterRoot, transform.position, Quaternion.Euler(Vector3.up)).GetComponent<CharacterAI>();
        unit.SetUnitData(characterInfos);
        unit.ResetUnit();
        unit.SetTower(this);
        unit.OnDead += () => { units.Remove(unit); };
        if (!isPlayer)
            unit.OnDead += () =>
            {
                if (!unit.IsSelfDestruct)
                {
                    stage.GetExp(unit.unitData.initDropExp);
                    stage.GetGold(unit.unitData.initDropGold);
                }
            };
        units.Add(unit);
    }



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.isTrigger)
            return;

        var unit = collision.GetComponent<CharacterAI>();
        if (unit == null)
            return;

        if (unit.isPlayer == isPlayer)
            isBlocked = true;
        else
            unit.SetIsBlocked(true, this);


    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.isTrigger)
            return;

        var unit = collision.GetComponent<CharacterAI>();
        if (unit == null)
            return;

        if (unit.isPlayer == isPlayer)
            isBlocked = true;
        else
            unit.SetIsBlocked(true, this);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.isTrigger)
            return;

        var unit = collision.GetComponent<CharacterAI>();
        if (unit == null)
            return;

        if (unit.isPlayer == isPlayer)
            isBlocked = false;
        else
            unit.SetIsBlocked(false, this);

    }

}
