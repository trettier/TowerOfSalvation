using System.Collections.Generic;
using UnityEngine;

public class EffectData : ScriptableObject
{
    public Sprite Icon;
    public string id;
    public string label;
    public string description;
    public int level;
    public int rarity;
    public float value_1;
    public float value_2;
    public float value_3;

    public bool permanent;
    public float duration;
    public float interval;
    public List<string> tags = new();
}