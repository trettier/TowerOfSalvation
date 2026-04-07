using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[CreateAssetMenu(fileName = "SidesSettings", menuName = "Game/Sides Settings")]
public class SidesSettings : ScriptableObject
{
    public List<SideSettings> sideSettings;

    public bool IsEnemy(Side mySide, Side targetSide)
    {
        SideSettings settings = sideSettings.FirstOrDefault(side => side.side.name == mySide.name);
        if (settings == null) return false;

        // Сравниваем по имени, а не по ссылке
        return settings.enemies.Any(enemy => enemy.name == targetSide.name);
    }
}

[Serializable]
public class SideSettings
{
    public Side side;
    public List<Side> enemies;
}

[Serializable]
public class Side
{
    public string name;
    public Side(string name)
    {
        this.name = name; 
    }
}