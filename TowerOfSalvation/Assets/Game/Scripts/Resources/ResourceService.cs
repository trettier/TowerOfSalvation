using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static GameData;

public class ResourceService : MonoBehaviour
{
    public List<PlayerResourcesView> resources = new List<PlayerResourcesView>();

    public event Action<ResourceType, int> ResourcesChanged;

    public void Initialize()
    {
        foreach (var resource in resources)
        {
            ResourcesChanged += resource.OnResourceQuantityChanged;
            resource.quantity.text = Get(resource.type).ToString();
        }
    }

    public void ResetPlayer()
    {
        PlayerResources.Set(ResourceType.Victual, 0);
    }

    public int Get(ResourceType type)
    {
        return PlayerResources.Get(type);
    }

    public void Add(ResourceType type, int amount)
    {
        int current = PlayerResources.Get(type) + amount;
        PlayerResources.Set(type, current);
        ResourcesChanged.Invoke(type, current);
        G.instance.audioManager.PlayCoinCollect();
    }

    public bool TrySpend(ResourceType type, int amount)
    {
        int current = PlayerResources.Get(type);

        if (current < amount)
        {
            G.instance.audioManager.PlayCoinCollect();
            return false;
        }
        current -= amount;
        PlayerResources.Set(type, current);
        ResourcesChanged.Invoke(type, current);
        G.instance.audioManager.PlayButtonClick();
        return true;
    }

    private void OnDestroy()
    {
        foreach (var resource in resources)
        {
            ResourcesChanged -= resource.OnResourceQuantityChanged;
        }
    }
}

[Serializable]
public class PlayerResourcesView
{
    public ResourceType type;
    public TMP_Text quantity;

    public void OnResourceQuantityChanged(ResourceType otherType, int newQuantity)
    {
        if (type == otherType)
            quantity.text = newQuantity.ToString();
    }
}

[Serializable]
public class ResourceInformation
{
    public ResourceType resourceType;

    public int quantity;
}