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

    public static readonly string formatStats = "���� {0}/ü�� {1}/���ݼӵ� {2}\n��ȯ ���� {3}���\n��ȯ ��Ÿ�� {4}��";

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
