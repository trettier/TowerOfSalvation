using System.Collections.Generic;
using UnityEngine;

public class Drop : MonoBehaviour
{
    public Animator animator;

    public DropType type;

    public Dictionary<DropType, string> dropDictionary = new()
    {
        { DropType.coin, "Expirience" },
        { DropType.expirience, "Coin" }
    };

    public void Initialize(DropType type, Vector2 direction)
    {
        this.type = type;

        animator.Play(dropDictionary[type]);

        GetComponent<Rigidbody2D>().AddForce(direction * Random.Range(5, 10), ForceMode2D.Impulse);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var character = collision.gameObject.GetComponent<CharacterView>();

        if (character == null)
            return;

        if (character.side.name != "human")
            return;

        character.presenter.IncreaseExpirience(1);
        Destroy(this);
    }
}

public enum DropType
{
    coin,
    expirience,
    weapon
}