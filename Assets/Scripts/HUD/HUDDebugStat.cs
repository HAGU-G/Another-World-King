using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HUDDebugStat : MonoBehaviour
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
        text.text = $"{character.unitData.division}\n{character.upgrade}\n체력\n{character.HP}\n공격력\n{character.AttackDamage}\n공속\n{character.AttackSpeed}\n\n{character.UnitState}";
    }
}
