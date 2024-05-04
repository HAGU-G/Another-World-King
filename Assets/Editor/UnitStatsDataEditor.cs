using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(InitStats))]
public class UnitStatsDataEditor : Editor
{
    private InitStats stats = null;

    private void OnEnable()
    {
        stats = target as InitStats;
    }

    public override void OnInspectorGUI()
    {
        Undo.RecordObject(stats, "유닛 스탯 변경");

        //ID
        stats.id = EditorGUILayout.IntField("ID", stats.id);

        //체력
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("체력");
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.Space(10,false);
        EditorGUILayout.BeginVertical();
        stats.initHP = Mathf.Clamp(EditorGUILayout.IntField("최대 체력", stats.initHP), 0, int.MaxValue);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("시작값 사용", GUILayout.MaxWidth(65));
        stats.useStartHP = EditorGUILayout.Toggle(stats.useStartHP, GUILayout.Width(15));
        if (stats.useStartHP)
            stats.initHPStart = Mathf.Clamp(EditorGUILayout.IntField(stats.initHPStart), 0, stats.initHP);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();

        //공격
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("공격");
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.Space(10, false);
        EditorGUILayout.BeginVertical();
        stats.initAttackDamage = EditorGUILayout.IntField("공격력", stats.initAttackDamage);
        stats.initAttackSpeed = Mathf.Clamp(EditorGUILayout.FloatField("공격 속도", stats.initAttackSpeed),0f,float.MaxValue);
        stats.initAttackRange = Mathf.Clamp(EditorGUILayout.FloatField("공격 범위", stats.initAttackRange),0f,float.MaxValue);
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();

        //힐
        EditorGUILayout.Space(10);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("힐",GUILayout.Width(15));
        stats.isHealer = EditorGUILayout.Toggle(stats.isHealer, GUILayout.Width(15));
        EditorGUILayout.EndHorizontal();
        if (stats.isHealer)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space(10, false);
            EditorGUILayout.BeginVertical();
            stats.initHeal = Mathf.Clamp(EditorGUILayout.IntField("힐량", stats.initHeal), 0, int.MaxValue);
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }

        //기타
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("기타");
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.Space(10, false);
        EditorGUILayout.BeginVertical();
        stats.initMoveSpeed = Mathf.Clamp(EditorGUILayout.FloatField("이동 속도",stats.initMoveSpeed),0f,float.MaxValue);
        stats.cost = Mathf.Clamp(EditorGUILayout.IntField("소환 가격", stats.cost),0,int.MaxValue);
        stats.initDropGold = Mathf.Clamp(EditorGUILayout.IntField("처치 골드", stats.initDropGold),0,int.MaxValue);
        stats.initDropExp = Mathf.Clamp(EditorGUILayout.IntField("처치 경험치", stats.initDropExp),0,int.MaxValue);
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();

        PrefabUtility.RecordPrefabInstancePropertyModifications(stats);
        EditorUtility.SetDirty(stats);
    }
}
