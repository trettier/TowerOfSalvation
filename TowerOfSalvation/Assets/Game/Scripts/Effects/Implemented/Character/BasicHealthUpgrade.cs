using System;

[Serializable]
public class BasicHealthUpgrade : CharacterEffect
{
    public override void Apply()
    {
        base.Apply();
        model.healthPoints.Increase(value);
    }

    public override void Update(float deltaTime)
    {
        Expire();
    }
}