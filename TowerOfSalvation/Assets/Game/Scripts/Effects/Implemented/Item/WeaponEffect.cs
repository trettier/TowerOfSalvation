using System;

[Serializable]
public abstract class WeaponEffect : Effect
{
    public ItemPresenter presenter;
    public ItemView view;
    public WeaponModel model;

    public virtual void Initialize(ItemPresenter target)
    {
        this.presenter = target;
        this.view = target.view;
        this.model = (WeaponModel)target.model;
    }
}