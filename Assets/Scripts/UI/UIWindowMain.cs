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
    }
}
