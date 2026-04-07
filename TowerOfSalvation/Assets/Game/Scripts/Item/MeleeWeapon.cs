public class MeleeWeapon : ItemView
{
    public override void PerformAttack()
    {
        base.PerformAttack();
        presenter.PerformMeleeAttack();
    }
}