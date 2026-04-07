using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Characters levels", menuName = "Settings/Characters levels setups")]
public class CharactersLevelsSettings : ScriptableObject
{
    public EffectsSettings effectsSettings;

    public int teleportCharactersQuantity;

    public List<CharacterLevelSetup> humanLevelSetups = new List<CharacterLevelSetup>();
    public List<CharacterLevelSetup> zombyLevelSetups = new List<CharacterLevelSetup>();

    private void OnValidate()
    {
        for (int i = 0; i < humanLevelSetups.Count; i++)
        {
            if (humanLevelSetups[i] != null)
            {
                humanLevelSetups[i].SetLevel(i, effectsSettings);
                zombyLevelSetups[i].SetLevel(i, effectsSettings);
            }
        }
    }

    public CharacterModel GetRandomHuman(int level)
    {
        CharacterLevelSetup characterLevelSetup = humanLevelSetups.FirstOrDefault(setup => setup.Level == level);
        CharacterModel newCharacter = characterLevelSetup.GenerateCharacter("human");
        return newCharacter;
    }

    public CharacterModel GetRandomZomby(int level)
    {
        CharacterLevelSetup characterLevelSetup = zombyLevelSetups.FirstOrDefault(setup => setup.Level == level);
        CharacterModel newCharacter = characterLevelSetup.GenerateCharacter("zomby");
        return newCharacter;
    }
}

[Serializable]
public class CharacterLevelSetup
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
    [SerializeField] private float speed;
    [SerializeField] private HealthPoints healthPoints;
    [SerializeField] private Stamina stamina;
    [SerializeField] private List<GameObject> prefabs;

    public CharacterModel GenerateCharacter(string side)
    {
        int randomPrefab = UnityEngine.Random.Range(0, prefabs.Count);

        CharacterModel newModel = new(
            prefabs[randomPrefab],
            new Side(side),
            speed,
            new HealthPoints(healthPoints.max),
            new Stamina(stamina.max),
            new List<CharacterEffect>()
            );

        for (int i = 0; i < effectsQuantity; i++) 
        {
            newModel.effectsData.Add(effectsSettings.GetRandomCharacterEffect());
        }

        return newModel;
    }
}

