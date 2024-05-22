using UnityEngine;

public class CharacterEvenListener : MonoBehaviour
{
    public event System.Action onAttackHit;
    public event System.Action onAttackEnd;
    public event System.Action onPlayAttackEffect;
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
    public void PlayAttackEffect()
    {
        if (onPlayAttackEffect != null)
            onPlayAttackEffect();
    }
}
