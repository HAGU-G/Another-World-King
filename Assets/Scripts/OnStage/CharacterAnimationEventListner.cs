using UnityEngine;

public class CharacterAnimationEventListner : MonoBehaviour
{
    public event System.Action onAttackHit;
    public event System.Action onAttackEnd;
    public event System.Action onPlayAttackEffect;
    public event System.Action onKillSelf;

    private CharacterSound characterSound;

    public void Init()
    {
        characterSound = GetComponentInParent<CharacterSound>();
    }

    public void AttackHit()
    {
        if (onAttackHit != null)
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
    public void PlayAttackSound()
    {
        if(characterSound != null)
            characterSound.PlayAttackSound();
    }
    public void KillSelf()
    {
        if (onKillSelf != null)
            onKillSelf();
    }

}
