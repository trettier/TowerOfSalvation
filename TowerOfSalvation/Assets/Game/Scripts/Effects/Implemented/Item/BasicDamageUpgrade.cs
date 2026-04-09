using System;

[Serializable]
public class BasicDamageUpgrade : WeaponEffect
{
    public BasicDamageUpgrade(EffectData data) : base(data)
    {
    }
    public override void Apply()
    {
        base.Apply();
        model.damage.IncreaseValue(data.value_1);
    }

    public override void Update(float deltaTime)
    {
        Expire();
    }
}