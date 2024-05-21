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
    public static readonly string formatStats = "���� {0}/ü�� {1}/���ݼӵ� {2}\n��ȯ ���� {3}���\n��ȯ ��Ÿ�� {4}��";

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
