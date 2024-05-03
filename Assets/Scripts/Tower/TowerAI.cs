using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(UnitStats))]   
public class TowerAI : MonoBehaviour
{
    private UnitStats towerStats;
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

    private void ResetAI()
    {
        if (towerStats.isPlayer)
            transform.localScale = new(-1f, 1f, 1f);
        else
            transform.localScale = Vector3.one;
    }

}
