using System.Collections.Generic;
using System.Runtime.Serialization;
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
    public List<string> purchasedID { get; private set; } = new();
    public CharacterInfos[] Expedition { get; private set; } = new CharacterInfos[5];
    private int selectedStageID;
    public int SelectedStageID
    {
        get => selectedStageID;
        set
        {
            int max = (DataTableManager.MinStageID + StageClearInfo.Count) < DataTableManager.MaxStageID
                ? (DataTableManager.MinStageID + StageClearInfo.Count) : DataTableManager.MaxStageID;
            selectedStageID = Mathf.Clamp(value, DataTableManager.MinStageID, max);
        }
    }
    public Dictionary<int, int> StageClearInfo { get; private set; } = new();


    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        SelectedStageID = DataTableManager.MinStageID;
    }
    private void Start()
    {
        if (Instance != this)
            Destroy(gameObject);
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
}
