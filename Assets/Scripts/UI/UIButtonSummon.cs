using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIButtonSummon : MonoBehaviour
{
    public TextMeshProUGUI cost;
    public TextMeshProUGUI level;
    public Button button;
    public CharacterInfos CharacterInfos { get; private set; } = new();
    public Slider cooldown;
    public Outline outline;
    public int UpgradeExpDamage { get; private set; }
    public int UpgradeExpHP { get; private set; }

    public bool IsUpgraded { get; set; }

    public void SetData(CharacterInfos characterInfos)
    {
        if (characterInfos == null)
        {
            gameObject.SetActive(false);
            return;
        }
        else
        {
            gameObject.SetActive(true);
        }
        CharacterInfos.SetData(characterInfos.unitData);
        cost.text = CharacterInfos.unitData.cost.ToString();
        level.text = CharacterInfos.unitData.ignore;
        cooldown.maxValue = CharacterInfos.unitData.spawnTime;
        cooldown.value = 0f;

        if (DataTableManager.Upgrades.ContainsKey(CharacterInfos.unitData.upgradeDamageID))
        {
            UpgradeExpDamage = DataTableManager.Upgrades[CharacterInfos.unitData.upgradeDamageID].Exp;
            CharacterInfos.upgradeDamage = DataTableManager.Upgrades[CharacterInfos.unitData.upgradeDamageID].Value;
        }
        else
        {
            UpgradeExpDamage = int.MaxValue;
        }

        if (DataTableManager.Upgrades.ContainsKey(CharacterInfos.unitData.upgradeHPID))
        {
            UpgradeExpHP = DataTableManager.Upgrades[CharacterInfos.unitData.upgradeHPID].Exp;
            CharacterInfos.upgradeHP = DataTableManager.Upgrades[CharacterInfos.unitData.upgradeHPID].Value;
        }
        else
        {
            UpgradeExpHP = int.MaxValue;
        }
        
    }

    private void Update()
    {
        if (cooldown.value > 0f)
            cooldown.value -= Time.deltaTime;
    }

    public void Summoned()
    {
        cooldown.value = cooldown.maxValue;
    }
    public void UpgradeDamage()
    {
        CharacterInfos.upgrade = UnitBase.UPGRADE.DAMAGE;
    }
    public void UpgradeHP()
    {
        CharacterInfos.upgrade = UnitBase.UPGRADE.HP;
    }
}
