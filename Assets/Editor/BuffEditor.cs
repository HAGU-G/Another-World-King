using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Buff))]
public class BuffEditor : Editor
{
    private Buff buff = null;

    private void OnEnable()
    {
        buff = target as Buff;
    }

    public override void OnInspectorGUI()
    {
        Undo.RecordObject(buff, "���� ���� ����");

        //ID
        buff.id = EditorGUILayout.IntField("ID", buff.id);

        //ü��
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("ü��");
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.Space(10,false);
        EditorGUILayout.BeginVertical();
        buff.hp = EditorGUILayout.IntField("ü��", buff.hp);
        buff.hp_P = EditorGUILayout.FloatField("ü��%", buff.hp_P);
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();

        //����
        EditorGUILayout.Space(10);
        EditorGUILayout.PrefixLabel("����");
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.Space(10, false);
        EditorGUILayout.BeginVertical();
        buff.attackDamage = EditorGUILayout.IntField("���ݷ�", buff.attackDamage);
        buff.attackDamage_P = EditorGUILayout.FloatField("���ݷ�%", buff.attackDamage_P);
        buff.attackRange = EditorGUILayout.FloatField("���� �ӵ�", buff.attackRange);
        buff.attackRange_P = EditorGUILayout.FloatField("���� �ӵ�%", buff.attackRange_P);
        buff.attackSpeed = EditorGUILayout.FloatField("���� ����", buff.attackSpeed);
        buff.attackSpeed_P = EditorGUILayout.FloatField("���� ����%", buff.attackSpeed_P);
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();

        //��Ÿ
        EditorGUILayout.Space(10);
        EditorGUILayout.PrefixLabel("��Ÿ");
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.Space(10, false);
        EditorGUILayout.BeginVertical();
        buff.dropGold = EditorGUILayout.IntField("óġ ���", buff.dropGold);
        buff.dropGold_P = EditorGUILayout.FloatField("óġ ���%", buff.dropGold_P);
        buff.dropExp = EditorGUILayout.IntField("óġ ����ġ", buff.dropExp);
        buff.dropExp_P = EditorGUILayout.FloatField("óġ ����ġ%", buff.dropExp_P);
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();

        PrefabUtility.RecordPrefabInstancePropertyModifications(buff);
        EditorUtility.SetDirty(buff);
    }
}
