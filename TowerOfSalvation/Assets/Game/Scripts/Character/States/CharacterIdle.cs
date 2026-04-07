using UnityEngine;

public class CharacterIdle : CharacterState
{
    public override void Enter(CharacterPresenter presenter)
    {
        base.Enter(presenter);
        view.Stop();

        view.vision.enemyDetection += OnEnemyDetection;
        view.TargetPositionSet += OnTargetPositionSet;
    }

    public void OnEnemyDetection()
    {
        presenter.ChangeState(new ChaseEnemy());
    }

    public void OnTargetPositionSet()
    {
        presenter.ChangeState(new FollowingTarget());
    }

    public override void Exit()
    {
        view.vision.enemyDetection -= OnEnemyDetection;
        view.TargetPositionSet -= OnTargetPositionSet;
    }
}