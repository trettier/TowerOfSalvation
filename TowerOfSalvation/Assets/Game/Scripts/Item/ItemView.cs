using JetBrains.Annotations;
using System;
using UnityEngine;
using UnityEngine.UI;

public abstract class ItemView : MonoBehaviour
{
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    public ItemSoundController soundController;
    public Transform shadow;
    public Transform display;
    [HideInInspector]
    public Side side;

    public ItemPresenter presenter;

    public float speedWhenAttackMultiplier = 0;
    public Conditions conditions;

    bool isInitialized = false;

    public event Action<Collider2D> Collide;
    public event Action AttackEnd;
    public void Initialize(ItemModel model, CharacterView character)
    {
        presenter = new ItemPresenter(model, this, character);
        isInitialized = true;
    }

    public bool CompareConditions(CharacterView origin, CharacterView target, Stamina stamina)
    {
        if ((origin.transform.position - target.transform.position).magnitude < conditions.distance)
            if (stamina.current > conditions.stamina)
                return true;
        return false;
    }

    public void Update()
    {
        if (!isInitialized)
            return;

        presenter.Update();
    }

    public void FixedUpdate()
    {
        if (!isInitialized)
            return;

        presenter.FixedUpdate();
        shadow.position = display.position - transform.parent.localPosition;
        shadow.rotation = display.rotation;
    }

    public void SetSide(Side side)
    {
        this.side = side;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var view = collision.GetComponent<CharacterView>();
        if (view == null)
            return;

        if (G.instance.sidesSettings.IsEnemy(presenter.owner.side, view.side) && !view.CompareTag("dead"))
        {
            Collide?.Invoke(collision);
        }
    }

    // activates in animator because it easier
    public void EndAttack()
    {
        animator.ResetTrigger("attack");

        AttackEnd?.Invoke();
    }

    public virtual void PerformAttack()
    {
        animator.SetTrigger("attack");
    }

    public virtual void LooseUp()
    {
        presenter.LooseUp();
    }

    public void HitSound()
    {
        soundController.Hit();
    }

    public void OnDeath()
    {
        presenter.ChangeState(new ItemOnDeath());
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        presenter.OnDestroy();
    }
}

[Serializable]
public class Conditions
{
    public float distance = 0;
    public float stamina = 0;
}