public static class Tags
{
    public static readonly string tower = "Tower";
    public static readonly string unit = "Unit";
    public static readonly string gameManager = "GameController";
    public static readonly string window = "Window";
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