using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class EffectsService : MonoBehaviour
{
    public GameData gameData;

    public List<EffectData> CharactersEffects = new();
    public List<EffectData> WeaponsEffects = new();

    public void Awake()
    {
#if UNITY_EDITOR
        GenerateEffects();
#endif
        var charactersEffectsDict = gameData.effects.Where(a => a.Value.tags.Contains("Character"));
        CharactersEffects = charactersEffectsDict.Select(a => a.Value).ToList(); 
        var weaponsEffectsDict = gameData.effects.Where(a => a.Value.tags.Contains("Weapon"));
        WeaponsEffects = weaponsEffectsDict.Select(a => a.Value).ToList(); 
    }

    private void GenerateEffects()
    {
        foreach (var type in gameData.effectsInstances)
        {
            if (!gameData.effects.ContainsKey(type))
            {
                EffectData asset = AssetFactory.CreateOrLoadEffectDataAsset(
                    $"Effect_{type.Name}",
                    type
                );

                asset.id = type.Name;
                asset.label = type.Name;


                gameData.effects.Add(type, asset);

                EditorUtility.SetDirty(asset);
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("Effects synced");
    }

    public static Effect Create(Type type, EffectData data)
    {
        var effect = (Effect)Activator.CreateInstance(type, new object[] { data });
        return effect;
    }

    public EffectData GetRandomCharacterEffect()
    {
        var effect = CharactersEffects[UnityEngine.Random.Range(0, CharactersEffects.Count)];
        return effect;
    }

    public EffectData GetRandomItemEffect()
    {
        var effect = WeaponsEffects[UnityEngine.Random.Range(0, WeaponsEffects.Count)];
        return effect;
    }
}
