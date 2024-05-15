using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    #region INSTANCE
    private static GameManager instance;

    public static GameManager Instance
    {
        get
        {
            if (instance == null)
                instance = Instantiate(Resources.Load<GameObject>(Paths.resourcesGameManager)).GetComponent<GameManager>();

            return instance;
        }
    }
    #endregion
    private int flags;
    public int Flags
    {
        get => flags;
        set
        {
            flags = value;
            if (flags < 0)
                flags = 0;
        }
    }
    public List<int> purchasedID { get; private set; } = new();
    public CharacterInfos[] Expedition { get; private set; } = new CharacterInfos[5];
    private int selectedStageID;
    public int SelectedStageID
    {
        get => selectedStageID;
        set
        {
            //int max = (DataTableManager.MinStageID + StageClearInfo.Count) < DataTableManager.MaxStageID
            //    ? (DataTableManager.MinStageID + StageClearInfo.Count) : DataTableManager.MaxStageID;
            //selectedStageID = Mathf.Clamp(value, DataTableManager.MinStageID, max);
            selectedStageID = Mathf.Clamp(value, DataTableManager.MinStageID, DataTableManager.MaxStageID);
        }
    }
    public Dictionary<int, int> StageClearInfo { get; private set; } = new();
    public string nextScene;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        SelectedStageID = DataTableManager.MinStageID;

        //TESTCODE
        purchasedID.Add(1101);
        purchasedID.Add(1102);
        purchasedID.Add(1201);
        purchasedID.Add(1202);
        purchasedID.Add(1301);
        SetExpeditions(1101, 0);
        SetExpeditions(1102, 1);
        SetExpeditions(1201, 2);

        flags = int.MaxValue / 2;
    }
    private void Start()
    {
        if (Instance != this)
            Destroy(gameObject);
    }

    public void LoadingScene(string name)
    {
        nextScene = name;
        SceneManager.LoadScene(Scenes.devLoading);
    }
    public void ChangeScene(string name)
    {
        SceneManager.LoadScene(name);
    }

    public void SetExpedition(CharacterInfos characterInfos, int index = -1)
    {
        if (index < 0)
        {
            for (int i = 0; i < Expedition.Length; i++)
            {
                if (Expedition[i] != null)
                    continue;
                index = i;
                break;
            }
        }

        if (index < 0)
            return;

        Expedition[index] = characterInfos;
    }

    public void SetExpeditions(int id, int index)
    {
        UnitData character = Resources.Load<UnitData>(string.Format(Paths.resourcesPlayer, id));

        var characterInfos = new CharacterInfos();
        characterInfos.SetData(character);
        SetExpedition(characterInfos, index);
    }


    public CharacterInfos GetExpedition(int index)
    {
        return Expedition[index];
    }

    public void StageClear(int index, int star, int flag)
    {
        if (StageClearInfo.ContainsKey(index))
            StageClearInfo[index] = StageClearInfo[index] < star ? star : StageClearInfo[index];
        else
            StageClearInfo.Add(index, star);
        flags += flag;
    }

    public bool AddPurchasedID(CharacterInfos characterInfos)
    {
        if (purchasedID.Contains(characterInfos.unitData.id))
            return false;

        if (flags >= characterInfos.unitData.price)
        {
            flags -= characterInfos.unitData.price;
            purchasedID.Add(characterInfos.unitData.id);
            return true;
        }
        else
        {
            return false;
        }
    }
}
