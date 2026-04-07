using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class CharacterView : MonoBehaviour
{
    public CharacterPresenter presenter;
    public Animator animator;
    public Vision vision;
    public Rigidbody2D rb;
    public SpriteRenderer spriteRenderer;
    public Transform rightHand;
    public ItemView weapon;
    public CharacterSoundController sound;
    public Side side;
    public bool isInitialized = false;
    public Vector3 targetPosition;
    public event Action<CharacterView> Death;
    public event Action<Collision2D> Collided;
    public event Action TargetPositionSet;

    public IEnumerator delayedDeath;

    public SortingGroup sortingGroup => GetComponent<SortingGroup>();

    public void Initialize(CharacterModel model)
    {
        presenter = new CharacterPresenter(model, this);
        this.side = model.side;
        vision.Initialize(side);
        isInitialized = true;

        G.instance.uiFactory.CreateHealthBar(transform, model.healthPoints);
        G.instance.uiFactory.CreateExpirienceBar(transform, model.healthPoints, model.level);
    }

    public ID GetID()
    {
        return presenter.model.id;
    }

    private void Update()
    {
        if (!isInitialized)
            return;
        presenter.Update();
    }

    private void FixedUpdate()
    {
        if (!isInitialized)
            return;
        presenter.FixedUpdate();
    }

    public void Drag(Vector3 position)
    {
        transform.position = position;
    }

    public void SetTarget(Vector3 position)
    {
        if (presenter.currentState is Death)
            return;
        //transform.position = position;
        targetPosition = position;
        TargetPositionSet?.Invoke();
    }

    public void Move(Vector3 direction, float speed)
    {
        rb.AddForce(direction * speed);
        animator.SetBool("move", true);
        sound.StepStart();
        Flip(direction);
    }

    public void Stop()
    {
        animator.SetBool("move", false);
        sound.StepStop();
    }

    public void OnDeath()
    {
        Death?.Invoke(this);
        delayedDeath = CoroutineHolder.instance.AfterSeconds(10, () =>
        {
            if (gameObject != null)
                Destroy(gameObject);
        });
    }

    public void RecieveDamage()
    {
        animator.SetTrigger("hit");
        sound.Hit();
    }

    public void RecieveKnockback(Vector3 knockback)
    {
        rb.AddForce(knockback);
        animator.SetBool("move", false);
    }

    public void Flip(Vector3 direction)
    {
        spriteRenderer.flipX = direction.x < 0;

        if (direction.x < 0)
            rightHand.transform.localScale = new Vector3(-1, 1, 1);
        else
            rightHand.transform.localScale = new Vector3(1, 1, 1);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Collided?.Invoke(collision);
    }

    public CharacterModel GetModel()
    {
        return presenter.model;
    }

    private void OnDestroy()
    {
        presenter.OnDestroy();
        if (delayedDeath != null)
            StopCoroutine(delayedDeath);
    }
}