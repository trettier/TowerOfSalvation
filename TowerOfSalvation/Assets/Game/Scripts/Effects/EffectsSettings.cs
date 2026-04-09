using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Effects Settings", menuName = "Settings/Effects Settings")]
public class EffectsSettings : ScriptableObject
{

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

    public Duration(float full, float interval, bool permanent = false)
    {
        this.full = full;
        this.remaining = full;
        this.interval = interval;
    }
}