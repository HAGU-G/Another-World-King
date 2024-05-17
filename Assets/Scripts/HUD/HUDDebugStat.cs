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
        text.text = $"ü��\n{character.HP}\n���ݷ�\n{character.AttackDamage}\n����\n{character.AttackSpeed}";
    }
}
