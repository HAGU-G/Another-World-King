using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIOnStage : MonoBehaviour
{
    public UIButtonSummon[] buttonSummons;
    public TowerAI playerTower;
    public TextMeshProUGUI textGold;
    public TextMeshProUGUI textExp;

    public Toggle toggleUpgardeDamage;
    public Toggle toggleUpgardeHP;
    public UIWindowStagePause windowStagePause;
    public Button pause;
    public Toggle toggleGameSpeedFast;
    public Toggle toggleGameSpeedNormal;

    private void Start()
    {
        windowStagePause.Close();
    }
}
