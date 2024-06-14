using Newtonsoft.Json;
using System.IO;
using UnityEngine;
using System.Security.Cryptography;
using CurrentSaveVersion = SaveV2;

public static class SaveManager
{
    private static readonly string saveDirectory = $"{Application.persistentDataPath}/save";
    public static readonly string saveFile = "save_obt.king";
    private static readonly string key = "fje1f553d54fe3g9";


    public static Save saveData = new CurrentSaveVersion();
    public static byte[] EncryptedSaveData { get; private set; } = null;

    public static void GameSave()
    {
        if (saveData == null)
            saveData = new CurrentSaveVersion();

        var save = saveData as CurrentSaveVersion;
        var gm = GameManager.Instance;

        //Version 1
        save.doneTutorial = gm.IsDoneTutorial;

        save.flags = gm.Flags;
        foreach (var id in gm.UnlockedID)
        {
            if (!save.unlockedID.Contains(id))
                save.unlockedID.Add(id);
        }
        foreach (var id in gm.PurchasedID)
        {
            if (!save.purchasedID.Contains(id))
                save.purchasedID.Add(id);
        }
        foreach (var id in gm.StageClearInfo)
        {
            if (save.stageClearInfo.ContainsKey(id.Key))
                save.stageClearInfo[id.Key] = id.Value;
            else
                save.stageClearInfo.Add(id.Key, id.Value);
        }
        for (int i = 0; i < gm.Expedition.Length; i++)
        {
            if (gm.Expedition[i] == null)
            {
                if (save.expedition.ContainsKey(i))
                {
                    save.expedition.Remove(i);
                    continue;
                }
            }
            else
            {
                if (save.expedition.ContainsKey(i))
                    save.expedition[i] = gm.Expedition[i].unitData.id;
                else
                    save.expedition.Add(i, gm.Expedition[i].unitData.id);
            }
        }
        save.selectedStageID = gm.SelectedStageID;
        
        //Version 2
        save.cumulativeFlags = gm.cumulativeFlags;

        if (!Directory.Exists(saveDirectory))
            Directory.CreateDirectory(saveDirectory);

        var path = Path.Combine(saveDirectory, saveFile);
        using (var stringWriter = new StringWriter())
        {
            using (var jsonWriter = new JsonTextWriter(stringWriter))
            {
                var serializer = new JsonSerializer();
                serializer.Formatting = Formatting.Indented;
                serializer.TypeNameHandling = TypeNameHandling.All;
                serializer.Serialize(jsonWriter, save);
            }

            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(stringWriter.ToString());
            ICryptoTransform cryptoTransform = NewRijndaeManaged().CreateEncryptor();
            EncryptedSaveData = cryptoTransform.TransformFinalBlock(bytes, 0, bytes.Length);
            File.WriteAllBytes(path, EncryptedSaveData);
        }
    }

    public static void GameLoad(byte[] data = null)
    {
        

        if (data == null)
        {
            if (!Directory.Exists(saveDirectory))
                return;

            var path = Path.Combine(saveDirectory, saveFile);
            if (!File.Exists(path))
                return;

            EncryptedSaveData = File.ReadAllBytes(path);
        }
        else
        {
            EncryptedSaveData = data;
        }

        Save loadedSaveData = new CurrentSaveVersion();

        ICryptoTransform cryptoTransform2 = NewRijndaeManaged().CreateDecryptor();
        byte[] result = cryptoTransform2.TransformFinalBlock(EncryptedSaveData, 0, EncryptedSaveData.Length);
        using (var reader = new JsonTextReader(new StringReader(System.Text.Encoding.UTF8.GetString(result))))
        {
            var serializer = new JsonSerializer();
            serializer.Formatting = Formatting.Indented;
            serializer.TypeNameHandling = TypeNameHandling.All;
            loadedSaveData = serializer.Deserialize<Save>(reader);
        }
        
        while (loadedSaveData.Version != saveData.Version)
        {
            if(loadedSaveData.Version < saveData.Version)
            {
                loadedSaveData = loadedSaveData.VersionUp();
            }
            else if(loadedSaveData.Version > saveData.Version)
            {
                loadedSaveData = loadedSaveData.VersionDown();
            }
        }

        saveData = loadedSaveData;
        
        var load = saveData as CurrentSaveVersion;
        var gameManager = GameManager.Instance;

        //Version 1
        gameManager.IsDoneTutorial = load.doneTutorial;
        gameManager.Flags = load.flags;

        gameManager.UnlockedID.Clear();
        foreach (var item in load.unlockedID)
        {
            gameManager.UnlockedID.Add(item);
        }

        gameManager.PurchasedID.Clear();
        foreach (var item in load.purchasedID)
        {
            gameManager.PurchasedID.Add(item);
        }

        gameManager.StageClearInfo.Clear();
        foreach (var item in load.stageClearInfo)
        {
            gameManager.StageClearInfo.Add(item.Key, item.Value);
        }

        for (int i = 0; i < gameManager.Expedition.Length; i++)
        {
            if (load.expedition.ContainsKey(i))
                gameManager.SetExpedition(load.expedition[i], i);
            else
                gameManager.SetExpedition(null, i);
        }

        gameManager.SelectedStageID = load.selectedStageID;

        //Version 2
        gameManager.cumulativeFlags = load.cumulativeFlags;
    }

    public static void GameReset()
    {
        saveData = null;

        if (!Directory.Exists(saveDirectory))
            return;
        var path = Path.Combine(saveDirectory, saveFile);
        if (File.Exists(path))
            File.Delete(path);

        GameManager.Instance.Flags = 0;
        GameManager.Instance.UnlockedID.Clear();
        GameManager.Instance.PurchasedID.Clear();
        GameManager.Instance.StageClearInfo.Clear();
        for (int i = 0; i < GameManager.Instance.Expedition.Length; i++)
        {
            GameManager.Instance.SetExpedition(null, i);
        }
    }


    public static RijndaelManaged NewRijndaeManaged()
    {
        byte[] keys = System.Text.Encoding.UTF8.GetBytes(key);
        byte[] newKeys = new byte[keys.Length];
        System.Array.Copy(keys, 0, newKeys, 0, keys.Length);

        RijndaelManaged rijndaelManaged = new RijndaelManaged();
        rijndaelManaged.Key = newKeys;
        rijndaelManaged.Mode = CipherMode.ECB;
        rijndaelManaged.Padding = PaddingMode.PKCS7;

        return rijndaelManaged;
    }

}