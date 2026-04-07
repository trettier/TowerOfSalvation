using DG.Tweening;
using System;
using System.Data;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ItemPresenter
{
    public ItemModel model;
    public ItemView view;
    public Transform Direction;
    public WeaponEffectManager effectManager;
    public CharacterView owner;
    public CharacterView target;

    public ItemState currentState;

    public bool isInitized = false;
    public ItemPresenter(ItemModel model, ItemView view, CharacterView character)
    {
        this.model = model;
        this.view = view;
        this.owner = character;

        effectManager = new WeaponEffectManager();
        effectManager.Initialize(model.effectsData, this);

        ChangeState(new ItemIdle());

        isInitized = true;
    }

    public void ChangeState(ItemState newState)
    {
        if (currentState != null)
            currentState.Exit();
        currentState = newState;
        newState.Enter(this);
    }

    public void Update()
    {
        currentState.Update();
    }

    public void FixedUpdate()
    {
        currentState.FixedUpdate();
        effectManager.Update(Time.deltaTime);
    }

    public void LooseUp()
    {
        ChangeState(new ItemIdle());
    }

    public void PerformMeleeAttack()
    {
        ChangeState(new WeaponMeleeAttack());
    }

    public void PerformRangeAttack()
    {
        ChangeState(new WeaponRangeAttack());
    }

    public void ApplyEffect(WeaponEffect effect)
    {
        effectManager.ApplyEffect(effect);
    }

    public void OnDestroy()
    {
        currentState.Exit();
        effectManager.OnDestroy();
        effectManager = null;
    }
}

public abstract class ItemState
{
    protected ItemModel model;
    protected ItemView view;
    protected ItemPresenter presenter;
    public virtual void Enter(ItemPresenter presenter)
    {
        this.presenter = presenter;
        model = presenter.model;
        view = presenter.view;
    }
    public virtual void Update() { }
    public virtual void FixedUpdate() { }
    public virtual void Exit() { }
}