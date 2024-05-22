using System.Runtime.CompilerServices;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class CharacterSound : MonoBehaviour
{
    public AudioClip attackSound;
    public AudioClip attackHitSound;
    public AudioClip attackCastleSound;
    private AudioSource attackSource;

    private void Awake()
    {
        attackSource = GetComponent<AudioSource>();
    }

    public void PlayAttackSound()
    {
        if(attackSound != null)
        {
            attackSource.PlayOneShot(attackSound);
        }
    }

    public void PlayAttackHitSound()
    {
        if (attackHitSound != null)
        {
            attackSource.PlayOneShot(attackHitSound);
        }
    }
    public void PlayAttackCastleSound()
    {
        if (attackCastleSound != null)
        {
            attackSource.PlayOneShot(attackCastleSound);
        }
    }
}