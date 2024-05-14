using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "스킬 ID", menuName = "데이터 생성/스킬")]
public class SkillData : ScriptableObject
{
    public string ignore;

    public string id;
    public int target;

    public int nesting = 1;
    public int Count { get; private set; }
    public float duration = 1f;
    public bool infinityDuration;
    public float CurrentDuration { get; private set; }
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

    public bool Apply()
    {
        bool applied = false;

        if (Count < nesting)
        {
            Count++;
            applied = true;
        }

        if (doResetDurationOnApply)
        {
            CurrentDuration = 0f;
            applied = true;
        }

        return applied;
    }

    public int UpdateDuration(float deltaTime)
    {
        if ((CurrentDuration += deltaTime) >= duration)
        {
            CurrentDuration = 0f;
            Count--;
        }
        return Count;
    }
}
