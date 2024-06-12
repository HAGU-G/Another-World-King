using GooglePlayGames.BasicApi;
using GooglePlayGames;
using GooglePlayGames.BasicApi.SavedGame;
using System;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SocialPlatforms.Impl;


public class GPGSManager : Singleton<GPGSManager>
{
    public TextMeshProUGUI log;

    public bool IsSigned { get; private set; }

    private void Awake()
    {
        if(!IsSigned)
        {
            Init();
        }
    }

    public void Init()
    {
        //PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();
        SignIn();
    }

    public void SignIn()
    {
        PlayGamesPlatform.Instance.Authenticate(OnAuthentication);
    }

    void OnAuthentication(SignInStatus result)
    {
        if (result == SignInStatus.Success)
        {
            log.text = "Signed in successfully.";
            // Signed in successfully, we can now proceed with saving or loading
        }
        else
        {
            log.text = "Failed to sign in.";
        }
        IsSigned = result == SignInStatus.Success;
    }

    public void ShowAchievementUI()
    {
        PlayGamesPlatform.Instance.ShowAchievementsUI();
    }

    public void ShowLeaderboard()
    {

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
            log.text = "Failed to open saved game.";
        }
    }

    void OnSavedGameCommit(SavedGameRequestStatus commitStatus, ISavedGameMetadata updatedGame)
    {
        if (commitStatus == SavedGameRequestStatus.Success)
        {
            log.text = "Game saved successfully.";
        }
        else
        {
            log.text = "Failed to save game.";
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
            log.text = "Failed to open saved game.";
        }
    }

    void OnSavedGameDataRead(SavedGameRequestStatus readStatus, byte[] data)
    {
        if (readStatus == SavedGameRequestStatus.Success)
        {
            SaveManager.GameLoad(data);
            string loadedData = System.Text.Encoding.UTF8.GetString(data);
            log.text = "Game loaded successfully: \n" + loadedData;
        }
        else
        {
            log.text = "Failed to load game.";
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
