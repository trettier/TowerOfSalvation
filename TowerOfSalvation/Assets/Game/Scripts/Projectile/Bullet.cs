using UnityEngine;

public class Bullet : Projectile
{
    public Transform display;
    public Transform shadow;


    public override void Initilize(ProjectileModel model, Vector3 direction, Vector3 offset, ItemView owner)
    {
        base.Initilize(model, direction, offset, owner);

        animator.Play("shot");

        //CoroutineHolder.instance.AfterSeconds(5, () =>
        //{
        //    G.instance.visualEffectsFactory.SpawnHitEffect(transform.position);
        //    Destroy(gameObject);
        //});
    }

    private void FixedUpdate()
    {
        Move();
        shadow.position = display.position - _offset;
        shadow.rotation = display.rotation;
    }

    private void Move()
    {
        rb.AddForce(model.speed * _direction, ForceMode2D.Impulse);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var character = collision.gameObject.GetComponent<CharacterView>();

        if (character == null)
            return;

        if (!G.instance.sidesSettings.IsEnemy(model.side, character.side))
            return;

        G.instance.damage.Deal(this, character, _direction);

        G.instance.visualEffectsFactory.SpawnHitEffect(transform.position);
        Destroy(gameObject);
    }
}