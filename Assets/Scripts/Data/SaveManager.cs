using Newtonsoft.Json;
using System.IO;
using UnityEngine;
using System.Security.Cryptography;
using CurrentSaveVersion = SaveV1;

public static class SaveManager
{
    private static readonly string saveDirectory = $"{Application.persistentDataPath}/save";
    private static readonly string saveFile = "save_obt.king";
    private static readonly string key = "fje1f553d54fe3g9";


    private static Save saveData;

    public static void GameSave()
    {
        if (saveData == null)
            saveData = new CurrentSaveVersion();

        var save = saveData as CurrentSaveVersion;
        var gm = GameManager.Instance;

#if !UNITY_EDITOR
        save.doneTutorial = gm.IsDoneTutorial;
#endif
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
            byte[] result = cryptoTransform.TransformFinalBlock(bytes, 0, bytes.Length);
            File.WriteAllBytes(path, result);
            //TODO 구글 플레이 게임과 연동
        }

    }

    public static void GameLoad()
    {
        saveData = new CurrentSaveVersion();

        
        //TODO 구글 플레이 게임과 연동


        if (!Directory.Exists(saveDirectory))
            return;
        var path = Path.Combine(saveDirectory, saveFile);
        if (!File.Exists(path))
            return;

        byte[] bytes = File.ReadAllBytes(path);

        ICryptoTransform cryptoTransform2 = NewRijndaeManaged().CreateDecryptor();
        byte[] result = cryptoTransform2.TransformFinalBlock(bytes, 0, bytes.Length);
        using (var reader = new JsonTextReader(new StringReader(System.Text.Encoding.UTF8.GetString(result))))
        {
            var serializer = new JsonSerializer();
            serializer.Formatting = Formatting.Indented;
            serializer.TypeNameHandling = TypeNameHandling.All;
            saveData = serializer.Deserialize<Save>(reader);
        }

        var load = saveData as CurrentSaveVersion;

        GameManager.Instance.IsDoneTutorial = load.doneTutorial;
        GameManager.Instance.Flags = load.flags;
        GameManager.Instance.UnlockedID.Clear();
        foreach (var item in load.unlockedID)
        {
            GameManager.Instance.UnlockedID.Add(item);
        }
        GameManager.Instance.PurchasedID.Clear();
        foreach (var item in load.purchasedID)
        {
            GameManager.Instance.PurchasedID.Add(item);
        }
        GameManager.Instance.StageClearInfo.Clear();
        foreach (var item in load.stageClearInfo)
        {
            GameManager.Instance.StageClearInfo.Add(item.Key, item.Value);
        }
        for (int i = 0; i < GameManager.Instance.Expedition.Length; i++)
        {
            if (load.expedition.ContainsKey(i))
                GameManager.Instance.SetExpedition(load.expedition[i], i);
            else
                GameManager.Instance.SetExpedition(null, i);
        }
        GameManager.Instance.SelectedStageID = load.selectedStageID;
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