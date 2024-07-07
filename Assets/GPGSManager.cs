using GooglePlayGames.BasicApi;
using GooglePlayGames;
using GooglePlayGames.BasicApi.SavedGame;
using System;
using UnityEngine;


public class GPGSManager : Singleton<GPGSManager>
{
    public bool IsSigned { get; private set; }

    private void Awake()
    {
        if (!IsSigned)
        {
            Init();
        }
    }

    public void Init()
    {
#if UNITY_EDITOR
        PlayGamesPlatform.DebugLogEnabled = true;
#else
        PlayGamesPlatform.DebugLogEnabled = false;
#endif
        PlayGamesPlatform.Activate();
        SignIn();
    }

    public void SignIn()
    {
        PlayGamesPlatform.Instance.Authenticate(OnAuthentication);
    }

    private void OnAuthentication(SignInStatus result)
    {
        IsSigned = result == SignInStatus.Success;
    }

    public void ShowAchievementUI()
    {
        Social.ShowAchievementsUI();
    }

    public void ShowLeaderboard()
    {
        Social.ShowLeaderboardUI();
    }

    public void UnlockAchievement(string achievementID, float progress = 100f)
    {
        Social.ReportProgress(achievementID, progress, (success) => { });
    }

    public void ReportLeaderBoard(string leaderboardID, int value)
    {
        Social.ReportScore(value, leaderboardID, (success) => { });
    }

    public void ShowSelectUI()
    {
        uint maxNumToDisplay = 5;
        bool allowCreateNew = false;
        bool allowDelete = true;

        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
        savedGameClient.ShowSelectSavedGameUI("저장된 게임 선택",
            maxNumToDisplay,
            allowCreateNew,
            allowDelete,
            OnSavedGameSelected);
    }

    public void ShowLeaderboardUI()
    {
        Social.ShowLeaderboardUI();
    }

    public void OnSavedGameSelected(SelectUIStatus status, ISavedGameMetadata game)
    {
        if (status == SelectUIStatus.SavedGameSelected)
        {
            // handle selected game save
            OpenSavedGame(game.Filename, OnSavedGameOpenedForLoad);


        }
        else
        {
            // handle cancel or error
        }
    }

    public void SaveGame()
    {
        SaveManager.GameSave();
        OpenSavedGame(SaveManager.saveFile, OnSavedGameOpenedForSave);
    }

    void OnSavedGameOpenedForSave(SavedGameRequestStatus status, ISavedGameMetadata game)
    {
        if (status == SavedGameRequestStatus.Success && SaveManager.EncryptedSaveData != null)
        {
            SavedGameMetadataUpdate update = new SavedGameMetadataUpdate.Builder()
                .WithUpdatedDescription("저장 일시 : " + DateTime.Now.ToString())
                .Build();

            PlayGamesPlatform.Instance.SavedGame.CommitUpdate(game, update, SaveManager.EncryptedSaveData, OnSavedGameCommit);
        }
        else
        {
        }
    }

    void OnSavedGameCommit(SavedGameRequestStatus commitStatus, ISavedGameMetadata updatedGame)
    {
        if (commitStatus == SavedGameRequestStatus.Success)
        {
        }
        else
        {
        }
    }

    public void LoadGame()
    {
        OpenSavedGame(SaveManager.saveFile, OnSavedGameOpenedForLoad);
    }

    void OnSavedGameOpenedForLoad(SavedGameRequestStatus status, ISavedGameMetadata game)
    {
        if (status == SavedGameRequestStatus.Success)
        {
            PlayGamesPlatform.Instance.SavedGame.ReadBinaryData(game, OnSavedGameDataRead);
        }
        else
        {
        }
    }

    void OnSavedGameDataRead(SavedGameRequestStatus readStatus, byte[] data)
    {
        if (readStatus == SavedGameRequestStatus.Success)
        {
            SaveManager.GameLoad(data);
            string loadedData = System.Text.Encoding.UTF8.GetString(data);
        }
        else
        {
        }
    }

    private void OpenSavedGame(string filename, Action<SavedGameRequestStatus, ISavedGameMetadata> callback)
    {
        PlayGamesPlatform.Instance.SavedGame.OpenWithAutomaticConflictResolution(
            filename,
            DataSource.ReadCacheOrNetwork,
            ConflictResolutionStrategy.UseLongestPlaytime,
            callback);
    }
}
