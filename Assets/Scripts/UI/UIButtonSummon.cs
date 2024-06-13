using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIButtonSummon : MonoBehaviour
{
    public TextMeshProUGUI cost;
    public GameObject damageUpgradeLabel;
    public TextMeshProUGUI damageUpgradeText;
    public GameObject hpUpgradeLabel;
    public TextMeshProUGUI hpUpgradeText;
    public Button button;
    public CharacterInfos CharacterInfos { get; private set; } = new();
    public Slider cooldown;
    public Image outline;
    public UISlotCharacter uISlotCharacter;
    public static readonly string upgradeTextFormat = "Lv {0}";

    public int DamageUpgradeExp { get; private set; }
    public int DamageUpgradedCount
    {
        get
        {
            return CharacterInfos.damageUpgradedCount;
        }
        private set
        {
            CharacterInfos.damageUpgradedCount = value;
            AchievementCheck();
        }
    }
    public int DamageUpgradeMaxCount { get; private set; }
    public int HPUpgradeExp { get; private set; }
    public int HPUpgradedCount
    {
        get
        {
            return CharacterInfos.hpUpgradedCount;
        }
        private set
        {
            CharacterInfos.hpUpgradedCount = value;
            AchievementCheck();
        }
    }
    public int HPUpgradeMaxCount { get; private set; }

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
        SetUpgradeText();

        cooldown.maxValue = CharacterInfos.unitData.spawnTime;
        cooldown.value = 0f;

        if (DataTableManager.Upgrades.ContainsKey(CharacterInfos.unitData.upgradeDamageID))
        {
            var upgrade = DataTableManager.Upgrades[CharacterInfos.unitData.upgradeDamageID];
            DamageUpgradeExp = upgrade.Exp;
            CharacterInfos.damageOnceUpgradeValue = upgrade.Value;
            DamageUpgradeMaxCount = upgrade.Count;
        }

        if (DataTableManager.Upgrades.ContainsKey(CharacterInfos.unitData.upgradeHPID))
        {
            var upgrade = DataTableManager.Upgrades[CharacterInfos.unitData.upgradeHPID];
            HPUpgradeExp = upgrade.Exp;
            CharacterInfos.hpOnceUpgradeValue = upgrade.Value;
            HPUpgradeMaxCount = upgrade.Count;
        }

        uISlotCharacter.SetData(CharacterInfos);

    }

    private void Update()
    {
        if (cooldown.value > 0f)
        {
            cooldown.value -= Time.deltaTime;
            if (button.interactable && !outline.enabled)
                button.interactable = false;
        }
        else if (!button.interactable && !outline.enabled)
        {
            button.interactable = true;
        }
    }

    public void Summoned()
    {
        cooldown.value = cooldown.maxValue;
        button.interactable = false;
    }
    public void UpgradeDamage()
    {
        DamageUpgradedCount++;
        SetUpgradeText();
    }
    public void UpgradeHP()
    {
        HPUpgradedCount++;
        SetUpgradeText();
    }

    private void AchievementCheck()
    {
        if(HPUpgradedCount == HPUpgradeMaxCount
            && DamageUpgradedCount == DamageUpgradeMaxCount
            && !StageManager.Instance.IsTutorial)
        {
            GPGSManager.Instance.UnlockAchievement(GPGSIds.achievement_upgrade);
        }
    }

    public void SetUpgradeText()
    {
        damageUpgradeText.text = string.Format(upgradeTextFormat, 1 + DamageUpgradedCount);
        hpUpgradeText.text = string.Format(upgradeTextFormat, 1 + HPUpgradedCount);
    }
}
