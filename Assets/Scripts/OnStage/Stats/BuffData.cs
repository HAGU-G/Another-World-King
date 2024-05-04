using UnityEngine;

[CreateAssetMenu(fileName = "Buff_", menuName = "CreateData/Buff")]
public class BuffData : ScriptableObject
{
    public int id;

    public int overlap = 1;
    public int Count { get; private set; }
    public float duration = 1f;
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

    public void Apply()
    {
        if (Count < overlap)
            Count++;

        if (doResetDurationOnApply)
            CurrentDuration = 0f;
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
