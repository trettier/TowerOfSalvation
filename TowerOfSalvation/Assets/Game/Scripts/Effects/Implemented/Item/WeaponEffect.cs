public abstract class WeaponEffect : Effect
{
    public ItemPresenter presenter;
    public ItemView view;
    public WeaponModel model;
    public WeaponEffect(EffectData data) : base(data)
    {
    }
    public virtual void Initialize(ItemPresenter target)
    {
        this.presenter = target;
        this.view = target.view;
        this.model = (WeaponModel)target.model;
    }
}