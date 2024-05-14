using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SkillData))]
public class BuffDataEditor : Editor
{
    private SkillData skill = null;

    private void OnEnable()
    {
        skill = target as SkillData;
    }

    public override void OnInspectorGUI()
    {
        Undo.RecordObject(skill, "스킬 정보 변경");

        //ID
        skill.id = EditorGUILayout.TextField("ID", skill.id);

        //적용 규칙
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("적용 규칙");
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.Space(10, false);
        EditorGUILayout.BeginVertical();
        skill.duration = Mathf.Clamp(EditorGUILayout.FloatField("지속 시간", skill.duration), 0f, float.MaxValue);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("적용될 때 지속시간 초기화", GUILayout.MaxWidth(145));
        EditorGUILayout.Space(0);
        skill.doResetDurationOnApply = EditorGUILayout.Toggle(skill.doResetDurationOnApply, GUILayout.Width(15));
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();

        //체력
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("체력");
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.Space(10, false);
        EditorGUILayout.BeginVertical();
        skill.hp = EditorGUILayout.IntField("최대 체력", skill.hp);
        skill.hp_P = EditorGUILayout.FloatField("최대 체력%", skill.hp_P);
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();

        //공격
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("공격");
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.Space(10, false);
        EditorGUILayout.BeginVertical();
        skill.attackDamage = EditorGUILayout.IntField("공격력", skill.attackDamage);
        skill.attackDamage_P = EditorGUILayout.FloatField("공격력%", skill.attackDamage_P);
        skill.attackSpeed = EditorGUILayout.FloatField("공격 속도", skill.attackSpeed);
        skill.attackSpeed_P = EditorGUILayout.FloatField("공격 속도%", skill.attackSpeed_P);
        skill.attackRange = EditorGUILayout.FloatField("공격 범위", skill.attackRange);
        skill.attackRange_P = EditorGUILayout.FloatField("공격 범위%", skill.attackRange_P);
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();

        //기타
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("기타");
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.Space(10, false);
        EditorGUILayout.BeginVertical();
        skill.moveSpeed = EditorGUILayout.FloatField("이동 속도", skill.moveSpeed);
        skill.moveSpeed_P = EditorGUILayout.FloatField("이동 속도%", skill.moveSpeed_P);
        skill.dropGold = EditorGUILayout.IntField("처치 골드", skill.dropGold);
        skill.dropGold_P = EditorGUILayout.FloatField("처치 골드%", skill.dropGold_P);
        skill.dropExp = EditorGUILayout.IntField("처치 경험치", skill.dropExp);
        skill.dropExp_P = EditorGUILayout.FloatField("처치 경험치%", skill.dropExp_P);
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();

        PrefabUtility.RecordPrefabInstancePropertyModifications(skill);
        EditorUtility.SetDirty(skill);
    }
}
