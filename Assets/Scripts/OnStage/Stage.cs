using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Stage : MonoBehaviour
{
    public UIOnStage uiOnStage;
    public TowerAI tower;
    private int gold;
    private int exp;
    public int Gold 
    {
        get => gold;
        private set
        {
            gold = value;
            uiOnStage.textGold.text = gold.ToString();
        }
    }
    public int Exp
    {
        get => exp;
        private set
        {
            exp = value;
           // uiOnStage.textExp.text = exp.ToString();
        }
    }
    private void Start()
    {
        Gold = int.MaxValue;
        for (int i = 0; i < uiOnStage.buttonSummons.Length; i++)
        {
            uiOnStage.buttonSummons[i].SetData(GameManager.Instance.GetExpedition(i));
            int index = i;
            uiOnStage.buttonSummons[i].button.onClick.AddListener(() =>
            {
                if (tower.CanSpawnUnit() && UseGold(uiOnStage.buttonSummons[index].CharacterInfos.initStats.cost))
                    tower.SpawnUnit(uiOnStage.buttonSummons[index].CharacterInfos);
            });
        }
    }

    public void GetExp(int exp)
    {
        Exp += exp;
    }

    public void GetGold(int gold)
    {
        Gold += gold;
    }

    public bool UseExp(int exp)
    {
        if (Exp >= exp)
        {
            Exp -= exp;
            return true;
        }
        else
        {
            return false;
        }
    }
    public bool UseGold(int gold)
    {
        if (Gold >= gold)
        {
            Gold -= gold;
            return true;
        }
        else
        {
            return false;
        }
    }
}
