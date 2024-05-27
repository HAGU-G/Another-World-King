using System.Collections.Generic;
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
    public UIIconDivision iconDivision;
    public GameObject monsterMessage;

    private void Start()
    {
        Refresh();
        buttonNextStage.onClick.AddListener(() => { GameManager.Instance.SelectedStageID++; Refresh(); });
        buttonPrevStage.onClick.AddListener(() => { GameManager.Instance.SelectedStageID--; Refresh(); });
        buttonExpedition.onClick.AddListener(() => { expedition.Open(); Close(); });
        buttonShop.onClick.AddListener(() => { shop.Open(); Close(); });
        buttonPlay.onClick.AddListener(() => { GameManager.Instance.LoadingScene(Scenes.stage); });
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

        int count = 0;
        if (GameManager.Instance.StageClearInfo.ContainsKey(selectedID))
            count = GameManager.Instance.StageClearInfo[selectedID];
        foreach (var star in stars)
        {
            star.isOn = count > 0;
            count--;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
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
    public void MonsterMessageOnOff()
    {
        monsterMessage.SetActive(!monsterMessage.activeSelf);
    }
}
