using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Effects Settings", menuName = "Settings/Effects Settings")]
public class EffectsSettings : ScriptableObject
{
    [SerializeReference]
    public List<CharacterEffect> characterEffects = new List<CharacterEffect>
    {
        new BasicHealthUpgrade(),
    };

    [SerializeReference]
    public List<WeaponEffect> itemEffects = new List<WeaponEffect>
    {
        new BasicDamageUpgrade(),
        new BasicIncrementUpgrade()
    };

    public CharacterEffect GetRandomCharacterEffect()
    {
        CharacterEffect effectData = characterEffects[UnityEngine.Random.Range(0, characterEffects.Count)]; 
        return (CharacterEffect)effectData.Clone();
    }

    public WeaponEffect GetRandomItemEffect()
    {
        WeaponEffect effectData = itemEffects[UnityEngine.Random.Range(0, itemEffects.Count)];
        return (WeaponEffect)effectData.Clone();
    }
}

public class EffectStack
{
    public bool stackable = false;
    public int quantity = 0;
}

[Serializable]
public class Duration
{
    public float full;
    public float remaining;
    public float interval;
    public bool permanent;

    public Duration(float full, float interval)
    {
        this.full = full;
        this.remaining = full;
        this.interval = interval;
    }
}