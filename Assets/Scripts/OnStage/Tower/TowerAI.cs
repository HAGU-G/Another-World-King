using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class TowerAI : RuntimeStats
{
    public TowerAI enemyTower;
    public List<UnitAI> units { get; private set; } = new();
    private bool isBlocked;

    private void Awake()
    {
        OnDead += () =>
        {
            foreach (var item in GameObject.FindGameObjectsWithTag(Tags.unit))
            {
                if(item.GetComponent<RuntimeStats>().isPlayer == isPlayer)
                    Destroy(item);
            };
            Destroy(gameObject);
        };
    }
    protected virtual void OnEnable()
    {
        ResetAI();
    }

    public void ResetAI()
    {
        ResetStats();
        if (isPlayer)
            transform.localScale = new(-1f, 1f, 1f);
        else
            transform.localScale = Vector3.one;
    }


    public void TrySpawnUnit(GameObject prefab)
    {
        if (isBlocked)
            return;

        var unit = Instantiate(prefab, transform.position, Quaternion.Euler(Vector3.up)).GetComponent<UnitAI>();
        unit.OnDead += () => { units.Remove(unit); };
        unit.tower = this;
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

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.isTrigger)
            return;
        var unit = collision.GetComponent<RuntimeStats>();
        if (unit != null && unit.isPlayer == isPlayer)
            isBlocked = false;

    }
}
