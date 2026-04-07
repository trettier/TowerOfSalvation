using DG.Tweening;
using UnityEngine;

public class CharacterAttack : CharacterState
{
    public CharacterView target;
    public float speedWhenAttackMultiplier;

    public override void Enter(CharacterPresenter presenter)
    {
        base.Enter(presenter);
        target = presenter.target;
        view.weapon.PerformAttack();
        view.weapon.AttackEnd += OnAttackEnd;
        speedWhenAttackMultiplier = view.weapon.speedWhenAttackMultiplier;
        if (speedWhenAttackMultiplier == 0)
            view.Stop();
    }

    public void OnAttackEnd()
    {
        if (!target.CompareTag("dead") && view.weapon.CompareConditions(view, target, model.stamina))
            view.weapon.PerformAttack();
        else
            presenter.ChangeState(new ChaseEnemy());
    }

    public override void FixedUpdate()
    {
        if (target == null)
            return;

        Vector3 direction = (target.transform.position - view.transform.position).normalized;
        view.Flip(direction);
        if (speedWhenAttackMultiplier != 0)
            view.Move(direction, model.speed * speedWhenAttackMultiplier);

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        if (direction.x < 0)
        {
            angle += 180f;
        }
        Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);
        //view.rightHand.transform.rotation = targetRotation;
        view.rightHand.transform.DORotateQuaternion(targetRotation, 0.1f);

    }

    public override void Exit()
    {
        view.weapon.LooseUp();
        view.weapon.AttackEnd -= OnAttackEnd;
    }

}