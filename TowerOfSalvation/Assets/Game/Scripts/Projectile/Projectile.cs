using System;
using UnityEngine;

public abstract class Projectile : MonoBehaviour
{
    public Rigidbody2D rb;
    public Animator animator;

    public ProjectileModel model;

    protected Vector3 _direction;
    protected Vector3 _offset;

    public ItemView _owner;

    public virtual void Initilize(ProjectileModel model, Vector3 direction, Vector3 offset, ItemView owner)
    {
        this.model = model;
        this._direction = direction;
        this._offset = offset;
        this._owner = owner;
    }

    public virtual void OnDestroy()
    {

    }
}

[Serializable]
public class ProjectileModel
{
    public GameObject prefab;
    public Side side;
    public float speed;
    public Damage.Parameters damage;

    public ProjectileModel(Side side, float speed, Damage.Parameters damage)
    {
        this.side = side;
        this.speed = speed;
        this.damage = damage;
    }
}