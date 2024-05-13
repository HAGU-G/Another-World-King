using UnityEngine;

public class CharacterInfos
{
    public UnitData unitData;
    public GameObject animator;

    public void SetData(UnitData unitData)
    {
        this.unitData = unitData;
        animator = Resources.Load<GameObject>(string.Format(Paths.resourcesPrefabs, unitData.prefab));
        if (animator == null)
            animator = Resources.Load<GameObject>(string.Format(Paths.resourcesPrefabs, Strings.nonePrefab));
    }
}