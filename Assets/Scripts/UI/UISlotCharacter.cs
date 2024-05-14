using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UISlotCharacter : MonoBehaviour
{
    public Toggle toggle;
    public TextMeshProUGUI textName;
    public GameObject pos;
    public CharacterInfos characterInfos = new();

    public void SetData(CharacterInfos characterInfos)
    {
        this.characterInfos = characterInfos;
        textName.text = characterInfos.unitData.ignore;
        Instantiate(characterInfos.dress, pos.transform).transform.localScale = Vector3.one * 80f;
    }
}
