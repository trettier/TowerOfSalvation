using Extentions;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Items levels", menuName = "Settings/Items levels setups")]
public class ItemsLevelsSettings : ScriptableObject
{
    public EffectsSettings effectsSettings;

    public List<WeaponLevelSetup> humansLevelSetups = new List<WeaponLevelSetup>();
    public List<WeaponLevelSetup> zompiesLevelSetups = new List<WeaponLevelSetup>();


    private void OnValidate()
    {
        for (int i = 0; i < humansLevelSetups.Count; i++)
        {
            if (humansLevelSetups[i] != null)
            {
                humansLevelSetups[i].SetLevel(i, effectsSettings);
            }
        }
        for (int i = 0; i < zompiesLevelSetups.Count; i++)
        {
            if (zompiesLevelSetups[i] != null)
            {
                zompiesLevelSetups[i].SetLevel(i, effectsSettings);
            }
        }
    }

    public WeaponModel GetRandomHumanWeapon(int level)
    {
        WeaponLevelSetup weaponLevelSetup = humansLevelSetups.FirstOrDefault(setup => setup.Level == level);
        WeaponModel newItem = weaponLevelSetup.GenerateCharacter();
        return newItem;
    }

    public WeaponModel GetRandomZombyWeapon(int level)
    {
        WeaponLevelSetup weaponLevelSetup = zompiesLevelSetups.FirstOrDefault(setup => setup.Level == level);
        WeaponModel newItem = weaponLevelSetup.GenerateCharacter();
        return newItem;
    }
}

[Serializable]
public class WeaponLevelSetup
{
    public int Level => level;
    private EffectsSettings effectsSettings;
    public void SetLevel(int newLevel, EffectsSettings effectsSettings)
    {
        level = newLevel;
        this.effectsSettings = effectsSettings;
    }


    [SerializeField] private int level;
    [SerializeField] private int effectsQuantity;
    [SerializeField] private Damage.Parameters damage;
    [SerializeField] private List<GameObject> prefabs;


    public WeaponModel GenerateCharacter()
    {
        int randomPrefab = UnityEngine.Random.Range(0, prefabs.Count);

        Damage.Critical crit = new(damage.crit.chance, damage.crit.multiplier);
        Damage.Knockback knockback = new(damage.knockback.value);
        Damage.Parameters newDamage = new(damage.value, damage.increment, crit, knockback);

        WeaponModel newModel = new WeaponModel(
            newDamage,
            prefabs[randomPrefab]
            );
        
        newModel.effectsData = new List<EffectData>();

        for (int i = 0; i < effectsQuantity; i++)
        {
            newModel.effectsData.Add(G.instance.effectsService.GetRandomItemEffect());
        }

        return newModel;
    }
}

