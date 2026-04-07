using System;

[Serializable]
public class BasicDamageUpgrade : WeaponEffect
{
    public override void Apply()
    {
        base.Apply();
        model.damage.IncreaseValue(value);
    }

    public override void Update(float deltaTime)
    {
        Expire();
    }
}