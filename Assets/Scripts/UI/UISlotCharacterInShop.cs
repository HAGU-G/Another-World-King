using TMPro;
using UnityEngine;

public class UISlotCharacterInShop : MonoBehaviour
{
    public UISlotCharacter slot;
    public TextMeshProUGUI price;

    public void SetData(CharacterInfos characterInfos)
    {
        slot.SetData(characterInfos);
        price.text = characterInfos.unitData.price.ToString();
    }
}
