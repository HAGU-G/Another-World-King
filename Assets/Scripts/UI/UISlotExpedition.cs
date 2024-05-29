using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UISlotExpedition : MonoBehaviour,IDeselectHandler
{
    public Button button;
    public TextMeshProUGUI textName;
    public CharacterInfos characterInfos;
    public UISlotCharacter slot;
    public event System.Action onDeselect;
    public UIIconDivision uiIconDivision;

    public void OnDeselect(BaseEventData eventData)
    {
        if (onDeselect != null)
            onDeselect();
    }

    public void SetData(CharacterInfos characterInfos)
    {
        if (characterInfos == null)
        {
            this.characterInfos = null;
            textName.text = string.Empty;
            slot.gameObject.SetActive(false);
            uiIconDivision.gameObject.SetActive(false);
        }
        else
        {
            slot.gameObject.SetActive(true);
            slot.SetData(characterInfos);
            this.characterInfos = characterInfos;
            textName.text = DataTableManager.GetString(characterInfos.unitData.prefab);
            uiIconDivision.gameObject.SetActive(true);
            uiIconDivision.SetDivision(characterInfos.unitData.division);
        }
    }
}
