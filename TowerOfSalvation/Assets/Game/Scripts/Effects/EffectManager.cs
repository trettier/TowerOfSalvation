using System;
using System.Collections.Generic;
using UnityEngine;

public class CharacterEffectManager
{
    private List<CharacterEffect> _effects;

    public void Initialize(List<EffectData> effects, CharacterPresenter character)
    {
        _effects = effects;

        foreach (var effect in effects)
        {
            effect.Initialize(character);
            ApplyEffect(effect, false);
        }
    }

    public void ApplyEffect(Effect effect, bool add = true)
    {
        effect.expired += OnExpired;
        if (add)
            _effects.Add((CharacterEffect)effect);
        effect.Apply();
    }

    private void OnExpired(Effect effect)
    {
        effect.expired -= OnExpired;
        _effects.Remove((CharacterEffect)effect);
    }

    public void Update(float deltaTime)
    {
        for (int i = _effects.Count - 1; i >= 0; i--)
        {
            _effects[i].Update(deltaTime);
        }
    }

    public void OnDestroy()
    {
        foreach (var effect in _effects)
            effect.expired -= OnExpired;

        _effects.Clear();
    }
}

public class WeaponEffectManager
{
    private List<WeaponEffect> _effects;

    public void Initialize(List<WeaponEffect> effects, ItemPresenter item)
    {
        _effects = effects;

        foreach (var effect in effects)
        {
            effect.Initialize(item);
            ApplyEffect(effect, false);
        }
    }

    public void ApplyEffect(Effect effect, bool add=true)
    {
        effect.expired += OnExpired;
        if (add)
            _effects.Add((WeaponEffect)effect);
        effect.Apply();
    }

    private void OnExpired(Effect effect)
    {
        effect.expired -= OnExpired;
        _effects.Remove((WeaponEffect)effect);
    }

    public void Update(float deltaTime)
    {
        for (int i = _effects.Count - 1; i >= 0; i--)
        {
            _effects[i].Update(deltaTime);
        }
    }

    public void OnDestroy()
    {
        foreach (var effect in _effects)
            effect.expired -= OnExpired;

        _effects.Clear();
    }
}