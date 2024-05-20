using UnityEngine;

public class CharacterInfos
{
    public UnitData unitData;
    public GameObject dress;
    public SkillData skillData;
    public SkillData counterSkillData;

    public UnitBase.UPGRADE upgrade;

    public int upgradeDamage;
    public int upgradeHP;

    public void SetData(UnitData unitData)
    {
        this.unitData = unitData;

        dress = Resources.Load<GameObject>(string.Format(Paths.resourcesPrefabs, unitData.prefab));
        if (dress == null)
            dress = Resources.Load<GameObject>(string.Format(Paths.resourcesPrefabs, Strings.nonePrefab));

        if (unitData.skill != Strings.zero)
            skillData = Resources.Load<SkillData>(string.Format(Paths.resourcesSkill, unitData.skill));
        if (unitData.typeCounter != string.Empty)
            counterSkillData = Resources.Load<SkillData>(string.Format(Paths.resourcesCounter, unitData.typeCounter));
    }

}