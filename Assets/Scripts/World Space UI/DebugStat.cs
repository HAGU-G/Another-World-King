using TMPro;
using UnityEngine;

public class DebugStat : MonoBehaviour
{
    public TextMeshPro text;
    private CharacterAI character;

    private void Start()
    {
        character = GetComponentInParent<CharacterAI>();
    }
    void Update()
    {
        transform.localScale = character.isPlayer? Vectors.filpX : Vector3.one;
        text.text = $"{character.CurrnetUnitData.division}\nü��\n{character.HP}\n���ݷ�\n{character.AttackDamage}\n����\n{character.AttackSpeed}\n\n{character.UnitState}";
    }
}
