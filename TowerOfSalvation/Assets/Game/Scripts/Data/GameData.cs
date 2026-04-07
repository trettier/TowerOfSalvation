using System.Collections.Generic;
using UnityEngine;

public class GameData
{
    public static int GetCurrentDay()
    {
        return PlayerPrefs.GetInt("CurrentDay", 0);
    }

    [HideInInspector]
    public List<ZoneData> charactersZones = new List<ZoneData>();

    public UnityEngine.Resources resources = new UnityEngine.Resources();

    public static class PlayerResources
    {
        private static string GetKey(ResourceType type)
        {
            return $"resource_{type}";
        }

        public static int Get(ResourceType type)
        {
            return PlayerPrefs.GetInt(GetKey(type), 0);
        }

        public static void Set(ResourceType type, int value)
        {
            PlayerPrefs.SetInt(GetKey(type), value);
        }
    }
}