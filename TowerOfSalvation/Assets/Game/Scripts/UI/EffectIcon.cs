using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class EffectIcon : MonoBehaviour, IPointerEnterHandler
{
    private Effect _effect;

    public Image icon;

    public void Initialize(Effect effect)
    {
        _effect = effect;

        icon.sprite = _effect.icon;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {

    }
}
