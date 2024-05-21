using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIShopPopup : MonoBehaviour
{
    public RawImage rawImage;
    public TextMeshProUGUI textCost;
    public TextMeshProUGUI textName;
    public TextMeshProUGUI textDescription;
    public TextMeshProUGUI textStats;
    public static readonly string formatStats = "공격 {0}/체력 {1}/공격속도 {2}\n소환 가격 {3}골드\n소환 쿨타임 {4}초";

    public void SetData(Rect rect, CharacterInfos characterInfos)
    {
        rawImage.uvRect = rect;
        textName.text = DataTableManager.GetString(characterInfos.unitData.prefab);
        textDescription.text = DataTableManager.GetString(characterInfos.unitData.desc);
        textStats.text = string.Format(formatStats,
            characterInfos.unitData.initAttackDamage,
            characterInfos.unitData.initHP,
            characterInfos.unitData.initAttackSpeed,
            characterInfos.unitData.cost,
            characterInfos.unitData.spawnTime);
        textCost.text = characterInfos.unitData.cost.ToString();
    }

    public void Popup(bool value)
    {
        gameObject.SetActive(value);
    }
}
