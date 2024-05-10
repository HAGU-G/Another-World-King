public static class Paths
{
    public static readonly string dataTablePlayer = "DataTables/Player";

    public static readonly string resourcesRaw = "Assets/Resources/";
    public static readonly string asset = ".asset";
    public static readonly string resourcesPlayer = "Scriptable Objects/Player/{0}";
    public static readonly string resourcesEnemy = "Scriptable Objects/Enemy/{0}";
    public static readonly string resourcesPrefabs = "Prefabs/{0}";
}

public static class Tags
{
    public static readonly string tower = "Tower";
    public static readonly string unit = "Unit";
    public static readonly string gameManager = "GameController";
    public static readonly string window = "Window";
    public static readonly string player = "Player";
}

public static class Scenes
{
    public static readonly string devMain = "DevMain";
    public static readonly string devStage = "DevStage";
}

public enum COMBAT_TYPE
{
    STOP_ON_HAVE_TARGET,
    STOP_ON_ATTACK,
    DONT_STOP
}