using UnityEngine;

public class CharacterInfos
{
    public UnitData unitData;
    public GameObject dress;
    public SkillData skillData;
    public SkillData counterSkillData;

    public int damageOnceUpgradeValue;
    public int damageUpgradedCount;
    public int hpOnceUpgradeValue;
    public int hpUpgradedCount;

    public void SetData(UnitData unitData)
    {
        this.unitData = unitData;

        dress = Resources.Load<GameObject>(string.Format(Paths.resourcesPrefabs, unitData.prefab));
        if (dress == null)
            dress = Resources.Load<GameObject>(string.Format(Paths.resourcesPrefabs, Defines.nonePrefab));

        if (unitData.skill != Defines.zero)
            skillData = Resources.Load<SkillData>(string.Format(Paths.resourcesSkill, unitData.skill));
        if (unitData.typeCounter != string.Empty)
            counterSkillData = Resources.Load<SkillData>(string.Format(Paths.resourcesCounter, unitData.typeCounter));
    }

}