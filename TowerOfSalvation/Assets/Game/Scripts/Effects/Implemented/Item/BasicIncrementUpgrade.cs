using System;

[Serializable]
public class BasicIncrementUpgrade : WeaponEffect
{
    public override void Apply()
    {
        base.Apply();
        model.damage.IncreaseIncrement(value);
    }

    public override void Update(float deltaTime)
    {
        Expire();
    }
}