
public class SkillBase
{
    public SkillData skillData;

    public SkillBase(SkillData skillData = null)
    {
        this.skillData = skillData;
    }

    public int Count { get; private set; }

    public float CurrentDuration { get; private set; }
    public bool Apply()
    {
        bool applied = false;

        if (skillData.nesting == 0 && Count == 0)
        {
            Count++;
            applied = true;
        }
        else if (Count < skillData.nesting)
        {
            Count++;
            applied = true;
        }

        if (skillData.doResetDurationOnApply)
        {
            CurrentDuration = 0f;
            applied = true;
        }

        return applied;
    }

    public int UpdateDuration(float deltaTime)
    {
        if (skillData.infinityDuration)
            return Count;

        if ((CurrentDuration += deltaTime) >= skillData.duration)
        {
            CurrentDuration = 0f;
            Count--;
        }
        return Count;
    }

    public void IncreaseDuration(float increaseValue)
    {
        CurrentDuration -= increaseValue;
    }
}

