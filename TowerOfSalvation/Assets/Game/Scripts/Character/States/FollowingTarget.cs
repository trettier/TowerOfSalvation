using Goldmetal.UndeadSurvivor;
using UnityEngine;
using static Unity.VisualScripting.Member;
using static UnityEngine.GraphicsBuffer;
public class FollowingTarget : CharacterState
{
    public CharacterView target;
    public bool isEnemyInSight = false;
    public override void Enter(CharacterPresenter presenter)
    {
        base.Enter(presenter);
        view.vision.enemyDetection += OnEnemyDetection;
        view.vision.enemyLoseSight += OnEnemyLoseSight;
        view.Collided += OnCollide;
    }

    public override void FixedUpdate()
    {
        Vector3 direction = (view.targetPosition - view.transform.position).normalized;

        view.Move(direction, model.speed);
    }

    public override void Update()
    {
        if ((view.targetPosition - view.transform.position).magnitude < 0.05f)
            OnTargetReach();
        else if (target != null)
            if (view.weapon.CompareConditions(view, target, model.stamina))
                presenter.ChangeState(new CharacterAttack());
    }

    public void OnEnemyDetection()
    {
        isEnemyInSight = true;
        SelectTarget(target);
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

    public void OnEnemyLoseSight()
    {
        isEnemyInSight = false;
        presenter.target = null;
    }

    public void OnTargetReach()
    {
        if (isEnemyInSight)
            presenter.ChangeState(new ChaseEnemy());
        else
            presenter.ChangeState(new CharacterIdle());
    }

    public void OnCollide(Collision2D collider)
    {
        var target = collider.gameObject.GetComponent<CharacterView>();
        if (target != null)
        {
            Vector3 knockback = Damage.CalculateKnockback(view.transform, target.transform, ((WeaponModel)view.weapon.presenter.model).damage.knockback);
            target.presenter.RecieveKnockback(knockback / 5);
            target.presenter.RecieveDamage(0, presenter);
        }
    }

    public override void Exit()
    {
        SelectTarget(target);
        view.Collided -= OnCollide;
        view.vision.enemyDetection -= OnEnemyDetection;
        view.vision.enemyLoseSight -= OnEnemyLoseSight;
    }
}