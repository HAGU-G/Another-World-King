using TMPro;
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
    public Image[] stars;

    public TextMeshProUGUI currentStage;
    public TextMeshProUGUI flags;

    private void Start()
    {
        Refresh();
        buttonNextStage.onClick.AddListener(() => { GameManager.Instance.SelectedStageID++; Refresh(); });
        buttonPrevStage.onClick.AddListener(() => { GameManager.Instance.SelectedStageID--; Refresh(); });
        buttonExpedition.onClick.AddListener(() => { expedition.Open(); Close(); });
        buttonShop.onClick.AddListener(() => { shop.Open(); Close(); });
        buttonPlay.onClick.AddListener(() => { GameManager.Instance.LoadingScene(Scenes.devStage); });
    }

    public override void Refresh()
    {
        flags.text = GameManager.Instance.Flags.ToString();
        currentStage.text = DataTableManager.GetString(DataTableManager.Stages[GameManager.Instance.SelectedStageID].String_ID);

        int count = 0;
        if (GameManager.Instance.StageClearInfo.ContainsKey(GameManager.Instance.SelectedStageID))
            count = GameManager.Instance.StageClearInfo[GameManager.Instance.SelectedStageID];
        foreach (var star in stars)
        {
            if (count > 0)
                star.color = UnityEngine.Color.white;
            else
                star.color = Colors.transparent;
            count--;
        }
    }


    public void GameSave() => SaveManager.GameSave();
    public void GameLoad() => SaveManager.GameLoad();
    public void GameReset() => SaveManager.GameReset();

}
