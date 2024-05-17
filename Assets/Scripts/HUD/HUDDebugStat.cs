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
        text.text = $"체력\n{character.HP}\n공격력\n{character.AttackDamage}\n공속\n{character.AttackSpeed}";
    }
}
