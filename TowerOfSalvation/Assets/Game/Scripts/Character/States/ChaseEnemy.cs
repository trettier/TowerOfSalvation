using UnityEngine;

public class ChaseEnemy : CharacterState
{
    public CharacterView target;

    public override void Enter(CharacterPresenter presenter)
    {
        base.Enter(presenter);
        SelectTarget(null);
        if (target == null)
            return;

        view.vision.enemyLoseSight += OnEnemyLoseSight;
    }

    public void SelectTarget(CharacterView dead)
    {
        if (target != null)
            target.Death -= SelectTarget;
        target = view.vision.ClosestEnemy();
        if (target == null)
        {
            OnEnemyLoseSight();
            return;
        }

        target.Death += SelectTarget;
        presenter.target = target;
    }

    private void OnEnemyLoseSight()
    {
        presenter.target = null;
        presenter.ChangeState(new FollowingTarget());
    }

    public override void Exit()
    {
        if (target != null)
            target.Death -= SelectTarget;
        view.vision.enemyLoseSight -= OnEnemyLoseSight;
    }

    public override void FixedUpdate()
    {
        if (target == null || view.weapon == null)
            return;

        if (view.weapon.CompareConditions(view, target, model.stamina))
        {
            presenter.ChangeState(new CharacterAttack());
        }

        Vector3 direction = (target.transform.position - view.transform.position).normalized;

        view.Move(direction, model.speed);
    }
}
