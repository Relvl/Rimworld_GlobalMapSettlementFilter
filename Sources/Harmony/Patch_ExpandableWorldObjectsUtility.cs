using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using RimWorld.Planet;

namespace GlobalMapSettlementFilter.Harmony;

[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
[SuppressMessage("ReSharper", "UnusedMember.Global")]
[HarmonyPatch(typeof(ExpandableWorldObjectsUtility))]
public static class Patch_ExpandableWorldObjectsUtility
{
    /// <summary>
    ///     Allows to hide some settlements on a world map from render
    /// </summary>
    [HarmonyPrefix]
    [HarmonyPatch("SortByExpandingIconPriority")]
    public static void SortByExpandingIconPriority(List<WorldObject> worldObjects)
    {
        foreach (var worldObject in worldObjects.ToList())
        {
            if (worldObject is not Settlement settlement) continue;
            if (!GlobalMapSettlementFilter.ShouldDrawSettlement(settlement))
                worldObjects.Remove(worldObject);
        }
    }
}