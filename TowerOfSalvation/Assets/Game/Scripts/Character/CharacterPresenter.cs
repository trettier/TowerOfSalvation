using Goldmetal.UndeadSurvivor;
using System;
using System.Linq;
using UnityEngine;

public class CharacterPresenter
{
    public CharacterModel model;
    public CharacterView view;
    public CharacterState currentState;
    public CharacterEffectManager effectManager;
    public CharacterView target;
    public CharacterPresenter(CharacterModel model, CharacterView view)
    {
        this.model = model;
        this.view = view;

        effectManager = new CharacterEffectManager();
        effectManager.Initialize(model.effectsData, this);

        this.model.healthPoints.Death += OnDeath;

        currentState = new CharacterIdle();
        currentState.Enter(this);

        UpdateWeapon();
    }

    public void ChangeState(CharacterState newState)
    {
        if (currentState is Death)
            return;

        currentState.Exit();
        currentState = newState;
        newState.Enter(this);
    }

    public void Update()
    {
        currentState.Update();
        if (currentState is Death)
            return;
        effectManager.Update(Time.deltaTime);
    }

    public void FixedUpdate()
    {
        currentState.FixedUpdate();
    }

    public void GetExpirience(float value)
    {
        Debug.Log(value);
        model.level.expirience.Increase(value);
    }

    public void LevelUp()
    {
        var effect1 = G.instance.effectsSettings.GetRandomCharacterEffect();
        var effect2 = G.instance.effectsSettings.GetRandomCharacterEffect();
        var effect3 = G.instance.effectsSettings.GetRandomCharacterEffect();

        G.instance.uiFactory.CreateCharacterLevelUpChoice(this, effect3, effect2, effect1);
    }

    public void RecieveDamage(float damage, CharacterPresenter source)
    {
        model.healthPoints.RecieveDamage(damage);
        view.RecieveDamage();
        if (model.healthPoints.current == 0)
            source.GetExpirience(model.level.value + 2);
    }

    public void RecieveKnockback(Vector3 knockback)
    {
        view.RecieveKnockback(knockback);
    }

    public void ApplyEffect(CharacterEffect effect)
    {
        effectManager.ApplyEffect(effect);
    }

    public void OnDeath()
    {
        ChangeState(new Death());
    }

    public void UpdateWeapon()
    {
        view.weapon = G.instance.entityFactory.CreateWeapon((WeaponModel)model.itemSlot.item, view);
    }

    public void OnDestroy()
    {
        currentState.Exit();
        this.model.healthPoints.Death -= OnDeath;
        effectManager.OnDestroy();
    }

    public void ShowInPanel()
    {
        view.sortingGroup.sortingLayerName = "UI";
    }

    public void HideFromPanel()
    {
        view.sortingGroup.sortingLayerName = "environment";
    }
}

public abstract class CharacterState
{
    protected CharacterModel model;
    protected CharacterView view;
    protected CharacterPresenter presenter;
    public virtual void Enter(CharacterPresenter presenter) 
    {
        this.presenter = presenter;
        model = presenter.model;
        view = presenter.view;
    }
    public virtual void Update() { }
    public virtual void FixedUpdate() { }
    public virtual void Exit() { }
}