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
        Gold = 700;
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
        playerTower.OnDead += Defeat;

        enemyTower.unitData = Resources.Load<UnitData>(string.Format(Paths.resourcesStage, GameManager.Instance.SelectedStageID));
        enemyTower.isPlayer = false;
        enemyTower.ResetUnit();
        enemyTower.OnDead += Victory;
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
        int flag;

        switch (playerTower.HP)
        {
            case int hp when hp >= DataTableManager.Stages[GameManager.Instance.SelectedStageID].Stars_3_CastleHp:
                star = 3;
                break;
            case int hp when hp >= DataTableManager.Stages[GameManager.Instance.SelectedStageID].Stars_2_CastleHp:
                star = 2;
                break;
            default:
                star = 1;
                break;
        };

        if (GameManager.Instance.StageClearInfo.ContainsKey(GameManager.Instance.SelectedStageID))
        {
            flag = DataTableManager.Stages[GameManager.Instance.SelectedStageID].Repeat_Reward;
        }
        else
        {
            flag = star switch
            {
                3 => DataTableManager.Stages[GameManager.Instance.SelectedStageID].Stars_3_reward,
                2 => DataTableManager.Stages[GameManager.Instance.SelectedStageID].Stars_2_reward,
                _ => DataTableManager.Stages[GameManager.Instance.SelectedStageID].Stars_1_reward,
            };
        }
        GameManager.Instance.StageClear(GameManager.Instance.SelectedStageID, star, flag);
        GameManager.Instance.LoadingScene(Scenes.devMain);
    }
}
