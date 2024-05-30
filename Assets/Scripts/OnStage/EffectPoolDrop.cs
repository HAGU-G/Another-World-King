using TMPro;
using UnityEngine;

public class EffectPoolDrop : EffectPoolObject
{
    public float duration;
    public float speed;

    public TextMeshPro gold;
    public TextMeshPro exp;
    public SpriteRenderer[] icons;

    private static readonly string formatValue = "+ {0}";
    private float lifeTime;

    private void Update()
    {
        transform.position += Vector3.up * speed * Time.deltaTime;

        var color = Color.Lerp(Color.white, Colors.transparent, lifeTime / duration);
        foreach (var icon in icons)
        {
            icon.color = color;
        }
        gold.color = color;
        exp.color = color;

        lifeTime += Time.deltaTime;
        if (lifeTime > duration)
        {
            gameObject.SetActive(false);
        }
    }

    public void SetValue(int gold, int exp)
    {
        if (gold <= 0)
            this.gold.gameObject.SetActive(false);
        else
            this.gold.text = string.Format(formatValue, gold);

        if (exp <= 0)
            this.exp.gameObject.SetActive(false);
        else
            this.exp.text = string.Format(formatValue, exp);
    }

    public override void ResetEffect()
    {
        lifeTime = 0f;
    }
}
