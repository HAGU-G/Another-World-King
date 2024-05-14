using System.Diagnostics.Eventing.Reader;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UnitData))]
public class UnitStatsDataEditor : Editor
{
    private UnitData stats = null;
    private SerializedProperty attackEnemyOrder = null;

    private void OnEnable()
    {
        stats = target as UnitData;
        attackEnemyOrder = serializedObject.FindProperty("initAttackEnemyOrder");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        Undo.RecordObject(stats, "���� ���� ����");

        //ID & State
        stats.id = EditorGUILayout.TextField("ID", stats.id);
        stats.prefab = EditorGUILayout.TextField("������", stats.prefab);
        stats.isTower = EditorGUILayout.Toggle("Ÿ��", stats.isTower);

        //ü��
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("ü��");
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.Space(10, false);
        EditorGUILayout.BeginVertical();
        stats.initHP = Mathf.Clamp(EditorGUILayout.IntField("�ִ� ü��", stats.initHP), 0, int.MaxValue);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("���۰� ���", GUILayout.MaxWidth(65));
        stats.useStartHP = EditorGUILayout.Toggle(stats.useStartHP, GUILayout.Width(15));
        if (stats.useStartHP)
            stats.initHPStart = Mathf.Clamp(EditorGUILayout.IntField(stats.initHPStart), 0, stats.initHP);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();

        if (stats.isTower)
            return;

        //����
        EditorGUILayout.Space(10);
        stats.division = (DIVISION)EditorGUILayout.EnumPopup("����", stats.division);
        bool isHealer;
        if (stats.division == DIVISION.HEALER)
            isHealer = true;
        else
            isHealer = false;
        EditorGUILayout.LabelField("����");
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.Space(10, false);
        EditorGUILayout.BeginVertical();
        stats.combatType = (COMBAT_TYPE)EditorGUILayout.EnumPopup("���� ���", stats.combatType);
        if (isHealer)
            stats.initHeal = Mathf.Clamp(EditorGUILayout.IntField("����", stats.initHeal), 0, int.MaxValue);
        else
            stats.initAttackDamage = EditorGUILayout.IntField("���ݷ�", stats.initAttackDamage);
        stats.initAttackSpeed = Mathf.Clamp(EditorGUILayout.FloatField("���� �ӵ�", stats.initAttackSpeed), 0f, float.MaxValue);
        stats.initAttackRange = Mathf.Clamp(EditorGUILayout.FloatField("��Ÿ�", stats.initAttackRange), 0f, float.MaxValue);
        if (!isHealer)
            stats.initAttackEnemyCount = Mathf.Clamp(EditorGUILayout.IntField("���� ������ ���� ��", stats.initAttackEnemyCount), 1, int.MaxValue);
        stats.initAttackOrder = Mathf.Clamp(EditorGUILayout.IntField("���� ���� ������", stats.initAttackOrder), 1, int.MaxValue);
        if (!isHealer)
        {
            EditorGUILayout.PropertyField(attackEnemyOrder, new GUIContent("���� ������ ���� ������", "���� �켱������ ���� ������ ����"));
            if (attackEnemyOrder.arraySize < stats.initAttackEnemyCount)
                EditorGUILayout.HelpBox("���� ������ '���� ������ ��'�� '���� ��'�̻��̾�� �մϴ�.", MessageType.Warning);
            for (int i = 0; i < attackEnemyOrder.arraySize; i++)
            {
                if (attackEnemyOrder.GetArrayElementAtIndex(i).intValue < 1)
                {
                    EditorGUILayout.HelpBox("1 �̻��� int���� �ʿ��մϴ�.", MessageType.Warning);
                    break;
                }
            }
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();



        //��Ÿ
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("��Ÿ");
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.Space(10, false);
        EditorGUILayout.BeginVertical();
        stats.initMoveSpeed = Mathf.Clamp(EditorGUILayout.FloatField("�̵� �ӵ�", stats.initMoveSpeed), 0f, float.MaxValue);
        stats.cost = Mathf.Clamp(EditorGUILayout.IntField("��ȯ ����", stats.cost), 0, int.MaxValue);
        stats.initDropGold = Mathf.Clamp(EditorGUILayout.IntField("óġ ���", stats.initDropGold), 0, int.MaxValue);
        stats.initDropExp = Mathf.Clamp(EditorGUILayout.IntField("óġ ����ġ", stats.initDropExp), 0, int.MaxValue);
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();

        serializedObject.ApplyModifiedProperties();
        PrefabUtility.RecordPrefabInstancePropertyModifications(stats);
        EditorUtility.SetDirty(stats);
    }
}
