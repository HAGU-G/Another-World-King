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
        Undo.RecordObject(skill, "��ų ���� ����");

        //ID
        skill.id = EditorGUILayout.TextField("ID", skill.id);

        //���� ��Ģ
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("���� ��Ģ");
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.Space(10, false);
        EditorGUILayout.BeginVertical();
        skill.duration = Mathf.Clamp(EditorGUILayout.FloatField("���� �ð�", skill.duration), 0f, float.MaxValue);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("����� �� ���ӽð� �ʱ�ȭ", GUILayout.MaxWidth(145));
        EditorGUILayout.Space(0);
        skill.doResetDurationOnApply = EditorGUILayout.Toggle(skill.doResetDurationOnApply, GUILayout.Width(15));
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();

        //ü��
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("ü��");
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.Space(10, false);
        EditorGUILayout.BeginVertical();
        skill.hp = EditorGUILayout.IntField("�ִ� ü��", skill.hp);
        skill.hp_P = EditorGUILayout.FloatField("�ִ� ü��%", skill.hp_P);
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();

        //����
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("����");
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.Space(10, false);
        EditorGUILayout.BeginVertical();
        skill.attackDamage = EditorGUILayout.IntField("���ݷ�", skill.attackDamage);
        skill.attackDamage_P = EditorGUILayout.FloatField("���ݷ�%", skill.attackDamage_P);
        skill.attackSpeed = EditorGUILayout.FloatField("���� �ӵ�", skill.attackSpeed);
        skill.attackSpeed_P = EditorGUILayout.FloatField("���� �ӵ�%", skill.attackSpeed_P);
        skill.attackRange = EditorGUILayout.FloatField("���� ����", skill.attackRange);
        skill.attackRange_P = EditorGUILayout.FloatField("���� ����%", skill.attackRange_P);
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();

        //��Ÿ
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("��Ÿ");
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.Space(10, false);
        EditorGUILayout.BeginVertical();
        skill.moveSpeed = EditorGUILayout.FloatField("�̵� �ӵ�", skill.moveSpeed);
        skill.moveSpeed_P = EditorGUILayout.FloatField("�̵� �ӵ�%", skill.moveSpeed_P);
        skill.dropGold = EditorGUILayout.IntField("óġ ���", skill.dropGold);
        skill.dropGold_P = EditorGUILayout.FloatField("óġ ���%", skill.dropGold_P);
        skill.dropExp = EditorGUILayout.IntField("óġ ����ġ", skill.dropExp);
        skill.dropExp_P = EditorGUILayout.FloatField("óġ ����ġ%", skill.dropExp_P);
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();

        PrefabUtility.RecordPrefabInstancePropertyModifications(skill);
        EditorUtility.SetDirty(skill);
    }
}
