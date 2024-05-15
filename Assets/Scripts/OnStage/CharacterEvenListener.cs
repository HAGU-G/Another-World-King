using UnityEngine;

public class CharacterEvenListener : MonoBehaviour
{
    public event System.Action onAttackHit;
    public event System.Action onAttackEnd;
    public void AttackHit()
    {
        if(onAttackHit != null)
            onAttackHit();
    }
    public void AttackEnd()
    {
        if (onAttackEnd != null)
            onAttackEnd();
    }
}
