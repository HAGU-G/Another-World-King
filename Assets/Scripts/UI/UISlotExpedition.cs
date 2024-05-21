using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UISlotExpedition : MonoBehaviour
{
    public Button button;
    public TextMeshProUGUI textName;
    public CharacterInfos characterInfos;
    public UISlotCharacter slot;

    private void Awake()
    {
        slot.toggle.isOn = true;
    }
    public void SetData(CharacterInfos characterInfos)
    {
        if (characterInfos == null)
        {
            this.characterInfos = null;
            textName.text = string.Empty;
            slot.gameObject.SetActive(false);
        }
        else
        {
            slot.gameObject.SetActive(true);
            slot.SetData(characterInfos);
            this.characterInfos = characterInfos;
            textName.text = DataTableManager.GetString(characterInfos.unitData.prefab);
        }
    }
}
