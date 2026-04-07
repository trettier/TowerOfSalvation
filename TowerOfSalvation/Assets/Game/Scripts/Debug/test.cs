using UnityEngine;

public class test : MonoBehaviour
{

    public void OnMouseDrag()
    {
        Debug.Log("Drag");
    }

    public void OnMouseEnter()
    {
        Debug.Log("Enter");
    }

    public void OnMouseOver()
    {
        Debug.Log(gameObject.name);
    }

    public void OnMouseExit()
    {
        Debug.Log("Leave");
    }
}