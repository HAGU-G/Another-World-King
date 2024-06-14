using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI;

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
            {
                instance = Instantiate(Resources.Load<GameManager>(Paths.resourcesGameManager));
            }

            return instance;
        }
    }
    #endregion

    public AudioSource musicPlayer;
    public AudioSource uiSoundPlayer;
    public AudioClip audioBack;
    public bool IsDoneTutorial { get; set; }
    public bool IsFastGameSpeed { get; set; }
    public bool IsSettingPlayTutorial { get; set; }
    public List<int> PrevExpedition { get; set; } = new();
    public int PrevSelectedStageID { get; set; }

    private int flags;
    public int Flags
    {
        get => flags;
        set
        {
            flags = value;
            if (flags < 0)
            {
                flags = 0;
            }
        }
    }
    public int cumulativeFlags;
    public List<int> UnlockedID { get; private set; } = new();
    public List<int> NewCharacters { get; private set; } = new();
    public List<int> PurchasedID { get; private set; } = new();
    public CharacterInfos[] Expedition { get; private set; } = new CharacterInfos[5];
    private int selectedStageID;
    public int SelectedStageID
    {
        get => selectedStageID;
        set
        {
            selectedStageID = Mathf.Clamp(value, DataTableManager.MinStageID + ((IsDoneTutorial && !IsSettingPlayTutorial) ? 1 : 0), DataTableManager.MaxStageID);
        }
    }
    public Dictionary<int, int> StageClearInfo { get; private set; } = new();

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
        {
            Destroy(gameObject);
        }
    }

    public void SetExpedition(CharacterInfos characterInfos, int index = -1)
    {
        if (index < 0)
        {
            for (int i = 0; i < Expedition.Length; i++)
            {
                if (Expedition[i] != null)
                {
                    continue;
                }
                index = i;
                break;
            }
        }

        if (index < 0)
        {
            return;
        }

        Expedition[index] = characterInfos;
    }

    public void SetExpedition(int id, int index)
    {
        UnitData character = Resources.Load<UnitData>(string.Format(Paths.resourcesPlayer, id));
        if (character == null)
        {
            SetExpedition(null, index);
        }
        else
        {
            var characterInfos = new CharacterInfos();
            characterInfos.SetData(character);
            SetExpedition(characterInfos, index);
        }
    }


    public CharacterInfos GetExpedition(int index)
    {
        return Expedition[index];
    }

    public void StageClear(int stageID, int star, int flag)
    {
        if (StageClearInfo.ContainsKey(stageID))
        {
            StageClearInfo[stageID] = StageClearInfo[stageID] < star ? star : StageClearInfo[stageID];
        }
        else
        {
            StageClearInfo.Add(stageID, star);
        }



        Flags += flag;

        if (DataTableManager.StageUnlockID.ContainsKey(stageID))
        {
            foreach (var charID in DataTableManager.StageUnlockID[stageID])
            {
                if (charID == 0)
                {
                    continue;
                }
                if (!UnlockedID.Contains(charID))
                {
                    UnlockedID.Add(charID);
                    NewCharacters.Add(charID);
                }
            }
        }

        AchievementCheck(stageID, flag);
        SaveManager.GameSave();
    }

    private void AchievementCheck(int stageID, int flag)
    {
        //스테이지 클리어
        switch (stageID)
        {
            case 104:
                GPGSManager.Instance.UnlockAchievement(GPGSIds.achievement_clear3);
                break;
            case 106:
                GPGSManager.Instance.UnlockAchievement(GPGSIds.achievement_clear5);
                break;
            case 108:
                GPGSManager.Instance.UnlockAchievement(GPGSIds.achievement_clear7);
                break;
        }

        //누적 깃발
        if (flag > 0)
        {
            cumulativeFlags += flag;
            GPGSManager.Instance.ReportLeaderBoard(GPGSIds.leaderboard_flags, cumulativeFlags);
        }

        //모든 스테이지 별 3개로 클리어
        if (StageClearInfo.Count != DataTableManager.StageCount)
        {
            return;
        }

        foreach (var info in StageClearInfo)
        {
            if (info.Value != 3)
            {
                return;
            }
        }

        GPGSManager.Instance.UnlockAchievement(GPGSIds.achievement_stars3);

    }

    public bool AddPurchasedID(CharacterInfos characterInfos)
    {
        if (PurchasedID.Contains(characterInfos.unitData.id))
        {
            return false;
        }

        if (flags >= characterInfos.unitData.price)
        {
            flags -= characterInfos.unitData.price;
            PurchasedID.Add(characterInfos.unitData.id);
            if(PurchasedID.Count == DataTableManager.CharacterCount)
            {
                GPGSManager.Instance.UnlockAchievement(GPGSIds.achievement_shopper);
            }
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

    public void PlayAudioBack()
    {
        Instance.uiSoundPlayer.PlayOneShot(audioBack);
    }
}
