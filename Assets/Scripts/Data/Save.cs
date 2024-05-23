using System.Collections.Generic;

public abstract class Save
{
    public int Version { get; protected set; }
}

public class SaveV1 : Save
{
    public SaveV1() => Version = 1;

    public bool doneTutorial;
    public int flags;
    public List<int> unlockedID = new();
    public List<int> purchasedID = new();
    /// <summary>
    /// id, star
    /// </summary>
    public Dictionary<int,int> stageClearInfo = new();

    /// <summary>
    /// index, id
    /// </summary>
    public Dictionary<int,int> expedition = new();
    public int selectedStageID;
}