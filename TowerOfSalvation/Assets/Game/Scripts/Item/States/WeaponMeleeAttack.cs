using UnityEngine;

public class WeaponMeleeAttack : ItemState
{
    public override void Enter(ItemPresenter presenter)
    {
        base.Enter(presenter);
        view.Collide += OnCollide;
    }

    public void OnCollide(Collider2D enemyCollider)
    {
        CharacterView target = enemyCollider.gameObject.GetComponent<CharacterView>();

        G.instance.damage.Deal(presenter.owner, target, (WeaponModel)model);

        view.HitSound();
    }

    public override void Exit()
    {
        view.Collide -= OnCollide;
    }
}