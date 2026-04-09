using System;

[Serializable]
public abstract class CharacterEffect : Effect
{
    public CharacterPresenter presenter;
    public CharacterView view;
    public CharacterModel model;

    public CharacterEffect(EffectData data) : base(data)
    {
    }
    public virtual void Initialize(CharacterPresenter target)
    {
        this.presenter = target;
        this.view = presenter.view;
        this.model = presenter.model;
    }
}