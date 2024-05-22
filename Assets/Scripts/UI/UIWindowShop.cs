using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIWindowShop : UIWindow
{
    public UIWindow uiMain;
    public ScrollRect scrollRect;
    public UISlotCharacterInShop prefabSlot;
    public ToggleGroup toggleGroup;
    public Button buttonBack;
    public Button purchase;
    public Button cancel;
    public TextMeshProUGUI flags;

    public UIPopupShop popup;

    private CharacterInfos select;
    private UISlotCharacterInShop selectSlot;

    private void Start()
    {
        ClosePopup();
        buttonBack.onClick.AddListener(() => { uiMain.Open(); Close(); ClosePopup(); });
        cancel.onClick.AddListener(ClosePopup);

    }

    public void Purchase()
    {
        if (select == null)
            return;

        if (selectSlot.IsUnlocked && !selectSlot.IsPurchased && GameManager.Instance.AddPurchasedID(select))
        {
            selectSlot.SoldOut();
            select = null;
            ClosePopup();
            Refresh();
        }
    }

    public override void Refresh()
    {
        base.Refresh();
        flags.text = GameManager.Instance.Flags.ToString();

        var grid = scrollRect.content.GetComponent<GridLayoutGroup>();
        var cellSizeX = (scrollRect.viewport.rect.width - grid.padding.right - grid.padding.left - grid.spacing.x * (grid.constraintCount - 1)) / grid.constraintCount;
        grid.cellSize = new(cellSizeX, cellSizeX * 1.33f);

        for (int i = 0; i < scrollRect.content.childCount; i++)
        {
            select = null;
            selectSlot = null;
            Destroy(scrollRect.content.GetChild(i).gameObject);
        }

        UnitData[] characters = Resources.LoadAll<UnitData>(string.Format(Paths.resourcesPlayer, string.Empty));
        for (int i = 0; i < characters.Length; i++)
        {
            var characterInfos = new CharacterInfos();
            characterInfos.SetData(characters[i]);

            var slot = Instantiate(prefabSlot, scrollRect.content);

            if (GameManager.Instance.UnlockedID.Contains(characters[i].id))
                slot.Unlock();

            if (GameManager.Instance.PurchasedID.Contains(characters[i].id))
                slot.SoldOut();

            slot.SetData(characterInfos);
            slot.slot.toggle.group = toggleGroup;
            slot.slot.toggle.onValueChanged.AddListener((x) =>
            {
                if (x)
                {
                    select = slot.slot.characterInfos;
                    selectSlot = slot;
                    popup.SetData(selectSlot.slot.rawImage.uvRect, selectSlot);
                }
                else if (selectSlot == slot)
                {
                    select = null;
                    selectSlot = null;
                }
                popup.Popup(x);
            });
        }
    }

    public void ClosePopup()
    {
        if (selectSlot != null)
            selectSlot.slot.toggle.isOn = false;
        else
            popup.Popup(false);
    }
}
