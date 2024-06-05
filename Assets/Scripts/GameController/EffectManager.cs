using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class EffectManager : MonoBehaviour
{

    public EffectPoolDrop effectPoolDrop;

    public Dictionary<string, IObjectPool<EffectPoolObject>> EffectPool { get; private set; } = new();
    //TODO 스테이지매니저나 다른 곳에 할당해두고 사용하도록 변경
    public static EffectManager Instance => GameObject.FindWithTag(Tags.player)?.GetComponent<EffectManager>();

    private void Awake()
    {
        var effects = Resources.LoadAll<GameObject>(string.Format(Paths.resourcesEffects, string.Empty));
        foreach (var effect in effects)
        {
            var poolObject = effect.GetComponent<EffectPoolObject>();
            if (poolObject != null)
                AddEffectPool(effect.name, poolObject);
        }

        //드랍 이펙트
        AddEffectPool(effectPoolDrop.name, effectPoolDrop);
    }

    public void AddEffectPool(string key, EffectPoolObject poolObject)
    {
        if (!EffectPool.ContainsKey(key))
        {
            int capacity = 5;
            var pool = new ObjectPool<EffectPoolObject>(
                () =>
                {
                    var po = Instantiate(poolObject);
                    po.pool = EffectPool[key];
                    return po;
                },
                OnGetPoolObejct,
                OnReleasePoolObejct,
                OnDestroyPoolObejct, true, capacity, 1000);
            EffectPool.Add(key, pool);

            //이펙트 미리 생성
            //List<EffectPoolObject> effects = new();
            //for (int i = 0; i < capacity; i++)
            //{
            //    effects.Add(EffectPool[key].Get());
            //}
            //foreach (var effect in effects)
            //{
            //    effect.gameObject.SetActive(false);
            //}
        }
    }

    public void ReleaseEffect(string key, EffectPoolObject poolObject)
    {
        if (EffectPool.ContainsKey(key))
            EffectPool[key].Release(poolObject);
        else
            Destroy(poolObject.gameObject);
    }

    public void OnGetPoolObejct(EffectPoolObject poolObject)
    {
        poolObject.gameObject.SetActive(true);
        poolObject.ResetEffect();
    }
    public void OnReleasePoolObejct(EffectPoolObject poolObject)
    {
    }
    public void OnDestroyPoolObejct(EffectPoolObject poolObject)
    {
        Destroy(poolObject.gameObject);
    }

}
