using UnityEngine;
using UnityEngine.UI;

public class UIWindowExpedition : UIWindow
{
    public UIWindow uiMain;
    public ScrollRect scrollRect;
    public UISlotCharacter prefabSlot;
    public ToggleGroup toggleGroup;
    public Button buttonBack;

    public UISlotExpedition[] expeditionSlots;
    public UIPopupExpedition popup;
    public Button popupClose;
    public GameObject counterInfo;

    private UISlotCharacter selectedSlotCharacter;
    private UISlotExpedition selectedSlotExpedition;

    private void Start()
    {
        ClosePopup();
        popupClose.onClick.AddListener(ClosePopup);
        buttonBack.onClick.AddListener(() => { uiMain.Open(); Close(); });

        for (int i = 0; i < expeditionSlots.Length; i++)
        {
            int index = i;
            expeditionSlots[i].button.onClick.AddListener(() => { SelectSlotExpedition(index); });
            expeditionSlots[i].onDeselect += () =>
            {
                if (selectedSlotExpedition == expeditionSlots[index])
                {
                    selectedSlotExpedition = null;
                    ClosePopup();
                }
            };
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            uiMain.Open(); Close();
        }
    }
    public void ClosePopup()
    {
        if (selectedSlotCharacter != null)
            selectedSlotCharacter.toggle.isOn = false;
        else
            popup.Popup(false);
    }

    public void SelectSlotExpedition(int index)
    {
        if (selectedSlotCharacter != null)
        {
            for (int i = 0; i < expeditionSlots.Length; i++)
            {
                if (expeditionSlots[i].characterInfos != null
                    && expeditionSlots[i].characterInfos.unitData.id == selectedSlotCharacter.characterInfos.unitData.id)
                {
                    expeditionSlots[i].SetData(null);
                    GameManager.Instance.SetExpedition(null, i);
                }
            }
            expeditionSlots[index].SetData(selectedSlotCharacter.characterInfos);
            GameManager.Instance.SetExpedition(expeditionSlots[index].characterInfos, index);
        }

        if (selectedSlotExpedition != expeditionSlots[index])
        {
            selectedSlotExpedition = expeditionSlots[index];

            if (selectedSlotCharacter != null)
            {
                selectedSlotCharacter.toggle.isOn = false;
            }
            else if (selectedSlotExpedition.characterInfos != null)
            {
                popup.SetData(selectedSlotExpedition.characterInfos.unitData, selectedSlotExpedition.GetComponent<RectTransform>(), true);
                popup.Popup(true);
            }

        }
        else
        {
            expeditionSlots[index].SetData(null);
            selectedSlotExpedition = null;
            GameManager.Instance.SetExpedition(null, index);
            ClosePopup();
        }

    }

    public override void Refresh()
    {
        base.Refresh();
        ClosePopup();
        counterInfo.SetActive(false);
        var grid = scrollRect.content.GetComponent<GridLayoutGroup>();
        var cellSizeX = (scrollRect.viewport.rect.width - grid.padding.right - grid.padding.left - grid.spacing.x * (grid.constraintCount - 1)) / grid.constraintCount;
        grid.cellSize = new(cellSizeX, cellSizeX);

        for (int i = 0; i < scrollRect.content.childCount; i++)
        {
            selectedSlotCharacter = null;
            selectedSlotExpedition = null;
            Destroy(scrollRect.content.GetChild(i).gameObject);
        }

        UnitData[] characters = Resources.LoadAll<UnitData>(string.Format(Paths.resourcesPlayer, string.Empty));
        for (int i = 0; i < expeditionSlots.Length; i++)
        {
            expeditionSlots[i].SetData(GameManager.Instance.Expedition[i]);
        }
        for (int i = 0; i < characters.Length; i++)
        {
#if !UNITY_EDITOR
            if (!GameManager.Instance.PurchasedID.Contains(characters[i].id))
                continue;
#endif
            var characterInfos = new CharacterInfos();
            characterInfos.SetData(characters[i]);

            var slot = Instantiate(prefabSlot, scrollRect.content);
            slot.SetData(characterInfos);
            slot.toggle.group = toggleGroup;
            slot.toggle.onValueChanged.AddListener(x =>
            {
                if (x)
                {
                    selectedSlotCharacter = slot;
                    popup.SetData(slot.characterInfos.unitData, slot.GetComponent<RectTransform>());
                }
                else
                {
                    selectedSlotCharacter = null;
                }
                popup.Popup(x);
            });
        }

    }

    public void ShowCounterInfo()
    {
        counterInfo.SetActive(!counterInfo.activeSelf);
    }
}
