using UnityEngine;

public enum TARGET
{
    NONE,
    ENEMY,
    TEAM,
    ONESELF,
}

[CreateAssetMenu(fileName = "��ų ID", menuName = "������ ����/��ų")]
public class SkillData : ScriptableObject
{
    public string ignore;

    public string id;
    public TARGET target;

    public int nesting = 1;
    public float duration;
    public bool infinityDuration;
    public bool doResetDurationOnApply;

    public int hp;
    public float hp_P;

    public int attackDamage;
    public float attackDamage_P;
    public float attackSpeed;
    public float attackSpeed_P;
    public float attackRange;
    public float attackRange_P;

    public float moveSpeed;
    public float moveSpeed_P;

    public int dropGold;
    public float dropGold_P;
    public int dropExp;
    public float dropExp_P;
    public int clearFlag;
    public float clearFlag_P;

    public int onApplyGold;
    public int onApplyExp;
    public int onApplyDrain;

    public bool sturn;
}
