using System.Collections.Generic;
using Unity.Android.Types;
using Unity.VisualScripting;
using UnityEngine;

public class TowerAI : UnitBase
{
    private Stage stage;
    public TowerAI enemyTower;
    public CharacterAI characterRoot;
    public List<CharacterAI> units { get; private set; } = new();

    private bool isBlocked;
    private float spawnInterval = 1f;
    private float lastSpawnTime = 0f;


    private void Awake()
    {
        OnDead += () =>
        {
            foreach (var item in GameObject.FindGameObjectsWithTag(Tags.unit))
            {
                if (item.GetComponent<UnitBase>().isPlayer == isPlayer)
                    Destroy(item);
            };
            Destroy(gameObject);
            GameManager.Instance.ChangeScene(Scenes.devMain);
        };
        ResetUnit();
    }
    private void Start()
    {
        stage = GameObject.FindWithTag(Tags.player).GetComponent<Stage>();
    }

    protected override void Update()
    {
        base.Update();
        if (isPlayer)
            return;

        if (Time.time >= lastSpawnTime + spawnInterval && CanSpawnUnit())
        {
            SpawnUnit(GameManager.Instance.Expedition[0]);
            lastSpawnTime = Time.time;
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
        var unit = Instantiate(characterRoot, transform.position, Quaternion.Euler(Vector3.up)).GetComponent<CharacterAI>();
        var animator = Instantiate(characterInfos.animator, unit.transform);
        unit.initStats = characterInfos.initStats;
        unit.ResetUnit();
        unit.SetTower(this);
        unit.OnDead += () => { units.Remove(unit); };
        if (!isPlayer)
            unit.OnDead += () =>
            {
                stage.GetExp(unit.initStats.initDropExp);
                stage.GetGold(unit.initStats.initDropGold);
            };
        units.Add(unit);
    }



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.isTrigger)
            return;
        var unit = collision.GetComponent<UnitBase>();
        if (unit != null && unit.isPlayer == isPlayer)
            isBlocked = true;

    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.isTrigger)
            return;
        var unit = collision.GetComponent<UnitBase>();
        if (unit != null && unit.isPlayer == isPlayer)
            isBlocked = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.isTrigger)
            return;
        var unit = collision.GetComponent<UnitBase>();
        if (unit != null && unit.isPlayer == isPlayer)
            isBlocked = false;

    }

}
