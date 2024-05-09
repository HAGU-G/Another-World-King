using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TowerAI : RuntimeStats
{
    public TowerAI enemyTower;
    public UnitAI characterRoot;
    public List<UnitAI> units { get; private set; } = new();

    private bool isBlocked;
    private float spawnInterval = 1f;
    private float lastSpawnTime = 0f;


    private void Awake()
    {
        OnDead += () =>
        {
            foreach (var item in GameObject.FindGameObjectsWithTag(Tags.unit))
            {
                if (item.GetComponent<RuntimeStats>().isPlayer == isPlayer)
                    Destroy(item);
            };
            Destroy(gameObject);
        };
    }
    protected virtual void OnEnable()
    {
        ResetAI();
    }

    protected override void Update()
    {
        if (!isPlayer && Time.time >= lastSpawnTime + spawnInterval)
        {
            TrySpawnUnit(GameManager.Instance.Expedition[0]);
            lastSpawnTime = Time.time;
        }
    }

    public void ResetAI()
    {
        ResetStats();
        if (isPlayer)
            transform.localScale = new(-1f, 1f, 1f);
        else
            transform.localScale = Vector3.one;
    }


    public void TrySpawnUnit(CharacterInfos characterInfos)
    {
        if (isBlocked)
            return;

        var unit = Instantiate(characterRoot, transform.position, Quaternion.Euler(Vector3.up)).GetComponent<UnitAI>();
        var animator = Instantiate(characterInfos.animator, unit.transform);
        unit.initStats = characterInfos.initStats;
        unit.OnDead += () => { units.Remove(unit); };
        unit.Tower = this;
        unit.ResetAI();
        units.Add(unit);
    }



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.isTrigger)
            return;
        var unit = collision.GetComponent<RuntimeStats>();
        if (unit != null && unit.isPlayer == isPlayer)
            isBlocked = true;

    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.isTrigger)
            return;
        var unit = collision.GetComponent<RuntimeStats>();
        if (unit != null && unit.isPlayer == isPlayer)
            isBlocked = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.isTrigger)
            return;
        var unit = collision.GetComponent<RuntimeStats>();
        if (unit != null && unit.isPlayer == isPlayer)
            isBlocked = false;

    }

}
