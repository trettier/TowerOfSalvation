using UnityEngine;
using UnityEngine.EventSystems;

public class ItemPanel : MonoBehaviour, IPointerExitHandler
{
    public Transform effectsContainer;
    public GameObject effectIconPrefab;


    public void Initialize(ItemPresenter item)
    {

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Destroy(gameObject);
    }

    public void UpgradeLevel()
    {

    }
}