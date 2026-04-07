using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

[Serializable]
public abstract class CharactersZone : MonoBehaviour
{
    private ZoneData data;
    private BoxCollider2D borders => GetComponent<BoxCollider2D>();

    [HideInInspector]
    public Dictionary<CharacterView, Vector3> characters;

    public bool _isInitialized = false;
    public virtual void Initialize(ZoneData data)
    {
        characters = new Dictionary<CharacterView, Vector3>();

        this.data = data;
        if (this.data != null) 
            LoadZone();
        else
            this.data = new ZoneData();

        _isInitialized = true;
    }

    // if there will be conditions to take away and release, could be a problem
    public static bool TryChangeZone(CharactersZone originZone, CharactersZone targetZone, CharacterView view)
    {
        if (!originZone.TryTakeAwayCharacter(view)) 
            return false;
        if (!targetZone.TryReleaseCharacter(view))
            return false;
        return true;
    }

    public virtual Vector3 GetRandomPositionInside()
    {
        Bounds bounds = borders.bounds;
        float randomX = UnityEngine.Random.Range(bounds.min.x, bounds.max.x);
        float randomY = UnityEngine.Random.Range(bounds.min.y, bounds.max.y);
        return new Vector3(randomX, randomY, 0f); 
    }

    protected void AddCharacter(CharacterView view, Vector3 position)
    {
        view.Death += OnCharacterDeath;
        characters[view] = position - transform.position;
        data.characters[view.GetModel()] = position - transform.position;
    }

    protected void RemoveCharacter(CharacterView view)
    {
        view.Death -= OnCharacterDeath;
        characters.Remove(view);
        data.characters.Remove(view.GetModel());
    }

    public void ChangePosition(CharacterView view)
    {
        characters[view] = view.transform.position - transform.position;
        data.characters[view.GetModel()] = view.transform.position - transform.position;
    }

    public virtual void SpawnCharacter(CharacterView view, Vector3 position = default)
    {
        if (position == default)
            position = GetRandomPositionInside();
        else
            position += transform.position;
        view.transform.position = position;
        view.SetTarget(position);
        AddCharacter(view, position);
    }

    public virtual bool TryReleaseCharacter(CharacterView view)
    {
        AddCharacter(view, view.transform.position);
        view.SetTarget(view.transform.position);
        return true;
    }

    public virtual bool TryTakeAwayCharacter(CharacterView view)
    {
        RemoveCharacter(view);
        return true;
    }

    public virtual void ReturnCharacter(CharacterView view)
    {
        view.transform.position = transform.position + characters[view];
    }

    public virtual void LoadZone()
    {
        Dictionary<CharacterModel, Vector3>  reserveCharacters = new Dictionary<CharacterModel, Vector3>(data.characters);

        foreach (var model in reserveCharacters)
        {
            CharacterView character = G.instance.entityFactory.CreateCharacter(model.Key, this, model.Value);
        }
    }

    protected void OnCharacterDeath(CharacterView deadCharacter)
    {
        if (characters.ContainsKey(deadCharacter))
        {
            RemoveCharacter(deadCharacter);
        }
    }

    protected void Clear()
    {
        foreach (var character in characters.Keys.ToList())
        {
            if (character != null)
            {
                character.Death -= OnCharacterDeath;
                Destroy(character.gameObject);
            }
        }
        characters.Clear();
        data.characters.Clear();

    }

    public virtual void OnDestroy()
    {
        foreach (var character in characters.Keys.ToList())
        {
            if (character != null)
            {
                character.Death -= OnCharacterDeath;
            }
        }
        characters.Clear();
    }
}

[Serializable]
public class ZoneData
{
    public string name;
    public int capacity;
    public GameObject prefab;

    [HideInInspector]
    public Dictionary<CharacterModel, Vector3> characters;

    public ZoneData()
    {
        characters = new Dictionary<CharacterModel, Vector3>();
    }
}
