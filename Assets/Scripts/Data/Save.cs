using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine.Rendering;

public abstract class Save
{
    public int Version { get; protected set; }

    public abstract Save VersionUp();
    public abstract Save VersionDown();
}

public class SaveV1 : Save
{
    public SaveV1() => Version = 1;

    public bool doneTutorial;
    public int flags;
    public List<int> unlockedID = new();
    public List<int> purchasedID = new();
    /// <summary>
    /// stageID, star
    /// </summary>
    public Dictionary<int, int> stageClearInfo = new();

    /// <summary>
    /// index, characterID
    /// </summary>
    public Dictionary<int, int> expedition = new();
    public int selectedStageID;

    public override Save VersionUp()
    {
        var saveVersionUp = new SaveV2();
        saveVersionUp.doneTutorial = doneTutorial;
        saveVersionUp.flags = flags;
        saveVersionUp.unlockedID = unlockedID.ConvertAll(x => x);
        saveVersionUp.purchasedID = purchasedID.ConvertAll(x => x);
        saveVersionUp.stageClearInfo = stageClearInfo.ToDictionary(x => x.Key, x => x.Value);
        saveVersionUp.expedition = expedition.ToDictionary(x => x.Key, x => x.Value);
        saveVersionUp.selectedStageID = selectedStageID;

        return saveVersionUp;
    }

    public override Save VersionDown()
    {
        throw new System.NotImplementedException();
    }
}

public class SaveV2 : Save
{
    public SaveV2() => Version = 2;

    public bool doneTutorial;
    public int flags;
    public List<int> unlockedID = new();
    public List<int> purchasedID = new();
    /// <summary>
    /// stageID, star
    /// </summary>
    public Dictionary<int, int> stageClearInfo = new();

    /// <summary>
    /// index, characterID
    /// </summary>
    public Dictionary<int, int> expedition = new();
    public int selectedStageID;
    public int cumulativeFlags;

    public override Save VersionUp()
    {
        throw new System.NotImplementedException();
    }

    public override Save VersionDown()
    {
        var saveVersionDown = new SaveV1();
        saveVersionDown.doneTutorial = doneTutorial;
        saveVersionDown.flags = flags;
        saveVersionDown.unlockedID = unlockedID.ConvertAll(x => x);
        saveVersionDown.purchasedID = purchasedID.ConvertAll(x => x);
        saveVersionDown.stageClearInfo = stageClearInfo.ToDictionary(x => x.Key, x => x.Value);
        saveVersionDown.expedition = expedition.ToDictionary(x => x.Key, x => x.Value);
        saveVersionDown.selectedStageID = selectedStageID;

        return saveVersionDown;
    }
}