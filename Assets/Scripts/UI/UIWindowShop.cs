using System.Collections;
using System.Collections.Generic;
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

    private void Start()
    {
        buttonBack.onClick.AddListener(() => { uiMain.Open(); Close(); });
        UnitData[] characters = Resources.LoadAll<UnitData>(string.Format(Paths.resourcesPlayer, string.Empty));
        for (int i = 0; i < characters.Length; i++)
        {
            var characterInfos = new CharacterInfos();
            characterInfos.unitData = characters[i];
            characterInfos.dress = Resources.Load<GameObject>(string.Format(Paths.resourcesPrefabs, characters[i].prefab));

            var slot = Instantiate(prefabSlot, scrollRect.content);
            slot.slot.SetData(characterInfos);
            slot.slot.toggle.group = toggleGroup;
        }



    }
}
