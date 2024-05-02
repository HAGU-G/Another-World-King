using System.Text;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI text;
    public TextMeshProUGUI text2;

    public BuffData buff;

    public UnitAI player;
    public UnitAI enemy;


    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.A))
        {
            player.unitStats.HP++;
            player.unitStats.ApplyBuff(buff);
        }



        var sb = new StringBuilder();
        sb.AppendLine(player.unitStats.data.id.ToString());
        sb.AppendLine(player.unitStats.MaxHP.ToString());
        sb.AppendLine(player.unitStats.HP.ToString());
        sb.AppendLine(player.unitStats.AttackDamage.ToString());
        sb.AppendLine(player.unitStats.AttackSpeed.ToString());
        sb.AppendLine(player.unitStats.AttackRange.ToString());
        sb.AppendLine(player.unitStats.data.price.ToString());
        sb.AppendLine(player.unitStats.DropGold.ToString());
        sb.AppendLine(player.unitStats.DropExp.ToString());
        text.text = sb.ToString();

        sb.Clear();
        sb.AppendLine(enemy.unitStats.data.id.ToString());
        sb.AppendLine(enemy.unitStats.MaxHP.ToString());
        sb.AppendLine(enemy.unitStats.HP.ToString());
        sb.AppendLine(enemy.unitStats.AttackDamage.ToString());
        sb.AppendLine(enemy.unitStats.AttackSpeed.ToString());
        sb.AppendLine(enemy.unitStats.AttackRange.ToString());
        sb.AppendLine(enemy.unitStats.data.price.ToString());
        sb.AppendLine(enemy.unitStats.DropGold.ToString());
        sb.AppendLine(enemy.unitStats.DropExp.ToString());
        text2.text = sb.ToString();

    }
}
