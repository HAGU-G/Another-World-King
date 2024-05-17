using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIButtonSummon : MonoBehaviour
{
    public TextMeshProUGUI cost;
    public TextMeshProUGUI level;
    public Button button;
    public CharacterInfos CharacterInfos { get; private set; } = new();
    public Slider cooldown;

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
        cooldown.maxValue = CharacterInfos.unitData.spawnTime;
        cooldown.value = 0f;
    }

    private void Update()
    {
        if (cooldown.value > 0f)
            cooldown.value -= Time.deltaTime;
    }

    public void Summoned()
    {
        cooldown.value = cooldown.maxValue;
    }
}
