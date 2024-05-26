using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDDivision : MonoBehaviour
{
    public UnitBase stats;
    public Color debufferColor;
    public Color noneColor;
    public Sprite[] circles;
    public Color healerColor;
    public Sprite[] icons;

    public SpriteRenderer circle;
    public SpriteRenderer icon;

    private void Start()
    {
        if(stats.isPlayer)
        {
            circle.flipX = true;
            icon.flipX = true;
        }

        if (stats.unitData.division == UnitData.DIVISION.NONE
            || stats.unitData.division == UnitData.DIVISION.CANNON
            || stats.unitData.division == UnitData.DIVISION.BOMBER)
        {
            circle.sprite = circles[0];
            circle.color = noneColor;
            icon.sprite = icons[0];
        }
        else
        {
            circle.sprite = circles[(int)stats.unitData.division];
            icon.sprite = icons[(int)stats.unitData.division];

            if (stats.unitData.division == UnitData.DIVISION.SPECIAL)
                circle.color = debufferColor;
            else
                circle.color = Color.white;

            if (stats.unitData.division == UnitData.DIVISION.HEALER)
                icon.color = healerColor;
            else
                icon.color = Color.white;
        }


    }
}
