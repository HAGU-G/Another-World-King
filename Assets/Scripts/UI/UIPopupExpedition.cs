using TMPro;
using UnityEngine;

public class UIPopupExpedition : MonoBehaviour
{
    public RectTransform rectTransform;
    public TextMeshProUGUI textDescription;
    public static readonly string formatDesc = "[{0}]\n{1}\n체력 : {2} / 공격력: {3} / 공격 속도 : {4}\n소환 가격 : {3}골드 /소환 쿨타임 : {4}초";

    public void SetData(UnitData unitData, RectTransform slot)
    {
        var corners = new Vector3[4];
        var myCorners = new Vector3[4];
        slot.GetWorldCorners(corners);

        if (Camera.main.WorldToScreenPoint(corners[3]).x + rectTransform.sizeDelta.x <= Screen.width)
        {
            rectTransform.pivot = new(0f, rectTransform.pivot.y);
            transform.position = (corners[2] + corners[3]) / 2f;
        }
        else if (Camera.main.WorldToScreenPoint(corners[0]).x - rectTransform.sizeDelta.x >= 0)
        {
            rectTransform.pivot = new(1f, rectTransform.pivot.y);
            transform.position = (corners[0] + corners[1]) / 2f;
        }
        else
        {
            rectTransform.pivot = new(1f, rectTransform.pivot.y);
            var screenRight = Camera.main.ScreenToWorldPoint(new(Screen.width, 0f));
            transform.position = new(screenRight.x, (corners[2].y + corners[3].y) / 2f, 0f);
        }

        textDescription.text = string.Format(formatDesc,
            DataTableManager.GetString(unitData.prefab),
            DataTableManager.GetString(unitData.desc),
            unitData.initHP,
            unitData.initAttackDamage,
            unitData.initAttackSpeed,
            unitData.cost,
            unitData.spawnTime);
    }

    public void Popup(bool value)
    {
        gameObject.SetActive(value);
    }
}
