using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ItemModel
{
    public ID id;
    //public string label;
    //public string description;
    public Sprite icon;
    [HideInInspector] public GameObject prefab;
    [HideInInspector] public List<WeaponEffect> effectsData;
}

[Serializable]
public class WeaponModel : ItemModel
{
    public Damage.Parameters damage;

    public WeaponModel(Damage.Parameters damage, GameObject prefab)
    {
        id = new ID();

        this.damage = damage;
        this.prefab = prefab;
    }
}
