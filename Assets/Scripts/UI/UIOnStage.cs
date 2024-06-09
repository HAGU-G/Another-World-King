using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIOnStage : MonoBehaviour
{
    public Canvas canvas;
    public UIButtonSummon[] buttonSummons;
    public TowerAI playerTower;

    public RectTransform costRoot;
    public TextMeshProUGUI textGold;
    public TextMeshProUGUI textExp;

    public Toggle toggleUpgardeDamage;
    public Toggle toggleUpgardeHP;
    public UIWindowStagePause windowStagePause;
    public Button pause;
    public Toggle toggleGameSpeedFast;
    public Toggle toggleGameSpeedNormal;
    public Toggle toggleUnitInfo;

    public UIMapSlider mapSlider;
    public RectTransform mapSliderPosition2;

    private void Start()
    {
        windowStagePause.Close();

        var mapSliderRectTransform = mapSlider.GetComponent<RectTransform>();
        var mapSliderCorners = new Vector3[4];
        mapSliderRectTransform.GetWorldCorners(mapSliderCorners);
        var costRootCorners = new Vector3[4];
        costRoot.GetWorldCorners(costRootCorners);

        if (mapSliderCorners[0].x < costRootCorners[3].x)
        {
            var stageManager = StageManager.Instance;
            mapSlider.transform.position = mapSliderPosition2.position;
            var sizeX = (Camera.main.WorldToScreenPoint(stageManager.enemyTower.GetComponentInChildren<HealthBar>().leftPosition.position).x
                - Camera.main.WorldToScreenPoint(stageManager.playerTower.GetComponentInChildren<HealthBar>().rightPosition.position).x)
                / canvas.scaleFactor * 0.95f;
            mapSliderRectTransform.sizeDelta = new(sizeX, mapSliderRectTransform.sizeDelta.y);
        }
    }
}
