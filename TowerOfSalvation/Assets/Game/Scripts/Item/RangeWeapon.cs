using UnityEngine;

public class RangeWeapon : ItemView
{
    public GameObject projectile;

    public override void PerformAttack()
    {
        base.PerformAttack();
        presenter.PerformRangeAttack();
    }

    public void Shoot()
    {
        HitSound(); // should be shoot sound
        Bullet bullet = Instantiate(projectile, transform.position, transform.parent.rotation).GetComponent<Bullet>();


        float angle = transform.parent.eulerAngles.z;

        // ѕреобразуем угол в направление (радианы -> градусы)
        float radians = angle * Mathf.Deg2Rad;
        Vector3 direction = new Vector3(Mathf.Cos(radians) * transform.parent.localScale.x, Mathf.Sin(radians) * transform.parent.localScale.x, 0);

        bullet.Initialize(
            new ProjectileModel(
                presenter.owner.side,
                1,
                ((WeaponModel)presenter.model).damage), direction, transform.parent.localPosition, this);
    }
}