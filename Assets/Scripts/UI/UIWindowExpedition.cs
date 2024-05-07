using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
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
            slot.toggle.group = toggleGroup;
            slot.toggle.onValueChanged.AddListener(x => { if (x) { GameManager.Instance.SetExpedition(slot.initStats); } Refresh(); });
        }
    }
    public void Refresh()
    {
        for (int i = 0; i < expedition.Length; i++)
        {
            expedition[i].text = GameManager.Instance.GetExpedition(i)?.id.ToString();
        }
    }
}
