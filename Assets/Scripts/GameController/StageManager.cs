using CsvHelper;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEditor;
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
    public int Stars_1_reward { get; set; }
    public int Repeat_Reward { get; set; }

#if UNITY_EDITOR
    public Stage() { }
    public Stage(UnitData stage)
    {
        List = stage.ignore;
        ID = stage.id;
        Castle_Hp = stage.initHP;
        Stars_3_CastleHp = stage.stars_3_CastleHp;
        Stars_3_reward = stage.stars_3_reward;
        Stars_2_CastleHp = stage.stars_2_CastleHp;
        Stars_2_reward = stage.stars_2_reward;
        Stars_1_reward = stage.stars_1_reward;
        Repeat_Reward = stage.repeat_Reward;
    }
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

        unitData.id = ID;
        unitData.ignore = List;
        unitData.isTower = true;
        unitData.initHP = Castle_Hp;
        unitData.stars_3_CastleHp = Stars_3_CastleHp;
        unitData.stars_3_reward = Stars_3_reward;
        unitData.stars_2_CastleHp = Stars_2_CastleHp;
        unitData.stars_2_reward = Stars_2_reward;
        unitData.stars_1_reward = Stars_1_reward;
        unitData.repeat_Reward = Repeat_Reward;

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
    private float goldInterval;
    #endregion
    #region Enemy
    public TowerAI enemyTower;
    #endregion


    private void Start()
    {
        Gold = 700;
        goldInterval = Time.time;
        for (int i = 0; i < uiOnStage.buttonSummons.Length; i++)
        {
            uiOnStage.buttonSummons[i].SetData(GameManager.Instance.GetExpedition(i));
            int index = i;
            uiOnStage.buttonSummons[i].button.onClick.AddListener(() =>
            {
                if (uiOnStage.buttonSummons[index].cooldown.value <= uiOnStage.buttonSummons[index].cooldown.minValue
                && playerTower.CanSpawnUnit()
                && UseGold(uiOnStage.buttonSummons[index].CharacterInfos.unitData.cost))
                {
                    playerTower.SpawnUnit(uiOnStage.buttonSummons[index].CharacterInfos);
                    uiOnStage.buttonSummons[index].Summoned();
                }
            });
        }

        enemyTower.unitData = playerTower.unitData = Resources.Load<UnitData>(string.Format(Paths.resourcesStage, GameManager.Instance.SelectedStageID));
        playerTower.isPlayer = true;
        playerTower.ResetUnit();
        playerTower.OnDead += Defeat;

        enemyTower.isPlayer = false;
        enemyTower.ResetUnit();
        enemyTower.OnDead += Victory;
    }

    private void Update()
    {
        if (Time.time >= goldInterval + 2f)
        {
            goldInterval = Time.time;
            Gold += 100;
        }
    }

    #region Cost
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
    #endregion

    public void Defeat()
    {
        GameManager.Instance.LoadingScene(Scenes.devMain);
    }

    public void Victory()
    {
        int star;
        int prevStar = 0;
        int flag = 0;

        switch (playerTower.HP)
        {
            case int hp when hp >= playerTower.unitData.stars_3_CastleHp:
                star = 3;
                break;
            case int hp when hp >= playerTower.unitData.stars_2_CastleHp:
                star = 2;
                break;
            default:
                star = 1;
                break;
        };

        if (GameManager.Instance.StageClearInfo.ContainsKey(playerTower.unitData.id))
        {
            prevStar = GameManager.Instance.StageClearInfo[playerTower.unitData.id];
        }

        for (int i = prevStar + 1; i <= star; i++)
        {
            flag += i switch
            {
                3 => playerTower.unitData.stars_3_reward - playerTower.unitData.stars_2_reward,
                2 => playerTower.unitData.stars_2_reward - playerTower.unitData.stars_1_reward,
                _ => playerTower.unitData.stars_1_reward
            };
        }

        GameManager.Instance.StageClear(GameManager.Instance.SelectedStageID, star, prevStar == 3 ? playerTower.unitData.repeat_Reward : flag);
        GameManager.Instance.LoadingScene(Scenes.devMain);
    }
}
