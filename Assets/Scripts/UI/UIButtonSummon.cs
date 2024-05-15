using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIButtonSummon : MonoBehaviour
{
    public TextMeshProUGUI cost;
    public TextMeshProUGUI level;
    public Button button;
    public CharacterInfos CharacterInfos { get; private set; } = new();

    public void SetData(CharacterInfos characterInfos)
    {
        if (characterInfos == null)
        {
            gameObject.SetActive(false);
            return;
        }
        else
        {
            gameObject.SetActive(true);
        }
        CharacterInfos = characterInfos;
        cost.text = CharacterInfos.unitData.cost.ToString();
        level.text = CharacterInfos.unitData.ignore;
    }
}
