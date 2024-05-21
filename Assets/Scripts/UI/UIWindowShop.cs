using TMPro;
using Unity.VisualScripting;
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

    public UIShopPopup popup;

    private CharacterInfos select;
    private UISlotCharacterInShop selectSlot;

    private void Start()
    {
        ClosePopup();
        buttonBack.onClick.AddListener(() => { uiMain.Open(); Close(); ClosePopup(); });
        cancel.onClick.AddListener(ClosePopup);
        UnitData[] characters = Resources.LoadAll<UnitData>(string.Format(Paths.resourcesPlayer, string.Empty));
        for (int i = 0; i < characters.Length; i++)
        {
            if (GameManager.Instance.PurchasedID.Contains(characters[i].id)
                || !GameManager.Instance.UnlockedID.Contains(characters[i].id))
                continue;

            var characterInfos = new CharacterInfos();
            characterInfos.SetData(characters[i]);

            var slot = Instantiate(prefabSlot, scrollRect.content);
            slot.SetData(characterInfos);
            slot.slot.toggle.group = toggleGroup;
            slot.slot.toggle.onValueChanged.AddListener((x) =>
            {
                if (x)
                {
                    select = slot.slot.characterInfos;
                    selectSlot = slot;
                    popup.Popup(true);
                    popup.SetData(selectSlot.slot.rawImage.uvRect, selectSlot.slot.characterInfos);
                }
                else if (selectSlot == slot)
                {
                    select = null;
                    selectSlot = null;
                    popup.Popup(false);
                }
            });
        }
    }

    public void Purchase()
    {
        if (select == null)
            return;

        if (GameManager.Instance.AddPurchasedID(select))
        {
            Destroy(selectSlot.gameObject);
            select = null;
            ClosePopup();
            Refresh();
        }
    }

    public override void Refresh()
    {
        base.Refresh();
        flags.text = GameManager.Instance.Flags.ToString();
        
    }

    public void ClosePopup()
    {
        if (selectSlot != null)
            selectSlot.slot.toggle.isOn = false;
        else
            popup.Popup(false);
    }
}
