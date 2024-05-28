using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIWindowMain : UIWindow
{
    public UIWindow shop;
    public UIWindow expedition;

    public Button buttonExpedition;
    public Button buttonShop;
    public Button buttonNextStage;
    public Button buttonPrevStage;
    public Button buttonPlay;
    public Toggle[] stars;

    public TextMeshProUGUI currentStage;
    public Image currentStageImage;
    public TextMeshProUGUI flags;

    public GameObject iconBoss;
    public UIIconDivision iconDivision;
    public GameObject monsterMessage;

    public GameObject popupGameExit;
    public GameObject popupCharacterUnlock;
    public GameObject unlockCharacters;
    public UISlotCharacter uiSlotCharacter;

    public UIPopupSetting popupSetting;

    private void Start()
    {
        Refresh();
        buttonNextStage.onClick.AddListener(() => { GameManager.Instance.SelectedStageID++; Refresh(); });
        buttonPrevStage.onClick.AddListener(() => { GameManager.Instance.SelectedStageID--; Refresh(); });
        buttonExpedition.onClick.AddListener(() => { expedition.Open(); Close(); });
        buttonShop.onClick.AddListener(() => { shop.Open(); Close(); });
        buttonPlay.onClick.AddListener(() => { GameManager.Instance.LoadingScene(Scenes.stage); });
        if (GameManager.Instance.NewCharacters.Count > 0)
            PopupCharacterUnlockOnOff(true);
    }

    public override void Refresh()
    {
        foreach (var characterInfo in GameManager.Instance.Expedition)
        {
            buttonPlay.interactable = characterInfo != null;
            if (buttonPlay.interactable)
                break;
        }

        var selectedID = GameManager.Instance.SelectedStageID;

        flags.text = GameManager.Instance.Flags.ToString();
        var stageStringID = DataTableManager.Stages[selectedID].String_ID;
        currentStage.text = DataTableManager.GetString(stageStringID);

        currentStageImage.sprite = Resources.Load<Sprite>(string.Format(Paths.resourcesImages, stageStringID));
        SetMostManyMonster(selectedID);

        if (DataTableManager.Stages[selectedID].Boss_ID != 0)
            iconBoss.SetActive(true);
        else
            iconBoss.SetActive(false);

        int count = 0;
        if (GameManager.Instance.StageClearInfo.ContainsKey(selectedID))
            count = GameManager.Instance.StageClearInfo[selectedID];
        foreach (var star in stars)
        {
            star.isOn = count > 0;
            count--;
        }

        if(selectedID == DataTableManager.MinStageID)
            buttonPrevStage.interactable = false;
        else
            buttonPrevStage.interactable = true;

        if (selectedID == DataTableManager.MaxStageID)
            buttonNextStage.interactable = false;
        else
            buttonNextStage.interactable = true;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GameManager.Instance.PlayAudioBack();
            if (popupSetting.gameObject.activeSelf)
            {
                popupSetting.PopupOnOff(false);
            }
            else
            {
                PopupGameExitOnOff(!popupGameExit.activeSelf); 
            }
        }
    }

    private void SetMostManyMonster(int selectedID)
    {
        if (DataTableManager.MonsterAppares.ContainsKey(selectedID))
        {
            iconDivision.gameObject.SetActive(true);
            Dictionary<int, int> monsters = new();
            for (int i = 0; i < 3; i++)
            {
                int appareID = selectedID + i * 100;
                if (!DataTableManager.MonsterAppares.ContainsKey(appareID))
                    continue;
                foreach (var patternSet in DataTableManager.MonsterAppares[appareID].PatternSets)
                {
                    if (monsters.ContainsKey(DataTableManager.Patterns[patternSet.pattern].Monster_1))
                        monsters[DataTableManager.Patterns[patternSet.pattern].Monster_1] += patternSet.weight;
                    else
                        monsters.Add(DataTableManager.Patterns[patternSet.pattern].Monster_1, patternSet.weight);

                    if (monsters.ContainsKey(DataTableManager.Patterns[patternSet.pattern].Monster_2))
                        monsters[DataTableManager.Patterns[patternSet.pattern].Monster_2] += patternSet.weight;
                    else
                        monsters.Add(DataTableManager.Patterns[patternSet.pattern].Monster_2, patternSet.weight);

                    if (monsters.ContainsKey(DataTableManager.Patterns[patternSet.pattern].Monster_3))
                        monsters[DataTableManager.Patterns[patternSet.pattern].Monster_3] += patternSet.weight;
                    else
                        monsters.Add(DataTableManager.Patterns[patternSet.pattern].Monster_3, patternSet.weight);
                }
            }

            int monsterID = 0;
            int mostWeight = 0;
            foreach (var monster in monsters)
            {
                if (monster.Key == 0)
                    continue;
                if (monster.Value > mostWeight)
                {
                    monsterID = monster.Key;
                    mostWeight = monster.Value;
                }
                //Debug.Log($"{monster.Key} {monster.Value}");
            }

            if (monsterID != 0)
                iconDivision.SetDivision(Resources.Load<UnitData>(string.Format(Paths.resourcesEnemy, monsterID)).division);
        }
        else
        {
            iconDivision.gameObject.SetActive(false);
        }
    }
    public void MonsterMessageOnOff(bool value)
    {
        monsterMessage.SetActive(value);
    }

    public void GameExit()
    {
        Application.Quit();
    }

    public void PopupGameExitOnOff(bool value)
    {
        popupGameExit.SetActive(value);
    }

    public void PopupCharacterUnlockOnOff(bool value)
    {
        popupCharacterUnlock.SetActive(value);
        if (value)
        {
            foreach (var newCharacter in GameManager.Instance.NewCharacters)
            {
                var slotNewCharacter = Instantiate(uiSlotCharacter, unlockCharacters.transform);
                CharacterInfos newCharacterInfos = new CharacterInfos();
                newCharacterInfos.SetData(Resources.Load<UnitData>(string.Format(Paths.resourcesPlayer, newCharacter)));
                slotNewCharacter.SetData(newCharacterInfos);
                slotNewCharacter.ViewUnderSlotName();
                slotNewCharacter.toggle.interactable = false;
            }
            GameManager.Instance.NewCharacters.Clear();
        }
    }
}
