using UnityEngine;
using UnityEngine.Pool;

public abstract class EffectPoolObject : MonoBehaviour
{
    public IObjectPool<EffectPoolObject> pool;

    private void OnDisable()
    {
        pool.Release(GetComponent<EffectPoolObject>());
    }
    public abstract void ResetEffect();
}