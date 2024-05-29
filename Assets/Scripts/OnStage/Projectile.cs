using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float gravity = -120f;
    public float velocityX = 10f;
    private float velocityY;
    public bool rotationImage;

    private UnitData.DIVISION ownerDivision;
    private UnitData.DIVISION counterDivision;
    private int damage;
    private int counterDamage;
    private bool isPlayer;
    private string effectAttackHit;
    private bool isTowerTargeting;

    private void Update()
    {
        transform.position += new Vector3(velocityX * (isPlayer ? 1f : -1f), velocityY, 0) * Time.deltaTime;
        var rotationDeg = Mathf.Rad2Deg * Mathf.Atan2(velocityY, velocityX);
        if (rotationImage)
            transform.rotation = Quaternion.Euler(0, 0, isPlayer ? rotationDeg : 180f - rotationDeg);
        velocityY += gravity * Time.deltaTime;
    }

    public void SetTowerTargeting()
    {
        isTowerTargeting = true;
    }

    public void Project(UnitBase owner, Vector3 targetPos, int damage, UnitData.DIVISION counterDivision, int counterDamage)
    {

        ownerDivision = owner.CurrnetUnitData.division;
        this.damage = damage;
        this.counterDivision = counterDivision;
        this.counterDamage = counterDamage;
        isPlayer = owner.isPlayer;
        effectAttackHit = owner.CurrnetUnitData.effectAttackHit;

        var distance = (targetPos.x - transform.position.x);
        velocityY = gravity * distance / 2f / velocityX * (isPlayer ? -1f : 1f);
        if (rotationImage)
            transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan2(velocityY, velocityX));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        HitCheck(collision);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        HitCheck(collision);
    }

    private void HitCheck(Collider2D collision)
    {
        var hitUnit = collision.GetComponent<UnitBase>();
        if (hitUnit == null)
        {
            Destroy(gameObject);
            return;
        }

        if (isPlayer != hitUnit.isPlayer)
        {
            if (!hitUnit.IsTower && collision.isTrigger)
                return;
            if (isTowerTargeting && !hitUnit.IsTower)
                return;

            hitUnit.Damaged(hitUnit.CurrnetUnitData.division == counterDivision ? counterDamage : damage);
            if (EffectManager.Instance.EffectPool.ContainsKey(effectAttackHit))
            {
                var effect = EffectManager.Instance.EffectPool[effectAttackHit].Get();
                effect.gameObject.transform.position = transform.position;
                effect.transform.localScale = isPlayer ? Vectors.filpX : Vector3.one;
            }
            Destroy(gameObject);
        }
    }
}
