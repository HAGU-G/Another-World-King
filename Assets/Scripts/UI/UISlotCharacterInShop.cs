using TMPro;
using UnityEngine;

public class UISlotCharacterInShop : MonoBehaviour
{
    public UISlotCharacter slot;
    public TextMeshProUGUI price;
    public bool IsPurchased { get; private set; }
    public bool IsUnlocked { get; private set; }

    public void SetData(CharacterInfos characterInfos)
    {
        slot.SetData(characterInfos);
        price.text = IsPurchased ? "∫∏¿Ø¡ﬂ" : (IsUnlocked ? characterInfos.unitData.price.ToString() : "¿·±Ë");
    }

    public void SoldOut()
    {
        IsPurchased = true;
    }

    public void Unlock()
    {
        IsUnlocked = true;
    }
}
