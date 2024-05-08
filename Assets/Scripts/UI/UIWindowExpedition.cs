using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIWindowExpedition : UIWindow
{
    public ScrollRect scrollRect;
    public UICharacterSlot prefabSlot;
    public ToggleGroup toggleGroup;

    public TextMeshProUGUI[] expedition;

    // Start is called before the first frame update
    private void Start()
    {
        for (int i = 0; i < 10; i++)
        {
            var slot = Instantiate(prefabSlot, scrollRect.content);
            slot.characterInfos.initStats = Resources.Load<InitStats>("UnitStats_01");
            slot.characterInfos.animator = Resources.Load<GameObject>("Player");
            slot.toggle.group = toggleGroup;
            slot.toggle.onValueChanged.AddListener(x => { if (x) { GameManager.Instance.SetExpedition(slot.characterInfos); } Refresh(); });
        }
    }
    public void Refresh()
    {
        for (int i = 0; i < expedition.Length; i++)
        {
            expedition[i].text = GameManager.Instance.GetExpedition(i)?.initStats.id.ToString();
        }
    }
}
