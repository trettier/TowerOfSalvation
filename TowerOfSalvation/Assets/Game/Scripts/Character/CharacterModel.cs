using NUnit.Framework;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;

[Serializable]
public class CharacterModel
{
    public ID id;
    public float speed;
    public HealthPoints healthPoints;
    public Stamina stamina;
    public Level level;

    [HideInInspector] public List<CharacterEffect> effectsData;
    [HideInInspector] public ItemSlot itemSlot;
    [HideInInspector] public Side side;
    [HideInInspector] public GameObject prefab;

    public CharacterModel(GameObject prefab, Side side, float speed, HealthPoints healthPoints, Stamina stamina, List<CharacterEffect> effectData)
    {
        id = new ID();
        this.prefab = prefab;
        this.side = side;

        this.speed = speed;
        this.healthPoints = healthPoints;
        this.stamina = stamina;

        this.effectsData = effectData;

        itemSlot = new ItemSlot();

        level = new Level(3);
    }
}

public class ID
{
    public string id;
    public ID()
    {
        GenerateID();
    }

    private void GenerateID()
    {
        id = Guid.NewGuid().ToString();
    }
}

[Serializable]
public class HealthPoints
{
    public float current;
    public float max;

    public event Action Death;
    public event Action IncreaseEvent;
    public event Action GetDamage;

    public HealthPoints(float max)
    {
        current = max;
        this.max = max;
    }

    public void RecieveDamage(float damage)
    {
        if (current > damage)
        {
            current -= damage;
            GetDamage.Invoke();
        }
        else
        {
            if (current == 0)
                return;
            current = 0;
            GetDamage.Invoke();
            Death.Invoke();
        }
    }

    public void Increase(float value)
    {
        float ratio = current / max;
        max += value;
        current = ratio * max;
        IncreaseEvent?.Invoke();
    }
}

[Serializable]
public class Stamina
{
    public float current;
    public float max;
    public float regeneration;

    public Stamina(float agilitty)
    {
        current = agilitty;
        max = agilitty;
    }

    public bool IsFull()
    {
        if (current == max)
            return true;
        return false;
    }

    public void Regeneration()
    {
        if (IsFull())
            return;

        current += regeneration;

        if (current > max)
            current = max;
    }

    public bool TryUse(float value)
    {
        if (value > current)
            return false;

        current -= value;
        return true;
    }
}

public class Level
{
    public int value = 0;
    public Expirience expirience;

    public int availableUpgrades;

    public Level(int expMax)
    {
        expirience = new Expirience(expMax);
        expirience.Full += OnExpirienceFull;

        availableUpgrades = 0;
    }

    public void OnExpirienceFull()
    {
        availableUpgrades++;
    }

    public void LevelUp()
    {
        expirience.current = 0;
        value++;
        availableUpgrades--;
    }

    public void OnDestroy()
    {
        expirience.Full -= OnExpirienceFull;
    }
}

public class Expirience
{
    public float current;
    public float max;
    public float surplus;

    public event Action Updated;
    public event Action Full;

    public Expirience(float max)
    {
        this.max = max;
    }

    public void Increase(float value)
    {
        if (current + value < max)
        {
            current += value;
            Updated.Invoke();
        }
        else
        {
            current = (current + value) % max;
            Updated.Invoke();
            Full.Invoke();
        }
    }

    public void LevelUp()
    {

    }
}