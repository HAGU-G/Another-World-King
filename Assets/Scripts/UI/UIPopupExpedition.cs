using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIPopupExpedition : MonoBehaviour, IDeselectHandler
{
    public RectTransform rectTransform;
    public TextMeshProUGUI textDescription;
    public RectTransform clampRect;
    private RectTransform selectedSlot;
    private float minY;
    private float maxY;
    private bool isSlotExpedition;
    private static readonly string formatDesc = "[{0}]\n{1}\n체력 : {2} / 공격력: {3} / 공격 속도 : {4}\n소환 가격 : {5}골드 /소환 쿨타임 : {6}초";

    public void Selected()
    {
        EventSystem.current.SetSelectedGameObject(gameObject);
    }

    public void Awake()
    {
        var clampCorners = new Vector3[4];
        clampRect.GetWorldCorners(clampCorners);
        minY = clampCorners[0].y;
        maxY = clampCorners[1].y;
    }

    public void SetData(UnitData unitData, RectTransform slot, bool isSlotExpedition = false)
    {
        this.isSlotExpedition = isSlotExpedition;
        selectedSlot = slot;
        textDescription.text = string.Format(formatDesc,
            DataTableManager.GetString(unitData.prefab),
            DataTableManager.GetString(unitData.desc),
            unitData.initHP,
            unitData.initAttackDamage,
            unitData.initAttackSpeed,
            unitData.cost,
            unitData.spawnTime);

        var corners = new Vector3[4];
        var myCorners = new Vector3[4];
        selectedSlot.GetWorldCorners(corners);
        rectTransform.GetWorldCorners(myCorners);

        if (isSlotExpedition)
        {
            rectTransform.pivot = new(0.5f, 1f);
            SetPosition(Screen.width / 2f, selectedSlot.position.y, transform.position.z);
        }
        else
        {
            if (corners[3].x + myCorners[3].x - myCorners[0].x <= Screen.width)
            {
                rectTransform.pivot = new(0f, 0.5f);
                SetPosition((corners[2] + corners[3]) / 2f);
            }
            else if (corners[0].x - (myCorners[3].x - myCorners[0].x) >= 0)
            {
                rectTransform.pivot = new(1f, 0.5f);
                SetPosition((corners[0] + corners[1]) / 2f);
            }
            else
            {
                rectTransform.pivot = new(1f, 0.5f);
                var screenRight = new Vector2(Screen.width, 0f);
                SetPosition(new(screenRight.x, (corners[2].y + corners[3].y) / 2f, 0f));
            }
        }
    }

    public void Popup(bool value)
    {
        gameObject.SetActive(value);
    }

    private void Update()
    {
        if (selectedSlot != null)
        {
            SetPosition(
                transform.position.x,
                selectedSlot.position.y,
                transform.position.z);
        }
    }

    private void SetPosition(Vector3 position)
    {
        SetPosition(position.x, position.y, position.z);
    }
    private void SetPosition(float x, float y, float z)
    {
        transform.position = new Vector3(
            x,
            isSlotExpedition ? y : Mathf.Clamp(y, minY, maxY),
            z);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        Popup(false);
    }

    //public void OnPointerDown(PointerEventData eventData)
    //{
    //    Popup(false);
    //}
}
