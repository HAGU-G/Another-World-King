using System.Collections.Generic;
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

    public RectTransform flagRect;
    public TextMeshProUGUI flags;

    public UIPopupShop popup;

    private CharacterInfos select;
    private UISlotCharacterInShop selectSlot;

    private void Awake()
    {
        var popupCorners = new Vector3[4];
        var FlagCorners = new Vector3[4];
        popup.GetComponent<RectTransform>().GetWorldCorners(popupCorners);
        flagRect.GetWorldCorners(FlagCorners);
        if (popupCorners[2].x > FlagCorners[0].x)
        {
            popup.transform.position += Vector3.right * (FlagCorners[0].x - popupCorners[2].x);
        }

    }

    private void Start()
    {
        ClosePopup();
        buttonBack.onClick.AddListener(() => { uiMain.Open(); Close(); ClosePopup(); });
        cancel.onClick.AddListener(ClosePopup);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GameManager.Instance.PlayAudioBack();
            if (popup.gameObject.activeSelf)
            {
                ClosePopup();
            }
            else
            {
                uiMain.Open();
                Close();
            }
        }
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

        LoadSlots();

    }

    public void ClosePopup()
    {
        if (selectSlot != null)
            selectSlot.slot.toggle.isOn = false;
        else
            popup.Popup(false);
    }

    public void LoadSlots()
    {
        for (int i = 0; i < scrollRect.content.childCount; i++)
        {
            select = null;
            selectSlot = null;
            Destroy(scrollRect.content.GetChild(i).gameObject);
        }

        List<int> unlockSort = new();

        foreach (var stage in DataTableManager.Stages)
        {
            if (!unlockSort.Contains(stage.Value.Reward_Char1))
                unlockSort.Add(stage.Value.Reward_Char1);
            if (!unlockSort.Contains(stage.Value.Reward_Char2))
                unlockSort.Add(stage.Value.Reward_Char2);
            if (!unlockSort.Contains(stage.Value.Reward_Char3))
                unlockSort.Add(stage.Value.Reward_Char3);
            if (!unlockSort.Contains(stage.Value.Reward_Char4))
                unlockSort.Add(stage.Value.Reward_Char4);
        }

        foreach (var charID in unlockSort)
        {
            if (charID == 0)
                continue;

            UnitData character = Resources.Load<UnitData>(string.Format(Paths.resourcesPlayer, charID));
            var characterInfos = new CharacterInfos();
            characterInfos.SetData(character);

            var slot = Instantiate(prefabSlot, scrollRect.content);

            if (GameManager.Instance.UnlockedID.Contains(character.id))
                slot.Unlock();

            if (GameManager.Instance.PurchasedID.Contains(character.id))
                slot.SoldOut();

            slot.SetData(characterInfos);
            slot.slot.toggle.group = toggleGroup;
            slot.slot.toggle.onValueChanged.AddListener((x) =>
            {
                if (x)
                {
                    select = slot.slot.characterInfos;
                    selectSlot = slot;
                    popup.SetData(selectSlot);
                }
                else if (selectSlot == slot)
                {
                    select = null;
                    selectSlot = null;
                }
                popup.Popup(x);
            });

        }


        //UnitData[] characters = Resources.LoadAll<UnitData>(string.Format(Paths.resourcesPlayer, string.Empty));
        //for (int i = 0; i < characters.Length; i++)
        //{
        //    var characterInfos = new CharacterInfos();
        //    characterInfos.SetData(characters[i]);

        //    var slot = Instantiate(prefabSlot, scrollRect.content);

        //    if (GameManager.Instance.UnlockedID.Contains(characters[i].id))
        //        slot.Unlock();

        //    if (GameManager.Instance.PurchasedID.Contains(characters[i].id))
        //        slot.SoldOut();

        //    slot.SetData(characterInfos);
        //    slot.slot.toggle.group = toggleGroup;
        //    slot.slot.toggle.onValueChanged.AddListener((x) =>
        //    {
        //        if (x)
        //        {
        //            select = slot.slot.characterInfos;
        //            selectSlot = slot;
        //            popup.SetData(selectSlot.slot.rawImage.uvRect, selectSlot);
        //        }
        //        else if (selectSlot == slot)
        //        {
        //            select = null;
        //            selectSlot = null;
        //        }
        //        popup.Popup(x);
        //    });
        //}
    }
}
