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
    public int Reward_Char1 { get; set; }
    public int Reward_Char2 { get; set; }
    public int Reward_Char3 { get; set; }
    public int Reward_Char4 { get; set; }
    public string String_ID {  get; set; }


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
        String_ID = stage.prefab;
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
        unitData.prefab = String_ID;

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
    public int startGold;
    public int startExp;
    public int getGoldPerSeconds;
    public int getExpPerSeconds;
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
            uiOnStage.textExp.text = exp.ToString();
            foreach (var button in uiOnStage.buttonSummons)
            {
                button.outline.enabled = isUpgrading
                && !button.IsUpgraded
                && ((uiOnStage.toggleUpgardeDamage.isOn && exp >= button.UpgradeExpDamage)
                || (uiOnStage.toggleUpgardeHP.isOn && exp >= button.UpgradeExpHP));

            }
        }
    }
    private float goldInterval;
    #endregion
    #region Enemy
    public TowerAI enemyTower;
    #endregion

    private bool isUpgrading;

    private void Start()
    {
        Gold = startGold;
        Exp = startExp;
        goldInterval = Time.time;
        SetSummonButton();
        SetUpgradeToggle();

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
            Gold += getGoldPerSeconds;
            Exp += getExpPerSeconds;
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


    public void SetSummonButton()
    {
        for (int i = 0; i < uiOnStage.buttonSummons.Length; i++)
        {
            uiOnStage.buttonSummons[i].SetData(GameManager.Instance.GetExpedition(i));
            int index = i;
            uiOnStage.buttonSummons[i].button.onClick.AddListener(() =>
            {
                if (isUpgrading)
                {
                    if (uiOnStage.buttonSummons[index].IsUpgraded == true)
                        return;

                    if (uiOnStage.toggleUpgardeDamage.isOn
                    && uiOnStage.buttonSummons[index].outline.enabled)
                    {
                        uiOnStage.buttonSummons[index].UpgradeDamage();
                        uiOnStage.toggleUpgardeDamage.isOn = false;
                        Exp -= uiOnStage.buttonSummons[index].UpgradeExpDamage;
                    }

                    if (uiOnStage.toggleUpgardeHP.isOn
                     && uiOnStage.buttonSummons[index].outline.enabled)
                    {
                        uiOnStage.buttonSummons[index].UpgradeHP();
                        uiOnStage.toggleUpgardeHP.isOn = false;
                        Exp -= uiOnStage.buttonSummons[index].UpgradeExpHP;
                    }

                    uiOnStage.buttonSummons[index].IsUpgraded = true;
                    return;
                }

                if (uiOnStage.buttonSummons[index].cooldown.value <= uiOnStage.buttonSummons[index].cooldown.minValue
                && playerTower.CanSpawnUnit()
                && UseGold(uiOnStage.buttonSummons[index].CharacterInfos.unitData.cost))
                {
                    playerTower.SpawnUnit(uiOnStage.buttonSummons[index].CharacterInfos);
                    uiOnStage.buttonSummons[index].Summoned();
                }
            });
        }
    }

    public void SetUpgradeToggle()
    {
        uiOnStage.toggleUpgardeDamage.onValueChanged.AddListener(x =>
        {
            if (x)
            {
                bool canUpgrade = false;
                foreach (var buttonsummon in uiOnStage.buttonSummons)
                {
                    if (buttonsummon.gameObject.activeSelf && exp >= buttonsummon.UpgradeExpDamage)
                    {
                        canUpgrade = true;
                    }
                }

                if (!canUpgrade)
                {
                    uiOnStage.toggleUpgardeDamage.isOn = false;
                    return;
                }
            }

            isUpgrading = x;

            foreach (var button in uiOnStage.buttonSummons)
            {

                button.outline.enabled = x && !button.IsUpgraded && exp >= button.UpgradeExpDamage;
                button.outline.color = Color.red;
            }

        });
        uiOnStage.toggleUpgardeHP.onValueChanged.AddListener(x =>
        {
            if (x)
            {
                bool canUpgrade = false;
                foreach (var buttonsummon in uiOnStage.buttonSummons)
                {
                    if (buttonsummon.gameObject.activeSelf && exp >= buttonsummon.UpgradeExpHP)
                    {
                        canUpgrade = true;
                    }
                }

                if (!canUpgrade)
                {
                    uiOnStage.toggleUpgardeHP.isOn = false;
                    return;
                }
            }
            isUpgrading = x;

            foreach (var button in uiOnStage.buttonSummons)
            {
                button.outline.enabled = x && !button.IsUpgraded && exp >= button.UpgradeExpHP;
                button.outline.color = Color.blue;
            }
        });

    }
}
