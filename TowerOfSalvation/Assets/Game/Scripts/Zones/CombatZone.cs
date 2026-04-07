using UnityEngine;

public class CombatZone : CharactersZone
{
    private ZoneState _currentState;

    public override void Initialize(ZoneData data)
    {
        base.Initialize(data);
        ChangeState(new ZoneStill());

        _isInitialized = true;
    }

    public void ChangeState(ZoneState newState)
    {
        if (_currentState != null)
            _currentState.Exit();
        _currentState = newState;
        newState.Enter(this);
    }

    private void Update()
    {
        if (!_isInitialized)
            return;
        _currentState.Update();
    }

    private void FixedUpdate()
    {
        if (!_isInitialized)
            return;
        _currentState.FixedUpdate();
    }

    public void EnterBattle()
    {
        ChangeState(new ZoneCombat());
    }

    public void SetTarget(Vector2 target)
    {
        _currentState.SetTarget(target);
    }
    public override void OnDestroy()
    {
        base.OnDestroy();
        _currentState.Exit();
    }
}

public abstract class ZoneState
{
    public CharactersZone zone;
    public virtual void Enter(CharactersZone zone) 
    {
        this.zone = zone;
    }
    public virtual void SetTarget(Vector2 target) { }
    public virtual void Update() { }
    public virtual void FixedUpdate() { }
    public virtual void Exit() { }
}

public class ZoneStill : ZoneState { }
public class ZoneCombat : ZoneState
{
    private Vector2 targetPosition;
    private bool hasTarget = false;

    // Настройки движения и поворота (можно вынести в отдельный ScriptableObject)
    private float moveSpeed = 5f;
    private float stopDistance = 0.5f;

    public override void Enter(CharactersZone zone)
    {
        base.Enter(zone);
        hasTarget = false;
    }

    public override void SetTarget(Vector2 target)
    {
        Vector2 currentPosition = zone.transform.position;
        Vector2 direction = (target - currentPosition).normalized;

        if (direction != Vector2.zero)
        {
            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            zone.transform.rotation = Quaternion.Euler(0, 0, targetAngle);
        }

        targetPosition = target;
        hasTarget = true;
        foreach (var character in zone.characters)
        {
            int c = 1;
            var rotation = zone.transform.rotation;
            if (direction.x < 0)
            {
                rotation = Quaternion.Inverse(zone.transform.rotation);
            }
            Vector3 characterOffset = rotation * character.Value;
            if (direction.x < 0)
            {
                characterOffset.y = -characterOffset.y;
                //characterOffset.x = -characterOffset.x;
            }

            Vector3 finalTarget = (Vector3)target + characterOffset;
            CoroutineHolder.instance.AfterSeconds(Random.Range(0.1f, 0.3f), () =>
            {
                character.Key.SetTarget(finalTarget);
            });
        }
    }

    public override void FixedUpdate()
    {
        if (!hasTarget) return;

        Vector2 currentPosition = zone.transform.position;

        Vector2 directionToTarget = (targetPosition - currentPosition).normalized;

        float distanceToTarget = Vector2.Distance(currentPosition, targetPosition);

        if (distanceToTarget > stopDistance)
        {
            Vector2 newPosition = Vector2.MoveTowards(
                currentPosition,
                targetPosition,
                moveSpeed * Time.fixedDeltaTime
            );


            zone.transform.position = new Vector3(newPosition.x, newPosition.y, zone.transform.position.z);
        }
    }

    public override void Exit()
    {
        base.Exit();
        hasTarget = false;
    }
}