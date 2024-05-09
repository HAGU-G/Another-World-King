using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIWindowExpedition : UIWindow
{
    public ScrollRect scrollRect;
    public UICharacterSlot prefabSlot;
    public ToggleGroup toggleGroup;

    public UICharacterSlot[] expedition;

    // Start is called before the first frame update
    private void Start()
    {
        InitStats[] characters = Resources.LoadAll<InitStats>("Scriptable Objects/Player");
        for (int i = 0; i < characters.Length; i++)
        {
            var slot = Instantiate(prefabSlot, scrollRect.content);
            slot.characterInfos.initStats = characters[i];
            slot.characterInfos.animator = Resources.Load<GameObject>($"Animation/{characters[i].prefab}");
            slot.textName.text = characters[i].id.ToString();
            slot.toggle.group = toggleGroup;
            slot.toggle.onValueChanged.AddListener(x => { if (x) { GameManager.Instance.SetExpedition(slot.characterInfos); } Refresh(); });
        }
    }
    public void Refresh()
    {
        for (int i = 0; i < expedition.Length; i++)
        {
            expedition[i].textName.text = GameManager.Instance.GetExpedition(i)?.initStats.id.ToString();
        }
    }
}
