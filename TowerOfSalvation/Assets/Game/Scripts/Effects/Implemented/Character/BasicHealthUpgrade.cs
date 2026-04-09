public class BasicHealthUpgrade : CharacterEffect
{
    public BasicHealthUpgrade(EffectData data) : base(data)
    {
    }
    public override void Apply()
    {
        base.Apply();
        model.healthPoints.Increase(data.value_1);
    }

    public override void Update(float deltaTime)
    {
        Expire();
    }
}