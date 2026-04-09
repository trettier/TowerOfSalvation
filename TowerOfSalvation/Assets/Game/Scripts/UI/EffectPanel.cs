using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class EffectPanel : MonoBehaviour, IPointerExitHandler
{
    public Image icon;
    public TMP_Text label;
    public TMP_Text description;

    public Effect effect;

    public Button button;

    public void Initialize(Effect effect)
    {
        this.effect = effect;

        //icon.sprite = this.effect.icon;
        //label.text = this.effect.name;
        //description.text = this.effect.description;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Destroy(gameObject);
    }
}
