using System.Collections;
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

    public TextMeshProUGUI currentStage;

    private void Start()
    {
        Refresh();
        buttonNextStage.onClick.AddListener(() => { GameManager.Instance.SelectedStageID++; Refresh(); });
        buttonPrevStage.onClick.AddListener(() => { GameManager.Instance.SelectedStageID--; Refresh(); });
        buttonExpedition.onClick.AddListener(() => { expedition.Open(); Close(); });
        buttonShop.onClick.AddListener(() => { shop.Open(); Close(); });
        buttonPlay.onClick.AddListener(() => { GameManager.Instance.ChangeScene(Scenes.devStage); });
    }

    public override void Refresh()
    {
        currentStage.text = (GameManager.Instance.SelectedStageID - 100).ToString();
    }
}
