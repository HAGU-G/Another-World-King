using ScrollBGTest;
using UnityEditor;
using UnityEditor.SceneManagement;
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
    public string String_ID { get; set; }
    public int Boss_ID { get; set; }


#if UNITY_EDITOR
    public Stage() { }
    public Stage(TowerData stage)
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
        Boss_ID = stage.bossID;
    }
    public void ToScriptable()
    {
        TowerData unitData;

        unitData = Resources.Load<TowerData>(string.Format(Paths.resourcesStage, ID));
        bool create = false;
        if (unitData == null)
        {
            unitData = ScriptableObject.CreateInstance<TowerData>();
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
        unitData.bossID = Boss_ID;

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
    public CameraManager stageCamera;
    public UIOnStage uiOnStage;
    public AudioClip audioWin;
    public AudioClip audioLose;
    public AudioSource audioSource;
    public int startGold;
    public int startExp;
    public int getGoldPer2Seconds;
    public int getExpPer2Seconds;
    public int castleDamage;
    public int gameSpeedValue;
    public bool IsTutorial { get; set; }
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
                button.outline.enabled =
                    isUpgrading
                    && (
                    (uiOnStage.toggleUpgardeDamage.isOn
                    && button.DamageUpgradedCount < button.DamageUpgradeMaxCount
                    && exp >= button.DamageUpgradeExp)
                    || (uiOnStage.toggleUpgardeHP.isOn
                    && button.HPUpgradedCount < button.HPUpgradeMaxCount
                    && exp >= button.HPUpgradeExp)
                    );
            }
            UpdateUpgradeToggle();
        }
    }
    private float goldInterval;
    #endregion
    #region Enemy
    public TowerAI enemyTower;
    #endregion

    private bool isUpgrading;
    private bool canUpgradeDamage;
    private bool canUpgradeHP;

    public static StageManager Instance => GameObject.FindWithTag(Tags.player)?.GetComponent<StageManager>();

    private void Start()
    {
        Gold = startGold;
        Exp = startExp;
#if UNITY_EDITOR
        Gold += 1000000;
        Exp += 1000000;
#endif
        goldInterval = Time.time;
        InitSummonButton();
        InitUpgradeToggle();
        UpdateUpgradeToggle();
        InitGameSpeedToggle();

        stageCamera.background = Instantiate(Resources.Load<ScrollBackgroundCtrl>(string.Format(Paths.resourcesBackgrounds, DataTableManager.Stages[GameManager.Instance.SelectedStageID].String_ID)), stageCamera.transform);
        enemyTower.unitData = playerTower.unitData = Resources.Load<TowerData>(string.Format(Paths.resourcesStage, GameManager.Instance.SelectedStageID));
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
            Gold += getGoldPer2Seconds;
            Exp += getExpPer2Seconds;
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
        audioSource.PlayOneShot(audioLose);
        uiOnStage.windowStagePause.Defeat();
    }

    public void Victory()
    {
        int star;
        int prevStar = 0;
        int flag = 0;

        var playerTowerData = playerTower.unitData as TowerData;

        switch (playerTower.HP)
        {
            case int hp when hp >= playerTowerData.stars_3_CastleHp:
                star = 3;
                break;
            case int hp when hp >= playerTowerData.stars_2_CastleHp:
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
                3 => playerTowerData.stars_3_reward - playerTowerData.stars_2_reward,
                2 => playerTowerData.stars_2_reward - playerTowerData.stars_1_reward,
                _ => playerTowerData.stars_1_reward
            };
        }

        var getFlags = prevStar == 3 ? playerTowerData.repeat_Reward : flag;
        float getFlags_P = 0f;
        foreach (var character in GameManager.Instance.Expedition)
        {
            if (character != null && character.skillData != null)
            {
                getFlags += character.skillData.clearFlag;
                getFlags_P += character.skillData.clearFlag_P;
            }
        }
        GameManager.Instance.StageClear(GameManager.Instance.SelectedStageID, star, getFlags);
        audioSource.PlayOneShot(audioWin);
        uiOnStage.windowStagePause.Victory(star, getFlags);
    }


    public void InitSummonButton()
    {
        for (int i = 0; i < uiOnStage.buttonSummons.Length; i++)
        {
            uiOnStage.buttonSummons[i].SetData(GameManager.Instance.GetExpedition(i));
            int index = i;
            uiOnStage.buttonSummons[i].button.onClick.AddListener(() =>
            {
                if (isUpgrading)
                {
                    if (uiOnStage.toggleUpgardeDamage.isOn)
                    {
                        if (uiOnStage.buttonSummons[index].outline.enabled)
                        {
                            uiOnStage.buttonSummons[index].UpgradeDamage();
                            uiOnStage.toggleUpgardeDamage.isOn = false;
                            Exp -= uiOnStage.buttonSummons[index].DamageUpgradeExp;
                        }
                    }
                    else if (uiOnStage.toggleUpgardeHP.isOn)
                    {
                        if (uiOnStage.buttonSummons[index].outline.enabled)
                        {
                            uiOnStage.buttonSummons[index].UpgradeHP();
                            uiOnStage.toggleUpgardeHP.isOn = false;
                            Exp -= uiOnStage.buttonSummons[index].HPUpgradeExp;
                        }
                    }
                }
                else
                {
                    if (uiOnStage.buttonSummons[index].cooldown.value <= uiOnStage.buttonSummons[index].cooldown.minValue
                    && playerTower.CanSpawnUnit()
                    && UseGold(uiOnStage.buttonSummons[index].CharacterInfos.unitData.cost))
                    {
                        playerTower.SpawnUnit(uiOnStage.buttonSummons[index].CharacterInfos);
                        uiOnStage.buttonSummons[index].Summoned();
                    }
                }
            });
        }
    }

    private void UpdateUpgradeToggle()
    {
        canUpgradeDamage = false;
        foreach (var buttonsummon in uiOnStage.buttonSummons)
        {
            if (buttonsummon.gameObject.activeSelf
                && exp >= buttonsummon.DamageUpgradeExp
                && buttonsummon.DamageUpgradedCount < buttonsummon.DamageUpgradeMaxCount)
            {
                canUpgradeDamage = true;
            }
        }
        uiOnStage.toggleUpgardeDamage.interactable = canUpgradeDamage&& !IsTutorial;

        canUpgradeHP = false;
        foreach (var buttonsummon in uiOnStage.buttonSummons)
        {
            if (buttonsummon.gameObject.activeSelf
                && exp >= buttonsummon.HPUpgradeExp
                && buttonsummon.HPUpgradedCount < buttonsummon.HPUpgradeMaxCount)
            {
                canUpgradeHP = true;
            }
        }
        uiOnStage.toggleUpgardeHP.interactable = canUpgradeHP && !IsTutorial;

    }
    private void InitUpgradeToggle()
    {
        //Damage
        uiOnStage.toggleUpgardeDamage.onValueChanged.AddListener(x =>
        {
            foreach (var buttonsummon in uiOnStage.buttonSummons)
            {

                buttonsummon.outline.enabled = x && buttonsummon.DamageUpgradedCount < buttonsummon.DamageUpgradeMaxCount && exp >= buttonsummon.DamageUpgradeExp;
                buttonsummon.outline.color = Color.red;
                if(x)
                {
                    if (buttonsummon.gameObject.activeSelf
                    && exp >= buttonsummon.DamageUpgradeExp
                    && buttonsummon.DamageUpgradedCount < buttonsummon.DamageUpgradeMaxCount)
                        buttonsummon.button.interactable = true;
                    else
                        buttonsummon.button.interactable = false;
                }
            }
            isUpgrading = x;
        });

        //HP
        uiOnStage.toggleUpgardeHP.onValueChanged.AddListener(x =>
        {
            foreach (var buttonsummon in uiOnStage.buttonSummons)
            {

                buttonsummon.outline.enabled = x && buttonsummon.HPUpgradedCount < buttonsummon.HPUpgradeMaxCount && exp >= buttonsummon.HPUpgradeExp;
                buttonsummon.outline.color = Color.red;
                if (x)
                {
                    if (buttonsummon.gameObject.activeSelf
                    && exp >= buttonsummon.HPUpgradeExp
                    && buttonsummon.HPUpgradedCount < buttonsummon.HPUpgradeMaxCount)
                        buttonsummon.button.interactable = true;
                    else
                        buttonsummon.button.interactable = false;
                }
                else
                {
                    buttonsummon.button.interactable = true;
                }
            }
            isUpgrading = x;
        });
    }

    private void InitGameSpeedToggle()
    {
        uiOnStage.toggleGameSpeedFast.onValueChanged.AddListener(x =>
        {
            if (x)
            {
                Time.timeScale = gameSpeedValue;
            }
            else
            {
                Time.timeScale = 1f;
            }
            GameManager.Instance.IsFastGameSpeed = x;
            uiOnStage.toggleGameSpeedNormal.isOn = !x;
        });
        uiOnStage.toggleGameSpeedFast.isOn = GameManager.Instance.IsFastGameSpeed;
    }
}
