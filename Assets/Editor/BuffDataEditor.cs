using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BuffData))]
public class BuffDataEditor : Editor
{
    private BuffData buff = null;

    private void OnEnable()
    {
        buff = target as BuffData;
    }

    public override void OnInspectorGUI()
    {
        Undo.RecordObject(buff, "버프 정보 변경");

        //ID
        buff.id = EditorGUILayout.IntField("ID", buff.id);

        //적용 규칙
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("적용 규칙");
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.Space(10, false);
        EditorGUILayout.BeginVertical();
        buff.duration = Mathf.Clamp(EditorGUILayout.FloatField("지속 시간", buff.duration), 0f, float.MaxValue);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("적용될 때 지속시간 초기화",GUILayout.MaxWidth(145));
        EditorGUILayout.Space(0);
        buff.doResetDurationOnApply = EditorGUILayout.Toggle(buff.doResetDurationOnApply,GUILayout.Width(15));
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();

        //체력
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("체력");
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.Space(10, false);
        EditorGUILayout.BeginVertical();
        buff.hp = EditorGUILayout.IntField("최대 체력", buff.hp);
        buff.hp_P = EditorGUILayout.FloatField("최대 체력%", buff.hp_P);
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();

        //공격
        EditorGUILayout.Space(10);
        EditorGUILayout.PrefixLabel("공격");
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.Space(10, false);
        EditorGUILayout.BeginVertical();
        buff.attackDamage = EditorGUILayout.IntField("공격력", buff.attackDamage);
        buff.attackDamage_P = EditorGUILayout.FloatField("공격력%", buff.attackDamage_P);
        buff.attackRange = EditorGUILayout.FloatField("공격 속도", buff.attackRange);
        buff.attackRange_P = EditorGUILayout.FloatField("공격 속도%", buff.attackRange_P);
        buff.attackSpeed = EditorGUILayout.FloatField("공격 범위", buff.attackSpeed);
        buff.attackSpeed_P = EditorGUILayout.FloatField("공격 범위%", buff.attackSpeed_P);
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();

        //기타
        EditorGUILayout.Space(10);
        EditorGUILayout.PrefixLabel("기타");
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.Space(10, false);
        EditorGUILayout.BeginVertical();
        buff.dropGold = EditorGUILayout.IntField("처치 골드", buff.dropGold);
        buff.dropGold_P = EditorGUILayout.FloatField("처치 골드%", buff.dropGold_P);
        buff.dropExp = EditorGUILayout.IntField("처치 경험치", buff.dropExp);
        buff.dropExp_P = EditorGUILayout.FloatField("처치 경험치%", buff.dropExp_P);
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();

        PrefabUtility.RecordPrefabInstancePropertyModifications(buff);
        EditorUtility.SetDirty(buff);
    }
}
