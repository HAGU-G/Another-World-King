using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class TowerAI : RuntimeStats
{
    private List<RuntimeStats> units = new();
    private bool isBlocked;

    private void Awake()
    {
        OnDead += () => Destroy(gameObject);
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

        var stats = Instantiate(prefab, transform.position, Quaternion.Euler(Vector3.up)).GetComponent<RuntimeStats>();
        stats.OnDead += () => { units.Remove(stats); };
        var unit = stats as UnitAI;
        unit.towerUnits = units;
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
