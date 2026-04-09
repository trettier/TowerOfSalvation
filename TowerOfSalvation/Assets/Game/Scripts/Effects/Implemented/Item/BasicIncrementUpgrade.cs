using System;

[Serializable]
public class BasicIncrementUpgrade : WeaponEffect
{
    public BasicIncrementUpgrade(EffectData data) : base(data)
    {
    }
    public override void Apply()
    {
        base.Apply();
        model.damage.IncreaseIncrement(data.value_1);
    }

    public override void Update(float deltaTime)
    {
        Expire();
    }
}