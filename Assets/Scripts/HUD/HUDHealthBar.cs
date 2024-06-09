using UnityEngine;
using UnityEngine.UI;

public class HUDHealthBar : MonoBehaviour
{
    public UnitBase stats;
    public Slider healthBar;
    public RectTransform slider;
    public RectTransform leftPosition;
    public RectTransform rightPosition;
    public void Init()
    {
        if (stats != null)
        {
            transform.localScale = stats.isPlayer ? Vectors.filpX : Vector3.one;
        }
    }
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
        if (stats.IsTower)
        {
            var safeAreaXMin = Camera.main.ScreenToWorldPoint(new(Screen.safeArea.xMin, 0f)).x;
            var safeAreaXMax = Camera.main.ScreenToWorldPoint(new(Screen.safeArea.xMax, 0f)).x;

            transform.position = stats.transform.position;
            if (stats.isPlayer
                & leftPosition.position.x < safeAreaXMin)
            {
                transform.position -= new Vector3(
                    leftPosition.position.x - safeAreaXMin, 0f);
            }
            else if (!stats.isPlayer
                && rightPosition.position.x > safeAreaXMax)
            {
                transform.position -= new Vector3(
                    rightPosition.position.x - safeAreaXMax, 0f);
            }
        }

    }
}