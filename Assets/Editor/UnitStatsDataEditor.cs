using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(InitStats))]
public class UnitStatsDataEditor : Editor
{
    private InitStats stats = null;
    private SerializedProperty attackEnemyOrder = null;

    private void OnEnable()
    {
        stats = target as InitStats;
        attackEnemyOrder = serializedObject.FindProperty("initAttackEnemyOrder");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        Undo.RecordObject(stats, "유닛 스탯 변경");

        //ID & State
        stats.id = EditorGUILayout.IntField("ID", stats.id);
        stats.isTower = EditorGUILayout.Toggle("타워", stats.isTower);

        //체력
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("체력");
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.Space(10, false);
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

        if (stats.isTower)
            return;

        //공격
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("공격");
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.Space(10, false);
        EditorGUILayout.BeginVertical();
        stats.combatType = (COMBAT_TYPE)EditorGUILayout.EnumPopup("전투 방식", stats.combatType);
        stats.initAttackDamage = EditorGUILayout.IntField("공격력", stats.initAttackDamage);
        stats.initAttackSpeed = Mathf.Clamp(EditorGUILayout.FloatField("공격 속도", stats.initAttackSpeed), 0f, float.MaxValue);
        stats.initAttackRange = Mathf.Clamp(EditorGUILayout.FloatField("공격 범위", stats.initAttackRange), 0f, float.MaxValue);
        stats.initAttackEnemyCount = Mathf.Clamp(EditorGUILayout.IntField("공격 가능한 적의 수", stats.initAttackEnemyCount), 1, int.MaxValue);
        stats.initAttackOrder = Mathf.Clamp(EditorGUILayout.IntField("공격 가능 포지션", stats.initAttackOrder), 1, int.MaxValue);
        EditorGUILayout.PropertyField(attackEnemyOrder, new GUIContent("공격 가능한 적의 포지션", "공격 우선순위 오름차순"));
        if(attackEnemyOrder.arraySize < stats.initAttackEnemyCount)
            EditorGUILayout.HelpBox("공격 가능한 '적의 포지션 수'가 '적의 수'이상이어야 합니다.", MessageType.Warning);
        for (int i = 0; i < attackEnemyOrder.arraySize; i++)
        {
            if (attackEnemyOrder.GetArrayElementAtIndex(i).intValue < 1)
            {
                EditorGUILayout.HelpBox("1 이상의 int값이 필요합니다.", MessageType.Warning);
                break;
            }
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();

        //힐
        EditorGUILayout.Space(10);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("힐", GUILayout.Width(15));
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
        stats.initMoveSpeed = Mathf.Clamp(EditorGUILayout.FloatField("이동 속도", stats.initMoveSpeed), 0f, float.MaxValue);
        stats.cost = Mathf.Clamp(EditorGUILayout.IntField("소환 가격", stats.cost), 0, int.MaxValue);
        stats.initDropGold = Mathf.Clamp(EditorGUILayout.IntField("처치 골드", stats.initDropGold), 0, int.MaxValue);
        stats.initDropExp = Mathf.Clamp(EditorGUILayout.IntField("처치 경험치", stats.initDropExp), 0, int.MaxValue);
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();

        serializedObject.ApplyModifiedProperties();
        PrefabUtility.RecordPrefabInstancePropertyModifications(stats);
        EditorUtility.SetDirty(stats);
    }
}
