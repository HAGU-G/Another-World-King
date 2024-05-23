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

    private void Start()
    {
        windowStagePause.Close();
    }
}
