using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPopupShop : MonoBehaviour
{
    public RawImage rawImage;
    public TextMeshProUGUI textCost;
    public TextMeshProUGUI textName;
    public TextMeshProUGUI textDescription;
    public TextMeshProUGUI textStats;
    public Button buttonsPurchase;

    public static readonly string formatStats = "공격 {0}/체력 {1}/공격속도 {2}\n소환 가격 {3}골드\n소환 쿨타임 {4}초";

    public void SetData(Rect rect, UISlotCharacterInShop selectSlot)
    {
        rawImage.uvRect = rect;
        textName.text = DataTableManager.GetString(selectSlot.slot.characterInfos.unitData.prefab);
        textDescription.text = DataTableManager.GetString(selectSlot.slot.characterInfos.unitData.desc);
        textStats.text = string.Format(formatStats,
            selectSlot.slot.characterInfos.unitData.initAttackDamage,
            selectSlot.slot.characterInfos.unitData.initHP,
            selectSlot.slot.characterInfos.unitData.initAttackSpeed,
            selectSlot.slot.characterInfos.unitData.cost,
            selectSlot.slot.characterInfos.unitData.spawnTime);
        textCost.text = selectSlot.slot.characterInfos.unitData.cost.ToString();

        if (!selectSlot.IsPurchased && selectSlot.IsUnlocked)
            buttonsPurchase.interactable = true;
        else
            buttonsPurchase.interactable = false;
    }

    public void Popup(bool value)
    {
        gameObject.SetActive(value);
    }
}
