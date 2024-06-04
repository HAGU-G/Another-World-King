using System.Collections.Generic;

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
    public Dictionary<int,int> stageClearInfo = new();

    /// <summary>
    /// index, characterID
    /// </summary>
    public Dictionary<int,int> expedition = new();
    public int selectedStageID;

    public override Save VersionUp()
    {
        throw new System.NotImplementedException();
    }

    public override Save VersionDown()
    {
        throw new System.NotImplementedException();
    }
}