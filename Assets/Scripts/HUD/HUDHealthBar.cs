using System;
using UnityEngine;
using UnityEngine.UI;

public class HUDHealthBar : MonoBehaviour
{
    public UnitBase stats;
    public Slider healthBar;
    public RectTransform sliderRoot;
    private void Start()
    {
        gameObject.SetActive(true);
        UpdateHealthBar();
    }

    private void Update()
    {
        if (!gameObject.activeSelf)
            return;

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
            float overPosition = sliderRoot.position.x + sliderRoot.sizeDelta.x * (stats.isPlayer ? -1f : 1f);
            if (stats.isPlayer
                && overPosition < Screen.safeArea.xMin)
            {
                sliderRoot.position -= new Vector3(overPosition - Screen.safeArea.xMin, 0f);
            }
            else if (!stats.isPlayer
                && overPosition > Screen.safeArea.xMax)
            {
                sliderRoot.position -= new Vector3(overPosition - Screen.safeArea.xMax, 0f);
            }
        }

    }
}