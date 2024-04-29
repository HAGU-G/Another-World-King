using UnityEngine;

[CreateAssetMenu(fileName = "CharName", menuName = "CreateData/CharacterData")]
public class CharacterData : ScriptableObject
{
    public int maxHp = 100;
    public int attackdamage = 10;
    public float attackSpeed = 2f;
    public float attackRange = 1f;
    public Color color;
}

