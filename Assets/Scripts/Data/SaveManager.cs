using Newtonsoft.Json;
using System.IO;
using UnityEngine;
using SaveVersionClass = SaveV1;

public static class SaveManager
{
    private static string saveDirectory = $"{Application.persistentDataPath}/save";
    private static string saveFile = "save.king";

    private static Save saveData;

    public static void GameSave()
    {
        if (saveData == null)
            saveData = new SaveVersionClass();

        var save = saveData as SaveVersionClass;
        var gm = GameManager.Instance;

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
        using (var writer = new JsonTextWriter(new StreamWriter(path)))
        {
            var serializer = new JsonSerializer();
            serializer.Formatting = Formatting.Indented;
            serializer.TypeNameHandling = TypeNameHandling.All;
            serializer.Serialize(writer, save);
        }
    }

    public static void GameLoad()
    {
        saveData = new SaveVersionClass();

        if (!Directory.Exists(saveDirectory))
            return;
        var path = Path.Combine(saveDirectory, saveFile);
        if (!File.Exists(path))
            return;

        using (var reader = new JsonTextReader(new StreamReader(path)))
        {
            var serializer = new JsonSerializer();
            serializer.Formatting = Formatting.Indented;
            serializer.TypeNameHandling = TypeNameHandling.All;
            saveData = serializer.Deserialize<Save>(reader);
        }

        var load = saveData as SaveVersionClass;
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


}