using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(UnitStats))]
public class TowerAI : MonoBehaviour
{
    private UnitStats towerStats;
    private List<GameObject> units = new();
    private bool isBlocked;

    private void Awake()
    {
        towerStats = GetComponent<UnitStats>();

        towerStats.OnDead += () => Destroy(gameObject);
    }
    protected virtual void OnEnable()
    {
        towerStats.ResetStats();
        ResetAI();
    }

    private void Update()
    {
    }

    private void ResetAI()
    {
        if (towerStats.isPlayer)
            transform.localScale = new(-1f, 1f, 1f);
        else
            transform.localScale = Vector3.one;
    }


    public void TrySpawnUnit(GameObject prefab)
    {
        if (isBlocked)
            return;

        var unit = Instantiate(prefab, transform.position, Quaternion.Euler(Vector3.up));
        unit.GetComponent<UnitStats>().OnDamaged += () => { units.Remove(unit); };
        units.Add(unit);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.isTrigger)
            return;
        var unit = collision.GetComponent<UnitStats>();
        if(unit != null && unit.isPlayer == towerStats.isPlayer)
            isBlocked = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.isTrigger)
            return;
        var unit = collision.GetComponent<UnitStats>();
        if (unit != null && unit.isPlayer == towerStats.isPlayer)
            isBlocked = false;
        
    }
}
