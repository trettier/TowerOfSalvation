using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ZonesSettings", menuName = "Settings/Zones Settings")]
public class ZonesSettings : ScriptableObject
{
    public List<ZoneData> zones;
}