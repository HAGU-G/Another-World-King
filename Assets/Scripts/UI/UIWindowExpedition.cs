using UnityEngine;
using UnityEngine.UI;

public class UIWindowExpedition : UIWindow
{
    public UIWindow uiMain;
    public ScrollRect scrollRect;
    public UISlotCharacter prefabSlot;
    public ToggleGroup toggleGroup;
    public Button buttonBack;

    public UISlotExpedition[] expedition;

    private UISlotCharacter select;
    private UISlotExpedition selectSlot;

    private void Start()
    {
        buttonBack.onClick.AddListener(() => { uiMain.Open(); Close(); });

        for (int i = 0; i < expedition.Length; i++)
        {
            int index = i;
            expedition[i].button.onClick.AddListener(() => { SelectSlotExpedition(index); });
        }

    }

    public void SelectSlotExpedition(int index)
    {

        if (select != null)
        {
            for (int i = 0; i < expedition.Length; i++)
            {
                if (expedition[i].characterInfos != null && expedition[i].characterInfos.unitData.id == select.characterInfos.unitData.id)
                {
                    expedition[i].SetData(null);
                    GameManager.Instance.SetExpedition(null, i);
                }
            }
            expedition[index].SetData(select.characterInfos);
            select.toggle.isOn = false;
            select = null;
            selectSlot = expedition[index];
            GameManager.Instance.SetExpedition(expedition[index].characterInfos, index);
        }
        else
        {
            if (selectSlot != expedition[index])
            {
                selectSlot = expedition[index];
            }
            else
            {
                expedition[index].SetData(null);
                selectSlot = null;
                GameManager.Instance.SetExpedition(null, index);
            }
        }

    }

    public override void Refresh()
    {
        base.Refresh();
        for (int i = 0; i < scrollRect.content.childCount; i++)
        {
            select = null;
            selectSlot = null;
            Destroy(scrollRect.content.GetChild(i).gameObject);
        }

        UnitData[] characters = Resources.LoadAll<UnitData>(string.Format(Paths.resourcesPlayer, string.Empty));
        for (int i = 0; i < characters.Length; i++)
        {
#if !UNITY_EDITOR
            if (!GameManager.Instance.purchasedID.Contains(characters[i].id)
                || characters[i].id >= 2000)
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
                    select = slot;
                    selectSlot = null;
                }
                else
                {
                    select = null;
                }
            });
        }
        for (int i = 0; i < expedition.Length; i++)
        {
            expedition[i].SetData(GameManager.Instance.Expedition[i]);
        }
    }
}
