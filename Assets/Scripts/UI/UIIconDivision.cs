using UnityEngine;
using UnityEngine.UI;
using static UnitData;

public class UIIconDivision : MonoBehaviour
{
    public Color debufferColor;
    public Color noneColor;
    public Sprite[] circles;
    public Color healerColor;
    public Sprite[] icons;

    public Image circle;
    public Image icon;

    public void SetDivision(DIVISION division)
    {
        if (division == DIVISION.NONE
            || division == DIVISION.CANNON
            || division == DIVISION.BOMBER)
        {
            circle.sprite = circles[0];
            circle.color = noneColor;
            icon.sprite = icons[0];
        }
        else
        {
            circle.sprite = circles[(int)division];
            icon.sprite = icons[(int)division];

            if (division == DIVISION.SPECIAL)
                circle.color = debufferColor;
            else
                circle.color = Color.white;

            if (division == DIVISION.HEALER)
                icon.color = healerColor;
            else
                icon.color = Color.white;
        }
    }
}
