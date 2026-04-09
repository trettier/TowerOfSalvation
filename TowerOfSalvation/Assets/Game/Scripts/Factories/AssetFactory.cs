using System;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;

#if UNITY_EDITOR
public static class AssetFactory
{
    public static EffectData CreateOrLoadEffectDataAsset(string assetName, Type type)
    {

        string path = $"Assets/Game/Assets/Effects/{assetName}.asset";

        // ѕытаемс€ загрузить
        var existing = AssetDatabase.LoadAssetAtPath<EffectData>(path);
        if (existing != null)
            return existing;

        // —оздаЄм новый
        var asset = ScriptableObject.CreateInstance<EffectData>();

        AssetDatabase.CreateAsset(asset, path);
        ModifyAsset(asset, type);
        AssetDatabase.SaveAssets();

        return asset;
    }

    public static void ModifyAsset(EffectData data, Type type)
    {
        data.id = type.Name;
        data.label = type.Name;

        if (type.IsSubclassOf(typeof(CharacterEffect)))
        {
            data.tags.Add("Character");
        }
        if (type.IsSubclassOf(typeof(WeaponEffect)))
        {
            data.tags.Add("Weapon");
        }
    }
}
#endif
