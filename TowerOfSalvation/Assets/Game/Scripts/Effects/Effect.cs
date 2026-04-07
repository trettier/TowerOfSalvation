using JetBrains.Annotations;
using System;
using System.Collections;
using UnityEngine;

[Serializable]
public abstract class Effect
{
    public int level;

    public string name;
    public string description;
    public Sprite icon;
    public Duration duration; // нужен не ссылочный тип
    public float value = 1;

    public event Action<Effect> expired;
    public event Action<Effect> applied;

    public virtual Effect Clone()
    {
        var clone = (Effect)this.MemberwiseClone();

        // ВАЖНО: копируем ссылочные поля вручную
        clone.duration = new Duration(duration.full, duration.interval)
        {
            remaining = duration.remaining,
            permanent = duration.permanent
        };

        return clone;
    }
    public virtual void Apply()
    {
        applied?.Invoke(this);
    }

    public virtual void Update(float deltaTime)
    {
        if (!duration.permanent)
        {
            duration.remaining -= deltaTime;
            if (duration.remaining <= 0 && !duration.permanent)
            {
                Expire();
            }
        }
    }

    public virtual void Expire()
    {
        expired?.Invoke(this);
    }

    public virtual bool TryStack(Effect other)
    {
        return true;
    }

    public virtual void MultiplyDuration(float value)
    {
        duration.remaining *= value;
    }
}