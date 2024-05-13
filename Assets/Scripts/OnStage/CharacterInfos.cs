using UnityEngine;

public class CharacterInfos
{
    public UnitData unitData;
    public GameObject dress;

    public void SetData(UnitData unitData)
    {
        this.unitData = unitData;
        dress = Resources.Load<GameObject>(string.Format(Paths.resourcesPrefabs, unitData.prefab));
        if (dress == null)
            dress = Resources.Load<GameObject>(string.Format(Paths.resourcesPrefabs, Strings.nonePrefab));
    }
}