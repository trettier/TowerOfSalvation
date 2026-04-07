using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class CharacterSelection : MonoBehaviour
{
    public SpriteRenderer selectionSprite;
    public CharacterView view;

    private void Start()
    {
        selectionSprite.material = new Material(selectionSprite.material);
    }

    public void Select()
    {
        selectionSprite.material.SetFloat("_Outline", 1);
    }

    public void Deselect()
    {
        selectionSprite.material.SetFloat("_Outline", 0);
    }
}