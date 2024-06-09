using System.Runtime.InteropServices.WindowsRuntime;

public static class AnimatorTriggers
{
    public static readonly string idle = "Idle";
    public static readonly string move = "Move";
    public static readonly string attack = "Attack";
    public static readonly string dead = "Dead";
    public static readonly string cantAct = "CantAct";
}
public static class AudioParameters
{
    public static readonly string musicVolume = "Music";
    public static readonly string sfxVolume = "Sfx";
    public static readonly string uiVolume = "UI";
}

public static class Paths
{
    public static readonly string folderResources = "Assets/Resources/";
    public static readonly string resourcesPlayer = "Scriptable Objects/Player/{0}";
    public static readonly string resourcesStage = "Scriptable Objects/Stage/{0}";
    public static readonly string resourcesEnemy = "Scriptable Objects/Enemy/{0}";
    public static readonly string resourcesSkill = "Scriptable Objects/Skill/{0}";
    public static readonly string resourcesCounter = "Scriptable Objects/Counter/{0}";
    public static readonly string resourcesPrefabs = "Prefabs/{0}";
    public static readonly string resourcesEffects = "Prefabs/Effects/{0}";
    public static readonly string resourcesBackgrounds = "Prefabs/Backgrounds/{0}";
    public static readonly string resourcesImages = "Prefabs/Images/{0}";
    public static readonly string resourcesProjectiles = "Prefabs/Projectiles/{0}";

    public static readonly string resourcesGameManager = "Prefabs/GameController/GameManager";
    public static readonly string resourcesSceneLoadManager = "Prefabs/GameController/SceneLoadManager";

    public static readonly string resourcesPatternTable = "DataTables/Pattern_Table";
    public static readonly string resourcesMonAppareTable = "DataTables/MonAppare_Table";
    public static readonly string resourcesStageTable = "DataTables/Stage_Table";
    public static readonly string resourcesUpgradeTable = "DataTables/Upgrade_Table";
    public static readonly string resourcesStringTable = "DataTables/String_Table";

#if UNITY_EDITOR
    public static readonly string folderScriptableObjects = "Assets/Resources/Scriptable Objects";

    public static readonly string resourcesDebugStat = "Prefabs/Debug/DebugStat";

    public static readonly string resourcesCharTable = "DataTables/Char_Table";
    public static readonly string resourcesMonTable = "DataTables/Mon_Table";
    public static readonly string resourcesSkillTable = "DataTables/Skill_Table";
    public static readonly string resourcesCounterTable = "DataTables/Counter_Table";

    public static readonly string _asset = ".asset";
    public static readonly string _csv = ".csv";
#endif
}

public static class Tags
{
    public static readonly string tower = "Tower";
    public static readonly string unit = "Unit";
    public static readonly string gameManager = "GameController";
    public static readonly string window = "Window";
    public static readonly string stageManager = "StageController";
    public static readonly string uiManager = "UIController";
    public static readonly string touchManager = "TouchController";
    public static readonly string sceneLoadManager = "SceneController";
}

public static class Scenes
{
    public static readonly string main = "Main";
    public static readonly string title = "Title";
    public static readonly string stage = "Stage";
    public static readonly string loading = "Loading";
}

public static class Defines
{
    public static readonly string nonePrefab = "Brave";
    public static readonly string zero = "0";
    public static readonly string victory = "Victory";
    public static readonly string defeat = "Defeat";
    public static readonly string pause = "일시정지";

    public static readonly string playerfrabsStorySkip = "StorySkip";
}

public static class Vectors
{
    public static readonly UnityEngine.Vector3 filpX = new(-1f, 1f, 1f);
}

public static class Colors
{
    public static readonly UnityEngine.Color transparent = new(1f, 1f, 1f, 0f);
}

public static class Effects
{
    public static readonly string effectDrop = "Effect_Drop";
}