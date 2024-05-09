using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UISlotCharacter : MonoBehaviour
{
    public Toggle toggle;
    public TextMeshProUGUI textName;
    public CharacterInfos characterInfos = new();

    public void SetData(CharacterInfos characterInfos)
    {
        this.characterInfos = characterInfos;
        textName.text = characterInfos.initStats.id.ToString();
    }
}
