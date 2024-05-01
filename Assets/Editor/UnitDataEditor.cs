using UnityEditor;
using UnityEngine.UIElements;

[CustomEditor(typeof(UnitStats))]
public class UnitDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var stats = target as UnitStats;
        EditorGUILayout.IntField(stats.id);
    }
}
