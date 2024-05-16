using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class UISlotCharacter : MonoBehaviour
{
    public Toggle toggle;
    public TextMeshProUGUI textName;
    public RawImage rawImage;
    public CharacterInfos characterInfos = new();

    public void SetData(CharacterInfos characterInfos)
    {
        this.characterInfos = characterInfos;
        textName.text = characterInfos.unitData.ignore;
        
        if(characterInfos.dress != null)
            rawImage.uvRect = GameObject.FindWithTag(Tags.uiManager).GetComponent<UIManager>().AddSlotRenderers(characterInfos.dress);
        
    }
}
