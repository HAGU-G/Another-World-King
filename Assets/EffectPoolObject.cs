using UnityEngine;
using UnityEngine.Pool;

public class EffectPoolObject : MonoBehaviour
{
    public IObjectPool<EffectPoolObject> pool;
    public ParticleSystem Particle { get; private set; }

    private void Awake()
    {
        Particle = GetComponent<ParticleSystem>();
    }

    private void OnDisable()
    {
        pool.Release(GetComponent<EffectPoolObject>());
    }
}
