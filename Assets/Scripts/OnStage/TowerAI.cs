using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TowerAI : RuntimeStats
{
    private Player player;
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
        ResetAI();
    }
    private void Start()
    {
        player = GameObject.FindWithTag(Tags.player).GetComponent<Player>();
    }

    protected override void Update()
    {
        base.Update();
        if (!isPlayer && Time.time >= lastSpawnTime + spawnInterval && CanSpawnUnit())
        {
            SpawnUnit(GameManager.Instance.Expedition[0]);
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
    public bool CanSpawnUnit()
    {
        if (isBlocked)
            return false;
        else
            return true;
    }

    public void SpawnUnit(CharacterInfos characterInfos)
    {
        var unit = Instantiate(characterRoot, transform.position, Quaternion.Euler(Vector3.up)).GetComponent<UnitAI>();
        var animator = Instantiate(characterInfos.animator, unit.transform);
        unit.initStats = characterInfos.initStats;
        unit.OnDead += () => { units.Remove(unit); };
        if (isPlayer)
            unit.OnDead += () => { player.GetExp(unit.initStats.initDropExp); player.GetGold(unit.initStats.initDropGold); };
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
