using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Stage
{
    public string List { get; set; }
    public int ID { get; set; }
    public int Castle_Hp { get; set; }
    public int Stars_3_CastleHp { get; set; }
    public int Stars_3_reward { get; set; }
    public int Stars_2_CastleHp { get; set; }
    public int Stars_2_reward { get; set; }
    public int Stars_1_CastleHp { get; set; }
    public int Stars_1_reward { get; set; }
    public int Repeat_Reward { get; set; }

#if UNITY_EDITOR
    public void ToScriptable()
    {
        UnitData unitData;

        unitData = Resources.Load<UnitData>(string.Format(Paths.resourcesStage, ID));
        bool create = false;
        if (unitData == null)
        {
            unitData = ScriptableObject.CreateInstance<UnitData>();
            create = true;
        }

        unitData.id = ID.ToString();
        unitData.ignore = List;
        unitData.isTower = true;
        unitData.initHP = Castle_Hp;

        if (create)
        {
            AssetDatabase.CreateAsset(unitData,
                string.Concat(
                    Paths.folderResources,
                    string.Format(Paths.resourcesStage, unitData.id),
                    Paths._asset));
        }
    }
#endif
}

public class StageManager : MonoBehaviour
{
    public UIOnStage uiOnStage;
    #region Player
    public TowerAI playerTower;
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
    #endregion
    #region Enemy
    public TowerAI enemyTower;
    #endregion


    private void Start()
    {
        Gold = int.MaxValue / 2;
        for (int i = 0; i < uiOnStage.buttonSummons.Length; i++)
        {
            uiOnStage.buttonSummons[i].SetData(GameManager.Instance.GetExpedition(i));
            int index = i;
            uiOnStage.buttonSummons[i].button.onClick.AddListener(() =>
            {
                if (playerTower.CanSpawnUnit() && UseGold(uiOnStage.buttonSummons[index].CharacterInfos.unitData.cost))
                    playerTower.SpawnUnit(uiOnStage.buttonSummons[index].CharacterInfos);
            });
        }

        playerTower.unitData = Resources.Load<UnitData>(string.Format(Paths.resourcesStage, GameManager.Instance.SelectedStageID));
        playerTower.isPlayer = true;
        playerTower.ResetUnit();
        enemyTower.unitData = Resources.Load<UnitData>(string.Format(Paths.resourcesStage, GameManager.Instance.SelectedStageID));
        enemyTower.isPlayer = false;
        enemyTower.ResetUnit();
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
