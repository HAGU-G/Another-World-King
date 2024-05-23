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

        flags.text = GameManager.Instance.Flags.ToString();
        var stageStringID = DataTableManager.Stages[GameManager.Instance.SelectedStageID].String_ID;
        currentStage.text = DataTableManager.GetString(stageStringID);

        currentStageImage.sprite = Resources.Load<Sprite>(string.Format(Paths.resourcesImages, stageStringID));

        int count = 0;
        if (GameManager.Instance.StageClearInfo.ContainsKey(GameManager.Instance.SelectedStageID))
            count = GameManager.Instance.StageClearInfo[GameManager.Instance.SelectedStageID];
        foreach (var star in stars)
        {
            star.isOn = count > 0;
            count--;
        }
    }


    public void GameSave() => SaveManager.GameSave();
    public void GameLoad() => SaveManager.GameLoad();
    public void GameReset() => SaveManager.GameReset();

}
