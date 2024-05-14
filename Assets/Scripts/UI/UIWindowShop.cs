using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.SceneManagement;
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
    public TextMeshProUGUI flags;

    private CharacterInfos select;
    private GameObject selectSlot;

    private void Start()
    {
        buttonBack.onClick.AddListener(() => { uiMain.Open(); Close(); });
        UnitData[] characters = Resources.LoadAll<UnitData>(string.Format(Paths.resourcesPlayer, string.Empty));
        for (int i = 0; i < characters.Length; i++)
        {
            if (characters[i].price <= 0
                || characters[i].id >= 2000
                || GameManager.Instance.purchasedID.Contains(characters[i].id))
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
                    selectSlot = slot.gameObject;
                }
                else if (selectSlot == slot.gameObject)
                {
                    select = null;
                    selectSlot = null;
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
            Destroy(selectSlot);
            select = null;
            Refresh();
        }
    }

    public override void Refresh()
    {
        base.Refresh();
        flags.text = GameManager.Instance.Flags.ToString();
    }
}
