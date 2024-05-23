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
        price.text = IsPurchased ? "������" : (IsUnlocked ? characterInfos.unitData.price.ToString() : "���");
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
