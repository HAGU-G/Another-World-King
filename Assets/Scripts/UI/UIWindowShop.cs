using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class UIWindowShop : UIWindow
{
    public ScrollRect scrollRect;
    public UISlotCharacterInShop prefabSlot;
    public ToggleGroup toggleGroup;

    private void Start()
    {
        InitStats[] characters = Resources.LoadAll<InitStats>(string.Format(Paths.resourcesPlayer, string.Empty));
        for (int i = 0; i < characters.Length; i++)
        {
            var characterInfos = new CharacterInfos();
            characterInfos.initStats = characters[i];
            characterInfos.animator = Resources.Load<GameObject>(string.Format(Paths.resourcesPrefabs, characters[i].prefab));

            var slot = Instantiate(prefabSlot, scrollRect.content);
            slot.slot.SetData(characterInfos);
            slot.slot.toggle.group = toggleGroup;
        }

       

    }
}
