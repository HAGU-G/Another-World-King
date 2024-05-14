using UnityEngine;

[RequireComponent(typeof(UnitBase))]
public class HUDHealthBar : MonoBehaviour
{
    public UnitBase stats;
    public SpriteRenderer healthBar;

    private void Update()
    {
        healthBar.transform.localScale = new Vector3(Mathf.Lerp(0f, 0.5f, (float)stats.HP / stats.MaxHP), 0.05f, 1.0f);
    }
}
