using TMPro;
using UnityEngine;

public class HUDStat : MonoBehaviour
{
    public GameObject background;
    public TextMeshPro text;
    private CharacterAI character;
    public readonly static string textFormat = "���ݷ�\n{0}\nü��\n{1}\n���ݼӵ�\n{2}";

    private void Start()
    {
        character = GetComponentInParent<CharacterAI>();
    }
    private void Update()
    {
        if(character.IsDead) 
            gameObject.SetActive(false);
        HUDStatOnOff(StageManager.Instance.IsShowHUDStat);
        transform.localScale = character.isPlayer ? Vectors.filpX : Vector3.one;
        text.text = string.Format(textFormat, character.AttackDamage, character.HP, character.AttackSpeed);
    }

    private void HUDStatOnOff(bool value)
    {
        background.SetActive(value);
        text.gameObject.SetActive(value);
    }
}
