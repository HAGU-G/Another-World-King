using UnityEngine;

public class EffectPoolParticle : EffectPoolObject
{
    public ParticleSystem Effect { get; private set; }
    private void Awake()
    {
        Effect = GetComponent<ParticleSystem>();
    }
    public override void ResetEffect()
    {
        Effect.Play();
    }
}