using DG.Tweening;
using UnityEngine;

public class ItemIdle : ItemState
{
    public override void Enter(ItemPresenter presenter)
    {
        base.Enter(presenter);
        view.transform.parent.transform.DORotate(Vector3.zero, 2f);
    }
}