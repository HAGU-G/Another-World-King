using UnityEngine;
using UnityEngine.UI;

public class HUDHealthBar : MonoBehaviour
{
    public UnitBase stats;
    public Slider healthBar;
    public RectTransform sliderRoot;
    public RectTransform leftPosision;
    public RectTransform rightPosision;

    private void Start()
    {
        gameObject.SetActive(true);
        UpdateHealthBar();
    }

    private void Update()
    {
        UpdateHealthBar();
        if (stats.IsDead && !stats.IsTower)
            gameObject.SetActive(false);
    }

    private void UpdateHealthBar()
    {
        healthBar.value = (float)stats.HP / stats.MaxHP;
        sliderRoot.position = Camera.main.WorldToScreenPoint(stats.transform.position);
        if (stats.IsTower)
        {
            if (stats.isPlayer 
                & leftPosision.position.x < Screen.safeArea.xMin)
            {
                sliderRoot.position -= new Vector3(leftPosision.position.x - Screen.safeArea.xMin, 0f);
            }
            else if (!stats.isPlayer
                && rightPosision.position.x > Screen.safeArea.xMax)
            {
                sliderRoot.position -= new Vector3(rightPosision.position.x - Screen.safeArea.xMax, 0f);
            }
        }

    }
}