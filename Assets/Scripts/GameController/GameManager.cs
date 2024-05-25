using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public TouchManager touchManager;
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

    public AudioSource musicPlayer;
    public AudioSource uiSoundPlayer;
    public bool DoneTutorial { get; set; }

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
    public List<int> UnlockedID { get; private set; } = new();
    public List<int> PurchasedID { get; private set; } = new();
    public CharacterInfos[] Expedition { get; private set; } = new CharacterInfos[5];
    private int selectedStageID;
    public int SelectedStageID
    {
        get => selectedStageID;
        set
        {
#if UNITY_EDITOR
            selectedStageID = Mathf.Clamp(value, DataTableManager.MinStageID, DataTableManager.MaxStageID);
#else
            int max = (DataTableManager.MinStageID + StageClearInfo.Count) < DataTableManager.MaxStageID
                ? (DataTableManager.MinStageID + StageClearInfo.Count) : DataTableManager.MaxStageID;
            selectedStageID = Mathf.Clamp(value, DataTableManager.MinStageID + (DoneTutorial ? 1 : 0), max);
#endif
        }
    }
    public Dictionary<int, int> StageClearInfo { get; private set; } = new();
    public string NextScene { get; set; }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        SelectedStageID = DataTableManager.MinStageID;

#if UNITY_EDITOR
        flags = 1000;
#endif
    }
    private void Start()
    {
        if (Instance != this)
            Destroy(gameObject);
    }

    public void LoadingScene(string name)
    {
        SaveManager.GameSave();
        NextScene = name;
        SceneManager.LoadScene(Scenes.loading);
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

    public void SetExpedition(int id, int index)
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

    public void StageClear(int stageID, int star, int flag)
    {
        if (StageClearInfo.ContainsKey(stageID))
            StageClearInfo[stageID] = StageClearInfo[stageID] < star ? star : StageClearInfo[stageID];
        else
            StageClearInfo.Add(stageID, star);



        flags += flag;

        if (DataTableManager.StageUnlockID.ContainsKey(stageID))
        {
            foreach (var charID in DataTableManager.StageUnlockID[stageID])
            {
                if (!UnlockedID.Contains(charID))
                    UnlockedID.Add(charID);
            }
        }
        SaveManager.GameSave();
    }

    public bool AddPurchasedID(CharacterInfos characterInfos)
    {
        if (PurchasedID.Contains(characterInfos.unitData.id))
            return false;

        if (flags >= characterInfos.unitData.price)
        {
            flags -= characterInfos.unitData.price;
            PurchasedID.Add(characterInfos.unitData.id);
            SaveManager.GameSave();
            return true;
        }
        else
        {
            return false;
        }
    }


    public static void PlayUISound(AudioClip clip)
    {
        Instance.uiSoundPlayer.PlayOneShot(clip);
    }
    public static void PlayMusic(AudioClip clip)
    {
        Instance.musicPlayer.Stop();
        Instance.musicPlayer.clip = clip;
        Instance.musicPlayer.Play();
    }
}
