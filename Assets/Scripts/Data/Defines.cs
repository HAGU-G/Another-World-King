﻿using Unity.VisualScripting;
using UnityEditor.Search;

public static class AnimatorTriggers
{
    public static readonly string idle = "Idle";
    public static readonly string move = "Move";
    public static readonly string attackNormal = "Attack_Normal";
    public static readonly string attackMagic = "Attack_Magic";
    public static readonly string attackBow = "Attack_Bow";
    public static readonly string dead = "Dead";
}

public static class Paths
{
    public static readonly string folderResources = "Assets/Resources/";
    public static readonly string resourcesPlayer = "Scriptable Objects/Player/{0}";
    public static readonly string resourcesStage = "Scriptable Objects/Stage/{0}";
    public static readonly string resourcesEnemy = "Scriptable Objects/Enemy/{0}";
    public static readonly string resourcesPrefabs = "Prefabs/{0}";

    public static readonly string resourcesGameManager = "Prefabs/GameController/GameManager";

    public static readonly string resourcesPatternTable = "DataTables/Pattern_Table";
    public static readonly string resourcesMonAppareTable = "DataTables/MonAppare_Table";
    public static readonly string resourcesStageTable = "DataTables/Stage_Table";

#if UNITY_EDITOR
    public static readonly string resourcesMonTable = "DataTables/Mon_Table";
    public static readonly string resourcesCharTable = "DataTables/Char_Table";

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
    public static readonly string player = "Player";
}

public static class Scenes
{
    public static readonly string devMain = "DevMain";
    public static readonly string devStage = "DevStage";
}



public static class Strings
{
    public static readonly string nonePrefab = "None";
    public static readonly string dataTableNone = "0";
}

public static class Vectors
{
    public static readonly UnityEngine.Vector3 filpX = new(-1, 1, 1);
}