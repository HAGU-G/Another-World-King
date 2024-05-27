using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UISlotCharacter : MonoBehaviour
{
    public Toggle toggle;
    public TextMeshProUGUI textName;
    public TextMeshProUGUI textNameUnderSlot;

    public RawImage rawImage;
    public CharacterInfos characterInfos;

    public void SetData(CharacterInfos characterInfos)
    {
        this.characterInfos = characterInfos;
        var name = DataTableManager.GetString(characterInfos.unitData.prefab);
        textName.text = textNameUnderSlot.text = name;

        if (characterInfos.dress != null)
        {
            rawImage.uvRect = GameObject.FindWithTag(Tags.uiManager).GetComponent<UIManager>().AddSlotRenderers(characterInfos.dress);
        }
    }

    public void ViewUnderSlotName()
    {
        textNameUnderSlot.gameObject.SetActive(true);
    }
}
