using UnityEngine;
using static UnitData;

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

    public void Init()
    {
        if (stats != null)
        {
            if (stats.isPlayer)
            {
                circle.flipX = true;
                icon.flipX = true;
            }
            SetDivision(stats.CurrnetUnitData.division);
        }
    }
    private void Update()
    {
        if(stats.IsDead)
            gameObject.SetActive(false);
    }

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
