using UnityEngine;
using UnityEngine.UI;

public class UIWindowExpedition : UIWindow
{
    public ScrollRect scrollRect;
    public UISlotCharacter prefabSlot;
    public ToggleGroup toggleGroup;
    public Button goStage; //TEMP

    public UISlotExpedition[] expedition;

    private UISlotCharacter select;
    private UISlotExpedition selectSlot;

    // Start is called before the first frame update
    private void Start()
    {
        goStage.onClick.AddListener(() => GameManager.Instance.ChangeScene(Scenes.devStage));
        for (int i = 0; i < expedition.Length; i++)
        {
            expedition[i].SetData(GameManager.Instance.Expedition[i]);
        }

        InitStats[] characters = Resources.LoadAll<InitStats>(string.Format(Paths.resourcesPlayer, string.Empty));
        for (int i = 0; i < characters.Length; i++)
        {
            var characterInfos = new CharacterInfos();
            characterInfos.initStats = characters[i];
            characterInfos.animator = Resources.Load<GameObject>(string.Format(Paths.resourcesPrefabs, characters[i].prefab));

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
            int index = i;
            expedition[i].button.onClick.AddListener(() => { SelectSlotExpedition(index); });
        }

    }

    public void SelectSlotExpedition(int index)
    {

        if (select != null)
        {
            foreach (var slot in expedition)
            {
                if (slot.characterInfos == select.characterInfos)
                    slot.SetData(null);
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
                GameManager.Instance.SetExpedition(expedition[index].characterInfos, index);
            }
        }

    }
}