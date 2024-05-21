using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UISlotExpedition : MonoBehaviour
{
    public Button button;
    public TextMeshProUGUI textName;
    public CharacterInfos characterInfos;

    public void SetData(CharacterInfos characterInfos)
    {
        if (characterInfos == null)
        {
            this.characterInfos = null;
            textName.text = string.Empty;
        }
        else
        {
            this.characterInfos = characterInfos;
            textName.text = DataTableManager.GetString(characterInfos.unitData.prefab);
        }
    }
}
